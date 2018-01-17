using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class Checkbox : SelectionButton, ICheckbox
    {
        internal Checkbox(Page parentPage)
            : base(parentPage, "Checkbox")
        {
            _widgetType = WidgetType.Checkbox;
            _text = "Checkbox";

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

        static Checkbox()
        {
            // Add support style properties and default value.
            _supportStyleProperties[StylePropertyNames.FONT_FAMILY_PROP] = new StyleProperty(StylePropertyNames.FONT_FAMILY_PROP, "Arial");
            _supportStyleProperties[StylePropertyNames.FONT_SIZE_PROP] = new StyleDoubleProperty(StylePropertyNames.FONT_SIZE_PROP, 13);
            _supportStyleProperties[StylePropertyNames.BOLD_PROP] = new StyleBooleanProperty(StylePropertyNames.BOLD_PROP, false);
            _supportStyleProperties[StylePropertyNames.ITALIC_PROP] = new StyleBooleanProperty(StylePropertyNames.ITALIC_PROP, false);
            _supportStyleProperties[StylePropertyNames.UNDERLINE_PROP] = new StyleBooleanProperty(StylePropertyNames.UNDERLINE_PROP, false);     // CSS - text-decoration-line : underline
            _supportStyleProperties[StylePropertyNames.OVERLINE_PROP] = new StyleBooleanProperty(StylePropertyNames.OVERLINE_PROP, false);     // CSS - text-decoration-line : overline
            _supportStyleProperties[StylePropertyNames.STRIKETHROUGH_PROP] = new StyleBooleanProperty(StylePropertyNames.STRIKETHROUGH_PROP, false); // CSS - text-decoration-line : line-through
            _supportStyleProperties[StylePropertyNames.FONT_COLOR_PROP] = new StyleColorProperty(StylePropertyNames.FONT_COLOR_PROP, new StyleColor(ColorFillType.Solid, -16777216)); // -16777216 is 0xff000000 (Black);
            _supportStyleProperties[StylePropertyNames.OPACITY_PROP] = new StyleIntegerProperty(StylePropertyNames.OPACITY_PROP, 100);
            _supportStyleProperties[StylePropertyNames.HORZ_ALIGN_PROP] = new StyleEnumProperty<Alignment>(StylePropertyNames.HORZ_ALIGN_PROP, Alignment.Left);
            _supportStyleProperties[StylePropertyNames.VERT_ALIGN_PROP] = new StyleEnumProperty<Alignment>(StylePropertyNames.VERT_ALIGN_PROP, Alignment.Top);
        }
    }
}

