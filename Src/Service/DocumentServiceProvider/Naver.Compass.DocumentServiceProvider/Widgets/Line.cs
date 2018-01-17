using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class Line : Widget, ILine
    {
        internal Line(Page parentPage)
            : base(parentPage, "Line")
        {
            _widgetType = WidgetType.Line;

            _orientation = Orientation.Horizontal;

            InitializeBaseViewStyleFromDefaultStyle();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            LoadEnumFromChildElementInnerText<Orientation>("Orientation", element, ref _orientation);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement lineElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(lineElement);

            SaveStringToChildElement("Orientation", _orientation.ToString(), xmlDoc, lineElement);

            base.SaveDataToXml(xmlDoc, lineElement);
        }

        #endregion

        public Orientation Orientation 
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        internal static bool SupportedStyleProperty(string stylePropertyName)
        {
            return _supportStyleProperties.ContainsKey(stylePropertyName);
        }

        protected override Dictionary<string, StyleProperty> SupportStyleProperties
        {
            get { return _supportStyleProperties; }
        }

        private static Dictionary<string, StyleProperty> _supportStyleProperties = new Dictionary<string, StyleProperty>();

        static Line()
        {
            // Add support style properties and default value.
            _supportStyleProperties[StylePropertyNames.WIDGET_ROTATE_PROP] = new StyleDoubleProperty(StylePropertyNames.WIDGET_ROTATE_PROP, 0);
            _supportStyleProperties[StylePropertyNames.LINE_COLOR_PROP] = new StyleColorProperty(StylePropertyNames.LINE_COLOR_PROP, new StyleColor(ColorFillType.Solid, -16777216));  // -16777216 is 0xff000000 (Black)
            _supportStyleProperties[StylePropertyNames.LINE_WIDTH_PROP] = new StyleDoubleProperty(StylePropertyNames.LINE_WIDTH_PROP, 1);
            _supportStyleProperties[StylePropertyNames.LINE_STYLE_PROP] = new StyleEnumProperty<LineStyle>(StylePropertyNames.LINE_STYLE_PROP, LineStyle.Solid);
            _supportStyleProperties[StylePropertyNames.ARROW_STYLE_PROP] = new StyleEnumProperty<ArrowStyle>(StylePropertyNames.ARROW_STYLE_PROP, ArrowStyle.None);
            _supportStyleProperties[StylePropertyNames.OPACITY_PROP] = new StyleIntegerProperty(StylePropertyNames.OPACITY_PROP, 100);
        }

        private Orientation _orientation;
    }
}

