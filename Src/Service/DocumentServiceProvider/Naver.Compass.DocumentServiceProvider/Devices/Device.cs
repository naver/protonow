using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class Device : XmlElementObject, IDevice
    {
        internal Device(DeviceSet set, string name)
            : base("Device")
        {
            _set = set;
            _name = name;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadBoolFromChildElementInnerText("IsVisible", element, ref _isVisible);
            LoadBoolFromChildElementInnerText("IsChecked", element, ref _isChecked);
            LoadStringFromChildElementInnerText("Name", element, ref _name);
            LoadIntFromChildElementInnerText("Width", element, ref _width);
            LoadIntFromChildElementInnerText("Height", element, ref _height);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            SaveStringToChildElement("IsVisible", _isVisible.ToString(), xmlDoc, element);
            SaveStringToChildElement("IsChecked", _isChecked.ToString(), xmlDoc, element);
            SaveStringToChildElement("Name", _name, xmlDoc, element);
            SaveStringToChildElement("Width", _width.ToString(), xmlDoc, element);
            SaveStringToChildElement("Height", _height.ToString(), xmlDoc, element);
        }

        #endregion

        #region INamedObject

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region IDevice

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        public string Description
        {
            get
            {
                return String.Format(@"{0}({1} x {2})", _name,
                                    _width == 0 ? "any" : _width.ToString(),
                                    _height == 0 ? "any" : _height.ToString());
            }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public IDeviceSet DeviceSet
        {
            get { return _set; }
        }

        #endregion

        #region Private Fields

        private DeviceSet _set;
        private bool _isVisible = true;
        private bool _isChecked = false;
        private string _name;
        private int _width;
        private int _height;

        #endregion
    }
}
