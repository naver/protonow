using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Interactivity;
using Naver.Compass.Common.Helper;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Module
{
    /// <summary>
    /// Interaction logic for WidgetManagerView.xaml
    /// </summary>
    public partial class WidgetManagerView : UserControl
    {
        public WidgetManagerView()
        {
            InitializeComponent();

            this.DataContext = new WidgetManagerViewMode(this);
        }

        private Point mouse_P;
        private WidgetListItem _dragInfo = null;
        private WidgetListItem _MouseDownInfo = null;

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is Image))
            {
                ListBoxItem Item = sender as ListBoxItem;

                (DataContext as WidgetManagerViewMode).OnOpenChildwidgetPage(Item.DataContext);
            }
          
        }

        private void Tree_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            (DataContext as WidgetManagerViewMode).OnMenuOpen();
        }

        private void Tree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool bFindItem = false;
            bool bFineListBox = false;

            DependencyObject obj = e.OriginalSource as DependencyObject;

            while (obj != null)
            {
                if (obj.GetType() == typeof(ScrollBar))
                {
                    break;
                }

                if (obj.GetType() == typeof(ListBoxItem))
                {
                    bFindItem = true;
                }

                if (obj.GetType() == typeof(ListBox))
                {
                   if (!bFindItem)
                   {
                       ObjectList.UnselectAll();
                       
                       break;
                   }
                }

                obj = System.Windows.Media.VisualTreeHelper.GetParent(obj);
            }

        }

        private void ToolTipOpenedHandler(object sender, RoutedEventArgs e)
        {
            ToolTip toolTip = (ToolTip)sender;
            UIElement target = toolTip.PlacementTarget;

            ScrollViewer sv =Naver.Compass.Common.CommonBase.CPSUIHelper.FindVisualChild<ScrollViewer>(ObjectList);
            if (sv != null && sv.ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                toolTip.Width = ObjectList.ActualWidth - 12;
            }
            else
            {
                toolTip.Width = ObjectList.ActualWidth;
            }

            toolTip.UpdateLayout();

            toolTip.HorizontalOffset = 24 - toolTip.ActualWidth;

            Point adjust = target.TranslatePoint(new Point(4, 0), toolTip);

            Point WidgetList = ObjectList.PointToScreen(new Point(0, 0));
            Point targetPos = target.PointToScreen(new Point(0, 0));
            if ((WidgetList.Y + ObjectList.ActualHeight) < (targetPos.Y + 20 + toolTip.ActualHeight))
            {
                toolTip.Placement = PlacementMode.Top;
                toolTip.Tag = new Thickness(adjust.X,-2, 0, 0);
            }
            else
            {
                toolTip.Placement = PlacementMode.Bottom;
                toolTip.Tag = new Thickness(adjust.X, -1.5, 0, -1.5);
            }
        }

        #region  ListBox item Drag

        private void ListBoxItem_DragOver(object sender, DragEventArgs e)
        {
            WidgetListItem item = (e.Source as ContentPresenter).Content as WidgetListItem;

            if (item != null && _dragInfo != null)
            {
                //if ((!item.ParentID.Equals(_dragInfo.ParentID))||
                //    (item.WidgetID.Equals(_dragInfo.WidgetID)) || 
                //    (item.zOrder - _dragInfo.zOrder == 1))
                if ((!item.ParentID.Equals(_dragInfo.ParentID))
                   )
                {
                    if (item.WidgetID.Equals(_dragInfo.ParentID))
                    {
                        item.EnableDropFlag = true;
                    }
                    else
                    {
                         e.Handled = true;
                         e.Effects = DragDropEffects.None;
                         return;
                    }
                }
                else
                {
                    item.EnableDropFlag = true;
                }
                
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Last drag info is error");
            }

        }

        private void ListBoxItem_DragLeave(object sender, DragEventArgs e)
        {
            WidgetListItem item = (e.Source as ContentPresenter).Content as WidgetListItem;

            if (item != null && _dragInfo != null)
            {
                item.EnableDropFlag = false;
            }
        }

        private void ListBoxItem_Drop(object sender, DragEventArgs e)
        {
            WidgetListItem item = (e.Source as ContentPresenter).Content as WidgetListItem;

            if (item != null && _dragInfo != null)
            {
                if (item.ParentID.Equals(_dragInfo.ParentID) || item.WidgetID.Equals(_dragInfo.ParentID))
                {
                    (DataContext as WidgetManagerViewMode).ProcessDrapDropChangeZorder(_dragInfo, item);
                }

                item.EnableDropFlag = false;
            }
        }

        private void ListBoxItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed )
            {
                if (mouse_P == default(Point))
                {
                    mouse_P = e.GetPosition(ObjectList);
                }
                else
                {
                    var pcurrent = e.GetPosition(ObjectList);
                    var distance = Math.Sqrt(Math.Pow(mouse_P.X - pcurrent.X, 2) + Math.Pow(mouse_P.Y - pcurrent.Y, 2));
                    if (distance > 2 && _MouseDownInfo != null)
                    {
                        var SelectItem = ObjectList.SelectedValue as WidgetListItem;
                        if (SelectItem != null && EnableDrag(SelectItem)  )
                        {
                            if (_MouseDownInfo.WidgetID.Equals(SelectItem.WidgetID))
                            {
                           
                                 _dragInfo = SelectItem;
                        
                                  DragDropEffects finalDropEffect = DragDrop.DoDragDrop(ObjectList, ObjectList.SelectedValue, DragDropEffects.Move);
                            }
                            
                        }
                    }
                }
            }
        }

        private void ListBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WidgetListItem item = (e.Source as ContentPresenter).Content as WidgetListItem;
            if (item != null)
            {
                _MouseDownInfo = item;
            }
        }

        private bool EnableDrag(WidgetListItem item)
        {

            if (!(item.ItemType == ListItemType.GroupChildItem ||
                item.ItemType == ListItemType.PageItem ||
                item.ItemType == ListItemType.DynamicPanelStateItem))
            {
                if (!item.PlaceFlag)
                {
                    return true;
                }
            }

            return false;

        }

        #endregion

        #region RootItem Drag And Drop

        private void Border_DragOver(object sender, DragEventArgs e)
        {
            if (sender is Border && ((Border)sender).Name.Equals("RootBorder"))
            {
                if (_dragInfo != null && ((DataContext as WidgetManagerViewMode).IsChildofRootItem(_dragInfo)))
                {
                    RootBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 157, 217));
                }
                else
                {
                    e.Handled = true;
                    e.Effects = DragDropEffects.None;
                    return;
                }
            }
        }

        private void Border_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is Border && ((Border)sender).Name.Equals("RootBorder"))
            {
                if (_dragInfo != null)
                {
                    RootBorder.BorderBrush= new SolidColorBrush(Color.FromRgb(232,232,232));
                }
            }
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border && ((Border)sender).Name.Equals("RootBorder"))
            {
                if (_dragInfo != null && ((DataContext as WidgetManagerViewMode).IsChildofRootItem(_dragInfo)))
                {
                    RootBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(232, 232, 232));

                    (DataContext as WidgetManagerViewMode).ProcessDrapDropChangeZorder(_dragInfo,null);
                }
            }
        }

        #endregion
    }

    public class DropDownButtonBehavior : Behavior<Button>
    {
        private bool isContextMenuOpen;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click), true);
        }

        void AssociatedObject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button source = sender as Button;
            if (source != null && source.ContextMenu != null)
            {
                if (!isContextMenuOpen)
                {
                    // Add handler to detect when the ContextMenu closes
                    source.ContextMenu.AddHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed), true);
                    // If there is a drop-down assigned to this button, then position and display it 
                    source.ContextMenu.PlacementTarget = source;
                    source.ContextMenu.Placement = PlacementMode.Bottom;
                    source.ContextMenu.IsOpen = true;
                    isContextMenuOpen = true;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click));
        }

        void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            isContextMenuOpen = false;
            var contextMenu = sender as ContextMenu;
            if (contextMenu != null)
            {
                contextMenu.RemoveHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed));
            }
        }
    }
   
}
