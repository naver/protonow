using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Service;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.InfoStructure
{
    class DeselectAllWidgetsCommand : IUndoableCommand
    {
        internal DeselectAllWidgetsCommand()
        {
            ISelectionService selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            if (selectionService != null)
            {
                _pageVM = selectionService.GetCurrentPage();
            }
        }

        public void Undo()
        {
            DeselectAllWidgets();
        }

        public void Redo()
        {
            DeselectAllWidgets();
        }

        private void DeselectAllWidgets()
        {
            if (_pageVM != null)
            {
                foreach (IWidgetPropertyData widget in _pageVM.GetSelectedwidgets())
                {
                    WidgetViewModBase widgetVM = widget as WidgetViewModBase;
                    if (widgetVM != null)
                    {
                        if (widgetVM.IsGroup)
                        {
                            // Group VM has IsSelected and GroupStatus.UnSelect property, set GroupStatus will also set IsSelected, but set 
                            // set IsSelected will not set status, they are not synchronized. We use status to set group selection in undo/redo.
                            // See SelectCommand.
                            IGroupOperation pageVM = _pageVM as IGroupOperation;
                            if (pageVM != null)
                            {
                                pageVM.SetGroupStatus(widgetVM.widgetGID, GroupStatus.UnSelect);
                            }
                        }
                        else
                        {
                            widgetVM.Raw_IsSelected = false;
                        }
                    }
                }
            }
        }

        private IPagePropertyData _pageVM;
    }
}
