using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Naver.Compass.Common.Helper;
using System.Windows;
using System.Windows.Media.Imaging;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Module
{
    partial class WidgetManagerViewMode
    {
        public void OnMenuOpen()
        {

            if (IsEnableMenuOpen())
            {
                ContextMenu menu = new ContextMenu();
                menu.IsOpen = true;

                if (IsSelectUnplaceValue())
                {
                    AddPlaceMenu(menu);
                }
                else
                {
                    AddEditMenu(menu);
                    AddCutMenu(menu);
                    AddCopyMenu(menu);
                    AddUnPlaceMenu(menu);
                    menu.Items.Add(new Separator());
                    AddDeleteMenu(menu);
                    AddStatusMenu(menu);
                }
            }
        }

        private bool IsSelectEditValue()
        {
            foreach (Guid ID in _oldSelectedList)
            {
                WidgetListItem item = FindUIItemByGUID(ID);

                if (item != null && item.PlaceFlag == true)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsSelectUnplaceValue()
        {
            foreach (Guid ID in _oldSelectedList)
            {
                WidgetListItem item = FindUIItemByGUID(ID);

                if (item.PlaceFlag == true)
                {
                    return true;
                }
            }

            return false;
        }


        private bool IsEnableMenuOpen()
        {
            if (_oldSelectedList != null)
            {
                if (_oldSelectedList.Count < 1)
                {
                    return false;
                }
            }
            else
            {
                NLogger.Warn("IsEnableMenuOpen->_oldSelectedList = null");//test code for find out reason
                return false;
            }

            foreach (Guid ID in _oldSelectedList)
            {
                WidgetListItem item = FindUIItemByGUID(ID);

                if (item != null)
                {
                    if (item.ItemType == ListItemType.GroupItem || item.ItemType == ListItemType.DynamicPanelStateItem)
                    {
                        return false;
                    }
                }
                else
                {
                    NLogger.Warn("IsEnableMenuOpen->Founded item = null");//test code for find out reason
                    return false;
                }
            }

            return true;
        }

        private void AddEditMenu(ContextMenu menu)
        {
            MenuItem item = new MenuItem();//Edit
            item.Header = GlobalData.FindResource("ObjectListManager_ContextMenu_Edit");
            item.Style = Application.Current.Resources["topLevel"] as Style;
            item.Command = WidgetEditCommand;
            item.CommandParameter = _oldSelectedList[0];
            item.IsEnabled = false;

            if (_oldSelectedList.Count == 1)
            {
                WidgetListItem listitem = FindItemByGUID(_oldSelectedList[0]);

                if (listitem != null && !listitem.HideFlag)
                {
                    item.IsEnabled = true;
                }
            }

            menu.Items.Add(item);
        }

        private void AddCutMenu(ContextMenu menu)
        {
            MenuItem item = new MenuItem();//Edit
            item.Header = GlobalData.FindResource("ObjectListManager_ContextMenu_Cut");
            item.Style = Application.Current.Resources["topLevel"] as Style;
            item.Icon = Application.Current.Resources["Cut"];
            item.Command = WidgetCutCommand;
            menu.Items.Add(item);
        }

        private void AddCopyMenu(ContextMenu menu)
        {
            MenuItem item = new MenuItem();//Copy
            item.Header = GlobalData.FindResource("ObjectListManager_ContextMenu_Copy");
            item.Style = Application.Current.Resources["topLevel"] as Style;
            item.Icon = Application.Current.Resources["Copy"];
            item.Command = WidgetCopyCommand;
            menu.Items.Add(item);
        }

        private void AddUnPlaceMenu(ContextMenu menu)
        {
            if (_Page != null && _Page.PageViews.Count > 1)
            {
                menu.Items.Add(new Separator());

                MenuItem item = new MenuItem();//Copy
                item.Header = GlobalData.FindResource("ObjectListManager_ContextMenu_Resolution_delete");
                item.Style = Application.Current.Resources["topLevel"] as Style;
                item.Command = WidgetUnPlaceCommand;
                menu.Items.Add(item);
            }
        }

        private void AddDeleteMenu(ContextMenu menu)
        {
            MenuItem item = new MenuItem();//Delete
            item.Header = GlobalData.FindResource("ObjectListManager_ContextMenu_Delete");
            item.Style = Application.Current.Resources["topLevel"] as Style;
            item.Command = WidgetDeleteCommand;
            item.Icon = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/Naver.Compass.Module.WidgetManagerPanel;component/Resources/icon-16-delete.png", UriKind.RelativeOrAbsolute)) };
            menu.Items.Add(item);
        }

        private void AddPlaceMenu(ContextMenu menu)
        {
            MenuItem item = new MenuItem();//Delete
            item.Header = GlobalData.FindResource("ObjectListManager_ContextMenu_Resolution_add");
            item.Style = Application.Current.Resources["topLevel"] as Style;
            item.Command = WidgetPlaceCommand;
            menu.Items.Add(item);
        }

        private void AddStatusMenu(ContextMenu menu)
        {
            MenuItem item = new MenuItem();//Show status
            item.Icon = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/Naver.Compass.Module.WidgetManagerPanel;component/Resources/Visible.png", UriKind.RelativeOrAbsolute)) };
            item.Header = GlobalData.FindResource("Object List Manager_ContextMenu_Hide");
            item.Style = Application.Current.Resources["topLevel"] as Style;

            if (_oldSelectedList.Count == 1)
            {
                WidgetListItem Widget = FindItemByGUID(_oldSelectedList[0]);
                if (Widget.HideFlag)
                {
                    item.Header = GlobalData.FindResource("Object List Manager_ContextMenu_Show");
                    item.Icon = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/Naver.Compass.Module.WidgetManagerPanel;component/Resources/disable.png", UriKind.RelativeOrAbsolute)) };
                }

                item.Command = WidgetSwitchDisplayCommand;
                item.CommandParameter = _oldSelectedList[0];
            }
            else
            {
                item.IsEnabled = false;
            }

            menu.Items.Add(item);
        }
    }
}
