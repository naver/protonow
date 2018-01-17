using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using Naver.Compass.Service.Document;
using System.Windows.Controls;

namespace Naver.Compass.Common.CommonBase
{
    [ValueConversion(typeof(bool), typeof(String))]
    public class BoldDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = (bool)value;
            if (bValue)
            {
                return FontWeights.Bold;
            }
            else
            {
                return FontWeights.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (FontWeights.Bold.Equals(value))
            {
                return true;
            }

             return false;
         }
     }

    [ValueConversion(typeof(Alignment), typeof(TextAlignment))]
    public class DocmentAlignDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Alignment)value)
            {
                case Alignment.Left:
                    return TextAlignment.Left;
                case Alignment.Right:
                    return TextAlignment.Right;
                case Alignment.Center:
                    return TextAlignment.Center;
                default:
                    return TextAlignment.Left;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((TextAlignment)value)
            {
                case TextAlignment.Left:
                    return Alignment.Left;
                case TextAlignment.Right:
                    return Alignment.Right;
                case TextAlignment.Center:
                    return Alignment.Center;
                default:
                    return Alignment.Left;
            }
        }
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class AlignDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0:
                    return "Left";
                case 1:
                    return "Center";
                case 2:
                    return "Right";
                case 3:
                    return "Top";
                case 4:
                    return "Bottom";

                default:
                    return "Center";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value.ToString();
            if (strValue == null)
            {
                return "Center";
            }
            switch (strValue)
            {
                case "Left":
                    return 0;
                case "Center":
                    return 1;
                case "Right":
                    return 2;
                case "Top":
                    return 3;
                case "Bottom":
                    return 4;
                default:
                    return 1;
            }
        }
    }

    [ValueConversion(typeof(Alignment), typeof(VerticalAlignment))]
    public class VerticalAlignDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Alignment)value)
            {
                case Alignment.Top:
                    return VerticalAlignment.Top;
                case Alignment.Center:
                    return VerticalAlignment.Center;
                case Alignment.Bottom:
                    return VerticalAlignment.Bottom;
                default:
                    return VerticalAlignment.Center;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((VerticalAlignment)value)
            {
                case VerticalAlignment.Top:
                    return Alignment.Top;
                case VerticalAlignment.Center:
                    return Alignment.Center;
                case VerticalAlignment.Bottom:
                    return Alignment.Bottom;
                default:
                    return Alignment.Center;
            }
        }
    }

    [ValueConversion(typeof(Dictionary<string, bool>), typeof(TextDecorationCollection))]
    public class DecorationsDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TextDecorationCollection rValue = new TextDecorationCollection();
            try
            {
               
                Dictionary<string, bool> data = (Dictionary<string, bool>)value;
                if (data.ContainsKey("underline") && data["underline"])
                {
                    rValue.Add(CommonFunction.GetUnderline());
                }

                if (data.ContainsKey("strikethrough") && data["strikethrough"])
                {
                    rValue.Add(CommonFunction.GetStrikeThough());
                }
            }
            catch (System.Exception ex)
            {
                NLogger.Error("Decoration error!");
            }
            

            return rValue;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }

    [ValueConversion(typeof(bool), typeof(TextDecorationCollection))]
    public class UnderlineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((bool)value)
            {
                return CommonFunction.GetUnderline();
            }
            else
            {
                return new TextDecorationCollection();
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }

    [ValueConversion(typeof(bool), typeof(String))]
    public class ItalicDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = (bool)value;
            if (bValue)
            {
                return FontStyles.Italic;
            }
            else
            {
                return FontStyles.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (FontStyles.Italic.Equals(value))
            {
                return true;
            }

            return false;
        }
    }

    /*[ValueConversion(typeof(Color), typeof(String))]
    public class ColorDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //string szColor = ColorTranslator.ToHtml((Color)value);
            //int iColor = ColorTranslator.ToWin32((Color)value);
            Color cColor = (Color)value;
            string TragetColor = string.Format(@"#{0:X2}{1:X2}{2:X2}{3:X2}",
                cColor.A, cColor.R, cColor.G, cColor.B);
            //System.Convert.ToString(cColor.A, 16)
            return TragetColor;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color tarColor = ColorTranslator.FromHtml(value.ToString());
            return tarColor;


        }
    }*/

    public class ColorDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Color)
            {
                Color cColor = (Color)value;
                string TragetColor = string.Format(@"#{0:X2}{1:X2}{2:X2}{3:X2}",
                    cColor.A, cColor.R, cColor.G, cColor.B);
                return TragetColor;
            }
            if (value is StyleColor)
            {
                var sc = (StyleColor)value;
                if (sc.FillType == ColorFillType.Solid)
                {
                    return new System.Windows.Media.SolidColorBrush(this.FromArgb(sc.ARGB));
                }
                else if (sc.FillType == ColorFillType.Gradient)
                {
                    var lineb = new System.Windows.Media.LinearGradientBrush();
                    lineb.StartPoint = new System.Windows.Point(0d, 0.5d);
                    lineb.EndPoint = new System.Windows.Point(1d, 0.5d);
                    var newFrames = sc.Frames;
                    if (newFrames == null || newFrames.Count == 0)
                    {
                        newFrames = new Dictionary<double, int>();
                        newFrames[0] = -1;
                        newFrames[1] = -16777216;
                    }
                    foreach (var keypair in newFrames)
                    {
                        var gradientStop = new System.Windows.Media.GradientStop(
                            this.FromArgb(keypair.Value),
                            keypair.Key);
                        lineb.GradientStops.Add(gradientStop);
                    }

                    var aRotateTransform = new System.Windows.Media.RotateTransform();
                    aRotateTransform.CenterX = 0.5;
                    aRotateTransform.CenterY = 0.5;
                    aRotateTransform.Angle = sc.Angle;
                    lineb.RelativeTransform = aRotateTransform;
                    return lineb;
                }
            }
          

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private System.Windows.Media.Color FromArgb(int argb)
        {
            var drawc = System.Drawing.Color.FromArgb(argb);
            return System.Windows.Media.Color.FromArgb(
                    drawc.A,
                    drawc.R,
                    drawc.G,
                    drawc.B);
        }
    }

     [ValueConversion(typeof(object), typeof(object))]
    public class BorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color)
            {
                Color cColor = (Color)value;
                string TragetColor = string.Format(@"#{0:X2}{1:X2}{2:X2}{3:X2}",
                    cColor.A, cColor.R, cColor.G, cColor.B);
                return TragetColor;
            }
            if (value is StyleColor)
            {
                var sc = (StyleColor)value;
                if (sc.FillType == ColorFillType.Solid)
                {
                    return new System.Windows.Media.SolidColorBrush(this.FromArgb(sc.ARGB));
                }
                else if (sc.FillType == ColorFillType.Gradient)
                {
                    var lineb = new System.Windows.Media.LinearGradientBrush();
                    lineb.StartPoint = new System.Windows.Point(0d, 0.5d);
                    lineb.EndPoint = new System.Windows.Point(1d, 0.5d);
                    var newFrames = sc.Frames;
                    if (newFrames == null || newFrames.Count == 0)
                    {
                        newFrames = new Dictionary<double, int>();
                        newFrames[0] = -1;
                        newFrames[1] = -16777216;
                    }
                    foreach (var keypair in newFrames)
                    {
                        var gradientStop = new System.Windows.Media.GradientStop(
                            this.FromArgb(keypair.Value),
                            keypair.Key);
                        lineb.GradientStops.Add(gradientStop);
                    }

                    var aRotateTransform = new System.Windows.Media.RotateTransform();
                    aRotateTransform.CenterX = 0.5;
                    aRotateTransform.CenterY = 0.5;
                    aRotateTransform.Angle = sc.Angle;
                    lineb.RelativeTransform = aRotateTransform;
                    return lineb;
                }
            }

            else
            {
                return new System.Windows.Media.SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            }
           

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private System.Windows.Media.Color FromArgb(int argb)
        {
            var drawc = System.Drawing.Color.FromArgb(argb);
            return System.Windows.Media.Color.FromArgb(
                    drawc.A,
                    drawc.R,
                    drawc.G,
                    drawc.B);
        }
    }

    [ValueConversion(typeof(int), typeof(String))]
    public class StrokeDashArrayDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sReturnValue = "";
            int nStrokeType = (int)value;
            if (nStrokeType == 8)//dot
            {
                sReturnValue = "1,1,1,1";
            }
            else if (nStrokeType == 5)//dashDot
            {
                sReturnValue = "3,3,3,3";
            }
            else if (nStrokeType == 1)//solid
            {
                sReturnValue = "";
            }
            else if (nStrokeType == 6)//DashDotDot
            {
                sReturnValue = "1,3,3,1";
            }
            else
            {
                sReturnValue = "";
            }
            return sReturnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int iReturnValue = 0;
            string sReturnValue = value as string;
            switch (sReturnValue)
            {
                case "{1,1,1,1}":
                    iReturnValue = 8;
                    break;
                case "{3,3,3,3}":
                    iReturnValue = 5;
                    break;
                case "{1,3,1,3}":
                    iReturnValue = 6;
                    break;
                default:
                    iReturnValue = 1;
                    break;

            }

            return iReturnValue;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class Bool2VisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = (bool)value;
            if (bValue)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility strValue = (Visibility)value;
            if (strValue == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class Bool2CollapseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = (bool)value;
            if (bValue)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility strValue = (Visibility)value;
            if (strValue == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class Boolreverse2CollapseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = (bool)value;
            if (bValue)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility strValue = (Visibility)value;
            if (strValue == Visibility.Visible)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class MultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SliderWidthConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double w = (double)values[0];
            double h = (double)values[1];
            double width = Math.Min(w, h) / 2;
            return Math.Max(0, width);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class DisableAttributeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = (bool)value;
            return !bValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = (bool)value;
            return !bValue;
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           return !(bool)value;
        }
    }

    [ValueConversion(typeof(double), typeof(double))]
    public class HalfValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double bValue = (double)value;
            return bValue * 0.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(double), typeof(RotateTransform))]
    public class RotateTransConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double bValue = System.Convert.ToDouble(value);
            return  new RotateTransform(bValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(TabItem), typeof(int))]
    public class TabIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TabItem tabItem = value as TabItem;
            return ItemsControl.ItemsControlFromItemContainer(tabItem)
                .ItemContainerGenerator.IndexFromContainer(tabItem);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
