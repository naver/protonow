using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    /*
     * Set for device. The device name can be duplicated.
     * */
    public interface IDeviceSet
    {
        IDocument ParentDocument { get; }

        bool IsVisible { get; set; }

        IDevices Devices { get; }

        IDevice CreateDevice(string name);

        void DeleteDevice(IDevice device);

        bool MoveDevice(string name, int delta);

        void Clear();
    }
}
