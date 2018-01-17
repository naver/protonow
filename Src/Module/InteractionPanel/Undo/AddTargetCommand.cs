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
    class AddTargetCommand : IUndoableCommand
    {
        internal AddTargetCommand(InteractionTabVM tabVM, Guid targetValue, Guid targetWidgetGuid)
        {
            _tabVM = tabVM;
            _targetValue = targetValue;

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
            if (_pageVM  == null || _widgetVM == null)
            {
                return;
            }

            DeselectOtherWidgets();

            _widgetVM.IsSelected = true;

            _tabVM.WidgetShowHideAction.DeleteTagetObject(_targetValue);
            _tabVM.LoadPageWidgets();
        }

        public void Redo()
        {
            if (_pageVM == null || _widgetVM == null)
            {
                return;
            }

            DeselectOtherWidgets();

            _widgetVM.IsSelected = true;

            _tabVM.WidgetShowHideAction.AddTargetObject(_targetValue);

            //set showhide type of the added target 
            _tabVM.SetShowHideAction();
            _tabVM.LoadPageWidgets();

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
        private Guid _targetValue;
        private WidgetViewModBase _widgetVM;
    }
}
