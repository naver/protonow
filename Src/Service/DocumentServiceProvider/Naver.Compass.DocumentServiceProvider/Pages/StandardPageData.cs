using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class StandardPageData : PageData
    {
        internal StandardPageData(StandardPage page, string tagName)
            : base(page, tagName)
        {
            _events = new InteractionEvents(page);
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            // Load Events
            XmlElement eventsElement = element["Events"];
            if (eventsElement != null)
            {
                _events.LoadDataFromXml(eventsElement);
            }
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

            _events.SaveDataToXml(xmlDoc, pageElement);
        }

        #endregion

        internal InteractionEvents Events
        {
            get { return _events; }
        }

        private InteractionEvents _events;
    }

}
