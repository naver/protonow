using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Naver.Compass.Module
{
    public class LockEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Int2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int iValue = (int)value;
            if (iValue <= 0)
            {
                iValue = 0;
            }
            return iValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int iValue;
            bool res = int.TryParse(value.ToString(), out iValue);
            if (res)
            {
                return iValue;
            }
            else
            {
                return 0;
            }
        }
    }
}
