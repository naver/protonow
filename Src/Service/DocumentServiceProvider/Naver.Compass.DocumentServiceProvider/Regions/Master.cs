using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class Master : Region, IMaster
    {
        internal Master()
            : this(null, null)
        {
        }

        internal Master(MasterPage masterPage, Page parentPage)
            : base(parentPage, "Master")
        {
            Guid baseViewGuid = Guid.Empty;
            if(ParentDocument != null && ParentDocument.IsOpened)
            {
                baseViewGuid = ParentDocument.AdaptiveViewSet.Base.Guid;
            }
            _masterStyle = new MasterStyle(this, baseViewGuid);

            _masterStyles = new MasterStyles(this);
            InitializeAdaptiveViewStyles();

            _masterPage = masterPage;

            if (masterPage != null)
            {
                _masterPageGuid = _masterPage.Guid;
            }
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            LoadGuidFromChildElementInnerText("MasterPageGuid", element, ref _masterPageGuid);
            LoadGuidFromChildElementInnerText("ParentGroupGuid", element, ref _parentGroupGuid);
            LoadBoolFromChildElementInnerText("IsLocked", element, ref _isLocked);
            LoadBoolFromChildElementInnerText("IsLockedToMasterLocation", element, ref _isLockedToMasterLocation);
            LoadGuidFromChildElementInnerText("CreatedViewGuid", element, ref _createdViewGuid);
            LoadBoolFromChildElementInnerText("HasBeenPlacedInBaseView", element, ref _hasBeenPlacedInBaseView);

            XmlElement styleElement = element[_masterStyle.TagName];
            if (styleElement != null)
            {
                _masterStyle.LoadDataFromXml(styleElement);
            }

            XmlElement stylesElement = element[_masterStyles.TagName];
            if (stylesElement != null)
            {
                _masterStyles.LoadDataFromXml(stylesElement);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement masterElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(masterElement);

            base.SaveDataToXml(xmlDoc, masterElement);

            SaveStringToChildElement("MasterPageGuid", _masterPageGuid.ToString(), xmlDoc, masterElement);

            if (_parentGroupGuid != Guid.Empty)
            {
                SaveStringToChildElement("ParentGroupGuid", _parentGroupGuid.ToString(), xmlDoc, masterElement);
            }

            SaveStringToChildElement("IsLocked", _isLocked.ToString(), xmlDoc, masterElement);
            SaveStringToChildElement("IsLockedToMasterLocation", _isLockedToMasterLocation.ToString(), xmlDoc, masterElement);
            SaveStringToChildElement("CreatedViewGuid", _createdViewGuid.ToString(), xmlDoc, masterElement);
            SaveStringToChildElement("HasBeenPlacedInBaseView", _hasBeenPlacedInBaseView.ToString(), xmlDoc, masterElement);

            _masterStyle.SaveDataToXml(xmlDoc, masterElement);

            _masterStyles.SaveDataToXml(xmlDoc, masterElement);
        }

        #endregion

        #region IRegion

        public override IRegions GetChildRegions(Guid viewGuid)
        {
            if(MasterPage != null)
            {
                IPageView pageView = MasterPage.PageViews[viewGuid];
                if(pageView != null)
                {
                    return pageView.GetChildRegions(viewGuid);
                }
            }

            return new Regions();
        }

        public override IRegionStyle RegionStyle
        {
            get { return MasterStyle; }
        }

        public override IRegionStyle GetRegionStyle(Guid viewGuid)
        {
            return GetMasterStyle(viewGuid);
        }

        #endregion

        #region IMaster
        public string MD5 { get; set; }

        public IAnnotation Annotation
        {
            get 
            {
                // Get master page note as the annotation of the master. Because UX team require all masters created with the 
                // same master page should have same annotation and changes in a master will affect all masters.
                return MasterPage != null ? MasterPage.Annotation : null;
            }
        }

        public IMasterPage MasterPage
        {
            get
            {
                if (_masterPage == null && ParentDocument != null && ParentDocument.IsOpened)
                {
                    _masterPage = ParentDocument.MasterPages[_masterPageGuid] as MasterPage;
                }

                return _masterPage;
            }
            set
            {
                _masterPage = value as MasterPage;
                if (_masterPage != null)
                {
                    _masterPageGuid = _masterPage.Guid;
                }
                else
                {
                    _masterPageGuid = Guid.Empty;
                }
            }
        }

        public Guid MasterPageGuid
        {
            get { return _masterPageGuid; }
            set 
            {
                _masterPageGuid = value;
                _masterPage = null;
            }
        }

        public IGroup ParentGroup
        {
            get
            {
                if (_parentGroup == null && ParentPage != null)
                {
                    // If object is null, try to get it in page with its guid.
                    _parentGroup = ParentPage.Groups[_parentGroupGuid] as Group;
                }

                return _parentGroup;
            }

            set
            {
                _parentGroup = value as Group;
                if (_parentGroup != null)
                {
                    _parentGroupGuid = _parentGroup.Guid;
                }
                else
                {
                    _parentGroupGuid = Guid.Empty;
                }
            }
        }

        public Guid ParentGroupGuid
        {
            get { return _parentGroupGuid; }
            set
            {
                _parentGroupGuid = value;
                _parentGroup = null;
            }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }

        public bool IsLockedToMasterLocation
        {
            get { return _isLockedToMasterLocation; }
            set { _isLockedToMasterLocation = value; }
        }


        public IMasterStyle MasterStyle
        {
            get { return _masterStyle; }
        }

        public IMasterStyle GetMasterStyle(Guid viewGuid)
        {
            if (viewGuid == Guid.Empty)
            {
                return null;
            }

            // If it is base view.
            if (ParentDocument != null && ParentDocument.IsOpened
                && viewGuid == ParentDocument.AdaptiveViewSet.Base.Guid)
            {
                return _masterStyle;
            }
            else
            {
                // If view style already exists, just return it.
                if (_masterStyles.Contains(viewGuid))
                {
                    return _masterStyles[viewGuid];
                }
                else if (ParentDocument != null && ParentDocument.IsOpened)
                {
                    // Add view style if the adaptive view is in document adaptive view set.
                    IAdaptiveView view = ParentDocument.AdaptiveViewSet.AdaptiveViews[viewGuid];
                    if (view != null)
                    {
                        return AddMasterStyle(view as AdaptiveView);
                    }
                }

                return null;
            }
        }

        #endregion

        #region Events

        internal void OnAddToDocument()
        {
            // Here, the parent document must exist.
            Document document = ParentDocument as Document;
            if (document == null)
            {
                throw new CannotAddMasterException("Cannot add master to document, the document is null.");
            }

            MasterPage masterPage = MasterPage as MasterPage;
            Debug.Assert(masterPage != null);

            // Open the master page
            if (!masterPage.IsOpened)
            {
                masterPage.Open();
            }

            List<Guid> deleteViewGuidList = new List<Guid>();
            foreach (MasterStyle style in _masterStyles)
            {
                if (!document.AdaptiveViewSet.AdaptiveViews.Contains(style.ViewGuid))
                {
                    deleteViewGuidList.Add(style.ViewGuid);
                }
            }

            foreach (Guid guid in deleteViewGuidList)
            {
                DeleteMasterStyle(guid);
            }

            _masterStyle.ViewGuid = document.AdaptiveViewSet.Base.Guid;

            InitializeAdaptiveViewStyles();

            masterPage.AddActiveConsumerPage(ParentPage.Guid);
        }

        internal void OnDeleteFromDocument(bool isMasterPageDeleted)
        {
            if (!isMasterPageDeleted)
            {
                // Remove this master from master page consumer collection if no any other masters created with the master page.
                MasterPage masterPage = MasterPage as MasterPage;
                if (masterPage != null)
                {
                    if (!ParentPage.Masters.Any<IMaster>(x => x.MasterPageGuid == masterPage.Guid))
                    {
                        masterPage.RemoveActiveConsumerPage(ParentPage.Guid);
                    }
                }
            }
        }

        internal void OnAddAdaptiveView(AdaptiveView view)
        {
            AddMasterStyle(view);
        }

        internal void OnDeleteAdaptiveView(Guid viewGuid)
        {
            DeleteMasterStyle(viewGuid);
        }

        #endregion

        internal MasterStyle AddMasterStyle(AdaptiveView view)
        {
            MasterStyle style = new MasterStyle(this, view.Guid);
            _masterStyles.Add(style.ViewGuid, style);
            return style;
        }

        internal void DeleteMasterStyle(Guid viewGuid)
        {
            _masterStyles.Remove(viewGuid);
        }

        // Clear all view styles and make base view style has the value of the specific new base style
        internal void RebuildStyleChain(MasterStyle newBaseStyle)
        {
            // Clear all view styles
            _masterStyles.Clear();

            // Reset base view style value.
            if(newBaseStyle != null)
            {
                Style.CopyStyle(newBaseStyle, _masterStyle, null, null);
            }
        }

        internal MasterStyles MasterStyles
        {
            get { return _masterStyles; }
        }

        internal virtual Guid CreatedViewGuid
        {
            get { return _createdViewGuid; }
            set { _createdViewGuid = value; }
        }

        internal virtual bool HasBeenPlacedInBaseView
        {
            get { return _hasBeenPlacedInBaseView; }
            set { _hasBeenPlacedInBaseView = value; }
        }

        private void InitializeAdaptiveViewStyles()
        {
            // Add adaptive view style if needed.
            if (ParentDocument != null && ParentDocument.IsOpened)
            {
                foreach (AdaptiveView view in ParentDocument.AdaptiveViewSet.AdaptiveViews)
                {
                    if (!_masterStyles.Contains(view.Guid))
                    {
                        AddMasterStyle(view);
                    }
                }
            }
        }

        private MasterPage _masterPage;
        private Guid _masterPageGuid;

        private Group _parentGroup;
        private Guid _parentGroupGuid;

        private bool _isLocked;
        private bool _isLockedToMasterLocation;

        private MasterStyle _masterStyle;
        private MasterStyles _masterStyles;

        private Guid _createdViewGuid = Guid.Empty;
        private bool _hasBeenPlacedInBaseView = false;
    }
}
