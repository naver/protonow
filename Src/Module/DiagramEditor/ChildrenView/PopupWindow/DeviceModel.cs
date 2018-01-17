using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module
{
    class DeviceModel
    {
        DeviceModel()
        {
            DeviceList = new ObservableCollection<DeviceNode>();
        }

        public static DeviceModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DeviceModel();
            }
            return _instance;
        }
        public ObservableCollection<DeviceNode> DeviceList { get; set; }
        public DeviceType CheckedDeviceType { get; set; }

        public void ResetData()
        {
            CheckedDeviceType = DeviceType.Mobile;
            LoadDevices();
            _document.IsDirty = false;
        }

        public void LoadDevices()
        {
            DeviceList.Clear();
            if (Devices.Count > 0)
            {
                foreach (IDevice item in Devices)
                {
                    if (item.Name == CommonDefine.UserSetting)
                        continue;
                    DeviceList.Add(new DeviceNode(item));
                }
                //User setting always be the last one in the devices menu.
                IDevice iUserSetting = Devices.GetDevice(CommonDefine.UserSetting);
                if (iUserSetting != null)
                    DeviceList.Add(new DeviceNode(iUserSetting));
            }
            else
            {
                LoadDefault();
            }
        }

        public DeviceNode AddDevice()
        {
            IDevice device = _deviceSet.CreateDevice(GetNoteName());
            DeviceNode node = new DeviceNode(device);
            DeviceList.Add(node);
            return node;
        }
        public void MoveUp(DeviceNode node)
        {
            int index = DeviceList.IndexOf(node);
            DeviceList.Remove(node);
            DeviceList.Insert(--index, node);
            _deviceSet.MoveDevice(node.Name, -1);
        }
        public void MoveDown(DeviceNode node)
        {
            int index = DeviceList.IndexOf(node);
            DeviceList.Remove(node);
            DeviceList.Insert(++index, node);
            _deviceSet.MoveDevice(node.Name, 1);
        }

        public void Delete(DeviceNode node)
        {
            DeviceList.Remove(node);
            _deviceSet.DeleteDevice(node.IDevice);
        }
        private void LoadDefault()
        {
            IDevice device = _deviceSet.CreateDevice(CommonDefine.PresetOff);
            DeviceList.Add(new DeviceNode(device));
            device = _deviceSet.CreateDevice(CommonDefine.PCRetina);
            device.Width = 2560;
            device.Height = 1600;
            DeviceList.Add(new DeviceNode(device, DeviceType.PCWeb));
            device = _deviceSet.CreateDevice(CommonDefine.PC1280);
            device.Width = 1280;
            device.Height = 800;
            DeviceList.Add(new DeviceNode(device, DeviceType.PCWeb));
            device = _deviceSet.CreateDevice(CommonDefine.PC1024);
            device.Width = 1024;
            device.Height = 768;
            DeviceList.Add(new DeviceNode(device, DeviceType.PCWeb));
            device = _deviceSet.CreateDevice(CommonDefine.IPhone6Plus);
            device.Width = 1242;
            device.Height = 2208;
            DeviceList.Add(new DeviceNode(device, DeviceType.Mobile));
            device = _deviceSet.CreateDevice(CommonDefine.IPhone6);
            device.Width = 750;
            device.Height = 1334;
            DeviceList.Add(new DeviceNode(device, DeviceType.Mobile));
            device = _deviceSet.CreateDevice(CommonDefine.IPhone5);
            device.Width = 640;
            device.Height = 1136;
            DeviceList.Add(new DeviceNode(device, DeviceType.Mobile));
            device = _deviceSet.CreateDevice(CommonDefine.GalaxyS6);
            device.Width = 1440;
            device.Height = 2560;
            DeviceList.Add(new DeviceNode(device, DeviceType.Mobile));
            device = _deviceSet.CreateDevice(CommonDefine.GalaxyS5);
            device.Width = 1080;
            device.Height = 1920;
            DeviceList.Add(new DeviceNode(device, DeviceType.Mobile));
            device = _deviceSet.CreateDevice(CommonDefine.GalaxyS3);
            device.Width = 720;
            device.Height = 1280;
            device.IsChecked = true;
            DeviceList.Add(new DeviceNode(device, DeviceType.Mobile));
            device = _deviceSet.CreateDevice(CommonDefine.LGOptimusG3);
            device.Width = 1440;
            device.Height = 2560;
            DeviceList.Add(new DeviceNode(device, DeviceType.Mobile));
            device = _deviceSet.CreateDevice(CommonDefine.GoogleNexus5);
            device.Width = 1080;
            device.Height = 1920;
            DeviceList.Add(new DeviceNode(device, DeviceType.Mobile));
            device = _deviceSet.CreateDevice(CommonDefine.IPadPro);
            device.Width = 2732;
            device.Height = 2048;
            DeviceList.Add(new DeviceNode(device, DeviceType.Tablet));
            device = _deviceSet.CreateDevice(CommonDefine.IpadMini2);
            device.Width = 2048;
            device.Height = 1536;
            DeviceList.Add(new DeviceNode(device, DeviceType.Tablet));
            device = _deviceSet.CreateDevice(CommonDefine.IpadMini1);
            device.Width = 1024;
            device.Height = 768;
            DeviceList.Add(new DeviceNode(device, DeviceType.Tablet));
            device = _deviceSet.CreateDevice(CommonDefine.GalaxyTabS2);
            device.Width = 2048;
            device.Height = 1536;
            DeviceList.Add(new DeviceNode(device, DeviceType.Tablet));
            device = _deviceSet.CreateDevice(CommonDefine.GalaxyTabA);
            device.Width = 1024;
            device.Height = 768;
            DeviceList.Add(new DeviceNode(device, DeviceType.Tablet));
            device = _deviceSet.CreateDevice(CommonDefine.GoogleNexus7);
            device.Width = 1824;
            device.Height = 1200;
            DeviceList.Add(new DeviceNode(device, DeviceType.Tablet));
            device = _deviceSet.CreateDevice(CommonDefine.Watch42mm);
            device.Width = 312;
            device.Height = 390;
            DeviceList.Add(new DeviceNode(device, DeviceType.Watch));
            device = _deviceSet.CreateDevice(CommonDefine.Watch38mm);
            device.Width = 272;
            device.Height = 340;
            DeviceList.Add(new DeviceNode(device, DeviceType.Watch));

            device = _deviceSet.CreateDevice(CommonDefine.UserSetting);
            DeviceList.Add(new DeviceNode(device));
        }

        public IDevices Devices
        {
            get
            {
                if (_document != null && _document.DeviceSet != null)
                    return _document.DeviceSet.Devices;
                return null;
            }
        }

        private IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        private IDeviceSet _deviceSet
        {
            get
            {
                if (_document != null)
                    return _document.DeviceSet;
                return null;
            }
        }

        private static DeviceModel _instance;
        private string DefaultNoteName
        {
            get { return String.Concat("Device ", _noteCounter++); }
        }

        private string GetNoteName()
        {
            string noteName = DefaultNoteName;

            if (DeviceList.Any(x => x.Name == noteName))
            {
                return GetNoteName();
            }

            return noteName;
        }
        private int _noteCounter = 1;

    }
}
