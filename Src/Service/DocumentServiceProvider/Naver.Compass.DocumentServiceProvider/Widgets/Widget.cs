using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;

namespace Naver.Compass.Service.Document
{
    internal abstract class Widget : Region, IWidget
    {
        #region Constructors

        internal Widget()
            : this(null, "")
        {
        }

        internal Widget(Page parentPage, string tagName)
            : base(parentPage, tagName)
        {
            // The value in  _widgetStyle will be set in specific widget(Button, Checkbox...) constructor 
            // or loaded from xml element.
            Guid baseViewGuid = Guid.Empty;
            if (ParentDocument != null && ParentDocument.IsOpened)
            {
                baseViewGuid = ParentDocument.AdaptiveViewSet.Base.Guid;
            }
            _widgetStyle = new WidgetStyle(this, baseViewGuid);

            _widgetStyles = new WidgetStyles(this);
            InitializeAdaptiveViewStyles();

            _annotation = new Annotation(this);

            _events = new InteractionEvents(this);
            AddWidgetSupportEvents();
        }

        #endregion

        #region IAnnotatedObject, IInteractiveObject

        public IAnnotationFieldSet AnnotationFieldSet
        {
            get
            {
                if (ParentDocument != null)
                {
                    return ParentDocument.WidgetAnnotationFieldSet;
                }

                return null;
            }
        }

        public IAnnotation Annotation
        {
            get { return _annotation; }
        }

        public IPage ContextPage
        {
            get { return ParentPage; }
        }

        public IInteractionEvents Events
        {
            get { return _events; }
        }

        #endregion

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            XmlElement propertiesElement = element["Properties"];
            if (propertiesElement != null)
            {
                LoadGuidFromChildElementInnerText("ParentGroupGuid", propertiesElement, ref _parentGroupGuid);
                LoadBoolFromChildElementInnerText("IsDisabled", propertiesElement, ref _isDisabled);
                LoadBoolFromChildElementInnerText("IsLocked", propertiesElement, ref _isLocked);
                LoadStringFromChildElementInnerText("Text", propertiesElement, ref _text);
                LoadStringFromChildElementInnerText("RichText", propertiesElement, ref _richText);
                LoadStringFromChildElementInnerText("Tooltip", propertiesElement, ref _tooltip);

                // If we cannot load those two values from file, this means the file was created before DOM version 5.0.0.0, 
                // we make them all have been placed in base view.
                if (!LoadGuidFromChildElementInnerText("CreatedViewGuid", propertiesElement, ref _createdViewGuid)
                    || !LoadBoolFromChildElementInnerText("HasBeenPlacedInBaseView", propertiesElement, ref _hasBeenPlacedInBaseView))
                {
                    _hasBeenPlacedInBaseView = true;
                }
            }

            // Load Widget Annotation
            XmlElement widgetAnnotationElement = element["Annotation"];
            if (widgetAnnotationElement != null)
            {
                _annotation.LoadDataFromXml(widgetAnnotationElement);
            }

            // Load Events
            XmlElement eventsElement = element["Events"];
            if (eventsElement != null)
            {
                _events.LoadDataFromXml(eventsElement);
            }

            // Load WidgetStyle in Base view
            XmlElement styleElement = element["WidgetStyle"];
            if (styleElement != null)
            {
                _widgetStyle.LoadDataFromXml(styleElement);
            }

            // Load widget styles in adaptive views
            XmlElement widgetStylesElement = element["WidgetStyles"];
            if (widgetStylesElement != null)
            {
                _widgetStyles.LoadDataFromXml(widgetStylesElement);
            }

            MatchTextAndRichText();
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            base.SaveDataToXml(xmlDoc, parentElement);

            //Save Properties
            XmlElement propertiesElement = xmlDoc.CreateElement("Properties");
            parentElement.AppendChild(propertiesElement);

            if (_parentGroupGuid != Guid.Empty)
            {
                SaveStringToChildElement("ParentGroupGuid", _parentGroupGuid.ToString(), xmlDoc, propertiesElement);
            }

