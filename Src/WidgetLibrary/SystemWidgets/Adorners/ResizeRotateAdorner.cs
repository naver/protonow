using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace Naver.Compass.WidgetLibrary
{
    public class ResizeRotateAdorner : Adorner
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

        public ResizeRotateAdorner(ContentControl designerItem)
            : base(designerItem)
        {
            SnapsToDevicePixels = true;
            BaseWidgetItem item = designerItem as BaseWidgetItem;
            if (item == null)
            {
                this.chrome = new ResizeChrome();
                this.chrome.DataContext = designerItem;
                this.visuals = new VisualCollection(this);
                this.visuals.Add(this.chrome);
                return;

            }

            if (item.DecoChrome == DecoChromeType.HlineDecorator)
            {
                this.chrome = new HLineChrome();
                this.chrome.DataContext = designerItem;
                this.visuals = new VisualCollection(this);
                this.visuals.Add(this.chrome);
            }
            else if (item.DecoChrome == DecoChromeType.VlineDecorator)
            {
                this.chrome = new VLineChrome();
                this.chrome.DataContext = designerItem;
                this.visuals = new VisualCollection(this);
                this.visuals.Add(this.chrome);
            }
            else if (item.DecoChrome == DecoChromeType.RounedRotateDecorator)
            {
                //if (item.CanRotate==true)
                this.chrome = new RounedRotatChrome();
                this.chrome.DataContext = designerItem;
                this.visuals = new VisualCollection(this);
                this.visuals.Add(this.chrome);
            }
            else if (item.DecoChrome == DecoChromeType.RotateDecorator)
            {
                //if (item.CanRotate==true)
                this.chrome = new ResizeRotateChrome();
                this.chrome.DataContext = designerItem;
                this.visuals = new VisualCollection(this);
                this.visuals.Add(this.chrome);
            }
            else if (item.DecoChrome == DecoChromeType.NoRotateDecorator)
            {
                this.chrome = new ResizeChrome();
                this.chrome.DataContext = designerItem;
                this.visuals = new VisualCollection(this);
                this.visuals.Add(this.chrome);
            }
            else if (item.DecoChrome == DecoChromeType.HorRiszeDecorator)
            {
                this.chrome = new HorResizeChrome();
                this.chrome.DataContext = designerItem;
                this.visuals = new VisualCollection(this);
                this.visuals.Add(this.chrome);
            }
            else if (item.DecoChrome == DecoChromeType.MasterDecorator)
            {
                this.chrome = new MasterChrome();
                this.chrome.DataContext = designerItem;
                this.visuals = new VisualCollection(this);
                this.visuals.Add(this.chrome);
            }

            this.Loaded += (loads, loade) =>
            {
                if (timer == null)
                {
                    timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(1);
                    timer.Tick += (ticks, ticke) =>
                    {
                        System.Diagnostics.Debug.WriteLine("timer ticked.");
                        timer.Stop();
                        timer = null;
                        System.Threading.ThreadPool.QueueUserWorkItem(obj =>
                        {
                            System.Threading.Thread.Sleep(5);
                            this.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                var ador = FindVisualParent<AdornerLayer>(this);
                                if (ador != null)
                                {
                                    ador.InvalidateMeasure();
                                    ador.InvalidateArrange();
                                }
                            }),
                            null);
                        });
                    };

                    timer.Start();
                }
                else
                {
                    timer.Stop();
                    timer.Start();
                }
            };
        }

        private static DispatcherTimer timer;

        public T FindVisualParent<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                var parent = VisualTreeHelper.GetParent(depObj);
                if (parent == null)
                {
                    return null;
                }

                if (parent != null && parent is T)
                {
                    return (T)parent;
                }

                return FindVisualParent<T>(parent);
            }

            return null;
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
}
