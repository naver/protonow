using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Naver.Compass.Service.Document
{
    internal class CustomObjectPageData : PageData
    {
        internal CustomObjectPageData(CustomObjectPage page, string tagName)
            : base(page, tagName)
        {

        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            LoadBoolFromChildElementInnerText("UseThumbnailAsIcon", element, ref _useThumbnailAsIcon);
            LoadStringFromChildElementInnerText("Tooltip", element, ref _tooltip);
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

            SaveStringToChildElement("UseThumbnailAsIcon", _useThumbnailAsIcon.ToString(), xmlDoc, pageElement);
            SaveStringToChildElement("Tooltip", _tooltip, xmlDoc, pageElement);
        }

        #endregion

        internal bool UseThumbnailAsIcon
        {
            get { return _useThumbnailAsIcon; }
            set { _useThumbnailAsIcon = value; }
        }

        internal string Tooltip
        {
            get { return _tooltip; }
            set { _tooltip = value; }
        }

        protected bool _useThumbnailAsIcon = true;
        protected string _tooltip;
    }
}
