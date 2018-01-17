using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    internal class Pages : XmlElementDictionary<Guid, DocumentPage>, IPages
    {
        internal Pages(Document document)
            : base("Pages")
        {
            Debug.Assert(document != null);
            _document = document;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            XmlNodeList childList = element.ChildNodes;
            if (childList == null || childList.Count <= 0)
            {
                return;
            }

            // Just load page guid and name
            foreach (XmlElement childElement in childList)
            {
                DocumentPage page = null;
                if (_document.DocumentType == DocumentType.Library)
                {
                    page = new CustomObjectPage(_document, "");
                }
                else
                {
                    page = new StandardPage(_document, "");
                }
                page.PageData.Clear(); // Page data is cleared now.

                page.LoadDataFromXml(childElement);
                Add(page.Guid, page);
                page.OnAddToDocument();
            }
        }

        #endregion

        public new IEnumerator<IDocumentPage> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public IDocumentPage GetPage(Guid pageGuid)
        {
            return Get(pageGuid);
        }

        public new IDocumentPage this[Guid pageGuid] 
        {
            get { return GetPage(pageGuid); }
        }

        private Document _document;
    }
}
