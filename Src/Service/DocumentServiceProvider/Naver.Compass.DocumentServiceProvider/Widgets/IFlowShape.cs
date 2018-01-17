using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naver.Compass.Service.Document
{
    public enum FlowShapeType
    {
        None,
        Rectangle,
        StackedRectangle,
        RoundedRectangle,
        StackedRound,
        BeveledRectangle,
        Diamond,
        File,
        StackedFile,
        Bracket,
        Semicircle,
        Triangle,
        Trapezoid,
        Ellipse,
        Hexagon,
        Parallelogram,
        Actor,
        Database,
    }

    public interface IFlowShape : IWidget
    {
        FlowShapeType FlowShapeType { get; set; }
    }
}
