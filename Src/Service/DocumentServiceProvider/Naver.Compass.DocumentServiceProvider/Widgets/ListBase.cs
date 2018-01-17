using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class ListItem : IListItem
    {
        public ListItem(string textValue, bool isSelected)
        {
            TextValue = textValue;
            IsSelected = isSelected;
        }

        public string TextValue { get; set; }
        public bool IsSelected { get; set; }
    }

    internal abstract class ListBase : Widget, IListBase
    {
        internal ListBase(Page parentPage, string tagName)
            : base(parentPage, tagName)
        {
            AddWidgetSupportEvents();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            XmlElement listItemsElement = element["ListItems"];
            if (listItemsElement != null)
            {
                XmlNodeList childList = listItemsElement.ChildNodes;
                if (childList == null || childList.Count <= 0)
                {
                    return;
                }

                foreach (XmlElement childElement in childList)
                {
                    string textValue = "";
                    LoadStringFromChildElementInnerText("TextValue", childElement, ref textValue);
                    bool isSelected = false;
                    LoadBoolFromChildElementInnerText("IsSelected", childElement, ref isSelected);

                    ListItem item = new ListItem(textValue, isSelected);
                    _list.Add(item);
                }
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            base.SaveDataToXml(xmlDoc, parentElement);

            XmlElement listItemsElement = xmlDoc.CreateElement("ListItems");
            parentElement.AppendChild(listItemsElement);

            foreach (IListItem item in _list)
            {
                XmlElement listItemElement = xmlDoc.CreateElement("ListItem");
                listItemsElement.AppendChild(listItemElement);

                SaveStringToChildElement("TextValue", item.TextValue, xmlDoc, listItemElement);
                SaveStringToChildElement("IsSelected", item.IsSelected.ToString(), xmlDoc, listItemElement);
            }
        }

        #endregion

        public List<IListItem> Items 
        {
            get { return _list; }
            set { _list = value; }
        }

        public IListItem CreateItem(string textValue)
        {
            ListItem item = new ListItem(textValue, false);
            _list.Add(item);
            return item;
        }

        private void AddWidgetSupportEvents()
        {
            InteractionEvent newEvent = new InteractionEvent(this, EventType.OnSelectionChange);
            _events.Add(EventType.OnSelectionChange, newEvent);
        }

        protected List<IListItem> _list = new List<IListItem>();
    }
}
