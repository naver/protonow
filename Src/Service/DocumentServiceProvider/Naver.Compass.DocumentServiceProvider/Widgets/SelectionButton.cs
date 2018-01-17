using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal abstract class SelectionButton : Widget, ISelectionButton
    {
        internal SelectionButton(Page parentPage, string tagName)
            : base(parentPage, tagName)
        {
            AddWidgetSupportEvents();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            XmlElement propertiesElement = element["Properties"];
            if (propertiesElement != null)
            {
                LoadBoolFromChildElementInnerText("IsSelected", propertiesElement, ref _isSelected);
                LoadEnumFromChildElementInnerText<AlignButton>("AlignButton", propertiesElement, ref _alignButton);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            base.SaveDataToXml(xmlDoc, parentElement);

            XmlElement propertiesElement = parentElement["Properties"];
            SaveStringToChildElement("IsSelected", _isSelected.ToString(), xmlDoc, propertiesElement);
            SaveStringToChildElement("AlignButton", _alignButton.ToString(), xmlDoc, propertiesElement);
        }

        #endregion

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public AlignButton AlignButton
        {
            get { return _alignButton; }
            set { _alignButton = value; }
        }

        private void AddWidgetSupportEvents()
        {
            InteractionEvent newEvent = new InteractionEvent(this, EventType.OnCheckedChange);
            _events.Add(EventType.OnCheckedChange, newEvent);
        }

        protected bool _isSelected = false;
        protected AlignButton _alignButton = AlignButton.Left; 
    }
}
