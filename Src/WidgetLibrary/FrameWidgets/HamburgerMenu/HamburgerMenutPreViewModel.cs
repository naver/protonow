using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Naver.Compass.WidgetLibrary
{
    public class HamburgerMenutPreViewModel : WidgetPreViewModeBase
    {
        public HamburgerMenutPreViewModel(IWidget widget)
            : base(widget)
        {
            //Infra Structure
            _model = new HamburgerMenuModel(widget);
            IsImgConvertType = true;
        }
        public double MenuPageLeft
        {
            get
            {
                return (_model as HamburgerMenuModel).MenuPageLeft;
            }
            set
            {
                if ((_model as HamburgerMenuModel).MenuPageLeft != value)
                {
                    (_model as HamburgerMenuModel).MenuPageLeft = value;
                    FirePropertyChanged("MenuPageLeft");
                }
            }
        }
        public double MenuPageTop
        {
            get
            {
                return (_model as HamburgerMenuModel).MenuPageTop;
            }
            set
            {
                if ((_model as HamburgerMenuModel).MenuPageTop != value)
                {
                    (_model as HamburgerMenuModel).MenuPageTop = value;
                    FirePropertyChanged("MenuPageTop");
                }
            }
        }
        public double MenuPageWidth
        {
            get
            {
                return (_model as HamburgerMenuModel).MenuPageWidth;
            }
            set
            {
                if ((_model as HamburgerMenuModel).MenuPageWidth != value)
                {
                    (_model as HamburgerMenuModel).MenuPageWidth = value;
                    FirePropertyChanged("MenuPageWidth");
                }
            }
        }
        public double MenuPageHeight
        {
            get
            {
                return (_model as HamburgerMenuModel).MenuPageHeight;
            }
            set
            {
                if ((_model as HamburgerMenuModel).MenuPageHeight != value)
                {
                    (_model as HamburgerMenuModel).MenuPageHeight = value;
                    FirePropertyChanged("MenuPageHeight");
                }
            }
        }
        public ImageSource ImgSource
        {
            get
            {
                
                HamburgerMenuModel hamburget = _model as HamburgerMenuModel;

                if (hamburget.ImageStream == null)
                {
                    BackgroundShow = Visibility.Visible;
                    return null;
                }

                _imgSource = new BitmapImage();
                _imgSource.BeginInit();
                _imgSource.StreamSource = hamburget.ImageStream;
                _imgSource.EndInit();
                BackgroundShow = Visibility.Collapsed;
                
                return _imgSource;
            }
            set
            {
                FirePropertyChanged("ImgSource");
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

        private BitmapImage _imgSource;
        private Visibility _backgroundShow = Visibility.Visible;

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
