using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum VisibilityType
    {
        None,
        Show,
        Hide,
        Toggle
    }

    public enum ShowHideAnimateType
    {
        None,
        Fade,
        SlideRight,
        SlideLeft,
        SlideUp,
        SlideDown
    }

    public interface IShowHideActionTarget : IUniqueObject
    {
        VisibilityType VisibilityType { get; set; }
        ShowHideAnimateType AnimateType { get; set; }
        int AnimateTime { get; set; }
    }

    public interface IInteractionShowHideAction : IInteractionAction
    {
        IShowHideActionTarget AddTargetObject(Guid widgetGuid);
        bool DeleteTagetObject(Guid widgetGuid);

        void SetAllVisibilityType(VisibilityType type);
        void SetAllAnimateType(ShowHideAnimateType type);
        void SetAllAnimateTime(int time);

        IShowHideActionTarget GetTarget(Guid widgetGuid);

        IShowHideActionTarget this[Guid widgetGuid] { get; }
        List<IShowHideActionTarget> TargetObjects { get; }
    }
}
