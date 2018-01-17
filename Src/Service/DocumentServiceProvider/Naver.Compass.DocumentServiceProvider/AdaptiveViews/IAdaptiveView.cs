using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum AdaptiveViewCondition
    {
        LessOrEqual,
        GreaterOrEqual,
    }

    public interface IAdaptiveView : IUniqueObject, INamedObject
    {
        IAdaptiveViewSet AdaptiveViewSet { get; }

        bool IsVisible { get; set; }

        bool IsChecked { get; set; }

        string Description { get; }

        AdaptiveViewCondition Condition { get; set; }

        int Width { get; set; }

        int Height { get; set; }

        IAdaptiveView ParentView { get; }

        IAdaptiveViews ChildViews { get; }
    }
}
