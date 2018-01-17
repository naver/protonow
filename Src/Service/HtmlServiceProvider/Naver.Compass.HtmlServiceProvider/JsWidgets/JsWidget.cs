using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    /*
    "widgets" : [
        {
            "id" : "886d17da-5765-403b-a0c3-959c42fc9bcc",
            "type" : "Shape",
            // Other properties as current definitions ....

            "style" : {},                      // Widget style in Base view or there is no defined adaptive view. See below.
            "adaptiveViewStyles" : [],         // Widget styles array in every defined adaptive views. See below.
            "actions" : {}
        }
    ] 

    "adaptiveViewStyles" : [
        {
            "adaptiveViewID" : "8b49632b-70d8-4539-9c37-930d6f4765cd",  // Adaptive view ID
            // Below data is the same in "style"
            "width" : "20px",
            "height" : "510px",
            "top" : "470px",
            "left" : "100px"
            // Other style data .......  
        },
        {
            "adaptiveViewID" : "b0178080-8261-43fd-b0e5-0905c41a961d",   // Adaptive view ID 
            // Other style data ....... 
        }
    ]
     * */

    internal class JsWidget
    {
        public JsWidget(HtmlServiceProvider service, IWidget widget)
        {
            _service = service;
            _widget = widget;
        }
        public bool IsSetMD5{get;set;}

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");

            builder.AppendFormat("\"id\":\"{0}\",", _widget.Guid.ToString());
            if (IsSetMD5 == true)
            {
                builder.AppendFormat("\"MD5\":\"{0}\",", _widget.MD5);
            }

            // They js use "Flicking" if widget is DynamicPanel
            if (_widget.WidgetType == WidgetType.DynamicPanel)
            {
                builder.Append("\"type\":\"Flicking\",");
            }
            else
            {
                builder.AppendFormat("\"type\":\"{0}\",", _widget.WidgetType.ToString());
            }

            string name = JsHelper.ReplaceSpecialCharacters(_widget.Name);
            name = JsHelper.ReplaceNewLineWithBRTag(name);
            builder.AppendFormat("\"name\":\"{0}\",", name);

            string toolTip = JsHelper.ReplaceSpecialCharacters(_widget.Tooltip);
            toolTip = JsHelper.ReplaceNewLineWithBRTag(toolTip);
            builder.AppendFormat("\"toolTip\":\"{0}\",", toolTip);

            builder.AppendFormat("\"isDisabled\":{0},", _widget.IsDisabled.ToString().ToLower());
            
            AppendSpecificTypeProperties(builder);

            if (_widget.Annotation.IsEmpty == false)
            {
                JsWidgetNotes notes = new JsWidgetNotes(_service, _widget);
                builder.Append(notes.ToString());

                builder.Append(",");
            }

            builder.Append("\"style\":");
            IPageView basePageView = _widget.ParentPage.PageViews[_widget.ParentPage.ParentDocument.AdaptiveViewSet.Base.Guid];
            JsWidgetStyle jsWidgetStyle = new JsWidgetStyle(this, _widget.WidgetStyle, basePageView, IsSetMD5);
            builder.Append(jsWidgetStyle.ToString());
            builder.Append(",");

            //this is just for debug test, delete it later           
            //string szhash =MD5HashManager.GetHash(_widget,true);
            if (_widget.ParentPage is IDocumentPage)
            {
               // System.Diagnostics.Debug.WriteLine("====widget md5:" + _widget.MD5 + "  ===========================");
            }

            builder.Append("\"adaptiveViewStyles\":[");
            foreach (IAdaptiveView view in _widget.ParentPage.ParentDocument.AdaptiveViewSet.AdaptiveViews)
            {
                IWidgetStyle widgetViewStyle = _widget.GetWidgetStyle(view.Guid);
                IPageView pageView = _widget.ParentPage.PageViews[view.Guid];
                if (widgetViewStyle != null && pageView != null)
                {
                    JsWidgetStyle jsWidgetViewStyle = new JsWidgetStyle(this, widgetViewStyle, pageView, IsSetMD5);
                    builder.Append(jsWidgetViewStyle.ToString());
                    builder.Append(",");
                }
            }
            JsHelper.RemoveLastComma(builder);
            builder.Append("],");

            JsEvents events = new JsEvents(_service, _widget.Events);
            builder.Append(events.ToString());

            JsHelper.RemoveLastComma(builder);

            builder.Append("}");

            return builder.ToString();
        }

        public HtmlServiceProvider HtmlService
        {
            get { return _service; }
        }

        public virtual void AppendSpecificTypeStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            return;
        }

        protected virtual void AppendSpecificTypeProperties(StringBuilder builder)
        {
            return;
        }

        protected HtmlServiceProvider _service;
        protected IWidget _widget;
    }
}
