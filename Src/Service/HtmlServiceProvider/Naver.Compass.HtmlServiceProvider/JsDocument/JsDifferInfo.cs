using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace Naver.Compass.Service.Html
{
    internal class JsDifferInfo : JsFileObject
    {
        public JsDifferInfo(HtmlServiceProvider service, List<IDocument> Docs)
        {
            _service = service;
            _documents = Docs;
        }

        protected override string SaveDirectory
        {
            get
            {
                string directory = _service.OutputFolder;
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
            content.Append(@"var PNComparingData={""projects"":[");

            for(int i=0;i<_documents.Count;i++)
            {
                //head
                content.Append(@"{");

                _service.RenderingDocument = _documents[i];
                content.AppendFormat("\"id\":\"{0}\",", i);                
                content.AppendFormat("\"name\":\"{0}\",", Path.GetFileNameWithoutExtension(_service.RenderingDocument.Name));
                if(_service.RenderingDocument.TimeStamp.StartsWith("2000-01-01"))
                {
                    content.AppendFormat("\"timestamp\":\"\",");
                }
                else
                {
                    content.AppendFormat("\"timestamp\":\"{0}\",", _service.RenderingDocument.TimeStamp);
                }
                
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

                //tail
                if (i< _documents.Count-1)
                {
                    content.Append(@"},");
                }
                else
                {
                    content.Append(@"}");
                }
            }


            content.Append(@"]};");
            return content.ToString();
        }

        private HtmlServiceProvider _service;
        private List<IDocument> _documents;
    }
}
