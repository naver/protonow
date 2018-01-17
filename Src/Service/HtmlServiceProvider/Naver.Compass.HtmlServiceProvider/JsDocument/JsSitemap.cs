using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsNode
    {
        public JsNode(HtmlServiceProvider service, ITreeNode node)
        {
            _service = service;
            _node = node;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            builder.AppendFormat("\"pageName\":\"{0}\",", JsHelper.EscapeDoubleQuote(_node.Name));

            if(_node.NodeType == TreeNodeType.Folder)
            {
                builder.Append("\"type\":\"Folder\"");
            }
            else
            {
                builder.Append("\"type\":\"Page\",");
                builder.AppendFormat("\"url\":\"{0}\"", _node.Guid.ToString());
            }

            if (_node.ChildNodesCount > 0)
            {
                builder.Append(",");
                builder.Append("\"children\":[");
                
                foreach (ITreeNode node in _node.ChildNodes)
                {
                    JsNode jsNode = new JsNode(_service, node);
                    builder.Append(jsNode.ToString());
                }

                JsHelper.RemoveLastComma(builder);

                builder.Append("]");
            }
            
            builder.Append("},");

            return builder.ToString();
        }

        private HtmlServiceProvider _service;
        private ITreeNode _node;
    }

    internal class JsSitemap
    {
        public JsSitemap(HtmlServiceProvider service)
        {
            _service = service;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("\"sitemap\":{");

            Guid startPageGuid = _service.RenderingDocument.GeneratorConfigurationSet.DefaultHtmlConfiguration.StartPage;
            if (startPageGuid != Guid.Empty)
            {
                builder.AppendFormat("\"startPage\": \"{0}\",", startPageGuid.ToString());
            }

            builder.Append("\"rootNodes\":[");

            Guid currentPageGuid = _service.RenderingDocument.GeneratorConfigurationSet.DefaultHtmlConfiguration.CurrentPage;
            if (currentPageGuid != Guid.Empty)
            {
                // Only generate current page
                IPage currentPage = _service.RenderingDocument.Pages[currentPageGuid]; 
                if(currentPage != null)
                {
                    builder.Append("{");
                    builder.AppendFormat("\"pageName\":\"{0}\",", JsHelper.EscapeDoubleQuote(currentPage.Name));
                    builder.Append("\"type\":\"Page\",");
                    builder.AppendFormat("\"url\":\"{0}\"", currentPage.Guid.ToString());
                    builder.Append("}");
                }
            }
            else
            {
                // Generate first level nodes
                foreach (ITreeNode node in _service.RenderingDocument.DocumentSettings.LayoutSetting.PageTree.ChildNodes)
                {
                    JsNode jsNode = new JsNode(_service, node);
                    builder.Append(jsNode.ToString());
                }
            }
            
            JsHelper.RemoveLastComma(builder);

            builder.Append("]");
            builder.Append("}");

            return builder.ToString();
        }

        private HtmlServiceProvider _service;
    }
}
