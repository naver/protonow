using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    internal interface IWidgetStyles : IEnumerable<IWidgetStyle>
    {
        IWidgetStyle GetWidgetStyle(Guid viewGuid);

        bool Contains(Guid viewGuid);

        int Count { get; }

        IWidgetStyle this[Guid viewGuid] { get; }
    }
}
