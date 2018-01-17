using System;
using System.Collections.Generic;
using System.IO;
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

namespace Naver.Compass.Module.PreviewModule
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : UserControl
    {
        public PreviewWindow()
        {
            InitializeComponent();
            this.DataContext = new PagePreViewModel();
        }

        private void PreCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            PreviewPageCanvas preCanvas = sender as PreviewPageCanvas;
            PagePreViewModel context = this.DataContext as PagePreViewModel;
            context.PreBorder = PreBorder;
            context.PreCanvas = preCanvas;
        }

    }
}
