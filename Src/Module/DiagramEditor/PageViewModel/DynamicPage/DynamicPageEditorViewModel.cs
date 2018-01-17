using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Common.Helper;
using System.Windows.Controls;
using System.Windows.Documents;
using Naver.Compass.Service;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Markup;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Naver.Compass.Module
{
    public class DynamicPageEditorViewModel:WidgetPageEditorViewModel
    {
        //public DynamicPageEditorViewModel(PageEditorViewModel parentPageVM,IWidget widget)
        public DynamicPageEditorViewModel(IWidget widget)
            : base(widget)
        {
            _pageType = Common.CommonBase.PageType.DynamicPanelPage;
            InitializeDynamicPage(widget,Guid.Empty);
            InitializeCommonData();
        }

        public DynamicPageEditorViewModel(IWidget widget,Guid childPageID)
            : base(widget)
        {
            _pageType = Common.CommonBase.PageType.DynamicPanelPage;
            InitializeDynamicPage(widget, childPageID);
            InitializeCommonData();
        }     
  
        #region Private Function and  Property
        //private PageEditorViewModel _parentPageVM;
        private void InitializeDynamicPage(IWidget widget,Guid childPanelGID)
        {
            if (widget == null)
            {
                return;
            }
            _copyTime = 0;
            Title = @"Swipe Views";//+ widget.Name
            _pageGID = widget.Guid;
            ContentId = _pageGID.ToString();
            DyncWidget = widget as IDynamicPanel;
            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler);

            if (childPanelGID==Guid.Empty)
            {
                _acitiveCurrentChildPage = (widget as IDynamicPanel).StartPanelStatePage;
                _model = new PageEditorModel((widget as IDynamicPanel).StartPanelStatePage);

            }
            else
            {
                IPage item = DyncWidget.PanelStatePages.FirstOrDefault(x => x.Guid == childPanelGID);
                if (item.Guid == childPanelGID)
                {
                    _acitiveCurrentChildPage = item;
                    _model = new PageEditorModel(item);
                }

            }
            
            SetDefaultAdaptive();
            
            DynamicChildren = new ObservableCollection<DynamicChildNodViewModel>();
            Page2WidgetsTable = new Dictionary<Guid, List<WidgetViewModBase>>();
            
            DynamicChildren.CollectionChanged += DynamicChildren_CollectionChanged;
            ChangeActivedPageCommand = new DelegateCommand<object>(ChangeActivedPageExecute);
            EditChildrenNodesCommand = new DelegateCommand<object>(EditChildrenNodesExecute);
            Return2ParentPageCommand = new DelegateCommand<object>(Return2ParentPageExecute);
            CreateChildPageCommand = new DelegateCommand<object>(CreateChildPageExecute);
            RemoveChildPageCommand = new DelegateCommand<object>(RemoveChildPageExecute);

            LoadChildrenNode();

            //initialize the icon list status
            foreach (DynamicChildNodViewModel item in DynamicChildren)
            {
                if (item.GID == _acitiveCurrentChildPage.Guid)
                {
                    item.IsChecked = true;
                    break;
                }
            }
            //DynamicChildren[0].IsChecked = true;
        }
        private void DynamicChildren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int i = 1;
            bool canDelete = false;
            if (DynamicChildren.Count() > 3)
            {
                canDelete = true;
            }
            foreach (DynamicChildNodViewModel item in DynamicChildren)
            {
                item.ShowNumber = i++;
                item.CanDelete = canDelete;
            }
            ShowType = DyncWidget.NavigationType;
        }
        private void LoadChildrenNode()
        {
            DynamicChildren.Clear();
            bool canDelete = false;
            if (DyncWidget.PanelStatePages.Count() > 3)
            {
                canDelete = true;
            }
            foreach (IPage item in DyncWidget.PanelStatePages)
            {
                DynamicChildNodViewModel childVM = new DynamicChildNodViewModel(item);
                childVM.CanDelete = canDelete;
                DynamicChildren.Add(childVM);
            }
        }
        private void RefreshPageUI(Guid OldChildPageID,IPage NewChildPage)
        {

            if (Page2WidgetsTable.ContainsKey(OldChildPageID))
            {
                Page2WidgetsTable[OldChildPageID].Clear();
            }
            else
            {
                Page2WidgetsTable.Add(OldChildPageID, new List<WidgetViewModBase>());
            }

            foreach(WidgetViewModBase wdg in Items)
            {
                if (wdg.IsSelected == true)
                {
                    wdg.IsSelected = false;
                }
                Page2WidgetsTable[OldChildPageID].Add(wdg);
            }

            items.CollectionChanged -= items_CollectionChanged;
            Items.Clear();

            _model = new PageEditorModel(NewChildPage);
            _model.SetActivePageView(_curAdaptiveViewGID);
            AsyncLoadAllWidgets();
        }
        private Dictionary<Guid, List<WidgetViewModBase>> Page2WidgetsTable;
        #endregion

        #region Public Property
        public IDynamicPanel DyncWidget;
        override public IPage AcitiveCurrentChildPage
        {
            get
            {
                return _acitiveCurrentChildPage;
            }
            set
            {
                if (_acitiveCurrentChildPage != value)
                {
                    //_acitiveCurrentChildPage.Close();
                    Guid OldGID = _acitiveCurrentChildPage.Guid;
                    value.Open();
                    _acitiveCurrentChildPage = value;                    
                    RefreshPageUI(OldGID,_acitiveCurrentChildPage);
                }
            }
        }
        public NavigationType ShowType
        {
            get
            {
                return DyncWidget.NavigationType;
            }
            set
            {
                if (DyncWidget.NavigationType != value)
                {
                    DyncWidget.NavigationType = value;
                }                

                foreach (DynamicChildNodViewModel item in DynamicChildren)
                {
                    switch(value)
                    {
                        case NavigationType.None:
                            item.ShowType = 1;
                            break;
                        case NavigationType.Number:
                            item.ShowType = 2;
                            break;
                        default:
                            item.ShowType = 0;
                            break;
                    }
                }
            }
        }

        public Visibility NavigationVisibility
        {
            get
            {
                if(DyncWidget.ViewMode == DynamicPanelViewMode.Scroll)
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

        #region Pulic Binding Property
        public ObservableCollection<DynamicChildNodViewModel> DynamicChildren { get; set; }
        public DelegateCommand<object> ChangeActivedPageCommand { get; private set; }    
        public DelegateCommand<object> EditChildrenNodesCommand { get; private set; }
        public DelegateCommand<object> Return2ParentPageCommand { get; private set; }
        public DelegateCommand<object> CreateChildPageCommand { get; private set; }
        public DelegateCommand<object> RemoveChildPageCommand { get; private set; }
     
        public bool IsShowArrow
        {
            get
            {
                return DyncWidget.ShowAffordanceArrow;
            }
            set
            {
                if (DyncWidget.ShowAffordanceArrow != value)
                {
                    DyncWidget.ShowAffordanceArrow = value;
                    FirePropertyChanged("IsShowArrow");
                } 
            }
        }

        public override int FlickingWidth
        {
            get
            {
                IDynamicPanel panel = _widget as IDynamicPanel;
                double percent = (panel.ViewMode == DynamicPanelViewMode.Full) ? 1 : panel.PanelWidth * 0.01;

                return Convert.ToInt32(_widget.GetWidgetStyle(_model.ActivePageView.Guid).Width * percent * EditorScale);
            }
        }
        #endregion

        #region Override Property and Functions
        override public IPage ActivePage
        {
            get
            {
                return AcitiveCurrentChildPage;
            }
            set
            {
                if (AcitiveCurrentChildPage.Guid != value.Guid)
                {
                    //AcitiveCurrentChildPage = value;
                    foreach (DynamicChildNodViewModel item in DynamicChildren)
                    {
                        if (item.GID == value.Guid && item.IsChecked == false)
                        {
                            item.IsChecked = true;
                            break;
                        }
                    }
                }
            }
        }
        protected override void LoadWidgets()
        {
            if (_model.Widgets == null)
            {
                return;
            }

            if (Page2WidgetsTable.ContainsKey(_acitiveCurrentChildPage.Guid))
            {
                //Async Add widget from DOM to UI
                List<WidgetViewModBase> OldWidgets = Page2WidgetsTable[_acitiveCurrentChildPage.Guid];
                foreach (WidgetViewModBase widgetVM in OldWidgets)
                { 
                    widgetVM.ChangeCurrentPageView(_model.ActivePageView);
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
                    {                       
                        Items.Add(widgetVM);
                    }));
                }
            }
            else
            {
                base.LoadWidgets();
            }
        }
  
        override protected void OnPannelSelected(bool bIsSelected)
        {
            if (_model.IsPageOpen() == false)
            {
                return;
            }

            base.OnPannelSelected(bIsSelected);
            if (bIsSelected == true)
            {
                FirePropertyChanged("FlickingWidth");
                FirePropertyChanged("FlickingHeight");
                FirePropertyChanged("IsShowArrow");
                FirePropertyChanged("NavigationVisibility");
                ShowType = DyncWidget.NavigationType;
            }
            else
            {
                //Leave the page, and send message to parent flicking to refresh UI                
                //Guid DyncGID = DyncWidget.ParentPage.Guid;

                //if (_isThumbnailUpdate == true)
                //{
                //    Guid DyncGID = DyncWidget.Guid;
                //    _ListEventAggregator.GetEvent<RefreshWidgetChildPageEvent>().Publish(DyncGID);
                //    _isThumbnailUpdate = false;
                //}

            }
        }

        public override void Close(Guid PageGID)
        {
            //if (PageGID != Guid.Empty)
            //{
                
            //}

            if (DyncWidget.ParentPage.IsOpened)
            {
                Return2ParentPageExecute(null);
                _ListEventAggregator.GetEvent<CloseWidgetPageEvent>().Publish(DyncWidget);
                return;
            }   
            else
            {
                foreach (IPage page in DyncWidget.PanelStatePages)
                {
                    if (page.IsOpened)
                    {
                        page.Close();
                    }
                }
            }             
            
            
        }
        
        #endregion

        #region Page Command Handler
        private void ChangeActivedPageExecute(object obj)
        {
            DynamicChildNodViewModel NewPageVM = obj as DynamicChildNodViewModel;
            if(AcitiveCurrentChildPage.Guid==NewPageVM.GID)
            {
                return;
            }

            if (NewPageVM.IsChecked == true)
            {
                foreach (IPage item in DyncWidget.PanelStatePages)
                {
                    if (item.Guid == NewPageVM.GID)
                    {
                        AcitiveCurrentChildPage = item;
                        break;
                    }
                }
            }
        }
        private void EditChildrenNodesExecute(object obj)
        {
            DynamicPanelStatesChangeCommand cmd = new DynamicPanelStatesChangeCommand(this);

            FlickingStateManagerWindow device = new FlickingStateManagerWindow();
            FlickingStateManagerViewModel managerVM = new FlickingStateManagerViewModel(this);
            device.DataContext = managerVM;
            device.Owner = Application.Current.MainWindow;
            device.ShowDialog();

            if (managerVM.HasChange)
            {
                cmd.SaveCurrentStates();
                _undoManager.Push(cmd);
            }
        }
        private void Return2ParentPageExecute(object obj)
        {
            Guid parentGID = DyncWidget.ParentPage.Guid;
            _ListEventAggregator.GetEvent<OpenNormalPageEvent>().Publish(parentGID);
        }

        private void CreateChildPageExecute(object obj)
        {
            //Dom
            int size = DyncWidget.PanelStatePages.Count;
            string szNumber=DyncWidget.PanelStatePages[size - 1].Name.Substring(6);
            int nNumber = Convert.ToInt16(szNumber)+1;

            string pageName;
            if(nNumber<10)
            {
                pageName = "Panel 0" + nNumber;
            }
            else
            {
                pageName = "Panel " + nNumber;
            }
            IPage newPage = DyncWidget.CreatePanelStatePage(pageName);
            if (newPage == null)
            {
                return;
            }

            //UI
            DynamicChildNodViewModel childVM = new DynamicChildNodViewModel(newPage);
            DynamicChildren.Add(childVM);

            //Select
            SelectValue = childVM;
            //SelectValue.IsChecked = true;

            //Set Dirty
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            doc.Document.IsDirty = true;

            CreateDynamicPanelStateCommand cmd = new CreateDynamicPanelStateCommand(this, childVM);
            _undoManager.Push(cmd);
        }
        private void RemoveChildPageExecute(object obj)
        {
            DynamicChildNodViewModel node = obj as DynamicChildNodViewModel;

            // This undo command will save the index of node, so have to create it before 
            // removing it from collection.
            DeleteDynamicPanelStateCommand cmd = new DeleteDynamicPanelStateCommand(this, node);

            //UI
            int index = DynamicChildren.IndexOf(node);
            DynamicChildren.Remove(node);

            //DOM
            //DyncWidget.PanelStatePages.Remove(node.Page as IPanelStatePage);
            DyncWidget.DeletePanelStatePage(node.Page.Guid);
            if (DyncWidget.StartPanelStatePage == node.Page)
            {
                DyncWidget.StartPanelStatePage = DyncWidget.PanelStatePages[0];
            }

            //Select
            if (index > 0)
            {
                SelectValue = DynamicChildren.ElementAt(--index);
            }
            else
            {
                SelectValue = DynamicChildren.ElementAt(0);
            }
            if (node.IsChecked == true)
            {
                SelectValue.IsChecked = true;
            }

            //Set Dirty
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            doc.Document.IsDirty = true;

            _undoManager.Push(cmd);

        }
        #endregion  

        #region Ready to Remove..
        private DynamicChildNodViewModel selectValue;
        public DynamicChildNodViewModel SelectValue
        {
            get
            {
                return selectValue;
            }
            set
            {
                if (selectValue != value)
                {
                    //unselect old item
                    if (selectValue != null)
                        selectValue.IsEditboxFocus = false;
                    selectValue = value;
                    //select new item
                    if (selectValue != null)
                        selectValue.IsEditboxFocus = true;
                    FirePropertyChanged("SelectValue");
                }
            }
        }
        #endregion     

    }
}
