using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Naver.Compass.Module
{
    class DeviceTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            DeviceType type = (DeviceType)value;
            int param = int.Parse(parameter.ToString());
            switch (param)
            {
                case 0:
                    return (DeviceType.PCWeb == type);
                case 1:
                    return (DeviceType.Mobile== type);
                case 2:
                    return (DeviceType.Tablet == type);
                case 3:
                    return (DeviceType.Watch == type);
                default:
                    return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int param = int.Parse(parameter.ToString());
            switch (param)
            {
                case 0:
                    return value.Equals(true) ? DeviceType.PCWeb : Binding.DoNothing;
                case 1:
                    return value.Equals(true) ? DeviceType.Mobile : Binding.DoNothing;
                case 2:
                    return value.Equals(true) ? DeviceType.Tablet : Binding.DoNothing;
                case 3:
                    return value.Equals(true) ? DeviceType.Watch : Binding.DoNothing;
                default:
                    return Binding.DoNothing;
            }
        }
    }

    class ViewTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            ViewType type = (ViewType)value;
            int param = int.Parse(parameter.ToString());
            switch (param)
            {
                case 0:
                    return (ViewType.Portait == type);
                case 1:
                    return (ViewType.Landscape == type);
                default:
                    return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int param = int.Parse(parameter.ToString());
            switch (param)
            {
                case 0:
                    return value.Equals(true) ? ViewType.Portait : Binding.DoNothing;
                case 1:
                    return value.Equals(true) ? ViewType.Landscape : Binding.DoNothing;
                default:
                    return Binding.DoNothing;
            }
        }
    }

    class ViewTypeEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            return true;

            DeviceNode node  = (DeviceNode)value;
            if (node.Type == DeviceType.None)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}
