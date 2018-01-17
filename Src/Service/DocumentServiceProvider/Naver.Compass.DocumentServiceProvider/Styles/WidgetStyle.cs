using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class WidgetStyle : RegionStyle, IWidgetStyle
    {
        #region Constructors

        internal WidgetStyle()
            : this(null, Guid.Empty)
        {
        }

        internal WidgetStyle(Widget ownerWidget, Guid viewGuid)
            : base(viewGuid, "WidgetStyle")
        {
            _ownerWidget = ownerWidget;
        }

        #endregion

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            Guid viewGuid = Guid.Empty;
            LoadGuidFromChildElementInnerText("AdaptiveViewGuid", element, ref viewGuid);
            ViewGuid = viewGuid;

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

            SaveStringToChildElement("AdaptiveViewGuid", ViewGuid.ToString(), xmlDoc, widgetStyleElement);

            base.SaveDataToXml(xmlDoc, widgetStyleElement);
        }

        #endregion

        public override IRegion OwnerRegion
        {
            get { return OwnerWidget; }
        }

        public IWidget OwnerWidget
        {
            get { return _ownerWidget; }
            set { _ownerWidget = value as Widget; }
        }
        public string MD5 { get; set; }

        public bool IsFixed
        {
            get
            {
                StyleBooleanProperty property = GetStyleProperty(StylePropertyNames.IS_FIXED_PROP) as StyleBooleanProperty;
                if (property != null)
                {
                    return property.BooleanValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.IsFixed;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.IS_FIXED_PROP, value);
            }
        }

        public override bool IsVisible
        {
            get
            {
                StyleBooleanProperty property = GetStyleProperty(StylePropertyNames.IS_VISIBLE_PROP) as StyleBooleanProperty;
                if (property != null)
                {
                    return property.BooleanValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.IsVisible;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.IS_VISIBLE_PROP, value);
            }
        }

        public override double X
        {
            get
            {
                StyleDoubleProperty property = GetStyleProperty(StylePropertyNames.X_Prop) as StyleDoubleProperty;
                if (property != null)
                {
                    return property.DoubleValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.X;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.X_Prop, value);
            }
        }

        public override double Y
        {
            get
            {
                StyleDoubleProperty property = GetStyleProperty(StylePropertyNames.Y_Prop) as StyleDoubleProperty;
                if (property != null)
                {
                    return property.DoubleValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Y;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.Y_Prop, value);
            }
        }

        public override double Height
        {
            get
            {
                StyleDoubleProperty property = GetStyleProperty(StylePropertyNames.HEIGHT_PROP) as StyleDoubleProperty;
                if (property != null)
                {
                    return property.DoubleValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Height;
                }
                else
                {
                    return 100;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.HEIGHT_PROP, value);
            }
        }

        public override double Width
        {
            get
            {
                StyleDoubleProperty property = GetStyleProperty(StylePropertyNames.WIDTH_PROP) as StyleDoubleProperty;
                if (property != null)
                {
                    return property.DoubleValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Width;
                }
                else
                {
                    return 100;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.WIDTH_PROP, value);
            }
        }

        public override int Z
        {
            get
            {
                StyleIntegerProperty property = GetStyleProperty(StylePropertyNames.Z_PROP) as StyleIntegerProperty;
                if (property != null)
                {
                    return property.IntegerValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Z;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.Z_PROP, value);
            }
        }

        public override double Rotate
        {
            get { return WidgetRotate; }
        }

        public double WidgetRotate
        {
            get
            {
                StyleDoubleProperty property = GetStyleProperty(StylePropertyNames.WIDGET_ROTATE_PROP) as StyleDoubleProperty;
                if (property != null)
                {
                    return property.DoubleValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.WidgetRotate;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.WIDGET_ROTATE_PROP, value);
            }
        }

        public double TextRotate
        {
            get
            {
                StyleDoubleProperty property = GetStyleProperty(StylePropertyNames.TEXT_ROTATE_PROP) as StyleDoubleProperty;
                if (property != null)
                {
                    return property.DoubleValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.TextRotate;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.TEXT_ROTATE_PROP, value);
            }
        }

        public string FontFamily
        {
            get
            {
                StyleProperty property = GetStyleProperty(StylePropertyNames.FONT_FAMILY_PROP) as StyleProperty;
                if (property != null)
                {
                    return property.Value;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.FontFamily;
                }
                else
                {
                    return "Arial";
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.FONT_FAMILY_PROP, value);
            }
        }

        public double FontSize
        {
            get
            {
                StyleDoubleProperty property = GetStyleProperty(StylePropertyNames.FONT_SIZE_PROP) as StyleDoubleProperty;
                if (property != null)
                {
                    return property.DoubleValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.FontSize;
                }
                else
                {
                    return 13;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.FONT_SIZE_PROP, value);
            }
        }

        public bool Bold
        {
            get
            {
                StyleBooleanProperty property = GetStyleProperty(StylePropertyNames.BOLD_PROP) as StyleBooleanProperty;
                if (property != null)
                {
                    return property.BooleanValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Bold;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.BOLD_PROP, value);
            }
        }

        public bool Italic
        {
            get
            {
                StyleBooleanProperty property = GetStyleProperty(StylePropertyNames.ITALIC_PROP) as StyleBooleanProperty;
                if (property != null)
                {
                    return property.BooleanValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Italic;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.ITALIC_PROP, value.ToString());
            }
        }

        public bool Underline
        {
            get
            {
                StyleBooleanProperty property = GetStyleProperty(StylePropertyNames.UNDERLINE_PROP) as StyleBooleanProperty;
                if (property != null)
                {
                    return property.BooleanValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Underline;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.UNDERLINE_PROP, value);
            }
        }

        public bool Baseline
        {
            get
            {
                StyleBooleanProperty property = GetStyleProperty(StylePropertyNames.BASELINE_PROP) as StyleBooleanProperty;
                if (property != null)
                {
                    return property.BooleanValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Baseline;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.BASELINE_PROP, value);
            }
        }

        public bool Overline
        {
            get
            {
                StyleBooleanProperty property = GetStyleProperty(StylePropertyNames.OVERLINE_PROP) as StyleBooleanProperty;
                if (property != null)
                {
                    return property.BooleanValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Overline;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.OVERLINE_PROP, value);
            }
        }

        public bool Strikethrough
        {
            get
            {
                StyleBooleanProperty property = GetStyleProperty(StylePropertyNames.STRIKETHROUGH_PROP) as StyleBooleanProperty;
                if (property != null)
                {
                    return property.BooleanValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Strikethrough;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.STRIKETHROUGH_PROP, value);
            }
        }

        public StyleColor FontColor
        {
            get
            {
                StyleColorProperty property = GetStyleProperty(StylePropertyNames.FONT_COLOR_PROP) as StyleColorProperty;
                if (property != null)
                {
                    return property.ColorValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.FontColor;
                }
                else
                {
                    return new StyleColor(ColorFillType.Solid, -16777216);  // -16777216 is 0xff000000 (Black);
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.FONT_COLOR_PROP, value);
            }
        }


        public TextMarkerStyle BulletedList
        {
            get
            {
                StyleEnumProperty<TextMarkerStyle> property = GetStyleProperty(StylePropertyNames.BULLETED_LIST_PROP) as StyleEnumProperty<TextMarkerStyle>;
                if (property != null)
                {
                    return property.EnumValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.BulletedList;
                }
                else
                {
                    return System.Windows.TextMarkerStyle.None;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.BULLETED_LIST_PROP, value);
            }
        }

        public StyleColor LineColor
        {
            get
            {
                StyleColorProperty property = GetStyleProperty(StylePropertyNames.LINE_COLOR_PROP) as StyleColorProperty;
                if (property != null)
                {
                    return property.ColorValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.LineColor;
                }
                else
                {
                    return new StyleColor(ColorFillType.Solid, -16777216);  // -16777216 is 0xff000000 (Black);
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.LINE_COLOR_PROP, value);
            }
        }

        public double LineWidth
        {
            get
            {
                StyleDoubleProperty property = GetStyleProperty(StylePropertyNames.LINE_WIDTH_PROP) as StyleDoubleProperty;
                if (property != null)
                {
                    return property.DoubleValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.LineWidth;
                }
                else
                {
                    return 1;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.LINE_WIDTH_PROP, value);
            }
        }

        public LineStyle LineStyle
        {
            get
            {
                StyleEnumProperty<LineStyle> property = GetStyleProperty(StylePropertyNames.LINE_STYLE_PROP) as StyleEnumProperty<LineStyle>;
                if (property != null)
                {
                    return property.EnumValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.LineStyle;
                }
                else
                {
                    return LineStyle.Solid;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.LINE_STYLE_PROP, value);
            }
        }

        public ArrowStyle ArrowStyle
        {
            get
            {
                StyleEnumProperty<ArrowStyle> property = GetStyleProperty(StylePropertyNames.ARROW_STYLE_PROP) as StyleEnumProperty<ArrowStyle>;
                if (property != null)
                {
                    return property.EnumValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.ArrowStyle;
                }
                else
                {
                    return ArrowStyle.None;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.ARROW_STYLE_PROP, value);
            }
        }

        public int CornerRadius
        {
            get
            {
                StyleIntegerProperty property = GetStyleProperty(StylePropertyNames.CORNER_RADIUS_PROP) as StyleIntegerProperty;
                if (property != null)
                {
                    return property.IntegerValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.CornerRadius;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.CORNER_RADIUS_PROP, value);
            }
        }

        public StyleColor FillColor
        {
            get
            {
                StyleColorProperty property = GetStyleProperty(StylePropertyNames.FILL_COLOR_PROP) as StyleColorProperty;
                if (property != null)
                {
                    return property.ColorValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.FillColor;
                }
                else
                {
                    return new StyleColor(ColorFillType.Solid, -1);  // -1 is 0xffffffff (White)
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.FILL_COLOR_PROP, value);
            }
        }

        public int Opacity
        {
            get
            {
                StyleIntegerProperty property = GetStyleProperty(StylePropertyNames.OPACITY_PROP) as StyleIntegerProperty;
                if (property != null)
                {
                    return property.IntegerValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Opacity;
                }
                else
                {
                    return 100;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.OPACITY_PROP, value);
            }
        }

        public Alignment HorzAlign
        {
            get
            {
                StyleEnumProperty<Alignment> property = GetStyleProperty(StylePropertyNames.HORZ_ALIGN_PROP) as StyleEnumProperty<Alignment>;
                if (property != null)
                {
                    return property.EnumValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.HorzAlign;
                }
                else
                {
                    return Alignment.Center;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.HORZ_ALIGN_PROP, value);
            }
        }

        public Alignment VertAlign
        {
            get
            {
                StyleEnumProperty<Alignment> property = GetStyleProperty(StylePropertyNames.VERT_ALIGN_PROP) as StyleEnumProperty<Alignment>;
                if (property != null)
                {
                    return property.EnumValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.VertAlign;
                }
                else
                {
                    return Alignment.Center;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.VERT_ALIGN_PROP, value);
            }
        }

        internal static void CopyWidgetStyle(WidgetStyle source, WidgetStyle target)
        {
            target.IsVisible = source.IsVisible;
            target.X = source.X;
            target.Y = source.Y;
            target.Height = source.Height;
            target.Width = source.Width;
            target.Z = source.Z;
            target.IsFixed = source.IsFixed;
            target.WidgetRotate = source.WidgetRotate;
            target.TextRotate = source.TextRotate;
            target.FontFamily = source.FontFamily;
            target.FontSize = source.FontSize;
            target.Bold = source.Bold;
            target.Italic = source.Italic;
            target.Underline = source.Underline;
            target.Baseline = source.Baseline;
            target.Overline = source.Overline;
            target.Strikethrough = source.Strikethrough;
            target.FontColor = source.FontColor;
            target.BulletedList = source.BulletedList;
            target.LineColor = source.LineColor;
            target.LineWidth = source.LineWidth;
            target.LineStyle = source.LineStyle;
            target.ArrowStyle = source.ArrowStyle;
            target.CornerRadius = source.CornerRadius;
            target.FillColor = source.FillColor;
            target.Opacity = source.Opacity;
            target.HorzAlign = source.HorzAlign;
            target.VertAlign = source.VertAlign;
        }

        private WidgetStyle ParentStyle
        {
            get
            {
                if (ParentDocument != null && ParentDocument.IsOpened)
                {
                    // This is base view style.
                    if ( ViewGuid == ParentDocument.AdaptiveViewSet.Base.Guid)
                    {
                        return null;
                    }
                    else
                    {
                        // Return style in parent view
                        IAdaptiveView view = ParentDocument.AdaptiveViewSet.AdaptiveViews[ViewGuid];
                        if (view != null && view.ParentView != null)
                        {
                            WidgetStyle style = _ownerWidget.GetWidgetStyle(view.ParentView.Guid) as WidgetStyle;
                            if (style == this)
                            {
                                // Stack overflow, it is infinite recursive!!!!!!
                                // This typically is that UI hold IWidget, IPage after document is closed !!!
                                // Force it to null to avoid infinite recursive.
                                style = null;
                            }

                            return style;
                        }
                    }
                }
                    
                return null;
            }
        }

        private Widget _ownerWidget;
    }
}
