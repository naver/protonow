using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    /*
    "groups" : [
         {
             "id" : "1c782d16-9cd2-496c-aed8-70cbeb37cc3d",          // Group ID.
             // Group location and size, if it is top level group, location is absolute value, otherwise it is relative value.
             // relative to top level parent group.
             "width" : x,     
             "height" : x,
             "top" : x,
             "left" : x,
             
             "adaptiveViewStyles" : [
                {
                    "adaptiveViewID" : "8b49632b-70d8-4539-9c37-930d6f4765cd",  // Adaptive view ID
                    "width" : x,     
                    "height" : x,
                    "top" : x,
                    "left" : x,
                },
                {
                    "adaptiveViewID" : "b0178080-8261-43fd-b0e5-0905c41a961d",   // Adaptive view ID 
                    // Other style data ....... 
                }
             ],
     
             "widgets" : ["886d17da-5765-403b-a0c3-959c42fc9bcc"],   // IDs of widgets in this group
             "groups" : ["1c782d16-9cd2-496c-aed8-70cbeb37cc3d"]     // IDs of groups in this group. (Sub group)
         }
      ]
     * */

    internal class JsGroup
    {
        public JsGroup(HtmlServiceProvider service, IGroup group)
        {
            _service = service;
            _group = group;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");

            builder.AppendFormat("\"id\":\"{0}\",", _group.Guid.ToString());

            if(_group.ParentGroup != null)
            {
                builder.AppendFormat("\"parentGroupID\":\"{0}\",", _group.ParentGroup.Guid.ToString());
            }

            builder.Append("\"style\":");
            JsGroupStyle groupStyle = new JsGroupStyle(_service, _group, _group.ParentDocument.AdaptiveViewSet.Base);
            builder.Append(groupStyle.ToString());
            builder.Append(",");

            builder.Append("\"adaptiveViewStyles\":[");
            foreach (IAdaptiveView view in _group.ParentDocument.AdaptiveViewSet.AdaptiveViews)
            {
                groupStyle = new JsGroupStyle(_service, _group, view);
                builder.Append(groupStyle.ToString());
                builder.Append(",");
            }

            JsHelper.RemoveLastComma(builder);
            builder.Append("],");

            // Child widgets:
            builder.Append("\"widgets\":[");
            foreach(IWidget widget in _group.Widgets)
            {
                builder.AppendFormat("\"{0}\",", widget.Guid.ToString());
            }

            JsHelper.RemoveLastComma(builder);
            builder.Append("],");


            // Child masters:
            builder.Append("\"masters\":[");
            foreach (IMaster master in _group.Masters)
            {
                builder.AppendFormat("\"{0}\",", master.Guid.ToString());
            }

            JsHelper.RemoveLastComma(builder);
            builder.Append("],");

            // Child groups:
            builder.Append("\"groups\":[");
            foreach (IGroup group in _group.Groups)
            {
                builder.AppendFormat("\"{0}\",", group.Guid.ToString());
            }

            JsHelper.RemoveLastComma(builder);
            builder.Append("]");

            builder.Append("}");

            return builder.ToString();
        }

        protected HtmlServiceProvider _service;
        protected IGroup _group;
    }
}
