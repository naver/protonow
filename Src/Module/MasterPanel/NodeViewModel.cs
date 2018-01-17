using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Common.CommonBase;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace Naver.Compass.Module
{
    public class NodeViewModel : ViewModelBase
    {
        ITreeNode _treeNodeObject;
        #region Constructor
        public NodeViewModel(UndoManager undoManager, ITreeNode treeNodeObject, NodeInfo info, bool isRootNode = false)
        {
            _undoManager = undoManager;
            _treeNodeObject = treeNodeObject;

            IsRootNode = isRootNode;
            NodeInfo = info;

            InitData();
        }
        private NodeViewModel(UndoManager undoManager, ITreeNode treeNodeObject, NodeViewModel parent)
        {
            _undoManager = undoManager;
            _treeNodeObject = treeNodeObject;

            IsRootNode = false;
            Parent = parent;
            NodeInfo = parent.NodeInfo;
            InitData();
        }

        private void InitData()
        {
            Children = new ObservableCollection<NodeViewModel>();
        }
        #endregion

        #region Private Member
        IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }
        #endregion

        #region Public Property
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
        public NodeInfo NodeInfo { get; private set; }
        public NodeViewModel Parent { get; set; }
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
        public string Name
        {
            get
            {
                return _treeNodeObject.Name;
            }
            set
            {
                if (_treeNodeObject.Name != value && value != string.Empty)
                {
                    string oldValue = _treeNodeObject.Name;
                    Raw_Name = value;

                    if (_undoManager != null)
                    {
                        PropertyChangeCommand cmd = new PropertyChangeCommand(this, "Raw_Name", oldValue, value);
                        _undoManager.Push(cmd);
                    }
                }
            }
        }

        public string Raw_Name
        {
            set
            {
                _document.IsDirty = true;
                _treeNodeObject.Name = value;
                FirePropertyChanged("Name");
                _ListEventAggregator.GetEvent<RenamePageEvent>().Publish(_treeNodeObject.Guid);
            }
        }
        public bool IsExpanded
        {
            get
            {
                return _treeNodeObject.IsExpanded;
            }
            set
            {
                _document.IsDirty = true;
                _treeNodeObject.IsExpanded = value;
                FirePropertyChanged("IsExpanded");
                OnIsExpandedChanged();
            }
        }
        bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value || (value && NodeInfo.SelectedNode != this))
                {
                    isSelected = value;
                    FirePropertyChanged("IsSelected");
                    OnIsSelectedChanged();
                }
            }
        }

        bool isNodeEditable;
        public bool IsNodeEditable
        {
            get { return isNodeEditable; }
            set
            {
                if (isNodeEditable != value)
                {
                    isNodeEditable = value;
                    FirePropertyChanged("IsNodeEditable");
                }
            }
        }

        bool isEditboxFocus;
        public bool IsEditboxFocus
        {
            get { return isEditboxFocus; }
            set
            {
                if (isEditboxFocus != value)
                {
                    isEditboxFocus = value;
                    FirePropertyChanged("IsEditboxFocus");
                }
            }
        }
        public bool IsRootNode { get; set; }
        public int IndexInParent
        {
            get { return Parent.Children.IndexOf(this); }
        }
        public ObservableCollection<NodeViewModel> Children { get; set; }
        #endregion

        #region Public function(DOM)
        public NodeViewModel Add(string name, IMasterPage masterPage = null)
        {
            _document.IsDirty = true;
            NodeViewModel newNode;
            if (this.IsRootNode)
            {
                ITreeNode treeNode = _treeNodeObject.AddChild(TreeNodeType.MasterPage);
                if(masterPage == null)
                {
                    treeNode.AttachedObject = CreatePage(name);
                }
                else
                {
                    treeNode.AttachedObject = masterPage;
                }
                newNode = new NodeViewModel(_undoManager, treeNode, this);
                Children.Add(newNode);
            }
            else
            {
                newNode = AddNodeAfter(name, masterPage);
            }
            return newNode;
        }

        /// <summary>
        /// Add new master to the end of the list.
        /// </summary>
        public NodeViewModel AddNodeAfter(string name, IMasterPage masterPage = null)
        {
            _document.IsDirty = true;
            ITreeNode treeNode = _treeNodeObject.ParentNode.AddChild(TreeNodeType.MasterPage);
            if (masterPage == null)
            {
                treeNode.AttachedObject = CreatePage(name);
            }
            else
            {
                treeNode.AttachedObject = masterPage;
            }

            NodeViewModel newNode = new NodeViewModel(_undoManager, treeNode, Parent);
            Parent.Children.Add(newNode);

            return newNode;
        }
        public void InsertChild(NodeViewModel node, int index)
        {
            _document.IsDirty = true;
            _treeNodeObject.InsertChild(node._treeNodeObject, index);

            AddPage(node);

            IsExpanded = true;
            Children.Insert(index, node);
            RefreshInfoCount(1);
        }

        /// <summary>
        /// Remove page
        /// </summary>
        /// <returns>Current select Note</returns>
        public void Remove()
        {
            if (Parent != null)
            {
                int indexInparent = IndexInParent;
                _treeNodeObject.RemoveMe();

                DeletePage(_treeNodeObject);

                Parent.Children.Remove(this);

                _document.IsDirty = true;

                if (Parent.Children.Count == 0)
                {
                    Parent.IsSelected = true;
                }
                else if (indexInparent >= Parent.Children.Count)
                {
                    Parent.Children.ElementAt(indexInparent - 1).IsSelected = true;
                }
                else
                {
                    Parent.Children.ElementAt(indexInparent).IsSelected = true;
                }
            }
        }
      
        #region Duplicate
        /// <summary>
        /// Duplicate page,folder or Branch
        /// </summary>
        /// <param name="bBranch">Is duplicate branch or node only</param>
        /// <returns></returns>
        public NodeViewModel Duplicate(bool bBranch = true)
        {
            string nodename = GetCopyeName(this.Name);
            _document.IsDirty = true;
            ITreeNode treeNode = _treeNodeObject.InsertSiblingAfter(NodeType);
            if (this.NodeType == TreeNodeType.MasterPage)
            {
                IDocumentPage sourcePage = this.TreeNodeObject.AttachedObject;
                IMasterPage newPage = _document.DuplicateMasterPage(sourcePage.Guid);
                treeNode.AttachedObject = newPage;
                _document.AddMasterPage(newPage);

                //open this page
                _ListEventAggregator.GetEvent<OpenMasterPageEvent>().Publish(newPage.Guid);
            }
            else
            {
                treeNode.Name = nodename;
            }
            var idx = IndexInParent;

            NodeViewModel newNode = new NodeViewModel(_undoManager, treeNode, Parent);
            newNode.IsExpanded = true;
            newNode.IsNodeEditable = true;
            newNode.IsSelected = true;
            Parent.Children.Insert(++idx, newNode);

            //clone child node page
            if (bBranch)
            {
                foreach (var item in Children)
                {
                    item.DuplicateChild(newNode);
                }
            }
            return newNode;
        }
        public void DuplicateChild(NodeViewModel parent)
        {
            ITreeNode treeNode = parent._treeNodeObject.AddChild(NodeType);
            if (this.NodeType == TreeNodeType.MasterPage)
            {
                IDocumentPage sourcePage = this.TreeNodeObject.AttachedObject;
                IMasterPage newPage = _document.DuplicateMasterPage(sourcePage.Guid);
                treeNode.AttachedObject = newPage;
                _document.AddMasterPage(newPage);
            }
            else
            {
                treeNode.Name = this.Name;
            }

            var idx = IndexInParent;
            NodeViewModel newNode = new NodeViewModel(_undoManager, treeNode, parent);
            parent.Children.Insert(idx, newNode);
            newNode.IsExpanded = true;
            foreach (var item in Children)
            {
                item.DuplicateChild(newNode);
            }
        }

        #endregion

        #endregion

        #region Private function
        void RefreshInfoCount(int addition)
        {
            if (NodeInfo != null)
            {
                NodeInfo.SetCount(NodeInfo.Count + addition);
            }
        }
        private IDocumentPage CreatePage(string name)
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null)
            {
                return doc.Document.CreateMasterPage(name);
            }

            return null;
        }

        private void AddPage(NodeViewModel node, bool isRecursive = true)
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null)
            {
                if (node.NodeType == TreeNodeType.MasterPage)
                {
                    doc.Document.AddMasterPage(node.TreeNodeObject.AttachedObject as IMasterPage);
                }

                if (isRecursive)
                {
                    foreach (NodeViewModel child in node.Children)
                    {
                        AddPage(child, isRecursive);
                    }
                }
            }
        }

        private void DeletePage(ITreeNode treeNodeObject, bool isRecursive = true)
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null && treeNodeObject != null)
            {
                if (treeNodeObject.NodeType == TreeNodeType.MasterPage)
                {
                    IEnumerable<IWidget> extendPageWidget = null;
                    IPage page = null;
                    if (doc.Document.MasterPages.Contains(treeNodeObject.Guid) && doc.Document.MasterPages[treeNodeObject.Guid] != null)
                    {
                        page = doc.Document.MasterPages[treeNodeObject.Guid];
                        bool isOpened = page.IsOpened;
                        if (isOpened == false)
                        {
                            page.Open();
                        }
                        extendPageWidget = page.Widgets.OfType<IWidget>().Where(x => x is Naver.Compass.Service.Document.IDynamicPanel || x is Naver.Compass.Service.Document.IHamburgerMenu || x is Naver.Compass.Service.Document.IToast);

                        if (extendPageWidget != null)
                        {
                            foreach (var extendpage in extendPageWidget)
                            {
                                _ListEventAggregator.GetEvent<ClosePageEvent>().Publish(extendpage.Guid);
                            }
                        }

                        if(!isOpened)
                        {
                            page.Close();
                        }
                    }           

                    doc.Document.DeleteMasterPage(treeNodeObject.Guid);
                    _ListEventAggregator.GetEvent<ClosePageEvent>().Publish(treeNodeObject.Guid);
                    _ListEventAggregator.GetEvent<FocusSitemapEvent>().Publish(null);


                    RefreshInfoCount(-1);
                }

                if (isRecursive)
                {
                    foreach (ITreeNode child in treeNodeObject.ChildNodes)
                    {
                        DeletePage(child, isRecursive);
                    }
                }
            }
        }

        private string GetCopyeName(string name)
        {
            return string.Concat("Copy of ", name);
        }

        #endregion

        #region search

        private Visibility isMatch;
        public Visibility IsMatch
        {
            get { return isMatch; }
            set
            {
                if (value != isMatch)
                {
                    isMatch = value;
                    FirePropertyChanged("IsMatch");
                }
            }
        }
        public void ApplyFilter(string criteria, Stack<NodeViewModel> ancestors)
        {
            if (IsFilterMatched(criteria))
            {
                IsMatch = Visibility.Visible;
                foreach (var ancestor in ancestors)
                {
                    ancestor.IsMatch = Visibility.Visible;
                }
            }
            else
                IsMatch = Visibility.Collapsed;

            ancestors.Push(this);
            foreach (var child in Children)
                child.ApplyFilter(criteria, ancestors);

            ancestors.Pop();
        }

        private bool IsFilterMatched(string criteria)
        {
            return String.IsNullOrEmpty(criteria) || Name.ToLower().Contains(criteria.ToLower());
        }
        #endregion

        #region Event

        public event EventHandler IsExpandedChanged;
        public event EventHandler IsSelectedChanged;

        protected virtual void OnIsExpandedChanged()
        {
            if (IsExpandedChanged != null)
                IsExpandedChanged(this, EventArgs.Empty);
        }

        protected virtual void OnIsSelectedChanged()
        {
            if (IsSelectedChanged != null)
                IsSelectedChanged(this, EventArgs.Empty);

            if (IsSelected)
                NodeInfo.SelectedNode = this;

            if (IsSelected)
                IsEditboxFocus = true;
            else
                IsEditboxFocus = false;
        }

        #endregion

        private UndoManager _undoManager;
    }
}
