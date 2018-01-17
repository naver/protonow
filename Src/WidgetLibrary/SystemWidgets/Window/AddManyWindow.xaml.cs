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

namespace Naver.Compass.WidgetLibrary
{
    /// <summary>
    /// Interaction logic for AddManyWindow.xaml
    /// </summary>
    public partial class AddManyWindow : BaseWindow
    {
        public AddManyWindow()
        {
            InitializeComponent();
            this.DataContext = new AddManyViewModel();
        }
    }
}
