using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Module
{
    public class DeviceNode : ViewModelBase
    {
        public DeviceNode(IDevice device, DeviceType type = DeviceType.None)
        {
            Type = type;

            _device = device;

            if (Type == DeviceType.PCWeb || Type == DeviceType.Tablet)
            {
                _viewType = Module.ViewType.Landscape;
            }
            //In case error:rename, duplicate, delete
            _preName = device.Name;

            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            _document = doc.Document;
        }
        public IDevice IDevice
        {
            get
            {
                return _device;
            }
        }

        public string Name
        {
            get
            {
                return _device.Name;
            }
            set
            {
                if (!String.IsNullOrEmpty(value) && _device.Name != value)
                {
                    _device.Name = value;
                    _document.IsDirty = true;

                    FirePropertyChanged("Name");
                }
            }
        }
        public string NameWidthSize
        {
            get
            {
                if (_device.Name == CommonDefine.PresetOff || _device.Name == CommonDefine.UserSetting)
                    return _device.Name;
                return string.Format("{0}({1}X{2})", _device.Name, _device.Width, _device.Height);
            }
            set
            {
                Name = value;
            }
        }

        public int Width
        {
            get
            {
                return _device.Width;
            }
            set
            {
                if (_device.Width != value)
                {
                    _device.Width = value;
                    FirePropertyChanged("Width");
                    _document.IsDirty = true;
                }
            }
        }

        public int Height
        {
            get
            {
                return _device.Height;
            }
            set
            {
                if (_device.Height != value)
                {
                    _device.Height = value;
                    FirePropertyChanged("Height");
                    _document.IsDirty = true;
                }
            }
        }

        public bool IsChecked
        {
            get
            {
                return _device.IsChecked;
            }
            set
            {
                if (_device.IsChecked != value)
                {
                    if(value == true)
                    {
                        foreach (var device in DeviceModel.GetInstance().DeviceList)
                        {
                            if (device.IDevice != _device)
                            {
                                device.IsChecked = false;
                            }
                        }
                    }
                    _device.IsChecked = value;
                    FirePropertyChanged("IsChecked");
                    _document.IsDirty = true;

                }
            }
        }

        bool isEditboxFocus;
        public bool IsEditboxFocus
        {
            get { return isEditboxFocus; }
            set
            {
                if (isEditboxFocus != value)
                {
                    isEditboxFocus = value;
                    FirePropertyChanged("IsEditboxFocus");
                }
            }
        }

        //binding to UI :be changed when click empty area on listbox 
        public bool IsNodeEditable
        {
            get { return _isNodeEditable; }
            set
            {
                if (_isNodeEditable != value)
                {
                    // Change to readonly.
                    if (value == false)
                    {
                        if (IsNameCollision(_device.Name))
                        {
                            MessageBox.Show(GlobalData.FindResource("PageNoteDialog_NameAlert"),
                                GlobalData.FindResource("PageNoteDialog_Warning"),
                                MessageBoxButton.OK);
                            Name = _preName;
                            return;
                        }
                        else
                        {
                            _preName = Name;
                        }
                    }

                    _isNodeEditable = value;
                    FirePropertyChanged("IsNodeEditable");
                }
            }
        }

        public bool IsVisible
        {
            get
            {
                return Type == DeviceModel.GetInstance().CheckedDeviceType;
            }
        }

        public ViewType ViewType
        {
            get
            {
                return _viewType;
            }
            set
            {
                if(_viewType!=value)
                {
                    _viewType = value;
                    FirePropertyChanged("ViewType");
                }
            }
        }

        public DeviceType Type { get; private set; }
        private bool IsNameCollision(string name)
        {
            DeviceNode node = DeviceModel.GetInstance().DeviceList.FirstOrDefault(x => x.Name == name);
            if ((node != null && node != this) || name == CommonDefine.PresetOff)
            {
                return true;
            }
            return false;
        }

        private ViewType _viewType;

        private IDevice _device;
       
        private IDocument _document;

        private bool _isNodeEditable;

        // This is a workaround for checking name collision. Remove this when we can set text after lost focus.
        private string _preName;
    }
}
