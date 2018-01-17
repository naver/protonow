using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module
{
    class DeletePageGuideCommand : IUndoableCommand
    {
        public DeletePageGuideCommand(IPageView view, List<IGuide> guides)
        {
            _listEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _pageView = view;
            _guides = guides;
        }

        public void Undo()
        {
            foreach(var item in _guides)
            {
                _pageView.AddGuide(item);
            }
            _listEventAggregator.GetEvent<UpdateGridGuide>().Publish(GridGuideType.Guide);
        }

        public void Redo()
        {
            foreach (var item in _guides)
            {
                _pageView.DeleteGuide(item.Guid);
            }
            _listEventAggregator.GetEvent<UpdateGridGuide>().Publish(GridGuideType.Guide);
        }

        IEventAggregator _listEventAggregator;
        IPageView _pageView;
        List<IGuide> _guides;
    }
}
