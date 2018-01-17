using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsSimpleTextWidget : JsWidget
    {
        public JsSimpleTextWidget(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            string text = JsHelper.ReplaceSpecialCharacters(_widget.Text);
            text = JsHelper.ReplaceNewLineWithBRTag(text);
            builder.AppendFormat("\"text\":\"{0}\",", text);
        }

        public override void AppendSpecificTypeStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            JsHelper.AppendFontStyle(builder, widgetStyle);
            JsHelper.AppendSimpleTextStyle(builder, widgetStyle);
        }
    }
}
