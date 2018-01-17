using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum ColorFillType
    {
        Solid,
        Gradient
    }

    [Serializable]
    public struct StyleColor
    {
        public StyleColor(ColorFillType fillType, int argb)
        {
            FillType = fillType;
            ARGB = argb;

            Angle = 90d;

            Frames = new Dictionary<double, int>();
            Frames[0] = -1; // -1 is 0xffffffff (White)
            Frames[1] = -16777216; // -16777216 is 0xff000000 (Black)
        }

        public StyleColor(StyleColor other)
        {
            FillType = other.FillType;
            ARGB = other.ARGB;
            Angle = other.Angle;

            Frames = new Dictionary<double, int>(other.Frames);
        }

        //public StyleColor(string colorString)
        //{
        //    // TODO : Convert string to StyleColor
        //}

        public StyleColor Clone()
        {
            StyleColor newColor = new StyleColor(this);
            return newColor;
        }

        public ColorFillType FillType;
        public int ARGB;

        // Properties for Gradient color fill type
        public double Angle;

        // Beware of deep copy:  = operator is not deep copy. Clone method or copy constructor is deep copy.
        public Dictionary<double, int> Frames;

        public override bool Equals(object obj)
        {
            if (!(obj is StyleColor))
            {
                return false;
            }

            var sc = (StyleColor)obj;
            if (sc.FillType != this.FillType)
            {
                return false;
            }

            if (sc.FillType == ColorFillType.Solid)
            {
                return sc.ARGB == this.ARGB;
            }
            else if (sc.FillType == ColorFillType.Gradient)
            {
                if (sc.Angle == this.Angle && sc.Frames.SequenceEqual(this.Frames))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // Return the color value in HTML service, change to CSS standard string value in future.
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            switch (FillType)
            {
                case ColorFillType.Solid:
                    {
                        builder.Append("\"type\":\"solid\",");
                        builder.AppendFormat("\"color\":\"{0}\",", GetRGBAColor(ARGB));
                        break;
                    }
                case ColorFillType.Gradient:
                    {
                        builder.Append("\"type\":\"linearGradient\",");
                        builder.AppendFormat("\"gradientRotate\":\"{0}\",", Math.Round(Angle));

                        builder.Append("\"gradientColors\": [");
                        foreach (KeyValuePair<double, int> frame in Frames.OrderBy(x => x.Key))
                        {
                            builder.Append("{");
                            builder.AppendFormat("\"offset\":\"{0}%\",", Math.Round(frame.Key * 100));
                            builder.AppendFormat("\"color\":\"{0}\"", GetRGBAColor(frame.Value));
                            builder.Append("},");
                        }
                        RemoveLastComma(builder);
                        builder.Append("],");

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return builder.ToString();
        }

        private string GetRGBAColor(int argb)
        {
            // Hightest btye is A 
            byte[] bytes = BitConverter.GetBytes(argb);
            byte aVal = bytes[3];
            byte rVal = bytes[2];
            byte gVal = bytes[1];
            byte bVal = bytes[0];

            double a = Convert.ToDouble(aVal);
            a /= 255;
            string rgba = String.Format("rgba({0},{1},{2},{3})", rVal, gVal, bVal, Math.Round(a, 2).ToString());

            return rgba;
        }

        private void RemoveLastComma(StringBuilder builder)
        {
            if (builder.Length > 0 && builder[builder.Length - 1] == ',')
            {
                builder.Length -= 1;
            }
        }
    }
}
