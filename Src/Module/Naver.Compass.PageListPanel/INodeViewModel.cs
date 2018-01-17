using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module
{
    public interface INodeViewModel
    {
        ITreeNode TreeNodeObject { get; set; }
        INodeViewModel Parent { get; set; }
        Guid Guid { get; }
        TreeNodeType NodeType { get; }
        ObservableCollection<INodeViewModel> Children { get; set; }
        NodeInfo NodeInfo { get; set; }
        string Name { get; set; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        bool IsMultiSelected { get; set; }
        bool IsNodeEditable { get; set; }
        bool IsRootNode { get; set; }
        int IndexInParent { get; }
        Visibility IsMatch { get; set; }
        INodeViewModel Add(string name);
        INodeViewModel AddFolder(string name);
        INodeViewModel AddChild(string name);
        void InsertChild(INodeViewModel node, int index);
        INodeViewModel InsertSiblingAfter(string name);
        INodeViewModel InsertSiblingBefore(string name);
        void Move(INodeViewModel newParentNode, int index);
        void Remove();
        void MoveUp();
        void MoveDown();
        void Indent();
        void Outdent();
        void DragTo(INodeViewModel newParentNode, int index);
        INodeViewModel Duplicate(bool bBranch = true);
        void DuplicateChild(INodeViewModel parent);
        void ApplyFilter(string criteria, Stack<INodeViewModel> ancestors);
        bool IsDragInto { get; set; }
    }
}
