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

namespace Naver.Compass.Module.Styles
{
    /// <summary>
    /// Interaction logic for HamburgerStyle.xaml
    /// </summary>
    public partial class HamburgerStyle : UserControl
    {
        public HamburgerStyle()
        {
            InitializeComponent();
            this.DataContext = new HamburgerStyleViewModel();
        }
    }
}
