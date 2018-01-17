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
    /// Interaction logic for PropertyPage.xaml
    /// </summary>
    public partial class TabStylePage : UserControl
    {
        public TabStylePage()
        {
            InitializeComponent();
            this.DataContext = new Styles.StylePageViewModel();
        }

        private void OnFocused(object sender, MouseButtonEventArgs e)
        {
            //Focus change will causee the data update
            Grid frame = (sender as Grid);
            if(frame!=null)
            {
                frame.Focus();
            }            
            //(sender as Grid).UpdateLayout();            
            
        }
    }
}
