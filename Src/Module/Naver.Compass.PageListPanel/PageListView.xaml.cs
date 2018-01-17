using Naver.Compass.Common;
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

namespace Naver.Compass.Module
{
    /// <summary>
    /// Interaction logic for PageListView.xaml
    /// </summary>
    public partial class PageListView : UserControl
    {
        private PageListViewModel _viewmodel;
        public PageListView()
        {
            InitializeComponent();
            _viewmodel = new PageListViewModel(PageTreeView);
            this.DataContext = _viewmodel;
            this._dragInfo = new DragInfo();

            base.AddHandler(KeyDownEvent, new KeyEventHandler(PagesFrame_KeyDown), true);
        }

        private void PagesFrame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Make sure PageTree has focus so short cut key can work well.
            PageTreeView.Focus();
        }

        private void PagesFrame_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                PageTreeView.Focus();
            }
        }

        private void TreeItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                NodeViewModel selectItem = PageTreeView.SelectedItem as NodeViewModel;
                if (selectItem != null)
                {
                    selectItem.IsNodeEditable = true;
                }
            }
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
                
                var multiselected = this._viewmodel.GetAllNodes(this._viewmodel.RootNode).Where(n => n.IsMultiSelected);
                if (multiselected.Count() == 1)
                {
                    multiselected.First().IsSelected = true;
                    this._viewmodel.ClearMultiSelected();
                }
            }
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        private Point mouse_P;

        private void treeView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                mouse_P = new Point();
            }
        }

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                //do not support drag if Library mode
                if (e.LeftButton == MouseButtonState.Pressed && _viewmodel.IsStandardDocument)
                {
                    if (mouse_P == default(Point))
                    {
                        mouse_P = e.GetPosition(PageTreeView);
                    }
                    else
                    {
                        var pcurrent = e.GetPosition(PageTreeView);
                        var distance = Math.Sqrt(Math.Pow(mouse_P.X - pcurrent.X, 2) + Math.Pow(mouse_P.Y - pcurrent.Y, 2));
                        if (distance > 1)
                        {
                            var selectNode = PageTreeView.SelectedValue as NodeViewModel;
                            if (!selectNode.IsNodeEditable)
                            {
                                this._isEditable = false;
                                this._dragInfo.NodeViewModel = selectNode;
                                this.GetAllTreeViewItemPanel();
                                DragDropEffects finalDropEffect = DragDrop.DoDragDrop(PageTreeView, PageTreeView.SelectedValue, DragDropEffects.Move);
                            }
                            else
                            {
                                this._isEditable = true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void treeView_DragLeave(object sender, DragEventArgs e)
        {
            var type = e.OriginalSource.GetType();
            if (type.GetProperty("Name") != null)
            {
                dynamic objDynamic = e.OriginalSource;
                var onname = objDynamic.Name;
                if (onname == "InsertIndicatorEllipse" || onname == "InsertIndicatorGrid")
                {
                    return;
                }
            }


            this.RemoveLastInsertIndicator();
            this.HideIsDragInto();
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (_viewmodel.IsStandardDocument==false || this._isEditable || this._itemBorders == null)
                {
                    e.Handled = true;
                    e.Effects = DragDropEffects.None;
                    return;
                }

                ///如果当前位置位于InsertIndicator上那么直接忽略
                ///之前使用GetClosetParent[T]判断是否位于InsertIndicator上
                ///但是由于GetClosetParent[T]过于耗时，替换方法
                var type = e.OriginalSource.GetType();
                if (type.GetProperty("Name") != null)
                {
                    dynamic objDynamic = e.OriginalSource;
                    var onname = objDynamic.Name;
                    if (onname == "InsertIndicatorEllipse" || onname == "InsertIndicatorGrid")
                    {
                        e.Handled = true;
                        return;
                    }
                }

                this.RemoveLastInsertIndicator();
                this.HideIsDragInto();
                var relativePosition = default(Point);
                var border = this.GetClosetParentWithName<Grid>(e.OriginalSource as DependencyObject, "ItemBd");
                if (border != null)
                {

                    ///拖动到一个ItemBd上面，包括每个元素的图片文本以及后面的空白的位置
                    relativePosition = e.GetPosition(border);
                    if (relativePosition.Y <= 5)
                    {
                        ///如果在鼠标在ItemBd的Y坐标<=5,说明在元素的上沿
                        ///插入指示要放置在[上一个]元素
                        ///[上一个]元素也有两种情况：
                        ///1，如果当前ItemBd所在的TreeViewItem是父节点的第一个元素，那么[上一个]元素是父节点
                        ///2，如果当前ItemBd所在的TreeViewItem不是父节点的第一个元素，那么[上一个]元素是上一个兄弟节点
                        ///统一在拖动开始前获取所有ItemBd，然后在DragOver时获取上一个节点
                        var prepanel = this._itemBorders.TakeWhile(x => x != border).LastOrDefault();
                        if (prepanel != null)
                        {
                            ///上一个节点不为空
                            this.ShowIsDragInto(prepanel);
                            this._dragInfo.Parent = prepanel;
                            this._dragInfo.PreviousBrother = null;
                            if (!IsDescent(this._dragInfo.NodeViewModel, prepanel.DataContext as NodeViewModel))
                            {
                                ///不是拖动到自己或者自己的子节点内
                                this.ShowInsertIndicator(prepanel, System.Windows.VerticalAlignment.Bottom);

                            }
                            else
                            {
                                this.ShowDisabledInsertIndicator(prepanel, System.Windows.VerticalAlignment.Bottom);
                                e.Handled = true;
                                e.Effects = DragDropEffects.None;
                                return;
                            }
                        }
                        else
                        {
                            ///上一个节点为空，说明当前位置为第一个节点，插入到当前ItemBd的Top位置
                            this.ShowInsertIndicator(border, System.Windows.VerticalAlignment.Top);

                            this._dragInfo.Parent = null;
                            this._dragInfo.PreviousBrother = null;
                        }
                    }
                    else
                    {
                        ///否则，插入到当前ItemBd的Bottom位置
                        this.ShowIsDragInto(border);

                        this._dragInfo.Parent = border;
                        this._dragInfo.PreviousBrother = null;
                        if (!IsDescent(this._dragInfo.NodeViewModel, border.DataContext as NodeViewModel))
                        {
                            this.ShowInsertIndicator(border, System.Windows.VerticalAlignment.Bottom);
                        }
                        else
                        {
                            this.ShowDisabledInsertIndicator(border, System.Windows.VerticalAlignment.Bottom);
                            e.Handled = true;
                            e.Effects = DragDropEffects.None;
                            return;
                        }
                    }
                }
                else
                {
                    ///拖动到TreeViewItemPanel上面
                    var actualPanel = GetActualTreeviewPanel(e, e.OriginalSource as DependencyObject);
                    if (actualPanel != null)
                    {
                        var fstchildgrid = FindVisualChildren<Grid>(actualPanel).FirstOrDefault();
                        if (fstchildgrid != null)
                        {
                            var insertIndicator = new InsertIndicator();
                            var transform = new TranslateTransform(fstchildgrid.Margin.Left + 14, 3.5d);
                            insertIndicator.RenderTransform = transform;
                            insertIndicator.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                            insertIndicator.IsHitTestVisible = false;
                            this.ShowInsertIndicator(actualPanel, insertIndicator);

                            var parentPanel = this.GetClosetParentWithName<Grid>(VisualTreeHelper.GetParent(actualPanel), "TreeViewItemPanel");
                            if (parentPanel != null)
                            {
                                this.ShowIsDragInto(parentPanel);

                                this._dragInfo.Parent = parentPanel;
                                this._dragInfo.PreviousBrother = actualPanel;

                                if (IsDescent(this._dragInfo.NodeViewModel, parentPanel.DataContext as NodeViewModel))
                                {
                                    insertIndicator.IsEnable = false;
                                    e.Handled = true;
                                    e.Effects = DragDropEffects.None;
                                    return;
                                }
                            }
                            else
                            {
                                this._dragInfo.Parent = null;
                                this._dragInfo.PreviousBrother = actualPanel;
                            }
                        }
                    }
                }


                e.Handled = true;
                e.Effects = DragDropEffects.Move;
            }
            catch (Exception)
            {
            }
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                this.RemoveLastInsertIndicator();
                this.HideIsDragInto();
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                this._dragInfo.PerformChange(this.DataContext as PageListViewModel);
            }
            catch (Exception)
            {
            }
        }

        private Grid _lastPanel;
        private InsertIndicator _lastInsertIndicator;
        private IEnumerable<Grid> _allPanels;
        private IEnumerable<Grid> _itemBorders;
        private NodeViewModel _lastDragViewModel;
        private bool _isEditable;
        private DragInfo _dragInfo;

        private void GetAllTreeViewItemPanel()
        {
            var allgrids = FindVisualChildren<Grid>(PageTreeView);
            this._allPanels = allgrids.Where(x => x.IsVisible && x.Name == "TreeViewItemPanel");
            this._itemBorders = allgrids.Where(x => x.IsVisible && x.Name == "ItemBd");
#if DEBUG
            foreach (var panel in this._allPanels)
            {
                var tb = FindVisualChildren<TextBlock>(panel).FirstOrDefault();
                if (tb != null)
                {
                    System.Diagnostics.Debug.WriteLine(tb.Text);
                }
            }
#endif
        }

        private InsertIndicator CreateInsertIndicator(VerticalAlignment alignment)
        {
            var insertIndicator = new InsertIndicator();
            if (alignment == System.Windows.VerticalAlignment.Top)
            {
                var transform = new TranslateTransform(0d, -3.5d);
                insertIndicator.RenderTransform = transform;
            }
            else if (alignment == System.Windows.VerticalAlignment.Bottom)
            {
                var transform = new TranslateTransform(15d, 3d);
                insertIndicator.RenderTransform = transform;
            }

            insertIndicator.VerticalAlignment = alignment;
            insertIndicator.IsHitTestVisible = false;
            return insertIndicator;
        }

        private void ShowInsertIndicator(Grid panel, VerticalAlignment alignment)
        {
            var insertIndicator = CreateInsertIndicator(alignment);
            this.ShowInsertIndicator(panel, insertIndicator);
        }

        private void ShowDisabledInsertIndicator(Grid panel, VerticalAlignment alignment)
        {
            var insertIndicator = CreateInsertIndicator(alignment);
            insertIndicator.IsEnable = false;
            this.ShowInsertIndicator(panel, insertIndicator);
        }

        private void ShowInsertIndicator(Grid panel, InsertIndicator insertIndicator)
        {
            var columnCtn = panel.ColumnDefinitions.Count;
            var rowCtn = panel.RowDefinitions.Count;
            if (columnCtn > 1)
            {
                Grid.SetColumnSpan(insertIndicator, columnCtn);
            }

            if (rowCtn > 1)
            {
                Grid.SetRowSpan(insertIndicator, rowCtn);
            }

            panel.Children.Add(insertIndicator);
            this._lastPanel = panel;
            this._lastInsertIndicator = insertIndicator;
        }

        private void RemoveLastInsertIndicator()
        {
            if (this._lastPanel != null && this._lastInsertIndicator != null)
            {
                if (this._lastPanel.Children.Contains(this._lastInsertIndicator))
                {
                    this._lastPanel.Children.Remove(this._lastInsertIndicator);
                }
            }
        }

        private void ShowIsDragInto(Grid panel)
        {
            if (panel.DataContext is NodeViewModel)
            {
                var nodeVm = panel.DataContext as NodeViewModel;
                nodeVm.IsDragInto = true;
                _lastDragViewModel = nodeVm;
            }
        }

        private void HideIsDragInto()
        {
            if (_lastDragViewModel != null)
            {
                _lastDragViewModel.IsDragInto = false;
            }
        }

        internal static bool IsDescent(INodeViewModel root, INodeViewModel node)
        {
            if (node == null)
            {
                return false;
            }
            else if (root == node || node.Parent == root)
            {
                return true;
            }
            else
            {
                return IsDescent(root, node.Parent);
            }
        }

        private Grid GetActualTreeviewPanel(DragEventArgs e, DependencyObject dobj)
        {
            var panel = this.GetClosetParentWithName<Grid>(dobj, "TreeViewItemPanel");
            if (panel != null)
            {
                var relativePosition = e.GetPosition(panel);
                var childbds = FindVisualChildren<Grid>(panel);

                var childbd = childbds.FirstOrDefault();
                var p = childbd.TranslatePoint(new Point(), panel);
                if (p.X < relativePosition.X)
                {
                    return panel;
                }
                else
                {
                    return GetActualTreeviewPanel(e, VisualTreeHelper.GetParent(panel));
                }
            }

            return null;
        }

        private T GetClosetParent<T>(DependencyObject dobject) where T : DependencyObject
        {
            var parent = dobject;
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent is T ? (T)(object)parent : default(T);
        }

        private T GetClosetParentWithName<T>(DependencyObject dobject, string parentName) where T : DependencyObject
        {
            var parent = dobject as FrameworkElement;
            while (parent != null && (!(parent is T) || ((parent is T) && parent.Name != parentName)))
            {
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }

            return parent is T ? (T)(object)parent : default(T);
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void bd_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift)
                || Keyboard.IsKeyDown(Key.RightShift)
                || Keyboard.IsKeyDown(Key.LeftCtrl)
                || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                e.Handled = true;
            }
            else
            {
                var multiselected = this._viewmodel.GetAllNodes(this._viewmodel.RootNode).Where(n => n.IsMultiSelected);
                if (multiselected.Count() > 1)
                {
                    e.Handled = true;
                }
            }
        }

    }

    class DragInfo
    {
        /// <summary>
        /// 进行拖拽的Node
        /// </summary>
        public NodeViewModel NodeViewModel { get; set; }

        /// <summary>
        /// 拖拽到的目标Parent容器
        /// 该值为空，则拖动到根节点
        /// </summary>
        public Grid Parent { get; set; }

        /// <summary>
        /// 上一个兄弟节点
        /// 该值为空则说明是第一个节点
        /// </summary>
        public Grid PreviousBrother { get; set; }

        public void PerformChange(PageListViewModel pagelistVm)
        {
            if (pagelistVm == null || NodeViewModel == null)
            {
                return;
            }

            var parentNode = Parent == null ? null : Parent.DataContext as NodeViewModel;

            if (!PageListView.IsDescent(NodeViewModel, parentNode))
            {
                pagelistVm.DragTo(
                    NodeViewModel,
                    parentNode,
                    PreviousBrother == null ? null : PreviousBrother.DataContext as NodeViewModel);
            }
        }
    }
}
