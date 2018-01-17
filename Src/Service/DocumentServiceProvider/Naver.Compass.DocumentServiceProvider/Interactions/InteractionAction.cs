using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal abstract class InteractionAction : XmlElementObject, IInteractionAction
    {
        #region Constructor

        public InteractionAction(IInteractionCase interactionCase)
            : base("Action")
        {
            InteractionCase = interactionCase;
        }

        #endregion

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadElementGuidAttribute(element, ref _guid);

            LoadEnumFromChildElementInnerText("ActionType", element, ref _actionType);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            SaveElementGuidAttribute(parentElement, _guid);

            SaveStringToChildElement("ActionType", _actionType.ToString(), xmlDoc, parentElement);
        }

        #endregion

        public Guid Guid
        {
            get { return _guid; }
        }

        public ActionType ActionType
        {
            get { return _actionType; }
        }

        public IInteractionCase InteractionCase
        {
            get;
            set;
        }

        internal virtual void Update()
        {
            return;
        }

        internal virtual void Update(Dictionary<Guid, IObjectContainer> newTargets)
        {
            return;
        }

        protected Guid _guid = Guid.NewGuid();
        protected ActionType _actionType = ActionType.None;
    }
}
