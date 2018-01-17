using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace Naver.Compass.WidgetLibrary
{
    public class DesignerItemDecorator : Control
    {
        private Adorner adorner=null, _backupAdorner=null;
        private bool _isLoadRender=false;
   
        public bool ShowDecorator
        {
            get { return (bool)GetValue(ShowDecoratorProperty); }
            set { SetValue(ShowDecoratorProperty, value); }
        }

        public static readonly DependencyProperty ShowDecoratorProperty =
            DependencyProperty.Register("ShowDecorator", typeof(bool), typeof(DesignerItemDecorator),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowDecoratorProperty_Changed)));

        public DesignerItemDecorator()
        {
            Unloaded += new RoutedEventHandler(this.DesignerItemDecorator_Unloaded);
            Loaded +=new RoutedEventHandler(this.DesignerItemDecorator_Loaded);
        }

        private void HideAdorner()
        {
            if (this.adorner != null)
            {
                this.adorner.Visibility = Visibility.Hidden;
            }
        }

        private void ShowAdorner()
        {

            if (this._backupAdorner != null)
            {
                this.adorner = _backupAdorner;
                _backupAdorner = null;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer == null)
                {
                    return;
                }
                adornerLayer.Add(this.adorner);
            }

            if (this.adorner == null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

                if (adornerLayer != null)
                {
                    ContentControl designerItem = this.DataContext as ContentControl;
                    Canvas canvas = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(designerItem)) as Canvas;
                    this.adorner = new ResizeRotateAdorner(designerItem);
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

        private void DesignerItemDecorator_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.adorner != null)
            {
                _backupAdorner = this.adorner;

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this.adorner);
                }
                //this.adorner = null;
                
               // _isLoadRender = true;
            }
            if (ShowDecorator == true)
            {
                _isLoadRender = true;
            }
            else
            {
                _isLoadRender = false;
            }

        }
        
        //re-load the adorner when switch the page tabs,20140307
        private void DesignerItemDecorator_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (_isLoadRender == true)
            {               
                ShowAdorner();
            }
        }
        private static void ShowDecoratorProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DesignerItemDecorator decorator = (DesignerItemDecorator)d;
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

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            base.ArrangeOverride(arrangeBounds);
            return arrangeBounds;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }
    }
}
