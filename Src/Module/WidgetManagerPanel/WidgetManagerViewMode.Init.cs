using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;
using System.Windows.Threading;

namespace Naver.Compass.Module
{
    partial class WidgetManagerViewMode
    {
        private void InitSearchTimer()
        {
            _inputDelayTimer = new DispatcherTimer();
            _inputDelayTimer.Interval = TimeSpan.FromMilliseconds(300);
            _inputDelayTimer.Tick += InputDelayTimer_Tick;

            SearchString = String.Empty;
            HideSearchBox = true;
        }

        public void PageChangeEventHandler(Guid EventArg)
        {
            try
            {
                if (EventArg.CompareTo(Guid.Empty) == 0)
                {
                    ClearData("None page selected");
                    return;
                }

                if ((_Page != null) && _Page.Guid.CompareTo(EventArg) == 0)
                {
                    return;
                }

                IPagePropertyData pageVM = _selectionService.GetCurrentPage();

                if (pageVM != null && pageVM.EditorCanvas != null)
                {
                    _expandStatusList.Clear();//Clear expand info when Change page.

                    FlashPageData(pageVM);
                }
            }
            catch (System.Exception ex)
            {
                NLogger.Error("PageChangeEventHandler" + ex.Message);
            }
        }

        private void FlashPageData(IPagePropertyData pageVM)
        {
            try
            {
                if (pageVM != null)
                {
                    IWidget parentWidget = null;

                    if (pageVM.PageType == PageType.NormalPage)
                    {
                        IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();

                        if (doc == null || doc.Document == null || doc.Document.Pages == null)
                        {
                            return;
                        }

                        _Page = doc.Document.Pages.GetPage(pageVM.PageGID);

                        if (_Page == null || _Page.IsOpened == false)
                        {
                            return;
                        }

                        RootName = _Page.Name;
                        RootType = ListItemType.PageItem;
                    }
                    else if (pageVM.PageType == PageType.MasterPage)
                    {
                        if (pageVM.ActivePage == null || pageVM.ActivePage.IsOpened == false)
                        {
                            return;
                        }

                        _Page = pageVM.ActivePage;

                        RootName = _Page.Name;
                        RootType = ListItemType.MasterItem;
                    }
                    else
                    {
                        if (pageVM.ActivePage == null || pageVM.ActivePage.IsOpened == false)
                        {
                            return;
                        }

                        _Page = pageVM.ActivePage;

                        if (_Page is IEmbeddedPage)
                        {
                            if (pageVM.PageType == PageType.ToastPage)
                                RootType = ListItemType.ToastItem;
                            else if (pageVM.PageType == PageType.HamburgerPage)
                                RootType = ListItemType.MenuItem;
                            else if (pageVM.PageType == PageType.DynamicPanelPage)
                                RootType = ListItemType.DynamicPanelItem;

                            parentWidget = ((IEmbeddedPage)_Page).ParentWidget;
                            RootName = parentWidget.Name + " (" + GetWidgetTypeString(RootType) + ")";
                        }
                    }

                    CmdTarget = pageVM.EditorCanvas;

                    if (parentWidget != null)
                    {
                        _rootItem.WidgetID = parentWidget.Guid;
                    }
                    else
                    {
                         _rootItem.WidgetID = _Page.Guid;
                    }

                    _rootItem.OrderedChildren.Clear();
                    if (pageVM.PageType == PageType.DynamicPanelPage)
                    {
                        WidgetListItem.BelongWidget = parentWidget;
                        InitDynamicPanel(parentWidget as IDynamicPanel, _rootItem, pageVM.CurAdaptiveViewGID);
                    }
                    else
                    {
                        WidgetListItem.BelongWidget = parentWidget;
                        CovertPageData(_Page, pageVM.CurAdaptiveViewGID, _rootItem);
                    }

                    UpdateUIData();

                }
            }
            catch (System.Exception ex)
            {
                NLogger.Error("FlashPageData" + ex.Message);
            }

        }

        private void GetGroupWidget(IGroup group, List<IWidget> list)
        {
            foreach (IWidget widget in group.Widgets)
            {
                list.Add(widget);
            }

            if (group.Groups != null && group.Groups.Count > 0)
            {
                foreach (IGroup childGroup in group.Groups)
                {
                    GetGroupWidget(childGroup, list);
                }
            }
        }

