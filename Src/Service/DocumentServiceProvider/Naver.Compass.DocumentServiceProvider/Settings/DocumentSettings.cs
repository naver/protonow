using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class DocumentSettings : XmlElementObject, IDocumentSettings
    {
        internal DocumentSettings(Document document)
            : base("DocumentSettings")
        {
            _layout = new LayoutSetting(document);
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            XmlElement layoutElement = element["LayoutSetting"];
            if (layoutElement != null)
            {
                _layout.LoadDataFromXml(layoutElement);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement settingsElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(settingsElement);

            _layout.SaveDataToXml(xmlDoc, settingsElement);
        }

        #endregion

        public ILayoutSetting LayoutSetting
        {
            get
            {
                return _layout;
            }
        }

        internal void Clear()
        {
            _layout.Clear();
        }

        private LayoutSetting _layout;
    }
}