            SaveStringToChildElement("IsDisabled", _isDisabled.ToString(), xmlDoc, propertiesElement);
            SaveStringToChildElement("IsLocked", _isLocked.ToString(), xmlDoc, propertiesElement);            
            SaveStringToChildElement("Text", _text, xmlDoc, propertiesElement);
            SaveStringToChildElement("RichText", _richText, xmlDoc, propertiesElement);
            SaveStringToChildElement("Tooltip", _tooltip, xmlDoc, propertiesElement);
            SaveStringToChildElement("CreatedViewGuid", _createdViewGuid.ToString(), xmlDoc, propertiesElement);
            SaveStringToChildElement("HasBeenPlacedInBaseView", _hasBeenPlacedInBaseView.ToString(), xmlDoc, propertiesElement);

            // Save Widget Annotation
            _annotation.SaveDataToXml(xmlDoc, parentElement);

            // Save Events
            _events.SaveDataToXml(xmlDoc, parentElement);

            // Save WidgetStyle in Base view
            _widgetStyle.SaveDataToXml(xmlDoc, parentElement);

            // Save widget styles in adaptive views
            _widgetStyles.SaveDataToXml(xmlDoc, parentElement);
        }

        #endregion

        #region IRegion

        public override IRegions GetChildRegions(Guid viewGuid)
        {
            // Empty collection.
            return new Regions();
        }

        public override IRegionStyle RegionStyle
        {
            get { return WidgetStyle; }
        }

        public override IRegionStyle GetRegionStyle(Guid viewGuid)
        {
            return GetWidgetStyle(viewGuid);
        }

        #endregion

        #region IWidget
        public string MD5 { get; set; }

        public WidgetType WidgetType
        {
            get { return _widgetType; }
            set { _widgetType = value; }
        }

        public IWidgetStyle WidgetStyle
        {
            get { return _widgetStyle; }
        }

        public IGroup ParentGroup
        {
            get
            {
                if (_parentGroup == null && ParentPage != null)
                {
                    // If object is null, try to get it in page with its guid.
                    _parentGroup = ParentPage.Groups[_parentGroupGuid] as Group;
                }

                return _parentGroup;
            }

            set
            {
                _parentGroup = value as Group;
                if (_parentGroup != null)
                {
                    _parentGroupGuid = _parentGroup.Guid;
                }
                else
                {
                    _parentGroupGuid = Guid.Empty;
                }
            }
        }

        public Guid ParentGroupGuid
        {
            get { return _parentGroupGuid; }
            set
            {
                _parentGroupGuid = value;
                _parentGroup = null;
            }
        }

        public bool IsDisabled
        {
            get { return _isDisabled; }
            set { _isDisabled = value; }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }
                
        public string Text
        {
            get { return _text; }

            set
            {
                _text = value;
                _richText = BuildRichText(_text, _widgetStyle);
            }
        }

        public string RichText 
        { 
            get
            {
                if (String.IsNullOrEmpty(_richText))
                {
                    // Rich text is null or empty, build it with simple text and text style
                    _richText = BuildRichText(_text, _widgetStyle);
                }

                return _richText;
            }
            set
            {
                _richText = value;
                _text = GetSimpleText(_richText);
            }
        }

        public string Tooltip
        {
            get { return _tooltip; }
            set { _tooltip = value; }
        }

        public void SetRichText(string simpleText, IWidgetStyle style = null)
        {
            if (style == null)
            {
                _richText = BuildRichText(simpleText, _widgetStyle);
            }
            else
            {
                _richText = BuildRichText(simpleText, style as WidgetStyle);
            }

            _text = simpleText;
        }

        public virtual void SetWidgetStyleAsDefaultStyle(Guid viewGuid)
        {
            if (ParentDocument != null && ParentDocument.IsOpened)
            {
                // Copy value to default style
                WidgetStyle style = GetWidgetStyle(viewGuid) as WidgetStyle;

                if (style != null)
                {
                    List<string> excludingList = new List<string>() 
                    { 
                        StylePropertyNames.IS_FIXED_PROP,  
                        StylePropertyNames.IS_VISIBLE_PROP,
                        StylePropertyNames.X_Prop,
                        StylePropertyNames.Y_Prop,
                        StylePropertyNames.Z_PROP,
                        StylePropertyNames.HEIGHT_PROP,
                        StylePropertyNames.WIDTH_PROP
                    };

                    Style.CopyStyle(style, WidgetDefaultStyle, null, excludingList);
                }
            }
        }
        
