using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Converters;

namespace Naver.Compass.WidgetLibrary
{
    public class SVGWidgetPreViewModel : WidgetPreViewModeBase
    {
        public SVGWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            IsImgConvertType = true;
        }

        #region Initialzie Override
        override protected void IniCreateDataModel(IRegion obj)
        {
            _model = new SVGModel(obj as IWidget);
        }
        #endregion

        #region  private member
        private DrawingImage _svgSource;
        private Visibility _backgroundShow = Visibility.Visible;

        private DrawingImage LoadSvg(Stream svgStream)
        {
            if (svgStream == null)
                return null;

            // Create conversion options
            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = true;
            settings.TextAsGeometry = false;

            // Create a file reader
            FileSvgReader converter = new FileSvgReader(settings);
            // Read the SVG file
            DrawingGroup drawing = converter.Read(svgStream);

            if (drawing != null)
            {
                return new DrawingImage(drawing);
            }
            return null;
        }
        #endregion

        #region Binding Prope
        public DrawingImage SVGSource
        {
            get
            {
                if (_svgSource == null)
                {
                    _svgSource = LoadSvg((_model as SVGModel).SVGStream);
                }

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
