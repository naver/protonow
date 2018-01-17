using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public interface IGuides : IEnumerable
    {
        IGuide GetGuide(Guid guideGuid);

        bool Contains(Guid guideGuid);

        int Count { get; }

        IGuide this[Guid guideGuid] { get; }
    }
}
