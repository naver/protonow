using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum TreeNodeType
    {
        None,
        Folder,
        Page,
        MasterPage
    }

    public interface ITreeNode
    {
        ITreeNode AddChild(TreeNodeType nodeType);

        ITreeNode InsertChild(TreeNodeType nodeType, int index);
        
        void InsertChild(ITreeNode node, int index);

        ITreeNode InsertSiblingAfter(TreeNodeType nodeType);

        ITreeNode InsertSiblingBefore(TreeNodeType nodeType);

        void RemoveChild(int index);

        void RemoveChild(ITreeNode node);

        void RemoveAllChildren();

        void RemoveMe();

        void Move(ITreeNode newParentNode, int index);

        int IndexOf(ITreeNode child);

        string Name { get; set; }

        Guid Guid { get; }
        
        TreeNodeType NodeType { get; }
        
        IDocumentPage AttachedObject { get; set; }

        ITreeNode ParentNode { get; set; }
        
        List<ITreeNode> ChildNodes { get; }

        bool IsRootNode { get; }

        int ChildNodesCount { get; }

        int IndexInParent { get; }

        bool IsExpanded { get; set; }
    }
}
