using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum SatisfyType
    {
        All,
        Any
    }

    public interface IInteractionCase : IUniqueObject, INamedObject
    {
        IInteractionEvent InteractionEvent { get; }

        // Case description, including condistions descriptions and actions descriptions.
        string Description { get; }

        // How evaluate condtions if there are two or more conditions, logical And or logical Or.
        SatisfyType SatisfyType { get; set; }

        // If all conditions in this case are satisfied.
        bool IsTrue { get; }

        IInteractionConditions Conditions { get; }

        IInteractionActions Actions { get; }

        IInteractionCondition CreateCondition();

        void DeleteCondition(Guid conditionGuid);

        void ClearAllConditions();

        IInteractionAction CreateAction(ActionType actionType);

        void DeleteAction(Guid actionGuid);

        bool MoveAction(Guid actionGuid, int delta);

        void AddAction(IInteractionAction action, int index);

        void ClearAllActions();

    }
}
