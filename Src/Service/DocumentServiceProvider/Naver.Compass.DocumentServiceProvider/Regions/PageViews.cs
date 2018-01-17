using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class PageViews : XmlElementList<PageView>, IPageViews
    {
        internal PageViews(Page parentPage)
            : base("PageViews")
        {
            // PageViews must exist with its parent page.
            Debug.Assert(parentPage != null);
            _parentPage = parentPage;
        }

        #region XmlElementList

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            XmlNodeList childList = element.ChildNodes;
            if (childList == null || childList.Count <= 0)
            {
                return;
            }

            foreach (XmlElement childElement in childList)
            {
                PageView view = new PageView(_parentPage, Guid.Empty);
                view.LoadDataFromXml(childElement);
                if(view.Guid != Guid.Empty)
                {
                    Add(view);
                }
            }
        }

        #endregion

        public new IEnumerator<IPageView> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public IPageView GetPageView(Guid viewGuid)
        {
            return _list.FirstOrDefault(x => x.Guid == viewGuid);
        }

        public bool Contains(Guid viewGuid)
        {
            return _list.Any(x => x.Guid == viewGuid);
        }

        public IPageView this[Guid viewGuid]
        {
            get { return GetPageView(viewGuid); }
        }

        private Page _parentPage;
    }
}
