using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum TextFieldType
    {
        Text,
        Password,
        Email,
        Number,
        Tel,
        Url,
        Search,
        File,
        Date,
        Month,
        Time,
        Null
    }

    public interface ITextField : ITextBase
    {
        TextFieldType TextFieldType { get; set; }
    }
}
