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
using Microsoft.Windows.Controls.Ribbon;
using System.Windows.Interop;
using Microsoft.Windows.Shell;
using Naver.Compass.Common;
using Naver.Compass.Common.CommonBase;
using System.Windows.Threading;
using System.Threading;
using Naver.Compass.Module.Model;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Update;
using Naver.Compass.Service;
using Naver.Compass.Common.Win32;


namespace Naver.Compass.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainIntegrationWindow : MainBase
    {

        public MainIntegrationWindow()
        {
            InitializeComponent();

            this.DataContext = new MainIntegrationViewModel(this);
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            (sender as MediaElement).Position = (sender as MediaElement).Position.Add(TimeSpan.FromMilliseconds(1));       
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Home || e.Key == Key.End)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift
                  && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    MainIntegrationViewModel VM = this.DataContext as MainIntegrationViewModel;
                    if (VM != null)
                    {
                        VM.OnOpenSitemapPage(e.Key == Key.Home);
                    }
                }
            }
        }
        
    }
}

