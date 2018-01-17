using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum ShapeType
    {
        None,

        Rectangle,
        RoundedRectangle,
        Ellipse,
        Diamond,
        Triangle,

        Paragraph,
        H1,
        H2,
        H3,
        H4,
        H5,
        H6,
        Top,
        Bottom,
        Left,
        Right,
        TabLeft,
        TabRight,
        TabBottomLeft,
        TabBottomRight,
        Placeholder,
        ArrowButtonRight,
        ArrowButtonLeft,
        Drop,
        UpsideDownDrop,
        SquareBracket,
        CulyBracket,
        Star,
        Heart,
        Plus,
        ArrowRight,
        ArrowLeft,
        SpeechBubbleRight,
        SpeechBubbleLeft
    }

    public interface IShape : IWidget
    {
        ShapeType ShapeType { get; set; }
    }
}
