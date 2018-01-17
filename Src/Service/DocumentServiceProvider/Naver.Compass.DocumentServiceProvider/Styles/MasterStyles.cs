using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class MasterStyles : XmlElementDictionary<Guid, MasterStyle>, IMasterStyles
    {
        internal MasterStyles(Master ownerMaster)
            : base("MasterStyles")
        {
            _ownerMaster = ownerMaster;
        }

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
                MasterStyle masterStyle = new MasterStyle(_ownerMaster, Guid.Empty);
                masterStyle.LoadDataFromXml(childElement);
                if (masterStyle.ViewGuid != Guid.Empty)
                {
                    Add(masterStyle.ViewGuid, masterStyle);
                }
            }
        }

        public new IEnumerator<IMasterStyle> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public IMasterStyle GetMasterStyle(Guid viewGuid)
        {
            return Get(viewGuid);
        }

        public new IMasterStyle this[Guid viewGuid]
        {
            get { return Get(viewGuid); }
        }

        private Master _ownerMaster;
    }
}
