using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal abstract class GeneratorConfiguration : XmlElementObject, IGeneratorConfiguration
    {
        internal GeneratorConfiguration(GeneratorConfigurationSet set, string tagName, string name)
            : base(tagName)
        {
            _set = set;
            _name = name;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadStringFromChildElementInnerText("Name", element, ref _name);
            LoadEnumFromChildElementInnerText<GeneratorType>("GeneratorType", element, ref _type);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            SaveStringToChildElement("Name", _name, xmlDoc, parentElement);
            SaveStringToChildElement("GeneratorType", _type.ToString(), xmlDoc, parentElement);
        }

        #endregion

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public GeneratorType GeneratorType
        {
            get { return _type; }
        }

        public IGeneratorConfigurationSet GeneratorConfigurationSet
        {
            get { return _set; }
        }

        protected GeneratorConfigurationSet _set; 
        protected string _name;
        protected GeneratorType _type;
    }
}
