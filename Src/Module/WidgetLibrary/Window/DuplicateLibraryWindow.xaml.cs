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
    /// Interaction logic for DuplicateLibraryWindow.xaml
    /// </summary>
    public partial class DuplicateLibraryWindow : BaseWindow
    {
        public bool? Result { get; set; }

        public DuplicateLibraryWindow()
        {
            InitializeComponent();
        }

        private void Upgrade_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            this.Close();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            this.Close();
        }
    }
}
