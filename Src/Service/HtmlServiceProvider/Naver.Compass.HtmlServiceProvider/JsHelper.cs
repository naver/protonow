using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Service.Html
{
    static class JsHelper
    {
        public static void RemoveLastComma(StringBuilder builder)
        {
            if (builder.Length > 0 && builder[builder.Length - 1] == ',')
            {
                builder.Length -= 1;
            }
        }

        public static string ReplaceNewLineWithBRTag(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            string result = value.Replace("\r\n", @"<br>");
            result = result.Replace("\r", @"<br>");
            result = result.Replace("\n", @"<br>");
            
            return result;
        }

        public static string ReplaceSpecialCharacters(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            string result = value.Replace("&", @"&amp;");
            result = result.Replace("\"", @"&quot;");
            result = result.Replace("'", @"&apos;");
            result = result.Replace("<", @"&lt;");
            result = result.Replace(">", @"&gt;");
            result = result.Replace(@"\", @"\\");
            return result;
        }

        public static string EscapeDoubleQuote(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Replace("\"", "\\\"");
        }

        public static string EscapeSingleQuote(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Replace("\'", "\\\'");
        }

        public static string GetRGBAColor(StyleColor styleColor)
        {
            return GetRGBAColor(styleColor.ARGB);
        }

        public static string GetRGBAColor(int argb)
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

        public static void AppendFillStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            switch (widgetStyle.FillColor.FillType)
            {
                case ColorFillType.Solid:
                    {
                        builder.Append("\"type\":\"solid\",");
                        builder.AppendFormat("\"color\":\"{0}\",", GetRGBAColor(widgetStyle.FillColor));
                        break;
                    }
                case ColorFillType.Gradient:
                    {
                        builder.Append("\"type\":\"linearGradient\",");
                        builder.AppendFormat("\"gradientRotate\":\"{0}\",", Math.Round(widgetStyle.FillColor.Angle));

                        builder.Append("\"gradientColors\": [");
                        foreach (KeyValuePair<double, int> frame in widgetStyle.FillColor.Frames.OrderBy(x => x.Key))
                        {
                            builder.Append("{");
                            builder.AppendFormat("\"offset\":\"{0}%\",", Math.Round(frame.Key * 100));
                            builder.AppendFormat("\"color\":\"{0}\"", GetRGBAColor(frame.Value));
                            builder.Append("},");
                        }
                        JsHelper.RemoveLastComma(builder);
                        builder.Append("],");

                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        public static void AppendBorderStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            // There is no border
            if (widgetStyle.LineWidth < 1)
            {
                return;
            }

            builder.AppendFormat("\"width\":\"{0}px\",", Math.Round(widgetStyle.LineWidth));

            switch(widgetStyle.LineStyle)
            {
                case LineStyle.Solid:
                    {
                        builder.Append("\"type\":\"1\",");
                        break;
                    }
                case LineStyle.Dot:
                    {
                        builder.Append("\"type\":\"2\",");
                        break;
                    }
                case LineStyle.DashDot:
                    {
                        builder.Append("\"type\":\"3\",");
                        break;
                    }
                case LineStyle.DashDotDot:
                    {
                        builder.Append("\"type\":\"4\",");
                        break;
                    }
                default:
                    {
                        builder.Append("\"type\":\"0\",");
                        break;
                    }
            }

            builder.AppendFormat("\"color\":\"{0}\",", JsHelper.GetRGBAColor(widgetStyle.LineColor));
        }


        public static void AppendFontStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            // Font Family
            builder.AppendFormat("\"font-family\":\"{0}\",", widgetStyle.FontFamily);

            // Font Size
            builder.AppendFormat("\"font-size\":\"{0}px\",", widgetStyle.FontSize);

            // Font Weight
            if (widgetStyle.Bold)
            {
                builder.Append("\"font-weight\":\"bold\",");
            }

            // Font Style
            if (widgetStyle.Italic)
            {
                builder.Append("\"font-style\":\"italic\",");
            }
        }

        public static void AppendSimpleTextStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            if (widgetStyle.VertAlign == Alignment.Center || widgetStyle.VertAlign == Alignment.Middle)
            {
                builder.Append("\"vertical-align\":\"middle\",");
            }
            else
            {
                builder.AppendFormat("\"vertical-align\":\"{0}\",", widgetStyle.VertAlign.ToString().ToLower());
            }

            // Text Alignment
            builder.AppendFormat("\"text-align\":\"{0}\",", widgetStyle.HorzAlign.ToString().ToLower());

            // Text Decoration
            string decoration = "\"text-decoration\":\"";
            if (widgetStyle.Strikethrough)
            {
                decoration += @" line-through ";
            }
            if (widgetStyle.Underline)
            {
                decoration += @" underline ";
            }
            if (widgetStyle.Overline)
            {
                decoration += @" overline ";
            }
            if (decoration != "\"text-decoration\":\"")
            {
                builder.Append(decoration + "\",");
            }

            // Text Color
            builder.AppendFormat("\"color\":\"{0}\",", GetRGBAColor(widgetStyle.FontColor));
        }


        public static void AppendRichTextStyle(StringBuilder builder, IWidgetStyle widgetStyle)
        {
            if (widgetStyle.VertAlign == Alignment.Center || widgetStyle.VertAlign == Alignment.Middle)
            {
                builder.Append("\"vertical-align\":\"middle\",");
            }
            else
            {
                builder.AppendFormat("\"vertical-align\":\"{0}\",", widgetStyle.VertAlign.ToString().ToLower());
            }

            // Text Alignment
            builder.AppendFormat("\"text-align\":\"{0}\",", widgetStyle.HorzAlign.ToString().ToLower());
        }

        public static string GetHtmlTextFromRichText(string richText)
        {
            try
            {
                string htmlText = HtmlFromXamlConverter.TransformXamlToHtml(richText);
                htmlText = htmlText.Replace(@"'", @"\'");
                htmlText = ReplaceNewLineWithBRTag(htmlText);
                return htmlText;
            }
            catch(Exception exp)
            {
                System.Diagnostics.Debug.WriteLine("Convert rich text to html text error : " + exp.ToString());
            }

            return string.Empty;
        }
    }
}
