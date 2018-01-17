using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Naver.Compass.Module
{
    public class VerticalSliceLineVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null
                || values.Length != 2
                || !(values[0] is bool)
                || !(values[1] is SliceType))
            {
                return Visibility.Collapsed;
            }

            if ((bool)values[0] && (SliceType)values[1] != SliceType.Horizontal)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HorizontalSliceLineVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null
                || values.Length != 2
                || !(values[0] is bool)
                || !(values[1] is SliceType))
            {
                return Visibility.Collapsed;
            }

            if ((bool)values[0] && (SliceType)values[1] != SliceType.Vertical)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
