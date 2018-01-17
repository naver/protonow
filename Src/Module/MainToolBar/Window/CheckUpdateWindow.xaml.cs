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
    /// Interaction logic for CheckUpdate.xaml
    /// </summary>
    public partial class CheckUpdateWindow : BaseWindow
    {
        public CheckUpdateWindow()
        {
            InitializeComponent();
            this.DataContext = new CheckUpdateViewModel();
        }

        private void updateWin_Closed(object sender, EventArgs e)
        {
            CheckUpdateViewModel vm = DataContext as CheckUpdateViewModel;
            if(vm != null)
            {
                vm.Dispose();
            }
        }
    }
}
