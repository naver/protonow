using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class InteractionCases : XmlElementList<InteractionCase>, IInteractionCases
    {
        internal InteractionCases(IInteractionEvent interactionEvent)
            : base("Cases")
        {
            InteractionEvent = interactionEvent;
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
                InteractionCase interactionCase = new InteractionCase(InteractionEvent, "");
                interactionCase.LoadDataFromXml(childElement);
                Add(interactionCase);
            }
        }

        #endregion

        public IInteractionCase GetCase(string caseName)
        {
            return _list.FirstOrDefault(x => x.Name == caseName);
        }

        public bool Contains(string caseName)
        {
            return _list.Any(x => x.Name == caseName);
        }

        public IInteractionCase this[string caseName]
        {
            get { return GetCase(caseName); }
        }

        public bool Contains(Guid caseGuid)
        {
            return _list.Any(x => x.Guid == caseGuid);
        }

        public IInteractionCase GetCase(Guid caseGuid)
        {
            return _list.FirstOrDefault(x => x.Guid == caseGuid);
        }

        public IInteractionCase this[Guid caseGuid]
        {
            get { return GetCase(caseGuid); }
        }

        public IInteractionEvent InteractionEvent
        {
            get;
            private set;
        }
    }
}
