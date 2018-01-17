using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.Service
{
    public interface IFormatPainterService
    {
        void SetPaintFormat(WidgetViewModelDate wdg);

        PaintFormat GetPaintFormat();

        bool GetMode();

        void SetMode(bool val);
    }
}
