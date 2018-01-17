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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MasterView : UserControl
    {
        private MasterListViewModel _viewmodel;
        public MasterView()
        {
            InitializeComponent();

            _viewmodel = new MasterListViewModel(PageTreeView);
            this.DataContext = _viewmodel;
            //this._dragInfo = new DragInfo();

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

            }
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }
        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                var selectNode = PageTreeView.SelectedValue as NodeViewModel;

                if (selectNode != null && !selectNode.IsNodeEditable)
                {
                    DataObject dataObject = new DataObject("MASTER_ITEM", selectNode.Guid);
                    DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
                }
            }
            
        }

    }
}
