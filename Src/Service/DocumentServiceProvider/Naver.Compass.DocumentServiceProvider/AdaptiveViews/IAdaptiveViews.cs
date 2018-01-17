using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public interface IAdaptiveViews : IEnumerable
    {
        IAdaptiveViewSet AdaptiveViewSet { get; }

        IAdaptiveView GetAdaptiveView(Guid viewGuid);

        bool Contains(Guid viewGuid);

        int Count { get; }

        IAdaptiveView this[Guid viewGuid] { get; }

        int IndexOf(IAdaptiveView view);
    }
}
