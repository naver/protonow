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
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    /// <summary>
    /// Interaction logic for HorizontalLine.xaml
    /// </summary>
    public partial class HorizontalLineWidget : UserControl
    {
        public HorizontalLineWidget()
        {
            InitializeComponent();
        }
       
        //public ArrowStyle Arrow
        //{
        //    get { return (ArrowStyle)GetValue(ArrowProperty); }
        //    set { SetValue(ArrowProperty, value); }
        //}

        //public static readonly DependencyProperty ArrowProperty =
        //  DependencyProperty.Register("Arrow", typeof(ArrowStyle),
        //                              typeof(LineWidget),
        //                              new FrameworkPropertyMetadata(ArrowStyle.None));


        //private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (e.HeightChanged == true)
        //    {
        //        ActualWidgetHeight = AdaptiveBorder.ActualHeight + 10;
        //    }
        //}

        //#region Dependency Propery
        //public double ActualWidgetHeight
        //{
        //    get { return (double)GetValue(ActualWidgetHeightProperty); }
        //    set { SetValue(ActualWidgetHeightProperty, value); }
        //}

        //public static readonly DependencyProperty ActualWidgetHeightProperty =
        //  DependencyProperty.Register("ActualWidgetHeight", typeof(double),
        //                              typeof(HorizontalLineWidget),
        //                              new FrameworkPropertyMetadata(0.0));
        //#endregion

    }


}
