using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class WidgetDefaultStyle : Style, IWidgetDefaultStyle
    {
        internal WidgetDefaultStyle(WidgetDefaultStyleSet set, string styleName)
            : base("WidgetDefaultStyle")  
        {
            // WidgetDefaultStyle must exist with the WidgetDefaultStyleSet.
            Debug.Assert(set != null);

            _set = set;
            _name = styleName;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadStringFromChildElementInnerText("Name", element, ref _name);

            XmlElement propertiesElement = element["StyleProperties"];
            if (propertiesElement == null || propertiesElement.ChildNodes.Count <= 0)
            {
                return;
            }

            // Try to load all supported widget style prpoperties.
            // Create type specific style property.
            LoadStyleBooleanProperty(propertiesElement, StylePropertyNames.IS_FIXED_PROP);
            LoadStyleBooleanProperty(propertiesElement, StylePropertyNames.IS_VISIBLE_PROP);
            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.X_Prop);
            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.Y_Prop);
            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.HEIGHT_PROP);
            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.WIDTH_PROP);
            LoadStyleIntegerProperty(propertiesElement, StylePropertyNames.Z_PROP);

            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.WIDGET_ROTATE_PROP);
            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.TEXT_ROTATE_PROP);

            LoadStyleStringProperty(propertiesElement, StylePropertyNames.FONT_FAMILY_PROP);
            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.FONT_SIZE_PROP);
            LoadStyleBooleanProperty(propertiesElement, StylePropertyNames.BOLD_PROP);
            LoadStyleBooleanProperty(propertiesElement, StylePropertyNames.ITALIC_PROP);
            LoadStyleBooleanProperty(propertiesElement, StylePropertyNames.UNDERLINE_PROP);
            LoadStyleBooleanProperty(propertiesElement, StylePropertyNames.OVERLINE_PROP);
            LoadStyleBooleanProperty(propertiesElement, StylePropertyNames.STRIKETHROUGH_PROP);
            LoadStyleColorProperty(propertiesElement, StylePropertyNames.FONT_COLOR_PROP);
            LoadStyleEnumProperty<System.Windows.TextMarkerStyle>(propertiesElement, StylePropertyNames.BULLETED_LIST_PROP);

            LoadStyleColorProperty(propertiesElement, StylePropertyNames.LINE_COLOR_PROP);
            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.LINE_WIDTH_PROP);
            LoadStyleEnumProperty<LineStyle>(propertiesElement, StylePropertyNames.LINE_STYLE_PROP);
            LoadStyleEnumProperty<ArrowStyle>(propertiesElement, StylePropertyNames.ARROW_STYLE_PROP);

            LoadStyleIntegerProperty(propertiesElement, StylePropertyNames.CORNER_RADIUS_PROP);
            LoadStyleColorProperty(propertiesElement, StylePropertyNames.FILL_COLOR_PROP);
            LoadStyleIntegerProperty(propertiesElement, StylePropertyNames.OPACITY_PROP);
            LoadStyleEnumProperty<Alignment>(propertiesElement, StylePropertyNames.HORZ_ALIGN_PROP);
            LoadStyleEnumProperty<Alignment>(propertiesElement, StylePropertyNames.VERT_ALIGN_PROP);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement widgetStyleElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(widgetStyleElement);

            SaveStringToChildElement("Name", _name, xmlDoc, widgetStyleElement);

            base.SaveDataToXml(xmlDoc, widgetStyleElement);
        }

        #endregion

        public IWidgetDefaultStyleSet WidgetDefaultStyleSet
        {
            get { return _set; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                throw new NotSupportedException("Cannot change the name of widget default style!");
            }
        }
 
        private string _name;
        private WidgetDefaultStyleSet _set;
    }
}
