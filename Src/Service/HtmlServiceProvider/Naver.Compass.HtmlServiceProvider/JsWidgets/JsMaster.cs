using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsMaster
    {
        public JsMaster(HtmlServiceProvider service, IMaster master,bool bIsSetMD5)
        {
            _service = service;
            _master = master;
            IsSetMD5 = bIsSetMD5;
        }
        public bool IsSetMD5 { get; set; }

        public override string ToString()
        {
            //this is just for debug test, delete it later      
            if(_master.ParentPage is IDocumentPage)
            {
               // System.Diagnostics.Debug.WriteLine("====Master md5:" + _master.MD5 + "  ===========================");
            }
            

            StringBuilder builder = new StringBuilder();
            builder.Append("{");

            builder.AppendFormat("\"id\":\"{0}\",", _master.Guid.ToString());
            if (IsSetMD5 == true)
            {
                builder.AppendFormat("\"MD5\":\"{0}\",", _master.MD5);
            }

            builder.AppendFormat("\"masterPageID\":\"{0}\",", _master.MasterPageGuid.ToString());

            if (_master.ParentGroup != null)
            {
                builder.AppendFormat("\"parentGroupID\":\"{0}\",", _master.ParentGroup.Guid.ToString());
            }

            builder.Append("\"style\":");
            IPageView basePageView = _master.ParentPage.PageViews[_master.ParentDocument.AdaptiveViewSet.Base.Guid];
            JsMasterStyle masterStyle = new JsMasterStyle(_master, _master.MasterStyle, basePageView, IsSetMD5, _master.ParentDocument.AdaptiveViewSet.Base.Guid);
            builder.Append(masterStyle.ToString());
            builder.Append(",");

            builder.Append("\"adaptiveViewStyles\":[");
            foreach (IAdaptiveView view in _master.ParentDocument.AdaptiveViewSet.AdaptiveViews)
            {
                IMasterStyle masterViewStyle = _master.GetMasterStyle(view.Guid);
                IPageView pageView = _master.ParentPage.PageViews[view.Guid];
                if (masterViewStyle != null && pageView != null)
                {
                    masterStyle = new JsMasterStyle(_master, masterViewStyle, pageView, IsSetMD5,view.Guid);
                    builder.Append(masterStyle.ToString());
                    builder.Append(",");
                }
            }

            JsHelper.RemoveLastComma(builder);
            builder.Append("]");

            builder.Append("}");

            return builder.ToString();
        }

        protected HtmlServiceProvider _service;
        protected IMaster _master;
    }
}