        private void CovertWidget(IRegion region, IPage belongPage, WidgetListItem ParentItem)
        {
            WidgetListItem Item = null;

            if (region is IWidget)
            {
                IWidget widget = region as IWidget;
                if (widget is IDynamicPanel)
                {
                    Item = new SwipeViewListItem(widget as IDynamicPanel);
                    Item.ItemType = ListItemType.DynamicPanelItem;
                }
                else if (widget is IHamburgerMenu)
                {
                    Item = new HamburgerListItem(widget as IHamburgerMenu);
                    Item.ItemType = ListItemType.MenuItem;
                }
                else if (widget is IToast)
                {
                    Item = new ToastViewListItem(widget as IToast);
                    Item.ItemType = ListItemType.ToastItem;
                }
                else
                {
                    Item = new WidgetListItem(widget);
                    Item.ItemType = ListItemType.defaultItem;
                }
            }
            else if (region is IMaster)
            {
                IMaster master = region as IMaster;
                Item = new MasterListItem(master as IMaster);
                Item.ItemType = ListItemType.MasterItem;
            }

            Item.ParentPage = belongPage;
            Item.ParentID = ParentItem.WidgetID;

            Item.UpdateAllFlagByViewChange();

            ParentItem.OrderedChildren.Add(Item);
        }

        private void CovertGroup(IPage page, WidgetListItem ParentItem, IGroups groupList, List<Guid> GroupWidget, bool bTop = true)
        {
            foreach (IGroup group in groupList)
            {
                if (group.ParentGroup != null && bTop)
                {
                    continue;
                }

                WidgetListItem GroupItem = null;

                if (group.ParentGroup == null)
                {
                    GroupItem = new GroupListItem(group);
                    GroupItem.ItemType = ListItemType.GroupItem;
                }
                else
                {
                    GroupItem = new GroupChildListItem(group);
                    GroupItem.ItemType = ListItemType.GroupChildItem;
                    GroupItem.OptionFlag = false;
                }

                GroupItem.ParentPage = page;
                GroupItem.ParentID = ParentItem.WidgetID;
                GroupItem.IsExpanded = true;

                GroupItem.OrderedChildren = new List<WidgetListItem>();

                CovertGroup(page, GroupItem, group.Groups, GroupWidget, false);
                foreach (IRegion region in group.Widgets)
                {
                    CovertWidget(region, page, GroupItem);
                    GroupWidget.Add(region.Guid);
                }
                foreach (IRegion region in group.Masters)
                {
                    CovertWidget(region, page, GroupItem);
                    GroupWidget.Add(region.Guid);
                }

                GroupItem.OrderedChildren.Sort(CompareByZorder);

                if (GroupItem.OrderedChildren.Count > 0)
                {
                    GroupItem.zOrder = GroupItem.OrderedChildren[0].zOrder;
                }
                if (bTop)
                {
                    TreatGroupFlags(GroupItem as GroupListItem);
                }

                ParentItem.OrderedChildren.Add(GroupItem);
            }
        }

        private bool CovertPageData(IPage page, Guid curViewID, WidgetListItem ParentItem)
        {
            try
            {
                WidgetListItem.CurViewID = curViewID;

                List<Guid> ALLGroupWidgetList = new List<Guid>();

                CovertGroup(page, ParentItem, page.Groups, ALLGroupWidgetList);

                foreach (IRegion region in page.WidgetsAndMasters)
                {
                    if (ALLGroupWidgetList.Contains(region.Guid))
                    {
                        continue;
                    }

                    CovertWidget(region, page, ParentItem);
                }

                ParentItem.OrderedChildren.Sort(CompareByZorder);

            }
            catch (System.Exception ex)
            {
                NLogger.Error("CovertPageData" + ex.Message.ToString());
            }


            return true;
        }

        private void InitDynamicPanel(IDynamicPanel Panel, WidgetListItem parentItem, Guid curViewID)
        {
            bool bSelfOPen = false;
            parentItem.IsExpanded = true;
            parentItem.OrderedChildren = new List<WidgetListItem>();
            foreach (IPanelStatePage statePage in Panel.PanelStatePages)
            {
                bSelfOPen = false;
                WidgetListItem stateItem = new SwipeViewPanelListItem(statePage);
                stateItem.ItemType = ListItemType.DynamicPanelStateItem;

                stateItem.ParentID = parentItem.WidgetID;
                stateItem.ParentPage = statePage;

                stateItem.IsExpanded = true;
                stateItem.OrderedChildren = new List<WidgetListItem>();

                if (!statePage.IsOpened)
                {
                    statePage.Open();
                    bSelfOPen = true;
                }

                CovertPageData(statePage, curViewID, stateItem);


                if (bSelfOPen && statePage.IsOpened)
                {
                    statePage.Close();
                }

                TreatSwipePanelFlags(stateItem);

                stateItem.OrderedChildren.Sort(CompareByZorder);

                parentItem.OrderedChildren.Add(stateItem);
            }
        }

