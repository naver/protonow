using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsTextBase : JsSimpleTextWidget
    {
        public JsTextBase(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            base.AppendSpecificTypeProperties(builder);

            ITextBase textBase = _widget as ITextBase;
            builder.AppendFormat("\"hintText\":\"{0}\",", JsHelper.ReplaceSpecialCharacters(textBase.HintText));
            builder.AppendFormat("\"hideBorder\":{0},", textBase.HideBorder.ToString().ToLower());
            builder.AppendFormat("\"readOnly\":{0},", textBase.ReadOnly.ToString().ToLower());

            if (textBase.MaxLength >= 0)
            {
                builder.AppendFormat("\"maxLength\":{0},", textBase.MaxLength);
            }
            else
            {
                // Negative value means unlimited.
                builder.AppendFormat("\"maxLength\": ,");
            }

            if (_widget.WidgetType == WidgetType.TextField)
            {
                ITextField textField = _widget as ITextField;
                builder.AppendFormat("\"textFieldType\":\"{0}\",", textField.TextFieldType.ToString().ToLower());
            }
        }

        public override void AppendSpecificTypeStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            base.AppendSpecificTypeStyle(builder, widgetStyle);

            ITextBase textBase = widgetStyle.OwnerWidget as ITextBase;
            if (textBase.HideBorder)
            {
                builder.Append("\"border-color\":\"transparent\",");
            }

            builder.AppendFormat("\"background-color\":\"{0}\",", JsHelper.GetRGBAColor(widgetStyle.FillColor));
        }
    }
}
