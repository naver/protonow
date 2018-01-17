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
    public partial class SetDeviceWindow: BaseWindow
    {
        private SetDeviceWindowModel vm;
        public SetDeviceWindow()
        {
            InitializeComponent();
            vm = new SetDeviceWindowModel();
            this.DataContext = vm;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.vm.FreshOKCommandd();
        }

        private void NumEnterTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.vm.FreshOKCommandd();
        }
    }
}
