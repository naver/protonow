using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class Masters : XmlElementDictionary<Guid, Master>, IMasters
    {
        internal Masters(Page parentPage)
            : base("Masters")
        {
            _parentPage = parentPage;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            XmlNodeList childList = element.ChildNodes;
            if (childList == null || childList.Count <= 0)
            {
                return;
            }

            foreach (XmlElement childElement in childList)
            {
                Master master = new Master(null, _parentPage);
                master.LoadDataFromXml(childElement);
                if(master.MasterPageGuid != Guid.Empty)
                {
                    Add(master.Guid, master);
                }
            }
        }

        #endregion

        public new IEnumerator<IMaster> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public IMaster GetMaster(Guid masterGuid)
        {
            return Get(masterGuid);
        }

        public new IMaster this[Guid masterGuid]
        {
            get { return Get(masterGuid); }
        }

        private Page _parentPage;
    }
}
