using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Naver.Compass.Module
{
    public class CloseSettingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            ToastCloseSetting closeSetting = (ToastCloseSetting)value;
            int param = int.Parse(parameter.ToString());
            switch (param)
            {
                case 0:
                    return (ToastCloseSetting.ExposureTime == closeSetting);
                case 1:
                    return (ToastCloseSetting.CloseButton == closeSetting);
                case 2:
                    return (ToastCloseSetting.AreaTouch == closeSetting);
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
                    return value.Equals(true) ?ToastCloseSetting.ExposureTime : Binding.DoNothing;
                case 1:
                    return value.Equals(true) ? ToastCloseSetting.CloseButton : Binding.DoNothing;
                case 2:
                    return value.Equals(true) ? ToastCloseSetting.AreaTouch : Binding.DoNothing;
                default:
                    return Binding.DoNothing;
            }
        }
    }

    public class DisplayPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            ToastDisplayPosition displayPosition = (ToastDisplayPosition)value;
            int param = int.Parse(parameter.ToString());
            switch (param)
            {
                case 0:
                    return (ToastDisplayPosition.UserSetting == displayPosition);
                case 1:
                    return (ToastDisplayPosition.Top == displayPosition);
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
                    return value.Equals(true) ? ToastDisplayPosition.UserSetting : Binding.DoNothing;
                case 1:
                    return value.Equals(true) ? ToastDisplayPosition.Top : Binding.DoNothing;
                default:
                    return Binding.DoNothing;
            }
        }
    }

    /// <summary>
    /// Swipe Views: View mode
    /// </summary>
    public class ViewModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            DynamicPanelViewMode type = (DynamicPanelViewMode)value;
            int param = int.Parse(parameter.ToString());
            switch (param)
            {
                case 0:
                    return (DynamicPanelViewMode.Full == type);
                case 1:
                    return (DynamicPanelViewMode.Card == type);
                case 2:
                    return (DynamicPanelViewMode.Preview == type);
                case 3:
                    return (DynamicPanelViewMode.Scroll == type);
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
                    return value.Equals(true) ? DynamicPanelViewMode.Full : Binding.DoNothing;
                case 1:
                    return value.Equals(true) ? DynamicPanelViewMode.Card : Binding.DoNothing;
                case 2:
                    return value.Equals(true) ? DynamicPanelViewMode.Preview : Binding.DoNothing;
                case 3:
                    return value.Equals(true) ? DynamicPanelViewMode.Scroll : Binding.DoNothing;
                default:
                    return Binding.DoNothing;
            }
        }
    }

}
