using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using Naver.Compass.Common.Helper;
using System.Collections.ObjectModel;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Module
{
    #region device
    public class Number2PercentTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            double percent = (double)value;
            if (percent <= 0)
                return string.Empty;

            string res = percent * 100 + "%";
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            double right = (double)value;

            Thickness rightspacing = new Thickness(0, 3, right, 3);
            return rightspacing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            int width = (int)value;
            return width;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EditableWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = (int)value;
            if (index == 0)
            {
                return " ";
            }
            else
                return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int iValue;
            bool res = int.TryParse(value.ToString(), out iValue);
            if (res)
            {
                if (iValue > CommonDefine.MaxEditorWidth)
                    return CommonDefine.MaxEditorWidth;
                else
                    return iValue;
            }
            else
            {
                return 0;
            }
        }
    }

    public class EditableHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = (int)value;
            if (index == 0)
            {
                return "";
            }
            else
                return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int iValue;
            bool res = int.TryParse(value.ToString(), out iValue);
            if (res)
            {
                if (iValue > CommonDefine.MaxEditorHeight)
                    return CommonDefine.MaxEditorHeight;
                else
                    return iValue;
            }
            else
            {
                return 0;
            }
        }
    }

    public class ImageVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(int.Parse(parameter.ToString()) == 1)
            {
                if ((bool)value == true)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else
            {
                if ((bool)value == true)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NameConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;
            if (value.ToString() == CommonDefine.PresetOff)
                return GlobalData.FindResource("Responsive_PresetOff");
            else if (value.ToString() == CommonDefine.UserSetting)
                return GlobalData.FindResource("Responsive_UserSetting");
            else return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.ToString() == GlobalData.FindResource("Responsive_PresetOff"))
                return CommonDefine.PresetOff;
            if (value.ToString() == GlobalData.FindResource("Responsive_UserSetting"))
                return CommonDefine.UserSetting;
            return value;
        }
    }
    #endregion

    #region adaptive

    public class ConditionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            AdaptiveViewCondition condition = (AdaptiveViewCondition)value;
            int param = int.Parse(parameter.ToString());
            switch(param)
            {
                case 0:
                    return (AdaptiveViewCondition.LessOrEqual == condition);
                case 1:
                    return (AdaptiveViewCondition.GreaterOrEqual == condition);
            }
            if ((AdaptiveViewCondition)value == AdaptiveViewCondition.LessOrEqual)
                return 0;
            else
                return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int param = int.Parse(parameter.ToString());

            switch (param)
            {
                case 0:
                    return value.Equals(true) ? AdaptiveViewCondition.LessOrEqual : Binding.DoNothing;
                case 1:
                    return value.Equals(true) ? AdaptiveViewCondition.GreaterOrEqual : Binding.DoNothing;
                default:
                    return Binding.DoNothing;
            }
        }
    }

    public class WidthSetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = (int)value;
            if (index == 0)
            {
                return string.Empty;
            }
            else
                return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int iValue;
            bool res = int.TryParse(value.ToString(), out iValue);
            if (res)
            {
                if (iValue > CommonDefine.MaxEditorWidth)
                    return CommonDefine.MaxEditorWidth;
                else
                    return iValue;
            }
            else
            {
                return 0;
            }
        }
    }

    public class HeightSetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = (int)value;
            if (index == 0)
            {
                return string.Empty;
            }
            else
                return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int iValue;
            bool res = int.TryParse(value.ToString(), out iValue);
            if (res)
            {
                if (iValue > CommonDefine.MaxEditorHeight)
                    return CommonDefine.MaxEditorHeight;
                else
                    return iValue;
            }
            else
            {
                return 0;
            }

        }
    }

    public class SpacingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            double left = (double)value;

            Thickness leftspacing = new Thickness(left, 0, 0, 0);
            return leftspacing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            Thickness leftspacing = (Thickness)value;
            return leftspacing.Left;
        }
    }

    public class ViewListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            ObservableCollection<AdaptivVieweNode> tree = (ObservableCollection<AdaptivVieweNode>)value;
            if (tree.Count <= 0)
                return null;

            ObservableCollection<AdaptivVieweNode> list = new ObservableCollection<AdaptivVieweNode>();
            ConvertTreeToList(tree, ref list);
            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// inorder to diaply the adaptive collection to the listbox in UI
        /// we have to convert adaptive tree to list.
        /// </summary>
        /// <param name="tree">adaptive view collection tree in VM</param>
        /// <param name="list">list to be diaplayed in listbox</param>
        private void ConvertTreeToList(ObservableCollection<AdaptivVieweNode> tree, ref ObservableCollection<AdaptivVieweNode> list)
        {
            foreach (var item in tree)
            {
                list.Add(item);
                ConvertTreeToList(item.Children, ref list);
            }

        }
    }


    /// <summary>
    /// Convert less/greater image according to Condition
    /// </summary>
    public class LessGreaterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            AdaptiveViewCondition condition = (AdaptiveViewCondition)value;
            int param = int.Parse(parameter.ToString());

            if (param == 0)
            {
                return (condition == AdaptiveViewCondition.LessOrEqual)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            else
            {
                return (condition == AdaptiveViewCondition.GreaterOrEqual)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// If show less/greater icon 
    /// </summary>
    public class IconVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            string name = value.ToString();

            if (name == "Base")
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
#endregion

}
