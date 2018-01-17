using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Naver.Compass.InfoStructure
{
    public class PropertyMemento
    {
        public PropertyMemento(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public string PropertyName;
        public object OldValue;
        public object NewValue;
    }

    public class PropertyMementos : IEnumerable
    {
        public PropertyMementos()
        {
        }

        public IEnumerator<PropertyMemento> GetEnumerator()
        {
            CheckAndCreateDictionary();

            return _propertyMementoDic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void AddPropertyMemento(PropertyMemento memento)
        {
            if (memento == null)
            {
                throw new ArgumentNullException();
            }

            CheckAndCreateDictionary();

            _propertyMementoDic[memento.PropertyName] = memento;
        }

        public bool RemovePropertyMemento(string propertyName)
        {
            CheckAndCreateDictionary();

            return _propertyMementoDic.Remove(propertyName);
        }

        public void SetPropertyOldValue(string propertyName, object value)
        {
            CheckAndCreateDictionary();
            
            PropertyMemento memento = null;
            if(_propertyMementoDic.TryGetValue(propertyName, out memento))
            {
                if (memento != null)
                {
                    memento.OldValue = value;
                }
            }
        }

        public void SetPropertyNewValue(string propertyName, object value)
        {
            CheckAndCreateDictionary();

            PropertyMemento memento = null;
            if (_propertyMementoDic.TryGetValue(propertyName, out memento))
            {
                if (memento != null)
                {
                    memento.NewValue = value;
                }
            }
        }

        public object GetPropertyOldValue(string propertyName)
        {
            if (_propertyMementoDic == null)
                return null;

             PropertyMemento memento = null;
            if (_propertyMementoDic.TryGetValue(propertyName, out memento))
            {
                if(memento !=null)
                {
                    return memento.OldValue;
                }
            }
            return null;
        }

        public void Clear()
        {
            _propertyMementoDic.Clear();
        }

        private void CheckAndCreateDictionary()
        {
            if (_propertyMementoDic == null)
            {
                _propertyMementoDic = new Dictionary<string, PropertyMemento>();
            }
        }

        private Dictionary<string, PropertyMemento> _propertyMementoDic;
    }
}
