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
    class CreateGlobalGuideCommand : IUndoableCommand
    {
        public CreateGlobalGuideCommand(IDocument doc, List<IGuide> guides)
        {
            _listEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _document = doc;
            _guides = guides;
        }

        public void Undo()
        {
            foreach(var item in _guides)
            {
                _document.DeleteGlobalGuide(item.Guid);
            }
            _listEventAggregator.GetEvent<UpdateGridGuide>().Publish(GridGuideType.Guide);
        }

        public void Redo()
        {
            foreach(var item in _guides)
            {
                _document.AddGlobalGuide(item);
            }
            _listEventAggregator.GetEvent<UpdateGridGuide>().Publish(GridGuideType.Guide);
        }

        IEventAggregator _listEventAggregator;
        IDocument _document;
        List<IGuide> _guides;
    }
}
