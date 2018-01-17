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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Naver.Compass.Differ
{
    /// <summary>
    /// Interaction logic for DifferView.xaml
    /// </summary>
    public partial class DifferView : Canvas
    {
        public DifferView()
        {
            InitializeComponent();
            Loaded += DifferView_Loaded;
        }

        private void DifferView_Loaded(object sender, RoutedEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 3)
            {
                string file1 = args[1];
                string file2 = args[2];

                DifferViewModel vm = this.DataContext as DifferViewModel;
                if(vm!=null)
                {
                    vm.InitializeSetting(file1,file2);
                }
            }
        }


    }
}
