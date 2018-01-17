using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public interface IPages : IEnumerable<IDocumentPage>
    {
        IDocumentPage GetPage(Guid pageGuid);

        bool Contains(Guid pageGuid);

        int Count { get; }

        IDocumentPage this[Guid pageGuid] { get; }
    }
}
