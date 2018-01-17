using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using System.Windows.Input;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Controls;
using System.Windows.Data;
using Naver.Compass.Common.CommonBase;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using Naver.Compass.Common.Helper;

namespace Naver.Compass.Module
{
    public class PageListViewModel : ViewModelBase, ICommandSink, ISupportUndo
    {
        public PageListViewModel()
        {

        }
        public PageListViewModel(TreeView tree)
        {
            _pageTree = tree;
            _commandSink = new CommandSink();
            _commandSink.RegisterCommand(ApplicationCommands.Undo, param => CanRunUndoCommand, UndoCommandHandler);
            _commandSink.RegisterCommand(ApplicationCommands.Redo, param => CanRunRedoCommand, RedoCommandHandler);

            if (!IsInDesignMode)
            {
                this.PageNewCommand = new DelegateCommand<object>(AddPageExecute, CanExecuteAddPage);
                this.FolderNewCommand = new DelegateCommand<object>(AddFolderExecute, CanExecuteAddFolder);

                this.PageDeleteCommand = new DelegateCommand<object>(DaletePageExecute, CanExecuteDelete);
                this.PageUpCommand = new DelegateCommand<object>(PageUpExecute, CanExecuteUp);
                this.PageDownCommand = new DelegateCommand<object>(PageDownExecute, CanExecuteDown);
                this.PageOutdentCommand = new DelegateCommand<object>(PageOutdentExecute, CanExecuteOutdent);
                this.PageIndentCommand = new DelegateCommand<object>(PageIndentExecute, CanExecuteIndent);
                this.PageSearchCommand = new DelegateCommand<object>(PageSearchExecute, CanExecuteSearch);
                this.OpenPageCommand = new DelegateCommand<object>(OpenPageExecute);
                this.EditNodeCommand = new DelegateCommand<object>(EditNodeExecute);
                this.DeselectNodeCommand = new DelegateCommand<object>(DeselectNodeExecute);
                this.InsertSiblingBeforeCommand = new DelegateCommand<object>(InsertSiblingBeforeExecute);
                this.InsertSiblingAfterCommand = new DelegateCommand<object>(InsertSiblingAfterExecute);
                this.AddChildCommand = new DelegateCommand<object>(AddChildExecute);
                this.LeavePageTreeCommand = new DelegateCommand<object>(LeavePageTreeExecute);
                this.DuplicatePageCommand = new DelegateCommand<object>(DuplicateNodeExecute);
                this.DuplicateFolderCommand = new DelegateCommand<object>(DuplicateNodeExecute);
                this.DuplicateBranchCommand = new DelegateCommand<object>(DuplicateBranchExecute);
                this.PageClickCommand = new DelegateCommand<object>(PageClickExecute);
                this.DeleteSearchCommand = new DelegateCommand<object>(DeleteSearchExecute);
                this.SourceUpdatedCommand = new DelegateCommand<object>(PageTreeUpdatedExecute);

                NodeInfo = new NodeInfo();
                NodeInfo.SelectedNodeChanged += (s, e) => RefreshCommands();
                RootNode = new NodeViewModel(_document, _undoManager, null, NodeInfo, true);

                // CreateDefaultPages();

                _ListEventAggregator.GetEvent<DomLoadedEvent>().Subscribe(DomLoadedEventHandler, ThreadOption.UIThread);
                _ListEventAggregator.GetEvent<PagePreviewEvent>().Subscribe(PagePreviewEventHandler, ThreadOption.UIThread);
                _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler, ThreadOption.UIThread);
                _ListEventAggregator.GetEvent<AddNewPageEvent>().Subscribe(AddPageRequestExecute, ThreadOption.UIThread);
                _ListEventAggregator.GetEvent<FocusSitemapEvent>().Subscribe(FocusSitemapExecute, ThreadOption.UIThread);
                _ListEventAggregator.GetEvent<SiteMapEvent>().Subscribe(OnSiteMapEvent, ThreadOption.UIThread);
            }
        }

        #region Property and command
        public NodeViewModel RootNode { get; private set; }
        public NodeInfo NodeInfo { get; private set; }

