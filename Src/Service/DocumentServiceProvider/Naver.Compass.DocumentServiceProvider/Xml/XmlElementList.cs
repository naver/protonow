using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    // A list which can be loaded from or saved to a XmlElement object.
    internal abstract class XmlElementList<T> : XmlElementObject, IEnumerable<T>
        where T : XmlElementObject
    {
        internal XmlElementList(string tagName)
            : base(tagName)
        {
        }

        #region XmlElementObject

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement itemsElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(itemsElement);

            // Copy the collection in case the original collection is modifying at this time.
            // See comments in SaveDataToXml() in XmlElementDictionary.
            List<T> copyList = new List<T>(_list); 
            foreach (T item in copyList)
            {
                item.SaveDataToXml(xmlDoc, itemsElement);
            }
        }

        #endregion

        #region IEnumerable Methods

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Internal Methods

        public void Add(T item)
        {
            _list.Add(item);
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public int Count 
        {
            get { return _list.Count; }
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool MoveItem(T item, int delta)
        {
            if (_list.Contains(item))
            {
                int index = _list.IndexOf(item);
                if ((delta < 0) && ((index + delta) >= 0))
                {
                    _list.RemoveAt(index);
                    _list.Insert(index + delta, item);
                    return true;
                }
                if ((delta > 0) && ((index + delta) < _list.Count))
                {
                    _list.RemoveAt(index);
                    _list.Insert(index + delta, item);
                    return true;
                }
            }

            return false;
        }

        public bool MoveItemTo(T item, int index)
        {
            if (_list.Contains(item))
            {
                int oldIndex = _list.IndexOf(item);

                if(oldIndex == index)
                {
                    return true;
                }

                if(index >= 0 && index < _list.Count)
                {
                    _list.RemoveAt(oldIndex);
                    _list.Insert(index, item);
                    return true;
                }
            }

            return false;
        }

        #endregion

        protected List<T> _list = new List<T>();
    }
}
