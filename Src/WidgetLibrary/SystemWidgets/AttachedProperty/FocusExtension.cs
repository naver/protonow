using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace Naver.Compass.WidgetLibrary
{
    public static class FocusExtension
    {
        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }


        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }


        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
             "IsFocused", typeof(bool), typeof(FocusExtension),
             new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));


        private static void OnIsFocusedPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {

            BaseEditControl uie = d as BaseEditControl;
            if (uie == null)
            {
                Debug.WriteLine("###----->Error:text edit box is invalid\n");
                return;
            }
            if ((bool)e.NewValue)
            {

                var action = new Action(() =>
                    d.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        uie.IsInEditMode = (bool)e.NewValue;
                    })));
                Task.Factory.StartNew(action);
            }
        }
    }

}
