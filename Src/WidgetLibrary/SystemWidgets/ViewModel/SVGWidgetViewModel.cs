using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.Helper;
using System.Diagnostics;

using UndoCompositeCommand = Naver.Compass.InfoStructure.CompositeCommand;
using System.Collections.Generic;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Converters;
using SharpVectors.Dom.Svg;
using System.Xml;
using SharpVectors.Renderers.Utils;
using Naver.Compass.Common.CommonBase;
using SharpVectors.Dom.Css;

namespace Naver.Compass.WidgetLibrary
{
    public class SVGWidgetViewModel : WidgetRotateViewModBase
    {
        public SVGWidgetViewModel()
        {
           // SvgStrech = Stretch.Fill;
        }
        public SVGWidgetViewModel(IWidget widget)
        {
            _model = new SVGModel(widget);

            _bSupportBorder = false;
            _bSupportBackground = false;
            _bSupportText = false;
            _bSupportTextVerAlign = false;
            _bSupportTextHorAlign = false;
            widgetGID = widget.Guid;
            Type = ObjectType.SVG;
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = true;
            _bSupportTextRotate = false;

            _BackgroundColor = new StyleColor(ColorFillType.Solid, 0);
            _StrokeColor = new StyleColor(ColorFillType.Solid, 0);

            // Create conversion options and a file reader
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = true;
            settings.TextAsGeometry = false;
            _converter = new FileSvgReader(settings);
            SVGSource = LoadSvg((_model as SVGModel).SVGStream);    


        }
        
        #region  private member
        protected Stream _imageStream = null;
        protected ISvgDocument DomDocument = null;

        private FileSvgReader _converter;
        private Visibility _backgroundShow = Visibility.Collapsed;

        private StyleColor _BackgroundColor ;
        private StyleColor _StrokeColor ;
        private double _apectratio = 1;

        private DrawingImage LoadSvg(Stream svgStream)
        {
            if (svgStream == null)
                return null;          
            
            // Read the SVG file
            DrawingGroup drawing = _converter.Read(svgStream);  
            if (drawing != null)
            {
                DomDocument = _converter.DomDocument;
                ISvgSvgElement root = DomDocument.RootElement;
                Color? fillColor= GetColor(root,@"fill");
                Color? strokeColor= GetColor(root,@"stroke");
                if (fillColor != null)
                {
                    _BackgroundColor.ARGB = CommonFunction.CovertColorToInt(fillColor.Value);
                    _bSupportBackground = true;
                    FirePropertyChanged("vBackgroundColor");
                }

                if (strokeColor != null)
                {
                    _StrokeColor.ARGB = CommonFunction.CovertColorToInt(strokeColor.Value);
                    _bSupportBorder = true;
                    FirePropertyChanged("vBorderLineColor");
                }
                            

                string ratio =  root.GetAttribute("preserveAspectRatio");
                if(string.IsNullOrEmpty(ratio))
                {
                    root.SetAttribute("preserveAspectRatio", "none");
                    GetApectRatio(root.GetAttribute("viewBox"));
                }

                MemoryStream stream = new MemoryStream();
                (DomDocument as SvgDocument).Save(stream);
                (_model as SVGModel).SVGUpdateStream = stream;
                return new DrawingImage(drawing);
            }
            else 
            {
                DomDocument = null;
                _bSupportBackground = false;
                _bSupportBorder = false;
                FirePropertyChanged("vBackgroundColor");
                FirePropertyChanged("vBorderLineColor");
                return null;
            }            
        }

        private void GetApectRatio(string viewBox)
        {
            try
            {
                char[] separator = { 'o', ' ', ';' };
                string[] values = viewBox.Split(separator);
                double width;
                double.TryParse(values[2], out width);
                double height;
                double.TryParse(values[3], out height);
                _apectratio = width / height;
            }
            catch
            {
                _apectratio = 1;
            }
            
            if (_apectratio > 1)
            {
                ItemWidth = 150;
                ItemHeight = 150 / _apectratio;
            }
            else
            {
                ItemWidth = 150 * _apectratio;
                ItemHeight = 150;
            }
        }
        private void SetSVGFillColor(Color fillColor)
        {
            if (SVGSource == null || DomDocument == null || DomDocument.RootElement == null)
            {
                return;
            }

            //Set DOM
            setFillColor(DomDocument.RootElement, fillColor,@"fill");

            //Create the new stream
            MemoryStream stream = new MemoryStream();
            (DomDocument as SvgDocument).Save(stream);
            (_model as SVGModel).SVGStream = stream;
            
            //Set New Drawing Image          
            _converter.WfRenderer.Render(DomDocument);
            SVGSource = new DrawingImage(_converter.WfRenderer.Drawing);

        }
        private void SetSVGStrokeColor(Color fillColor)
        {
            if (SVGSource == null || DomDocument == null || DomDocument.RootElement == null)
            {
                return;
            }

            //Set DOM
            setFillColor(DomDocument.RootElement, fillColor,@"stroke");

            //Create the new stream
            MemoryStream stream = new MemoryStream();
            (DomDocument as SvgDocument).Save(stream);
            (_model as SVGModel).SVGStream = stream;

            //Set New Drawing Image          
            _converter.WfRenderer.Render(DomDocument);
            SVGSource = new DrawingImage(_converter.WfRenderer.Drawing);

        }

