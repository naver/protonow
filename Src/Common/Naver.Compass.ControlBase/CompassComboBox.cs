using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Naver.Compass.Common
{
    public class CompassComboBox : ComboBox
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {

            base.OnLostKeyboardFocus(e);
            if (e.NewFocus == null)
            {
                return;
            }

            var InnerParent = VisualTreeHelper.GetParent(e.NewFocus as FrameworkElement);
            //DependencyObject InnerParent = e.NewFocus as FrameworkElement; 
            while (InnerParent != null)
            {
                if (InnerParent == this)
                {
                    break;
                }
                //InnerParent = (InnerParent as FrameworkElement).Parent;
                InnerParent = VisualTreeHelper.GetParent(InnerParent);
            }
            if (InnerParent != null)
            {
                return;
            }
            var be = GetBindingExpression(ComboBox.TextProperty);
            if (be != null)
            {
                be.UpdateSource();
            }


        }
    }
}
