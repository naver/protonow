using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    class JsWidgetNotes
    {
        public JsWidgetNotes(HtmlServiceProvider service, IWidget widget)
        {
            _service = service;
            _widget = widget;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\"notes\":{");

            foreach(IAnnotationField field in _service.RenderingDocument.WidgetAnnotationFieldSet.AnnotationFields)
            {
                string text = _widget.Annotation.GetextValue(field.Name);
                if(String.IsNullOrEmpty(text))
                {
                    continue;
                }

                text = JsHelper.ReplaceSpecialCharacters(text);
                text = JsHelper.ReplaceNewLineWithBRTag(text);

                builder.AppendFormat("\"{0}\":\"{1}\",", field.Name, text);
            }

            JsHelper.RemoveLastComma(builder);

            builder.Append("}");

            return builder.ToString();
        }

        private HtmlServiceProvider _service;
        private IWidget _widget;
    }
}
