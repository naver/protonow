using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;
using System.Windows.Controls;

namespace Naver.Compass.Module
{
    partial class WidgetManagerViewMode
    {
        #region Command

        public DelegateCommand<object> WidgetForWardCommand { get; private set; }
        public DelegateCommand<object> WidgetBackWardCommand { get; private set; }
        public DelegateCommand<object> WidgetDeleteCommand { get; private set; }
        public DelegateCommand<object> WidgetSearchCommand { get; private set; }
        public DelegateCommand<object> WidgetPlaceCommand { get; private set; }
        public DelegateCommand<object> WidgetUnPlaceCommand { get; private set; }

        public DelegateCommand<object> WidgetSwitchDisplayCommand { get; private set; }
        public DelegateCommand<object> WidgetEditCommand { get; private set; }

        public DelegateCommand<object> WidgetCutCommand { get; private set; }
        public DelegateCommand<object> WidgetCopyCommand { get; private set; }

        public DelegateCommand<object> WidgetSearchChangedCommand { get; private set; }

        public IInputElement CmdTarget
        {
            get;
            set;
        }


        #endregion

        #region Private command process

        private void InitCommand()
        {
            CmdTarget = null;

            this.WidgetForWardCommand = new DelegateCommand<object>(WidgetForWardExecute, CanExecuteWidgetForWard);
            this.WidgetBackWardCommand = new DelegateCommand<object>(WidgetBackWardExecute, CanExecuteWidgetBackWard);
            this.WidgetDeleteCommand = new DelegateCommand<object>(WidgetWidgetDeleteExecute, CanExecuteWidgetDelete);
            this.WidgetSearchCommand = new DelegateCommand<object>(WidgetSearchExecute, CanExecuteWidgetSearch);

            this.WidgetSwitchDisplayCommand = new DelegateCommand<object>(ExcuteSwitchDisplayStatus);
            this.WidgetEditCommand = new DelegateCommand<object>(ExcuteWidgetEditCommand);

            this.WidgetCutCommand = new DelegateCommand<object>(ExcuteWidgetCutCommand);
            this.WidgetCopyCommand = new DelegateCommand<object>(ExcuteWidgetCopyCommand);

            this.WidgetPlaceCommand = new DelegateCommand<object>(ExcuteWidgetPlaceCommand);
            this.WidgetUnPlaceCommand = new DelegateCommand<object>(ExcuteWidgetUnplaceCommand);

            this.WidgetSearchChangedCommand = new DelegateCommand<object>(ExcuteWidgetSearchCommand);
        }

        private void RefreshToolbarCommands()
        {
            WidgetForWardCommand.RaiseCanExecuteChanged();
            WidgetBackWardCommand.RaiseCanExecuteChanged();
            WidgetDeleteCommand.RaiseCanExecuteChanged();
        }

        private string _lastSearchText = string.Empty;
        private void InputDelayTimer_Tick(object sender, EventArgs e)
        {
            this._inputDelayTimer.Stop();
            if (_lastSearchText != _searchString)
            {
                UpdateUIData();
                _lastSearchText = this._searchString;
            }
        }

        private void ExcuteWidgetSearchCommand(object cmdParameter)
        {
            if (cmdParameter is object[])
            {
                var cmdparameters = cmdParameter as object[];
                if (cmdparameters.Length == 2 && cmdparameters[1] is TextChangedEventArgs)
                {
                    var textChangedE = cmdparameters[1] as TextChangedEventArgs;
                    if (textChangedE.Source is TextBox)
                    {
                        _searchString = (textChangedE.Source as TextBox).Text.Trim();

                        if (!this._searchString.Any(c => c < 0x20 || c > 0x7e) && this._searchString.Length < 2)
                        {
                            _searchString = string.Empty;
                        }

                        if (this._inputDelayTimer.IsEnabled)
                        {
                            this._inputDelayTimer.Stop();
                        }

                        this._inputDelayTimer.Start();
                    }
                }
            }
        }

        private void OnSearchListItem()
        {
            if (!this._searchString.Any(c => c < 0x20 || c > 0x7e) && this._searchString.Length < 2)
            {
                _searchString = string.Empty;
            }

            if (this._inputDelayTimer.IsEnabled)
            {
                this._inputDelayTimer.Stop();
            }

            this._inputDelayTimer.Start();
        }

        private void ExcuteWidgetCutCommand(object cmdParameter)
        {
            System.Windows.Input.ApplicationCommands.Cut.Execute(cmdParameter, CmdTarget);
        }

        private void ExcuteWidgetCopyCommand(object cmdParameter)
        {
            System.Windows.Input.ApplicationCommands.Copy.Execute(cmdParameter, CmdTarget);
        }

