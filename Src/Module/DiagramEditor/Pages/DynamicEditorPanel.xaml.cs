using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;

namespace Naver.Compass.Module
{
    /// <summary>
    /// Interaction logic for DynamicEditorPanel.xaml
    /// </summary>
    public partial class DynamicEditorPanel : UserControl
    {
        public DynamicEditorPanel()
        {
            InitializeComponent();

            Loaded += RulerEditorPanel_Loaded;

        }

        private bool _bIsFirestLoaded = false;
        private void RulerEditorPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (_bIsFirestLoaded == false)
            {
                PageEditorViewModel page = DataContext as PageEditorViewModel;
                if (page != null)
                {
                    page.InitialLoadWidget();
                }
                _bIsFirestLoaded = true;
            }
        }
        #region View posize
        public double viewX
        {
            get { return (double)GetValue(viewXProperty); }
            set { SetValue(viewXProperty, value); }
        }
        public static readonly DependencyProperty viewXProperty =
             DependencyProperty.Register("viewX", typeof(double), typeof(DynamicEditorPanel),
                   new PropertyMetadata(Double.MinValue));

        public double viewY
        {
            get { return (double)GetValue(viewYProperty); }
            set { SetValue(viewYProperty, value); }
        }
        public static readonly DependencyProperty viewYProperty =
             DependencyProperty.Register("viewY", typeof(double), typeof(DynamicEditorPanel),
                 new PropertyMetadata(Double.MinValue));

        public double viewWidth
        {
            get { return (double)GetValue(viewWidthProperty); }
            set { SetValue(viewWidthProperty, value); }
        }
        public static readonly DependencyProperty viewWidthProperty =
             DependencyProperty.Register("viewWidth", typeof(double), typeof(DynamicEditorPanel),
                 new PropertyMetadata(Double.MinValue));

