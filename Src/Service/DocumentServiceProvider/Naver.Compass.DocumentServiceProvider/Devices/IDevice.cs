using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IDevice : INamedObject
    {
        IDeviceSet DeviceSet { get; }

        bool IsVisible { get; set; }

        bool IsChecked { get; set; }

        string Description { get; }

        int Width { get; set; }

        int Height { get; set; }
    }
}
