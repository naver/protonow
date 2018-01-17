using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsPage : JsFileObject
    {
        public JsPage(HtmlServiceProvider service, IPage page,bool byMD5=false)
        {
            _service = service;
            _page = page;
            _byMD5 = byMD5;
        }

        protected override string SaveDirectory
        {
            get
            {
                string directory = _service.OutputFolder;
                if (_byMD5 == false)
                {
                    directory += @"data\" + _page.Guid.ToString();
                }
                else
                {
                    directory += _page.Guid.ToString();
                }
                return directory;
            }
        }

        protected override string FileName
        {
            get { return @"page_data.js"; }
        }

        public override string ToString()
        {
            StringBuilder content = new StringBuilder();

            if (_page is IMasterPage)
            {
                content.Append("var master_data={");
            }
            else
            {
                content.Append("var page_data={");
            }

            content.AppendFormat("\"pageName\":\"{0}\",", JsHelper.EscapeDoubleQuote(_page.Name));
            content.AppendFormat("\"pageID\":\"{0}\",", _page.Guid.ToString());
            if(_byMD5==true)
            {
                content.AppendFormat("\"MD5\":\"{0}\",", _page.MD5);
            }
            

            JsPageNotes pageNotes = new JsPageNotes(_service, _page);
            content.Append(pageNotes.ToString());
            content.Append(",");

            // adaptiveViews - This array must be sorted by width value. from small width to great width
            List<IAdaptiveView> viewList = new List<IAdaptiveView>(_service.RenderingDocument.AdaptiveViewSet.AdaptiveViews.OfType<IAdaptiveView>());
            List<IAdaptiveView> orderedViewList = viewList.OrderBy(x => x.Width).ToList();
            content.Append("\"adaptiveView\":[");
            foreach (IAdaptiveView view in orderedViewList)
            {
                JsAdaptiveView jsAdaptiveView = new JsAdaptiveView(_service, view);
                content.Append(jsAdaptiveView.ToString());
                content.Append(",");
            }
            JsHelper.RemoveLastComma(content);
            content.Append("],");

            // widgets
            content.Append("\"widgets\":[");
            foreach (IWidget widget in _page.Widgets)
            {
                JsWidget jsWidget = JsWidgetFactory.CreateJsWidget(_service, widget,_byMD5);
                content.Append(jsWidget.ToString());
                content.Append(",");
            }
            JsHelper.RemoveLastComma(content);
            content.Append("],");

            // masters
            content.Append("\"masters\":[");
            foreach (IMaster master in _page.Masters)
            {
                JsMaster jsMaster = new JsMaster(_service, master, _byMD5);
                content.Append(jsMaster.ToString());
                content.Append(",");
            }
            JsHelper.RemoveLastComma(content);
            content.Append("],");

            // groups
            content.Append("\"groups\":[");
            foreach (IGroup group in _page.Groups)
            {
                JsGroup jsGroup = new JsGroup(_service, group);
                content.Append(jsGroup.ToString());
                content.Append(",");
            }
            JsHelper.RemoveLastComma(content);
            content.Append("]");

            content.Append("};");

            string sz=content.ToString(); ;
            return content.ToString();
        }

        private HtmlServiceProvider _service;
        private IPage _page;
        private bool _byMD5;
    }
}
