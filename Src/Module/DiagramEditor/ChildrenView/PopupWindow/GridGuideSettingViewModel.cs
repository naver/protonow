using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module
{
    public enum SettingType
    {
        Grid,
        Guides,
        OjectSnap
    }
    class GridGuideSettingViewModel:ViewModelBase
    {
        public GridGuideSettingViewModel(SettingType type)
        {
            _settingType = type;
            _model = SettingModel.GetInstance();
            _model.InitData();
            this.OKCommand = new DelegateCommand<object>(OKExecute);
            this.CancelCommand = new DelegateCommand<object>(CancelExecute);
        }

        public DelegateCommand<object> OKCommand { get; set; }
        public DelegateCommand<object> CancelCommand { get; set; }

        private void OKExecute(object obj)
        {
            _model.Save();
            _ListEventAggregator.GetEvent<UpdateGridGuide>().Publish(GridGuideType.All);

            Window win = obj as Window;
            win.DialogResult = true;
            win.Close();
        }
        private void CancelExecute(object obj)
        {
            Window win = obj as Window;
            win.DialogResult = false;
            win.Close();
        }

        public bool IsGridSelected
        {
            get
            {
                return (_settingType == SettingType.Grid);
            }
        }

        public bool IsGuidesSelected
        {
            get
            {
                return (_settingType == SettingType.Guides);
            }
        }
        public bool IsObjectSelected
        {
            get
            {
                return (_settingType == SettingType.OjectSnap);
            }
        }

        SettingModel _model;
        SettingType _settingType;
    }
}
