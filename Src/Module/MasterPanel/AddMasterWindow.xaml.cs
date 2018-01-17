using Naver.Compass.Common;
using Naver.Compass.Service.Document;
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
    /// AddMasterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddMasterWindow : BaseWindow
    {
        public AddMasterWindow(IMasterPage masterPage)
        {
            InitializeComponent();
            this.DataContext = new AddMasterViewModel(masterPage);
        }
    }
}
