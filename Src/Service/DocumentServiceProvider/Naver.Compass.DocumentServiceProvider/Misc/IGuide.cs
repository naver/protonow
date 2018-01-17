using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naver.Compass.Service.Document
{       
    public interface IGuide : IUniqueObject
    {
        Orientation Orientation { get; }
        double X { get; set; }
        double Y { get; set; }
        bool IsLocked { get; set; }
    }
}
