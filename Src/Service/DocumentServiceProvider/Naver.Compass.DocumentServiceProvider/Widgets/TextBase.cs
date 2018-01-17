using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal abstract class TextBase : Widget, ITextBase
    {
        internal TextBase(Page parentPage, string tagName)
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
                LoadStringFromChildElementInnerText("HintText", propertiesElement, ref _hintText);
                LoadIntFromChildElementInnerText("MaxLength", propertiesElement, ref _maxLength);
                LoadBoolFromChildElementInnerText("HideBorder", propertiesElement, ref _hideBorder);
                LoadBoolFromChildElementInnerText("ReadOnly", propertiesElement, ref _readOnly);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            base.SaveDataToXml(xmlDoc, parentElement);

            XmlElement propertiesElement = parentElement["Properties"];
            SaveStringToChildElement("HintText", _hintText, xmlDoc, propertiesElement);
            SaveStringToChildElement("MaxLength", _maxLength.ToString(), xmlDoc, propertiesElement);
            SaveStringToChildElement("HideBorder", _hideBorder.ToString(), xmlDoc, propertiesElement);
            SaveStringToChildElement("ReadOnly", _readOnly.ToString(), xmlDoc, propertiesElement);
        }

        #endregion

        public string HintText
        {
            get { return _hintText; }
            set { _hintText = value; }
        }

        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }

        public bool HideBorder
        {
            get { return _hideBorder; }
            set { _hideBorder = value; }
        }

        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        private void AddWidgetSupportEvents()
        {
            InteractionEvent newEvent = new InteractionEvent(this, EventType.OnTextChange);
            _events.Add(EventType.OnTextChange, newEvent);
        }

        protected string _hintText;
        protected int _maxLength = DEFAULT_MAXLENGTH_VALUE;
        protected bool _hideBorder = false;
        protected bool _readOnly = false;

        // Default value for MaxLength
        public const int DEFAULT_MAXLENGTH_VALUE = 100;

    }
}
