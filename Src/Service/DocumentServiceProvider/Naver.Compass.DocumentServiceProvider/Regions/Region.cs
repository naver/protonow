using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    internal abstract class Region : XmlElementObject, IRegion, ISerializableObject
    {
        public Region(Page parentPage, string tagName)
            : base(tagName)
        {
            _parentPage = parentPage;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadElementGuidAttribute(element, ref _guid);
            LoadStringFromChildElementInnerText("Name", element, ref _name);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            CheckTagName(parentElement);

            SaveElementGuidAttribute(parentElement, _guid);
            SaveStringToChildElement("Name", _name, xmlDoc, parentElement);
        }

        #endregion

        public virtual Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public IDocument ParentDocument
        {
            get
            {
                return ParentPage != null ? ParentPage.ParentDocument : null;
            }
        }

        public virtual IPage ParentPage
        {
            get { return _parentPage; }
            set { _parentPage = value as Page; }
        }

        public abstract IRegions GetChildRegions(Guid viewGuid);

        public abstract IRegionStyle RegionStyle { get; }

        public abstract IRegionStyle GetRegionStyle(Guid viewGuid);

        public Guid OriginalGuid
        {
            get;
            set;
        }

        public void UpdateGuid()
        {
            OriginalGuid = Guid;
            Guid = Guid.NewGuid();
        }

        protected Document ParentDocumentObject
        {
            get
            {
                return ParentPage != null ? ParentPage.ParentDocument as Document : null;
            }
        }

        private Guid _guid = Guid.NewGuid();
        private string _name;
        private Page _parentPage;
    }
}
