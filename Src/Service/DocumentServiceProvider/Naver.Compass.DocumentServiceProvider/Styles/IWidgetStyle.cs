using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Naver.Compass.Service.Document
{
    public interface IWidgetStyle : IRegionStyle
    {
        IWidget OwnerWidget { get; }

        new bool IsVisible { get; set; }
        new double X { get; set; }
        new double Y { get; set; }
        new double Height { get; set; }
        new double Width { get; set; }
        new int Z { get; set; }

        bool IsFixed { get; set; }
        double WidgetRotate { get; set; }
        double TextRotate { get; set; }
        string FontFamily { get; set; }
        double FontSize { get; set; }
        bool Bold { get; set; }
        bool Italic { get; set; }
        bool Underline { get; set; }
        bool Baseline { get; set; }
        bool Overline { get; set; }
        bool Strikethrough { get; set; }
        StyleColor FontColor { get; set; }
        TextMarkerStyle BulletedList { get; set; }
        StyleColor LineColor { get; set; }
        double LineWidth { get; set; }
        LineStyle LineStyle { get; set; }
        ArrowStyle ArrowStyle { get; set; }
        int CornerRadius { get; set; }
        StyleColor FillColor { get; set; }
        int Opacity { get; set; }
        Alignment HorzAlign { get; set; }
        Alignment VertAlign { get; set; }

        string MD5 { get; set; }
    }
}
