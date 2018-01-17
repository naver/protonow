using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsDynamicPanelState
    {
        public JsDynamicPanelState(HtmlServiceProvider service, IPanelStatePage page,bool bIsSetMD5)
        {
            _service = service;
            _page = page;
            _IsSetMD5 = bIsSetMD5;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");

            foreach (IWidget widget in _page.Widgets)
            {
                JsWidget jsWidget = JsWidgetFactory.CreateJsWidget(_service, widget,_IsSetMD5);
                builder.Append(jsWidget.ToString());
                builder.Append(",");
            }

            JsHelper.RemoveLastComma(builder);

            builder.Append("]");

            return builder.ToString();
        }

        private HtmlServiceProvider _service;
        private IPanelStatePage _page;
        private bool _IsSetMD5;
    }

    internal class JsDynamicPanel : JsWidget
    {
        public JsDynamicPanel(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            // Following uncommented codes are not supported in current HTML export.
            IDynamicPanel panel = _widget as IDynamicPanel;
            //builder.AppendFormat("\"bUseCircular\":{0},", panel.IsCircular.ToString().ToLower());
            builder.AppendFormat("\"bAutomatic\":{0},", panel.IsAutomatic.ToString().ToLower());
            builder.AppendFormat("\"automaticIntervalTime\":{0},", panel.AutomaticIntervalTime.ToString());
            builder.AppendFormat("\"durationTime\":{0},", panel.DurationTime.ToString());
            //builder.AppendFormat("\"navigationType\":\"{0}\",", panel.NavigationType.ToString());
            if (panel.NavigationType == NavigationType.Dot)
            {
                builder.AppendFormat("\"bUseNavigation\":true,");
            }
            else
            {
                builder.AppendFormat("\"bUseNavigation\":false,");
            }
            //builder.AppendFormat("\"bShowAffordanceArrow\":{0},", panel.ShowAffordanceArrow.ToString().ToLower());

            // Confirmed with Eunyoung, if it is full mode. the value is "100%": "panelWidth" : "100%".
            // And if it is other mode and user set panel width as 80%, the value is "80%" : "panelWidth" : "80%".
            // Must append "%" after the value in js.
            builder.AppendFormat("\"viewMode\":\"{0}\",", panel.ViewMode.ToString());
            switch(panel.ViewMode)
            {
                case DynamicPanelViewMode.Card:
                case DynamicPanelViewMode.Preview:
                case DynamicPanelViewMode.Scroll:
                    builder.AppendFormat("\"panelWidth\":\"{0}%\",", panel.PanelWidth);
                    builder.AppendFormat("\"panelSpacing\":\"{0}px\",", panel.LineWith);
                    break;
                default: //Default is Full mode
                    builder.AppendFormat("\"panelWidth\":\"100%\",");
                    builder.AppendFormat("\"panelSpacing\":\"0px\",");
                    break;
            }

            builder.AppendFormat("\"panelCount\":{0},", panel.PanelStatePages.Count.ToString());
            //int startPage = panel.PanelStatePages.IndexOf(panel.StartPanelStatePage);
            //startPage += 1; // startPage index is 1 based.
            //builder.AppendFormat("\"startPage\":" + startPage + ",");



            if (IsSetMD5 == true)
            {
                string conttenMd5=string.Empty;
                foreach (IPanelStatePage page in panel.PanelStatePages)
                {
                    conttenMd5 += page.MD5+",";
                }
                conttenMd5.TrimEnd(new char[] { ',' });
                builder.AppendFormat("\"Content\":\"{0}\",", conttenMd5);
            }


            builder.Append("\"panelWidgets\":[");
            foreach (IPanelStatePage page in panel.PanelStatePages)
            {
                bool isClosedPage = false;
                if (!page.IsOpened)
                {
                    isClosedPage = true;
                    page.Open();
                }

                JsDynamicPanelState jsState = new JsDynamicPanelState(_service, page,IsSetMD5);
                builder.Append(jsState.ToString());
                builder.Append(",");

                if (isClosedPage)
                {
                    page.Close();
                }
            }

            JsHelper.RemoveLastComma(builder);
            builder.Append("],");
        }
    }
}
