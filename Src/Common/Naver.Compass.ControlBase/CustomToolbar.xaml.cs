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

namespace Naver.Compass.Common
{
    /// <summary>
    /// Interaction logic for UserToolbar.xaml
    /// </summary>
    public partial class CustomToolbar : ToolBar
    {
        public CustomToolbar()
        {
            InitializeComponent();
        }
        
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;

            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;

            if (overflowGrid != null)
            {

                overflowGrid.Visibility = Visibility.Collapsed;

            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;

            if (mainPanelBorder != null)
            {

                mainPanelBorder.Margin = new Thickness(0);

            }
        }
    }
}
