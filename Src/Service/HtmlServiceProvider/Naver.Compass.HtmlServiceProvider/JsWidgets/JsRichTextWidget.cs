using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsRichTextWidget : JsWidget
    {
        public JsRichTextWidget(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            string text = JsHelper.GetHtmlTextFromRichText(_widget.RichText);
            builder.AppendFormat("\"text\":\'{0}\',", text);
        }

        public override void AppendSpecificTypeStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            JsHelper.AppendRichTextStyle(builder, widgetStyle);
        }
    }
}
