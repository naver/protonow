using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IMasters : IEnumerable<IMaster>
    {
        IMaster GetMaster(Guid masterGuid);

        bool Contains(Guid masterGuid);

        int Count { get; }

        IMaster this[Guid masterGuid] { get; }
    }
}
