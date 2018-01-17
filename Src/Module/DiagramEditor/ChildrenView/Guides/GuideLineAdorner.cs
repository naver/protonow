using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace Naver.Compass.Module
{
    public class GuideLinAdorner : Adorner
    {
        private VisualCollection visuals;
        private Control chrome;
        //private ResizeChrome chrome2;
        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public GuideLinAdorner(ContentControl designerItem)
            : base(designerItem)
        {
            SnapsToDevicePixels = true;
            GuideLineDecorator item = designerItem as GuideLineDecorator;
            if (item.IsVline == true)
            {
                this.chrome = new VGuidelineChrome();
                this.chrome.DataContext = designerItem;
                this.visuals = new VisualCollection(this);
                this.visuals.Add(this.chrome);
            }
            else 
            {
                this.chrome = new HGuidelineChrome();
                this.chrome.DataContext = designerItem;
                this.visuals = new VisualCollection(this);
                this.visuals.Add(this.chrome);
            }            
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }
    }

    public class VGuidelineChrome : Control
    {
        static VGuidelineChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VGuidelineChrome), new FrameworkPropertyMetadata(typeof(VGuidelineChrome)));
        }
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected", typeof(bool),
                                      typeof(VGuidelineChrome),
                                      new FrameworkPropertyMetadata(false));

        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        public static readonly DependencyProperty IsLockedProperty =
          DependencyProperty.Register("IsLocked", typeof(bool),
                                      typeof(VGuidelineChrome),
                                      new FrameworkPropertyMetadata(false));

        public SolidColorBrush LineColor
        {
            get { return (SolidColorBrush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        public static readonly DependencyProperty LineColorProperty =
          DependencyProperty.Register("LineColor", typeof(SolidColorBrush),
                                      typeof(VGuidelineChrome),
                                      new FrameworkPropertyMetadata(Brushes.BlueViolet));
    }

    public class HGuidelineChrome : Control
    {
        static HGuidelineChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HGuidelineChrome), new FrameworkPropertyMetadata(typeof(HGuidelineChrome)));
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected", typeof(bool),
                                      typeof(HGuidelineChrome),
                                      new FrameworkPropertyMetadata(false));

        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        public static readonly DependencyProperty IsLockedProperty =
          DependencyProperty.Register("IsLocked", typeof(bool),
                                      typeof(HGuidelineChrome),
                                      new FrameworkPropertyMetadata(false));

        public SolidColorBrush LineColor
        {
            get { return (SolidColorBrush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        public static readonly DependencyProperty LineColorProperty =
          DependencyProperty.Register("LineColor", typeof(SolidColorBrush),
                                      typeof(HGuidelineChrome),
                                      new FrameworkPropertyMetadata(Brushes.BlueViolet));
    }
}