        private void FilterDataBySearch(WidgetListItem rootItem)
        {
            rootItem.OnSearch(_searchString, new Stack<WidgetListItem>());
        }

        private List<WidgetListItem> FilterWidgetList(List<WidgetListItem> AllList)
        {
            List<WidgetListItem> rList = new List<WidgetListItem>();

            foreach (WidgetListItem item in AllList)
            {
                if (item.UnFilter && item.InSearch)
                {
                    rList.Add(item);
                }
            }

            return rList;
        }

        private void FilterDataAndUpdate()
        {

            List<WidgetListItem> FilterList = FilterWidgetList(_allWidgets);

            foreach (Guid ID in _expandStatusList.Keys)
            {
                foreach (WidgetListItem item in FilterList)
                {
                    if (item.WidgetID.Equals(ID))
                    {
                        item.IsExpanded = _expandStatusList[ID];

                        ExpandCollapseWidget(item, _expandStatusList[ID]);

                        break;
                    }
                }
            }

            UIWidgetItems = null;
         

            UIWidgetItems = FilterList;

        }

        private void UpdateUIData()
        {
            try
            {
                _allWidgets.Clear();

                _allWidgets = new List<WidgetListItem>();

                SetFilterFlag(_rootItem.OrderedChildren);

                FilterDataBySearch(_rootItem);

                UpdateTreeDataToFlatList(_rootItem, _allWidgets, 1, false);

                FilterDataAndUpdate();


            }
            catch (System.Exception ex)
            {
                NLogger.Error("UpdateUIData->" + ex.Message);
            }

        }

        private void UpdateTreeDataToFlatList(WidgetListItem listItem, List<WidgetListItem> flatList, int level, bool CollapseFlag)
        {
            foreach (WidgetListItem Item in listItem.OrderedChildren)
            {
                if (Item.OrderedChildren != null && Item.OrderedChildren.Count > 0)
                {
                    Item.HasChildren = true;
                    Item.Lavel = level;
                    Item.CollapseFlag = CollapseFlag;
                    flatList.Add(Item);

                    UpdateTreeDataToFlatList(Item, flatList, Item.Lavel + 1, !Item.IsExpanded);
                }
                else
                {
                    Item.HasChildren = false;
                    Item.Lavel = level;
                    Item.CollapseFlag = CollapseFlag;

                    flatList.Add(Item);
                }
            }
        }

        private static int CompareByZorder(WidgetListItem widget1, WidgetListItem widget2)
        {
            if (widget1 == null)
            {
                if (widget2 == null)
                {
                    return 0;
                }

                return -1;
            }

            if (widget2 == null)
            {
                return 1;
            }

            return (widget2.zOrder - widget1.zOrder); ;
        }

        private string GetWidgetTypeString(IWidget widget)
        {

            switch (widget.WidgetType)
            {
                case WidgetType.Shape:
                    return GetShapeTypeString(widget as IShape);
                case WidgetType.Image:
                    return GlobalData.FindResource("widgets_Image");
                case WidgetType.DynamicPanel:
                    return GlobalData.FindResource("widgets_SwipeViews");
                case WidgetType.HamburgerMenu:
                    return GlobalData.FindResource("widgets_DrawerMenu");
                case WidgetType.Line:
                    return GetLineTypeString(widget as ILine);
                case WidgetType.HotSpot:
                    return GlobalData.FindResource("widgets_Link");
                case WidgetType.TextField:
                    return GlobalData.FindResource("widgets_Textfield");
                case WidgetType.TextArea:
                    return GlobalData.FindResource("widgets_Textarea");
                case WidgetType.DropList:
                    return GlobalData.FindResource("widgets_Droplist");
                case WidgetType.ListBox:
                    return GlobalData.FindResource("widgets_Listbox");
                case WidgetType.Checkbox:
                    return GlobalData.FindResource("widgets_Checkbox");
                case WidgetType.RadioButton:
                    return GlobalData.FindResource("widgets_Radiobutton");
                case WidgetType.Button:
                    return GlobalData.FindResource("widgets_Button");
                case WidgetType.Toast:
                    return GlobalData.FindResource("widgets_Toast");
                case WidgetType.SVG:
                    return GlobalData.FindResource("widgets_SVG");
                default:
                    return "errortype";
            }


        }

