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

namespace Naver.Compass.WidgetLibrary
{
    public class ImageWidgetPreViewModel : WidgetPreViewModeBase
    {
        public ImageWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            IsImgConvertType = true;
            _imageStream = (_model as ImageModel).ImageStream;
        }

        #region Initialzie Override
        override protected void IniCreateDataModel(IRegion obj)
        {
            _model = new ImageModel(obj as IWidget);
        }
        #endregion

        #region  private member
        private bool _isNail = false;
        private Stream _imageStream = null;
        private Visibility _backgroundShow = Visibility.Visible;
        #endregion

        #region Binding Prope
        //BitmapImage _imgSource=null;
        public ImageSource ImgSource
        {
            get
            {
                try
                {
                    if (_imageStream == null)
                    {
                        //return new BitmapImage();
                        BackgroundShow = Visibility.Visible;
                        return null;
                    }

                    if (IsNail == true)
                    {
                        using (System.Drawing.Image drawingImage = System.Drawing.Image.FromStream(_imageStream))
                        {
                            if (drawingImage.Width > 100 || drawingImage.Height > 100)
                            {
                                using (System.Drawing.Image thumbImage =
                                    drawingImage.GetThumbnailImage(100, 100, () => { return true; }, IntPtr.Zero))
                                {

                                    MemoryStream ms = new MemoryStream();
                                    thumbImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                    _imageStream = ms;
                                }

                            }
                        } 
                    }
                    
                    //new image loading solution for imprvoed memory release
                    _imageStream.Seek(0, SeekOrigin.Begin);
                    BackgroundShow = Visibility.Collapsed;
                    ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                    return imageSourceConverter.ConvertFrom(_imageStream) as BitmapFrame;

                    #region old Image loading solution
                    //if (_imgSource == null)
                    //{
                        //_imgSource = new BitmapImage();
                        //_imageStream.Seek(0, SeekOrigin.Begin);
                        //_imgSource.BeginInit();
                        //_imgSource.StreamSource = _imageStream;
                        //_imgSource.EndInit();
                        //BackgroundShow = Visibility.Collapsed;
                        //Debug.WriteLine("??????????????IMAGE LOAD:" + _imgSource.StreamSource.Position);
                    //}                    
                    //return _imgSource;
                    #endregion
                }
                catch (System.Exception ex)
                {
                    return null;
                }                
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
        public double Opacity
        {
            get
            {
                return (_model as ImageModel).Opacity;
            }
            set
            {
                if ((_model as ImageModel).Opacity != value)
                {
                    value = Math.Max(0, value);
                    (_model as ImageModel).Opacity = value;
                    FirePropertyChanged("Opacity");
                }
            }
        }
        #endregion

        public  bool IsNail
        {
            get
            {
                return _isNail;
            }
            set
            {
                if (_isNail != value)
                {
                    _isNail = value;
                }
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
