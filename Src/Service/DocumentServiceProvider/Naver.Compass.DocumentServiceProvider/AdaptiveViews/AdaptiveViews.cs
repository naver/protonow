using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class AdaptiveViews : XmlElementList<AdaptiveView>, IAdaptiveViews
    {
        internal AdaptiveViews(AdaptiveViewSet set)
            : base("AdaptiveViews")
        {
            _set = set;
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

            foreach (XmlElement childElement in childList)
            {
                AdaptiveView view = new AdaptiveView(_set, "");
                view.LoadDataFromXml(childElement);
                Add(view);
            }
        }

        #endregion

        #region IAdaptiveViews

        public IAdaptiveView GetAdaptiveView(Guid viewGuid)
        {
            return _list.FirstOrDefault(x => x.Guid == viewGuid);
        }

        public bool Contains(Guid viewGuid)
        {
            return _list.Any(x => x.Guid == viewGuid);
        }

        public IAdaptiveView this[Guid viewGuid] 
        {
            get { return GetAdaptiveView(viewGuid); }
        }

        public int IndexOf(IAdaptiveView view)
        {
            return base.IndexOf(view as AdaptiveView);
        }

        public IAdaptiveViewSet AdaptiveViewSet
        {
            get { return _set; }
        }

        #endregion

        private AdaptiveViewSet _set;
    }
}
