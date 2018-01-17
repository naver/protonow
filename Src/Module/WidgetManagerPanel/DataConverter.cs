using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Naver.Compass.Common.Helper;
namespace Naver.Compass.Module
{

    public enum ListItemType
    {
        defaultItem,
        PageItem,
        MasterItem,
        DynamicPanelItem,
        DynamicPanelStateItem,
        ToastItem,
        MenuItem,
        GroupItem,
        GroupChildItem

    }

    class LibraryItemCollapseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;
            var flag = (bool)value;
            return flag ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class LibraryItemMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var libraryItem = (WidgetListItem)value;
            if (libraryItem.Lavel > 0)
            {
                return new Thickness(16 + (libraryItem.Lavel-1) * 10, 1, 0, 1);
            }
            else
            {
                return new Thickness(libraryItem.Lavel * 1, 1, 0, 1);//error data
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InforTooltipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null
                || values.Length != 2
                || !(values[0] is bool)
                || !(values[1] is bool))
            {
                return "error string";
            }

            if ((bool)values[0] )
            {
                return GlobalData.FindResource("ObjectListManager_Info_Remove");
            }
            else if ((bool)values[1])
            {
                return GlobalData.FindResource("ObjectListManager_Info_Resolution");
            }

            return "error string";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class HasChildreenVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;
            var hasChildren = (bool)value;
            return hasChildren ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class InfoMarkVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var libraryItem = (WidgetListItem)value;

            if (libraryItem == null)
                return Visibility.Hidden;

            if (libraryItem.LostFlag || libraryItem.PlaceFlag)
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CenterToolTipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.FirstOrDefault(v => v == DependencyProperty.UnsetValue) != null)
            {
                return double.NaN;
            }
            double placementTargetWidth = (double)values[0];
            double toolTipWidth = (double)values[1];
            return (placementTargetWidth / 2.0) - (toolTipWidth / 2.0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class RootTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                ListItemType Type = (ListItemType)value;

                switch (Type)
                {
                    case ListItemType.DynamicPanelItem:
                        return "Resources/icon_swipeViews.png";
                    case ListItemType.MenuItem:
                        return "Resources/icon_drawer.png";
                    case ListItemType.ToastItem:
                        return "Resources/icon_drawer.png";
                    case ListItemType.MasterItem:
                        return "Resources/icon_master.png";
                    case ListItemType.PageItem:
                        return "Resources/icon-15-page.png";
                    default:
                        return "Resources/icon-15-page.png";
                }
            }

            return "Resources/icon-15-page.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