        private void ExcuteWidgetPlaceCommand(object cmdParameter)
        {
            if (_oldSelectedList.Count > 0)
            {
                WDMgrPlaceStatusChangeInfo info = new WDMgrPlaceStatusChangeInfo();
                info.bPlace = true;

                foreach (Guid gui in _oldSelectedList)
                    info.WidgetList.Add(gui);


                _ListEventAggregator.GetEvent<WdgMgrPlacewidgetEvent>().Publish(info);
            }
        }

        private void ExcuteWidgetUnplaceCommand(object cmdParameter)
        {
            if (_oldSelectedList.Count > 0)
            {
                WDMgrPlaceStatusChangeInfo info = new WDMgrPlaceStatusChangeInfo();
                info.bPlace = false;

                foreach (Guid gui in _oldSelectedList)
                     info.WidgetList.Add(gui);

                _ListEventAggregator.GetEvent<WdgMgrPlacewidgetEvent>().Publish(info);
            }
        }


        private void ExcuteWidgetEditCommand(object cmdParameter)
        {
            Guid ItemID = (Guid)cmdParameter;

            WidgetListItem item = FindItemByGUID(ItemID);

            if (item != null)
            {
                if (item.ItemType == ListItemType.defaultItem)
                {
                    _ListEventAggregator.GetEvent<WdgMgrEditSelectionEvent>().Publish(item.ParentPage.Guid);
                }
                else
                {
                    OnOpenChildwidgetPage(item);
                }
            }
        }


        private void ExcuteSwitchDisplayStatus(object cmdParameter)
        {
            Guid ItemID = (Guid)cmdParameter;

            WidgetListItem item = FindItemByGUID(ItemID);

            if (item != null && CmdTarget != null && item.OptionFlag)
            {

                //if (item.ItemType == ListItemType.DynamicPanelStateItem)
                //{
                //    _ListEventAggregator.GetEvent<WdgMgrHideSelectionEvent>().Publish(item.ParentPage.Guid);
                //}
                //else
                //{
                //    WidgetPropertyCommands.Hide.Execute(!item.HideFlag, CmdTarget);
                //}

                //return;

                WDMgrHideStatusChangeInfo info = new WDMgrHideStatusChangeInfo();

                info.PageID = item.ParentPage.Guid;
                info.ID = item.WidgetID;
                info.HideFlag = item.HideFlag;
                if (item.ItemType == ListItemType.DynamicPanelStateItem)
                {
                    info.HideType = WDMgrHideEventEnum.SwipeViewPanel;
                }
                else
                {
                    info.HideType = WDMgrHideEventEnum.NormalWidget;
                }

                _ListEventAggregator.GetEvent<WdgMgrHideSelectionEvent>().Publish(info);
            }
        }


        private void WidgetBackWardExecute(object cmdParameter)
        {
            OnChangeZorder(false);
        }

        private bool CanExecuteWidgetBackWard(object cmdParameter)
        {
            List<Guid> SelectList = GetSelectItemList();

            Guid ParentGUID = Guid.Empty;
            if (SelectList.Count == 0)
            {
                return false;
            }

            foreach (Guid itemID in SelectList)
            {
                WidgetListItem curitem = FindItemByGUID(itemID);

                if (curitem == null)
                {
                    continue;
                }

                if (curitem.PlaceFlag)
                {
                    return false;
                }

                if (ParentGUID.Equals(Guid.Empty))
                {
                    ParentGUID = curitem.ParentID;
                }
                else if (!ParentGUID.Equals(curitem.ParentID))
                {
                    return false;
                }

                if (IsMinOrderInPage(curitem))
                {
                    return false;
                }
            }

            return true;
        }

        private void WidgetForWardExecute(object cmdParameter)
        {
            OnChangeZorder(true);
        }

        private bool CanExecuteWidgetForWard(object cmdParameter)
        {
            List<Guid> SelectList = GetSelectItemList();

            Guid ParentGUID = Guid.Empty;
            if (SelectList.Count == 0)
            {
                return false;
            }

            foreach (Guid itemID in SelectList)
            {
                WidgetListItem curitem = FindItemByGUID(itemID);

                if (curitem == null)
                {
                    continue;
                }

                if (curitem.PlaceFlag)
                {
                    return false;
                }

                if (ParentGUID.Equals(Guid.Empty))
                {
                    ParentGUID = curitem.ParentID;
                }
                else if (!ParentGUID.Equals(curitem.ParentID))
                {
                    return false;
                }

                if (IsMaxOrderInPage(curitem))
                {
                    return false;
                }
            }

            return true;

        }

        private void WidgetWidgetDeleteExecute(object cmdParameter)
        {
            List<Guid> SelectList = GetSelectItemList();


            if (SelectList.Count > 0)
            {
                Guid ItemID = SelectList.First();

                WidgetListItem Item = FindUIItemByGUID(ItemID);
                if (Item != null)
                {
                    WDMgrWidgetDeleteInfo info = new WDMgrWidgetDeleteInfo();
                    info.PageID = Item.ParentPage.Guid;
                    info.WidgetList = SelectList;

                    _ListEventAggregator.GetEvent<WdgMgrDeleteSelectionEvent>().Publish(info);
                }
            }
        }

