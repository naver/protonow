using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naver.Compass.Service.Document
{
    public enum EventType
    {
        None,
        OnClick,
        OnSelectionChange,
        OnTextChange,
        OnCheckedChange,

        OnMouseEnter,
        OnMouseOut,

        OnLoad,

        OnPageLoad

        // other support events....
    }

    public interface IInteractionEvent : IUniqueObject, INamedObject
    {
        IInteractiveObject InteractiveObject { get; }

        EventType EventType { get; }

        // Use "else if" to evaluate cases if there are multi cases.
        bool UseElseIf { get; set; }

        IInteractionCases Cases { get; }

        IInteractionCase CreateCase(string caseName);

        void DeleteCase(Guid caseGuid);

        bool MoveCase(Guid caseGuid, int delta);

        void AddCase(IInteractionCase interactionCase, int index);
    }
}
