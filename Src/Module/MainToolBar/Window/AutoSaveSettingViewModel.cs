using MainToolBar.Common;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module
{
    public class AutoSaveSettingViewModel : ViewModelBase
    {
        private AutoSaveSettingWindow _autoSaveSettingWindow;
        public DelegateCommand<object> ChangeAutosaveCommand { get; private set; }
        public DelegateCommand<object> ChangeFolderCommand { get; private set; }

        public AutoSaveSettingViewModel(AutoSaveSettingWindow autoSaveSettingWindow)
        {
            // TODO: Complete member initialization
            this._autoSaveSettingWindow = autoSaveSettingWindow;
            this.LoadDefaultSetting();
            this.ChangeAutosaveCommand = new DelegateCommand<object>(ChangeAutosaveExecute);
            this.ChangeFolderCommand = new DelegateCommand<object>(ChangeFolderExecute);
        }





        private void LoadDefaultSetting()
        {
            IsAutoSaveEnable = GlobalData.IsAutoSaveEnable;
            IsKeepLastAutoSaved = GlobalData.IsKeepLastAutoSaved;
            AutoSaveTick = GlobalData.AutoSaveTick;
            AutoSaveFileLocation = GlobalData.AutoSaveFileLocation;
            if (string.IsNullOrEmpty(AutoSaveFileLocation))
            {
                var mydocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
                AutoSaveFileLocation = Path.Combine(mydocument, "protoNow");
            }

            try
            {
                if (!Directory.Exists(AutoSaveFileLocation))
                {
                    Directory.CreateDirectory(AutoSaveFileLocation);
                }
            }
            catch
            {
                var mydocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
                AutoSaveFileLocation = Path.Combine(mydocument, "protoNow");
            }
        }

        private bool _isAutoSaveEnable;
        public bool IsAutoSaveEnable
        {
            get
            {
                return _isAutoSaveEnable;
            }
            set
            {
                if (value != _isAutoSaveEnable)
                {
                    _isAutoSaveEnable = value;
                    FirePropertyChanged("IsAutoSaveEnable");
                    if (!value)
                    {
                        IsKeepLastAutoSaved = false;
                    }
                }
            }
        }

        private bool _isKeepLastAutoSaved;
        public bool IsKeepLastAutoSaved
        {
            get
            {
                return _isKeepLastAutoSaved;
            }
            set
            {
                if (value != _isKeepLastAutoSaved)
                {
                    _isKeepLastAutoSaved = value;
                    FirePropertyChanged("IsKeepLastAutoSaved");
                    if (value)
                    {
                        IsAutoSaveEnable = true;
                    }
                }
            }
        }

        private int _autoSaveTick;
        public int AutoSaveTick
        {
            get
            {
                return _autoSaveTick;
            }
            set
            {
                if (value != _autoSaveTick)
                {
                    _autoSaveTick = value;
                    FirePropertyChanged("AutoSaveTick");
                }
            }
        }

        private string _autoSaveFileLocation;
        public string AutoSaveFileLocation
        {
            get
            {
                return _autoSaveFileLocation;
            }
            set
            {
                if (value != _autoSaveFileLocation)
                {
                    _autoSaveFileLocation = value;
                    FirePropertyChanged("AutoSaveFileLocation");
                }
            }
        }

        private void ChangeAutosaveExecute(object parameter)
        {
            var strParameter = parameter.ToString();
            if (strParameter == "1")
            {
                if (AutoSaveTick < 1)
                {
                    AutoSaveTick = 1;
                }
                else if (AutoSaveTick > 1000)
                {
                    AutoSaveTick = 1000;
                }

                GlobalData.IsAutoSaveEnable = IsAutoSaveEnable;
                GlobalData.IsKeepLastAutoSaved = IsKeepLastAutoSaved;
                GlobalData.AutoSaveTick = AutoSaveTick;
                GlobalData.AutoSaveFileLocation = AutoSaveFileLocation;
                _ListEventAggregator.GetEvent<AutoSaveSettingChangedEvent>().Publish(null);
            }

            _autoSaveSettingWindow.Close();
        }

        private void ChangeFolderExecute(object obj)
        {
            var fbdiaglog = new System.Windows.Forms.FolderBrowserDialog();
            if (fbdiaglog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AutoSaveFileLocation = fbdiaglog.SelectedPath;
            }
        }
    }
}
