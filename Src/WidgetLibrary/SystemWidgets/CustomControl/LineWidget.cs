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
    public class LineWidget : UserControl
    {

        public ArrowStyle Arrow
        {
            get { return (ArrowStyle)GetValue(ArrowProperty); }
            set { SetValue(ArrowProperty, value); }
        }

        public static readonly DependencyProperty ArrowProperty =
          DependencyProperty.Register("Arrow", typeof(ArrowStyle),
                                      typeof(LineWidget),
                                      new FrameworkPropertyMetadata(ArrowStyle.None));
    }
}
