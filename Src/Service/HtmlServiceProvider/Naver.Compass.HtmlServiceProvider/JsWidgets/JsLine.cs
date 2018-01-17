using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsLine : JsWidget
    {
        public JsLine(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        public override void AppendSpecificTypeStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            // Set the image file hash value for styles
            string hash = _service.ImagesStreamManager.GetConsumerStreamHash(_widget.Guid, widgetStyle.ViewGuid);
            if (String.IsNullOrEmpty(hash) &&
                widgetStyle.ViewGuid == _widget.ParentDocument.AdaptiveViewSet.Base.Guid)
            {
                // If not find and it is base view, try to load again with empty view guid.
                hash = _service.ImagesStreamManager.GetConsumerStreamHash(_widget.Guid, Guid.Empty);
            }

            if (!String.IsNullOrEmpty(hash))
            {
                builder.AppendFormat("\"hash\":\"{0}\",", hash);
            }
        }
    }
}
