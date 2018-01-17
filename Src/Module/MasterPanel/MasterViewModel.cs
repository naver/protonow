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
    public class MasterListViewModel : ViewModelBase, ICommandSink, ISupportUndo
    {
        public MasterListViewModel(TreeView tree)
        {
            _pageTree = tree;
            _commandSink = new CommandSink();
            _commandSink.RegisterCommand(ApplicationCommands.Undo, param => CanRunUndoCommand, UndoCommandHandler);
            _commandSink.RegisterCommand(ApplicationCommands.Redo, param => CanRunRedoCommand, RedoCommandHandler);

            if (!IsInDesignMode)
            {
                this.MasterNewCommand = new DelegateCommand<object>(AddMasterExecute, CanExecuteAddMaster);

                this.MasterDeleteCommand = new DelegateCommand<object>(DeleteMasterExecute, CanExecuteDelete);
                this.MasterSearchCommand = new DelegateCommand<object>(MasterSearchExecute, CanExecuteSearch);
                this.OpenMasterCommand = new DelegateCommand<object>(OpenMasterExecute);
                this.EditNodeCommand = new DelegateCommand<object>(EditNodeExecute);
                this.DeselectNodeCommand = new DelegateCommand<object>(DeselectNodeExecute);
                this.AddMasterCommand = new DelegateCommand<object>(AddMasterExecute);
                this.DuplicateCommand = new DelegateCommand<object>(DuplicateNodeExecute);
                this.DeleteSearchCommand = new DelegateCommand<object>(DeleteSearchExecute);
                this.Add2PagesCommand = new DelegateCommand<object>(Add2PagesExecute);
                this.DeleteFromAllPagesCommand = new DelegateCommand<object>(DeleteFromAllPagesExecute);

                NodeInfo = new NodeInfo();
                NodeInfo.SelectedNodeChanged += (s, e) => RefreshCommands();
                RootNode = new NodeViewModel(_undoManager, null, NodeInfo, true);

                _ListEventAggregator.GetEvent<DomLoadedEvent>().Subscribe(DomLoadedEventHandler, ThreadOption.UIThread);
                _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler, ThreadOption.UIThread);
                _ListEventAggregator.GetEvent<AddConvertedMasterEvent>().Subscribe(AddConvertedMasterHandler);
                //_ListEventAggregator.GetEvent<FocusSitemapEvent>().Subscribe(FocusSitemapExecute, ThreadOption.UIThread);
            }
        }

        #region Property and command
        public NodeViewModel RootNode { get; private set; }
        public NodeInfo NodeInfo { get; private set; }

        public DelegateCommand<object> MasterNewCommand { get; private set; }
        public DelegateCommand<object> FolderNewCommand { get; private set; }
        public DelegateCommand<object> MasterDeleteCommand { get; private set; }
        public DelegateCommand<object> MasterSearchCommand { get; private set; }
        public DelegateCommand<object> OpenMasterCommand { get; private set; }
        public DelegateCommand<object> DeselectNodeCommand { get; private set; }
        public DelegateCommand<object> EditNodeCommand { get; private set; }
        public DelegateCommand<object> InsertSiblingBeforeCommand { get; private set; }
        public DelegateCommand<object> AddMasterCommand { get; private set; }
        public DelegateCommand<object> DuplicateCommand { get; private set; }
        public DelegateCommand<object> DeleteSearchCommand { get; private set; }
        public DelegateCommand<object> Add2PagesCommand { get; private set; }
        public DelegateCommand<object> DeleteFromAllPagesCommand { get; private set; }

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

        public bool IsStandardDocument
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc.Document == null)
                    return true;
                return doc.Document.DocumentType == DocumentType.Standard;
            }
        }
        #endregion

        #region Event handler and load Files function
        public void DomLoadedEventHandler(FileOperationType loadType)
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();

            //close all page opened.
            _ListEventAggregator.GetEvent<ClosePageEvent>().Publish(Guid.Empty);


            //if loadType is Loaded, as follows
            SetOperationNode(RootNode);
            RootNode.Children.Clear();
            switch (loadType)
            {
                case FileOperationType.Create:
                    if (doc.Document != null)
                    {
                        RootNode.TreeNodeObject = doc.Document.DocumentSettings.LayoutSetting.MasterPageTree;
                        _undoManager.Clear();
                        InitParameter(true);
                        CreateDefaultMasters();
                    }
                    break;
                case FileOperationType.Open:
                    if (doc.Document != null)
                    {
                        RootNode.TreeNodeObject = doc.Document.DocumentSettings.LayoutSetting.MasterPageTree;
                        LoadNodeViewModelFromTreeNodeObject(RootNode, RootNode.TreeNodeObject, true);
                        InitParameter(true);

                        _undoManager.Clear();
                    }
                    break;
                case FileOperationType.Close:
                    InitParameter(false);

                    _undoManager.Clear();
                    break;
            }

        }

        void LoadNodeViewModelFromTreeNodeObject(NodeViewModel nodeVM, ITreeNode treeNodeObject, bool beHome)
        {
            if (nodeVM == null || treeNodeObject == null)
            {
                return;
            }
            foreach (ITreeNode treeNode in treeNodeObject.ChildNodes)
            {
                NodeViewModel node = new NodeViewModel(_undoManager, treeNode, NodeInfo);
                nodeVM.Children.Add(node);
                node.Parent = nodeVM;

                LoadNodeViewModelFromTreeNodeObject(node, treeNode, beHome);
            }
        }

        private void SelectionPageChangeHandler(Guid pageGuid)
        {
            //select page in treeview
            SelectPage(RootNode, pageGuid);
        }

        private void AddConvertedMasterHandler(object parameter)
        {
            IMasterPage page = parameter as IMasterPage;
            if (page != null)
            {
                AddMaster(page.Name, page, false);
            }
        }

        #endregion

        #region operate page list functions

        private void AddMasterExecute(object cmdParameter)
        {
            AddMaster(GetNextDataName());
        }

        private void OpenMasterExecute(object cmdParameter)
        {
            if (GetCtrlShiftStatus() == KeyStatus.None)
            {
                NodeViewModel node = cmdParameter as NodeViewModel;
                if (node != null && node.NodeType == TreeNodeType.MasterPage && !node.IsNodeEditable)
                {
                    _ListEventAggregator.GetEvent<OpenMasterPageEvent>().Publish(node.Guid);
                }
            }
        }

        private void DeleteMasterExecute(object cmdParameter)
        {
            if ((!GetOperationNode().IsRootNode) && _bVisible)
            {
                NodeViewModel Node = NodeInfo.SelectedNode;

                bool containedInPages = false;
                IMasterPage masterPage = Node.TreeNodeObject.AttachedObject as IMasterPage;
                if (masterPage != null && masterPage.AllConsumerPageGuidList.Count > 0)
                {
                    MessageBoxResult ret = MessageBox.Show(GlobalData.FindResource("MasterDelete_AlertContent"),
                        GlobalData.FindResource("MasterDelete_AlertTitle"), MessageBoxButton.OKCancel);
                    if (ret.Equals(MessageBoxResult.Cancel))
                        return;
                    containedInPages = true;
                }

                NodeViewModel parent = Node.Parent;
                //int index = Node.IndexInParent;

                Node.Remove();

                if (containedInPages)
                {
                    IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();

                    #region re-order the Z in page where master is deleted
                    foreach (var guid in masterPage.AllConsumerPageGuidList)
                    {
                        var page = doc.Document.Pages.GetPage(guid);
                        if (page == null)
                            continue;

                        bool isOpened = page.IsOpened;
                        if (!isOpened)
                        {
                            page.Open();
                        }

                        ReOrderZ(page);

                        if (!isOpened)
                        {
                            page.Close();
                        }
                    }
                    #endregion
                    _ListEventAggregator.GetEvent<DeleteMasterPageEvent>().Publish(masterPage);
                }

                //if (index >= 0)
                //{
                //    DeletePageCommand cmd = new DeletePageCommand(this, Node, parent, index, _pageTree);
                //    _undoManager.Push(cmd);
                //}
            }
            if (_pageTree != null)
                _pageTree.Focus();
        }

        private void EditNodeExecute(object cmdParameter)
        {
            NodeViewModel Node = NodeInfo.SelectedNode;
            Node.IsNodeEditable = true;
        }

        private void MasterSearchExecute(object obj)
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

        /// <summary>
        /// Duplicate page/folder
        /// </summary>
        private void DuplicateNodeExecute(object cmdParameter)
        {
            try
            {
                NodeViewModel Node = GetOperationNode().Duplicate(false);
                CreatePageCommand cmd = new CreatePageCommand(this, Node, Node.Parent, Node.IndexInParent, _pageTree);
                _undoManager.Push(cmd);
            }
            catch(Exception e)
            {
                MessageBox.Show(GlobalData.FindResource("Warn_Copy_Mem_Info"), GlobalData.FindResource("Common_Error"));
            }          
            
        }

        private void AddMaster(string name, IMasterPage page = null, bool isEditable = true)
        {
            NodeViewModel Node = GetOperationNode().Add(name, page);
            if (Node != null)
            {
                Node.IsSelected = true;
                Node.IsNodeEditable = isEditable;

                //CreatePageCommand cmd = new CreatePageCommand(this, Node, Node.Parent, Node.IndexInParent, _pageTree);
                //_undoManager.Push(cmd);
            }
        }


        private void Add2PagesExecute(object cmdParameter)
        {
           
            NodeViewModel node = cmdParameter as NodeViewModel;
            if (node != null)
            {
                node.IsSelected = true;

            }
            else
            {
                node = NodeInfo.SelectedNode;
                if (node == null)
                    return;
            }

            AddMasterWindow win = new AddMasterWindow(node.TreeNodeObject.AttachedObject as IMasterPage);
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
            //PageListModel.GetInstance()
        }

        private void DeleteFromAllPagesExecute(object cmdParameter)
        {
            if ((!GetOperationNode().IsRootNode) && _bVisible)
            {
                NodeViewModel Node = NodeInfo.SelectedNode;

                IMasterPage masterPage = Node.TreeNodeObject.AttachedObject as IMasterPage;
                if (masterPage != null && masterPage.ActiveConsumerPageGuidList.Count > 0)
                {
                    IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();

                    #region re-order the Z in page where master is deleted
                    foreach (var guid in masterPage.AllConsumerPageGuidList)
                    {
                        var page = doc.Document.Pages.GetPage(guid);
                        if (page == null)
                            continue;
                        bool isOpened = page.IsOpened;
                        if(!isOpened)
                        {
                            page.Open();
                        }
                        var masterList = page.Masters.Where(a => a.MasterPageGuid == masterPage.Guid).ToList();
                        foreach(var master in masterList)
                        {
                            page.DeleteMaster(master.Guid);
                        }

                        ReOrderZ(page);

                        if(!isOpened)
                        {
                            page.Close();
                        }
                    }
                    #endregion
                    _ListEventAggregator.GetEvent<DeleteMasterPageEvent>().Publish(masterPage);
                }

            }

        }

        private void DeleteSearchExecute(object cmdParameter)
        {
            SearchPageName = string.Empty;
        }

        void SetOperationNode(NodeViewModel node)
        {
            NodeInfo.SelectedNode = node;
        }
        private void DeselectNodeExecute(object cmdParameter)
        {
            if (NodeInfo.SelectedNode != null)
            {
                NodeInfo.SelectedNode.IsSelected = false;
                NodeInfo.SelectedNode = RootNode;
            }
        }

        private void FocusSitemapExecute(object obj)
        {
            _pageTree.Focus();
        }

        private bool CanExecuteAddMaster(object cmdParameter)
        {
            return _bCanAddPage && _bVisible;
        }
        private bool CanExecuteDelete(object cmdParameter)
        {
            return ((!GetOperationNode().IsRootNode) && _bVisible);
        }

        private bool CanExecuteSearch(object cmdParameter)
        {
            return _bCanSearch;
        }

        #endregion

        #region private member
        private static string _masterName = "Master ";
        private static int _masterCounter = 1;
        private bool _bCanAddPage = false;
        private bool _bCanSearch = false;
        private TreeView _pageTree;

        //Are buttons except search visible
        private bool _bVisible = true;

        public static string GetNextDataName()
        {
            return String.Concat(_masterName, _masterCounter++);
        }
        public static void RollBackDataName()
        {
            _masterCounter--;
        }
        NodeViewModel GetOperationNode()
        {
            if (NodeInfo.SelectedNode == null)
                return RootNode;
            return NodeInfo.SelectedNode;
        }

        void RefreshCommands()
        {
            MasterNewCommand.RaiseCanExecuteChanged();
            DuplicateCommand.RaiseCanExecuteChanged();
            MasterDeleteCommand.RaiseCanExecuteChanged();
            MasterSearchCommand.RaiseCanExecuteChanged();
        }

        private void ReOrderZ(IPage page)
        {
            var objects = page.WidgetsAndMasters.OrderBy(a => a.RegionStyle.Z).ToList();
            for (int index = 0; index < objects.Count(); index++)
            {
                if (objects[index] is IWidget)
                {
                    (objects[index] as IWidget).WidgetStyle.Z = index;
                    continue;
                }
                if (objects[index] is IMaster)
                {
                    (objects[index] as IMaster).MasterStyle.Z = index;
                }
            }
        }
        private void ApplyFilter()
        {
            foreach (var node in RootNode.Children)
            {
                node.ApplyFilter(SearchPageName, new Stack<NodeViewModel>());
            }
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


        private void InitParameter(bool bLoad)
        {
            _masterCounter = 1;
            if (bLoad)
            {
                _bCanAddPage = true;
                _bCanSearch = true;
            }
            else
            {
                _bCanAddPage = false;
                _bCanSearch = false;
            }
            RefreshCommands();
        }

        /// <summary>
        /// Select page in pagelist treeview
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="pageGuid"> page guid which is select in editview</param>
        private void SelectPage(NodeViewModel parent, Guid pageGuid)
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

        void CreateDefaultMasters()
        {
            RootNode.Add(GetNextDataName());
            RootNode.Add(GetNextDataName());
            RootNode.Add(GetNextDataName());

            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            doc.Document.IsDirty = false;
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

