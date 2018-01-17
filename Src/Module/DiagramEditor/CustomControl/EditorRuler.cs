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
    public class Ruler : FrameworkElement
    {
       // private DrawingVisual visual = new MyDrawingVisual();
        #region Fields
        private double SegmentHeight;
        private readonly Pen p = new Pen(new SolidColorBrush(Color.FromRgb(0xB8, 0xB8, 0xB8)), 1);
        private readonly Pen ThinPen = new Pen(new SolidColorBrush(Color.FromRgb(0xB9, 0xB9, 0xB9)), 1);
        private readonly Pen BorderPen = new Pen(new SolidColorBrush(Color.FromRgb(0xCA, 0xCA, 0xCA)),1);
        private readonly Pen RedPen = new Pen(Brushes.Red, 1.0);
        #endregion

        #region Properties
        #region Length
        /// <summary>
        /// Gets or sets the length of the ruler. If the <see cref="AutoSize"/> property is set to false (default) this
        /// is a fixed length. Otherwise the length is calculated based on the actual width of the ruler.
        /// </summary>
        public double Length
        {
            get
            {
                if (this.AutoSize)
                {
                    switch (Unit)
                    {
                        case Unit.Cm:
                            return (Double)DipHelper.DipToCm(this.ActualWidth) / this.Zoom;
                        case Unit.Inch:
                            return (Double)DipHelper.DipToInch(this.ActualWidth) / this.Zoom;
                        default:
                            return (Double)this.ActualWidth / 100 / this.Zoom;
                    }
                }
                else
                {
                    return (double)GetValue(LengthProperty);
                }
            }
            set
            {
                SetValue(LengthProperty, value);
            }
        }
        public static readonly DependencyProperty LengthProperty =
             DependencyProperty.Register(
                  "Length",
                  typeof(double),
                  typeof(Ruler),
                  new FrameworkPropertyMetadata(100D, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region AutoSize
        /// <summary>
        /// Gets or sets the AutoSize behavior of the ruler.
        /// false (default): the lenght of the ruler results from the <see cref="Length"/> property. If the window size is changed, e.g. wider
        ///						than the rulers length, free space is shown at the end of the ruler. No rescaling is done.
        /// true				 : the length of the ruler is always adjusted to its actual width. This ensures that the ruler is shown
        ///						for the actual width of the window.
        /// </summary>
        public bool AutoSize
        {
            get
            {
                return (bool)GetValue(AutoSizeProperty);
            }
            set
            {
                SetValue(AutoSizeProperty, value);
                this.InvalidateVisual();
            }
        }
        public static readonly DependencyProperty AutoSizeProperty =
             DependencyProperty.Register(
                  "AutoSize",
                  typeof(bool),
                  typeof(Ruler),
                  new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Zoom
        /// <summary>
        /// Gets or sets the zoom factor for the ruler. The default value is 1.0. 
        /// </summary>
        public double Zoom
        {
            get
            {
                return (double)GetValue(ZoomProperty);
            }
            set
            {
                SetValue(ZoomProperty, value);
                this.InvalidateVisual();
            }
        }
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(Ruler),
            new FrameworkPropertyMetadata((double)1.0,
                FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Chip
        /// <summary>
        /// Sets the location of the chip in the units of the ruler. it's unit is just for dip
        /// So, to set the chip to 10 in cm units the chip needs to be set to 10.
        /// Use the <see cref="DipHelper"/> class for conversions.
        /// </summary>
        public double Chip
        {
            get { return (double)GetValue(ChipProperty); }
            set { SetValue(ChipProperty, value); }
        }
        public static readonly DependencyProperty ChipProperty =
             DependencyProperty.Register("Chip", typeof(double), typeof(Ruler),
                  new FrameworkPropertyMetadata((double)-300,
                        FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region CountShift
        /// <summary>
        /// By default the counting of inches or cm starts at zero, this property allows you to shift the start counting.
        /// </summary>
        public int CountShift
        {
            get { return (int)GetValue(CountShiftProperty); }
            set { SetValue(CountShiftProperty, value); }
        }
        public static readonly DependencyProperty CountShiftProperty =
             DependencyProperty.Register("CountShift", typeof(int), typeof(Ruler),
                  new FrameworkPropertyMetadata(0,
                        FrameworkPropertyMetadataOptions.AffectsRender));
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
             DependencyProperty.Register("Marks", typeof(MarksLocation), typeof(Ruler),
                  new FrameworkPropertyMetadata(MarksLocation.Up,
                         FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion


        #region Unit
        /// <summary>
        /// Gets or sets the unit of the ruler.
        /// Default value is Unit.Cm.
        /// </summary>
        public Unit Unit
        {
            get { return (Unit)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        public static readonly DependencyProperty UnitProperty =
             DependencyProperty.Register(
                  "Unit", typeof(Unit), typeof(Ruler),
                  new FrameworkPropertyMetadata(Unit.DiP, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #endregion

        #region Constructor
        static Ruler()
        {
            HeightProperty.OverrideMetadata(typeof(Ruler), new FrameworkPropertyMetadata(17.0));
        }
        public Ruler()
        {
            SegmentHeight = this.Height ;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Participates in rendering operations.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            double xDest = this.ActualWidth;

            this.VisualEdgeMode = EdgeMode.Aliased;
           
            if (Marks == MarksLocation.Down)//hruler
            {
                drawingContext.DrawLine(BorderPen, new Point(0.0, Height), new Point(xDest, Height));
            }
            else//vruler
            {
                drawingContext.DrawLine(BorderPen, new Point(0.0, 0), new Point(xDest, 0));
            }
            double offset = 1;

            double DrawLength = Length + 2;
            double start = Math.Max(0, Convert.ToInt32(CountShift) / 100);
            double unit=100 / this.Zoom;
            double n = start;
            for (double dUnit = start; dUnit <= DrawLength*this.Zoom + start ; dUnit++,n++)
            {
                double d = dUnit * 100 ;

                for (int i = 1; i <= 9; i++)
                {
                    if (i != 5)
                    {//mark:10,20...110,120..
                        double nDip10 = 100 * (dUnit + 0.1 * i) - CountShift + offset;
                        if (Marks == MarksLocation.Up)
                            drawingContext.DrawLine(ThinPen, new Point(nDip10, 0), new Point(nDip10, SegmentHeight / 3.0));
                        else
                        {
                            drawingContext.DrawLine(ThinPen, new Point(nDip10, Height), new Point(nDip10, Height - SegmentHeight / 3.0));
                        }
                    }
                }//mark:50,150,250...
                double dMiddle = 100 * (dUnit + 0.5) - CountShift + offset;
                if (Marks == MarksLocation.Up)
                    drawingContext.DrawLine(p, new Point(dMiddle, 0 ), new Point(dMiddle, SegmentHeight * 2.0 / 3.0 ));
                else
                    drawingContext.DrawLine(p, new Point(dMiddle , Height), new Point(dMiddle, Height - SegmentHeight * 2.0 / 3.0));

                //mark:100,200,300...
                double dShift = d - CountShift + offset;
                if (dShift != offset)
                {
                    if (Marks == MarksLocation.Up)
                        drawingContext.DrawLine(p, new Point(dShift, 0), new Point(dShift, Height));
                    else
                        drawingContext.DrawLine(p, new Point(dShift, Height), new Point(dShift, 0));
                }
                
                if ((dUnit < DrawLength*this.Zoom +  CountShift / 100 ))//(dUnit != 0.0) &&
                {
                    double sNumber = Math.Round(n * 100 / this.Zoom);
                    string szRulerNumber = (sNumber).ToString(CultureInfo.CurrentCulture);
                    FormattedText ft = new FormattedText(
                         szRulerNumber,
                         CultureInfo.CurrentCulture,
                         FlowDirection.LeftToRight,
                         new Typeface("Arial"),
                         DipHelper.PtToDip(7),
                         new SolidColorBrush(Color.FromRgb(0x55,0x55,0x55)));
                    ft.SetFontWeight(FontWeights.Normal);
                    ft.TextAlignment = TextAlignment.Center;

                    //Draw text
                    if (Marks == MarksLocation.Up)
                        drawingContext.DrawText(ft, new Point(dShift + ft.Width / 2 + 2, Height - ft.Height));
                    else
                        drawingContext.DrawText(ft, new Point(dShift + ft.Width / 2 + 2, Height - SegmentHeight / 3.0 - ft.Height));//- SegmentHeight
                }
            }
        }

        /// <summary>
        /// Measures an instance during the first layout pass prior to arranging it.
        /// </summary>
        /// <param name="availableSize">A maximum Size to not exceed.</param>
        /// <returns>The maximum Size for the instance.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Unit == Unit.Cm)
            {
                return new Size(DipHelper.CmToDip(Length), Height);
            }
            else if (Unit == Unit.Inch)
            {
                return new Size(DipHelper.InchToDip(Length), Height);
            }
            else
            {
                return base.MeasureOverride(availableSize);
            }
        }
        #endregion
    }

    public enum Unit
    {
        DiP,// Dips,Divedie by 100pixes
        Cm,// the unit is Centimeter.
        Inch // The unit is Inch.
    };
    public enum MarksLocation
    {
        Up,
        Down
    }

    /// <summary>
    /// A helper class for DIP (Device Independent Pixels) conversion and scaling operations.
    /// </summary>
    public static class DipHelper
    {
        public static double MmToDip(double mm)
        {
            return CmToDip(mm / 10.0);
        }
        public static double CmToDip(double cm)
        {
            return (cm * 96.0 / 2.54);
        }

        public static double InchToDip(double inch)
        {
            return (inch * 96.0);
        }
        public static double DipToInch(double dip)
        {
            return dip / 96D;
        }
        public static double DipToCm(double dip)
        {
            return (dip * 2.54 / 96.0);
        }
        public static double DipToMm(double dip)
        {
            return DipToCm(dip) * 10.0;
        }

        /// <summary>
        /// Converts font points to DIP (Device Independant Pixels).
        /// </summary>
        /// <param name="pt">A font point value.</param>
        /// <returns>A DIP value.</returns>
        public static double PtToDip(double pt)
        {
            return (pt * 96.0 / 72.0);
        }

        /// <summary>
        /// Gets the system DPI scale factor (compared to 96 dpi).
        /// From http://blogs.msdn.com/jaimer/archive/2007/03/07/getting-system-dpi-in-wpf-app.aspx
        /// Should not be called before the Loaded event (else XamlException mat throw)
        /// </summary>
        /// <returns>A Point object containing the X- and Y- scale factor.</returns>
        private static Point GetSystemDpiFactor()
        {
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            Matrix m = source.CompositionTarget.TransformToDevice;
            return new Point(m.M11, m.M22);
        }

        private const double DpiBase = 96.0;

        /// <summary>
        /// Gets the system configured DPI.
        /// </summary>
        /// <returns>A Point object containing the X- and Y- DPI.</returns>
        public static Point GetSystemDpi()
        {
            Point sysDpiFactor = GetSystemDpiFactor();
            return new Point(
                 sysDpiFactor.X * DpiBase,
                 sysDpiFactor.Y * DpiBase);
        }

        /// <summary>
        /// Gets the physical pixel density (DPI) of the screen.
        /// </summary>
        /// <param name="diagonalScreenSize">Size - in inch - of the diagonal of the screen.</param>
        /// <returns>A Point object containing the X- and Y- DPI.</returns>
        public static Point GetPhysicalDpi(double diagonalScreenSize)
        {
            Point sysDpiFactor = GetSystemDpiFactor();
            double pixelScreenWidth = SystemParameters.PrimaryScreenWidth * sysDpiFactor.X;
            double pixelScreenHeight = SystemParameters.PrimaryScreenHeight * sysDpiFactor.Y;
            double formatRate = pixelScreenWidth / pixelScreenHeight;

            double inchHeight = diagonalScreenSize / Math.Sqrt(formatRate * formatRate + 1.0);
            double inchWidth = formatRate * inchHeight;

            double xDpi = Math.Round(pixelScreenWidth / inchWidth);
            double yDpi = Math.Round(pixelScreenHeight / inchHeight);

            return new Point(xDpi, yDpi);
        }
        /// <summary>
        /// Converts a DPI into a scale factor (compared to system DPI).
        /// </summary>
        /// <param name="dpi">A Point object containing the X- and Y- DPI to convert.</param>
        /// <returns>A Point object containing the X- and Y- scale factor.</returns>
        public static Point DpiToScaleFactor(Point dpi)
        {
            Point sysDpi = GetSystemDpi();
            return new Point(
                 dpi.X / sysDpi.X,
                 dpi.Y / sysDpi.Y);
        }
        /// <summary>
        /// Gets the scale factor to apply to a WPF application
        /// so that 96 DIP always equals 1 inch on the screen (whatever the system DPI).
        /// </summary>
        /// <param name="diagonalScreenSize">Size - in inch - of the diagonal of the screen</param>
        /// <returns>A Point object containing the X- and Y- scale factor.</returns>
        public static Point GetScreenIndependentScaleFactor(double diagonalScreenSize)
        {
            return DpiToScaleFactor(GetPhysicalDpi(diagonalScreenSize));
        }
    }
}
