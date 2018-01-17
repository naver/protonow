using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using System.Windows.Threading;

namespace Naver.Compass.Module
{
    partial class WidgetManagerViewMode : ViewModelBase
    {
        private ISelectionService _selectionService;

        public WidgetManagerViewMode(WidgetManagerView view)
        {

            _control = view;

            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(PageChangeEventHandler);

            _ListEventAggregator.GetEvent<SelectionPropertyChangeEvent>().Subscribe(WidgetPropertyChangeHandler);

            _ListEventAggregator.GetEvent<SelectionChangeEvent>().Subscribe(WidgetSelectionChangeHandler);

            _ListEventAggregator.GetEvent<WidgetsNumberChangedEvent>().Subscribe(WidgetsNumberChangeHandler);

            _ListEventAggregator.GetEvent<GroupChangedEvent>().Subscribe(WidgetGroupChangeHandler);

            _ListEventAggregator.GetEvent<UpdateAdaptiveView>().Subscribe(OnAdaptiveViewChange);

            _ListEventAggregator.GetEvent<SelectionChangedByItemNotify>().Subscribe(ListItemSelectedChangedNotify);

            _ListEventAggregator.GetEvent<ZorderChangedEvent>().Subscribe(OnWidgetZorderChanged);

            _ListEventAggregator.GetEvent<RenamePageEvent>().Subscribe(RenamePageEventHandler, ThreadOption.UIThread);

            _ListEventAggregator.GetEvent<DomLoadedEvent>().Subscribe(DomLoadedEventHandler, ThreadOption.UIThread);

            _ListEventAggregator.GetEvent<UpdateLanguageEvent>().Subscribe(UpdateLanguageEventHandler);

            _ListEventAggregator.GetEvent<SwipePanelHidddenEvent>().Subscribe(OnSwipeViewHidePropertyChanged);

            this.ToggleExpandCollapseCommand = new DelegateCommand<object>(OnExpandCollapseCommand);

            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();

            _rootItem = new RootListItem();
            _rootItem.OrderedChildren = new List<WidgetListItem>();
            _expandStatusList = new Dictionary<Guid, bool>();

            _oldSelectedList = new List<Guid>();

            _allWidgets = new List<WidgetListItem>();

            InitSearchTimer();

            InitFilterList();

            InitCommand();

        }

        #region  Control Data

        public DelegateCommand<object> ToggleExpandCollapseCommand { get; private set; }

        private List<Guid> _oldSelectedList;

        private Dictionary<Guid, bool> _expandStatusList { get; set; }

        private WidgetListItem _rootItem;

        private IPage _Page;

        private WidgetManagerView _control;

        private DispatcherTimer _inputDelayTimer;

        #region Binding Data

        public string RootName
        {
            get
            {
                return _rootItem.WidgetName;
            }

            set
            {
                if (value != null && RootName.CompareTo(value) != 0)
                {
                    _rootItem.WidgetName = Convert.ToString(value);
                    FirePropertyChanged("RootName");
                }
            }
        }

        public ListItemType RootType
        {
            get
            {
                return _rootItem.ItemType;
            }

            set
            {
                if (RootType != (ListItemType)value)
                {
                    _rootItem.ItemType = (ListItemType)value;
                    FirePropertyChanged("RootType");
                }
            }
        }

        public List<WidgetListItem> _allWidgets;

        public List<WidgetListItem> _UIWidgetLIst;
        public List<WidgetListItem> UIWidgetItems
        {
            get
            {
                return _UIWidgetLIst;
            }
            set
            {
                _UIWidgetLIst = (List<WidgetListItem>)value;
                FirePropertyChanged("UIWidgetItems");
            }
        }

