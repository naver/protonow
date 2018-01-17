using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Naver.Compass.WidgetLibrary
{
    public class SizeInfoChrome : Control
    {
        static SizeInfoChrome()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SizeInfoChrome), new FrameworkPropertyMetadata(typeof(SizeInfoChrome)));
        }
    }

    public class DoubleFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value;
            return Math.Round(d);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
