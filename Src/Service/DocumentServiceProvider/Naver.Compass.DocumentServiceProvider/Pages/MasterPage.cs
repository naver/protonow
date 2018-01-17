using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class MasterPage : DocumentPage, IMasterPage
    {
        internal MasterPage(Document document, string pageName)
            : base("MasterPage", document)
        {
            _pageData = new PageData(this, "MasterPage");
            _pageData.Name = pageName;

            InitializeBasePageView();
        }

        #region XmlDataFileObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            LoadElementBoolAttribute(element, "IsLockedToMasterLocation",  ref _isLockedToMasterLocation);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement pageElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(pageElement);

            SaveElementGuidAttribute(pageElement, PageData.Guid);
            SaveElementBoolAttribute(pageElement, "IsLockedToMasterLocation", _isLockedToMasterLocation);
            SaveStringToChildElement("Name", PageData.Name, xmlDoc, pageElement);
        }

        #endregion

        public override void Close()
        {
            lock (this)
            {
                if (_activeConsumerPageGuidList.Count > 0)
                {
                    // If there is a active consumer, we don't close master pages.
                    return;
                }

                base.Close();
            }
        }

        public override void AddMaster(IMaster master)
        {
            throw new CannotAddMasterException("Cannot add Master to an MasterPage.");
        }

        public ReadOnlyCollection<Guid> ActiveConsumerPageGuidList
        {
            get
            {
                return new ReadOnlyCollection<Guid>(_activeConsumerPageGuidList);
            }
        }

        public ReadOnlyCollection<Guid> AllConsumerPageGuidList
        {
            get
            {
                Document document = ParentDocument as Document;
                if (document == null)
                {
                    throw new DocumentIsClosedException("Failed to get document, the document is null.");
                }

                List<Guid> allConsumerPageGuidList = new List<Guid>();

                foreach(Page page in document.Pages)
                {
                    bool isClosedPage = false;
                    if(page.IsOpened == false)
                    {
                        isClosedPage = true;
                        page.Open();
                    }

                    if(page.Masters.Any<IMaster>(x => x.MasterPageGuid == Guid))
                    {
                        allConsumerPageGuidList.Add(page.Guid);
                    }

                    if (isClosedPage)
                    {
                        page.Close();
                    }
                }

                return new ReadOnlyCollection<Guid>(allConsumerPageGuidList);
            }
        }

        public bool IsLockedToMasterLocation
        {
            get { return _isLockedToMasterLocation; }
            set { _isLockedToMasterLocation = value; }
        }

        internal override void OnDeleteFromDocument()
        {
            lock (this)
            {
                // Here, the parent document must exist.
                Document document = ParentDocument as Document;
                if (document == null)
                {
                    throw new CannotDeletePageException("Failed to delete from document, the document is null.");
                }

                // Delete all related masters form all pages.
                foreach (Guid pageGuid in _activeConsumerPageGuidList)
                {
                    if (document.AllPages.ContainsKey(pageGuid))
                    {
                        Page page = document.AllPages[pageGuid];
                        if (page != null && page.IsOpened)
                        {
                            List<IMaster> deleteMasterList = page.Masters.Where<IMaster>(x => x.MasterPageGuid == Guid).ToList<IMaster>();
                            foreach (IMaster master in deleteMasterList)
                            {
                                page.DeleteMasterInternal(master.Guid, true);
                            }
                        }
                    }
                }

                // Keep the active page information.
                // ClearAllActiveConsumerPages();

                base.OnDeleteFromDocument();
            }
        }

        internal override PageData PageData
        {
            get { return _pageData; }
        }

        internal void AddActiveConsumerPage(Guid pageGuid)
        {
            if (!_activeConsumerPageGuidList.Contains(pageGuid))
            {
                _activeConsumerPageGuidList.Add(pageGuid);
            }
        }

        internal void RemoveActiveConsumerPage(Guid pageGuid)
        {
            _activeConsumerPageGuidList.Remove(pageGuid);
        }

        internal void ClearAllActiveConsumerPages()
        {
            _activeConsumerPageGuidList.Clear();
        }

        private PageData _pageData;

        // Active consumer page is the opened page which contains masters created by this master page.
        private List<Guid> _activeConsumerPageGuidList = new List<Guid>();

        // TODO: This flag is not saved in page data xml file, so if we want to copy this flag via serializer,
        // we need to consider moving this flag to page data.
        private bool _isLockedToMasterLocation;
    }
}
