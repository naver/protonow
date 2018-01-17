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

namespace Naver.Compass.WidgetLibrary
{
    /// <summary>
    /// Interaction logic for EditDroplist.xaml
    /// </summary>
    public partial class EditListWindow : BaseWindow
    {
        /// <summary>
        /// select one widget, always load list 
        /// </summary>
        public EditListWindow(List<IWidget> widgets)
            : this(widgets, true)
        {

        }
        /// <summary>
        /// select several widgets, load list only when widgets has same items.
        /// </summary>
        public EditListWindow(List<IWidget>widgets,bool bLoad)
        {
            InitializeComponent();
            this.DataContext = new EditListViewModel(widgets, bLoad);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBox.Focus();
        }
    }
}
