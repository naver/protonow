using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Naver.Compass.Common.CommonBase;
using System.Diagnostics;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Practices.ServiceLocation;

namespace Naver.Compass.Module.PublishModel
{
    public interface IDifferView
    {
        object Current { get; set; }
        void ResetView();
        void UploadError(string errorMessage);
    }

    public class ProcessViewModel : ViewModelBase
    {
        public DelegateCommand<Object> CloseCommand { get; private set; }
        public DelegateCommand<Object> CopyCommand { get; private set; }

        public event EventHandler<CancelEventArgs> Closing;

        private string _sErrMsg = "";
        private string _sUrl = "";
        private string _sShortUrl = "";
        private bool _IsCloseEnable = false;

        public ProcessViewModel()
        {
            CloseCommand = new DelegateCommand<Object>(OnCloseClick);
            CopyCommand = new DelegateCommand<Object>(OnCopyClick);
        }

        public BusyIndicatorContext BusyIndicator
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BusyIndicatorContext>();
            }
        }

        public string sErrorMessage
        {
            get { return _sErrMsg; }
            set
            {
                if (String.Compare(_sErrMsg, Convert.ToString(value)) != 0)
                {
                    _sErrMsg = Convert.ToString(value);
                    FirePropertyChanged("sErrorMessage");
                }
            }
        }

        public string sUrl
        {
            get { return _sUrl; }
            set
            {
                if (String.Compare(_sUrl, Convert.ToString(value)) != 0)
                {
                    _sUrl = Convert.ToString(value);
                    FirePropertyChanged("sUrl");
                }
            }
        }

        public string sShortUrl
        {
            get { return _sShortUrl; }
            set
            {
                if (String.Compare(_sShortUrl, Convert.ToString(value)) != 0)
                {
                    _sShortUrl = Convert.ToString(value);
                    QRSource = _sShortUrl;
                    FirePropertyChanged("sShortUrl");
                    FirePropertyChanged("IsCopyVisibility");
                }
            }
        }

        public bool IsCloseEnable
        {
            get { return _IsCloseEnable; }
            set
            {
               if (value != _IsCloseEnable)
               {
                   _IsCloseEnable = value;
                   FirePropertyChanged("IsCloseEnable");
               }
            }
        }

        public string IsCopyVisibility
        {
            get
            {
                if (sShortUrl.Length > 0)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }

        double _height = 222;
        public double dHeight
        {
            get
            {
                return _height;
            }
            set
            {
                if (value != _height)
                {
                    _height = Convert.ToDouble(value);
                    FirePropertyChanged("dHeight");
                }
            }
        }

        string _sQRSource="";
        public object QRSource
        {
            get
            {
                if (_sQRSource.Length > 0)
                {
                    return _sQRSource;
                }
                return DependencyProperty.UnsetValue;
            }
            set
            {
                if (Convert.ToString(value).Length > 0)
                {
                    _sQRSource = CommonFunction.BuildQRUrl(Convert.ToString(value));

                    FirePropertyChanged("QRSource");
                }
            }
        }

        public void StartProgress()
        {
            BusyIndicator.IsShow = true;
        }

        public void StopProgress()
        {
            BusyIndicator.IsShow = false;
        }

        private void OnCloseClick(Object obj)
        {
            if(obj is Window)
            {
                (obj as Window).Close();
            }
            else
            {
                sShortUrl = string.Empty;

                CancelEventArgs arg = new CancelEventArgs(false);
                OnClosing(arg);
            }
             
        }

        private void OnCopyClick(Object obj)
        {
            if (sShortUrl.Length > 0)
            {
                try
                {
                    //Clipboard.Clear();
                    //Clipboard.SetText(sShortUrl);
                    Clipboard.SetDataObject(sShortUrl);
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return;
                }
            }
        }
        public void OnClosing(CancelEventArgs args)
        {
            if (Closing != null)
            {
                Closing(this, args);
            }
        }
    }   
}
