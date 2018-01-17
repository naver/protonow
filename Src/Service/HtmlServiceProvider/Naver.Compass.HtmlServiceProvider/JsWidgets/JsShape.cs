using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsShape : JsRichTextWidget
    {
        public JsShape(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }
        
        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            // Generate text only exporting to svg
            if (_service.RenderingDocument.GeneratorConfigurationSet.DefaultHtmlConfiguration.ExportType == ExportType.Data)
            {
                base.AppendSpecificTypeProperties(builder);
            }

            IShape shape = _widget as IShape;
            builder.AppendFormat("\"shapeType\":\"{0}\",", shape.ShapeType.ToString());
            if (_service.RenderingDocument.GeneratorConfigurationSet.DefaultHtmlConfiguration.ExportType == ExportType.Data)
            {
                builder.AppendFormat("\"shapeFormat\":\"svg\","); // Viewer will create svg based on js data.
            }
            else
            {
                builder.Append("\"shapeFormat\":\"png\",");
            }
        }

        public override void AppendSpecificTypeStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            // Shape style only works when we export to a svg file.
            if (_service.RenderingDocument.GeneratorConfigurationSet.DefaultHtmlConfiguration.ExportType == ExportType.Data)
            {
                base.AppendSpecificTypeStyle(builder, widgetStyle);

                builder.Append("\"fill\": {");
                JsHelper.AppendFillStyle(builder, widgetStyle);
                JsHelper.RemoveLastComma(builder);
                builder.Append("},");

                builder.Append("\"stroke\": {");
                JsHelper.AppendBorderStyle(builder, widgetStyle);
                JsHelper.RemoveLastComma(builder);
                builder.Append("},");

                builder.AppendFormat("\"border-radius\":{0},", widgetStyle.CornerRadius);
            }
            else
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
}
