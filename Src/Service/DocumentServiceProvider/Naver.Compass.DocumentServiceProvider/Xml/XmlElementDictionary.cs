using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    // A dictionary which can be loaded from or saved to a XmlElement object.
    internal abstract class XmlElementDictionary<TKey, TValue> : XmlElementObject, IEnumerable<TValue>
        where TValue : XmlElementObject
    {
        internal XmlElementDictionary(string tagName)
            :base(tagName)
        {
        }

        #region XmlElementObject

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement itemsElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(itemsElement);

            /* Todo: How about the _dictionary is modifying in the copy constructor? 
             * Use the collection in System.Collections.Concurrent instead?
             *
             * We have to avoid the collection is modified when save the collection, 
             * in other words, the collection cannot be modified in foreach statement.
             * For now, we copy the collection and use the copy collection to save data to the XmlElement,  
             * this should work well in most of the time. BUT in extreme case, it doesn't work if the collection is
             * modified in the copy constructor, see following .NET source code snip from http://referencesource.microsoft.com/
             * 
             * public class Dictionary<TKey,TValue>: IDictionary<TKey,TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>, ISerializable, IDeserializationCallback  
             * {
             *     ...
             *     public Dictionary(IDictionary<TKey,TValue> dictionary): this(dictionary, null) {}
             *          
             *     public Dictionary(IDictionary<TKey,TValue> dictionary, IEqualityComparer<TKey> comparer):
             *         this(dictionary != null? dictionary.Count: 0, comparer) 
             *         {
             *             ...
             *             foreach (KeyValuePair<TKey,TValue> pair in dictionary) 
             *             {
             *                 Add(pair.Key, pair.Value);
             *             }
             *         }
             *         ...
             * } 
             * */

            // Copy the collection in case the original collection is modifying at this time.
            Dictionary<TKey, TValue> copyDictionary = new Dictionary<TKey, TValue>(_dictionary);
            foreach (KeyValuePair<TKey, TValue> item in copyDictionary)
            {
                TValue value = item.Value;
                value.SaveDataToXml(xmlDoc, itemsElement);
            }
        }

        #endregion

        #region IEnumerable Methods

        public IEnumerator<TValue> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Public Methods

        public void Add(TKey key, TValue value)
        {
            _dictionary[key] = value;
        }

        public void Remove(TKey key)
        {
            _dictionary.Remove(key);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public TValue Get(TKey key)
        {
            TValue value;
            _dictionary.TryGetValue(key, out value);
            return value;
        }

        public bool Contains(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public int Count 
        {
            get { return _dictionary.Count; }
        }

        public TValue this[TKey key] 
        {
            get  { return Get(key); }
            set  { _dictionary[key] = value; }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get { return _dictionary.Keys; }
        }

        #endregion

        protected Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
    }
}
