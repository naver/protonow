using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class Svg : StreamWidget, ISvg
    {
        internal Svg(Page parentPage)
            : base(parentPage, "Svg")
        {
            _widgetType = WidgetType.SVG;

            InitializeBaseViewStyleFromDefaultStyle();
        }

        #region XmlElementObject         

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement svgElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(svgElement);

            base.SaveDataToXml(xmlDoc, svgElement);
        }

        #endregion

        public override string StreamType
        {
            get { return @"svg"; }
            set { }
        }

        public Stream XmlStream
        {
            get { return DataStream; }

            set { DataStream = value; }
        }

        internal static bool SupportedStyleProperty(string stylePropertyName)
        {
            return _supportStyleProperties.ContainsKey(stylePropertyName);
        }
        
        protected override IHashStreamManager HashStreamManager
        {
            get
            {
                return ParentDocumentObject != null ? ParentDocumentObject.ImagesStreamManager : null;
            }
        }

        protected override Dictionary<string, StyleProperty> SupportStyleProperties
        {
            get { return _supportStyleProperties; }
        }

        private static Dictionary<string, StyleProperty> _supportStyleProperties = new Dictionary<string, StyleProperty>();

        static Svg()
        {
            // Add support style properties and default value.
            _supportStyleProperties[StylePropertyNames.WIDGET_ROTATE_PROP] = new StyleDoubleProperty(StylePropertyNames.WIDGET_ROTATE_PROP, 0);
            _supportStyleProperties[StylePropertyNames.OPACITY_PROP] = new StyleDoubleProperty(StylePropertyNames.OPACITY_PROP, 100);
        }
    }
}
