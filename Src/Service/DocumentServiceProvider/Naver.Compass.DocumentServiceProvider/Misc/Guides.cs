using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class Guides : XmlElementDictionary<Guid, Guide>, IGuides
    {
        public Guides()
            : base("Guides")
        {
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            XmlNodeList guidesList = element.ChildNodes;
            if (guidesList == null || guidesList.Count <= 0)
            {
                return;
            }

            foreach (XmlElement guideElement in guidesList)
            {
                Guide guide = new Guide();
                guide.LoadDataFromXml(guideElement);
                Add(guide.Guid, guide);
            }
        }

        #endregion

        public IGuide GetGuide(Guid guideGuid)
        {
            return Get(guideGuid);
        }

        public new IGuide this[Guid guideGuid]
        {
            get { return Get(guideGuid); }
        }
    }
}
