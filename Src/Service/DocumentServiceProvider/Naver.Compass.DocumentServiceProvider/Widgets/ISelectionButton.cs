using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum AlignButton
    {
        Left,
        Right
    }

    public interface ISelectionButton : IWidget
    {
        bool IsSelected { get; set; }
        AlignButton AlignButton { get; set; }
    }
}
