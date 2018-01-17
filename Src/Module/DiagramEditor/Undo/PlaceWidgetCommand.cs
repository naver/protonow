using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.Module
{
    class PlaceWidgetCommand : IUndoableCommand
    {
        public PlaceWidgetCommand(PageEditorViewModel pageVM, List<Guid> widgetList)
        {
            _pageVM = pageVM;
            _widgetList = widgetList;
        }

        public void Undo()
        {
            _pageVM.UnplaceWidgetsFromView(_widgetList);
        }

        public void Redo()
        {
            _pageVM.PlaceWidgets2View(_widgetList);
        }

        private PageEditorViewModel _pageVM;
        private List<Guid> _widgetList;
    }
}
