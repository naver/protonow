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
using Microsoft.Practices.Prism.Events;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.CommonBase;

namespace DockingLayout
{
    /// <summary>
    /// Interaction logic for DockingLayoutView.xaml
    /// </summary>
    public partial class DockingLayoutView : UserControl
    {
        DockingLayoutViewModel viewModel;
        public DockingLayoutView()
        {
            InitializeComponent();
           
            viewModel = new DockingLayoutViewModel(dockingManager);
            this.DataContext = viewModel;
            Loaded += DockingLayoutView_Loaded;
            
        }

        void DockingLayoutView_Loaded(object sender, RoutedEventArgs e)
        {
           viewModel.LoadLayout(Layout_config.Custom);
        }
    }
}