        private Color? GetColor(ISvgElement rootNode, string property)
        {
            if (rootNode.RenderingHint == SvgRenderingHint.Text || rootNode.RenderingHint == SvgRenderingHint.Shape)
            {
                Color? fillColor = GetNodeColor(rootNode, property);
                if (fillColor != null)
                {
                    return fillColor;
                }
            }

            foreach (XmlNode node in rootNode.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                ISvgElement element = node as ISvgElement;
                if (element != null)
                {
                    Color? fillColor = GetColor(element, property);
                    if (fillColor != null)
                    {
                        return fillColor;
                    }
                }
            }
            return null;
        }
        private Color? GetNodeColor(ISvgElement targetNode, string property)
        {
            //StyleColor fillColor = new StyleColor();
            //fillColor.FillType = ColorFillType.Solid;
            string szRGB=null;
            string szOpacity = null;
            string szNodeopacity = null;


            if (targetNode.RenderingHint == SvgRenderingHint.Text || targetNode.RenderingHint == SvgRenderingHint.Shape)
            {
                szRGB = (targetNode as SvgElement).GetComputedStyle("").GetPropertyValue(property);
                szOpacity = (targetNode as SvgElement).GetComputedStyle("").GetPropertyValue(property+@"-opacity");
                szNodeopacity = (targetNode as SvgElement).GetComputedStyle("").GetPropertyValue("opacity");
                
                if (string.IsNullOrEmpty(szRGB))
                {
                    return null;
                }
            }
            
            //Get RGB Color
            SvgPaint paint = new SvgPaint(szRGB);
            if (paint.RgbColor == null)
            {
                return null;
            }
            Color? solidColor = WpfConvert.ToColor(paint.RgbColor);
            if (solidColor == null)
            {               
                return null;
            }
            
            //Get Aplha
            Color result = solidColor.Value;
            if (szNodeopacity != null || szOpacity != null)
            {
                double opacityValue = 1;
                if (szNodeopacity != null && szNodeopacity.Length > 0)
                {
                
                    opacityValue *= SvgNumber.ParseNumber(szNodeopacity);
                }
                else if (szOpacity != null && szOpacity.Length > 0)
                {
                    opacityValue *= SvgNumber.ParseNumber(szOpacity);
                }
                opacityValue = Math.Min(opacityValue, 1);
                opacityValue = Math.Max(opacityValue, 0);
                result.A = Convert.ToByte(opacityValue * 255);
            }
            return result;
        }
        private void setFillColor(ISvgElement rootNode, Color fillColor, string property)
        {
            if (rootNode == null)
            {
                return;
            }

            if (rootNode.RenderingHint == SvgRenderingHint.Text || rootNode.RenderingHint == SvgRenderingHint.Shape)
            {
                setNodeFillColor(rootNode, fillColor, property);
            }

            foreach (XmlNode node in rootNode.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                ISvgElement element = node as ISvgElement;
                if (element != null)
                {
                    setFillColor(element, fillColor, property);
                }
            }

        }
        private void setNodeFillColor(ISvgElement targetNode, Color fillColor, string property)
        {
            string szRGB = null,szOpacity = null,szNodeopacity = null;
            if (targetNode.RenderingHint == SvgRenderingHint.Text || targetNode.RenderingHint == SvgRenderingHint.Shape)
            {
                szRGB = (targetNode as SvgElement).GetComputedStyle("").GetPropertyValue(property);
                

                //Special handling: if fill color is white and it is inner node, don't change it.
                //add id for svgs in Social/Gesture/Style BG. need to check svg if load new svg folder.
                if (string.IsNullOrEmpty(szRGB) || (szRGB == "rgb(255,255,255)" && !string.IsNullOrEmpty((targetNode as SvgElement).Id)))
                {
                    return;
                }

                //Get RGB Color
                SvgPaint paint = new SvgPaint(szRGB);
                if (paint.RgbColor == null)
                {
                    return;
                }
            }

            //Set RGB Color
            CssCollectedStyleDeclaration All = (targetNode as SvgElement).GetComputedStyle("") as CssCollectedStyleDeclaration;
            CssStyleSheetType origin = All.GetPropertyOrigin(property);
            switch (origin)
            {
                //For those default fill color is black,and has no this attribute.Add and set it. 
                case CssStyleSheetType.Unknown:
                case CssStyleSheetType.NonCssPresentationalHints:
                    {
                        string value = string.Format("rgb({0},{1},{2})", fillColor.R, fillColor.G, fillColor.B);
                        targetNode.SetAttribute(property, value);
                        break;
                    }
                case CssStyleSheetType.Inline:
                    {
                        string value = string.Format("rgb({0},{1},{2})", fillColor.R, fillColor.G, fillColor.B);
                        SvgStyleableElement styleNode = targetNode as SvgStyleableElement;
                        styleNode.Style.SetProperty(property, value, string.Empty);
                        targetNode.SetAttribute("style", styleNode.Style.CssText);
                        break;
                    }
                case CssStyleSheetType.Author:
                    {
                        break;
                    }
            }

            //Set Alpha value
            szOpacity = (targetNode as SvgElement).GetComputedStyle("").GetPropertyValue(property+@"-opacity");
            szNodeopacity = (targetNode as SvgElement).GetComputedStyle("").GetPropertyValue("opacity");
            if (szNodeopacity == null && szOpacity == null)
            {
                if(fillColor.A==255)
                {
                    return;
                }

                string value = string.Format("{0}", Convert.ToDouble((double)fillColor.A / (double)255));
                targetNode.SetAttribute(property + @"-opacity", value);
            }
            else
            {
                string Oproperty=null;
                if (szNodeopacity != null && szNodeopacity.Length > 0)
                {                
                    Oproperty="opacity";
                }
                else 
                {
                    Oproperty = property + @"-opacity";
                }


                origin = All.GetPropertyOrigin(Oproperty);
                switch (origin)
                {
                    case CssStyleSheetType.NonCssPresentationalHints:
                        {
                            string value = string.Format("{0}", Convert.ToDouble((double)fillColor.A /(double) 255));
                            targetNode.SetAttribute(Oproperty, value);
                            break;
                        }
                    case CssStyleSheetType.Unknown:
                        {
                            string value = string.Format("{0}", Convert.ToDouble((double)fillColor.A / (double)255));
                            targetNode.SetAttribute(Oproperty, value);
                            break;
                        }
                    case CssStyleSheetType.Inline:
                        {
                            string value = string.Format("{0}", Convert.ToDouble((double)fillColor.A / (double)255));
                            SvgStyleableElement styleNode = targetNode as SvgStyleableElement;
                            styleNode.Style.SetProperty(Oproperty, value, string.Empty);
                            targetNode.SetAttribute("style", styleNode.Style.CssText);
                            break;
                        }
                    case CssStyleSheetType.Author:
                        {
                            break;
                        }
                }

            }
            
        }
        #endregion

