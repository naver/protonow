using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Naver.Compass.Service.Update;
using Naver.Compass.Common.CommonBase;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace Naver.Compass.Module
{
    class CheckUpdateViewModel:ViewModelBase
    {
        public CheckUpdateViewModel()
        {
            _ListEventAggregator.GetEvent<CheckUpdateCompletedEvent>().Subscribe(CheckUpdateCompletedHandler, ThreadOption.UIThread);

            this.CloseCommand = new DelegateCommand<Window>(CloseExecute);
            this.UpdateCommand = new DelegateCommand<Window>(UpdateExecute, CanDoUpdate);
            this.CancelCommand = new DelegateCommand<Window>(CancelExecute);

            _updateService = ServiceLocator.Current.GetInstance<IUpdateService>();
            if (_updateService.IsAutoCheckUpdate && _updateService.UpdateInfo.HasError == false && _updateService.UpdateInfo.NeedToUpdate)
            {
                IsUpdateVisibility = Visibility.Visible;
            }
        }

        protected override void OnDispose()
        {
            _ListEventAggregator.GetEvent<CheckUpdateCompletedEvent>().Unsubscribe(CheckUpdateCompletedHandler);
        }

        public DelegateCommand<Window> CloseCommand { get; private set; }
        public DelegateCommand<Window> UpdateCommand { get; private set; }
        public DelegateCommand<Window> CancelCommand { get; private set; }
        
        private void CloseExecute(Window win)
        {
            win.Close();
        }
        private void UpdateExecute(Window win)
        {
            win.Close();
            _updateService.Update();
        }

        private bool CanDoUpdate(object parameter)
        {
            if (_updateService.UpdateInfo.HasError == false && _updateService.UpdateInfo.NeedToUpdate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CancelExecute(Window win)
        {
            win.Close();
        }

        public string VersionInfo
        {
            get
            {
                // Display version format is x.x.x
                string productVersion = _updateService.CurrentVersion;
                if (String.IsNullOrEmpty(productVersion))
                {
                    return String.Empty;
                }
                else
                {
                    productVersion = productVersion.Substring(0, productVersion.LastIndexOf("."));
                }
                return "  " + productVersion;
            }
        }

        public string UpdatesContent
        {
            get
            {
                if (_updateService.UpdateInfo.HasError == false)
                {
                    if (_updateService.UpdateInfo.NeedToUpdate)
                    {
                        return _updateService.UpdateInfo.TargetDescription;
                    }
                    else
                    {
                        return GlobalData.FindResource("Update_No_Update");
                    }
                }

                return _updateService.UpdateInfo.Message;
            }
        }
        
        public string UpdateBrief
        {
            get
            {
                if (_updateService.UpdateInfo.HasError == false && _updateService.UpdateInfo.NeedToUpdate)
                {
                    return _updateService.UpdateInfo.TargetTitle;
                }

                return String.Empty;
            }
        }

        public bool StartUpUpdate
        {
            get 
            {
                return _updateService.CheckUpdateAtStart;
            }
            set
            {
                _updateService.CheckUpdateAtStart = value;
            }
        }

        public Visibility IsCheckingVisibility
        {
            get
            {
                return (IsUpdateVisibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility IsUpdateVisibility
        {
            get
            {
                return isUpdateVisibility;
            }
            set
            {
                if (isUpdateVisibility != value)
                {
                    isUpdateVisibility = value;
                    FirePropertyChanged("IsCheckingVisibility");
                    FirePropertyChanged("IsUpdateVisibility");
                }
            }
        }

        public void CheckUpdateCompletedHandler(string parameter)
        {
            if (_updateService.IsAutoCheckUpdate == false)
            {
                IsUpdateVisibility = Visibility.Visible;
            }
            else
            {
                if (_updateService.UpdateInfo.HasError == false && _updateService.UpdateInfo.NeedToUpdate)
                {
                    IsUpdateVisibility = Visibility.Visible;
                }
            }

            FirePropertyChanged("IsCheckingVisibility");
            FirePropertyChanged("IsUpdateVisibility");
            FirePropertyChanged("UpdatesContent");
            FirePropertyChanged("StartUpUpdate");
            FirePropertyChanged("UpdateBrief");

            UpdateCommand.RaiseCanExecuteChanged();
        }

        #region Private

        private IUpdateService _updateService;
        private Visibility isUpdateVisibility = Visibility.Collapsed;

        #endregion
    }
}
