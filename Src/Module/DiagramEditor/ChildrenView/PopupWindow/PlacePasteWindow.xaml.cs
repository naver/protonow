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
    /// Interaction logic for ToolTipWindow.xaml
    /// </summary>
    public partial class PlacePasteWindow : BaseWindow
    {
        public PlacePasteWindow()
        {
            InitializeComponent();
            this.Closing += PlacePasteWindow_Closing;
            
        }

        void PlacePasteWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == null)
            {
                this.DialogResult = true;
            }
        }
        
        private void Place_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }




    }
}
