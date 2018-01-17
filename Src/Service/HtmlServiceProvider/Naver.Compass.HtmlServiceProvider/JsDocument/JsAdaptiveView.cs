using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    /*
         "adaptiveViews" : [
            //Comment For Victor :: !important! This array must be sorted by width value. from small width to great width 
            {
                "id" : "b0178080-8261-43fd-b0e5-0905c41a961d",   // Adaptive view ID
                "name" : "Apple 3GS",
                "width" : 600,
                "condition" : "lessThan",            // "greaterThan", "lessThan"
                "className" : "adaptive_b0178080-8261-43fd-b0e5-0905c41a961d"           
                                                     // When condition is matched, this classname will add to HTML element.
                                                     // naming : "adaptive_" + Adpative View ID 
                                                     // Image for this widget naming : ID + "_" + className + ".png"
            },
            { 
               "id" : "8b49632b-70d8-4539-9c37-930d6f4765cd",
                name : "New Views",
                width : 800,
                condition : "lessThan",
                className : "adaptive_8b49632b-70d8-4539-9c37-930d6f4765cd"
            }
        ]

     * */

    class JsAdaptiveView
    {
        public JsAdaptiveView(HtmlServiceProvider service, IAdaptiveView view)
        {
            _service = service;
            _view = view;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("{");
            builder.AppendFormat("\"id\":\"{0}\",", _view.Guid);
            builder.AppendFormat("\"name\":\"{0}\",", JsHelper.ReplaceSpecialCharacters(_view.Name));
            builder.AppendFormat("\"width\":{0},", _view.Width);

            if (_view.Condition == AdaptiveViewCondition.LessOrEqual)
            {
                builder.AppendFormat("\"condition\":\"lessThan\",");
            }
            else
            {
                builder.AppendFormat("\"condition\":\"greaterThan\",");
            }
            builder.AppendFormat("\"className\":\"adaptive_{0}\"", _view.Guid);

            builder.Append("}");

            return builder.ToString();
        }

        private HtmlServiceProvider _service;
        private IAdaptiveView _view;
    }
}