        #region public member
        public void ImportSvg(Stream svgStream)
        {
            if (svgStream == null)
                return;

            SVGSource = LoadSvg(svgStream);
            //(_model as SVGModel).SVGStream = svgStream;
        }
        #endregion

        #region Binding Propery
        private DrawingImage _svgSource;
        public DrawingImage SVGSource
        {
            get
            {
                if (_svgSource == null)
                {
                    BackgroundShow = Visibility.Visible; 
                }
                else
                {
                    BackgroundShow = Visibility.Collapsed;
                }
                return _svgSource;
            }
            set
            {
                if (value != null)
                {
                    _svgSource = value;
                }
                FirePropertyChanged("SVGSource");
            }
        }
        public Visibility BackgroundShow
        {
            get { return _backgroundShow; }
            set
            {
                if (_backgroundShow != value)
                {
                    _backgroundShow = value;
                    FirePropertyChanged("BackgroundShow");
                }
            }
        }
        #endregion

        public override StyleColor vBackgroundColor
        {
            get
            {
                return _BackgroundColor;
            }
            set
            {
                if (value.FillType == ColorFillType.Solid)
                {
                    _BackgroundColor = value;
                    Color fillColor = CommonFunction.CovertIntToColor(value.ARGB);
                    SetSVGFillColor(fillColor);
                }
                else
                {
                    //implement gradient color later.......
                    return;
                }
                FirePropertyChanged("vBackgroundColor");
            }
        }
        public override StyleColor vBorderLineColor
        {
            get
            {
                return _StrokeColor;
            }
            set
            {
                if (value.FillType == ColorFillType.Solid)
                {
                    _StrokeColor = value;
                    Color strokeColor = CommonFunction.CovertIntToColor(value.ARGB);
                    SetSVGStrokeColor(strokeColor);
                }
                else
                {
                    //implement gradient color later.......
                    return;
                }
                FirePropertyChanged("vBorderLineColor");
            }
        }

        #region Override UpdateWidgetStyle2UI Functions
        override protected void UpdateWidgetStyle2UI()
        {

            base.UpdateWidgetStyle2UI();
            //UpdateTextStyle();
            //UpdateFontStyle();
            //UpdateBackgroundStyle();
        }
        #endregion 

    }
}
