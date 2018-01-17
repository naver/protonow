using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.InfoStructure;
using Naver.Compass.Module.PublishModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Differ
{
    public class MainViewModel : ViewModelBase, IDifferView
    {
        public MainViewModel()
        {
            _current = _differVM = new DifferViewModel();
            CloseCommand = new DelegateCommand<object>(CloseExecute);
            OpenCommand = new DelegateCommand<object>(OpenExecute);

        }

        public DelegateCommand<object> CloseCommand { get; private set; }
        public DelegateCommand<object> OpenCommand { get; private set; }

        private object _differVM;
        private object _current;
        private bool _isShowErrorMesssage;
        private string _errorMessage;

        public object Current
        {
            get
            {
                return _current;
            }
            set
            {
                if (_current != value)
                {
                    _current = value;
                    FirePropertyChanged("Current");
                    FirePropertyChanged("IsOpenEnabled");
                }
            }
        }

        public object IsOpenEnabled
        {
            get
            {
                return _current is DifferViewModel;
            }
        }

        public bool IsShowErrorMessage
        {
            get
            {
                return _isShowErrorMesssage;
            }
            set
            {
                if(_isShowErrorMesssage!=value)
                {
                    _isShowErrorMesssage = value;
                    FirePropertyChanged("IsShowErrorMessage");
                }
            }
        }
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                if(_errorMessage!=value)
                {
                    _errorMessage = value;
                    FirePropertyChanged("ErrorMessage");
                }
            }
        }
        public BusyIndicatorContext BusyIndicator
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BusyIndicatorContext>();
            }
        }

        public void ResetView()
        {
            Current = _differVM;
        }

        private void CloseExecute(object parameter)
        {
            DifferViewModel diff = _differVM as DifferViewModel;
            {
                if(diff!=null)
                {
                    if(diff.DocArray[0]!=null && diff.DocArray[0].Document!=null)
                    {
                        diff.DocArray[0].Close();
                    }
                    if (diff.DocArray[1] != null&& diff.DocArray[1].Document!=null)
                    {
                        diff.DocArray[1].Close();
                    }

                    diff.ClearPreviewFolder();
                }
            }
            App.Current.Shutdown();
        }

        public void UploadError(string errorMessage)
        {
            if(Current is PrePublishViewModel)
            {
                (Current as PrePublishViewModel).ShowErrorMessage(errorMessage);
            }
        }
        private void OpenExecute(object parameter)
        {
            if (Current is DifferViewModel)
            {
                (Current as DifferViewModel).OpenProjects();
            }
        }
    }
}
