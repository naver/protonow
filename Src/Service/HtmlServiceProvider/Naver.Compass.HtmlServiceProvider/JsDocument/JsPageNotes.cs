using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    class JsPageNotes
    {
        public JsPageNotes(HtmlServiceProvider service, IPage page)
        {
            _service = service;
            _page = page;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\"pageNotes\":");

            string text = "";
            foreach(IAnnotationField field in _service.RenderingDocument.PageAnnotationFieldSet.AnnotationFields)
            {
                text += _page.Annotation.GetextValue(field.Name);
                if(String.IsNullOrEmpty(text))
                {
                    continue;
                }
                text += Environment.NewLine;
            }

            //remove last NewLine by viewer's request
            if(text.EndsWith(Environment.NewLine))
            {
                text = text.Remove(text.LastIndexOf(Environment.NewLine), Environment.NewLine.Count());
            }

            text = JsHelper.ReplaceSpecialCharacters(text);
            text = JsHelper.ReplaceNewLineWithBRTag(text);

            builder.AppendFormat("\"{0}\"", text);

            return builder.ToString();
        }

        private HtmlServiceProvider _service;
        private IPage _page;
    }
}
