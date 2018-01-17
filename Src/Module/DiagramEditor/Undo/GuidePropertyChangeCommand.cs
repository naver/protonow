using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module
{
    class GuidePropertyChangeCommand : IUndoableCommand
    {
        public GuidePropertyChangeCommand(object target, string propertyName, object oldValue, object newValue)
        {
            _listEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _target = target;

            _memento = new PropertyMemento(propertyName, oldValue, newValue);
        }

        public void Undo()
        {
            _target.GetType().GetProperty(_memento.PropertyName).SetValue(_target, _memento.OldValue, null);
            _listEventAggregator.GetEvent<UpdateGridGuide>().Publish(GridGuideType.Guide);

        }

        public void Redo()
        {
            _target.GetType().GetProperty(_memento.PropertyName).SetValue(_target, _memento.NewValue, null);
            _listEventAggregator.GetEvent<UpdateGridGuide>().Publish(GridGuideType.Guide);

        }

        IEventAggregator _listEventAggregator;
        private object _target;
        private PropertyMemento _memento;
    }
}
