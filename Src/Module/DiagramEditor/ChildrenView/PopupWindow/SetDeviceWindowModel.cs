using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module
{
    class SetDeviceWindowModel:ViewModelBase
    {
        public SetDeviceWindowModel()
        {
            _model = DeviceModel.GetInstance();
            _model.ResetData();
            this.OKCommand = new DelegateCommand<Window>(OKExecute, CanOKExecute);
            this.CancelCommand = new DelegateCommand<Window>(CancelExecute);
        }

        #region Commands and functions.
        public DelegateCommand<Window> OKCommand { get; private set; }
        public DelegateCommand<Window> CancelCommand { get; private set; }

        public void CancelExecute(Window win)
        {
            if (win != null)
            {
                win.DialogResult = false;
                win.Close();
            }
        }
        private void OKExecute(Window win)
        {
            if (win != null)
            {
                win.DialogResult = true;
                var selNode = DeviceList.FirstOrDefault(a => a.IsChecked);
                if (selNode != null)
                {
                    if (selNode.ViewType == Module.ViewType.Portait)
                    {
                        if (selNode.Type == DeviceType.PCWeb || selNode.Type == DeviceType.Tablet)
                        {
                            Swap(selNode);
                        }
                    }
                    else
                    {
                        if (selNode.Type == DeviceType.Mobile || selNode.Type == DeviceType.Watch)
                        {
                            Swap(selNode);
                        }
                    }
                }
                
                //UserSetting is checked 
                //if user input a name, add it as a new device, else as usersetting
                //Current UserSetting will be added as another device, and add default userSetting again.
                if(UserSetting.IsChecked)
                {
                    if (!string.IsNullOrEmpty(_name))
                    {
                        var device = _document.DeviceSet.CreateDevice(_name);
                        device.IsChecked = true;
                        device.Width = _width;
                        device.Height = _height;
                        DeviceList.Insert(DeviceList.Count()-2, new DeviceNode(device));
                        UserSetting.IsChecked = false;
                    }
                    else
                    {
                        UserSetting.Width = _width;
                        UserSetting.Height = _height;
                    }

                }
                win.Close();
            }
        }
        private bool CanOKExecute(object obj)
        {
            var node = _model.DeviceList.FirstOrDefault(a => a.IsChecked);
            if (node != null)
            {
                if(node.Name == CommonDefine.UserSetting)
                {
                    if (Width == 0 || Height == 0)
                        return false;
                }
                return true;
            }                
            else 
                return false;
        }


        private void Swap(DeviceNode node)
        {
            int item = node.Width;
            node.Width = node.Height;
            node.Height = item;
        }

        public void FreshOKCommandd()
        {
            OKCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Property


        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    FirePropertyChanged("Name");
                }
            }
        }
        //Binding with Width Edit-box
        public int Width
        {
            get
            {                
                return _width;
            }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    FirePropertyChanged("Width");
                }
            }
        }

        //Binding with Height Edit-box
        public int Height
        {
            get
            {
                    return _height;
            }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    FirePropertyChanged("Height");
                }
            }
        }

        public DeviceType DeviceType
        {
            get
            {
                return _model.CheckedDeviceType;
            }
            set
            {
                if (_model.CheckedDeviceType != value)
                {
                    _model.CheckedDeviceType = value;

                    //Uncheck other device when check a new DeviceType
                    var node = _model.DeviceList.FirstOrDefault(a => a.IsChecked == true&&a.Name != CommonDefine.UserSetting&&a.Name != CommonDefine.PresetOff);
                    if(node!=null)
                    {
                        node.IsChecked = false;
                    }
                    FreshOKCommandd();
                    FirePropertyChanged("DeviceType");
                    FirePropertyChanged("DeviceList");
                }
            }
        }
        public ObservableCollection<DeviceNode> DeviceList
        {
            get
            {
                return new ObservableCollection<DeviceNode>(_model.DeviceList.Where(a => a.Name != CommonDefine.PresetOff && a.Name != CommonDefine.UserSetting));
            }
        }

        public DeviceNode UserSetting
        {
            get
            {
                var node = _model.DeviceList.FirstOrDefault(a => a.Name == CommonDefine.UserSetting);
                return node;
            }
        }

        public DeviceNode NoSetting
        {
            get
            {
                var node = _model.DeviceList.FirstOrDefault(a => a.Name == CommonDefine.PresetOff);
                return node;
            }
        }

        private IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc != null)
                    return doc.Document;
                return null;
            }
        }

        private DeviceModel _model;
        private string _name;
        private int _width;
        private int _height;

        #endregion
    }
}
