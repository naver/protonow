using Naver.Compass.Common.CommonBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Service.Update;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Naver.Compass.Common.Win32;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service;

namespace Naver.Compass.Main
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        private MainIntegrationWindow _mainWindow;

        private DispatcherTimer _splashTimer;

        private static double _percent = 0.3;
        public SplashWindow()
        {
            InitializeComponent();

            if (NetEnvironmentCheck.IsValid())
            {
                _splashTimer = new DispatcherTimer();
                _splashTimer.Interval = TimeSpan.FromMilliseconds(400);
                _splashTimer.Tick += new EventHandler(splashTimer_Tick);
                _splashTimer.Start();
                _mainWindow = new MainIntegrationWindow();

            }
            else
            {
//                var warningContent = @"Before running protoNow, you have to install KB2468871 on your machine.
//The download page will be opened automatically after you clicking ok button.";
                MessageBox.Show(GlobalData.FindResource("Warn_FramePath_Info"), GlobalData.FindResource("Common_Title"));

                try
                {
                    System.Diagnostics.Process.Start("http://www.microsoft.com/en-us/download/details.aspx?id=3556");
                }
                catch (Exception exp)
                {
                    NLogger.Warn("Open URL error : "+exp.Message);
                }

                Application.Current.Shutdown();
            }
        }

        void splashTimer_Tick(object sender, EventArgs e)
        {
            //show progress first
            if (_percent < 1)
            {
                proBar.Value = _percent;
                _percent += 0.2;
                
            }
            else//show main widnow
            {
                if (_mainWindow.Visibility == Visibility.Collapsed)
                {
                    _mainWindow.Show();
                    _splashTimer.Stop(); 
                }
            }
        }
    }

}