        public double viewHeight
        {
            get { return (double)GetValue(viewHeightProperty); }
            set { SetValue(viewHeightProperty, value); }
        }
        public static readonly DependencyProperty viewHeightProperty =
             DependencyProperty.Register("viewHeight", typeof(double), typeof(DynamicEditorPanel),
                  new FrameworkPropertyMetadata(Double.MinValue,
                        FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        private void DesignerBaseCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point p = e.GetPosition(ExternCanvas);

            ruler_hMousePos.Margin = new Thickness(p.X - hruler.CountShift, 0, 0, 0);
            ruler_vMousePos.Margin = new Thickness(0, p.Y - vruler.CountShift, 0, 0);
            //ShowLocationX.Text = ((int)p.X).ToString();
            //ShowLocationY.Text = ((int)p.Y).ToString();

        }

        private void CountViewPosition()
        {
            viewX = DesignerScrollViewer.ContentHorizontalOffset;
            viewY = DesignerScrollViewer.ContentVerticalOffset;
            viewHeight = DesignerScrollViewer.ActualHeight;
            viewWidth = DesignerScrollViewer.ActualWidth;
        }
        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.Source == sender)
            {
                hruler.CountShift = Convert.ToInt32(e.HorizontalOffset);
                vruler.CountShift = Convert.ToInt32(e.VerticalOffset);

                grid.XCountShift = Convert.ToInt32(e.HorizontalOffset);
                grid.YCountShift = Convert.ToInt32(e.VerticalOffset);
                grid.HorizontalChange = Convert.ToInt32(e.HorizontalChange);
                grid.VerticalChange = Convert.ToInt32(e.VerticalChange);

               // DeviceBG.Margin = new Thickness(-e.HorizontalOffset, -e.VerticalOffset, 0, 0);

                CountViewPosition();
            }
        }

        private void EditCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            DesignerBaseCanvas it = sender as DesignerBaseCanvas;
            if (it == null)
                return;

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                IPagePropertyData page = this.DataContext as IPagePropertyData;
                Rect selectWdgBounding = page.BoundingRect;

                if (e.Delta > 0)
                {
                    //ExternCanvas.
                    ScaleTransform scaletransform = ExternCanvasBorder.RenderTransform as ScaleTransform;
                    //scaletransform.ScaleX = Math.Min(1.7, scaletransform.ScaleX + 0.01);
                    //scaletransform.ScaleY = Math.Min(1.7, scaletransform.ScaleY + 0.01);
                    scaletransform.ScaleX = PageScaleEnumerator.GetValue(scaletransform.ScaleX, true);
                    DesignerScrollViewer.ScrollToHorizontalOffset((selectWdgBounding.Left + selectWdgBounding.Width / 2) * scaletransform.ScaleX - DesignerScrollViewer.ActualWidth / 2);
                    DesignerScrollViewer.ScrollToVerticalOffset((selectWdgBounding.Top + selectWdgBounding.Height / 2) * scaletransform.ScaleX - DesignerScrollViewer.ActualHeight / 2);
                }
                else if (e.Delta < 0)
                {
                    ScaleTransform scaletransform = ExternCanvasBorder.RenderTransform as ScaleTransform;
                    //scaletransform.ScaleX = Math.Max(0.3, scaletransform.ScaleX - 0.01);
                    //scaletransform.ScaleY = Math.Max(0.3, scaletransform.ScaleY - 0.01);
                    scaletransform.ScaleX = PageScaleEnumerator.GetValue(scaletransform.ScaleX, false);
                    DesignerScrollViewer.ScrollToHorizontalOffset((selectWdgBounding.Left + selectWdgBounding.Width / 2) * scaletransform.ScaleX - DesignerScrollViewer.ActualWidth / 2);
                    DesignerScrollViewer.ScrollToVerticalOffset((selectWdgBounding.Top + selectWdgBounding.Height / 2) * scaletransform.ScaleX - DesignerScrollViewer.ActualHeight / 2);
                }
                e.Handled = true;
            }
        }

        private void Scroll_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }
        private void DesignerScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            //double a = e.GetPosition(DesignerScrollViewer).X;
            //double b = e.GetPosition(DesignerScrollViewer).Y;
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    Debug.WriteLine("scrollview-->>------" + a + "----" + b);
            //}

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Type ty = e.OriginalSource.GetType();
                //if(e.Source is CPSScrollViewer && (e.OriginalSource is Adorner)==false)
                //{ return; }
                if (e.OriginalSource.GetType().Equals(typeof(Thumb))
                    || e.OriginalSource.GetType().Equals(typeof(RepeatButton)))
                {
                    return;
                }

                Point position = e.GetPosition(DesignerScrollViewer);
                double xScroll = DesignerScrollViewer.ActualWidth - position.X;
                double yScroll = DesignerScrollViewer.ActualHeight - position.Y;
                if (xScroll < 30)
                {
                    DesignerScrollViewer.ScrollToHorizontalOffset(DesignerScrollViewer.HorizontalOffset + Math.Min(10, Math.Abs(30 - xScroll)));
                }
                else if (position.X < 10)
                {
                    DesignerScrollViewer.ScrollToHorizontalOffset(DesignerScrollViewer.HorizontalOffset - Math.Min(10, Math.Abs(10 - position.X)));
                }

                if (yScroll < 30)
                {
                    DesignerScrollViewer.ScrollToVerticalOffset(DesignerScrollViewer.VerticalOffset + Math.Min(10, Math.Abs(30 - yScroll)));
                }
                else if (position.Y < 10)
                {
                    DesignerScrollViewer.ScrollToVerticalOffset(DesignerScrollViewer.VerticalOffset - Math.Min(10, Math.Abs(10 - position.Y)));
                }
            }
        }

        private void DesignerScrollViewer_DragOver(object sender, DragEventArgs e)
        {
            Point position = e.GetPosition(DesignerScrollViewer);
            double xScroll = DesignerScrollViewer.ActualWidth - position.X;
            double yScroll = DesignerScrollViewer.ActualHeight - position.Y;

            if (xScroll < 50)
            {
                DesignerScrollViewer.ScrollToHorizontalOffset(DesignerScrollViewer.HorizontalOffset + 50 - xScroll);
            }
            else if (position.X < 30)
            {
                DesignerScrollViewer.ScrollToHorizontalOffset(DesignerScrollViewer.HorizontalOffset - 30 + position.X);
            }

            if (yScroll < 50)
            {
                DesignerScrollViewer.ScrollToVerticalOffset(DesignerScrollViewer.VerticalOffset + 50 - yScroll);
            }
            else if (position.Y < 30)
            {
                DesignerScrollViewer.ScrollToVerticalOffset(DesignerScrollViewer.VerticalOffset - 30 + position.Y);
            }

        }

        private void HRuler_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ruler_hGuideLine.Visibility = Visibility.Visible;
            editor_hGuideLine.Visibility = Visibility.Visible;
            Point p = e.GetPosition(hBorder);
            ruler_hGuideLine.Margin = new Thickness(p.X - 6, 0, 0, 0);
            editor_hGuideLine.Margin = new Thickness(p.X, 0, 0, 0);
        }

        private void HRuler_MouseLeave(object sender, MouseEventArgs e)
        {
            ruler_hGuideLine.Visibility = Visibility.Collapsed;
            editor_hGuideLine.Visibility = Visibility.Collapsed;
        }

        private void VRuler_MouseMove(object sender, MouseEventArgs e)
        {
            ruler_vGuideLine.Visibility = Visibility.Visible;
            editor_vGuideLine.Visibility = Visibility.Visible;
            Point p = e.GetPosition(vBorder);
            ruler_vGuideLine.Margin = new Thickness(0, p.Y - 6, 0, 0);
            editor_vGuideLine.Margin = new Thickness(0, p.Y, 0, 0);
        }

        private void VRuler_MouseLeave(object sender, MouseEventArgs e)
        {
            ruler_vGuideLine.Visibility = Visibility.Collapsed;
            editor_vGuideLine.Visibility = Visibility.Collapsed;
        }
        
    }
}
