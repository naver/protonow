using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;

namespace Naver.Compass.Module
{
    class DeleteDynamicPanelStateCommand : IUndoableCommand
    {
        public DeleteDynamicPanelStateCommand(DynamicPageEditorViewModel pageVM, DynamicChildNodViewModel stateVM)
        {
            _pageVM = pageVM;
            _stateVM = stateVM;

            _index = _pageVM.DynamicChildren.IndexOf(_stateVM);

            _docService = ServiceLocator.Current.GetInstance<IDocumentService>();
        }

        public void Undo()
        {
            _stateVM.IsChecked = false;
            _pageVM.DynamicChildren.Insert(_index, _stateVM);
            //_pageVM.DyncWidget.PanelStatePages.Insert(_index, _stateVM.Page as IPanelStatePage);
            _pageVM.DyncWidget.AddPanelStatePage((_stateVM.Page as IPanelStatePage), _index);
            if (_stateVM.ShowNumber == 1)
            {
                _pageVM.DyncWidget.StartPanelStatePage = _stateVM.Page as IPanelStatePage;
            }
            _docService.Document.IsDirty = true;
        }

        public void Redo()
        {
            _pageVM.DynamicChildren.RemoveAt(_index);
            //_pageVM.DyncWidget.PanelStatePages.RemoveAt(_index);
            _pageVM.DyncWidget.DeletePanelStatePage(_stateVM.GID);
            _docService.Document.IsDirty = true;
        }

        private IDocumentService _docService;
        private DynamicPageEditorViewModel _pageVM;
        private DynamicChildNodViewModel _stateVM;
        private int _index;
    }
}