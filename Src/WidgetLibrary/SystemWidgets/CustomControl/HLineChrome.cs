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
    public class HLineChrome : BaseChrome
    {
        static HLineChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HLineChrome), new FrameworkPropertyMetadata(typeof(HLineChrome)));
        }
    }
}
