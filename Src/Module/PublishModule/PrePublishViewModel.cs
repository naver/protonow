using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using System.Net.NetworkInformation;
using Naver.Compass.Common.Helper;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Events;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service.Document;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;

namespace Naver.Compass.Module.PublishModel
{
    public class PrePublishViewModel : ViewModelBase
    {

        public PrePublishViewModel()
        {
            this.PublishCommand = new DelegateCommand<object>(OnPublishCommand);
            this.CancelComman = new DelegateCommand<object>(OnCancelCommand);
            this.PassWordChangedCommand = new DelegateCommand<object>(OnPassWordChangedCommand);
            this.LoadPasswordCommand = new DelegateCommand<object>(OnLoadPasswordCommand);
        }
        public PrePublishViewModel(Object para) : this()
        {
            _para = para as UploadParameter;
        }
        public DelegateCommand<object> PublishCommand { get; set; }
        public DelegateCommand<object> CancelComman { get; set; }
        public DelegateCommand<object> PassWordChangedCommand { get; set; }
        public DelegateCommand<object> LoadPasswordCommand { get; set; }

        public event EventHandler<CancelEventArgs> Closing;

        #region private member

        private UploadParameter _para;

        private int _ViewSelectedIndex = 0;

        #endregion

        #region functions
        public void Initwindow()
        {

            if (!String.IsNullOrEmpty(_para.id))
            {

                IsEnableOverProject = true;
                IsOverProject = true;

                WebShortUrl = _para.sShortURL;
                _para.sTime = _para.sTime.Replace('-', '.');
                UploadDate ="(" + GlobalData.FindResource("Publish_Window_TipDate") + " "+_para.sTime+")";

                if (_para.IsPublic)
                {
                    IsUseAccess = false;
                    ProjectPassword = "";
                }
                else
                {
                    IsUseAccess = true;
                    ProjectPassword = _para.ProjectPassword; ;
                }
            }
            else
            {
                IsUseAccess = false;
               // _projectPassword = CommonFunction.MakeRandomPassword(8);
                ProjectPassword = "";

                IsEnableOverProject = false;
                IsOverProject = false;
            }

        }

        public void OnClosing(CancelEventArgs args)
        {
            if (Closing != null)
            {
                Closing(this, args);
            }
        }

        public void ShowErrorMessage(string message)
        {
            IsErrorMsgVisible = true;
            ErrorMessage = message;
        }

        #endregion

        #region Binding member

        private string _projectPassword = "";
        public string ProjectPassword
        {
            get
            {
                return _projectPassword;
            }
            set
            {
                if (String.Compare(_projectPassword, value) != 0)
                {
                    _projectPassword = value;
                    FirePropertyChanged("ProjectPassword");
                }
            }
        }

        private bool _IsOverProject = false;
        public bool IsOverProject
        {
            get { 
                return _IsOverProject; 
            }
            set
            {
                if (_IsOverProject != value)
                {
                    _IsOverProject = value;

                    FirePropertyChanged("IsOverProject");
                }
            }
        }

        private bool _IsEnableOverProject = false;
        public bool IsEnableOverProject
        {
            get { return _IsEnableOverProject; }
            set
            {
                if (_IsEnableOverProject != value)
                {
                    _IsEnableOverProject = value;

                    FirePropertyChanged("IsEnableOverProject");
                }
            }
        }

        private string _sShortUrl = "";
        public string WebShortUrl
        {
            get
            {
                return _sShortUrl; 
            }
            set
            {
                if (_sShortUrl.CompareTo(value) != 0)
                {
                    _sShortUrl = Convert.ToString(value);

                    FirePropertyChanged("WebShortUrl");
                }
            }
        }

        private string _sUploadDate = "";
        public string UploadDate
        {
            get {
                return _sUploadDate; 
            }
            set
            {
                if (_sUploadDate.CompareTo(value) != 0)
                {
                    _sUploadDate = Convert.ToString(value);

                    FirePropertyChanged("UploadDate");
                }
            }
        }

        private bool _isUseAccess = false;
        public bool IsUseAccess
        {
            get { return _isUseAccess; }
            set
            {
                if (_isUseAccess != value)
                {
                    _isUseAccess = value;

                    FirePropertyChanged("IsUseAccess");
                    FirePropertyChanged("IsEnableUpload");
                }
            }
        }

        private bool _isErrorPassword = false;
        public bool IsErrorPassword
        {
            get { return _isErrorPassword; }
            set
            {
                if (_isErrorPassword != value)
                {
                    _isErrorPassword = value;

                    FirePropertyChanged("IsErrorPassword");
                }
            }
        }

        private bool _isEnableUpload = false;
        public bool IsEnableUpload
        {
            get
            {
                return (_isEnableUpload || (!IsUseAccess));
            }
            set
            {
                if (_isEnableUpload != value)
                {
                    _isEnableUpload = value;

                    FirePropertyChanged("IsEnableUpload");
                }
            }
        }

        public bool IsEnableResolution
        {
            get {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc != null && doc.Document != null)
                {
                   if (doc.Document.AdaptiveViewSet.AdaptiveViews.Count > 0)
                   {
                       return false;
                   }
                    
                }
                return true; 
            }
            
        }

        private bool _isErrorMsgVisible;
        public bool IsErrorMsgVisible
        {
            get
            {
                return _isErrorMsgVisible;
            }
            set
            {
                if (_isErrorMsgVisible != value)
                {
                    _isErrorMsgVisible = value;
                    FirePropertyChanged("IsErrorMsgVisible");
                }
            }
        }

