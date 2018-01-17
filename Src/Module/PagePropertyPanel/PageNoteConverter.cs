using Naver.Compass.Common.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Naver.Compass.Module
{
    //Default not field converter
    public class DefaultFieldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            string defaultva = GlobalData.FindResource("PageNote_Default");

            if (value.ToString() == "Default")
                return defaultva;
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

            if (value.ToString() == GlobalData.FindResource("PageNote_Default"))
                return "Default";
            return value;
        }
    }
}
