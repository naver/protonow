using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class Viewport : XmlElementObject, IViewport
    {
        public Viewport()
            : base("Viewport")
        {
        }

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadBoolFromChildElementInnerText("IncludeViewportTag", element, ref _includeViewportTag);
            LoadStringFromChildElementInnerText("Name", element, ref _name);
            LoadIntFromChildElementInnerText("Width", element, ref _width);
            LoadIntFromChildElementInnerText("Height", element, ref _height);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            SaveStringToChildElement("IncludeViewportTag", _includeViewportTag.ToString(), xmlDoc, element);
            SaveStringToChildElement("Name", Name, xmlDoc, element);
            SaveStringToChildElement("Width", Width.ToString(), xmlDoc, element);
            SaveStringToChildElement("Height", Height.ToString(), xmlDoc, element);
        }

        public bool IncludeViewportTag
        {
            get { return _includeViewportTag; }
            set { _includeViewportTag = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        private bool _includeViewportTag;
        private string _name;
        private int _width;
        private int _height;
    }

    internal class HtmlGeneratorConfiguration : GeneratorConfiguration, IHtmlGeneratorConfiguration
    {
        internal HtmlGeneratorConfiguration(GeneratorConfigurationSet set, string name)
            : base(set, "HtmlGeneratorConfiguration", name)
        {
            _type = GeneratorType.Html;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            LoadStringFromChildElementInnerText("OutputFolder", element, ref _outputFolder);
            
            XmlElement viewportElement = element[_viewport.TagName];
            if (viewportElement != null)
            {
                _viewport.LoadDataFromXml(viewportElement);
            }

            if (LoadEnumFromChildElementInnerText<ExportType>("ExportType", element, ref _exportType) == false)
            {
                // If there is no ExportType, set to export image file.
                _exportType = ExportType.ImageFile;
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            base.SaveDataToXml(xmlDoc, element);

            SaveStringToChildElement("OutputFolder", _outputFolder, xmlDoc, element);
            
            _viewport.SaveDataToXml(xmlDoc, element);

            SaveStringToChildElement("ExportType", _exportType.ToString(), xmlDoc, element);
        }

        #endregion

        public string OutputFolder 
        {
            get { return _outputFolder; }
            set { _outputFolder = value; } 
        }
        
        public IViewport Viewport
        {
            get { return _viewport; }
        }

        public ExportType ExportType
        {
            get { return _exportType; }
            set { _exportType = value; }
        }

        public bool GenerateMobileFiles 
        {
            get { return _generateMobileFiles; }
            set { _generateMobileFiles = value; }
        }

        public Guid StartPage
        {
            get { return _startPage; }
            set { _startPage = value; }
        }

        public Guid CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }

        private string _outputFolder;
        private Viewport _viewport = new Viewport();
        private ExportType _exportType = ExportType.Data;

        private bool _generateMobileFiles = true;
        private Guid _startPage = Guid.Empty;
        private Guid _currentPage = Guid.Empty;
    }
}
