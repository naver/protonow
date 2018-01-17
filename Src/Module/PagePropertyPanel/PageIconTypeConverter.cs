using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Naver.Compass.Module
{

    public class PageIconTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (int.Parse(parameter.ToString()) == 1)
            {
                return ((bool)value == true);
            }
            else
            {
                return ((bool)value == false);
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (int.Parse(parameter.ToString()) == 1)
            {
                return ((bool)value == true);
            }
            else
            {
                return ((bool)value == false);
            }
        }
    }
}
