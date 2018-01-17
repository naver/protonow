using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class DeviceSet : XmlElementObject, IDeviceSet
    {
        public DeviceSet(Document document)
            : base("DeviceSet")
        {
            _document = document;
            _devices = new Devices(this);
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            LoadBoolFromChildElementInnerText("IsVisible", element, ref _isVisible);

            XmlElement devicesElement = element[_devices.TagName];
            if (devicesElement != null)
            {
                _devices.LoadDataFromXml(devicesElement);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            SaveStringToChildElement("IsVisible", _isVisible.ToString(), xmlDoc, element);

            _devices.SaveDataToXml(xmlDoc, element);
        }

        #endregion

        #region IDeviceSet

        public IDocument ParentDocument
        {
            get { return _document; }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public IDevices Devices 
        {
            get { return _devices; }
        }

        public IDevice CreateDevice(string name)
        {
            Device device = new Device(this, name);
            _devices.Add(device);
            return device;
        }

        public void DeleteDevice(IDevice device)
        {
            Device deviceToDelete = device as Device;

            if (deviceToDelete != null)
            {
                _devices.Remove(deviceToDelete);
            }
        }

        public bool MoveDevice(string name, int delta)
        {
            Device device = _devices.GetDevice(name) as Device;
            if (device == null)
            {
                return false;
            }

            return _devices.MoveItem(device, delta);
        }
       
        public void Clear()
        {
            _devices.Clear();
        }

        #endregion 

        #region Private Fields

        private Document _document;
        private Devices _devices;
        private bool _isVisible = true;

        #endregion
    }
}
