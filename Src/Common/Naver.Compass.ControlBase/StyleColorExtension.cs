using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Naver.Compass.Common
{
    public static class StyleColorExtension
    {
        public static System.Windows.Media.Brush ToBrush(this StyleColor sc)
        {
            if (sc.FillType == ColorFillType.Solid)
            {
                return new System.Windows.Media.SolidColorBrush(StyleColorExtension.FromArgb(sc.ARGB));
            }
            else if (sc.FillType == ColorFillType.Gradient)
            {
                var lineb = new System.Windows.Media.LinearGradientBrush();
                var newFrames = sc.Frames;
                if (newFrames == null || newFrames.Count == 0)
                {
                    newFrames = new Dictionary<double, int>();
                    newFrames[0] = -1;
                    newFrames[1] = -16777216;
                }
                foreach (var keypair in newFrames)
                {
                    var gradientStop = new System.Windows.Media.GradientStop(
                        StyleColorExtension.FromArgb(keypair.Value),
                        keypair.Key);
                    lineb.GradientStops.Add(gradientStop);
                }

                var aRotateTransform = new System.Windows.Media.RotateTransform();
                aRotateTransform.CenterX = 0.5;
                aRotateTransform.CenterY = 0.5;
                aRotateTransform.Angle = sc.Angle;
                lineb.RelativeTransform = aRotateTransform;
                return lineb;
            }

            return null;
        }

        private static System.Windows.Media.Color FromArgb(int argb)
        {
            var drawc = System.Drawing.Color.FromArgb(argb);
            return System.Windows.Media.Color.FromArgb(
                    drawc.A,
                    drawc.R,
                    drawc.G,
                    drawc.B);
        }

        public static System.Windows.Media.Brush ToBrush(this int argb)
        {
            return new System.Windows.Media.SolidColorBrush(FromArgb(argb));
        }

        public static System.Windows.Media.Color ToColor(this int argb)
        {
            return FromArgb(argb);
        }

        public static System.Windows.Media.Color ToColor(this StyleColor sc)
        {
            if (sc.FillType == ColorFillType.Solid)
            {
                return StyleColorExtension.FromArgb(sc.ARGB);
            }
            else if (sc.FillType == ColorFillType.Gradient)
            {
                var lineb = new System.Windows.Media.LinearGradientBrush();
                var newFrames = sc.Frames;
                if (newFrames == null || newFrames.Count == 0)
                {
                    newFrames = new Dictionary<double, int>();
                    newFrames[0] = -1;
                    newFrames[1] = -16777216;
                }

                return FromArgb(newFrames.First().Value);
            }

            return default(System.Windows.Media.Color);
        }

        public static int ToArgb(this System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B).ToArgb();
        }

        public static int ToArgb(this System.Windows.Media.Brush brush)
        {
            if (brush is SolidColorBrush)
            {
                return ToArgb((brush as SolidColorBrush).Color);
            }

            return -1;
        }

        public static StyleColor ToStyleColor(this System.Windows.Media.Brush brush)
        {
            return new StyleColor(ColorFillType.Solid, ToArgb(brush));
        }
    }
}
