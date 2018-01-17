using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsImage : JsStreamWidget
    {
        public JsImage(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
            IImage image = _widget as IImage;
            if (image.ImageStream != null)
            {
                _jsImageType = image.ImageType.ToString().ToLower();
            }
            else
            {
                // Use default svg file if customer doesn't import image.
                _jsImageType = "svg";
            }
        }

        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            base.AppendSpecificTypeProperties(builder);

            builder.AppendFormat("\"imageType\":\"{0}\",", _jsImageType);
            
            if (!String.IsNullOrEmpty(_hash))
            {
                builder.AppendFormat("\"hash\":\"{0}\",", _hash);
            }
        }

        protected override string StreamType
        {
            get
            {
                return _jsImageType;
            }
        }

        protected override string DefaultResourceName
        {
            get { return @"Naver.Compass.Service.Html.Res.01_Image_Select.svg"; }
        }

        protected override string DefaultStreamType
        {
            get { return "svg"; }
        }

        private string _jsImageType;
    }
}
