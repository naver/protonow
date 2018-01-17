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
    class ExternalLinkCommand : IUndoableCommand
    {
        internal ExternalLinkCommand(InteractionTabVM tabVM, bool oldValue, bool newValue, Guid targetWidgetGuid, string externalLink)
        {
            _tabVM = tabVM;
            _oldValue = oldValue;
            _newValue = newValue;
            _externalLink = externalLink;

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

            _tabVM.Raw_LinkChecked = _oldValue;

            if (_oldValue)
            {
                _tabVM.Raw_ExternalLink = _externalLink;
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

            _tabVM.Raw_LinkChecked = _newValue;

            if (_newValue)
            {
                _tabVM.Raw_ExternalLink = _externalLink;
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
        private string _externalLink;
        private bool _oldValue;
        private bool _newValue;
        private WidgetViewModBase _widgetVM;
    }
}
