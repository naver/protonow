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
using Naver.Compass.InfoStructure;
using Naver.Compass.Common;

namespace Naver.Compass.Module
{
    /// <summary>
    /// Interaction logic for LanguageSetting.xaml
    /// </summary>
    public partial class LanguageSettingWindow : BaseWindow
    {
        public LanguageSettingWindow()
        {
            InitializeComponent();
            this.DataContext = new LanguageSettingViewModel(this);  
        }
    }
}
