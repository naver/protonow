using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public interface IDevices : IEnumerable
    {
        IDeviceSet DeviceSet { get; }

        IDevice GetDevice(string deviceName);

        bool Contains(string deviceName);

        int Count { get; }

        IDevice this[string deviceName] { get; }

        int IndexOf(IDevice device);
    }
}