        public DelegateCommand<object> PageNewCommand { get; private set; }
        public DelegateCommand<object> FolderNewCommand { get; private set; }
        public DelegateCommand<object> PageDeleteCommand { get; private set; }
        public DelegateCommand<object> PageUpCommand { get; private set; }
        public DelegateCommand<object> PageDownCommand { get; private set; }
        public DelegateCommand<object> PageIndentCommand { get; private set; }
        public DelegateCommand<object> PageOutdentCommand { get; private set; }
        public DelegateCommand<object> PageLockCommand { get; private set; }

        public DelegateCommand<object> PageSearchCommand { get; private set; }
        public DelegateCommand<object> OpenPageCommand { get; private set; }
        public DelegateCommand<object> DeselectNodeCommand { get; private set; }
        public DelegateCommand<object> EditNodeCommand { get; private set; }
        public DelegateCommand<object> InsertSiblingBeforeCommand { get; private set; }
        public DelegateCommand<object> InsertSiblingAfterCommand { get; private set; }
        public DelegateCommand<object> AddChildCommand { get; private set; }
        public DelegateCommand<object> DuplicatePageCommand { get; private set; }
        public DelegateCommand<object> DuplicateFolderCommand { get; private set; }
        public DelegateCommand<object> DuplicateBranchCommand { get; private set; }
        public DelegateCommand<object> LeavePageTreeCommand { get; private set; }
        public DelegateCommand<object> PageClickCommand { get; private set; }
        public DelegateCommand<object> DeleteSearchCommand { get; private set; }
        public DelegateCommand<object> SourceUpdatedCommand { get; private set; }

        public Visual _preCanvas;
        public Visual PreCanvas
        {
            get
            {
                //PreviewModel pre = ServiceLocator.Current.GetInstance<PreviewModel>();
                //_preCanvas=pre.PreviviewCanvas;
                return _preCanvas;
            }
            set
            {
                if (_preCanvas != value)
                {
                    _preCanvas = value;
                    FirePropertyChanged("PreCanvas");
                }
            }
        }


        private string searchPageName;
        public string SearchPageName
        {
            get { return searchPageName; }
            set
            {
                if (searchPageName != value)
                {
                    searchPageName = value;
                    FirePropertyChanged("SearchPageName");
                    FirePropertyChanged("DeleteSearchVisibility");
                    ApplyFilter();
                }
            }
        }
        private Visibility searchBoxVisibility = Visibility.Collapsed;
        public Visibility SearchBoxVisibility
        {
            get
            {
                return searchBoxVisibility;
            }
            set
            {
                if (searchBoxVisibility != value)
                {
                    searchBoxVisibility = value;
                    FirePropertyChanged("SearchBoxVisibility");
                }
            }
        }

