using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Naver.Compass.Common
{
    public class ColorPickerManager
    {
        /// <summary>  
        ///   
        /// </summary>  
        /// <param name="x">鼠标相对于显示器的坐标X</param>  
        /// <param name="y">鼠标相对于显示器的坐标Y</param>  
        /// <returns></returns>  
        public static Color GetColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb(/*(byte)((int)(pixel & 0xFF000000) >> 16 >> 8)*/0xff, (byte)((int)(pixel & 0x000000FF)), (byte)((int)(pixel & 0x0000FF00) >> 8), (byte)((int)(pixel & 0x00FF0000) >> 16));

            return color;
        }

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(
        IntPtr hdcDest, // 目标设备的句柄   
        int nXDest, // 目标对象的左上角的X坐标   
        int nYDest, // 目标对象的左上角的X坐标   
        int nWidth, // 目标对象的矩形的宽度   
        int nHeight, // 目标对象的矩形的长度   
        IntPtr hdcSrc, // 源设备的句柄   
        int nXSrc, // 源对象的左上角的X坐标   
        int nYSrc, // 源对象的左上角的X坐标   
        int dwRop // 光栅的操作值   
        );

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDC(
        string lpszDriver, // 驱动名称   
        string lpszDevice, // 设备名称   
        string lpszOutput, // 无用，可以设定位"NULL"   
        IntPtr lpInitData // 任意的打印机数据   
        );

        /// <summary>  
        /// 获取指定窗口的设备场景  
        /// </summary>  
        /// <param name="hwnd">将获取其设备场景的窗口的句柄。若为0，则要获取整个屏幕的DC</param>  
        /// <returns>指定窗口的设备场景句柄，出错则为0</returns>  
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        /// <summary>  
        /// 释放由调用GetDC函数获取的指定设备场景  
        /// </summary>  
        /// <param name="hwnd">要释放的设备场景相关的窗口句柄</param>  
        /// <param name="hdc">要释放的设备场景句柄</param>  
        /// <returns>执行成功为1，否则为0</returns>  
        [DllImport("user32.dll")]
        private static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        /// <summary>  
        /// 在指定的设备场景中取得一个像素的RGB值  
        /// </summary>  
        /// <param name="hdc">一个设备场景的句柄</param>  
        /// <param name="nXPos">逻辑坐标中要检查的横坐标</param>  
        /// <param name="nYPos">逻辑坐标中要检查的纵坐标</param>  
        /// <returns>指定点的颜色</returns>  
        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
    }

    /// <summary>
    /// GradientEditor.xaml 的交互逻辑
    /// </summary>
    public partial class GradientEditor : UserControl
    {
        private GradientThumb selectedThumb;
        public GradientEditor()
        {
            InitializeComponent();
            this.Loaded += GradientEditor_Loaded;
        }

        void GradientEditor_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyPropertyDescriptor borderbg = DependencyPropertyDescriptor.FromProperty(Border.BackgroundProperty, typeof(Border));
            borderbg.AddValueChanged(this.GradientBorder, this.BorderBgChanged);
        }

        private void BorderBgChanged(object sender, EventArgs e)
        {
            var border = sender as Border;
            this.ColorSource = border.Background;
        }

        #region InitialColor

        public static readonly DependencyProperty InitialColorProperty =
            DependencyProperty.Register("InitialColor", typeof(StyleColor), typeof(GradientEditor),
                new FrameworkPropertyMetadata(default(StyleColor),
                    new PropertyChangedCallback(OnInitialColorChanged)));

        public StyleColor InitialColor
        {
            get { return (StyleColor)GetValue(InitialColorProperty); }
            set { SetValue(InitialColorProperty, value); }
        }

        private static void OnInitialColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GradientEditor target = (GradientEditor)d;
            StyleColor oldInitialColor = (StyleColor)e.OldValue;
            StyleColor newInitialColor = target.InitialColor;
            target.OnInitialColorChanged(oldInitialColor, newInitialColor);
        }

        protected virtual void OnInitialColorChanged(StyleColor oldInitialColor, StyleColor newInitialColor)
        {
            if (!(GradientBorder.Background is LinearGradientBrush))
            {
                return;
            }

            var linebrush = GradientBorder.Background as LinearGradientBrush;
            linebrush.GradientStops.Clear();

            var newFrames = newInitialColor.Frames;
            if (newFrames == null || newFrames.Count == 0)
            {
                newFrames = new Dictionary<double, int>();
                newFrames[0] = -1;
                newFrames[1] = -16777216;
            }

            var parentCanvas = this.canvas;
            parentCanvas.Children.Clear();
            var actualWidth = parentCanvas.ActualWidth;
            foreach (var keypair in newFrames)
            {
                var gradientThumb = new GradientThumb
                {
                    Template = this.Resources["GradientThumbTemplate"] as ControlTemplate,
                    Tag = keypair.Key
                };


                gradientThumb.DragDelta += gradientThumb_DragDelta;
                gradientThumb.MouseClicked += gradientThumb_MouseDown;
                gradientThumb.ThumbRemoving += gradientThumb_ThumbRemoving;
                gradientThumb.ThumbRecovering += gradientThumb_ThumbRecovering;
                gradientThumb.ThumbRemoved += gradientThumb_ThumbRemoved;

                Canvas.SetLeft(gradientThumb, keypair.Key * actualWidth);
                parentCanvas.Children.Add(gradientThumb);

                var drawingColor = System.Drawing.Color.FromArgb(keypair.Value);
                gradientThumb.SetColor(Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B));
                var gradientStop = new GradientStop(
                    Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B),
                    keypair.Key);
                linebrush.GradientStops.Add(gradientStop);
            }

            this.selectedThumb = parentCanvas.Children[0] as GradientThumb;
            this.selectedThumb.SetSelected(true);
            this.SelectedColor = linebrush.GradientStops[0].Color;

            CurrentStyle = newInitialColor;
        }


        void gradientThumb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.SetThumbSeletected(sender as GradientThumb);
            }
        }

        private void SetThumbSeletected(GradientThumb sthumb)
        {
            var parentCanvas = this.canvas;
            foreach (var thumb in parentCanvas.Children.OfType<GradientThumb>())
            {
                if (thumb == sthumb)
                {
                    thumb.SetSelected(true);
                    this.selectedThumb = thumb;
                    this.SelectedColor = thumb.GetColor();
                }
                else
                {
                    thumb.SetSelected(false);
                }
            }
        }

        void gradientThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var self = sender as Control;

            if (self != null && self.Parent is Canvas)
            {
                if (this.selectedThumb == null || !(this.selectedThumb.Tag is double))
                {
                    return;
                }

                if (!(GradientBorder.Background is LinearGradientBrush))
                {
                    return;
                }

                var linebrush = GradientBorder.Background as LinearGradientBrush;
                var offset = (double)this.selectedThumb.Tag;
                var stop = linebrush.GradientStops.Where(x => x.Offset == offset).FirstOrDefault();
                if (stop != null)
                {

                    var canvas = self.Parent as Canvas;
                    double left = Canvas.GetLeft(self);
                    var newOffset = Math.Round(left / canvas.ActualWidth, 2);

                    while (linebrush.GradientStops.Except(new List<GradientStop> { stop }).Any(x => x.Offset == newOffset))
                    {
                        if (offset < newOffset)
                        {
                            /// move to right
                            newOffset -= 0.0001d;
                        }
                        else
                        {
                            /// move to left
                            newOffset += 0.0001d;
                        }
                    }

                    stop.Offset = newOffset;
                    this.selectedThumb.Tag = newOffset;


                    var rtnStyle = new StyleColor();
                    rtnStyle.FillType = ColorFillType.Gradient;
                    rtnStyle.Frames = new Dictionary<double, int>();
                    foreach (var gs in linebrush.GradientStops)
                    {
                        if (!rtnStyle.Frames.ContainsKey(gs.Offset))
                        {
                            rtnStyle.Frames.Add(gs.Offset, gs.Color.ToArgb());
                        }
                    }

                    CurrentStyle = rtnStyle;
                }
            }

        }

        #endregion



        #region SelectedColor

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(GradientEditor),
                new FrameworkPropertyMetadata(default(Color),
                    new PropertyChangedCallback(OnSelectedColorChanged)));

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GradientEditor target = (GradientEditor)d;
            Color oldSelectedColor = (Color)e.OldValue;
            Color newSelectedColor = target.SelectedColor;
            target.OnSelectedColorChanged(oldSelectedColor, newSelectedColor);
        }

        protected virtual void OnSelectedColorChanged(Color oldSelectedColor, Color newSelectedColor)
        {
            if (this.selectedThumb == null || !(this.selectedThumb.Tag is double))
            {
                return;
            }

            if (!(GradientBorder.Background is LinearGradientBrush))
            {
                return;
            }

            this.selectedThumb.SetColor(newSelectedColor);
            var linebrush = GradientBorder.Background as LinearGradientBrush;
            var offset = (double)this.selectedThumb.Tag;
            var stop = linebrush.GradientStops.Where(x => x.Offset == offset).FirstOrDefault();
            if (stop != null)
            {
                stop.Color = newSelectedColor;
            }

            var rtnStyle = new StyleColor();
            rtnStyle.FillType = ColorFillType.Gradient;
            rtnStyle.Frames = new Dictionary<double, int>();
            foreach (var gs in linebrush.GradientStops)
            {
                rtnStyle.Frames.Add(gs.Offset, gs.Color.ToArgb());
            }

            CurrentStyle = rtnStyle;
        }

        #endregion

        #region CurrentStyle

        public static readonly DependencyProperty CurrentStyleProperty =
            DependencyProperty.Register("CurrentStyle", typeof(StyleColor), typeof(GradientEditor),
                new FrameworkPropertyMetadata(default(StyleColor),
                    FrameworkPropertyMetadataOptions.None));

        public StyleColor CurrentStyle
        {
            get { return (StyleColor)GetValue(CurrentStyleProperty); }
            set { SetValue(CurrentStyleProperty, value); }
        }

        #endregion

        #region ColorSource

        public static readonly DependencyProperty ColorSourceProperty =
            DependencyProperty.Register("ColorSource", typeof(Brush), typeof(GradientEditor),
                new FrameworkPropertyMetadata(default(Brush),
                    FrameworkPropertyMetadataOptions.None));

        public Brush ColorSource
        {
            get { return (Brush)GetValue(ColorSourceProperty); }
            set { SetValue(ColorSourceProperty, value); }
        }

        #endregion

        internal void RecoverHoverColor()
        {
            var Color32 = SelectedColor;
            if (this.selectedThumb == null || !(this.selectedThumb.Tag is double))
            {
                return;
            }

            if (!(GradientBorder.Background is LinearGradientBrush))
            {
                return;
            }

            this.selectedThumb.SetColor(Color32);
            var linebrush = GradientBorder.Background as LinearGradientBrush;
            var offset = (double)this.selectedThumb.Tag;
            var stop = linebrush.GradientStops.Where(x => x.Offset == offset).FirstOrDefault();
            if (stop != null)
            {
                stop.Color = Color32;
            }
        }

        internal void SetHoverColor(Color Color32)
        {
            if (this.selectedThumb == null || !(this.selectedThumb.Tag is double))
            {
                return;
            }

            if (!(GradientBorder.Background is LinearGradientBrush))
            {
                return;
            }

            this.selectedThumb.SetColor(Color32);
            var linebrush = GradientBorder.Background as LinearGradientBrush;
            var offset = (double)this.selectedThumb.Tag;
            var stop = linebrush.GradientStops.Where(x => x.Offset == offset).FirstOrDefault();
            if (stop != null)
            {
                stop.Color = Color32;
            }
        }

        internal void SetAlpha(byte A)
        {
            if (this.selectedThumb == null || !(this.selectedThumb.Tag is double))
            {
                return;
            }

            if (!(GradientBorder.Background is LinearGradientBrush))
            {
                return;
            }

            var c = Color.FromArgb(A, SelectedColor.R, SelectedColor.G, SelectedColor.B);
            SelectedColor = c;
            this.selectedThumb.SetColor(SelectedColor);
            var linebrush = GradientBorder.Background as LinearGradientBrush;
            var offset = (double)this.selectedThumb.Tag;
            var stop = linebrush.GradientStops.Where(x => x.Offset == offset).FirstOrDefault();
            if (stop != null)
            {
                stop.Color = SelectedColor;
            }

            var rtnStyle = new StyleColor();
            rtnStyle.FillType = ColorFillType.Gradient;
            rtnStyle.Frames = new Dictionary<double, int>();
            foreach (var gs in linebrush.GradientStops)
            {
                rtnStyle.Frames.Add(gs.Offset, gs.Color.ToArgb());
            }

            CurrentStyle = rtnStyle;
        }

        internal void SwitchToGradient()
        {
            var rtnStyle = new StyleColor();
            rtnStyle.FillType = ColorFillType.Gradient;
            rtnStyle.Frames = new Dictionary<double, int>();
            var linebrush = GradientBorder.Background as LinearGradientBrush;
            foreach (var gs in linebrush.GradientStops)
            {
                rtnStyle.Frames.Add(gs.Offset, gs.Color.ToArgb());
            }

            CurrentStyle = rtnStyle;
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            ///暂时屏蔽多点渐变色
            if (e.LeftButton == MouseButtonState.Pressed
                && e.RightButton == MouseButtonState.Released
                && sender is Canvas)
            {
                var canvas = sender as Canvas;
                var mouseposition = e.GetPosition(canvas);
                var newOffset = Math.Round(mouseposition.X / canvas.ActualWidth, 2);
                var linebrush = GradientBorder.Background as LinearGradientBrush;
                if (linebrush.GradientStops.Any(x => Math.Abs(x.Offset - newOffset) <= 0.05))
                {
                    return;
                }

                var gradientThumb = new GradientThumb
                {
                    Template = this.Resources["GradientThumbTemplate"] as ControlTemplate,
                    Tag = newOffset
                };

                gradientThumb.DragDelta += gradientThumb_DragDelta;
                gradientThumb.MouseClicked += gradientThumb_MouseDown;
                gradientThumb.ThumbRemoving += gradientThumb_ThumbRemoving;
                gradientThumb.ThumbRecovering += gradientThumb_ThumbRecovering;
                gradientThumb.ThumbRemoved += gradientThumb_ThumbRemoved;

                Canvas.SetLeft(gradientThumb, newOffset * canvas.ActualWidth);
                canvas.Children.Add(gradientThumb);

                var relativeposition = new System.Windows.Point(
                    newOffset * GradientBorder.ActualWidth,
                    GradientBorder.ActualHeight / 2);
                var screenposition = GradientBorder.PointToScreen(relativeposition);
                var drawingColor = ColorPickerManager.GetColor((int)screenposition.X, (int)screenposition.Y);
                gradientThumb.SetColor(Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B));
                var gradientStop = new GradientStop(
                    Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B),
                    newOffset);


                linebrush.GradientStops.Add(gradientStop);

                foreach (var thumb in canvas.Children.OfType<GradientThumb>())
                {
                    if (thumb == gradientThumb)
                    {
                        thumb.SetSelected(true);
                        this.selectedThumb = thumb;
                    }
                    else
                    {
                        thumb.SetSelected(false);
                    }
                }
                this.selectedThumb.SetSelected(true);
                this.SelectedColor = drawingColor;

                var rtnStyle = new StyleColor();
                rtnStyle.FillType = ColorFillType.Gradient;
                rtnStyle.Frames = new Dictionary<double, int>();
                foreach (var gs in linebrush.GradientStops)
                {
                    rtnStyle.Frames.Add(gs.Offset, gs.Color.ToArgb());
                }

                CurrentStyle = rtnStyle;
            }
        }

        void gradientThumb_ThumbRemoving(object sender, EventArgs e)
        {
            var thumb = sender as GradientThumb;
            if (thumb == null || !(thumb.Parent is Canvas))
            {
                return;
            }

            var canvas = thumb.Parent as Canvas;
            var thumbcount = canvas.Children.OfType<GradientThumb>().Count();

            if (thumbcount > 2)
            {
                this.RemoveGradientStop(thumb);
            }
        }

        private void RemoveGradientStop(GradientThumb toRemoveThumb)
        {
            if (toRemoveThumb == null || !(toRemoveThumb.Tag is double) || !(toRemoveThumb.Parent is Canvas))
            {
                return;
            }

            var offsetTag = (double)toRemoveThumb.Tag;
            var linebrush = GradientBorder.Background as LinearGradientBrush;
            var removeTag = linebrush.GradientStops.FirstOrDefault(x => x.Offset == offsetTag);
            if (removeTag != null)
            {
                linebrush.GradientStops.Remove(removeTag);
            }

            var canvas = toRemoveThumb.Parent as Canvas;
            //canvas.Children.Remove(toRemoveThumb);
            toRemoveThumb.Opacity = 0d;

            var rtnStyle = new StyleColor();
            rtnStyle.FillType = ColorFillType.Gradient;
            rtnStyle.Frames = new Dictionary<double, int>();
            foreach (var gs in linebrush.GradientStops)
            {
                rtnStyle.Frames.Add(gs.Offset, gs.Color.ToArgb());
            }

            CurrentStyle = rtnStyle;
        }

        void gradientThumb_ThumbRecovering(object sender, EventArgs e)
        {
            var thumb = sender as GradientThumb;
            if (thumb == null || !(thumb.Parent is Canvas))
            {
                return;
            }

            thumb.Opacity = 1d;
            var thumbColor = thumb.GetColor();
            var linebrush = GradientBorder.Background as LinearGradientBrush;
            var canvas = thumb.Parent as Canvas;
            var left = Canvas.GetLeft(thumb);
            var newOffset = Math.Round(left / canvas.ActualWidth, 2);

            while (linebrush.GradientStops.Any(x => x.Offset == newOffset))
            {
                if (newOffset == 0d)
                {
                    /// move to right
                    newOffset += 0.0001d;
                }
                else
                {
                    /// move to left
                    newOffset -= 0.0001d;
                }
            }

            var gradientStop = new GradientStop(
                    Color.FromArgb(thumbColor.A, thumbColor.R, thumbColor.G, thumbColor.B),
                    newOffset);
            linebrush.GradientStops.Add(gradientStop);

            thumb.Tag = newOffset;

            var rtnStyle = new StyleColor();
            rtnStyle.FillType = ColorFillType.Gradient;
            rtnStyle.Frames = new Dictionary<double, int>();
            foreach (var gs in linebrush.GradientStops)
            {
                if (!rtnStyle.Frames.ContainsKey(gs.Offset))
                {
                    rtnStyle.Frames.Add(gs.Offset, gs.Color.ToArgb());
                }
            }

            CurrentStyle = rtnStyle;
        }

        void gradientThumb_ThumbRemoved(object sender, EventArgs e)
        {
            var thumb = sender as GradientThumb;
            if (thumb == null || !(thumb.Parent is Canvas))
            {
                return;
            }

            var canvas = thumb.Parent as Canvas;
            canvas.Children.Remove(thumb);

            var toSelectedThumb = canvas.Children.OfType<GradientThumb>().OrderBy(x => x.Tag is double ? (double)x.Tag : double.MaxValue).FirstOrDefault();
            if (toSelectedThumb != null)
            {
                this.selectedThumb = toSelectedThumb;
                this.selectedThumb.SetSelected(true);
                this.SelectedColor = this.selectedThumb.GetColor();
            }
        }
    }
}
