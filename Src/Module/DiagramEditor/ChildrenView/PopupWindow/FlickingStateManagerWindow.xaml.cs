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
using System.Windows.Shapes;

namespace Naver.Compass.Module
{
    /// <summary>
    /// Interaction logic for AddDeviceWindow.xaml
    /// </summary>
    public partial class FlickingStateManagerWindow : BaseWindow
    {
        public FlickingStateManagerWindow()
        {
            InitializeComponent();
            //this.DataContext = new EditDeviceWindowModel();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DeviceListBox.Focus();
        }
    }
}
