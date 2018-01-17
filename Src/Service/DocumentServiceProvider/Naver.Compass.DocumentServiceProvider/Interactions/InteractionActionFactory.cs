using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    internal class InteractionActionFactory
    {
        internal static InteractionAction CreateAction(IInteractionCase interactionCase, ActionType type)
        {
            switch(type)
            {
                case ActionType.OpenAction:
                    {
                        return new InteractionOpenAction(interactionCase);
                    }
                case ActionType.CloseAction:
                    {
                        return new InteractionCloseAction(interactionCase);
                    }
                case ActionType.ShowHideAction:
                    {
                        return new InteractionShowHideAction(interactionCase);
                    }
                case ActionType.MoveAction:
                    {
                        return new InteractionMoveAction(interactionCase);
                    }
                default:
                    break;
            }

            return null;
        }
    }
}
