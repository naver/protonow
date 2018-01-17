using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsDocument : JsFileObject
    {
        public JsDocument(HtmlServiceProvider service)
        {
            _service = service;
        }

        protected override string SaveDirectory
        {
            get
            {
                string directory = _service.OutputFolder;
                directory += @"data";
                return directory;
            }
        }

        protected override string FileName
        {
            get { return @"document_data.js"; }
        }

        public override string ToString()
        {
            StringBuilder content = new StringBuilder();
            content.Append("var htDocumentData={");

            // Document file version which format is x.x.x.x, see VersionHistory.txt in details.
            content.AppendFormat("\"fileVersion\":\"{0}\",", _service.RenderingDocument.FileVersion);

            JsSitemap site = new JsSitemap(_service);
            content.Append(site.ToString());

            content.Append(",");

            JsViewport view = new JsViewport(_service);
            string viewport = view.ToString();
            if (!String.IsNullOrEmpty(viewport))
            {
                content.Append(viewport);
                content.Append(",");
            }

            content.AppendFormat("\"protoNowVer\":\"{0}\",", _service.ProductVersionInfo);

            JsHelper.RemoveLastComma(content);

            content.Append("};");

            return content.ToString();
        }

        private HtmlServiceProvider _service;
    }
}
