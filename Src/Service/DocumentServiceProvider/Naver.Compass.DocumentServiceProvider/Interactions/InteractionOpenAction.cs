using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class InteractionOpenAction : InteractionAction, IInteractionOpenAction
    {
        public InteractionOpenAction(IInteractionCase interactionCase)
            : base(interactionCase)
        {
            _actionType = ActionType.OpenAction;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            LoadEnumFromChildElementInnerText<LinkType>("LinkType", element, ref _linkType);
            LoadEnumFromChildElementInnerText<ActionOpenIn>("ActionOpenIn", element, ref _openIn);
            LoadGuidFromChildElementInnerText("LinkPageGuid", element, ref _pageGuid);
            LoadStringFromChildElementInnerText("ExternalUrl", element, ref _externalUrl);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            // Do not save if link type is none.
            if (_linkType == LinkType.None)
            {
                return;
            }

            XmlElement actionElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(actionElement);

            base.SaveDataToXml(xmlDoc, actionElement);

            SaveStringToChildElement("LinkType", _linkType.ToString(), xmlDoc, actionElement);
            SaveStringToChildElement("ActionOpenIn", _openIn.ToString(), xmlDoc, actionElement);
            SaveStringToChildElement("LinkPageGuid", _pageGuid.ToString(), xmlDoc, actionElement);
            SaveStringToChildElement("ExternalUrl", _externalUrl, xmlDoc, actionElement);
        }

        #endregion

        #region IInteractionOpenAction

        public LinkType LinkType
        {
            get { return _linkType; }
            set { _linkType = value; }
        }

        public ActionOpenIn OpenIn
        {
            get { return _openIn; }
            set { _openIn = value; }
        }

        public Guid LinkPageGuid
        {
            get { return _pageGuid; }
            set 
            { 
                _pageGuid = value;
                _externalUrl = null;
            }
        }

        public string ExternalUrl
        {
            get { return _externalUrl; }
            set 
            { 
                _externalUrl = value;
                _pageGuid = Guid.Empty;
            }
        }

        internal override void Update()
        {
            if (_linkType != LinkType.LinkToPage)
            {
                return;
            }

            if (InteractionCase != null
                && InteractionCase.InteractionEvent != null
                && InteractionCase.InteractionEvent.InteractiveObject != null)
            {
                IPage contextPage = InteractionCase.InteractionEvent.InteractiveObject.ContextPage;
                if (contextPage != null)
                {
                    IDocument document = contextPage.ParentDocument;
                    if (document != null && !document.Pages.Contains(_pageGuid))
                    {
                        _linkType = LinkType.None;
                        LinkPageGuid = Guid.Empty;
                    }
                }
            }
        }

        #endregion

        #region Private Fields

        private LinkType _linkType = LinkType.None;
        private ActionOpenIn _openIn = ActionOpenIn.CurrentWindow;
        private Guid _pageGuid;
        private string _externalUrl;

        #endregion

    }
}
