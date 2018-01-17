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

namespace Naver.Compass.Main
{
    /// <summary>
    /// WelcomeScreen.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomeScreen : Window
    {
        public WelcomeScreen()
        {
            InitializeComponent();
            this.DataContext = new WelcomeScreenViewModel(this);
        }

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                if (e.ClickCount == 1)
                {
                    this.DragMove();
                }
        }

        /// <summary>
        /// Hide window when try to create a project.
        /// Use Visibility.Collapse will cause SetDeviceWindow be not Modal. So add this function.
        /// </summary>
        public void HideDialog()
        {
            this.SizeToContent = SizeToContent.Manual;
            this.Height = 0;
            this.Width = 0;
        }

        public void UnHideDialog()
        {
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

    }
}
