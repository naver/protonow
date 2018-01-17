using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Naver.Compass.Common;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Module.PublishModel;
using Naver.Compass.Service.Document;
using Naver.Compass.Service.WebServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Naver.Compass.Differ
{
    public class DifferViewModel: ViewModelBase
    {
        public DifferViewModel()
        {
            if (IsInDesignMode)
                return;
            ExportCommand = new DelegateCommand<object>(ExportExecute);
            PublishCommand = new DelegateCommand<object>(PublishExecute);
            BrowseCommand = new DelegateCommand<object>(BrowseExecute);
            DropFileCommand = new DelegateCommand<object>(DropFileExecute);
            ClearDocument = new DelegateCommand<object>(ClearDocumentExecute);
            DocArray = new List<IDocumentService>();
            DocArray.Add(new DocumentService());
            DocArray.Add(new DocumentService());
        }


        public DelegateCommand<object> ExportCommand { get; private set; }
        public DelegateCommand<object> PublishCommand { get; private set; }
        public DelegateCommand<object> BrowseCommand { get; private set; }
        public DelegateCommand<object> DropFileCommand { get; private set; }
        public DelegateCommand<object> ClearDocument { get; private set; }

        public List<IDocumentService> DocArray;

        private string _p1FileName;
        private string _p2FileName;

        private bool _isP1Opened = false;
        private bool _isP2Opened = false;

        private bool _isErrorMsgVisible;
        private string _errorMessage;


        public BusyIndicatorContext BusyIndicator
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BusyIndicatorContext>();
            }
        }

        public string P1Name
        {
            get
            {
                return GetFileName(_p1FileName);
            }
            set
            {
                if (_p1FileName != value)
                {
                    _p1FileName = value;
                    FirePropertyChanged("P1Name");
                }
            }
        }

        public string P2Name
        {
            get
            {
                return GetFileName(_p2FileName);
            }
            set
            {
                if (_p2FileName != value)
                {
                    _p2FileName = value;
                    FirePropertyChanged("P2Name");
                }
            }
        }

        public bool IsP1Opened
        {
            get
            {
                return _isP1Opened;
            }
            set
            {
                if(_isP1Opened!=value)
                {
                    _isP1Opened = value;
                    FirePropertyChanged("IsP1Opened");
                }
            }
        }

        public bool IsP2Opened
        {
            get
            {
                return _isP2Opened;
            }
            set
            {
                if (_isP2Opened != value)
                {
                    _isP2Opened = value;
                    FirePropertyChanged("IsP2Opened");
                }
            }
        }

        public bool IsPublishEnabled
        {
            get
            {
                return (DocArray[0] != null && DocArray[0].Document != null && DocArray[1] != null && DocArray[1].Document != null);
            }
        }
        public ImageSource P1Thumbnail
        {
            get
            {
                if (DocArray.Count < 1 || DocArray[0] == null || DocArray[0].Document == null)
                {
                    return null;
                }
                IDocument p1Docunment = DocArray[0].Document;
                if (p1Docunment != null && p1Docunment.Pages.Count > 0 && p1Docunment.Pages.First().Thumbnail != null)
                {
                    ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                    var imgSoruce = imageSourceConverter.ConvertFrom(p1Docunment.Pages.First().Thumbnail) as BitmapFrame;
                    double scale = (double)imgSoruce.PixelHeight / (double)imgSoruce.PixelWidth;
                    if (scale < 1)
                    {
                        ImageHeight1 = 102;
                        ImageWidth1 = Math.Round(102 / scale);
                    }
                    else
                    {
                        ImageWidth1 = 102;
                        ImageHeight1 = Math.Round(102 * scale);
                    }

                    return imgSoruce;
                }
                return null;
            }
        }
        public double _imgH1, _imgW1, _imgH2, _imgW2;
        public double ImageHeight1
        {
            get
            {
                return _imgH1;
            }
            set
            {
                if (_imgH1 != value)
                {
                    _imgH1 = value;
                    FirePropertyChanged("ImageHeight1");
                }
            }
        }
        public double ImageWidth1
        {
            get
            {
                return _imgW1;
            }
            set
            {
                if (_imgW1 != value)
                {
                    _imgW1 = value;
                    FirePropertyChanged("ImageWidth1");
                }
            }
        }
        public double ImageHeight2
        {
            get
            {
                return _imgH2;
            }
            set
            {
                if (_imgH2 != value)
                {
                    _imgH2 = value;
                    FirePropertyChanged("ImageHeight2");
                }
            }
        }
        public double ImageWidth2
        {
            get
            {
                return _imgW2;
            }
            set
            {
                if (_imgW2 != value)
                {
                    _imgW2 = value;
                    FirePropertyChanged("ImageWidth2");
                }
            }
        }
        public ImageSource P2Thumbnail
        {
            get
            {
                if (DocArray.Count < 2 || DocArray[1] == null || DocArray[1].Document == null)
                {
                    return null;
                }
                IDocument p2Docunment = DocArray[1].Document;

                if (p2Docunment != null && p2Docunment.Pages.Count > 0 && p2Docunment.Pages.First().Thumbnail != null)
                {
                    ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                    var imgSoruce = imageSourceConverter.ConvertFrom(p2Docunment.Pages.First().Thumbnail) as BitmapFrame;
                    double scale = (double)imgSoruce.PixelHeight / (double)imgSoruce.PixelWidth;
                    if (scale < 1)
                    {
                        ImageHeight2 = 102;
                        ImageWidth2 = Math.Round(102 / scale);
                    }
                    else
                    {
                        ImageWidth2 = 102;
                        ImageHeight2 = Math.Round(102 * scale);
                    }
                    return imgSoruce;
                }
                return null;
            }
        }

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

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    FirePropertyChanged("ErrorMessage");
                }
            }
        }
        public void OpenProjects()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = true;
            openFile.Filter = "protoNow Files (*.pn)|*.pn";
            if (openFile.ShowDialog() == true)
            {
                if (openFile.FileNames.Count() == 1)
                {
                    if (!string.IsNullOrEmpty(P1Name) && string.IsNullOrEmpty(P2Name))
                    {
                        OpenP2(openFile.FileNames[0]);
                    }
                    else
                    {
                        OpenP1(openFile.FileNames[0]);
                    }
                }
                else if (openFile.FileNames.Count() > 1)
                {
                    OpenP1(openFile.FileNames[0]);
                    OpenP2(openFile.FileNames[1]);
                }

                FirePropertyChanged("IsPublishEnabled");
            }
        }

        private void BrowseExecute(object parameter)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "protoNow Files (*.pn)|*.pn";
            if (openFile.ShowDialog() == true)
            {
                if (parameter.ToString() == "1")
                {
                    OpenP1(openFile.FileName);
                }
                else
                {
                    OpenP2(openFile.FileName);
                }
                FirePropertyChanged("IsPublishEnabled");
            }
        }

        private void DropFileExecute(object parameter)
        {
            var paramters = parameter as object[];
            var cmdArgs = paramters[0].ToString();
            var eventArgs = paramters[1] as DragEventArgs;

            if (eventArgs != null && eventArgs.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])eventArgs.Data.GetData(DataFormats.FileDrop);
                if (files.Count() == 1)
                {
                    if (cmdArgs == "1")
                    {
                        OpenP1(files[0]);
                    }
                    else
                    {
                        OpenP2(files[0]);
                    }
                }
                else if (files.Count() > 1)
                {
                    OpenP1(files[0]);
                    OpenP2(files[1]);
                }
                FirePropertyChanged("IsPublishEnabled");

                eventArgs.Handled = true;
            }
        }

        private void ClearDocumentExecute(object parameter)
        {
            var paramters = parameter as object[];
            if (paramters[0].ToString() == "1")
            {
                if (DocArray[0] != null && DocArray[0].Document != null)
                {
                    DocArray[0].Close();
                    P1Name = string.Empty;
                    IsP1Opened = false;
                    FirePropertyChanged("P1Thumbnail");
                }
            }
            else
            {
                if (DocArray[1] != null && DocArray[1].Document != null)
                {
                    DocArray[1].Close();
                    P2Name = string.Empty;
                    IsP2Opened = false;
                    FirePropertyChanged("P2Thumbnail");
                }
            }

            var eventArgs = paramters[1] as MouseButtonEventArgs;
            eventArgs.Handled = true;

            FirePropertyChanged("IsPublishEnabled");
        }

        private void ExportExecute(object parameter)
        {
            try
            {
                // Save html files in temp folder as this is Preview html. 
                // IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();

                //Get save path from project file, it's redundancy now..
                string _previewPath = CommonFunction.GetPreViewTempPath();

                if (String.IsNullOrEmpty(_previewPath))
                {
                    IWebServer httpServer = ServiceLocator.Current.GetInstance<IWebServer>();
                    if (httpServer.StartWebService())
                    {
                        _previewPath = CommonFunction.GetPreViewTempPath();
                    }
                    else
                    {
                        return;//if webservice error,cancel preview
                    }
                }

                if (Directory.Exists(_previewPath) == false)
                {
                    Directory.CreateDirectory(_previewPath);

                    string imageOutPutPath = _previewPath + @"\images";
                    Directory.CreateDirectory(imageOutPutPath);
                }
                DiffGeneratorParameter para = new DiffGeneratorParameter();
                para.SavePath = _previewPath;
                //para.Docs = DocArray;

                if(DocArray[0].Document!=null && DocArray[1].Document!=null)
                {
                    List<IDocumentService> docs = DocArray.OrderByDescending(a => a.Document.TimeStamp).ToList<IDocumentService>();
                    para.Docs = docs;
                    _ListEventAggregator.GetEvent<GenerateMD5HTMLEvent>().Publish(para);
                }
                
            }
            catch (Exception ex)
            {

            }
        }

        private void PublishExecute(object parameter)
        {
            if (DocArray[0].Document != null && DocArray[1].Document != null)
            {
                List<IDocumentService> docs = DocArray.OrderByDescending(a => a.Document.TimeStamp).ToList<IDocumentService>();
                _ListEventAggregator.GetEvent<PublishMD5HTMLEvent>().Publish(docs);

            }
        }

        public void CancelOperation(object cmdParameter)
        {
            BusyIndicator.IsContinue = false;
        }

        private bool OpenP1(string file)
        {
            if (!file.EndsWith(".pn"))
                return false;

            IsErrorMsgVisible = false;

            if (DocArray[0].Document != null)
            {
                DocArray[0].Close();
                P1Name = string.Empty;
                IsP1Opened = false;
                FirePropertyChanged("P1Thumbnail");
            }

            if (file == _p2FileName)
            {
                IsErrorMsgVisible = true;
                ErrorMessage = GlobalData.FindResource("Differ_Warning_SameFiles");
                return false;
            }

            try
            {
                DocArray[0].Open(file);
                DocArray[0].Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.ExportType = ExportType.Data;

            }
            catch (HigherDocumentVersionException)
            {
                MessageBox.Show(GlobalData.FindResource("Common_UpdateNotice"), GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            P1Name = file;

            IsP1Opened = true;
            FirePropertyChanged("P1Thumbnail");
            return true;
        }

        private bool OpenP2(string file)
        {
            if (!file.EndsWith(".pn"))
                return false;

            IsErrorMsgVisible = false;

            if (DocArray[1].Document != null)
            {
                DocArray[1].Close();
                P2Name = string.Empty;
                IsP2Opened = false;
                FirePropertyChanged("P2Thumbnail");
            }

            if (file == _p1FileName)
            {
                IsErrorMsgVisible = true;
                ErrorMessage = GlobalData.FindResource("Differ_Warning_SameFiles");
                return false;
            }

            try
            {
                DocArray[1].Open(file);
                DocArray[1].Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.ExportType = ExportType.Data;
            }
            catch (HigherDocumentVersionException)
            {
                MessageBox.Show(GlobalData.FindResource("Common_UpdateNotice"), GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            P2Name = file;
            IsP2Opened = true;
            FirePropertyChanged("P2Thumbnail");
            return true;
        }

        private string GetFileName(string path)
        {
            if (string.IsNullOrEmpty(path)||!path.EndsWith(".pn"))
                return string.Empty;

            path = path.Remove(path.LastIndexOf(".pn"), 3);
            return path.Substring(path.LastIndexOf("\\") + 1);
        }

        public void ClearPreviewFolder()
    {
        string previewPath = CommonFunction.GetPreViewTempPath();
        try
        {
            //Delete preview folder when current document is closed.
            if (Directory.Exists(previewPath))
            {
                Directory.Delete(previewPath, true);
            }
        }
        catch
        {
        }
    }
        public void InitializeSetting(string file1, string file2)
        {
            OpenP1(file1);
            OpenP2(file2);
            FirePropertyChanged("IsPublishEnabled");
        }

    }




}
