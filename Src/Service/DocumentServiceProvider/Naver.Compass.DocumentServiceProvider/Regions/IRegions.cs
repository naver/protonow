using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IRegions : IEnumerable<IRegion>
    {
        IRegion GetRegion(Guid regionGuid);

        bool Contains(Guid regionGuid);

        int Count { get; }

        IRegion this[Guid regionGuid] { get; }
    }
}
