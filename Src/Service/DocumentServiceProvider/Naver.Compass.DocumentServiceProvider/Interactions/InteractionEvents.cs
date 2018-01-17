using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    [Serializable]
    internal class InteractionEvents : XmlElementDictionary<EventType, InteractionEvent>, IInteractionEvents 
    {
        public InteractionEvents(IInteractiveObject interactiveObject)
            : base("Events")
        {
            InteractiveObject = interactiveObject;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            // DO NOT clear the collection before loading, as the events key is the EventType and 
            // some interactive objects always support default event. Events have to have item even 
            // the xml element doesn't have any event data.
            
            CheckTagName(element);

            XmlNodeList childList = element.ChildNodes;
            if (childList == null || childList.Count <= 0)
            {
                return;
            }

            foreach (XmlElement childElement in childList)
            {
                InteractionEvent interactionEvent = new InteractionEvent(InteractiveObject);
                interactionEvent.LoadDataFromXml(childElement);
                Add(interactionEvent.EventType, interactionEvent);
            }
        }

        #endregion

        public IInteractionEvent GetEvent(string eventName)
        {
            return _dictionary.Where(x => String.Compare(x.Value.Name, eventName, true) == 0).FirstOrDefault().Value;
        }

        public IInteractionEvent GetEvent(EventType eventType)
        {
            return Get(eventType);
        }
                
        public bool Contains(string eventName)
        {
            return _dictionary.Any(x => String.Compare(x.Value.Name, eventName, true) == 0);
        }

        public IInteractionEvent this[string eventName]
        {
            get { return GetEvent(eventName); }
        }

        public new IInteractionEvent this[EventType eventType]
        {
            get { return GetEvent(eventType); }
        }

        public IInteractiveObject InteractiveObject
        {
            get;
            private set;
        }
    }
}
