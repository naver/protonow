using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsViewport
    {
        public JsViewport(HtmlServiceProvider service)
        {
            _service = service;
        }

        public override string ToString()
        {
            IViewport viewport = _service.RenderingDocument.GeneratorConfigurationSet.DefaultHtmlConfiguration.Viewport;

            // Do not generate viewport if width is less than 0.
            if (viewport.IncludeViewportTag == false || viewport.Width <= 0)
            {
                return "";
            }

            // Build adaptive view which width has value.
            StringBuilder builder = new StringBuilder();

            builder.Append("\"mobileViewport\":{");

            builder.AppendFormat("\"name\":\"{0}\",", JsHelper.ReplaceSpecialCharacters(viewport.Name));
            builder.AppendFormat("\"width\":{0},", viewport.Width);
            builder.AppendFormat("\"height\":{0}", viewport.Height);

            builder.Append("}");

            return builder.ToString();
        }

        private HtmlServiceProvider _service;
    }
}