        private string GetWidgetTypeString(ListItemType type)
        {
            switch (type)
            {
                case ListItemType.DynamicPanelItem:
                    return GlobalData.FindResource("widgets_SwipeViews");
                case ListItemType.MenuItem:
                    return GlobalData.FindResource("widgets_DrawerMenu");

                case ListItemType.ToastItem:
                    return GlobalData.FindResource("widgets_Toast");

                default:
                    return "errortype";
            }
        }

        private string GetShapeTypeString(IShape widget)
        {
            if (widget != null)
            {
                switch (widget.ShapeType)
                {
                    case ShapeType.Rectangle:
                        return GlobalData.FindResource("widgets_Rectangle");
                    case ShapeType.RoundedRectangle:
                        return GlobalData.FindResource("widgets_RoundedRectangle");
                    case ShapeType.Ellipse:
                        return GlobalData.FindResource("widgets_Circle");
                    case ShapeType.Diamond:
                        return GlobalData.FindResource("widgets_Diamond");
                    case ShapeType.Triangle:
                        return GlobalData.FindResource("widgets_Triangle");
                    case ShapeType.Paragraph:
                        return GlobalData.FindResource("widgets_Text");
                    default:
                        return "errortype";
                }
            }
            return "errortype";
        }

        private string GetLineTypeString(ILine widget)
        {
            if (widget != null)
            {
                if (widget.Orientation == Orientation.Horizontal)
                {
                    return GlobalData.FindResource("widgets_HorizontalLine");
                }
                else
                {
                    return GlobalData.FindResource("widgets_VerticalLine");
                }
            }

            return "errortype";
        }

        private void TreatGroupFlags(GroupListItem GroupItem)
        {

            System.Diagnostics.Debug.Assert(GroupItem != null, "TreatGroupFlags->GroupItem is null");
            bool? HideFlag = null;

            GroupItem.OptionFlag = true;

            List<WidgetListItem> Alllist = new List<WidgetListItem>();
            GetGroupAllwidget(GroupItem, Alllist);

            foreach (WidgetListItem item in Alllist)
            {
                if (HideFlag == null)
                {
                    HideFlag = item.HideFlag;
                }
                else if (HideFlag != item.HideFlag)
                {
                    GroupItem.HideFlag = false;
                    GroupItem.OptionFlag = false;
                    return;
                }
            }

            GroupItem.HideFlag = HideFlag.Value;
        }

        private void GetGroupAllwidget(WidgetListItem item, List<WidgetListItem> list)
        {

            if (item.OrderedChildren != null)
            {
                foreach (WidgetListItem childitem in item.OrderedChildren)
                {
                    if (childitem.ItemType == ListItemType.GroupChildItem)
                    {
                        GetGroupAllwidget(childitem, list);
                    }
                    else
                    {
                        list.Add(childitem);
                    }
                }
            }
        }

        private void TreatSwipePanelFlags(WidgetListItem PanelItem)
        {
            bool? HideFlag = null;

            PanelItem.OptionFlag = true;
            if (PanelItem.OrderedChildren != null)
            {
                foreach (WidgetListItem item in PanelItem.OrderedChildren)
                {

                    if (HideFlag == null)
                    {
                        if ((item.ItemType == ListItemType.GroupItem) && (item.OptionFlag == false))
                        {
                            PanelItem.HideFlag = false;
                            PanelItem.OptionFlag = false;
                            return;
                        }
                        else
                        {
                            HideFlag = item.HideFlag;
                        }
                    }
                    else if ((item.ItemType == ListItemType.GroupItem) && (item.OptionFlag == false) || (HideFlag != item.HideFlag))
                    {
                        PanelItem.HideFlag = false;
                        PanelItem.OptionFlag = false;
                        return;
                    }
                }

                PanelItem.HideFlag = (HideFlag == null) ? false : HideFlag.Value;
            }
        }

        private bool GetSwipePanelHideFlag(List<WidgetListItem> list)
        {
            foreach (WidgetListItem widget in list)
            {
                if (widget.HideFlag)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
