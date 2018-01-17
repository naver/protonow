using Microsoft.Expression.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Naver.Compass.Common
{
    public class GradientThumb : Thumb
    {
        public MouseButtonEventHandler MouseClicked;
        public event EventHandler ThumbRemoving;
        public event EventHandler ThumbRecovering;
        public event EventHandler ThumbRemoved;
        public GradientThumb()
        {
            DragDelta += GradientThumb_DragDelta;
            DragCompleted += GradientThumb_DragCompleted;
        }

        void GradientThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (Math.Abs(e.VerticalChange) > 25 && this.Opacity == 0d)
            {
                if (this.ThumbRemoved != null)
                {
                    this.ThumbRemoved(this, null);
                }
            }
        }

        void GradientThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var self = sender as Control;

            if (self != null && self.Parent is Canvas)
            {
                var canvas = self.Parent as Canvas;
                double left = Canvas.GetLeft(self);
                left = left + e.HorizontalChange;
                if (left < 0)
                {
                    left = 0;
                }
                else if (left > canvas.ActualWidth)
                {
                    left = canvas.ActualWidth;
                }

                Canvas.SetLeft(self, left);
                Canvas.SetTop(self, 0d);

                if (Math.Abs(e.VerticalChange) > 25)
                {
                    if (this.ThumbRemoving != null)
                    {
                        this.ThumbRemoving(this, null);
                    }
                }
                else
                {
                    if (this.Opacity == 0d
                        && this.ThumbRecovering != null)
                    {
                        this.ThumbRecovering(this, null);
                    }
                }
            }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.MouseClicked != null)
            {
                this.MouseClicked(this, e);
            }

            e.Handled = false;
        }

        internal void SetColor(Color color)
        {
            var rect = this.GetTemplateChild("colorHint") as Rectangle;
            if (rect == null)
            {
                RoutedEventHandler handler = default(RoutedEventHandler);
                handler = (sender, e) =>
                {
                    this.Loaded -= handler;
                    this.SetColor(color);
                };

                this.Loaded += handler;
            }
            else
            {
                rect.Fill = new SolidColorBrush(color);
            }
        }

        internal Color GetColor()
        {
            var rect = this.GetTemplateChild("colorHint") as Rectangle;
            var solidbrush = rect.Fill as SolidColorBrush;
            return solidbrush == null ? default(Color) : solidbrush.Color;
        }

        internal void SetSelected(bool isSelected)
        {
            var polygon = this.GetTemplateChild("selectedHint") as RegularPolygon;
            if (polygon == null)
            {
                RoutedEventHandler handler = default(RoutedEventHandler);
                handler = (sender, e) =>
                {
                    this.Loaded -= handler;
                    this.SetSelected(isSelected);
                };

                this.Loaded += handler;
            }
            else
            {
                if (!isSelected)
                {
                    polygon.Fill = new SolidColorBrush(Color.FromRgb(0xed, 0xed, 0xed));
                }
                else
                {
                    polygon.Fill = new SolidColorBrush(Colors.Black);
                }
            }
        }
    }
}
