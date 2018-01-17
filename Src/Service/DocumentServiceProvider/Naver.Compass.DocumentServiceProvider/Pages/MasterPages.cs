using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class MasterPages : XmlElementDictionary<Guid, MasterPage>, IMasterPages
    {
        internal MasterPages(Document document)
            : base("MasterPages")
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
                MasterPage page = new MasterPage(_document, "");
                page.PageData.Clear(); // Page data is cleared now.

                page.LoadDataFromXml(childElement);
                Add(page.Guid, page);
                page.OnAddToDocument();
            }
        }

        #endregion

        public new IEnumerator<IMasterPage> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public IMasterPage GetPage(Guid pageGuid)
        {
            return Get(pageGuid);
        }

        public new IMasterPage this[Guid pageGuid] 
        {
            get { return GetPage(pageGuid); }
        }

        private Document _document;
    }
}
