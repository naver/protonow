using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Module
{
    partial class WidgetManagerViewMode
    {
        public void OnWidgetZorderChanged(object obj)
        {
            UpdateListItemZorder();

            //if (_oldSelectedList.Count > 0)
            //{
            //    foreach (Guid id in _oldSelectedList)
            //    {
            //        WidgetListItem Item = FindItemByGUID(id);
            //        if (Item != null)
            //        {
            //            Item.UpdateItemSelectInfo(true);
            //        }
            //    }
            //}

            RefreshToolbarCommands();

            return;
        }

        private void UpdateListItemZorder()
        {
            IPagePropertyData pageVM = _selectionService.GetCurrentPage();

            //if (pageVM.PageType == PageType.DynamicPanelPage)
            //{
            //    UpDateDynamicZorder(_rootItem);
            //}
            //else
            //{
            ProcessPageZorderChange(_rootItem);
            //}

        }

        private void ProcessPageZorderChange(WidgetListItem item)
        {
            if (item == null || item.OrderedChildren == null)
            {
                return;
            }

            ProcessChildZorderChange(item);

            UpdateUIData();
        }

        private void ProcessChildZorderChange(WidgetListItem Item)
        {
            foreach (WidgetListItem childItem in Item.OrderedChildren)
            {
                if (childItem.OrderedChildren != null && childItem.OrderedChildren.Count > 0)
                {
                    ProcessChildZorderChange(childItem);
                }
            }

            Item.UpdateItemOrder();

            if (Item.OrderedChildren.Count > 0 && Item.ItemType != ListItemType.DynamicPanelItem)
            {
                Item.OrderedChildren.Sort(CompareByZorder);

                if (Item.ItemType == ListItemType.GroupItem || Item.ItemType == ListItemType.GroupChildItem)
                {
                    Item.zOrder = Item.OrderedChildren[0].zOrder;
                }
            }
        }

    }
}
