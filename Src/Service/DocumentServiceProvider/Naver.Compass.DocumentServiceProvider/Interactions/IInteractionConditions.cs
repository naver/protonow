using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public interface IInteractionConditions : IEnumerable
    {
        IInteractionCondition GetCondition(Guid conditionGuid);

        bool Contains(Guid conditionGuid);

        int Count { get; }

        IInteractionCondition this[Guid conditionGuid] { get; }
    }
}
