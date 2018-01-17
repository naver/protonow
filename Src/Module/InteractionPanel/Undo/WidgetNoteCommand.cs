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
    class WidgetNoteCommand : IUndoableCommand
    {
        internal WidgetNoteCommand(NotesViewModel notesVM, string propertyName, string oldValue, string newValue, Guid targetWidgetGuid)
        {
            _notesVM = notesVM;
            _propertyName = propertyName;
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

            _notesVM.GetType().GetProperty(_propertyName).SetValue(_notesVM, _oldValue, null);
        }

        public void Redo()
        {
            if (_pageVM == null || _widgetVM == null)
            {
                return;
            }

            DeselectOtherWidgets();

            _widgetVM.IsSelected = true;

            _notesVM.GetType().GetProperty(_propertyName).SetValue(_notesVM, _newValue, null);
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

        private NotesViewModel _notesVM;
        private string _propertyName;
        private string _oldValue;
        private string _newValue;
        private WidgetViewModBase _widgetVM;
    }
}
