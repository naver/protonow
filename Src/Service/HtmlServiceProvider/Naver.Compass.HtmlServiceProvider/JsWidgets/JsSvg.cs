using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsSvg : JsStreamWidget
    {
        public JsSvg(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            base.AppendSpecificTypeProperties(builder);

            if(!String.IsNullOrEmpty(_hash))
            {
                builder.AppendFormat("\"hash\":\"{0}\",", _hash);
            }
        }

        protected override string StreamType
        {
            get { return "svg"; }
        }

        protected override string DefaultResourceName
        {
            get { return string.Empty; }
        }

        protected override string DefaultStreamType
        {
            get { return "svg"; }
        }
    }
}
