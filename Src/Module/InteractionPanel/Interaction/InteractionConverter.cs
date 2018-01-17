using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Naver.Compass.Module
{
    class ShowHideTypeConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            VisibilityType type = (VisibilityType)value;
            int param = int.Parse(parameter.ToString());
            switch (param)
            {
                case 0:
                    return (VisibilityType.None == type);
                case 1:
                    return (VisibilityType.Show == type);
                case 2:
                    return (VisibilityType.Hide == type);
                case 3:
                    return (VisibilityType.Toggle == type);
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
                    return value.Equals(true) ? VisibilityType.None:Binding.DoNothing;
                case 1:
                    return value.Equals(true) ? VisibilityType.Show : Binding.DoNothing;
                case 2:
                    return value.Equals(true) ? VisibilityType.Hide : Binding.DoNothing;
                case 3:
                    return value.Equals(true) ? VisibilityType.Toggle : Binding.DoNothing;
                default:
                    return Binding.DoNothing;
            }
        }
    }

    class AnimateTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            ShowHideAnimateType type = (ShowHideAnimateType)value;
            int param = int.Parse(parameter.ToString());
            switch (param)
            {
                case 0:
                    return (ShowHideAnimateType.None == type);
                case 1:
                    return (ShowHideAnimateType.Fade == type);
                case 2:
                    return (ShowHideAnimateType.SlideRight == type);
                case 3:
                    return (ShowHideAnimateType.SlideLeft == type);
                case 4:
                    return (ShowHideAnimateType.SlideUp == type);
                case 5:
                    return (ShowHideAnimateType.SlideDown == type);
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
                    return value.Equals(true) ? ShowHideAnimateType.None : Binding.DoNothing;
                case 1:
                    return value.Equals(true) ? ShowHideAnimateType.Fade : Binding.DoNothing;
                case 2:
                    return value.Equals(true) ? ShowHideAnimateType.SlideRight : Binding.DoNothing;
                case 3:
                    return value.Equals(true) ? ShowHideAnimateType.SlideLeft : Binding.DoNothing;
                case 4:
                    return value.Equals(true) ? ShowHideAnimateType.SlideUp : Binding.DoNothing;
                case 5:
                    return value.Equals(true) ? ShowHideAnimateType.SlideDown : Binding.DoNothing;
                default:
                    return Binding.DoNothing;
            }
        }
    }

    class AnimateTimeConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            int time = int.Parse(value.ToString());
            int param = int.Parse(parameter.ToString());
            switch (param)
            {
                case 0:
                    return (time == 1500);
                case 1:
                    return (time == 500);
                case 2:
                    return (time == 100);
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
                    return value.Equals(true) ? 1500 : Binding.DoNothing;
                case 1:
                    return value.Equals(true) ? 500 : Binding.DoNothing;
                case 2:
                    return value.Equals(true) ? 100 : Binding.DoNothing;
                default:
                    return Binding.DoNothing;
            }
        }
    }
}
