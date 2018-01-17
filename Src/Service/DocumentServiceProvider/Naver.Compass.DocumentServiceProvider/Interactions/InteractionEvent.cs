using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    [Serializable]
    internal class InteractionEvent : XmlElementObject, IInteractionEvent
    {
        #region Constructors

        static InteractionEvent()
        {
            _nameDic = new Dictionary<EventType, string>();
            _nameDic[EventType.OnClick] = "OnClick";
            _nameDic[EventType.OnSelectionChange] = "OnSelectionChange";
            _nameDic[EventType.OnTextChange] = "OnTextChange";
            _nameDic[EventType.OnCheckedChange] = "OnCheckedChange";
            _nameDic[EventType.OnLoad] = "OnLoad";
            _nameDic[EventType.OnPageLoad] = "OnPageLoad";
        }

        internal InteractionEvent(IInteractiveObject interactiveObject)
            : this(interactiveObject, EventType.None)
        {
        }

        internal InteractionEvent(IInteractiveObject interactiveObject, EventType type)
            : base("Event")
        {
            InteractiveObject = interactiveObject;
            _eventType = type;
            _cases = new InteractionCases(this);
        }

        #endregion

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadElementGuidAttribute(element, ref _guid);

            LoadEnumFromChildElementInnerText("EventType", element, ref _eventType);
            LoadBoolFromChildElementInnerText("UseElseIf", element, ref _useElseIf);

            XmlElement casesElement = element["Cases"];
            if (casesElement != null)
            {
                _cases.LoadDataFromXml(casesElement);
            }

        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            // Do not save event if it doesn't have any case.
            if(_cases.Count <= 0)
            {
                return;
            }

            XmlElement eventElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(eventElement);

            SaveElementGuidAttribute(eventElement, _guid);

            SaveStringToChildElement("EventType", _eventType.ToString(), xmlDoc, eventElement);
            SaveStringToChildElement("UseElseIf", _useElseIf.ToString(), xmlDoc, eventElement);

            _cases.SaveDataToXml(xmlDoc, eventElement);
        }

        #endregion

        #region IUniqueObject, INamedObject

        public Guid Guid
        {
            get { return _guid; }
        }

        public string Name
        {
            get
            {
                string name;
                if (!_nameDic.TryGetValue(_eventType, out name))
                {
                    name = string.Empty;
                }

                return name;
            }
            set { return; }
        }

        #endregion

        #region IEvent
        
        public EventType EventType
        {
            get { return _eventType; }
        }

        public bool UseElseIf
        {
            get { return _useElseIf; }
            set { _useElseIf = value; }
        }

        public IInteractionCases Cases
        {
            get { return _cases; }
        }

        public IInteractionCase CreateCase(string caseName)
        {
            InteractionCase interactionCase = new InteractionCase(this, caseName);
            _cases.Add(interactionCase);
            return interactionCase;
        }

        public void DeleteCase(Guid caseGuid)
        {
            InteractionCase interactionCase = _cases.GetCase(caseGuid) as InteractionCase;
            if (interactionCase != null)
            {
                _cases.Remove(interactionCase);
            }
        }

        public bool MoveCase(Guid caseGuid, int delta)
        {
            InteractionCase interactionCase = _cases.GetCase(caseGuid) as InteractionCase;
            if (interactionCase != null)
            {
               return _cases.MoveItem(interactionCase, delta);
            }

            return false;
        }

        public void AddCase(IInteractionCase interactionCase, int index)
        {
            InteractionCase newCase = interactionCase as InteractionCase;
            if (newCase != null)
            {
                _cases.Insert(index, newCase);
                newCase.InteractionEvent = this;
            }
        }

        public IInteractiveObject InteractiveObject
        {
            get;
            private set;
        }

        #endregion

        internal void UpdateActions()
        {
            foreach (InteractionCase iCase in _cases)
            {
                foreach (InteractionAction iAction in iCase.Actions)
                {
                    iAction.Update();
                }
            }
        }

        internal void UpdateActions(Dictionary<Guid, IObjectContainer> newTargets)
        {
            foreach (InteractionCase iCase in _cases)
            {
                foreach (InteractionAction iAction in iCase.Actions)
                {
                    iAction.Update(newTargets);
                }
            }
        }

        #region Private Fields

        private Guid _guid = Guid.NewGuid();
        private EventType _eventType = EventType.None;
        private bool _useElseIf;
        private InteractionCases _cases;

        private static Dictionary<EventType, string> _nameDic;

        #endregion
    }
}
