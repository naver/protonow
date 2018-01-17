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
    /// Interaction logic for PagePropertyView.xaml
    /// </summary>
    public partial class PagePropertyView : UserControl
    {
        public PagePropertyView()
        {
            InitializeComponent();
            this.DataContext = new PagePropertyViewModel();
        }

    }
}
