using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naver.Compass.Service.Document
{
    public interface IGroups : IEnumerable<IGroup>
    {
        IGroup GetGroup(Guid groupGuid);

        bool Contains(Guid groupGuid);

        int Count { get; }

        IGroup this[Guid groupGuid] { get; }
    }
}
