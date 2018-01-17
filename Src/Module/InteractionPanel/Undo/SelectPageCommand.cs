using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Service;

namespace Naver.Compass.Module
{
    class SelectPageCommand : IUndoableCommand
    {
        internal SelectPageCommand(InteractionTabVM tabVM, Guid oldPageGuid, Guid newPageGuid, Guid targetWidgetGuid)
        {
            _tabVM = tabVM;
            _oldPageGuid = oldPageGuid;
            _newPageGuid = newPageGuid;

            ISelectionService selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            if (selectionService != null)
            {
                _pageVM = selectionService.GetCurrentPage();
                if (_pageVM != null)
                {
                    IWidgetPropertyData widgetVM = _pageVM.GetSelectedwidgets().FirstOrDefault(x => x.WidgetID == targetWidgetGuid);
                    _widgetVM = widgetVM as WidgetViewModBase;
                }
            }
        }

        public void Undo()
        {
            if (_pageVM == null || _widgetVM == null)
            {
                return;
            }

            DeselectOtherWidgets();

            _widgetVM.IsSelected = true;

            _tabVM.SelectPage(_oldPageGuid);
        }

        public void Redo()
        {
            if (_pageVM == null || _widgetVM == null)
            {
                return;
            }

            DeselectOtherWidgets();

            _widgetVM.IsSelected = true;

            _tabVM.SelectPage(_newPageGuid);
        }

        private void DeselectOtherWidgets()
        {
            if (_pageVM != null)
            {
                foreach (IWidgetPropertyData widget in _pageVM.GetSelectedwidgets())
                {
                    WidgetViewModBase widgetVM = widget as WidgetViewModBase;
                    if (widgetVM != null && widgetVM != _widgetVM)
                    {
                        widgetVM.IsSelected = false;
                    }
                }
            }
        }

        private IPagePropertyData _pageVM;

        private InteractionTabVM _tabVM;
        private Guid _oldPageGuid;
        private Guid _newPageGuid;
        private WidgetViewModBase _widgetVM;
    }
}
