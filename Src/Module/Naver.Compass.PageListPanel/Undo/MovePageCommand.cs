using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.Module
{
    class MovePageCommand : IUndoableCommand
    {
        public MovePageCommand(INodeViewModel node, INodeViewModel newParent, int indexInNewParent,
                               INodeViewModel oldParent, int indexInOldParent)
        {
            _node = node;
            _newParent = newParent;
            _indexInNewParent = indexInNewParent;
            _oldParent = oldParent;
            _indexInOldParent = indexInOldParent;
        }

        public void Undo()
        {
            _node.Move(_oldParent, _indexInOldParent);
        }

        public void Redo()
        {
            _node.Move(_newParent, _indexInNewParent);
        }

        private INodeViewModel _node;
        private INodeViewModel _newParent;
        private int _indexInNewParent;
        private INodeViewModel _oldParent;
        private int _indexInOldParent;
    }
}
