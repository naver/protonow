using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class Shape : Widget, IShape
    {
        internal Shape(Page parentPage, ShapeType shapeType)
            : base(parentPage, "Shape")
        {
            _widgetType = WidgetType.Shape;

            _shapeType = shapeType;

            if(_shapeType != ShapeType.None)
            {
                InitializeBaseViewStyleFromDefaultStyle();
            }
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            if (!LoadEnumFromChildElementInnerText<ShapeType>("ShapeType", element, ref _shapeType))
            {
                throw new XmlException("ShapeType element is required!");
            }

            base.LoadDataFromXml(element);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement shapeElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(shapeElement);

            SaveStringToChildElement("ShapeType", _shapeType.ToString(), xmlDoc, shapeElement);

            base.SaveDataToXml(xmlDoc, shapeElement);
        }

        #endregion

        public ShapeType ShapeType 
        {
            get
            {
                return _shapeType;
            }

            set
            {
                if(_shapeType != value)
                {
                    ShapeType oldType = _shapeType;
                    _shapeType = value;

                    // Reset the default style as shape type is changed.
                    WidgetDefaultStyle = null;

                    if (oldType == ShapeType.None)
                    {
                        InitializeBaseViewStyleFromDefaultStyle();
                    }
                }
            }
        }

        public override void SetWidgetStyleAsDefaultStyle(Guid viewGuid)
        {
            base.SetWidgetStyleAsDefaultStyle(viewGuid);

            // Set text relevant style form rich text.
            GetStyleValueFromRichText(_richText, WidgetDefaultStyle);
        }

        internal static bool SupportedStyleProperty(string stylePropertyName)
        {
            return _supportStyleProperties.ContainsKey(stylePropertyName);
        }

        protected override Dictionary<string, StyleProperty> SupportStyleProperties
        {
            get
            {
                if (_shapeType == ShapeType.Paragraph)
                {
                    return _paragraphSupportStyleProperties;
                }
                else if (_shapeType == ShapeType.RoundedRectangle)
                {
                    return _roundedRectSupportStyleProperties;
                }
                else
                {
                    return _supportStyleProperties;
                }
            }
        }

        private static Dictionary<string, StyleProperty> _supportStyleProperties = new Dictionary<string, StyleProperty>();
        private static Dictionary<string, StyleProperty> _paragraphSupportStyleProperties = new Dictionary<string, StyleProperty>();
        private static Dictionary<string, StyleProperty> _roundedRectSupportStyleProperties = new Dictionary<string, StyleProperty>();

        static Shape()
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

            foreach (StyleProperty property in _supportStyleProperties.Values)
            {
                if (property.Name == StylePropertyNames.HORZ_ALIGN_PROP)
                {
                    _paragraphSupportStyleProperties[StylePropertyNames.HORZ_ALIGN_PROP] = new StyleEnumProperty<Alignment>(StylePropertyNames.HORZ_ALIGN_PROP, Alignment.Left);
                    _roundedRectSupportStyleProperties[StylePropertyNames.HORZ_ALIGN_PROP] = property.Clone();
                    continue;
                }

                if (property.Name == StylePropertyNames.VERT_ALIGN_PROP)
                {
                    _paragraphSupportStyleProperties[StylePropertyNames.VERT_ALIGN_PROP] = new StyleEnumProperty<Alignment>(StylePropertyNames.VERT_ALIGN_PROP, Alignment.Top);
                    _roundedRectSupportStyleProperties[StylePropertyNames.VERT_ALIGN_PROP] = property.Clone();
                    continue;
                }

                if (property.Name == StylePropertyNames.FILL_COLOR_PROP)
                {
                    _paragraphSupportStyleProperties[StylePropertyNames.FILL_COLOR_PROP] = new StyleColorProperty(StylePropertyNames.FILL_COLOR_PROP, new StyleColor(ColorFillType.Solid, 0x00FFFFFF));
                    _roundedRectSupportStyleProperties[StylePropertyNames.FILL_COLOR_PROP] = property.Clone();
                    continue;
                }

                if (property.Name == StylePropertyNames.LINE_WIDTH_PROP)
                {
                    _paragraphSupportStyleProperties[StylePropertyNames.LINE_WIDTH_PROP] = new StyleDoubleProperty(StylePropertyNames.LINE_WIDTH_PROP, 0);
                    _roundedRectSupportStyleProperties[StylePropertyNames.LINE_WIDTH_PROP] = property.Clone();
                    continue;
                }

                if (property.Name == StylePropertyNames.CORNER_RADIUS_PROP)
                {
                    _paragraphSupportStyleProperties[StylePropertyNames.CORNER_RADIUS_PROP] = property.Clone();
                    _roundedRectSupportStyleProperties[StylePropertyNames.CORNER_RADIUS_PROP] = new StyleIntegerProperty(StylePropertyNames.CORNER_RADIUS_PROP, 10);
                    continue;
                }

                _paragraphSupportStyleProperties[property.Name] = property.Clone();
                _roundedRectSupportStyleProperties[property.Name] = property.Clone();
            }
        }

        private ShapeType _shapeType;
    }
}
