using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public interface IWidgets : IEnumerable<IWidget>
    {
        IWidget GetWidget(Guid widgetGuid);

        bool Contains(Guid widgetGuid);

        int Count { get; }

        IWidget this[Guid widgetGuid] { get; }
    }
}
