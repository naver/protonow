using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsListBase : JsSimpleTextWidget
    {
        public JsListBase(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            if (_widget.WidgetType == WidgetType.ListBox)
            {
                IListBox listBox = _widget as IListBox;
                builder.AppendFormat("\"allowMultiple\":{0},", listBox.AllowMultiple.ToString().ToLower());
            }

            IListBase listBase = _widget as IListBase;

            builder.Append("\"items\":[");

            foreach (IListItem item in listBase.Items)
            {
                builder.Append("{");

                builder.AppendFormat("\"textValue\":\"{0}\",", JsHelper.ReplaceSpecialCharacters(item.TextValue));
                builder.AppendFormat("\"isSelected\":{0},", item.IsSelected.ToString().ToLower());
                    
                builder.Append("},");
            }

            JsHelper.RemoveLastComma(builder);

            builder.Append("],");
        }

        public override void AppendSpecificTypeStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            base.AppendSpecificTypeStyle(builder, widgetStyle);

            builder.AppendFormat("\"background-color\":\"{0}\",", JsHelper.GetRGBAColor(widgetStyle.FillColor));
        }
    }
}
