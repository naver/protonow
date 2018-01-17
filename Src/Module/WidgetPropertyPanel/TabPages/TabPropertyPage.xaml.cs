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
    /// Interaction logic for TabPropertyPage.xaml
    /// </summary>
    public partial class TabPropertyPage : UserControl
    {
        public TabPropertyPage()
        {
            InitializeComponent();
            this.DataContext = new PropertyPageViewModel();
        }

        private void EnterTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = false;
        }

        private void EnterTextbox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            e.Handled = false;
            TextBox b = sender as TextBox;
            BindingExpression be = b.GetBindingExpression(TextBox.TextProperty);
            //be.UpdateTarget();
            be.UpdateSource();
            bool a1 = b.IsFocused;
            bool a2 = b.IsKeyboardFocused;
            bool a3 = Application.Current.MainWindow.IsActive;
        }

        private void OnFocused(object sender, MouseButtonEventArgs e)
        {
            //Focus change will cause the data update
            Border frame = (sender as Border);
            if (frame != null)
            {
                frame.Focus();
            }  
        }
    }
}
