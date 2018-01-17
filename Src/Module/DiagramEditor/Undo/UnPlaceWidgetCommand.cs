using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.WidgetLibrary;

namespace Naver.Compass.Module
{
    class UnplaceWidgetCommand: IUndoableCommand
    {
        public UnplaceWidgetCommand(PageEditorViewModel pageVM, List<Guid> widgetList)
        {
            _pageVM = pageVM;
            _widgetList = widgetList;
        }

        public void Undo()
        {
            _pageVM.PlaceWidgets2View(_widgetList);
        }

        public void Redo()
        {

            _pageVM.UnplaceWidgetsFromView(_widgetList);
        }

        private PageEditorViewModel _pageVM;
        private List<Guid> _widgetList;
    }
}
