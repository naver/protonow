using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class InteractionActions : XmlElementList<InteractionAction>, IInteractionActions
    {
        public InteractionActions(IInteractionCase interactionCase)
            : base("Actions")
        {
            InteractionCase = interactionCase;
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
                ActionType type = ActionType.None;
                if (!LoadEnumFromChildElementInnerText<ActionType>("ActionType", childElement, ref type))
                {
                    // Ignore unknown configuration
                    continue;
                }

                InteractionAction action = InteractionActionFactory.CreateAction(InteractionCase, type);
                action.LoadDataFromXml(childElement);
                Add(action);
            }
        }

        #endregion

        public IInteractionAction GetAction(Guid actionGuid)
        {
            return _list.FirstOrDefault( x => x.Guid == actionGuid);
        }

        public bool Contains(Guid actionGuid)
        {
            return _list.Any( x => x.Guid == actionGuid);
        }

        public IInteractionAction this[Guid actionGuid]
        {
            get { return GetAction(actionGuid); }
        }

        public IInteractionCase InteractionCase
        {
            get;
            private set;
        }
    }
}
