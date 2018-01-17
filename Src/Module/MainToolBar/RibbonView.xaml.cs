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
using Microsoft.Practices.Prism.Events;
using MainToolBar.ViewModel;

namespace MainToolBar
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    
    public partial class RibbonView : UserControl
    {
        public RibbonView()
        {
            InitializeComponent();
            this.DataContext = new RibbonViewModel();
           
        }

        private void ReUpdateFontsize()
        {
            if (DataContext is RibbonViewModel)
            {
                ((RibbonViewModel)DataContext).ResetFontsizeValue();
            }

        }

        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox control = sender as ComboBox;
            try
            {
                if (control != null && e.Key == Key.Enter)
                {
                    double size = Convert.ToDouble(double.Parse(control.Text).ToString("0"));

                    if (size >= 8 && size < 1001)
                    {
                        if (Naver.Compass.Common.CommonBase.FontCommands.Size.CanExecute(size, null))
                        {
                            Naver.Compass.Common.CommonBase.FontCommands.Size.Execute(size, null);
                        }
                    }
                    else
                    {
                        ReUpdateFontsize();
                        control.Focus();
                    }
                }
            }
            catch (System.Exception ex)
            {
                ReUpdateFontsize();
                control.Focus();
            }
        }

    }
}