        private bool CanExecuteWidgetDelete(object cmdParameter)
        {
            List<Guid> SelectList = GetSelectItemList();

            if (SelectList.Count == 0)
            {
                return false;
            }

            return true;
        }

        private void WidgetSearchExecute(object cmdParameter)
        {
            HideSearchBox = !HideSearchBox;

            if (HideSearchBox)
            {
                SearchString = String.Empty;
            }
        }

        private bool CanExecuteWidgetSearch(object cmdParameter)
        {
            return true;
        }

        private void WidgetWidgetFilter(object cmdParameter)
        {

        }

        private void OnChangeZorder(bool bForward)
        {
            List<Guid> SelectList = GetSelectItemList();
            if (SelectList.Count != 0)
            {
                Guid ItemID = SelectList.First();

                WidgetListItem Item = FindItemByGUID(ItemID);
                if (Item != null && Item.ParentPage != null)
                {
                    WDMgrZorderChangeInfo Info = new WDMgrZorderChangeInfo();
                    Info.PageID = Item.ParentPage.Guid;
                    Info.bForward = bForward;
                    _ListEventAggregator.GetEvent<WdgMgrZorderChangedEvent>().Publish(Info);
                }
            }
        }

        public void ProcessDrapDropChangeZorder(WidgetListItem DragItem, WidgetListItem TargetItem)
        {
            WDMgrZorderDragChangeInfo info = new WDMgrZorderDragChangeInfo();

            info.PageID = DragItem.ParentPage.Guid;
            info.widgetID = DragItem.WidgetID;
            
            if (TargetItem == null)
            {
                TargetItem = _rootItem;
            }
            
            if (TargetItem.OrderedChildren != null && TargetItem.OrderedChildren.Count > 0)
            {
                if (DragItem.ParentID.Equals(TargetItem.WidgetID))
                {
                    info.zIndex = TargetItem.OrderedChildren[0].zOrder + 1;
                }
                else if(DragItem.ParentID.Equals(TargetItem.ParentID))
                {
                    info.zIndex = GetMinZorder(TargetItem);
                }
                else
                {
                    NLogger.Error("ProcessDrapDropChangeZorder->target item error!");
                }
            }
            else
            {
                info.zIndex = TargetItem.zOrder;
            }

            System.Diagnostics.Debug.WriteLine(TargetItem.WidgetTypeName+"->"+info.zIndex.ToString());

            _ListEventAggregator.GetEvent<WdgMgrOrderwidgetEvent>().Publish(info);
        }

        private int GetMinZorder(WidgetListItem item)
        {
            System.Diagnostics.Debug.Assert(item.OrderedChildren.Count > 0, "GetMinZorder->child count error!");
            WidgetListItem childitem = item.OrderedChildren[item.OrderedChildren.Count - 1];

            int rValue = -1;
            if (childitem.OrderedChildren != null && childitem.OrderedChildren.Count > 0)
            {
                return  GetMinZorder(childitem);
            }
            else
            {
                return childitem.zOrder;
            }
        }

        public bool IsChildofRootItem(WidgetListItem item)
        {
            if (item.ParentID == _rootItem.WidgetID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsMaxOrderInPage(WidgetListItem curitem)
        {

            if (curitem.zOrder < 0)
            {
                return true;
            }

            WidgetListItem ParentItem = FindUIItemByGUID(curitem.ParentID);

            if (ParentItem == null)
            {
                ParentItem = _rootItem;
            }

            foreach (WidgetListItem item in ParentItem.OrderedChildren)
            {
                if (curitem.zOrder < item.zOrder)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsMinOrderInPage(WidgetListItem curitem)
        {
            if (curitem.zOrder < 0)
            {
                return true;
            }

            WidgetListItem ParentItem = FindUIItemByGUID(curitem.ParentID);

            if (ParentItem == null)
            {
                ParentItem = _rootItem;
            }


            foreach (WidgetListItem item in ParentItem.OrderedChildren)
            {
                if (curitem.zOrder > item.zOrder)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Public command interface

        public void OnOpenChildwidgetPage(object obj)
        {
            try
            {
                WidgetListItem item = (WidgetListItem)obj;// as WidgetListItem;

                if ((item != null) && (item.ItemType == ListItemType.DynamicPanelItem ||
                    item.ItemType == ListItemType.MenuItem ||
                    item.ItemType == ListItemType.MasterItem ||
                    item.ItemType == ListItemType.ToastItem) && (item.OptionFlag))
                {
                    _ListEventAggregator.GetEvent<WdgMgrOpenChildWidgetPage>().Publish(item.WidgetID);
                }
            }
            catch (System.Exception ex)
            {
                NLogger.Info("OnOpenChildWidgetPage exception:" + ex.Message.ToString());
            }

        }

        #endregion
    }
}
