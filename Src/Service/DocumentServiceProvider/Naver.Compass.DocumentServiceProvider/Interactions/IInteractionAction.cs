using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum ActionType
    {
        None,

        // Action to open.
        OpenAction,

        // Action to close.
        CloseAction,

        // Action to show or hide widgets
        ShowHideAction,

        // Action to move widgets
        MoveAction,

        // which is relevant to varaibles.
        VariableAction,
    }

    public interface IInteractionAction : IUniqueObject
    {
        IInteractionCase InteractionCase { get; }

        ActionType ActionType { get; }
    }
}
