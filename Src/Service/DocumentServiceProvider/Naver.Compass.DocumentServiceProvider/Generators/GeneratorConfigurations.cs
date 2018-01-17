using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class GeneratorConfigurations : XmlElementDictionary<string, GeneratorConfiguration>, IGeneratorConfigurations
    {
        internal GeneratorConfigurations(GeneratorConfigurationSet set)
            : base("GeneratorConfigurations")
        {
            _set = set;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            XmlNodeList childList = element.ChildNodes;
            if (childList == null || childList.Count <= 0)
            {
                return;
            }

            foreach (XmlElement childElement in childList)
            {
                if (childElement.Name == "HtmlGeneratorConfiguration")
                {
                    HtmlGeneratorConfiguration config = new HtmlGeneratorConfiguration(_set, "");
                    config.LoadDataFromXml(childElement);
                    Add(config.Name, config);
                }
            }
        }

        #endregion

        public IGeneratorConfiguration GetGeneratorConfiguration(string configName)
        {
            return Get(configName);
        }

        public new IGeneratorConfiguration this[string configName] 
        {
            get { return GetGeneratorConfiguration(configName); }
        }

        public IGeneratorConfigurationSet GeneratorConfigurationSet
        {
            get { return _set; }
        }

        private GeneratorConfigurationSet _set; 
    }
}
