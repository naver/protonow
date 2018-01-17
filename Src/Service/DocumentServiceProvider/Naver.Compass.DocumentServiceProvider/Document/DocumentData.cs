using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class DocumentData : XmlDocumentObject
    {
        internal DocumentData(Document document, DocumentType type)
            : base("Document")
        {
            Debug.Assert(document != null);

            _documentType = type;

            _pages = new Pages(document);

            _masterPages = new MasterPages(document);

            _pageAnnotationFieldSet = new AnnotationFieldSet(document, "PageAnnotationFieldSet");

            _widgetAnnotationFieldSet = new AnnotationFieldSet(document, "WidgetAnnotationFieldSet");

            _widgetDefaultStyleSet = new WidgetDefaultStyleSet(document);

            _generatorConfigurationSet = new GeneratorConfigurationSet(document);

            _adaptiveViewSet = new AdaptiveViewSet(document);

            _deviceSet = new DeviceSet(document);

            _documentSettings = new DocumentSettings(document);

            _globalGuides = new Guides();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            LoadElementGuidAttribute(element, ref _guid);
            LoadElementStringAttribute(element, "TimeStamp", ref _timeStamp);

            if (!LoadEnumFromChildElementInnerText<DocumentType>("DocumentType", element, ref _documentType))
            {
                throw new XmlException("DocumentType element is invalid!");
            }

            // Load PageAnnotationFieldSet
            XmlElement pageAnnotationFieldSetElement = element[_pageAnnotationFieldSet.TagName];
            if (pageAnnotationFieldSetElement != null)
            {
                _pageAnnotationFieldSet.LoadDataFromXml(pageAnnotationFieldSetElement);
            }

            // Load WidgetAnnotationFieldSet
            XmlElement widgetAnnotationFieldSetElement = element[_widgetAnnotationFieldSet.TagName];
            if (widgetAnnotationFieldSetElement != null)
            {
                _widgetAnnotationFieldSet.LoadDataFromXml(widgetAnnotationFieldSetElement);
            }

            // WidgetDefaultStyleSet
            XmlElement widgetStlyeSetElement = element[_widgetDefaultStyleSet.TagName];
            if (widgetStlyeSetElement != null)
            {
                _widgetDefaultStyleSet.LoadDataFromXml(widgetStlyeSetElement);
            }

            // Load GeneratorConfigurationSet
            XmlElement generatorSetElement = element[_generatorConfigurationSet.TagName];
            if (generatorSetElement != null)
            {
                _generatorConfigurationSet.LoadDataFromXml(generatorSetElement);
            }

            // Load AdaptiveViewSet
            XmlElement adaptiveViewSetElement = element[_adaptiveViewSet.TagName];
            if (adaptiveViewSetElement != null)
            {
                _adaptiveViewSet.LoadDataFromXml(adaptiveViewSetElement);
            }

            // Load DeviceSet
            XmlElement deviceSetElement = element[_deviceSet.TagName];
            if (deviceSetElement != null)
            {
                _deviceSet.LoadDataFromXml(deviceSetElement);
            }

            // Load Guides
            XmlElement guidesElement = element[_globalGuides.TagName];
            if (guidesElement != null)
            {
                _globalGuides.LoadDataFromXml(guidesElement);
            }

            // Load Master Pages
            XmlElement masterPagesElement = element[_masterPages.TagName];
            if (masterPagesElement != null)
            {
                _masterPages.LoadDataFromXml(masterPagesElement);
            }

            // Load Pages
            XmlElement pagesElement = element[_pages.TagName];
            if (pagesElement != null)
            {
                _pages.LoadDataFromXml(pagesElement);
            }

            // Load DocumentSettings
            XmlElement settingsElement = element[_documentSettings.TagName];
            if (settingsElement != null)
            {
                _documentSettings.LoadDataFromXml(settingsElement);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement documentElement = xmlDoc.CreateElement(TagName);
            xmlDoc.AppendChild(documentElement);

            SaveElementGuidAttribute(documentElement, _guid);
            SaveElementStringAttribute(documentElement,"TimeStamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            //SaveElementStringAttribute(documentElement,"TimeStamp", DateTime.Now.ToString());

            SaveStringToChildElement("FileVersion", VersionData.THIS_FILE_VERSION, xmlDoc, documentElement);
            SaveStringToChildElement("DocumentType", _documentType.ToString(), xmlDoc, documentElement);

            // Save PageAnnotationFieldSet
            _pageAnnotationFieldSet.SaveDataToXml(xmlDoc, documentElement);

            // Save WidgetAnnotationFieldSet
            _widgetAnnotationFieldSet.SaveDataToXml(xmlDoc, documentElement);

            // Save WidgetDefaultStyleSet
            _widgetDefaultStyleSet.SaveDataToXml(xmlDoc, documentElement);

            // Save GeneratorConfigurationSet
            _generatorConfigurationSet.SaveDataToXml(xmlDoc, documentElement);

            // Save AdaptiveViewSet
            _adaptiveViewSet.SaveDataToXml(xmlDoc, documentElement);

            // Save Device Set
            _deviceSet.SaveDataToXml(xmlDoc, documentElement);

            // Save DocumentSettings
            _documentSettings.SaveDataToXml(xmlDoc, documentElement);

            // Save Guides
            _globalGuides.SaveDataToXml(xmlDoc, documentElement);

            // Save Master Pages
            _masterPages.SaveDataToXml(xmlDoc, documentElement);

            // Save Pages
            _pages.SaveDataToXml(xmlDoc, documentElement);
        }

        #endregion

        #region Internal Methods

        internal Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        internal DocumentType DocumentType
        {
            get { return _documentType; }
            set { _documentType = value; }
        }

        internal Pages Pages
        {
            get { return _pages; }
        }

        internal MasterPages MasterPages
        {
            get { return _masterPages; }
        }

        internal AnnotationFieldSet PageAnnotationFieldSet
        {
            get { return _pageAnnotationFieldSet; }
        }

        internal AnnotationFieldSet WidgetAnnotationFieldSet
        {
            get { return _widgetAnnotationFieldSet; }
        }

        internal WidgetDefaultStyleSet WidgetDefaultStyleSet
        {
            get { return _widgetDefaultStyleSet; }
        }

        internal GeneratorConfigurationSet GeneratorConfigurationSet
        {
            get { return _generatorConfigurationSet; }
        }

        internal AdaptiveViewSet AdaptiveViewSet
        {
            get { return _adaptiveViewSet; }
        }

        internal DeviceSet DeviceSet
        {
            get { return _deviceSet; }
        }

        internal DocumentSettings DocumentSettings
        {
            get { return _documentSettings; }
        }

        internal Guides GlobalGuides
        {
            get { return _globalGuides; }
        }

        internal string TimeStamp
        {
            get { return _timeStamp; }
        }
        private void Clear()
        {
            _guid = Guid.Empty;
            _documentType = DocumentType.Standard;
            _pages.Clear();
            _masterPages.Clear();
            _pageAnnotationFieldSet.Clear();
            _widgetAnnotationFieldSet.Clear();
            _widgetDefaultStyleSet.Clear();
            _generatorConfigurationSet.Clear();
            _adaptiveViewSet.Clear();
            _deviceSet.Clear();
            _documentSettings.Clear();
            _globalGuides.Clear();
        }

        #endregion

        #region Private Fields

        private Guid _guid = Guid.NewGuid();
        
        private DocumentType _documentType;

        private Pages _pages;

        private MasterPages _masterPages;

        private AnnotationFieldSet _pageAnnotationFieldSet;

        private AnnotationFieldSet _widgetAnnotationFieldSet;

        private WidgetDefaultStyleSet _widgetDefaultStyleSet;

        private GeneratorConfigurationSet _generatorConfigurationSet;

        private AdaptiveViewSet _adaptiveViewSet;

        private DeviceSet _deviceSet;
                
        private DocumentSettings _documentSettings;

        private Guides _globalGuides;

        private string _timeStamp = "2000-01-01 00:00:00";

        #endregion
    }
}
