using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class HotSpot : Widget, IHotSpot
    {
        internal HotSpot(Page parentPage)
            : base(parentPage, "HotSpot")
        {
            _widgetType = WidgetType.HotSpot;

            InitializeBaseViewStyleFromDefaultStyle();
        }

        #region XmlElementObject

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            base.SaveDataToXml(xmlDoc, element);
        }

        #endregion

        internal static bool SupportedStyleProperty(string stylePropertyName)
        {
            return _supportStyleProperties.ContainsKey(stylePropertyName);
        }

        protected override Dictionary<string, StyleProperty> SupportStyleProperties
        {
            get { return _supportStyleProperties; }
        }

        private static Dictionary<string, StyleProperty> _supportStyleProperties = new Dictionary<string, StyleProperty>();

        static HotSpot()
        {
            // Add support style properties and default value.
            _supportStyleProperties[StylePropertyNames.WIDGET_ROTATE_PROP] = new StyleDoubleProperty(StylePropertyNames.WIDGET_ROTATE_PROP, 0);
        }
    }
}
