using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class InteractionCase : XmlElementObject, IInteractionCase
    {
        #region Constructor

        public InteractionCase(IInteractionEvent interactionEvent, string name)
            : base("Case")
        {
            InteractionEvent = interactionEvent;
            _name = name;
            _actions = new InteractionActions(this);
        }

        #endregion

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadElementGuidAttribute(element, ref _guid);

            LoadStringFromChildElementInnerText("Name", element, ref _name);

            string innerText = "";
            LoadStringFromChildElementInnerText("SatisfyType", element, ref innerText);
            if (Enum.TryParse<SatisfyType>(innerText, out _satisfyType) == false)
            {
                _satisfyType = SatisfyType.All;
            }

            XmlElement actionsElement = element["Actions"];
            if (actionsElement != null)
            {
                _actions.LoadDataFromXml(actionsElement);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement caseElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(caseElement);

            SaveElementGuidAttribute(caseElement, _guid);

            SaveStringToChildElement("Name", _name, xmlDoc, caseElement);
            SaveStringToChildElement("SatisfyType", _satisfyType.ToString(), xmlDoc, caseElement);

            _actions.SaveDataToXml(xmlDoc, caseElement);
        }

        #endregion

        #region IUniqueObject, INamedObject

        public Guid Guid
        {
            get { return _guid; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region IInteractionCase

        public string Description 
        {
            get
            {
                return null;
            }
        }

        public SatisfyType SatisfyType
        {
            get { return _satisfyType; }
            set { _satisfyType = value; }
        }

        public bool IsTrue
        {
            get { return true; }
        }

        public IInteractionConditions Conditions
        {
            get { return null; }
        }

        public IInteractionActions Actions
        {
            get { return _actions; }
        }

        public IInteractionCondition CreateCondition()
        {
            return null;
        }

        public void DeleteCondition(Guid conditionGuid)
        {
        }

        public void ClearAllConditions()
        {
        }

        public IInteractionAction CreateAction(ActionType actionType)
        {
            InteractionAction action = InteractionActionFactory.CreateAction(this, actionType);
            if(action != null)
            {
                _actions.Add(action);
            }
            return action;
        }

        public void DeleteAction(Guid actionGuid)
        {
            InteractionAction action = _actions.GetAction(actionGuid) as InteractionAction;
            if(action != null)
            {
                _actions.Remove(action);
            }
        }

        public bool MoveAction(Guid actionGuid, int delta)
        {
            InteractionAction action = _actions.GetAction(actionGuid) as InteractionAction;
            if (action != null)
            {
                return _actions.MoveItem(action, delta);
            }

            return false;
        }

        public void AddAction(IInteractionAction action, int index)
        {
            InteractionAction newAction = action as InteractionAction;
            if (newAction != null)
            {
                _actions.Insert(index, newAction);
                newAction.InteractionCase = this;
            }
        }

        public void ClearAllActions()
        {
            _actions.Clear();
        }

        public IInteractionEvent InteractionEvent
        {
            get;
            set;
        }

        #endregion

        #region Protected Fields

        private Guid _guid = Guid.NewGuid();
        private string _name;
        private SatisfyType _satisfyType = SatisfyType.All;
        private InteractionActions _actions;

        #endregion
    }
}
