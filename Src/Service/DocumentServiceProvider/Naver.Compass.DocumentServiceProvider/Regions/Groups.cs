using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class Groups : XmlElementDictionary<Guid, Group>, IGroups
    {
        internal Groups(Page parentPage)
            : base("Groups")
        {
            _parentPage = parentPage;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            XmlNodeList groupsList = element.ChildNodes;
            if (groupsList == null || groupsList.Count <= 0)
            {
                return;
            }

            foreach (XmlElement groupElement in groupsList)
            {
                Group group = new Group(_parentPage);
                group.LoadDataFromXml(groupElement);
                Add(group.Guid, group);
            }
        }

        #endregion

        public new IEnumerator<IGroup> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public IGroup GetGroup(Guid groupGuid)
        {
            return Get(groupGuid);
        }

        public new IGroup this[Guid groupGuid]
        {
            get
            {
                return Get(groupGuid);
            }
        }

        private Page _parentPage;
    }
}
