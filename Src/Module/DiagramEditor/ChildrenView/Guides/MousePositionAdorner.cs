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
    public class MousePositionAdorner : Adorner
    {
        private MousePositionChrome chrome;
        private VisualCollection visuals;
        private Control LineChromeItem;
        private bool IsVline=false;
        //private ContentControl designerItem;
        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public MousePositionAdorner(Control linethumb ,bool isvline)
            : base(linethumb)
        {
            this.SnapsToDevicePixels = true;
            this.LineChromeItem = linethumb;
            this.chrome = new MousePositionChrome();
            this.chrome.DataContext = linethumb;
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.chrome);
            IsVline = isvline;
            chrome.IsVline = IsVline;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.chrome.Arrange(new Rect(new Point(0.0, 0.0), arrangeBounds));
            return arrangeBounds;
        }
    }

    public class MousePositionChrome : Control
    {
        static MousePositionChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MousePositionChrome), new FrameworkPropertyMetadata(typeof(MousePositionChrome)));
        }

        #region dependency Propery

        //public bool IsVline { get; set; }
        public bool IsVline
        {
            get { return (bool)GetValue(IsVlineProperty); }
            set { SetValue(IsVlineProperty, value); }
        }

        public static readonly DependencyProperty IsVlineProperty =
          DependencyProperty.Register("IsVline", typeof(bool),
                                      typeof(MousePositionChrome),
                                      new FrameworkPropertyMetadata(false));
        #endregion
    }

}
