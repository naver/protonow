using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IStyleProperty : INamedObject
    {
        string Value { get; set; }

        bool GetBooleanValue(bool defaultValue);
        double GetDoubleValue(double defaultValue);
        T GetEnumValue<T>(T defaultValue) where T : struct, IConvertible;
        int GetIntegerValue(int defaultValue);
        StyleColor GetStyleColorValue(StyleColor defaultValue);
    }
}
