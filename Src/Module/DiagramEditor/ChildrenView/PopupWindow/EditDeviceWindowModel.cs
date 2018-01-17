using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
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
    class EditDeviceWindowModel:ViewModelBase
    {
        public EditDeviceWindowModel()
        {
            _model = DeviceModel.GetInstance();
            this.AddDeviceCommand = new DelegateCommand<object>(AddDeviceExecute);
            this.MoveUpCommand = new DelegateCommand<object>(MoveUpExecute, CanExecuteMoveUp);
            this.MoveDownCommand = new DelegateCommand<object>(MoveDownExecute, CanExecuteMoveDown);
            this.DeleteDeviceCommand = new DelegateCommand<object>(DeleteDeviceExecute, CanExecuteDeleteDevice);
            this.CloseWindowCommand = new DelegateCommand<Window>(CloseWindowExecute);
            this.ExchangeWidthHeightCommand = new DelegateCommand<object>(ExchangeWidthHeightExecute);
        }

        #region Commands and functions.
        public DelegateCommand<object> AddDeviceCommand { get; private set; }
        public DelegateCommand<object> MoveUpCommand { get; private set; }
        public DelegateCommand<object> MoveDownCommand { get; private set; }
        public DelegateCommand<object> DeleteDeviceCommand { get; private set; }
        public DelegateCommand<Window> CloseWindowCommand { get; private set; }
        public DelegateCommand<object> ExchangeWidthHeightCommand { get; private set; }

        private void AddDeviceExecute(object obj)
        {
            DeviceNode node = _model.AddDevice();
            node.IsNodeEditable = true;
            SelectValue = node;
            _document.IsDirty = true;
        }
        private void MoveUpExecute(object obj)
        {
            DeviceNode node = selectValue;
            _model.MoveUp(selectValue);
            SelectValue = node;
            _document.IsDirty = true;
        }
        public bool CanExecuteMoveUp(object obj)
        {
            return DeviceList.IndexOf(selectValue) > 0 && selectValue != null;
        }
        private void MoveDownExecute(object obj)
        {
            DeviceNode node = selectValue;
            _model.MoveDown(selectValue);
            SelectValue = node;
            _document.IsDirty = true;
        }
        private bool CanExecuteMoveDown(object obj)
        {
            return DeviceList.IndexOf(selectValue) < DeviceList.Count - 1 && selectValue != null;
        }

        private void DeleteDeviceExecute(object obj)
        {
            int index = DeviceList.IndexOf(selectValue);
            _model.Delete(selectValue);
            if (index > 0)
            {
                SelectValue = DeviceList.ElementAt(--index);
            }
            else
            {
                SelectValue = DeviceList.ElementAt(0);
            }
            _document.IsDirty = true;

        }
        public bool CanExecuteDeleteDevice(object obj)
        {
            return selectValue != null && DeviceList.Count > 1;
        }

        private void ExchangeWidthHeightExecute(object obj)
        {
            int item = EditableWidth;
            EditableWidth = EditableHeight;
            EditableHeight = item;
            _document.IsDirty = true;
        }
        public void CloseWindowExecute(Window win)
        {
            if (win != null)
            {
                win.Close();
            }
        }

        public void FixDeviceList()
        {
            if(!_model.DeviceList.Contains(_offNode))
            {
                _model.DeviceList.Insert(0, _offNode);
            }

            if(!_model.DeviceList.Contains(_customNode))
            {
                _model.DeviceList.Add(_customNode);
            }
        }

        #endregion

        #region Property

        //Binding with Width Edit-box
        public int EditableWidth
        {
            get
            {
                if (SelectValue != null)
                    return SelectValue.Width;
                else
                    return 0;
            }
            set
            {
                if (SelectValue.Width != value)
                {
                    SelectValue.Width = value;
                    FirePropertyChanged("EditableWidth");
                }
            }
        }

        //Binding with Height Edit-box
        public int EditableHeight
        {
            get
            {
                if (SelectValue != null)
                    return SelectValue.Height;
                else
                    return 0;
            }
            set
            {
                if (SelectValue.Height != value)
                {
                    SelectValue.Height = value;
                    FirePropertyChanged("EditableHeight");
                }
            }
        }

        public ObservableCollection<DeviceNode> DeviceList
        {
            get
            {
                //Without off/user setting
                if (_offNode == null || _customNode == null)
                {
                    var node = _model.DeviceList.FirstOrDefault(x => x.Name == CommonDefine.PresetOff);
                    if (node != null)
                    {
                        _offNode = node;
                        _model.DeviceList.Remove(node);
                    }
                    node = _model.DeviceList.FirstOrDefault(x => x.Name == CommonDefine.UserSetting);
                    if (node != null)
                    {
                        _model.DeviceList.Remove(node);
                        _customNode = node;
                    }
                }

                return _model.DeviceList;
            }
        }
        public IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc != null)
                    return doc.Document;
                return null;
            }
        }

        private DeviceNode selectValue;
        public DeviceNode SelectValue
        {
            get
            {
                return selectValue;
            }
            set
            {
                if (selectValue != value)
                {
                    //unselect old item
                    if(selectValue!=null)
                        selectValue.IsEditboxFocus = false;
                    selectValue = value;
                    //select new item
                    if (selectValue != null)
                        selectValue.IsEditboxFocus = true;
                    FirePropertyChanged("SelectValue");
                    RefreshCommands();
                    FirePropertyChanged("EditableWidth");
                    FirePropertyChanged("EditableHeight");
                    FirePropertyChanged("IsUserDefineEnabled");
                }
            }
        }

        public bool IsUserDefineEnabled
        {
            get
            {
                return (selectValue != null);
            }
        }

        private void RefreshCommands()
        {
            AddDeviceCommand.RaiseCanExecuteChanged();
            MoveUpCommand.RaiseCanExecuteChanged();
            MoveDownCommand.RaiseCanExecuteChanged();
            DeleteDeviceCommand.RaiseCanExecuteChanged();
        }

        private DeviceModel _model;
        private DeviceNode _offNode;
        private DeviceNode _customNode;

        #endregion
    }
}
