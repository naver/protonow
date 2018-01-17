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
    class PageLinkCommand : IUndoableCommand
    {
        internal PageLinkCommand(InteractionTabVM tabVM, bool oldValue, bool newValue, Guid targetWidgetGuid, Guid selectedPageGuid)
        {
            _tabVM = tabVM;
            _oldValue = oldValue;
            _newValue = newValue;
            _selectedPageGuid = selectedPageGuid;

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

            if (_oldValue)
            {
                _tabVM.WidgetOpenAction.LinkPageGuid = _selectedPageGuid;
            }

            _tabVM.Raw_PageChecked = _oldValue;
        }

        public void Redo()
        {
            if (_pageVM == null || _widgetVM == null)
            {
                return;
            }

            DeselectOtherWidgets();

            _widgetVM.IsSelected = true;

            if (_newValue)
            {
                _tabVM.WidgetOpenAction.LinkPageGuid = _selectedPageGuid;
            }

            _tabVM.Raw_PageChecked = _newValue;
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
        private Guid _selectedPageGuid;
        private bool _oldValue;
        private bool _newValue;
        private WidgetViewModBase _widgetVM;
    }
}
