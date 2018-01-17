using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class FlowShape : Widget, IFlowShape
    {
        internal FlowShape(Page parentPage, FlowShapeType flowShapeType)
            : base(parentPage, "FlowShape")
        {
            _widgetType = WidgetType.FlowShape;

            _flowShapeType = flowShapeType;

            InitializeBaseViewStyleFromDefaultStyle();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            if (!LoadEnumFromChildElementInnerText<FlowShapeType>("FlowShapeType", element, ref _flowShapeType))
            {
                throw new XmlException("FlowShapeType element is required!");
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement flowShapeElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(flowShapeElement);

            SaveStringToChildElement("FlowShapeType", _flowShapeType.ToString(), xmlDoc, flowShapeElement);

            base.SaveDataToXml(xmlDoc, flowShapeElement);
        }

        #endregion

        public FlowShapeType FlowShapeType 
        {
            get { return _flowShapeType; }
            set { _flowShapeType = value; }
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

        static FlowShape()
        {
            // Add support style properties and default value.
            _supportStyleProperties[StylePropertyNames.WIDGET_ROTATE_PROP] = new StyleDoubleProperty(StylePropertyNames.WIDGET_ROTATE_PROP, 0);
            _supportStyleProperties[StylePropertyNames.TEXT_ROTATE_PROP] = new StyleDoubleProperty(StylePropertyNames.TEXT_ROTATE_PROP, 0);
            _supportStyleProperties[StylePropertyNames.FONT_FAMILY_PROP] = new StyleProperty(StylePropertyNames.FONT_FAMILY_PROP, "Arial");
            _supportStyleProperties[StylePropertyNames.FONT_SIZE_PROP] = new StyleDoubleProperty(StylePropertyNames.FONT_SIZE_PROP, 13);
            _supportStyleProperties[StylePropertyNames.BOLD_PROP] = new StyleBooleanProperty(StylePropertyNames.BOLD_PROP, false);
            _supportStyleProperties[StylePropertyNames.ITALIC_PROP] = new StyleBooleanProperty(StylePropertyNames.ITALIC_PROP, false);
            _supportStyleProperties[StylePropertyNames.UNDERLINE_PROP] = new StyleBooleanProperty(StylePropertyNames.UNDERLINE_PROP, false);
            _supportStyleProperties[StylePropertyNames.OVERLINE_PROP] = new StyleBooleanProperty(StylePropertyNames.OVERLINE_PROP, false);
            _supportStyleProperties[StylePropertyNames.STRIKETHROUGH_PROP] = new StyleBooleanProperty(StylePropertyNames.STRIKETHROUGH_PROP, false);
            _supportStyleProperties[StylePropertyNames.FONT_COLOR_PROP] = new StyleColorProperty(StylePropertyNames.FONT_COLOR_PROP, new StyleColor(ColorFillType.Solid, -16777216)); // -16777216 is 0xff000000 (Black);
            _supportStyleProperties[StylePropertyNames.BULLETED_LIST_PROP] = new StyleEnumProperty<System.Windows.TextMarkerStyle>(StylePropertyNames.BULLETED_LIST_PROP, System.Windows.TextMarkerStyle.None);
            _supportStyleProperties[StylePropertyNames.LINE_COLOR_PROP] = new StyleColorProperty(StylePropertyNames.LINE_COLOR_PROP, new StyleColor(ColorFillType.Solid, -4473925));  // -4473925 is 0xFFBBBBBB
            _supportStyleProperties[StylePropertyNames.LINE_WIDTH_PROP] = new StyleDoubleProperty(StylePropertyNames.LINE_WIDTH_PROP, 1);
            _supportStyleProperties[StylePropertyNames.LINE_STYLE_PROP] = new StyleEnumProperty<LineStyle>(StylePropertyNames.LINE_STYLE_PROP, LineStyle.Solid);
            _supportStyleProperties[StylePropertyNames.CORNER_RADIUS_PROP] = new StyleIntegerProperty(StylePropertyNames.CORNER_RADIUS_PROP, 0);
            _supportStyleProperties[StylePropertyNames.FILL_COLOR_PROP] = new StyleColorProperty(StylePropertyNames.FILL_COLOR_PROP, new StyleColor(ColorFillType.Solid, -723724));   // -723724 is 0xFFF4F4F4
            _supportStyleProperties[StylePropertyNames.OPACITY_PROP] = new StyleIntegerProperty(StylePropertyNames.OPACITY_PROP, 100);
            _supportStyleProperties[StylePropertyNames.HORZ_ALIGN_PROP] = new StyleEnumProperty<Alignment>(StylePropertyNames.HORZ_ALIGN_PROP, Alignment.Center);
            _supportStyleProperties[StylePropertyNames.VERT_ALIGN_PROP] = new StyleEnumProperty<Alignment>(StylePropertyNames.VERT_ALIGN_PROP, Alignment.Center);
        }

        private FlowShapeType _flowShapeType;
    }
}
