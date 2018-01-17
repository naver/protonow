using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Service;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.InfoStructure
{
    class MoveFocusToEditCanvasCommand : IUndoableCommand
    {
        internal MoveFocusToEditCanvasCommand()
        {
            ISelectionService selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            if (selectionService != null)
            {
                _pageVM = selectionService.GetCurrentPage();
            }
        }

        public void Undo()
        {
            MoveFocusToEditCanvas();
        }

        public void Redo()
        {
            MoveFocusToEditCanvas();
        }

        private void MoveFocusToEditCanvas()
        {
            if (_pageVM != null && _pageVM.EditorCanvas != null)
            {
                _pageVM.EditorCanvas.Focus();
            }
        }

        private IPagePropertyData _pageVM;
    }
}