        public IWidgetStyle GetWidgetStyle(Guid viewGuid)
        {
            if(viewGuid == Guid.Empty)
            {
                return null;
            }

            // If it is base view.
            if (ParentDocument != null && ParentDocument.IsOpened
                && viewGuid == ParentDocument.AdaptiveViewSet.Base.Guid)
            {
                return _widgetStyle;
            }
            else
            {
                // If view style already exists, just return it.
                if (_widgetStyles.Contains(viewGuid))
                {
                    return _widgetStyles[viewGuid];
                }
                else if (ParentDocument != null && ParentDocument.IsOpened)
                {
                    // Add view style if the adaptive view is in document adaptive view set.
                    IAdaptiveView view = ParentDocument.AdaptiveViewSet.AdaptiveViews[viewGuid];
                    if (view != null)
                    {
                        return AddWidgetStyle(view as AdaptiveView);
                    }
                }

                return null;
            }
        }

        #endregion

        #region Events

        /*
         * When a widget is added to the document, the widget data has been loaded.
         * 1. Delete invalid adaptive view style.
         * 2. Add adaptive view style if needed.
         * 3. Delete invalid annotation.
         * */
        internal virtual void OnAddToDocument()
        {
            // Here, the parent document must exist.
            if (ParentDocumentObject == null)
            {
                throw new CannotAddWidgetException("Cannot add widget to document, the document is null.");
            }

            List<Guid> deleteViewGuidList = new List<Guid>();
            foreach (WidgetStyle style in _widgetStyles)
            {
                if (!ParentDocumentObject.AdaptiveViewSet.AdaptiveViews.Contains(style.ViewGuid))
                {
                    deleteViewGuidList.Add(style.ViewGuid);
                }
            }
            
            foreach (Guid guid in deleteViewGuidList)
            {
                DeleteWidgetStyle(guid);
            }

            _widgetStyle.ViewGuid = ParentDocumentObject.AdaptiveViewSet.Base.Guid;

            InitializeAdaptiveViewStyles();

            // Remove invalid page Annotation data.
            if (!_annotation.IsEmpty)
            {
                List<string> deleteFieldList = new List<string>();
                foreach (string field in _annotation.TextValues.Keys)
                {
                    if (!ParentDocumentObject.WidgetAnnotationFieldSet.AnnotationFields.Contains(field))
                    {
                        deleteFieldList.Add(field);
                    }
                }

                foreach (string field in deleteFieldList)
                {
                    _annotation.TextValues.Remove(field);
                }
            }
        }

        internal virtual void OnDeleteFromDocument(bool isParentPageDeleted)
        {
            return;
        }

        internal virtual void OnAddAdaptiveView(AdaptiveView view)
        {
            AddWidgetStyle(view);
        }

        internal virtual void OnDeleteAdaptiveView(Guid viewGuid)
        {
            DeleteWidgetStyle(viewGuid);
        }

        #endregion

        #region Internal Methods

