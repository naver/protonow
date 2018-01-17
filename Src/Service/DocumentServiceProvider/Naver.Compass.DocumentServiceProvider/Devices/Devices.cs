using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class Devices : XmlElementList<Device>, IDevices
    {
        internal Devices(DeviceSet set)
            : base("Devices")
        {
            _set = set;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            XmlNodeList childList = element.ChildNodes;
            if (childList == null || childList.Count <= 0)
            {
                return;
            }

            foreach (XmlElement childElement in childList)
            {
                Device device = new Device(_set, "");
                device.LoadDataFromXml(childElement);
                Add(device);
            }
        }

        #endregion

        #region IDevices

        public IDevice GetDevice(string deviceName)
        {
            return _list.FirstOrDefault(x => x.Name == deviceName);
        }

        public bool Contains(string deviceName)
        {
            return _list.Any(x => x.Name == deviceName);
        }

        public IDevice this[string deviceName] 
        {
            get { return GetDevice(deviceName); }
        }

        public int IndexOf(IDevice device)
        {
            return base.IndexOf(device as Device);
        }

        public IDeviceSet DeviceSet
        {
            get { return _set; }
        }

        #endregion

        private DeviceSet _set;
    }
}
