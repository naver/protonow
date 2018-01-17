using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class InteractionCloseAction : InteractionAction, IInteractionCloseAction
    {
        public InteractionCloseAction(IInteractionCase interactionCase)
            : base(interactionCase)
        {
            _actionType = ActionType.CloseAction;
        }

        #region XmlElementObject

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement actionElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(actionElement);

            base.SaveDataToXml(xmlDoc, actionElement);
        }

        #endregion
    }
}