        public Visibility IsPageNode
        {
            get
            {
                if (NodeInfo.SelectedNode.NodeType == TreeNodeType.Page)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public Visibility IsFolderNode
        {
            get
            {
                if (NodeInfo.SelectedNode.NodeType == TreeNodeType.Folder)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }


        public Visibility DeleteSearchVisibility
        {
            get
            {
                if(string.IsNullOrEmpty(searchPageName))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
        #endregion

        #region Event handler and load Files function
        void DomLoadedEventHandler(FileOperationType loadType)
        {
            //close all page opened.
            _ListEventAggregator.GetEvent<ClosePageEvent>().Publish(Guid.Empty);

            //if loadType is Loaded, as follows
            SetOperationNode(RootNode);
            RootNode.Children.Clear();
            switch (loadType)
            {
                case FileOperationType.Create:
                    if (_document != null)
                    {
                        InitParameter(true);
                        CreateDefaultPages();

                        _undoManager.Clear();
                    }
                    break;
                case FileOperationType.Open:
                    if (_document != null)
                    {
                        InitParameter(true);
                        RootNode.UpdateDocument(_document);
                        LoadNodeViewModelFromTreeNodeObject(RootNode, RootNode.TreeNodeObject);

                        _undoManager.Clear();
                    }
                    break;
                case FileOperationType.Close:
                    InitParameter(false);

                    _undoManager.Clear();
                    break;
            }

        }

        void LoadNodeViewModelFromTreeNodeObject(NodeViewModel nodeVM, ITreeNode treeNodeObject)
        {
            if (nodeVM == null || treeNodeObject == null)
            {
                return;
            }
            foreach (ITreeNode treeNode in treeNodeObject.ChildNodes)
            {
                NodeViewModel node = new NodeViewModel(_document, _undoManager, treeNode, NodeInfo);
                nodeVM.Children.Add(node);
                node.Parent = nodeVM;
                if (_bFirstPage && node.NodeType != TreeNodeType.Folder)
                {//select and open the first page
                    _ListEventAggregator.GetEvent<OpenNormalPageEvent>().Publish(node.Guid);
                    node.IsSelected = true;
                    _bFirstPage = false;
                }

                LoadNodeViewModelFromTreeNodeObject(node, treeNode);
            }
        }

        void CreateDefaultPages()
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document == null)
            {
                doc.NewDocument(DocumentType.Standard);
            }

            RootNode.UpdateDocument(doc.Document);
            
            if (doc.Document.DocumentType == DocumentType.Library)
            {
                INodeViewModel defaultNode = RootNode.Add(GetNextDataName(TreeNodeType.Page));
                _ListEventAggregator.GetEvent<OpenNormalPageEvent>().Publish(defaultNode.Guid);
                defaultNode.IsSelected = true;
            }
            else
            {
                INodeViewModel homeNode = RootNode.Add("Home");
                _ListEventAggregator.GetEvent<OpenNormalPageEvent>().Publish(homeNode.Guid);
                homeNode.IsSelected = true;
                homeNode.AddChild(GetNextDataName(TreeNodeType.Page));
                homeNode.AddChild(GetNextDataName(TreeNodeType.Page));
                homeNode.AddChild(GetNextDataName(TreeNodeType.Page));
            }
          
            doc.Document.IsDirty = false;
        }

        void PagePreviewEventHandler(Guid pageGID)
        {
            if(pageGID == Guid.Empty)
            {
                return;
            }

            PreviewModel pre = ServiceLocator.Current.GetInstance<PreviewModel>();
            if (_document == null)
            {
                return;
            }

            IPage CurrentPage = _document.Pages.GetPage(pageGID);
            if (CurrentPage == null)
            {
                return;
            }
            if (CurrentPage.IsOpened==true)
                PreCanvas = pre.PreviviewCanvas;
            else
                PreCanvas = pre.PreviviewBox;
        }

        private void SelectionPageChangeHandler(Guid pageGuid)
        {
            //select page in treeview
            SelectPage(RootNode, pageGuid);
        }

        #endregion

        #region operate page list functions

        private void AddPageExecute(object cmdParameter)
        {
            INodeViewModel Node = GetOperationNode().Add(GetNextDataName(TreeNodeType.Page));
            Node.IsSelected = true;
            Node.IsNodeEditable = true;

            PageTreeChanged();

            CreatePageCommand cmd = new CreatePageCommand(this, Node, Node.Parent, Node.IndexInParent, _pageTree);
            _undoManager.Push(cmd);
        }

        private bool CanExecuteAddPage(object cmdParameter)
        {
            return _bCanAddPage && _bVisible && !this._isMultiSelected;
        }

        private void AddFolderExecute(object cmdParameter)
        {
            INodeViewModel Node = GetOperationNode().AddFolder(GetNextDataName(TreeNodeType.Folder));
            Node.IsSelected = true;
            Node.IsNodeEditable = true;

            PageTreeChanged();

            CreatePageCommand cmd = new CreatePageCommand(this, Node, Node.Parent, Node.IndexInParent, _pageTree);
            _undoManager.Push(cmd);
        }
        private bool CanExecuteAddFolder(object cmdParameter)
        {
            return _bCanAddFolder && _bVisible && !this._isMultiSelected && IsStandardDocument;
        }



        private void OnSiteMapEvent(SiteMapEventEnum Data)
        {

            switch (Data)
            {
                case SiteMapEventEnum.CreateNewPage:
                    if (CanExecuteAddPage(null))
                        AddPageExecute(null);
                    break;
                case SiteMapEventEnum.OpenHomePage:
                    OpenSiteMagePage(true);
                    break;
                case SiteMapEventEnum.OpenEndPage:
                    OpenSiteMagePage(false);
                    break;
                default:
                    break;
            }

        }

        private void OpenSiteMagePage(bool bHome)
        {
            if (RootNode != null && RootNode.Children.Count > 0)
            {
                if (bHome)
                {
                    INodeViewModel fitst = RootNode.Children.First();

                    _ListEventAggregator.GetEvent<OpenNormalPageEvent>().Publish(fitst.Guid);
                }
                else
                {
                    INodeViewModel LastChild = RootNode.Children.Last();
                    while (LastChild.Children.Count > 0)
                    {
                        LastChild = LastChild.Children.Last();
                    }
                    _ListEventAggregator.GetEvent<OpenNormalPageEvent>().Publish(LastChild.Guid);
                }
            }

        }

        private void DaletePageExecute(object cmdParameter)
        {
            if ((!GetOperationNode().IsRootNode) && _bVisible)
            {
                INodeViewModel Node = NodeInfo.SelectedNode;
                INodeViewModel parent = Node.Parent;
                int index = Node.IndexInParent;

                Node.Remove();

                PageTreeChanged();

                if (index >= 0)
                {
                    DeletePageCommand cmd = new DeletePageCommand(this, Node, parent, index, _pageTree);
                    _undoManager.Push(cmd);
                }
            }
            else if (this._isMultiSelected)
            {
                var multiSelected = GetAllNodes(RootNode).Where(n => n.IsMultiSelected);
                if (multiSelected.Count() > 0)
                {
                    var cmds = new Naver.Compass.InfoStructure.CompositeCommand();
                    var multiSelectedClone = multiSelected.ToList();
                    foreach (var node in multiSelectedClone)
                    {
                        var _node = node;
                        var _parent = node.Parent;
                        var _index = node.IndexInParent;
                        if (_index >= 0)
                        {
                            var cmd = new DeletePageCommand(this, _node, _parent, _index, _pageTree);
                            cmds.AddCommand(cmd);
                        }
                    }

                    foreach (var node in multiSelectedClone)
                    {
                        node.Remove();
                    }

                    PageTreeChanged();

                    _undoManager.Push(cmds);
                    this.ClearMultiSelected();
                }
            }

            if (_pageTree != null)
                _pageTree.Focus();
        }

        private bool CanExecuteDelete(object cmdParameter)
        {
            return this._isMultiSelected || ((!GetOperationNode().IsRootNode) && _bVisible);
        }

        private void PageUpExecute(object cmdParameter)
        {
            INodeViewModel Node = NodeInfo.SelectedNode;
            int oldIndex = Node.IndexInParent;
            Node.MoveUp();

            PageTreeChanged();

            MovePageCommand cmd = new MovePageCommand(Node, Node.Parent, Node.IndexInParent, Node.Parent, oldIndex);
            _undoManager.Push(cmd);

            if (_pageTree != null)
                _pageTree.Focus();
        }

        private bool CanExecuteUp(object cmdParameter)
        {
            INodeViewModel node = GetOperationNode();
            return (!node.IsRootNode) && (node.IndexInParent != 0) && _bVisible && !this._isMultiSelected;
        }

        private void PageDownExecute(object cmdParameter)
        {
            INodeViewModel Node = NodeInfo.SelectedNode;
            int oldIndex = Node.IndexInParent;
            Node.MoveDown();

            PageTreeChanged();

            MovePageCommand cmd = new MovePageCommand(Node, Node.Parent, Node.IndexInParent, Node.Parent, oldIndex);
            _undoManager.Push(cmd);

            if (_pageTree != null)
                _pageTree.Focus();
        }

        private bool CanExecuteDown(object cmdParameter)
        {
            INodeViewModel node = GetOperationNode();
            if (node.IsRootNode)
                return false;
            int index = node.IndexInParent;
            int count = node.Parent.Children.Count();
            return (index < count - 1) && _bVisible && !this._isMultiSelected;
        }

        private void PageIndentExecute(object cmdParameter)
        {
            INodeViewModel Node = NodeInfo.SelectedNode;

            INodeViewModel oldParent = Node.Parent;
            int oldIndex = Node.IndexInParent;

            Node.Indent();

            PageTreeChanged();

            MovePageCommand cmd = new MovePageCommand(Node, Node.Parent, Node.IndexInParent, oldParent, oldIndex);
            _undoManager.Push(cmd);

            if (_pageTree != null)
                _pageTree.Focus();
        }

        public void DragTo(NodeViewModel beDrag, NodeViewModel parent, NodeViewModel previousBrother)
        {
            NodeViewModel Node = beDrag;

            INodeViewModel oldParent = Node.Parent;
            int oldIndex = Node.IndexInParent;

            if (parent == null)
            {
                parent = RootNode;
            }

            var newIndex = 0;
            if (previousBrother != null)
            {
                ///如果前一个同级节点为空，那么index则为0
                ///否则进入此处逻辑判断
                if (previousBrother == Node)
                {
                    ///如果要拖动到的前一个同级节点就是自己
                    ///那么index不改变
                    newIndex = Node.IndexInParent;
                }
                else
                {
                    ///否则如果拖动到的前一个同级节点和自己在同一个父节点下面
                    if (previousBrother.Parent == Node.Parent)
                    {
                        if (Node.IndexInParent <= previousBrother.IndexInParent)
                        {
                            newIndex = previousBrother.IndexInParent;
                        }
                        else
                        {
                            newIndex = previousBrother.IndexInParent + 1;
                        }
                    }
                    else
                    {
                        newIndex = previousBrother.IndexInParent + 1;
                    }
                }
            }

            if (parent == oldParent && newIndex == oldIndex)
            {
                Debug.WriteLine("not changed");
            }
            else
            {
                Node.DragTo(parent, newIndex);

                PageTreeChanged();
            }

            MovePageCommand cmd = new MovePageCommand(Node, Node.Parent, Node.IndexInParent, oldParent, oldIndex);
            _undoManager.Push(cmd);
        }

        private bool CanExecuteIndent(object cmdParameter)
        {
            return CanExecuteUp(cmdParameter) && _bVisible && !this._isMultiSelected && IsStandardDocument;
        }

        private bool CanExecuteSearch(object cmdParameter)
        {
            return _bCanSearch && !this._isMultiSelected;
        }
        private void PageOutdentExecute(object cmdParameter)
        {
            INodeViewModel Node = NodeInfo.SelectedNode;

            INodeViewModel oldParent = Node.Parent;
            int oldIndex = Node.IndexInParent;

            Node.Outdent();

            PageTreeChanged();

            MovePageCommand cmd = new MovePageCommand(Node, Node.Parent, Node.IndexInParent, oldParent, oldIndex);
            _undoManager.Push(cmd);

            if (_pageTree != null)
                _pageTree.Focus();
        }

        private bool CanExecuteOutdent(object cmdParameter)
        {
            INodeViewModel node = GetOperationNode();
            return (!node.IsRootNode) && (!node.Parent.IsRootNode) && _bVisible && !this._isMultiSelected && IsStandardDocument;
        }

        private void EditNodeExecute(object cmdParameter)
        {
            INodeViewModel Node = NodeInfo.SelectedNode;
            Node.IsNodeEditable = true;
        }

        private void PageSearchExecute(object obj)
        {
            if (SearchBoxVisibility == Visibility.Visible)
            {
                SearchBoxVisibility = Visibility.Collapsed;
                SearchPageName = string.Empty;
                _bVisible = true;
            }
            else
            {
                SearchBoxVisibility = Visibility.Visible;
                _bVisible = false;
            }
            RefreshCommands();
        }
        private void InsertSiblingBeforeExecute(object cmdParameter)
        {
            INodeViewModel Node = GetOperationNode().InsertSiblingBefore(GetNextDataName(TreeNodeType.Page));
            Node.IsNodeEditable = true;
            Node.IsSelected = true;

            PageTreeChanged();

            CreatePageCommand cmd = new CreatePageCommand(this, Node, Node.Parent, Node.IndexInParent, _pageTree);
            _undoManager.Push(cmd);
        }
        private void InsertSiblingAfterExecute(object cmdParameter)
        {
            INodeViewModel Node = GetOperationNode().InsertSiblingAfter(GetNextDataName(TreeNodeType.Page));
            Node.IsNodeEditable = true;
            Node.IsSelected = true;

            PageTreeChanged();

            CreatePageCommand cmd = new CreatePageCommand(this, Node, Node.Parent, Node.IndexInParent, _pageTree);
            _undoManager.Push(cmd);
        }
        private void AddChildExecute(object cmdParameter)
        {
            INodeViewModel Node = GetOperationNode().AddChild(GetNextDataName(TreeNodeType.Page));
            Node.IsSelected = true;
            Node.IsNodeEditable = true;

            PageTreeChanged();

            CreatePageCommand cmd = new CreatePageCommand(this, Node, Node.Parent, Node.IndexInParent, _pageTree);
            _undoManager.Push(cmd);
        }
        /// <summary>
        /// Duplicate page/folder
        /// </summary>
        private void DuplicateNodeExecute(object cmdParameter)
        {
            try
            {
                var node = GetOperationNode().Duplicate(false);
                if(node != null)
                {
                    PageTreeChanged();

                    CreatePageCommand cmd = new CreatePageCommand(this, node, node.Parent, node.IndexInParent, _pageTree);
                    _undoManager.Push(cmd);
                }

            }
            catch(Exception e)
            {
                MessageBox.Show(GlobalData.FindResource("Warn_Copy_Mem_Info"), GlobalData.FindResource("Common_Error"));
            }
            
            
        }
        private void DuplicateBranchExecute(object cmdParameter)
        {
            var node = GetOperationNode().Duplicate();
            if(node!=null)
            {
                PageTreeChanged();

                CreatePageCommand cmd = new CreatePageCommand(this, node, node.Parent, node.IndexInParent, _pageTree);
                _undoManager.Push(cmd);
            }

        }
        private void LeavePageTreeExecute(object cmdParameter)
        {
            _ListEventAggregator.GetEvent<PagePreviewEvent>().Publish(Guid.Empty);
        }

        void SetOperationNode(NodeViewModel node)
        {
            NodeInfo.SelectedNode = node;
        }
        private void ApplyFilter()
        {
            foreach (var node in RootNode.Children)
            {
                node.ApplyFilter(SearchPageName, new Stack<INodeViewModel>());
            }
        }

        private void OpenPageExecute(object cmdParameter)
        {
            if (GetCtrlShiftStatus() == KeyStatus.None)
            {
                NodeViewModel node = cmdParameter as NodeViewModel;
                if (node != null && node.NodeType == TreeNodeType.Page && !node.IsNodeEditable)
                {
                    _ListEventAggregator.GetEvent<OpenNormalPageEvent>().Publish(node.Guid);
                }

                InitializeMultiSelected();
            }
        }

        private List<NodeViewModel> _multiCheckedNode = new List<NodeViewModel>();
        private void PageClickExecute(object cmdParameter)
        {
            var keystatus = GetCtrlShiftStatus();
            if (keystatus != KeyStatus.None
                && cmdParameter is object[]
                && (cmdParameter as object[]).Length == 2
                && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var paramobjects = cmdParameter as object[];
                var tview = paramobjects[0] as TreeView;
                var eventArgs = paramobjects[1] as RoutedPropertyChangedEventArgs<object>;
                if (tview != null && tview.SelectedItem is NodeViewModel)
                {
                    var oldSelected = eventArgs.OldValue as NodeViewModel;
                    if (oldSelected != null)
                    {
                        ///item changed first time
                        oldSelected.IsMultiSelected = true;
                        _multiCheckedNode.Clear();
                        _multiCheckedNode.Add(oldSelected);
                    }

                    var node = tview.SelectedItem as NodeViewModel;
                    node.IsSelected = false;
                    if (keystatus == KeyStatus.Ctrl)
                    {
                        if (!node.IsMultiSelected)
                        {
                            node.IsMultiSelected = true;
                            if (!_multiCheckedNode.Contains(node))
                            {
                                _multiCheckedNode.Add(node);
                            }
                        }
                        else
                        {
                            var multiSelectedCount = GetAllNodes(RootNode).Count(n => n.IsMultiSelected);
                            if (multiSelectedCount > 1)
                            {
                                node.IsMultiSelected = false;
                                if (_multiCheckedNode.Contains(node))
                                {
                                    _multiCheckedNode.Remove(node);
                                }
                            }
                        }
                    }
                    else if (keystatus == KeyStatus.Shift)
                    {
                        if (_multiCheckedNode.Count == 0)
                        {
                            node.IsMultiSelected = true;
                            _multiCheckedNode.Add(node);
                        }
                        else
                        {
                            var lastCheckedNode = _multiCheckedNode.Last();

                            if (node == lastCheckedNode)
                            {
                                InitializeMultiSelected();
                                lastCheckedNode.IsMultiSelected = true;
                                _multiCheckedNode.Add(lastCheckedNode);
                            }
                            else
                            {
                                if (node.Parent == lastCheckedNode.Parent)
                                {
                                    InitializeMultiSelected();
                                    var from = Math.Min(lastCheckedNode.IndexInParent, node.IndexInParent);
                                    var to = Math.Max(lastCheckedNode.IndexInParent, node.IndexInParent);
                                    for (var i = from; i <= to; i++)
                                    {
                                        node.Parent.Children[i].IsMultiSelected = true;
                                    }

                                    _multiCheckedNode.Add(lastCheckedNode);
                                }
                            }
                        }
                    }

                    this._isMultiSelected = true;
                    this.RefreshCommands();
                    NodeInfo.SelectedNode.IsSelected = false;
                    NodeInfo.SelectedNode = RootNode;
                }
            }
            else
            {
                ClearMultiSelected();
            }
        }

        private void DeleteSearchExecute(object cmdParameter)
        {
            SearchPageName = string.Empty;
        }

        /// <summary>
        /// Triggered when child node upldated. eg: IsExpanded
        /// </summary>
        /// <param name="cmdParameter"></param>
        private void PageTreeUpdatedExecute(object cmdParameter)
        {
            PageTreeChanged();
        }

        private KeyStatus GetCtrlShiftStatus()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                return KeyStatus.Ctrl;
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                return KeyStatus.Shift;
            }
            else
            {
                return KeyStatus.None;
            }
        }

        public IEnumerable<INodeViewModel> GetAllNodes(INodeViewModel root)
        {
            foreach (var child in root.Children)
            {
                yield return child;
                foreach (var subnode in GetAllNodes(child))
                {
                    yield return subnode;
                }
            }
        }

        public void InitializeMultiSelected()
        {
            var multiSelectedNodes = GetAllNodes(RootNode).Where(n => n.IsMultiSelected);
            foreach (var node in multiSelectedNodes)
            {
                node.IsMultiSelected = false;
            }

            _multiCheckedNode.Clear();
        }

        public void ClearMultiSelected()
        {
            InitializeMultiSelected();
            this._isMultiSelected = false;
            this.RefreshCommands();
        }

        private void DeselectNodeExecute(object cmdParameter)
        {
            if (NodeInfo.SelectedNode != null)
            {
                NodeInfo.SelectedNode.IsSelected = false;
                NodeInfo.SelectedNode = RootNode;
                ClearMultiSelected();
            }
        }

        INodeViewModel GetOperationNode()
        {
            if (NodeInfo.SelectedNode == null)
                return RootNode;
            return NodeInfo.SelectedNode;
        }

        void RefreshCommands()
        {
            PageNewCommand.RaiseCanExecuteChanged();
            FolderNewCommand.RaiseCanExecuteChanged();
            PageDeleteCommand.RaiseCanExecuteChanged();
            PageUpCommand.RaiseCanExecuteChanged();
            PageDownCommand.RaiseCanExecuteChanged();
            PageIndentCommand.RaiseCanExecuteChanged();
            PageOutdentCommand.RaiseCanExecuteChanged();
            PageSearchCommand.RaiseCanExecuteChanged();
        }

        private void AddPageRequestExecute(Guid cmdParameter)
        {
            if (cmdParameter == Guid.Empty)
            {
                INodeViewModel Node = GetOperationNode().Add(GetNextDataName(TreeNodeType.Page));
                Node.IsSelected = true;

                CreatePageCommand cmd = new CreatePageCommand(this, Node, Node.Parent, Node.IndexInParent, _pageTree);
                _undoManager.Push(cmd);

                this._ListEventAggregator.GetEvent<AddNewPageEvent>().Publish(Node.Guid);
            }
        }

        private void FocusSitemapExecute(object obj)
        {
            _pageTree.Focus();
        }

        private void PageTreeChanged()
        {
            if(_document!=null)
            {
                _document.IsDirty = true;
            }

            if(_ListEventAggregator!=null)
            {
                _ListEventAggregator.GetEvent<UpdatePageTreeEvent>().Publish(string.Empty);
            }
        }

        #endregion

        #region private member
        private string _pageName = "Page ";
        private string _folderName = "Folder ";
        private string _widgetName = "Object ";
        private int _pageCounter = 1;
        private int _folderCounter = 1;
        private bool _bCanAddPage = false;
        private bool _bCanAddFolder = false;
        private bool _bCanSearch = false;
        private bool _isMultiSelected = false;
        private TreeView _pageTree;
        private bool _bFirstPage = true;

        //Are buttons except search visible
        private bool _bVisible = true;

        IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }
        public bool IsStandardDocument
        {
            get
            {
                if (_document == null)
                    return true;
                return _document.DocumentType == DocumentType.Standard;
            }
        }
        private string GetNextDataName(TreeNodeType type)
        {
            if (IsStandardDocument)
            {
                return (type == TreeNodeType.Page) ?
                    string.Concat(_pageName, _pageCounter++) :
                    String.Concat(_folderName, _folderCounter++);
            }
            else
            {
                return string.Concat(_widgetName, _pageCounter++);
            }
        }
        private void InitParameter(bool bLoad)
        {
            _pageCounter = 1;
            _folderCounter = 1;
            _bFirstPage = true;

            if (bLoad)
            {
                _bCanAddPage = true;
                _bCanAddFolder = true;
                _bCanSearch = true;
            }
            else
            {
                _bCanAddPage = false;
                _bCanAddFolder = false;
                _bCanSearch = false;
            }
            RefreshCommands();
            FirePropertyChanged("IsStandardDocument");
        }

