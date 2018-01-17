using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Naver.Compass.WidgetLibrary
{
    public class BaseEditControl : UserControl
    {

        public bool IsInEditMode
        {
            get
            {
                return (bool)GetValue(IsInEditModeProperty);
            }
            set
            {
                SetValue(IsInEditModeProperty, value);
            }
        }

        public static readonly DependencyProperty IsInEditModeProperty =
            DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(BaseEditControl),
            new PropertyMetadata(false, EditModeChange));

        private static void EditModeChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BaseEditControl)d).OnEdtModeChange(Convert.ToBoolean(e.NewValue));
        }

        virtual protected void OnEdtModeChange(bool newValue)
        {

        }
    }
}
