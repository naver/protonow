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
   public class NodeViewModel : ViewModelBase, INodeViewModel
    {
        #region Constructor
        public NodeViewModel(IDocument document, UndoManager undoManager, ITreeNode treeNodeObject, NodeInfo info, bool isRootNode = false)
            : this(document, undoManager, treeNodeObject, null, info, isRootNode)
        {
        }

        public NodeViewModel(IDocument document, UndoManager undoManager, ITreeNode treeNodeObject, INodeViewModel parent) 
            : this(document, undoManager, treeNodeObject, parent, parent.NodeInfo, false)
        {

        }

        private NodeViewModel(IDocument document, UndoManager undoManager, ITreeNode treeNodeObject, INodeViewModel parent, NodeInfo info, bool isRootNode)
        {
            _document = document;
            _undoManager = undoManager;
            _treeNodeObject = treeNodeObject;

            IsRootNode = isRootNode;
            Parent = parent;
            NodeInfo = info;

            Children = new ObservableCollection<INodeViewModel>();

            this.PagePreviewCommand = new DelegateCommand<object>(PagePreviewExecute);
            this.EndPreviewCommand = new DelegateCommand<object>(EndPreviewExecute);

            if (treeNodeObject != null)
            {
                SetNodeImage();
            }
        }

        #endregion

        #region Command Handler
        public DelegateCommand<object> PagePreviewCommand { get; private set; }
        public DelegateCommand<object> EndPreviewCommand { get; private set; }

        private void PagePreviewExecute(object cmdParameter)
        {
            if (this.NodeType == TreeNodeType.Folder)
                return;
            var values = (object[])cmdParameter;

            try
            {
                System.Windows.Point ptInGrid = Mouse.GetPosition(values[0] as Grid);
                System.Windows.Point ptInItem = Mouse.GetPosition(values[2] as StackPanel);
                Popup popup = values[1] as Popup;
                popup.IsOpen = true;
                popup.VerticalOffset = ptInGrid.Y - ptInItem.Y;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                _ListEventAggregator.GetEvent<PagePreviewEvent>().Publish(this.Guid);
            }
        }

        private void EndPreviewExecute(object cmdParameter)
        {
            Popup popup = cmdParameter as Popup;
            popup.IsOpen = false;
        }
        #endregion

        #region Private Member
        private string nodeImage;
        IDocument _document;
        private UndoManager _undoManager;
        private ITreeNode _treeNodeObject;
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
        public NodeInfo NodeInfo { get; set; }
        public INodeViewModel Parent { get; set; }
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
                _treeNodeObject.Name = value;
                FirePropertyChanged("Name");
                _ListEventAggregator.GetEvent<RenamePageEvent>().Publish(_treeNodeObject.Guid);
            }
        }

        public string NodeImage
        {
            get
            {
                return nodeImage;
            }
            set
            {
                nodeImage = value;
                FirePropertyChanged("NodeImage");
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

        private bool _isMultiSelected;
        public bool IsMultiSelected
        {
            get { return _isMultiSelected; }
            set
            {
                if (_isMultiSelected != value)
                {
                    _isMultiSelected = value;
                    FirePropertyChanged("IsMultiSelected");
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
        public ObservableCollection<INodeViewModel> Children { get; set; }
        #endregion

        #region Public function(DOM)
        public INodeViewModel Add(string name)
        {
            INodeViewModel newNode;
            if (this.IsRootNode)
            {
                ITreeNode treeNode = _treeNodeObject.AddChild(TreeNodeType.Page);
                treeNode.AttachedObject = CreatePage(name);
                newNode = new NodeViewModel(_document, _undoManager, treeNode, this);
                Children.Add(newNode);
            }
            else
            {
                newNode = InsertSiblingAfter(name);
            }
            return newNode;
        }
        public INodeViewModel AddFolder(string name)
        {
            NodeViewModel newNode;
            if (this.IsRootNode)
            {
                ITreeNode treeNode = _treeNodeObject.AddChild(TreeNodeType.Folder);
                treeNode.Name = name;

                newNode = new NodeViewModel(_document, _undoManager, treeNode, this);
                Children.Add(newNode);
            }
            else
            {
                ITreeNode treeNode = _treeNodeObject.InsertSiblingAfter(TreeNodeType.Folder);
                treeNode.Name = name;

                var idx = IndexInParent;
                newNode = new NodeViewModel(_document, _undoManager, treeNode, Parent);
                Parent.Children.Insert(++idx, newNode);
            }
            return newNode;
        }
        public INodeViewModel AddChild(string name)
        {
            ITreeNode treeNode = _treeNodeObject.AddChild(TreeNodeType.Page);
            treeNode.AttachedObject = CreatePage(name);

            IsExpanded = true;
            NodeViewModel newNode = new NodeViewModel(_document, _undoManager, treeNode, this);

            Children.Add(newNode);
            RefreshInfoCount(1);
            return newNode;
        }
        public void InsertChild(string name, int index)
        {
            ITreeNode treeNode = _treeNodeObject.InsertChild(TreeNodeType.Page, index);
            treeNode.AttachedObject = CreatePage(name);

            IsExpanded = true;
            Children.Insert(index, (new NodeViewModel(_document, _undoManager, treeNode, this)));
            RefreshInfoCount(1);
        }

        public void InsertChild(INodeViewModel node, int index)
        {
            _treeNodeObject.InsertChild(node.TreeNodeObject, index);

            AddPage(node);

            IsExpanded = true;
            Children.Insert(index, node);
            RefreshInfoCount(1);
        }

        public INodeViewModel InsertSiblingAfter(string name)
        {
            ITreeNode treeNode = _treeNodeObject.InsertSiblingAfter(TreeNodeType.Page);
            treeNode.AttachedObject = CreatePage(name);

            var idx = IndexInParent;
            NodeViewModel newNode = new NodeViewModel(_document, _undoManager, treeNode, Parent);
            Parent.Children.Insert(++idx, newNode);

            return newNode;
        }

        public INodeViewModel InsertSiblingBefore(string name)
        {
            ITreeNode treeNode = _treeNodeObject.InsertSiblingBefore(TreeNodeType.Page);
            treeNode.AttachedObject = CreatePage(name);

            NodeViewModel newNode = new NodeViewModel(_document, _undoManager, treeNode, Parent);
            Parent.Children.Insert(IndexInParent, newNode);
            return newNode;
        }
        public void Move(INodeViewModel newParentNode, int index)
        {
            _treeNodeObject.Move(newParentNode.TreeNodeObject, index);

            if (Parent != null)
            {
                Parent.Children.Remove(this);
                RefreshInfoCount(-1);
            }

            IsSelected = false;
            newParentNode.Children.Insert(index, this);
            this.Parent = newParentNode;
            IsSelected = true;
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
        public void MoveUp()
        {
            if (Parent != null)
            {
                var idx = IndexInParent;
                if (idx > 0)
                {
                    Move(Parent, --idx);
                }
            }
        }
        public void MoveDown()
        {
            if (Parent != null)
            {
                var idx = IndexInParent;
                if (idx < Parent.Children.Count - 1)
                {
                    Move(Parent, ++idx);
                }
            }
        }
        //move brother node to child node
        public void Indent()
        {

            var idx = Parent.Children.IndexOf(this);
            if (idx > 0)
            {
                INodeViewModel parentNode = Parent.Children.ElementAt(idx - 1);
                Move(parentNode, 0);
                parentNode.IsExpanded = true;
            }
        }
        //move child node to brother node
        public void Outdent()
        {
            if (!Parent.IsRootNode)
            {
                IsExpanded = true;
                Move(Parent.Parent, Parent.IndexInParent + 1);
            }
        }

        public void DragTo(INodeViewModel newParentNode, int index)
        {
            IsExpanded = true;
            Move(newParentNode, index);
            newParentNode.IsExpanded = true;
        }

        public void UpdateDocument(IDocument doc)
        {
            if(doc!=null)
            {
                _document = doc;
                _treeNodeObject = _document.DocumentSettings.LayoutSetting.PageTree;
            }
        }

        #region Duplicate
        /// <summary>
        /// Duplicate page,folder or Branch
        /// </summary>
        /// <param name="bBranch">Is duplicate branch or node only</param>
        /// <returns></returns>
        public INodeViewModel Duplicate(bool bBranch = true)
        {
            if (_document == null)
                return null;

            string nodename = GetCopyeName(this.Name);
            ITreeNode treeNode = _treeNodeObject.InsertSiblingAfter(NodeType);
            if (this.NodeType == TreeNodeType.Page)
            {
                IDocumentPage sourcePage = this.TreeNodeObject.AttachedObject;
                IDocumentPage newPage = _document.DuplicatePage(sourcePage.Guid);
                treeNode.AttachedObject = newPage;
                _document.AddPage(newPage);

                //open this page
                if(_ListEventAggregator!=null)
                {
                    _ListEventAggregator.GetEvent<OpenNormalPageEvent>().Publish(newPage.Guid);
                }
                
            }
            else
            {
                treeNode.Name = nodename;
            }
            var idx = IndexInParent;

            NodeViewModel newNode = new NodeViewModel(_document, _undoManager, treeNode, Parent);
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
        public void DuplicateChild(INodeViewModel parent)
        {
            if (_document == null)
                return ;

            ITreeNode treeNode = parent.TreeNodeObject.AddChild(NodeType);
            if (this.NodeType == TreeNodeType.Page)
            {
                IDocumentPage sourcePage = this.TreeNodeObject.AttachedObject;
                IDocumentPage newPage = _document.DuplicatePage(sourcePage.Guid);
                treeNode.AttachedObject = newPage;
                _document.AddPage(newPage);
            }
            else
            {
                treeNode.Name = this.Name;
            }

            var idx = IndexInParent;
            NodeViewModel newNode = new NodeViewModel(_document, _undoManager, treeNode, parent);
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
        void SetNodeImage()
        {
            switch (NodeType)
            {
                case TreeNodeType.Page:
                    if (_document.DocumentType == DocumentType.Standard)
                    {
                        NodeImage = "Resources/Images/icon-16-add-page.png";// new BitmapImage(new Uri("Resources/Images/file.png",UriKind.Relative));
                    }
                    else
                    {
                        NodeImage = "Resources/Images/icon-16-add-object.png";// new BitmapImage(new Uri("Resources/Images/file.png",UriKind.Relative));
                    }
                    
                    break;
                //case TreeNodeType.Master:
                //    NodeImage = "Resources/Images/master.png";
                //    break;
                case TreeNodeType.Folder:
                    NodeImage = "Resources/Images/icon-16-add-folder.png";
                    break;
                default:
                    NodeImage = "Resources/Images/icon-16-add-page.png";// new BitmapImage(new Uri("Resources/Images/file.png", UriKind.Relative));
                    break;
            }
        }
        private IDocumentPage CreatePage(string name)
        {
            if (_document != null)
            {
                return _document.CreatePage(name);
            }

            return null;
        }

        private void AddPage(INodeViewModel node, bool isRecursive = true)
        {
            if (_document != null)
            {
                if (node.NodeType == TreeNodeType.Page)
                {
                    _document.AddPage(node.TreeNodeObject.AttachedObject);
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
            if (_document != null && treeNodeObject != null)
            {
                if (treeNodeObject.NodeType == TreeNodeType.Page)
                {
                    IEnumerable<IWidget> extendPageWidget = null;
                    IPage page = null;
                    if (_document.Pages.Contains(treeNodeObject.Guid) && _document.Pages[treeNodeObject.Guid] != null)
                    {
                        page = _document.Pages[treeNodeObject.Guid];
                        if (page.IsOpened == false)
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
                    }

                    _document.DeletePage(treeNodeObject.Guid);
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
        public void ApplyFilter(string criteria, Stack<INodeViewModel> ancestors)
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

        private bool _isDragInto;
        public bool IsDragInto
        {
            get { return this._isDragInto; }
            set
            {
                if (this._isDragInto != value)
                {
                    this._isDragInto = value;

                    if (value)
                    {
                        DragIntoBorder = new SolidColorBrush(Color.FromRgb(0x00, 0x9d, 0xd9));
                    }
                    else
                    {
                        DragIntoBorder = null;
                    }

                    FirePropertyChanged("IsDragInto");
                    FirePropertyChanged("DragIntoBorder");
                }
            }
        }

        public SolidColorBrush DragIntoBorder { get; set; }
    }
}
