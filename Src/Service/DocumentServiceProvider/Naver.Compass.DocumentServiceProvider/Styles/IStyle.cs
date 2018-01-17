using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naver.Compass.Service.Document
{
    public enum Alignment
    {
        None = -1,
        Left,
        Center,
        Right,
        Top,
        Bottom,
        Middle
    }

    public enum LineStyle
    {
        None,
        Solid,
        LongDash,
        MediumDash,
        ShortDash,
        DashDot,
        DashDotDot,
        LongDashShortDash,
        Dot
    }

    // Left + Right
    public enum ArrowStyle
    {
        Default,
        None,
        NoneArrow,
        ArrowNone,
        NoneOpen,
        OpenNone,
        NoneOval,
        OvalNone,
        NoneStealth,
        StealthNone,
        OpenArrow,
        ArrowOpen,
        OvalArrow,
        ArrowOval,
        StealthArrow,
        ArrowStealth,
        OvalOpen,
        OpenOval,
        StealthOpen,
        OpenStealth,
        StealthOval,
        OvalStealth,
        ArrowArrow,
        StealthStealth,
        OpenOpen
    }

    public interface IStyle : IEnumerable<IStyleProperty>
    {
        int Count { get; }

        bool Contains(string name);

        IStyleProperty GetStyleProperty(string name);

        IStyleProperty SetStyleProperty(string name, bool value);
        IStyleProperty SetStyleProperty(string name, double value);
        IStyleProperty SetStyleProperty(string name, int value);
        IStyleProperty SetStyleProperty(string name, string value);
        IStyleProperty SetStyleProperty(string name, StyleColor value);
        IStyleProperty SetStyleProperty<T>(string name, T value) // Set enum value style
            where T : struct, IConvertible;

        bool RemoveStyleProperty(string name);

        void Clear();
    }
}