        bool _hideSearchBox = true;
        public bool HideSearchBox
        {
            get
            {
                return _hideSearchBox;
            }
            set
            {
                if (_hideSearchBox != (bool)value)
                {
                    _hideSearchBox = (bool)value;
                    FirePropertyChanged("HideSearchBox");
                }
            }
        }

        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                if (_searchString != value)
                {
                    _searchString = value;
                    OnSearchListItem();
                    FirePropertyChanged("SearchString");
                    FirePropertyChanged("DeleteSearchVisibility");
                }
            }
        }

        public System.Windows.Visibility DeleteSearchVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(SearchString))
                {
                    return System.Windows.Visibility.Collapsed;
                }
                else
                {
                    return System.Windows.Visibility.Visible;
                }
            }
        }
        #endregion

        #endregion

        private void ClearData(string rootName)
        {
            _Page = null;

            SearchString = String.Empty;
            HideSearchBox = true;

            if (_oldSelectedList != null)
            {
                _oldSelectedList.Clear();
            }

            if (_expandStatusList != null)
            {
                _expandStatusList.Clear();
            }

            _rootItem = new RootListItem();
            _rootItem.OrderedChildren = new List<WidgetListItem>();

            RootName = rootName;

            if (_UIWidgetLIst != null)
            {
                _UIWidgetLIst.Clear();
                FirePropertyChanged("UIWidgetItems");
            }

            if (this._inputDelayTimer.IsEnabled)
            {
                this._inputDelayTimer.Stop();
            }
        }

        #region Event handler

        private void OnExpandCollapseCommand(object obj)
        {
            Guid id = (Guid)obj;

            WidgetListItem Item = FindItemByGUID(id);

            if (Item.IsExpanded == false)
            {
                _expandStatusList[Item.WidgetID] = Item.IsExpanded;
            }
            else
            {
                if (_expandStatusList.ContainsKey(Item.WidgetID))
                {
                    _expandStatusList.Remove(Item.WidgetID);
                }
            }

            ExpandCollapseWidget(Item, Item.IsExpanded);

            RefreshToolbarCommands();
        }

        private void ExpandCollapseWidget(WidgetListItem item, bool expandFlag)
        {
            if (item != null)
            {
                foreach (WidgetListItem child in item.OrderedChildren)
                {
                    child.CollapseFlag = !expandFlag;

                    if (child.OrderedChildren != null && child.OrderedChildren.Count > 0)
                    {
                        ExpandCollapseWidget(child, expandFlag);
                    }
                }
            }
        }

        public void WidgetPropertyChangeHandler(string EventArgs)
        {
            try
            {
                if (EventArgs.CompareTo("DisplayName") == 0)
                {
                    OnWidgetDisplayNameChanged();

                }
                else if (EventArgs.CompareTo("IsHidden") == 0)
                {
                    OnWidgetHidepropertyChanged();
                }
                else if (EventArgs.CompareTo("IsShowInPageView2Adaptive") == 0)
                {
                    OnWidgetPlacePropertyChanged();
                }
                else if (EventArgs.CompareTo("IsGroupShowInView2Adaptive") == 0)
                {
                    OnWidgetPlacePropertyChanged();
                }
            }
            catch (System.Exception ex)
            {
                NLogger.Error("WidgetPropertyChangeHandler" + ex.Message);
            }
        }

        public void WidgetSelectionChangeHandler(string EventArg)
        {
            try
            {
                List<Guid> SelectList = _selectionService.GetSelectedWidgetGUIDs();

                foreach (Guid id in SelectList)
                {
                    _oldSelectedList.Remove(id);

                    WidgetListItem Item = FindUIItemByGUID(id);

                    if (Item != null)
                    {
                        Item.UpdateItemSelectInfo(true);
                    }
                }

                if (_oldSelectedList != null)
                {
                    foreach (Guid id in _oldSelectedList)
                    {
                        WidgetListItem Item = FindUIItemByGUID(id);
                        if (Item != null && !Item.PlaceFlag)
                        {
                            Item.UpdateItemSelectInfo(false);
                        }
                    }
                }

                _oldSelectedList.Clear();
                //_oldSelectedList = SelectList;

                foreach (WidgetListItem item in _control.ObjectList.Items)
                {
                    if (item.IsSelected)
                    {
                        _oldSelectedList.Add(item.WidgetID);
                    }
                }


                if (SelectList.Count > 0)
                {
                    WidgetListItem Item22 = FindUIItemByGUID(SelectList.First());
                    if (Item22 != null)
                    {
                        // ScrollToCenterOfView(_control.Tree, Item22);

                        _control.ObjectList.ScrollIntoView(Item22);
                    }
                }

                RefreshToolbarCommands();

            }
            catch (System.Exception ex)
            {
                NLogger.Error("WidgetSelectionChange" + ex.Message);
            }

        }

        public void OnAdaptiveViewChange(Guid ID)
        {
            //  IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            IPagePropertyData pageVM = _selectionService.GetCurrentPage();

            if (pageVM != null && pageVM.ActivePage != null)
            {
                WidgetListItem.CurViewID = ID;

                foreach (WidgetListItem Item in _allWidgets)
                {

                    Item.UpdateAllFlagByViewChange();
                }
            }
        }

        public void WidgetsNumberChangeHandler(Guid id)
        {
            try
            {
                IPagePropertyData pageVM = _selectionService.GetCurrentPage();

                FlashPageData(pageVM);

            }
            catch (System.Exception ex)
            {
                NLogger.Error("WidgetsNumberChange" + ex.Message);
            }
        }

        public void WidgetGroupChangeHandler(bool bGroup)
        {
            try
            {
                IPagePropertyData pageVM = _selectionService.GetCurrentPage();

                FlashPageData(pageVM);

                WidgetSelectionChangeHandler(null);
            }
            catch (System.Exception ex)
            {
                NLogger.Error("WidgetGroupChangeHandler" + ex.Message);
            }
        }

        public void OnSwipeViewHidePropertyChanged(object parameter)
        {
            WDMgrHideStatusChangeInfo info = parameter as WDMgrHideStatusChangeInfo;
            IPagePropertyData pageVM = _selectionService.GetCurrentPage();

            if (pageVM != null && info != null)
            {
                List<IWidgetPropertyData> VMList = pageVM.GetAllWidgets();
                List<Guid> ChildrenWidgets = new List<Guid>();
                foreach (IWidgetPropertyData data in VMList)
                {
                    ChildrenWidgets.Add(data.WidgetID);
                }

                ProcessHideChange(ChildrenWidgets, pageVM);
            }
        }

        public void OnWidgetHidepropertyChanged()
        {
            //List<Guid> SelectList = _selectionService.GetSelectedWidgetGUIDs();

            IPagePropertyData pageVM = _selectionService.GetCurrentPage();

            if (pageVM == null)
            {
                return;
            }

            List<Guid> SelectList = GetSelectListinPage();

            ProcessHideChange(SelectList, pageVM);

            return;
        }

        public void ProcessHideChange(List<Guid> ChildList, IPagePropertyData pageVM)
        {
            foreach (Guid id in ChildList)
            {
                WidgetListItem Item = FindItemByGUID(id);

                if (Item != null && Item.ItemType != ListItemType.DynamicPanelStateItem && Item.ItemType != ListItemType.GroupItem)
                {
                    Item.UpdateHideFlag();

                    //if (_rootItem.ItemType == ListItemType.DynamicPanelItem)
                    //{

                    if (Item.ParentID != null && !Item.ParentID.Equals(Guid.Empty))
                    {
                        WidgetListItem parentItem = FindItemByGUID(Item.ParentID);
                        if (parentItem != null)
                        {
                            if (_rootItem.ItemType == ListItemType.DynamicPanelItem)
                            {
                                if (parentItem.ItemType == ListItemType.GroupItem)
                                {
                                    TreatGroupFlags(parentItem as GroupListItem);

                                    WidgetListItem PanelItem = FindItemByGUID(parentItem.ParentID);

                                    TreatSwipePanelFlags(PanelItem);
                                }
                                else if (parentItem.ItemType == ListItemType.DynamicPanelStateItem)
                                {
                                    TreatSwipePanelFlags(parentItem);
                                }
                                else if (parentItem.ItemType == ListItemType.GroupChildItem)
                                {
                                    WidgetListItem RootGroupItem = FindRootGroupItem(parentItem);
                                    TreatGroupFlags(RootGroupItem as GroupListItem);

                                    WidgetListItem PanelItem = FindItemByGUID(RootGroupItem.ParentID);
                                    TreatSwipePanelFlags(PanelItem);
                                }
                            }
                            else if (parentItem.ItemType == ListItemType.GroupItem)
                            {
                                TreatGroupFlags(parentItem as GroupListItem);
                            }
                            else if (parentItem.ItemType == ListItemType.GroupChildItem)
                            {
                                TreatGroupFlags(FindRootGroupItem(parentItem) as GroupListItem);
                            }

                        }
                    }
                    //}
                }
            }
        }

        public void OnWidgetDisplayNameChanged()
        {
            List<Guid> SelectList = _selectionService.GetSelectedWidgetGUIDs();

            foreach (Guid id in SelectList)
            {
                WidgetListItem Item = FindUIItemByGUID(id);
                if (Item != null)
                {
                    Item.UpdateWidgetName();
                }
            }
        }

        public void OnWidgetPlacePropertyChanged()
        {

            List<Guid> SelectList = GetSelectListinPage();

            IPagePropertyData pageVM = _selectionService.GetCurrentPage();

            if (pageVM != null && pageVM.ActivePage != null)
            {
                foreach (Guid id in SelectList)
                {
                    WidgetListItem Item = FindItemByGUID(id);
                    if (Item != null)
                    {
                        Item.UpdateAllFlagByViewChange();
                    }
                }
            }
        }

        public void ListItemSelectedChangedNotify(object data)
        {
            WidgetSelectionInfo info = (WidgetSelectionInfo)data;
            WidgetListItem curitem = null;

            curitem = FindUIItemByGUID(info.WidgetID);
            if (curitem != null)
            {
                if (curitem.ItemType == ListItemType.GroupChildItem)
                {
                    curitem.UpdateItemSelectInfo(false);
                }
                else
                {
                    if (info.bSelected)
                    {
                        foreach (Guid ItemID in _oldSelectedList)
                        {
                            WidgetListItem tempitem = FindItemByGUID(ItemID);

                            if ((tempitem != null && tempitem.IsSelected == true) && curitem.ParentID != tempitem.ParentID)
                            {
                                curitem.UpdateItemSelectInfo(false);
                                return;
                            }
                        }

                        if (curitem.PlaceFlag == true)
                        {
                            _oldSelectedList.Add(info.WidgetID);
                            RefreshToolbarCommands();
                            return;
                        }
                    }
                    else
                    {
                        if (curitem.PlaceFlag == true)
                        {
                            _oldSelectedList.Remove(info.WidgetID);
                            return;
                        }
                    }


                    WidgetSelectionInfoExtra Notifydata = new WidgetSelectionInfoExtra();
                    Notifydata.WidgetID = curitem.WidgetID;
                    Notifydata.bSelected = info.bSelected;
                    Notifydata.PageID = curitem.ParentPage.Guid;
                    Notifydata.BelongWidget = WidgetListItem.BelongWidget;
                    Notifydata.pageType = GetparentType(curitem.ParentPage);
                    Notifydata.IsGroup = (curitem.ItemType == ListItemType.GroupItem);
                    Notifydata.IsSwipePanel = (curitem.ItemType == ListItemType.DynamicPanelStateItem);
                    _ListEventAggregator.GetEvent<WdgMgrChangeSelectionEvent>().Publish(Notifydata);
                }

            }

        }

        public void RenamePageEventHandler(Guid PageID)
        {
            if (_Page != null && _Page.Guid == PageID)
            {
                RootName = _Page.Name;
            }
        }

        public void UpdateLanguageEventHandler(object obj)
        {
            if (_UIWidgetLIst != null)
            {
                foreach (WidgetListItem item in _UIWidgetLIst)
                {
                    item.UpdateWidgetTypeName();

                    if (item.PlaceFlag || item.LostFlag)
                    {
                        item.UpdateTooltip();
                    }
                }
                
            }

        }

        public void DomLoadedEventHandler(FileOperationType type)
        {
            if (type == FileOperationType.Close)
            {
                ClearData("Docment closed");
            }
        }

        private PageType GetparentType(IPage page)
        {
            if (page is IToastPage)
            {
                return PageType.ToastPage;
            }
            else if (page is IHamburgerMenuPage)
            {
                return PageType.HamburgerPage;
            }
            else if (page is IPanelStatePage)
            {
                return PageType.DynamicPanelPage;
            }
            else
            {
                return PageType.NormalPage;
            }
        }

        public WidgetListItem FindItemByGUID(Guid ID)
        {
            foreach (WidgetListItem item in _allWidgets)
            {
                if (item.WidgetID.Equals(ID))
                {
                    return item;
                }
            }

            return null;
        }

        public WidgetListItem FindUIItemByGUID(Guid ID)
        {
            foreach (WidgetListItem item in UIWidgetItems)
            {
                if (item.WidgetID.Equals(ID))
                {
                    return item;
                }
            }

            return null;
        }

        public WidgetListItem FindRootGroupItem(WidgetListItem GroupChildItem)
        {
            WidgetListItem parentItem = FindItemByGUID(GroupChildItem.ParentID);

            while (parentItem.ParentID != null)
            {
                if (parentItem.ItemType == ListItemType.GroupItem)
                {
                    break;
                }

                parentItem = FindItemByGUID(parentItem.ParentID);


            }

            return parentItem;
        }

        private List<Guid> GetSelectItemList()
        {
            return _oldSelectedList;
        }

        private List<Guid> GetSelectListinPage()
        {
            //List<Guid> SelectList = _selectionService.GetSelectedWidgetGUIDs();

            List<Guid> list = new List<Guid>();

            List<IWidgetPropertyData> SelectWidgeList = _selectionService.GetSelectedWidgets();

            foreach (IWidgetPropertyData widgetdate in SelectWidgeList)
            {
                if (widgetdate.IsGroup == true)
                {
                    Naver.Compass.WidgetLibrary.GroupViewModel groupVM = widgetdate as Naver.Compass.WidgetLibrary.GroupViewModel;
                    foreach (IWidgetPropertyData data in groupVM.WidgetChildren)
                    {
                        list.Add(data.WidgetID);
                    }
                }
                else
                {
                    list.Add(widgetdate.WidgetID);
                }
            }


            return list;
        }

        #endregion

    }
}
