using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;

namespace Naver.Compass.Module
{
    class DynamicChildNodViewModelUndo
    {
        public DynamicChildNodViewModelUndo(DynamicChildNodViewModel vm, int index)
        {
            NodeViewModel = vm;
            Name = vm.Name;
            Index = index;
        }

        public int Index { get; set; }
        public string Name { get; set; }
        public DynamicChildNodViewModel NodeViewModel { get; set; }
    }

    class DynamicPanelStatesChangeCommand : IUndoableCommand
    {
        public DynamicPanelStatesChangeCommand(DynamicPageEditorViewModel pageVM)
        {
            _pageVM = pageVM;

            foreach(DynamicChildNodViewModel state in _pageVM.DynamicChildren)
            {
                DynamicChildNodViewModelUndo item = new DynamicChildNodViewModelUndo(state, _pageVM.DynamicChildren.IndexOf(state));
                _oldList.Add(item);
            }
        }

        public void SaveCurrentStates()
        {
            foreach (DynamicChildNodViewModel state in _pageVM.DynamicChildren)
            {
                DynamicChildNodViewModelUndo item = new DynamicChildNodViewModelUndo(state, _pageVM.DynamicChildren.IndexOf(state));
                _newList.Add(item);
            }
        }


        public void Undo()
        {
            BuildDynamicChildren(_oldList);
        }

        public void Redo()
        {
            BuildDynamicChildren(_newList);
        }

        private void BuildDynamicChildren(List<DynamicChildNodViewModelUndo> list)
        {
            _pageVM.DynamicChildren.Clear();
            _pageVM.DyncWidget.PanelStatePages.Clear();

            foreach (DynamicChildNodViewModelUndo item in list)
            {
                item.NodeViewModel.Name = item.Name;
                _pageVM.DynamicChildren.Insert(item.Index, item.NodeViewModel);
                _pageVM.DyncWidget.PanelStatePages.Insert(item.Index, item.NodeViewModel.Page as IPanelStatePage);
            }

            _pageVM.IsDirty = true;
        }

        private DynamicPageEditorViewModel _pageVM;
        private List<DynamicChildNodViewModelUndo> _oldList = new List<DynamicChildNodViewModelUndo>();
        private List<DynamicChildNodViewModelUndo> _newList = new List<DynamicChildNodViewModelUndo>();
    }
}