        /// <summary>
        /// Select page in pagelist treeview
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="pageGuid"> page guid which is select in editview</param>
        private void SelectPage(INodeViewModel parent, Guid pageGuid)
        {

            foreach (var item in parent.Children)
            {
                if (item.Guid == pageGuid)
                {
                    _pageTree.Focus();
                     
                    item.IsSelected = true;
                    return;
                }
                SelectPage(item, pageGuid);
            }
        }

        #endregion

        #region ICommandSink

        public bool CanExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            return _commandSink.CanExecuteCommand(command, parameter, out handled);
        }

        public void ExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            _commandSink.ExecuteCommand(command, parameter, out handled);
        }

        private readonly CommandSink _commandSink;

        #endregion


        #region Undo/Redo

        private void UndoCommandHandler(object parameter)
        {
            _undoManager.Undo();
        }

        private bool CanRunUndoCommand
        {
            get { return _undoManager.CanUndo; }
        }


        private void RedoCommandHandler(object parameter)
        {
            _undoManager.Redo();
        }

        private bool CanRunRedoCommand
        {
            get { return _undoManager.CanRedo; }
        }

        public UndoManager UndoManager
        {
            get { return _undoManager; }
            set { _undoManager = value; }
        }

        private UndoManager _undoManager = new UndoManager();

        #endregion

    }

    public enum KeyStatus
    {
        None,
        Shift,
        Ctrl
    }
}
