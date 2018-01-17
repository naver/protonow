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
    class CheckAllTargetCommand : IUndoableCommand
    {
        internal CheckAllTargetCommand(InteractionTabVM tabVM, Guid targetWidgetGuid, bool checkAll)
        {
            _tabVM = tabVM;
            _checkAll = checkAll;

            if (_checkAll)
            {
                _list = new List<Guid>();
                // Only store target list if check all is checked.
                foreach(WidgetNode node in _tabVM.WidgetList)
                {
                    if(node.IsSelected)
                    {
                        _list.Add(node.TargetObject.Guid);
                    }
                }
            }

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

            if (_checkAll)
            {
                // Set IsCheckAll to false
                _tabVM.Raw_IsCheckAll = false;

                // Select widgets
                foreach(Guid guid in _list)
                {
                    _tabVM.WidgetShowHideAction.AddTargetObject(guid);
                }
                _tabVM.SetShowHideAction();
            }
            else
            {
                _tabVM.Raw_IsCheckAll = true;
            }

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

            _tabVM.Raw_IsCheckAll = _checkAll;

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
        private WidgetViewModBase _widgetVM;
        private List<Guid> _list;
        private bool _checkAll;
    }
}
