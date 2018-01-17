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

namespace Naver.Compass.WidgetLibrary
{
    /// <summary>
    /// Interaction logic for RectangleWidget.xaml
    /// </summary>
    public partial class RadioButtonWidget : UserControl
    {
        public RadioButtonWidget()
        {
            InitializeComponent();
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.HeightChanged==true)
            {
                ActualWidgetHeight = AdaptiveBorder.ActualHeight;
            }            
        }

        #region Dependency Propery
        public double ActualWidgetHeight
        {
            get { return (double)GetValue(ActualWidgetHeightProperty); }
            set { SetValue(ActualWidgetHeightProperty, value); }
        }

        public static readonly DependencyProperty ActualWidgetHeightProperty =
          DependencyProperty.Register("ActualWidgetHeight", typeof(double),
                                      typeof(RadioButtonWidget),
                                      new FrameworkPropertyMetadata(0.0));
        #endregion
    }
}
