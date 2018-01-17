using Naver.Compass.Common.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Naver.Compass.Module
{
    /// <summary>
    /// A ruler control which supports both centimeters and inches. In order to use it vertically, change the <see cref="Marks">Marks</see> property to <c>Up</c> and rotate it ninety degrees.
    /// </summary>
    public class EditorGrid : FrameworkElement
    {
        // private DrawingVisual visual = new MyDrawingVisual();
        #region Fields
       // private readonly Pen ThinPen = new Pen(new SolidColorBrush(Color.FromRgb(0xDE, 0xF1, 0xFA)), 1);
        #endregion

        #region Properties

        #region Zoom
        /// <summary>
        /// Gets or sets the Zoom factor for the ruler. The default value is 1.0. 
        /// </summary>
        public double Zoom
        {
            get
            {
                return (double)GetValue(Zoom1Property);
            }
            set
            {
                SetValue(Zoom1Property, value);
                this.InvalidateVisual();
            }
        }
        public static readonly DependencyProperty Zoom1Property =
            DependencyProperty.Register("Zoom", typeof(double), typeof(EditorGrid),
            new FrameworkPropertyMetadata((double)1.0,
                FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region TriggerRender
        /// <summary>
        /// Gets or sets the Zoom factor for the ruler. The default value is 1.0. 
        /// </summary>
        public bool TriggerRender
        {
            get
            {
                return (bool)GetValue(TriggerRenderProperty);
            }
            set
            {
                SetValue(TriggerRenderProperty, value);
                //this.InvalidateVisual();
            }
        }
        public static readonly DependencyProperty TriggerRenderProperty =
            DependencyProperty.Register("TriggerRender", typeof(bool), typeof(EditorGrid),
            new FrameworkPropertyMetadata(true,
                FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region XCountShift
        /// <summary>
        /// By default the counting of inches or cm starts at zero, this property allows you to shift the start counting.
        /// </summary>
        public int XCountShift
        {
            get { return (int)GetValue(XCountShiftProperty); }
            set { SetValue(XCountShiftProperty, value); }
        }
        public static readonly DependencyProperty XCountShiftProperty =
             DependencyProperty.Register("XCountShift", typeof(int), typeof(EditorGrid),
                  new FrameworkPropertyMetadata(0,
                        FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region YCountShift
        /// <summary>
        /// By default the counting of inches or cm starts at zero, this property allows you to shift the start counting.
        /// </summary>
        public int YCountShift
        {
            get { return (int)GetValue(YCountShiftProperty); }
            set { SetValue(YCountShiftProperty, value); }
        }
        public static readonly DependencyProperty YCountShiftProperty =
             DependencyProperty.Register("YCountShift", typeof(int), typeof(EditorGrid),
                  new FrameworkPropertyMetadata(0,
                        FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region YCountShift
        public int VerticalChange { get; set; }
        #endregion

        #region Marks
        /// <summary>
        /// Gets or sets where the marks are shown in the ruler.
        /// </summary>
        public MarksLocation Marks
        {
            get { return (MarksLocation)GetValue(MarksProperty); }
            set { SetValue(MarksProperty, value); }
        }
        public static readonly DependencyProperty MarksProperty =
             DependencyProperty.Register("Marks", typeof(MarksLocation), typeof(EditorGrid),
                  new FrameworkPropertyMetadata(MarksLocation.Up,
                         FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Scroll change
        /// <summary>
        /// By default the counting of inches or cm starts at zero, this property allows you to shift the start counting.
        /// </summary>
        public int HorizontalChange { get; set; }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// Participates in rendering operations.
        /// if line: draw horizontal and vertical lines
        /// if intersection: draw only horizontal or verlical lines
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
           // base.OnRender(drawingContext);
            this.VisualEdgeMode = EdgeMode.Aliased;

            Color lineColor = Color.FromArgb(GlobalData.GridColor.A, GlobalData.GridColor.R, GlobalData.GridColor.G, GlobalData.GridColor.B);
            Pen ThinPen = new Pen(new SolidColorBrush(lineColor), 1);

            double offset = 0;//offset for intersection style

            //Draw Vertical lines
            double start = Math.Max(0, Convert.ToInt32(XCountShift / Zoom) / GlobalData.GRID_SIZE - 1) * GlobalData.GRID_SIZE;
            //line,horizontal change, horizontal and vertical no change.
            if (GlobalData.IsLineType || HorizontalChange != 0 || VerticalChange == 0)
            {
                if (!GlobalData.IsLineType)
                {
                    offset = 5;
                    DashStyle dash_style = new DashStyle(new double[] { 0, GlobalData.GRID_SIZE*Zoom }, 4);
                    ThinPen.DashStyle = dash_style;
                }

                for (double dUnit = start; dUnit <= this.ActualWidth / Zoom + start + 200; dUnit += GlobalData.GRID_SIZE)
                {
                    if (dUnit == 0 && !GlobalData.IsLineType)
                        continue;
                    double d = dUnit * this.Zoom - XCountShift + 1;
                    drawingContext.DrawLine(ThinPen, new Point(d, offset), new Point(d, this.ActualHeight));
                }
            }

            offset = 0;
            ThinPen = new Pen(new SolidColorBrush(lineColor), 1);

            //Draw Horizontal lines
            start = Math.Max(0, Convert.ToInt32(YCountShift / Zoom) / GlobalData.GRID_SIZE - 1) * GlobalData.GRID_SIZE;
            if (GlobalData.IsLineType || VerticalChange != 0 || HorizontalChange == 0)
            {
                if (!GlobalData.IsLineType)
                {
                    offset = 8;
                    DashStyle dash_style = new DashStyle(new double[] { 0, GlobalData.GRID_SIZE*Zoom }, 7);
                    ThinPen.DashStyle = dash_style;
                }

                for (double dUnit = start; dUnit <= this.ActualHeight / Zoom + start + 200; dUnit += GlobalData.GRID_SIZE)
                {
                    if (dUnit == 0 && !GlobalData.IsLineType)
                        continue;
                    double d = dUnit * this.Zoom - YCountShift + 1;
                    drawingContext.DrawLine(ThinPen, new Point(offset, d), new Point(this.ActualWidth, d));
                }
            }
        }

        #endregion
    }

}
