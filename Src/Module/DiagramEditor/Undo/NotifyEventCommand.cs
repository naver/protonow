using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Module
{
    class NotifyEventCommand: IUndoableCommand
    {
        public NotifyEventCommand(CompositeEventType type)
        {
            _type=type;

        }
        private CompositeEventType _type;


        public void Undo()
        {
            IEventAggregator _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            switch (_type)
            {
                case CompositeEventType.ZorderChange:
                    _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
                    return;
                case CompositeEventType.GroupChange:
                    _ListEventAggregator.GetEvent<GroupChangedEvent>().Publish(false);
                    return;
            }           
        }

        public void Redo()
        {
            IEventAggregator _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            switch (_type)
            {
                case CompositeEventType.ZorderChange:
                    _ListEventAggregator.GetEvent<ZorderChangedEvent>().Publish(null);
                    return;
                case CompositeEventType.GroupChange:
                    _ListEventAggregator.GetEvent<GroupChangedEvent>().Publish(false);
                    return;
            }      
        }
    }
}
