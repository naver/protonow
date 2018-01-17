using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    // EventType is unique in the collection. One type can only an event object.
    public interface IInteractionEvents : IEnumerable
    {
        IInteractionEvent GetEvent(EventType eventType);

        bool Contains(EventType eventType);

        int Count { get; }

        IInteractionEvent this[EventType eventType] { get; }
    }
}
