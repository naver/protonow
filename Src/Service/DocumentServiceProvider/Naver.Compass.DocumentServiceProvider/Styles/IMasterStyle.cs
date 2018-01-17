using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IMasterStyle : IRegionStyle
    {
        IMaster OwnerMaster { get; }

        new bool IsVisible { get; set; }
        new double X { get; set; }
        new double Y { get; set; }
        new int Z { get; set; }
        bool IsFixed { get; set; }
        string MD5 { get; set; }
    }
}
