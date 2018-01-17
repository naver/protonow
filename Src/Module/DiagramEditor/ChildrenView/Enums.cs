using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module
{
    enum ImageEditType
    {
        None,
        Slice,
        Crop
    }

    enum SliceType
    {
        Cross,
        Horizontal,
        Vertical,
    }

    public enum DeviceType
    {
        None,
        PCWeb,
        Mobile,
        Tablet,
        Watch
    }


    /// <summary>
    /// if DeviceType is PCWeb or Tablet, ViewType is Landscape,else Portait
    /// </summary>
    public enum ViewType
    {
        Portait,
        Landscape
    }
}
