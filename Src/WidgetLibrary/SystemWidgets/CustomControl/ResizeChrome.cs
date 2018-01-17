using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Naver.Compass.WidgetLibrary
{
    public class BaseChrome : Control
    {
        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        public static readonly DependencyProperty IsLockedProperty =
          DependencyProperty.Register("IsLocked", typeof(bool),
                                      typeof(BaseChrome),
                                      new FrameworkPropertyMetadata(false));

        public bool IsFixed
        {
            get { return (bool)GetValue(IsFixedProperty); }
            set { SetValue(IsFixedProperty, value); }
        }

        public static readonly DependencyProperty IsFixedProperty =
          DependencyProperty.Register("IsFixed", typeof(bool),
                                      typeof(BaseChrome),
                                      new FrameworkPropertyMetadata(false));
    }
    public class ResizeChrome : BaseChrome
    {
        static ResizeChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeChrome), new FrameworkPropertyMetadata(typeof(ResizeChrome)));
        }
    }
}
