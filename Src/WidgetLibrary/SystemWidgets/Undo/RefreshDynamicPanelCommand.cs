using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.WidgetLibrary
{
    public class RefreshDynamicPanelCommand : IUndoableCommand
    {

        public RefreshDynamicPanelCommand(Guid guid)
        {
            _widgetGid = guid;
        }
        public void Undo()
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<RefreshWidgetChildPageEvent>().Publish(_widgetGid);
        }

        public void Redo()
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<RefreshWidgetChildPageEvent>().Publish(_widgetGid);
        }

        private Guid _widgetGid;
    }
}
