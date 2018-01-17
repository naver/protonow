using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.Module
{
    class CreatePageCommand : IUndoableCommand 
    {
        public CreatePageCommand(PageListViewModel pageListVM, INodeViewModel node, INodeViewModel parent, int indexInParent, TreeView tree)
        {
            _tree = tree;
            _pageListVM = pageListVM;
            _node = node;
            _parent = parent;
            _indexInParent = indexInParent;
        }

        public void Undo()
        {
            _node.Remove();
            FocusTree();
        }

        public void Redo()
        {
            _parent.InsertChild(_node, _indexInParent);

            _pageListVM.NodeInfo.SelectedNode = _node;
            FocusTree();
        }

        private void FocusTree()
        {
            if (_tree != null)
            {
                _tree.Focus();
            }
        }

        private TreeView _tree;
        private PageListViewModel _pageListVM;
        private INodeViewModel _node;
        private INodeViewModel _parent;
        private int _indexInParent;
    }
}
