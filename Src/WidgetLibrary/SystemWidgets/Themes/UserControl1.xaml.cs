using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Naver.Compass.WidgetLibrary
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Grid
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void OnClick(object sender, RoutedEventArgs args)
        {
            CheckBox selectionCheckBox = sender as CheckBox;
            if (selectionCheckBox != null && selectionCheckBox.IsChecked == true)
            {
                foreach (Control child in DesignerCanvas.Children)
                {
                    Selector.SetIsSelected(child, true);
                }
            }
            else
            {
                foreach (Control child in DesignerCanvas.Children)
                {
                    Selector.SetIsSelected(child, false);
                }
            }
        }
    }
}
