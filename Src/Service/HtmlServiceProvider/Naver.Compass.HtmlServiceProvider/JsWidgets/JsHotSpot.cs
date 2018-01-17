using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsHotSpot : JsWidget
    {
        public JsHotSpot(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        public override void AppendSpecificTypeStyle(StringBuilder builder, IWidgetStyle _widgetStyle)
        {
            builder.Append("\"background-image\":\"url(../../resources/images/transparent.gif)\",");
            return;
        }
    }
}
