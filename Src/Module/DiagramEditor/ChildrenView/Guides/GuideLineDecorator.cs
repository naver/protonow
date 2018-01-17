using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace Naver.Compass.Module
{
    public class GuideLineDecorator : ContentControl
    {
        public GuideLineDecorator()
        {
            Unloaded += new RoutedEventHandler(this.Line_Unloaded);
            Loaded += new RoutedEventHandler(this.Line_Loaded);
        }

        #region Privte function and property
        private Adorner adorner;
        private void HideAdorner()
        {
            if (this.adorner != null)
            {
                this.adorner.Visibility = Visibility.Hidden;
            }
        }
        private void ShowAdorner()
        {
            if (this.adorner == null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

                if (adornerLayer != null)
                {
                    ContentControl designerItem = this.DataContext as ContentControl;
                    this.adorner = new GuideLinAdorner(this);
                    adornerLayer.Add(this.adorner);

                    if (this.ShowDecorator)
                    {
                        this.adorner.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.adorner.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                this.adorner.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Binding Propery
        public bool IsVline
        {
            get { return (bool)GetValue(IsVlineProperty); }
            set { SetValue(IsVlineProperty, value); }
        }
        public static readonly DependencyProperty IsVlineProperty =
            DependencyProperty.Register("IsVline", typeof(bool), typeof(GuideLineDecorator),new FrameworkPropertyMetadata(true, null));

        public bool ShowDecorator 
        {
            get { return (bool)GetValue(ShowDecoratorProperty); }
            set { SetValue(ShowDecoratorProperty, value); }
        }
        public static readonly DependencyProperty ShowDecoratorProperty =
            DependencyProperty.Register("ShowDecorator", typeof(bool), typeof(GuideLineDecorator),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback(ShowDecoratorProperty_Changed)));

        private static void ShowDecoratorProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GuideLineDecorator decorator = (GuideLineDecorator)d;
            bool showDecorator = (bool)e.NewValue;

            if (showDecorator)
            {
                decorator.ShowAdorner();
            }
            else
            {
                decorator.HideAdorner();
            }
        }
        #endregion Binding Propery


        #region Event Handler
        private void Line_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.adorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this.adorner);
                }
                this.adorner = null;
            }
        }
        private void Line_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAdorner();
        }
        #endregion

        #region Override function
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            base.ArrangeOverride(arrangeBounds);
            return arrangeBounds;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        #endregion

    }
}
