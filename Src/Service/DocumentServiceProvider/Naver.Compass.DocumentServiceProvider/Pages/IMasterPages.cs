using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IMasterPages : IEnumerable<IMasterPage>
    {
        IMasterPage GetPage(Guid pageGuid);

        bool Contains(Guid pageGuid);

        int Count { get; }

        IMasterPage this[Guid pageGuid] { get; }
    }
}
