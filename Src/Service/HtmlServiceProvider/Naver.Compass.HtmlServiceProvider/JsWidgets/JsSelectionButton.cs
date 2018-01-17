using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsSelectionButton : JsSimpleTextWidget
    {
        public JsSelectionButton(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            base.AppendSpecificTypeProperties(builder);

            ISelectionButton select = _widget as ISelectionButton;
            builder.AppendFormat("\"isSelected\":{0},", select.IsSelected.ToString().ToLower());
            builder.AppendFormat("\"alignButton\":\"{0}\",", select.AlignButton.ToString().ToLower());

            if (_widget.WidgetType == WidgetType.RadioButton)
            {
                IRadioButton radio = _widget as IRadioButton;
                builder.AppendFormat("\"radioGroup\":\"{0}\",", JsHelper.ReplaceSpecialCharacters(radio.RadioGroup));
            }
        }

        public override void AppendSpecificTypeStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            base.AppendSpecificTypeStyle(builder, widgetStyle);

            builder.Append("\"word-wrap\":\"break-word\",");
        }
    }
}
