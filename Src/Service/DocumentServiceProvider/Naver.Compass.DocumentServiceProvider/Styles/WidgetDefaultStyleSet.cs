using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class WidgetDefaultStyleSet : XmlElementObject, IWidgetDefaultStyleSet
    {
        public WidgetDefaultStyleSet(Document document)
            : base("WidgetDefaultStyleSet") 
        {
            // WidgetDefaultStyleSet must exist with the document.
            Debug.Assert(document != null);
            _document = document;

            _defaultStyles = new WidgetDefaultStyles(this);

            InitializeWidgetDefaultStyles();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            XmlElement defaultStylesElement = element[_defaultStyles.TagName];
            if (defaultStylesElement != null)
            {
                _defaultStyles.LoadDataFromXml(defaultStylesElement);
            }

            // Add missing default style.
            InitializeWidgetDefaultStyles();
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement setElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(setElement);

            _defaultStyles.SaveDataToXml(xmlDoc, setElement);
        }

        #endregion

        #region IWidgetDefaultStyleSet

        public IDocument ParentDocument
        {
            get { return _document; }
        }

        public IWidgetDefaultStyles WidgetDefaultStyles 
        {
            get { return _defaultStyles; }
        }

        public IWidgetDefaultStyle GetWidgetDefaultStyle(IWidget widget)
        {
            if(widget == null)
            {
                throw new ArgumentException("widget");
            }

            switch (widget.WidgetType)
            {
                case WidgetType.Shape:
                    {
                        IShape shape = widget as IShape;
                        if (shape != null)
                        {
                            return GetShapeDefaultStyle(shape.ShapeType);
                        }
                        else
                        {
                            return null;
                        }
                    }
                case WidgetType.FlowShape:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_FLOW_SHAPE_STYLE_NAME];

                case WidgetType.Image:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_IMAGE_STYLE_NAME];

                case WidgetType.DynamicPanel:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_DYNAMICPANEL_STYLE_NAME];

                case WidgetType.HamburgerMenu:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_HAMBURGERMENU_STYLE_NAME];

                case WidgetType.Toast:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_TOAST_STYLE_NAME];
                
                case WidgetType.Line:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_LINE_STYLE_NAME];

                case WidgetType.HotSpot:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_HOTSPOT_STYLE_NAME];

                case WidgetType.TextField:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_TEXTFIELD_STYLE_NAME];

                case WidgetType.TextArea:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_TEXTAREA_STYLE_NAME];

                case WidgetType.DropList:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_DROPLIST_STYLE_NAME];

                case WidgetType.ListBox:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_LISTBOX_STYLE_NAME];

                case WidgetType.Checkbox:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_CHECKBOX_STYLE_NAME];

                case WidgetType.RadioButton:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_RADIOBUTTON_STYLE_NAME];

                case WidgetType.Button:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_BUTTON_STYLE_NAME];

                case WidgetType.SVG:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_SVG_STYLE_NAME];

                default:
                    return null;
            }
        }

        #endregion

        #region Internal Methods

        internal void Clear()
        {
            _defaultStyles.Clear();
        }

        #endregion

        #region Private Methods

        private IWidgetDefaultStyle GetShapeDefaultStyle(ShapeType shapeType)
        {
            switch (shapeType)
            {
                case ShapeType.Rectangle:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_SHAPE_RECTANGLE_STYLE_NAME];

                case ShapeType.RoundedRectangle:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_SHAPE_ROUNDED_RECTANGLE_STYLE_NAME];

                case ShapeType.Ellipse:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_SHAPE_ELLIPSE_STYLE_NAME];

                case ShapeType.Diamond:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_SHAPE_DIAMOND_STYLE_NAME];

                case ShapeType.Triangle:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_SHAPE_TRIANGLE_STYLE_NAME];

                case ShapeType.Paragraph:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_SHAPE_PARAGRAPH_STYLE_NAME];

                default:
                    return _defaultStyles[DefaultStyleNames.DEFAULT_SHAPE_STYLE_NAME];
            }
        }

        private void InitializeWidgetDefaultStyles()
        {
            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_SHAPE_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_SHAPE_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_SHAPE_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_SHAPE_RECTANGLE_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_SHAPE_RECTANGLE_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_SHAPE_RECTANGLE_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_SHAPE_ROUNDED_RECTANGLE_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_SHAPE_ROUNDED_RECTANGLE_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_SHAPE_ROUNDED_RECTANGLE_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_SHAPE_ELLIPSE_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_SHAPE_ELLIPSE_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_SHAPE_ELLIPSE_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_SHAPE_DIAMOND_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_SHAPE_DIAMOND_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_SHAPE_DIAMOND_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_SHAPE_TRIANGLE_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_SHAPE_TRIANGLE_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_SHAPE_TRIANGLE_STYLE_NAME));
            }
            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_SHAPE_PARAGRAPH_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_SHAPE_PARAGRAPH_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_SHAPE_PARAGRAPH_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_IMAGE_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_IMAGE_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_IMAGE_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_BUTTON_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_BUTTON_STYLE_NAME,
                                  new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_BUTTON_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_FLOW_SHAPE_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_FLOW_SHAPE_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_FLOW_SHAPE_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_HOTSPOT_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_HOTSPOT_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_HOTSPOT_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_LINE_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_LINE_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_LINE_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_LISTBOX_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_LISTBOX_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_LISTBOX_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_DROPLIST_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_DROPLIST_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_DROPLIST_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_TEXTFIELD_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_TEXTFIELD_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_TEXTFIELD_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_TEXTAREA_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_TEXTAREA_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_TEXTAREA_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_CHECKBOX_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_CHECKBOX_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_CHECKBOX_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_RADIOBUTTON_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_RADIOBUTTON_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_RADIOBUTTON_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_DYNAMICPANEL_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_DYNAMICPANEL_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_DYNAMICPANEL_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_HAMBURGERMENU_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_HAMBURGERMENU_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_HAMBURGERMENU_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_TOAST_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_TOAST_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_TOAST_STYLE_NAME));
            }

            if (!_defaultStyles.Contains(DefaultStyleNames.DEFAULT_SVG_STYLE_NAME))
            {
                _defaultStyles.Add(DefaultStyleNames.DEFAULT_SVG_STYLE_NAME,
                                   new WidgetDefaultStyle(this, DefaultStyleNames.DEFAULT_SVG_STYLE_NAME));
            }
        }

        #endregion

        #region Private Fields

        private Document _document;
        private WidgetDefaultStyles _defaultStyles;

        #endregion
    }
}
