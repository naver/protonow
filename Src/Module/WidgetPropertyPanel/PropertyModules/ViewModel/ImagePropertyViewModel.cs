using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Naver.Compass.InfoStructure;
using Naver.Compass.WidgetLibrary;
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Common.Helper;
using System.IO;
using System.Windows.Media.Imaging;

namespace Naver.Compass.Module.Property
{
    class ImagePropertyViewModel : PropertyViewModelBase
    {

        public ImagePropertyViewModel()
        {

        }


        #region Override base function

       override  protected  void OnItemsAdd()
        {
            base.OnItemsAdd();
            FirePropertyChanged("ImgSource");
        }

        override public void OnPropertyChanged(string args)
        {
            base.OnPropertyChanged(args);
            switch (args)
            {
                case "NailStream":
                    {
                        FirePropertyChanged("ImgSource");
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Binding property

        public ImageSource ImgSource
        {
            get
            {
                if (_VMItems.Count == 1)
                {
                    ImageWidgetViewModel ImageData = _VMItems[0] as ImageWidgetViewModel;
                    if (ImageData != null)
                    {
                        if (null == ImageData.NailStream)
                        {
                            return null;
                        }
                        else
                        {
                            ImageData.NailStream.Seek(0, SeekOrigin.Begin);
                            ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                            return imageSourceConverter.ConvertFrom(ImageData.NailStream) as BitmapFrame;
                        }
                        
                    }
                }

                return null;
            }
        }

        #endregion

    }
}
