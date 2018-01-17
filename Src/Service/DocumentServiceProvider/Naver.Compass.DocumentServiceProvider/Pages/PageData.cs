using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class PageData : XmlDocumentObject
    {
        internal PageData(Page page, string tagName)
            : base(tagName)
        {
            Debug.Assert(page != null);

            IsCleared = false;

            _annotation = new Annotation(page);

            _widgets = new Widgets(page);

            _masters = new Masters(page);

            _groups = new Groups(page);

            _pageViews = new PageViews(page);
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            LoadElementGuidAttribute(element, ref _guid);

            // The page name in page data may be out of time.
            XmlElement nameElement = element["Name"];
            if (string.IsNullOrEmpty(_name))
            {
                _name = nameElement.InnerText;
            }

            LoadIntFromChildElementInnerText("Zoom", element, ref _zoom);

            // Load Widgets
            XmlElement widgetsElement = element["Widgets"];
            if (widgetsElement != null)
            {
                _widgets.LoadDataFromXml(widgetsElement);
            }

            // Load Masters
            XmlElement mastersElement = element["Masters"];
            if (mastersElement != null)
            {
                _masters.LoadDataFromXml(mastersElement);
            }

            // Load Groups
            XmlElement groupsElement = element["Groups"];
            if (groupsElement != null)
            {
                _groups.LoadDataFromXml(groupsElement);
            }

            // Load PageViews
            XmlElement pageViewsElement = element["PageViews"];
            if (pageViewsElement != null)
            {
                _pageViews.LoadDataFromXml(pageViewsElement);
            }

            // Load Page Annotation
            XmlElement pageAnnotationElement = element["Annotation"];
            if (pageAnnotationElement != null)
            {
                _annotation.LoadDataFromXml(pageAnnotationElement);
            }

            IsCleared = false;
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement pageElement = xmlDoc.CreateElement(TagName);

            if (parentElement != null)
            {
                parentElement.AppendChild(pageElement);
            }
            else
            {
                xmlDoc.AppendChild(pageElement);
            }

            SaveDataToXmlInternal(xmlDoc, pageElement);
        }

        protected void SaveDataToXmlInternal(XmlDocument xmlDoc, XmlElement pageElement)
        {
            SaveElementGuidAttribute(pageElement, _guid);

            SaveStringToChildElement("FileVersion", VersionData.THIS_FILE_VERSION, xmlDoc, pageElement);

            SaveStringToChildElement("Name", _name, xmlDoc, pageElement);

            SaveStringToChildElement("Zoom", _zoom.ToString(), xmlDoc, pageElement);

            _widgets.SaveDataToXml(xmlDoc, pageElement);

            _masters.SaveDataToXml(xmlDoc, pageElement);

            _groups.SaveDataToXml(xmlDoc, pageElement);

            _pageViews.SaveDataToXml(xmlDoc, pageElement);

            _annotation.SaveDataToXml(xmlDoc, pageElement);
        }

        #endregion

        #region Internal Methods

        internal Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        internal string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        internal int Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        internal Widgets Widgets
        {
            get { return _widgets; }
        }

        internal Masters Masters
        {
            get { return _masters; }
        }

        internal Groups Groups
        {
            get { return _groups; }
        }

        internal Annotation Annotation
        {
            get { return _annotation; }
        }

        internal PageViews PageViews
        {
            get { return _pageViews; }
        }

        internal Regions WidgetsAndMasters
        {
            get
            {
                Regions regions = new Regions();

                regions.AddRange(_widgets.ToList<IRegion>());
                regions.AddRange(_masters.ToList<IRegion>());

                return regions;
            }
        }

        internal void Clear()
        {
            _widgets.Clear();
            _masters.Clear();
            _groups.Clear();
            _annotation.Clear();
            _pageViews.Clear();

            IsCleared = true;
        }

        internal bool IsCleared
        {
            get;
            set;
        }

        #endregion

        #region Protected Fields

        protected Guid _guid = Guid.NewGuid();

        protected string _name;

        protected int _zoom = 100;  // Zoom percent, default value is 100%.

        protected Widgets _widgets;

        protected Masters _masters;

        protected Groups _groups;

        protected Annotation _annotation;

        protected PageViews _pageViews;
                
        #endregion
    }
}
