using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Naver.Compass.Module
{
    public class ViewRectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length != 4)
            {
                return default(Rect);
            }

            foreach (var value in values)
            {
                if(!(value is double))
                {
                    return default(Rect);
                }
            }

            return new Rect((double)values[0], (double)values[1], (double)values[2], (double)values[3]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
