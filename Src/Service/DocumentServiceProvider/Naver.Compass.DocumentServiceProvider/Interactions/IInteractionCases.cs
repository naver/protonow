using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public interface IInteractionCases : IEnumerable
    {
        IInteractionCase GetCase(string caseName);

        IInteractionCase GetCase(Guid caseGuid);

        bool Contains(string caseName);

        bool Contains(Guid caseGuid);

        int Count { get; }

        IInteractionCase this[string caseName] { get; }

        IInteractionCase this[Guid caseGuid] { get; }
    }
}
