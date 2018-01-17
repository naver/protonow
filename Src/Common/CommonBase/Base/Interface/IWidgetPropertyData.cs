using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Common.CommonBase
{
    public interface IWidgetPropertyData
    {      
        //Basic Property
        double Left{get;set;}
        double Raw_Left { get; set; }
        double Top{get;set;}
        double Raw_Top { get; set; }
        double ItemWidth { get; set; }
        double ItemHeight { get; set; }
        int RotateAngle { get; set; }
        int TextRotate { get; set; }
        string ItemContent { get; set; }
        int ZOrder { get; set; }
        bool IsGroup { get; }
        Guid WidgetID {get;}
        Guid ParentID { get; set; }
        bool IsLocked { get; }
        bool IsActual { get; set; }
        bool IsTarget { get; set; }
        bool CanDragResize { get; }
    }
}
