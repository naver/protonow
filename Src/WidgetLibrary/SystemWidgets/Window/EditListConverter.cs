using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Naver.Compass.WidgetLibrary
{
    public class TitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value!=null)
            {
                WidgetType type = (WidgetType)value;
                if (type == WidgetType.ListBox)
                    return GlobalData.FindResource("EditListBox_Title");
                if (type == WidgetType.DropList)
                    return GlobalData.FindResource("EditDropList_Title");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultipleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                WidgetType type = (WidgetType)value;
                if (type == WidgetType.ListBox)
                    return Visibility.Visible;
                if (type == WidgetType.DropList)
                    return Visibility.Collapsed;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
