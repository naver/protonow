using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface ITextBase : IWidget
    {
        string HintText { get; set; }
        int MaxLength { get; set; }
        bool HideBorder { get; set; }
        bool ReadOnly { get; set; }
    }
}
