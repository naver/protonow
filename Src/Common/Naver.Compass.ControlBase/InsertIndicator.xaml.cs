using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Naver.Compass.Common
{
    /// <summary>
    /// InsertIndicator.xaml 的交互逻辑
    /// </summary>
    public partial class InsertIndicator : UserControl
    {
        public static Color EnableColor = Color.FromRgb(0, 157, 217);
        public static Color DisableColor = Color.FromRgb(255, 0, 0);
        public InsertIndicator()
        {
            InitializeComponent();
            this.DataContext = null;
            this.Loaded += (sender, e) =>
            {
                this.DataContext = this;
            };
        }

        #region IsEnable

        public static readonly DependencyProperty IsEnableProperty =
            DependencyProperty.Register("IsEnable", typeof(bool), typeof(InsertIndicator),
                new FrameworkPropertyMetadata(true,
                    new PropertyChangedCallback(OnIsEnableChanged)));

        public bool IsEnable
        {
            get { return (bool)GetValue(IsEnableProperty); }
            set { SetValue(IsEnableProperty, value); }
        }

        private static void OnIsEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InsertIndicator target = (InsertIndicator)d;
            bool oldIsEnable = (bool)e.OldValue;
            bool newIsEnable = target.IsEnable;
            target.OnIsEnableChanged(oldIsEnable, newIsEnable);
        }

        protected virtual void OnIsEnableChanged(bool oldIsEnable, bool newIsEnable)
        {
            if (newIsEnable)
            {
                Foreground = new SolidColorBrush(EnableColor);
            }
            else
            {
                Foreground = new SolidColorBrush(DisableColor);
            }
        }

        #endregion

        #region Foreground

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(SolidColorBrush), typeof(InsertIndicator),
                new FrameworkPropertyMetadata(new SolidColorBrush(EnableColor),
                    FrameworkPropertyMetadataOptions.None));

        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        #endregion


    }
}
