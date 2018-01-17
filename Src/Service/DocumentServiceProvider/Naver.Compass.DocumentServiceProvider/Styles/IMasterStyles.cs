using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    internal interface IMasterStyles : IEnumerable<IMasterStyle>
    {
        IMasterStyle GetMasterStyle(Guid viewGuid);

        bool Contains(Guid viewGuid);

        int Count { get; }

        IMasterStyle this[Guid viewGuid] { get; }
    }
}
