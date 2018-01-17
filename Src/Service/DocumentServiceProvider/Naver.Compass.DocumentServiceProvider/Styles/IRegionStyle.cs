using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IRegionStyle : IViewStyle
    {
        IRegion OwnerRegion { get; }

        bool IsVisible { get; }
       
        double X { get; }
        double Y { get; }
        int Z { get; }
        
        double Height { get; }
        double Width { get; }

        double Rotate { get; }
    }
}
