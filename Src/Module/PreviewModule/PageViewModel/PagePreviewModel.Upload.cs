using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using Naver.Compass.Service.Html;
using Naver.Compass.Module.PublishModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Specialized;
using ICSharpCode.SharpZipLib.Zip;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Naver.Compass.Common.Helper;
using System.Security.Cryptography;
using System.ComponentModel;

namespace Naver.Compass.Module.PreviewModule
{
    enum UploadState
    {
        upload_Init,
        upload_Generate,
        upload_Update,
        upload_End,
    }

    partial class PagePreViewModel
    {
        #region Private Member

        private UploadState _uploadState = UploadState.upload_End;

        private SvnProcess _procesWindow;
        private ProcessViewModel _procesVM;

        UploadParameter _upLoadPara;
        IDifferView _differView
        {
            get
            {
                return Application.Current.MainWindow.DataContext as IDifferView;
            }
        }
        #endregion

        #region Private function

        List<IDocumentService> _docArray;
        private bool InitMD5Parameter(UploadParameter data, List<IDocumentService> DocArray)
        {
            if (!CommonFunction.IsNetworkAvailable() && DocArray.Count == 2)//Check Network before publish
            {
                System.Windows.MessageBox.Show(GlobalData.FindResource("Publish_Window_S_ErrorMessage"));

                return false;
            }

            string guidKey = string.Empty;

            Guid p1 = DocArray[0].Document.Guid;
            Guid p2 = DocArray[1].Document.Guid;
            if (p1.CompareTo(p2) < 0)
            {
                guidKey = p1.ToString() + p2.ToString();
            }
            else
            {
                guidKey = p2.ToString() + p1.ToString();
            }

            data.id = ConfigFileManager.GetSectionValue(ConstData.sRemoteID, guidKey.ToString());
            data.ProjectPassword = ConfigFileManager.GetSectionValue(ConstData.sProjectPassword, guidKey.ToString());
            data.DocGUID = guidKey.ToString();
            //doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.CurrentPage = Guid.Empty;

            if (String.IsNullOrEmpty(data.id))
            {
                return true;
            }
            else
            {
                return HttpGetFileInfo(data);
            }
        }
        private bool InitParameter(UploadParameter data)
        {
            if (!CommonFunction.IsNetworkAvailable())//Check Network before publish
            {
                System.Windows.MessageBox.Show(GlobalData.FindResource("Publish_Window_S_ErrorMessage"));

                return false;
            }

            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();

            if (doc != null && doc.Document != null)
            {
                Guid sGUID = doc.Document.Guid;

                data.id = ConfigFileManager.GetSectionValue(ConstData.sRemoteID, sGUID.ToString());
                data.ProjectPassword = ConfigFileManager.GetSectionValue(ConstData.sProjectPassword, sGUID.ToString());
                data.DocGUID = sGUID.ToString();

                doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.CurrentPage = Guid.Empty;
            }

            if (String.IsNullOrEmpty(data.id))
            {
                return true;
            }
            else
            {
                return HttpGetFileInfo(data);
            }
        }

        #region show UI

        private bool ShowPeUploadWindow(object para)
        {
            PrePublish win = new PrePublish(para);
            win.Owner = Application.Current.MainWindow;

            bool? bRValue = win.ShowDialog();

            return (bool)bRValue;
        }

        private void ShowProgressWindow()
        {
            _procesWindow = new SvnProcess();
            _procesWindow.IsAllowClose = false;
            _procesVM = _procesWindow.DataContext as ProcessViewModel;
            _procesWindow.Owner = Application.Current.MainWindow;
            _procesWindow.ShowDialog();
        }

        private void ReShowProgressWindow()
        {
            if (!_procesWindow.IsVisible)
            {
                string sLastContent = _busyIndicator.Content;
                sLastContent = sLastContent.Substring(0, sLastContent.LastIndexOf("...") - 1);
                int dLastProcess = _busyIndicator.Progress;
                _procesWindow = new SvnProcess();
                _procesWindow.IsAllowClose = true;
                _procesVM = _procesWindow.DataContext as ProcessViewModel;
                _procesVM.IsCloseEnable = true;
                _busyIndicator.Content = sLastContent;
                _busyIndicator.Progress = dLastProcess;
                _procesWindow.Owner = Application.Current.MainWindow;
                _procesWindow.Show();
            }
            else
            {
                _procesWindow.Activate();
            }
        }