        private string _errorMessage;
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
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if(_isEnabled!=value)
                {
                    _isEnabled = value;
                    FirePropertyChanged("IsEnabled");
                }
            }
        }

        #region Viewport Data

        public List<ViewPortData> ListData
        {
            get
            {
                List<ViewPortData> list = new List<ViewPortData>();

                ViewPortData defaultData = new ViewPortData();
                defaultData.Name = GlobalData.FindResource("Publish_Window_NoDevice");
                defaultData.Width = -1;
                defaultData.Height = -1;

                list.Insert(0, defaultData);

                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();


                if (doc != null && doc.Document != null)
                {
                    int index = 1;
                    
                    foreach (IDevice view in doc.Document.DeviceSet.Devices)
                    {
                        if (view.Height >= 0 && view.Width >= 0)
                        {
                            if (view.Name.Equals("Off"))
                            {
                                if (view.IsChecked)
                                {
                                    _ViewSelectedIndex = 0;//the default is 0.
                                }
                                continue;
                            }

                            ViewPortData data = new ViewPortData();

                            data.Name = view.Name;
                            data.Width = view.Width;
                            data.Height = view.Height;

                            if (view.IsChecked)
                            {
                                _ViewSelectedIndex = index;
                            }

                            list.Add(data);
                            index++;
                        }
                    }

                }

                return list;
            }
        }

        private int _selectIndex = 0;
        public int iSelectDevice
        {
            set
            {
                if (Convert.ToInt32(value) != _selectIndex)
                {
                    _selectIndex = Convert.ToInt32(value);
                    FirePropertyChanged("iSelectDevice");
                }
            }
            get
            {
                return _selectIndex;
            }
        }

        public ViewPortData SelectDevice
        {
            set
            {
                if (value is ViewPortData)
                {
                    ViewPortData date = (ViewPortData)value;

                    IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();

                    if (doc != null && doc.Document != null)
                    {
                        if (date.Height >= 0 && date.Width >= 0)
                        {
                            doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.Viewport.IncludeViewportTag = true;
                            doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.Viewport.Name = date.Name;
                            doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.Viewport.Height = date.Height;
                            doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.Viewport.Width = date.Width;
                        }
                        else
                        {
                            doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.Viewport.IncludeViewportTag = false;
                        }

                    }
                }
            }
        }

        #endregion

        #endregion

        #region Command Hanlder

        public void OnPublishCommand(object parameter)
        {
            string sProjectName = "";
            string sTempPath = "";


            if (_para.DocArray != null && _para.DocArray.Count == 2)
            {
                string p1 = _para.DocArray[0].Document.Title;
                string p2 = _para.DocArray[1].Document.Title;

                if (p1.CompareTo(p2) < 0)
                {
                    sProjectName = p1 + p2;
                }
                else
                {
                    sProjectName = p2 + p1;
                }
                sTempPath = _para.DocArray[0].Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.OutputFolder;
            }
            else
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc.Document != null)
                {
                    sProjectName = doc.Document.Title;
                    sTempPath = doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.OutputFolder;
                }
            }
      

            if (String.IsNullOrEmpty(sProjectName))
            {
                sProjectName = CommonDefine.Untitled;
            }

            if (String.IsNullOrEmpty(sTempPath))
            {
                sTempPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Compass\@Publish\" + sProjectName;
            }

            try
            {
                if (Directory.Exists(sTempPath + @"\images\") == false)
                {
                    Directory.CreateDirectory(sTempPath + @"\images\");
                }
            }
            catch(Exception )
            {
                System.Windows.MessageBox.Show(GlobalData.FindResource("Publish_Window_Path_Error"));
                return;
            }
           

            _para.IsNewProject = !IsOverProject;
            _para.ProjectPath = sTempPath;

            if (IsUseAccess)
            {
                _para.ProjectPassword = ProjectPassword;
            }
            else
            {
                _para.ProjectPassword = "";
            }


            if(parameter is PrePublish)
            {
                var window = parameter as PrePublish;
                window.DialogResult = true;
                window.Close();
            }
            else
            {
                CancelEventArgs arg = new CancelEventArgs(false);
                OnClosing(arg);
                IsEnabled = false;
            }
                   
        }

        private void OnCancelCommand(object parameter)
        {
            if (parameter is PrePublish)
            {
                var window = parameter as PrePublish;
                window.DialogResult = false;
                window.Close();
            }
            else
            {
                CancelEventArgs arg = new CancelEventArgs(true);
                OnClosing(arg);
            }
        }

        private void OnPassWordChangedCommand(object parameter)
        {
            IsErrorPassword = CommonFunction.IsErrorPassword(ProjectPassword);

            IsEnableUpload = !IsErrorPassword;
        }
        private void OnLoadPasswordCommand(object parameter)
        {
            IsEnableUpload = !CommonFunction.IsErrorPassword(ProjectPassword);
        }
        public void UpdateSelectedIndex()
        {
            iSelectDevice = _ViewSelectedIndex;
        }

        #endregion

       
    }

    public class ViewPortName : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string sReturn = "";
            if (values == null || values.Length != 3)
            {
                return sReturn;
            }

            if (System.Convert.ToInt32(values[1]) < 0 || System.Convert.ToInt32(values[2]) < 0)
            {
                return values[0].ToString();
            }
            else
            {
                sReturn = values[0].ToString() + @" (";
                if (System.Convert.ToInt32(values[1]) == 0)
                {
                    sReturn += "Any";
                }
                else
                {
                    sReturn += values[1].ToString();
                }

                sReturn += @" x ";

                if (System.Convert.ToInt32(values[2]) == 0)
                {
                    sReturn += "Any";
                }
                else
                {
                    sReturn += values[2].ToString();
                }

                sReturn += @")";
                return sReturn;
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
