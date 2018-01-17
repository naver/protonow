using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Module
{
    class DeleteWidgetCommand: IUndoableCommand
    {
        public DeleteWidgetCommand(PageEditorViewModel pageVM, WidgetViewModBase widgetVM)
        {
            _pageVM = pageVM;
            _widgetVM = widgetVM;

            foreach (IPageView pageView in _pageVM.PageEditorModel.PageViews)
            {
                if (pageView.Widgets.Contains(_widgetVM.WidgetID))
                {
                    _placedPageViewGuids.Add(pageView.Guid);
                }
            }
        }

        public void Undo()
        {
            _pageVM.AddWidgetItem(_widgetVM);
            _widgetVM.IsSelected = true;

            foreach (Guid guid in _placedPageViewGuids)
            {
                IPageView pageView = _pageVM.PageEditorModel.PageViews[guid];

                if (pageView != null)
                {
                    pageView.PlaceWidget(_widgetVM.WidgetModel.WdgDom.Guid);
                }
            }
        }

        public void Redo()
        {
            _widgetVM.IsSelected = false;
            _pageVM.DeleteItem(_widgetVM);
        }

        private PageEditorViewModel _pageVM;
        private WidgetViewModBase _widgetVM;
        private List<Guid> _placedPageViewGuids = new List<Guid>();
    }
}