        internal static string GetSimpleText(string richText)
        {
            try
            {
                TextElement textelem = System.Windows.Markup.XamlReader.Parse(richText) as TextElement;
                if (textelem == null)
                {
                    return string.Empty;
                }

                TextRange textRange = new TextRange(textelem.ContentStart, textelem.ContentEnd);
                return textRange.Text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return string.Empty;
        }

        internal static string BuildRichText(string simpleText, WidgetStyle style)
        {
            Section section = new Section();
            //section.FontFamily = new FontFamily(style.FontFamily);
            //section.Foreground = new SolidColorBrush(Color.FromArgb((byte)(style.FontColor.ARGB >> 24),
            //                (byte)(style.FontColor.ARGB >> 16),
            //                (byte)(style.FontColor.ARGB >> 8),
            //                (byte)(style.FontColor.ARGB)));

            //if (style.Bold)
            //{
            //    section.FontWeight = FontWeights.Bold;
            //}

            section.FontSize = style.FontSize;

            Paragraph para = new Paragraph();

            Run run = new Run(simpleText);

            if (style.Underline)
            {
                run.TextDecorations.Add(TextDecorations.Underline);
            }

            if (style.Strikethrough)
            {
                run.TextDecorations.Add(TextDecorations.Strikethrough);
            }

            if (style.Italic)
            {
                run.FontStyle = FontStyles.Italic;
            }

            if (style.Bold)
            {
                run.FontWeight = FontWeights.Bold;
            }

            run.FontFamily = new FontFamily(style.FontFamily);

            run.FontSize = style.FontSize;

            run.Foreground = new SolidColorBrush(Color.FromArgb((byte)(style.FontColor.ARGB >> 24),
                          (byte)(style.FontColor.ARGB >> 16),
                          (byte)(style.FontColor.ARGB >> 8),
                          (byte)(style.FontColor.ARGB)));

            para.Inlines.Add(run);

            if (style.HorzAlign == Alignment.Left)
            {
                para.TextAlignment = TextAlignment.Left;
            }
            else if (style.HorzAlign == Alignment.Center)
            {
                para.TextAlignment = TextAlignment.Center;
            }
            else
            {
                para.TextAlignment = TextAlignment.Right;
            }

            section.Blocks.Add(para);

            string rValue = System.Windows.Markup.XamlWriter.Save(section);

            return rValue;
        }

        internal static void GetStyleValueFromRichText(string richText, Style style)
        {
            if (String.IsNullOrEmpty(richText) || style == null)
            {
                return;
            }

            try
            {
                //Object obj = System.Windows.Markup.XamlReader.Load(richText);
                Object obj = System.Windows.Markup.XamlReader.Parse(richText);

                if (obj != null)
                {
                    TextElement line = null;
                    if (obj is Section)
                    {
                        Section section = obj as Section;

                        if (section.Blocks.Count > 0)
                        {
                            Block block = section.Blocks.FirstBlock;

                            if (block is Paragraph)
                            {
                                Paragraph para = block as Paragraph;
                                if (para.Inlines.Count > 0)
                                {
                                    line = para.Inlines.FirstInline;
                                }
                            }
                        }
                    }
                    else if (obj is Span)
                    {
                        Span span = obj as Span;
                        if (span.Inlines.Count > 0)
                        {
                            line = span.Inlines.FirstInline;
                        }
                    }

                    if (line != null)
                    {
                        // TextElement line = para.Inlines.FirstInline;

                        if (line is Run)
                        {
                            Run run = line as Run;

                            if (run.FontWeight == FontWeights.Bold)
                            {
                                style.SetStyleProperty(StylePropertyNames.BOLD_PROP, true);
                            }
                            else
                            {
                                style.SetStyleProperty(StylePropertyNames.BOLD_PROP, false);
                            }

                            if (run.FontStyle == FontStyles.Italic)
                            {
                                style.SetStyleProperty(StylePropertyNames.ITALIC_PROP, true);
                            }
                            else
                            {
                                style.SetStyleProperty(StylePropertyNames.ITALIC_PROP, false);
                            }

                            style.SetStyleProperty(StylePropertyNames.UNDERLINE_PROP, false);
                            style.SetStyleProperty(StylePropertyNames.STRIKETHROUGH_PROP, false);
                            foreach (TextDecoration dec in run.TextDecorations)
                            {
                                if (dec.Location == TextDecorationLocation.Underline)
                                {
                                    style.SetStyleProperty(StylePropertyNames.UNDERLINE_PROP, true);
                                }
                                else if (dec.Location == TextDecorationLocation.Strikethrough)
                                {
                                    style.SetStyleProperty(StylePropertyNames.STRIKETHROUGH_PROP, true);
                                }
                            }

                            style.SetStyleProperty(StylePropertyNames.FONT_SIZE_PROP, run.FontSize);
                            style.SetStyleProperty(StylePropertyNames.FONT_FAMILY_PROP, run.FontFamily.ToString());

                            Color tColor = ((SolidColorBrush)run.Foreground).Color;
                            StyleColor stylecolor = new StyleColor(ColorFillType.Solid, ((tColor.A << 24) | (tColor.R << 16) | (tColor.G << 8) | tColor.B));
                            style.SetStyleProperty(StylePropertyNames.FONT_COLOR_PROP, stylecolor);

                            // return;//only need first line style
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        internal WidgetStyle AddWidgetStyle(AdaptiveView view)
        {
            WidgetStyle style = new WidgetStyle(this, view.Guid);
            _widgetStyles.Add(style.ViewGuid, style);
            return style;
        }

        internal void DeleteWidgetStyle(AdaptiveView view)
        {
            if(view != null)
            {
                DeleteWidgetStyle(view.Guid);
            }
        }

        internal void DeleteWidgetStyle(Guid viewGuid)
        {
            _widgetStyles.Remove(viewGuid);
        }

        internal WidgetStyles WidgetStyles
        {
            get { return _widgetStyles; }
        }

        internal WidgetDefaultStyle WidgetDefaultStyle
        {
            get
            {
                if (_widgetDefaultStyle == null && ParentDocument != null && ParentDocument.IsOpened)
                {
                    _widgetDefaultStyle = ParentDocument.WidgetDefaultStyleSet.GetWidgetDefaultStyle(this) as WidgetDefaultStyle;
                }

                return _widgetDefaultStyle;
            }

            set { _widgetDefaultStyle = value; }
        }

        // Clear all view styles and make base view style has the value of the specific new base style
        internal virtual void RebuildStyleChain(WidgetStyle newBaseStyle)
        {
            // Clear all view styles
            _widgetStyles.Clear();

            // Reset base view style value.
            if (newBaseStyle != null)
            {
                Style.CopyStyle(newBaseStyle, _widgetStyle, null, null);
            }
        }

        internal virtual Guid CreatedViewGuid
        {
            get { return _createdViewGuid; }
            set { _createdViewGuid = value; }
        }

        internal virtual bool HasBeenPlacedInBaseView
        {
            get { return _hasBeenPlacedInBaseView; }
            set { _hasBeenPlacedInBaseView = value; }
        }

        internal virtual void UpdateAllGuids()
        {
            UpdateGuid();

            return;
        }

        internal virtual void UpdateActions()
        {
            foreach (InteractionEvent iEvent in _events)
            {
                iEvent.UpdateActions();
            }
        }

        internal virtual void UpdateActions(Dictionary<Guid, IObjectContainer> newTargets)
        {
            foreach (InteractionEvent iEvent in _events)
            {
                iEvent.UpdateActions(newTargets);
            }
        }

        #endregion

        #region Protected Methods

        protected abstract Dictionary<string, StyleProperty> SupportStyleProperties { get; }

        protected virtual void InitializeBaseViewStyleFromDefaultStyle()
        {
            if (ParentDocument != null && ParentDocument.HostService != null)
            {
                Style systemStyle = ParentDocument.HostService.WidgetSystemStyle as Style;
                Style.CopyStyle(systemStyle, _widgetStyle, SupportStyleProperties.Keys.ToList<string>(), null);
            }

            if (WidgetDefaultStyle != null)
            {
                Style.CopyStyle(WidgetDefaultStyle, _widgetStyle, null, null);
            }

            // Add support properties
            foreach (StyleProperty property in SupportStyleProperties.Values)
            {
                if (!_widgetStyle.Contains(property.Name))
                {
                    _widgetStyle.SetStyleProperty(property.Clone());
                }
            }
        }

        protected void InitializeAdaptiveViewStyles()
        {
            // Add adaptive view style if needed.
            if (ParentDocument != null && ParentDocument.IsOpened)
            {
                foreach (AdaptiveView view in ParentDocument.AdaptiveViewSet.AdaptiveViews)
                {
                    if (!_widgetStyles.Contains(view.Guid))
                    {
                        AddWidgetStyle(view);
                    }
                }
            }
        }

        // All widgets have OnClick event.
        private void AddWidgetSupportEvents()
        {
            InteractionEvent newEvent = new InteractionEvent(this, EventType.OnClick);
            _events.Add(EventType.OnClick, newEvent);
        }

        protected void MatchTextAndRichText()
        {
            if (!String.IsNullOrEmpty(_text) && String.IsNullOrEmpty(_richText))
            {
                _richText = BuildRichText(_text, _widgetStyle);
            }
            else if (String.IsNullOrEmpty(_text) && !String.IsNullOrEmpty(_richText))
            {
                _text = GetSimpleText(_richText);
            }
        }

        #endregion

        #region Protected Fields
        
        protected WidgetType _widgetType;
        
        protected Group _parentGroup;
        protected Guid _parentGroupGuid;
       
        protected string _text;
        protected string _richText; // Save format string for rich text
        protected string _tooltip;
        protected bool _isDisabled;
        protected bool _isLocked;

        protected WidgetStyle _widgetStyle;
        protected WidgetStyles _widgetStyles;
            
        protected Annotation _annotation;
        protected InteractionEvents _events;

        protected Guid _createdViewGuid = Guid.Empty;
        protected bool _hasBeenPlacedInBaseView = false;

        #endregion

        private WidgetDefaultStyle _widgetDefaultStyle;
    }
}
