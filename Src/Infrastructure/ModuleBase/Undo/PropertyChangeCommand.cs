using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.InfoStructure
{
    public class PropertyChangeCommand : IUndoableCommand
    {
        public PropertyChangeCommand(object target, string propertyName, object oldValue, object newValue)
        {
            _target = target;
            _mementos = new PropertyMementos();

            PropertyMemento memento = new PropertyMemento(propertyName, oldValue, newValue);
            _mementos.AddPropertyMemento(memento);
        }

        public PropertyChangeCommand(object target, PropertyMemento memento)
        {
            _target = target;
            _mementos = new PropertyMementos();
            _mementos.AddPropertyMemento(memento);
        }

        public PropertyChangeCommand(object target, PropertyMementos mementos)
        {
            _target = target;
            _mementos = mementos;
        }

        public void Undo()
        {
            foreach (PropertyMemento memento in _mementos)
            {
                _target.GetType().GetProperty(memento.PropertyName).SetValue(_target, memento.OldValue, null);
            }
        }

        public void Redo()
        {
            foreach (PropertyMemento memento in _mementos)
            {
                _target.GetType().GetProperty(memento.PropertyName).SetValue(_target, memento.NewValue, null);
            }
        }

        private object _target;
        private PropertyMementos _mementos;
    }
}
