using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public interface IPageViews : IEnumerable<IPageView>
    {
        IPageView GetPageView(Guid viewGuid);

        bool Contains(Guid viewGuid);

        int Count { get; }

        IPageView this[Guid viewGuid] { get; }
    }
}
