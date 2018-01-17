using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Module
{
    class ShowHideCommand : IUndoableCommand
    {
        internal ShowHideCommand(InteractionTabVM tabVM, object oldValue, object newValue, Guid targetWidgetGuid)
        {
            _tabVM = tabVM;
            _oldValue = oldValue;
            _newValue = newValue;

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

            if(_oldValue is VisibilityType)
            {
                _tabVM.Raw_ShowHideType = (VisibilityType)_oldValue;
            }
            else if(_oldValue is ShowHideAnimateType)
            {
                _tabVM.Raw_AnimateType = (ShowHideAnimateType)_oldValue;
            }
            else if(_oldValue is bool)
            {
                _tabVM.Raw_IsNewWindowChecked = (bool)_oldValue;
            }
            else if(_oldValue is int)
            {
                _tabVM.Raw_AnimateTime = (int)_oldValue;
            }
        }

        public void Redo()
        {
            if (_pageVM == null || _widgetVM == null)
            {
                return;
            }

            DeselectOtherWidgets();

            _widgetVM.IsSelected = true;

            if (_newValue is VisibilityType)
            {
                _tabVM.Raw_ShowHideType = (VisibilityType)_newValue;
            }
            else if (_newValue is ShowHideAnimateType)
            {
                _tabVM.Raw_AnimateType = (ShowHideAnimateType)_newValue;
            }
            else if (_newValue is bool)
            {
                _tabVM.Raw_IsNewWindowChecked = (bool)_newValue;
            }
            else if (_newValue is int)
            {
                _tabVM.Raw_AnimateTime = (int)_newValue;
            }
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
        private object _oldValue;
        private object _newValue;
        private WidgetViewModBase _widgetVM;
    }
}
