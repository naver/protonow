using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Converters
{
    public class LayoutDocumentPaneControSelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var svalue = string.Empty;
            if (value is LayoutDocument)
            {
                var layoutdoc = value as LayoutDocument;
                if (layoutdoc.Content != null)
                {
                    svalue = string.Format("{0}", layoutdoc.Content);
                }
            }

            return svalue.Equals("Naver.Compass.Module.DynamicPageEditorViewModel", StringComparison.OrdinalIgnoreCase) ? 62 : 44;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LayoutDocumentPaneControSelectedItemMarginLeftConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var svalue = string.Empty;
            if (value is LayoutDocument)
            {
                var layoutdoc = value as LayoutDocument;
                if (layoutdoc.Content != null)
                {
                    svalue = string.Format("{0}", layoutdoc.Content);
                }
            }

            return svalue.Equals("Naver.Compass.Module.DynamicPageEditorViewModel", StringComparison.OrdinalIgnoreCase)
                ? new System.Windows.Thickness(1, 0, 0, -36)
                : new System.Windows.Thickness(1, 0, 0, -18);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LayoutDocumentPaneControSelectedItemMarginRightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var svalue = string.Empty;
            if (value is LayoutDocument)
            {
                var layoutdoc = value as LayoutDocument;
                if (layoutdoc.Content != null)
                {
                    svalue = string.Format("{0}", layoutdoc.Content);
                }
            }//"Naver.Compass.Module.MasterPageEditorViewModel"

            return svalue.Equals("Naver.Compass.Module.DynamicPageEditorViewModel", StringComparison.OrdinalIgnoreCase)
                ? new System.Windows.Thickness(0, 0, 0, -36)
                : new System.Windows.Thickness(0, 0, 0, -18);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LayoutDocumentPaneControSelectedItemHeaderColorConverter:IValueConverter
    {
        private ResourceDictionary _resourceDictionary;
        public ResourceDictionary ResourceDictionary
        {
            get { return _resourceDictionary; }
            set
            {
                _resourceDictionary = value;
            }
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var svalue = string.Empty;
            if (value is LayoutDocument)
            {
                var layoutdoc = value as LayoutDocument;
                if (layoutdoc.Content != null)
                {
                    svalue = string.Format("{0}", layoutdoc.Content);
                }
            }

            return svalue.Equals("Naver.Compass.Module.MasterPageEditorViewModel", StringComparison.OrdinalIgnoreCase)
                ? _resourceDictionary["MasterDocumentHeaderColor"]
                : _resourceDictionary["CommonDocumentHeaderColor"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LayoutDocumentTypeHeaderHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            var svalue = string.Empty;
            if (value is LayoutDocument)
            {
                var layoutdoc = value as LayoutDocument;
                if (layoutdoc.Content != null)
                {
                    svalue = string.Format("{0}", layoutdoc.Content);
                }
            }

            return svalue.Equals("DockingLayout.NoPageViewModel", StringComparison.OrdinalIgnoreCase)
                ? new GridLength(0)
                : new GridLength(26);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LayoutDocumentTypeHeaderButtonVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var svalue = string.Empty;
            if (value is LayoutDocument)
            {
                var layoutdoc = value as LayoutDocument;
                if (layoutdoc.Content != null)
                {
                    svalue = string.Format("{0}", layoutdoc.Content);
                }
            }
            return svalue.Equals("DockingLayout.NoPageViewModel", StringComparison.OrdinalIgnoreCase)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LayoutDocumentTypeHeaderNewButtonMaxWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is double) || !(values[1] is LayoutDocumentPaneControl))
            {
                return 0d;
            }

            var panelwidth = (double)values[0];
            if (panelwidth == double.NaN || panelwidth == 0d)
            {
                return 0d;
            }

            var panelControl = values[1] as Xceed.Wpf.AvalonDock.Controls.LayoutDocumentPaneControl;
            return panelwidth - 18d * 3;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
