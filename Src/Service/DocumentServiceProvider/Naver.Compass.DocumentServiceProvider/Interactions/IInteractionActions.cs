using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public interface IInteractionActions : IEnumerable
    {
        IInteractionAction GetAction(Guid actionGuid);

        bool Contains(Guid actionGuid);

        int Count { get; }

        IInteractionAction this[Guid actionGuid] { get; }
    }
}
