using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class ImageModel : WidgetModel
    {
        public ImageModel(IWidget widget):base(widget)
        {
            _image = widget as IImage;
            if (widget is IHamburgerMenu)
            {
                IHamburgerMenu menu = widget as IHamburgerMenu;
                _image = menu.MenuButton;
            }
            else
            {
                _image = widget as IImage;
            }
            return;
        }

        #region private member
        private IImage _image=null;
        #endregion private member

        #region public base property for binding
        virtual public Stream ImageStream
        {
            get 
            {
                return _image.ImageStream;
            }
            set
            {
                _image.ImageStream = value;
                _document.IsDirty = true;
            }
        }
        public ImageType ImageType
        {
            get
            {
                return _image.ImageType;
            }
            set
            {
               if( _image.ImageType != value)
               {
                   _image.ImageType=value;
                   _document.IsDirty = true;
               }
                
            }
        }
        //public double Opacity
        //{
        //    get
        //    {
        //        return Convert.ToDouble(_image.WidgetStyle.Opacity)/100;
        //    }
        //    set
        //    {
        //        if (_image.WidgetStyle.Opacity != (value*100))
        //        {
        //            _document.IsDirty = true;
        //            _image.WidgetStyle.Opacity = Convert.ToInt32(value*100);
        //        }
        //    }
        //}
        #endregion public member
    }
}
