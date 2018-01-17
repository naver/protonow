using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Collections.ObjectModel;
using Naver.Compass.InfoStructure;
using Microsoft.Practices.ServiceLocation;
using System.Windows;

namespace Naver.Compass.Module
{
    /// <summary>
    /// Define node of PageTree in CaseEditor
    /// </summary>
    public class PageNode:ViewModelBase
    {
        public PageNode()
        {
            Children = new ObservableCollection<PageNode>();
        }
        public PageNode(ITreeNode treeNodeObject)
        {
            Children = new ObservableCollection<PageNode>();
            _treeNodeObject = treeNodeObject;
            SetNodeImage();
        }
        ITreeNode _treeNodeObject;
        public ITreeNode TreeNodeObject
        {
            get
            {
                return _treeNodeObject;
            }
            set
            {
                _treeNodeObject = value;
            }
        }

        public ObservableCollection<PageNode> Children { get; set; }
        public Guid Guid
        {
            get
            {
                return _treeNodeObject.Guid;
            }
        }
        public TreeNodeType NodeType
        {
            get
            {
                return _treeNodeObject.NodeType;
            }
        }

        public Visibility IsPageNode
        {
            get
            {
                if(NodeType == TreeNodeType.Page)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        private string nodeImage;
        public string NodeImage
        {
            get
            {
                return nodeImage;
            }
            set
            {
                nodeImage = value;
            }
        }
        public string Name
        {
            get
            {
                return _treeNodeObject.Name;
            }

        }
        private bool isExpanded = true;
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                isExpanded = value;
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if(isSelected!=value)
                {
                    isSelected = value;
                    FirePropertyChanged("IsSelected");
                }
            }
        }

        void SetNodeImage()
        {
            if(NodeType == TreeNodeType.Page)
            {
                NodeImage = "pack://application:,,,/Naver.Compass.PageListPanel;component/Resources/Images/icon-16-add-page.png";
            }
            else
            {
                NodeImage = "pack://application:,,,/Naver.Compass.PageListPanel;component/Resources/Images/icon-16-add-folder.png";
            }
        }
    }
}
