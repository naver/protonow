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
    /// Interaction logic for GridGuideSetting.xaml
    /// </summary>
    public partial class GridGuideSetting : BaseWindow
    {
        public GridGuideSetting(SettingType type)
        {
            InitializeComponent();
            this.DataContext = new GridGuideSettingViewModel(type);
        }
    }
}
