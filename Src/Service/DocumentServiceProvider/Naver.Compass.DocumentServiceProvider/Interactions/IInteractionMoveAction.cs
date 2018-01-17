using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum MoveType
    {
        By,
        To
    }

    public enum MoveAnimateType
    {
        None,
        Swing,
        Linear,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        Bounce,
        Elastic
    }

    public interface IMoveActionTarget : IUniqueObject
    {
        MoveType MoveType { get; set; }
        double X { get; set; }
        double Y { get; set; }
        MoveAnimateType AnimateType { get; set; }
        int AnimateTime { get; set; }
    }

    public interface IInteractionMoveAction : IInteractionAction
    {
        IMoveActionTarget AddTargetObject(Guid widgetGuid);
        bool DeleteTagetObject(Guid widgetGuid);

        void SetAllMoveType(MoveType type);
        void SetAllX(double x);
        void SetAllY(double y);
        void SetAllAnimateType(MoveAnimateType type);
        void SetAllAnimateTime(int time);

        IMoveActionTarget GetTarget(Guid widgetGuid);

        IMoveActionTarget this[Guid widgetGuid] { get; }
        List<IMoveActionTarget> TargetObjects { get; }
    }
}
