using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class GeneratorConfigurationSet : XmlElementObject, IGeneratorConfigurationSet
    {
        internal GeneratorConfigurationSet(Document document)
            : base("GeneratorConfigurationSet")
        {
            _document = document;

            _configs = new GeneratorConfigurations(this);

            InitializeDefaultGeneratorConfiguration();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            XmlElement configsElement = element[_configs.TagName];
            if (configsElement != null)
            {
                _configs.LoadDataFromXml(configsElement);
            }

            InitializeDefaultGeneratorConfiguration();
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement setElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(setElement);

            _configs.SaveDataToXml(xmlDoc, setElement);
        }

        #endregion

        public IDocument ParentDocument
        {
            get { return _document; }
        }

        public IHtmlGeneratorConfiguration DefaultHtmlConfiguration
        {
            get { return _configs[DEFAULT_HTML_CONFIGURATION_NAME] as HtmlGeneratorConfiguration; }
        }

        internal IGeneratorConfigurations Configurations
        {
            get { return _configs; }
        }

        internal IGeneratorConfiguration CreateConfiguration(string configName, GeneratorType type)
        {
            if (_configs.Contains(configName))
            {
                throw new ArgumentException(configName + " already exists!");
            }

            switch (type)
            {
                case GeneratorType.Html:
                    {
                        HtmlGeneratorConfiguration config = new HtmlGeneratorConfiguration(this, configName);
                        _configs.Add(config.Name, config);
                        return config;
                    }
                case GeneratorType.Word:
                    {
                        return null;
                    }
                case GeneratorType.Csv:
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }

        internal void DeleteConfiguration(string configName)
        {
            _configs.Remove(configName);
        }

        internal void Clear()
        {
            _configs.Clear();
        }

        #region Private Method

        private void InitializeDefaultGeneratorConfiguration()
        {
            if (!_configs.Contains(DEFAULT_HTML_CONFIGURATION_NAME))
            {
                HtmlGeneratorConfiguration defaultHtml = new HtmlGeneratorConfiguration(this, DEFAULT_HTML_CONFIGURATION_NAME);
                _configs.Add(defaultHtml.Name, defaultHtml);
            }
        }

        #endregion

        #region Private Fields

        private Document _document;
        private GeneratorConfigurations _configs;
        
        #endregion

        #region Constants

        public const string DEFAULT_HTML_CONFIGURATION_NAME = "HTML";
        public const string DEFAULT_PDF_CONFIGURATION_NAME = "PDF";
        public const string DEFAULT_WORD_CONFIGURATION_NAME = "WORD";
        public const string DEFAULT_CSV_CONFIGURATION_NAME = "CSV";

        #endregion
    }
}
