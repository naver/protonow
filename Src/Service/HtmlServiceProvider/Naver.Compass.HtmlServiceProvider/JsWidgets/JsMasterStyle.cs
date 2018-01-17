using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsMasterStyle
    {
        public JsMasterStyle(IMaster master, IMasterStyle masterStyle, IPageView pageView, bool bIsSetMD5, Guid AdaptiviewID)
        {
            _master = master;
            _masterStyle = masterStyle;
            _pageView = pageView;
            IsSetMD5 = bIsSetMD5;
            _childPageView = _master.MasterPage.PageViews[AdaptiviewID];

        }
        public bool IsSetMD5 { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");

            if (_pageView.Guid != _master.ParentDocument.AdaptiveViewSet.Base.Guid)
            {
                builder.AppendFormat("\"adaptiveViewID\":\"{0}\",", _pageView.Guid);
            }
            if (IsSetMD5 == true)
            {
                builder.AppendFormat("\"MD5\":\"{0}\",", _masterStyle.MD5);
            }
            builder.AppendFormat("\"width\":\"{0}px\",", Math.Round(_masterStyle.Width));
            builder.AppendFormat("\"height\":\"{0}px\",", Math.Round(_masterStyle.Height));

            builder.AppendFormat("\"top\":\"{0}px\",", Math.Round(_masterStyle.Y));
            builder.AppendFormat("\"left\":\"{0}px\",", Math.Round(_masterStyle.X));


            builder.AppendFormat("\"offsetY\":\"{0}px\",", Math.Round(_childPageView.RegionStyle.Y));
            builder.AppendFormat("\"offsetX\":\"{0}px\",", Math.Round(_childPageView.RegionStyle.X));

            // In current design, Z-Order doesn't support adaptive view.
            builder.AppendFormat("\"z-index\":{0},", _master.MasterStyle.Z);

            if (_masterStyle.IsFixed)
            {
                builder.AppendFormat("\"position\":\"fixed\",");
            }
            else
            {
                builder.AppendFormat("\"position\":\"absolute\",");
            }

            // isPlaced flag
            bool isPlaced = true;
            if (_pageView != null)
            {
                if (!_pageView.Masters.Contains(_master.Guid))
                {
                    isPlaced = false;
                }
            }
            else
            {
                // Check base vew
                Guid baseViewGuid = _master.ParentDocument.AdaptiveViewSet.Base.Guid;
                IPageView baseView = _master.ParentPage.PageViews[baseViewGuid];
                if (baseView != null && !baseView.Masters.Contains(_master.Guid))
                {
                    isPlaced = false;
                }
            }
            builder.AppendFormat("\"isPlaced\":{0},", isPlaced.ToString().ToLower());

            string visibility = "block";
            if (!_masterStyle.IsVisible)
            {
                visibility = "none";
            }
            builder.AppendFormat("\"display\":\"{0}\",", visibility);
                        
            JsHelper.RemoveLastComma(builder);

            builder.Append("}");

            return builder.ToString();
        }

        private IMaster _master;
        private IMasterStyle _masterStyle;
        private IPageView _pageView;
        private IPageView _childPageView;
    }
}