        private void ClearTempFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                if (Directory.Exists(_upLoadPara.ProjectPath))
                {
                    Directory.Delete(_upLoadPara.ProjectPath, true);
                }
            }
            catch (System.Exception ex)
            {
                NLogger.Error("Publish->Clear temp file occur exception.You can go on your work.\n"+ex.Message);
            }
        }

        #endregion

        #region Event Handler

        private void UploadHtmlEventHandler(bool isNormal = true)
        {
            // Make textbox update source data.
            ISelectionService _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            IPagePropertyData page = _selectionService.GetCurrentPage();
            if (page != null && page.EditorCanvas != null)
            {
                page.EditorCanvas.Focus();
            }

            if (_uploadState == UploadState.upload_End)
            {
                _uploadState = UploadState.upload_Init;

                _upLoadPara = new UploadParameter();

                if (!InitParameter(_upLoadPara))
                {
                    _uploadState = UploadState.upload_End;
                    return;
                }

                if (!ShowPeUploadWindow(_upLoadPara))
                {
                    _uploadState = UploadState.upload_End;
                    return;
                }

                _outputFolder = _upLoadPara.ProjectPath;
                string imgPath = _outputFolder + @"\images\";

                _htmlService.ImagesStreamManager.WorkingDirectory = imgPath;

                _ListEventAggregator.GetEvent<SynUploadEvent>().Publish(_upLoadPara);

                ShowProgressWindow();

            }
            else
            {
                ReShowProgressWindow();
            }
        }
        private void UploadMD5HtmlEventHandler(Object para)
        {
            _docArray = para as List<IDocumentService>;

            if (_uploadState == UploadState.upload_End)
            {
                _uploadState = UploadState.upload_Init;

                _upLoadPara = new UploadParameter();

                if (!InitMD5Parameter(_upLoadPara, _docArray))
                {
                    _uploadState = UploadState.upload_End;
                    return;
                }

                _upLoadPara.DocArray = _docArray;

                var prePublish = new PrePublishViewModel(_upLoadPara);
                prePublish.Initwindow();
                prePublish.Closing += PrePublish_Closing;

                // //Switch to PublishSetView
                _differView.Current = prePublish;
            }
            else
            {
                //ReShowProgressWindow();
            }
        }

        private void PrePublish_Closing(object sender, CancelEventArgs e)
        {
            if(e.Cancel)
            {
                _uploadState = UploadState.upload_End;
                RemovePrePublish_Closing();
                _differView.ResetView();
            }
            else
            {
                _outputFolder = _upLoadPara.ProjectPath;
                string imgPath = _outputFolder + @"\images\";

                _htmlService.ImagesStreamManager.WorkingDirectory = imgPath;

                _ListEventAggregator.GetEvent<SynUploadEvent>().Publish(_docArray);
                
                //_differView.PrePublishVM.IsEnabled = false;

                var uploadProcess = new ProcessViewModel();
                uploadProcess.Closing += UploadProcess_Closing;
                _procesVM = uploadProcess;
               // _procesVM = _differView.UploadProcessData;
                _procesVM.StartProgress();
            }
        }

        private void RemovePrePublish_Closing()
        {
            if (_differView.Current is PrePublishViewModel)
            {
                var prePublish = _differView.Current as PrePublishViewModel;
                prePublish.Closing -= PrePublish_Closing;
            }
        }
        private void UploadProcess_Closing(object sender, CancelEventArgs e)
        {
            _differView.ResetView();
        }

        public void OnSynUpload(object para)
        {
            UploadServer(para);
        }

        private async void UploadServer(object para)
        {
            List<IDocumentService> DocArray = para as List<IDocumentService>;

            _busyIndicator.Progress = 5;
            _busyIndicator.Content = @"begin to  project info...5%";

            #region  generate html

            //load page data and create all images
            if (DocArray == null)
            {
                _uploadState = UploadState.upload_Generate;
                await AsyncConvertAllPagesForUpload(5, 30);
            }


            _procesVM.IsCloseEnable = false;

            _busyIndicator.Progress = 35;
            _busyIndicator.Content = @"Generate the HTML Page...35%";

            //await Task.Factory.StartNew(GenerateHtml);
            bool bIsSuccessful=false;
            if (DocArray == null)
            {
                bIsSuccessful = await Task.Factory.StartNew<bool>(GenerateHtml);
            }
            else
            {
                _outputFolder = _outputFolder + @"\data";

                //foreach (IDocumentService doc in DocArray)
                //{
                //    await AsyncConvertAllPages(doc.Document);
                //}

                int i = 0;
                foreach (IDocumentService doc in DocArray)
                {
                    await AsyncConvertAllPages(doc.Document);

                    string szLocation = _outputFolder + @"\" + i++;
                    bIsSuccessful = await Task.Factory.StartNew<bool>(() => GenerateMD5Html(doc, szLocation));
                    if (bIsSuccessful == false)
                    {
                        _busyIndicator.IsShow = false;
                        _htmlService.IsHtmlGenerating = false;
                        break;
                    }
                    _busyIndicator.Progress = _busyIndicator.Progress + 3;
                }

                bool bSuccessful = await Task.Factory.StartNew<bool>(() => GenerateMD5DifferInfo(DocArray, _outputFolder));
                if (bSuccessful == false)
                {
                    _busyIndicator.IsShow = false;
                    _htmlService.IsHtmlGenerating = false;
                    MessageBox.Show(GlobalData.FindResource("Error_Generate_Html_Access"));
                    return;
                }

            }

            _htmlService.ImagesStreamManager.WorkingDirectory = string.Empty;

            if (!bIsSuccessful)
            {
                string errorMessage = @"Generate Html failed !";
                UploadFailed(errorMessage, string.Empty);
                return;
            }
            #endregion

            #region  ZIP File


            _busyIndicator.Progress = 40;
            _busyIndicator.Content = @"Zip file...40%";

            string upLoadPath = _upLoadPara.ProjectPath + @"Upload.zip";
            await ZIPFiles(_upLoadPara.ProjectPath, upLoadPath);
            #endregion

            #region Publish

            _busyIndicator.Progress = 50;
            //now enable Hide window
            _procesVM.IsCloseEnable = true;
            if (_procesWindow != null)
            {
                _procesWindow.IsAllowClose = true;
            }

            _uploadState = UploadState.upload_Update;
            _busyIndicator.Content = @"Publish File...50%";
            //  FileInfo 
            if (CheckFileInfo(upLoadPath))
            {
                if (_upLoadPara.IsNewProject)//Create new
                {
                    await HttpUploadFiles(upLoadPath,false);
                }
                else
                {
                    await HttpUploadFiles(upLoadPath,true, _upLoadPara.id);
                }

                Naver.Compass.Common.CommonBase.NLogger.Debug("Project publish end,Path->" + _upLoadPara.ProjectPath + ";GUID->" + _upLoadPara.DocGUID);
            }
            else
            {
                UploadFailed(GlobalData.FindResource("Publish_File_Size_Error"), upLoadPath);
            }

            #endregion

        }

        #endregion

        #region Get file infomation

        private bool HttpGetFileInfo(UploadParameter para)
        {
            try
            {
                if (!CommonFunction.IsNetworkAvailable())
                {
                    return false;
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConstData.uploadURL + para.id);

                request.Referer = ConstData.uploadReferer;
                request.Headers.Add(ConstData.uploadVersionName, CommonFunction.GetClientCurrentVersion());

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();

                var serializer = new DataContractJsonSerializer(typeof(GetFileResponseData));
                GetFileResponseData data = (GetFileResponseData)serializer.ReadObject(dataStream);

                if (data != null)
                {
                    if (data.is_exist)
                    {
                        para.sShortURL = data.shortUrl;
                        if (!String.IsNullOrEmpty(data.updatedAt))
                        {
                            para.sTime = data.updatedAt.Substring(0, data.updatedAt.IndexOf("T"));
                        }
                        else
                        {
                            para.sTime = data.createdAt.Substring(0, data.createdAt.IndexOf("T"));
                        }
                       
                        para.IsPublic = data.is_public;
                    }
                    else
                    {
                        para.id = "";
                        para.ProjectPassword = "";

                    }
                }
                else
                {
                    System.Windows.MessageBox.Show(GlobalData.FindResource("Error_Server_response"));
                    return false;
                }
            }
            catch (System.Exception ex)
            {
               // NLogger.Error("HttpGetFileInfo exception->"+ex.Message);
                return false;
            }

            return true;
        }

        private bool CheckFileInfo(string filePath)
        {
            if (File.Exists(filePath))
            {
                FileInfo info = new FileInfo(filePath);
                if (info != null)
                {
                    if (info.Length/(1024*1024) < 200)
                    {
                        return true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(GlobalData.FindResource("Publish_File_Size_Error"));
                    }
                }                
            }

            return false;
        }

        #endregion

        #region Convert project file

        private async Task AsyncConvertAllPagesForUpload(int isProgress, int ieProgress)
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc == null || doc.Document == null || doc.Document.Pages.Count <= 0)
            {
                return;
            }


            int nDiv = (ieProgress - isProgress) / doc.Document.Pages.Count;
            int nProgress = nDiv;

            List<IPage> TopPages = new List<IPage>();
            TopPages.AddRange(doc.Document.Pages);
            TopPages.AddRange(doc.Document.MasterPages);
            foreach (IPage CurrentPage in TopPages)
            {
                if (_busyIndicator.IsContinue == false)
                    break;
                _busyIndicator.Progress = nProgress;
                _busyIndicator.Content = @"Convert the image on page " + CurrentPage.Name;
                nProgress += nDiv;

                bool isClosedPage = false;
                if (!CurrentPage.IsOpened)
                {
                    isClosedPage = true;
                    CurrentPage.Open();
                }

                //Convert Current Page Self
                if (CurrentPage is IMasterPage)
                {
                    await AsyncConvertNormalPage(CurrentPage.Guid, doc.Document,true);

                    List<IPage> Chidlren = new List<IPage>();
                    GetAllChildrenPage(Chidlren, CurrentPage);
                    foreach (IPage ChildPage in Chidlren)
                    {
                        await AsyncConvertChildlPage(ChildPage, doc.Document);
                    }
                }
                else
                {

                   //Convert Current Page Self
                    await AsyncConvertNormalPage(CurrentPage.Guid, doc.Document);

                    //Convert All Children Pages(Dynamic/Hamburg)
                    List<IPage> Chidlren = new List<IPage>();
                    GetAllChildrenPage(Chidlren, CurrentPage);
                    foreach (IPage ChildPage in Chidlren)
                    {
                        await AsyncConvertChildlPage(ChildPage, doc.Document);
                    }
                } 

                if (isClosedPage)
                {
                    CurrentPage.Close();
                }
            }
        }

        private async Task ZIPFiles(string sPath, string upLoadPath)
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    FastZip _fastZip = new FastZip();

                    _fastZip.CreateZip(upLoadPath, sPath, true, "");
                }
                catch (System.Exception ex)
                {
                    NLogger.Error("Publish->Zip file error!\n"+ex.Message);
                }
                
            });
        }

        #endregion

        #region Upload project file

        private byte[] GetUploadData(string boundary, string filePath)
        {
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("password", _upLoadPara.DocGUID.ToString());

            string AccessPassword = "";
            string salt = "";
            if (!String.IsNullOrEmpty(_upLoadPara.ProjectPassword))
            {
                salt = Guid.NewGuid().ToString();
                AccessPassword = GetHashString(_upLoadPara.ProjectPassword, salt);
            }
            nvc.Add("salt_for_access", salt);
            nvc.Add("access_password", AccessPassword);

            MemoryStream rs = new MemoryStream();
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);

                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);
            string headerTemplate = "Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n";
            string header = string.Format(headerTemplate, filePath, "application / x - zip - compressed");
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);

            byte[] rByte = new byte[rs.Length];

            rs.Seek(0, SeekOrigin.Begin);

            rs.Read(rByte, 0, rByte.Length);

            return rByte;
        }

        private string GetHashString(string sPassword,string salt)
        {
            HashAlgorithm hash = new System.Security.Cryptography.HMACSHA1(Encoding.UTF8.GetBytes(salt));

            byte[] hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(sPassword));

            hash.Clear();
           
            return  BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
          
        }
 
        private async Task HttpUploadFiles(string localPath, bool bOverride, string id = "")
        {
            await Task.Factory.StartNew(() =>
            {
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

                try
                {
                    if (!CommonFunction.IsNetworkAvailable())
                    {
                        System.Windows.MessageBox.Show(GlobalData.FindResource("Publish_Window_S_ErrorMessage"));
                        ClearTempFile(localPath);
                        return;
                    }

                    using (WebClient client = new WebClient())
                    {
                        client.Credentials = System.Net.CredentialCache.DefaultCredentials;
                        client.Headers.Add(HttpRequestHeader.ContentType, "multipart/form-data; boundary=" + boundary);
                        client.Headers.Add(HttpRequestHeader.Referer, ConstData.uploadReferer);
                        client.Headers.Add(ConstData.uploadProtectName, ConstData.uploadProtectValue);
                        client.Headers.Add(ConstData.uploadVersionName, CommonFunction.GetClientCurrentVersion());
                        client.Encoding = Encoding.UTF8;
                        byte[] testbyte = GetUploadData(boundary, localPath);

                        client.UploadProgressChanged += new UploadProgressChangedEventHandler(delegate (object sender, UploadProgressChangedEventArgs e)
                        {
                            int iPercent = 50 + (int)e.ProgressPercentage;

                            if (iPercent <= 100)
                            {
                                _busyIndicator.Progress = iPercent;
                                _busyIndicator.Content = String.Format("Uploaded {0} of {1} bytes...",
                                     e.BytesSent,
                                     e.TotalBytesToSend);
                            }

                        });
                        client.UploadDataCompleted += new UploadDataCompletedEventHandler(delegate (object sender, UploadDataCompletedEventArgs e)
                        {
                            if (e.Error == null)
                            {
                                byte[] data = (byte[])e.Result;

                                var mStream = new MemoryStream(data);
                                var serializer = new DataContractJsonSerializer(typeof(ResponseData));
                                ResponseData response = (ResponseData)serializer.ReadObject(mStream);

                                if (response.CheckData())
                                {
                                    UploadSucessed(ref response, localPath);
                                }
                                else
                                {
                                    UploadFailed("Server response error!", localPath, bOverride);
                                }
                            }
                            else
                            {
                                UploadFailed(e.Error.Message, localPath, bOverride);
                            }                         

                        });

                        if (bOverride)
                        {
                            client.UploadDataAsync(new Uri(ConstData.uploadURL + id), "PUT", testbyte);
                        }
                        else
                        {
                            client.UploadDataAsync(new Uri(ConstData.uploadURL), "POST", testbyte);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    UploadFailed(ex.Message, localPath);
                }
            });
        }

        private void UploadSucessed(ref ResponseData data, string tempFiles)
        {
            ConfigFileManager.SetSectionValue("RemoteID", _upLoadPara.DocGUID, data.id);
            ConfigFileManager.SetSectionValue("ProjectPassword", _upLoadPara.DocGUID, _upLoadPara.ProjectPassword);

            _busyIndicator.Progress = 100;
            _busyIndicator.Content = @"Publish Success";

            _procesVM.sShortUrl = data.shortUrl;
            _procesVM.sUrl = data.shortUrl;

            _procesVM.IsCloseEnable = true;
            if (_procesWindow != null)
            {
                _procesWindow.IsAllowClose = true;
            }
            else
            {
                Application.Current.Dispatcher.Invoke(
                    new Action(
                        delegate
                        {
                            RemovePrePublish_Closing();
                            _differView.Current = _procesVM;
                        }));
                _procesVM.StopProgress();
            }
            _uploadState = UploadState.upload_End;

            ClearTempFile(tempFiles);
        }

        private void UploadFailed(string errorMessage, string tempFiles, bool bNeedClear = false)
        {
            if (bNeedClear)
            {
                ConfigFileManager.SetSectionValue("RemoteID", _upLoadPara.DocGUID, "");
                ConfigFileManager.SetSectionValue("ProjectPassword", _upLoadPara.DocGUID, "");
            }
            _procesVM.sErrorMessage = errorMessage;
            _busyIndicator.Progress = 100;
            _busyIndicator.Content = @"Publish Failed!";

            _procesVM.IsCloseEnable = true;
            if (_procesWindow != null)
            {
                _procesWindow.IsAllowClose = true;
            }
            else
            {
                Application.Current.Dispatcher.Invoke(
                    new Action(
                        delegate
                        {
                            _differView.UploadError(errorMessage);
                        }));
                _procesVM.StopProgress();
            }
            _uploadState = UploadState.upload_End;

            ClearTempFile(tempFiles);
        }
        #endregion

        #endregion

    }
}
