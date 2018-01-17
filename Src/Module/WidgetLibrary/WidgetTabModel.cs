using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Naver.Compass.Common;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Naver.Compass.Module
{
    public class WidgetTab : INotifyPropertyChanged, IMyLibrary
    {
        public string TabName { get; set; }
        public RangeObservableCollection<WidgetExpand> WidgetExpands { get; set; }
        public WidgetTab()
        {
            WidgetExpands = new RangeObservableCollection<WidgetExpand>();

        }

        public void PerformSearch(string searchTxt)
        {
            TextSearch = searchTxt;
            if (this.WidgetExpands != null)
            {
                var allModels = this.WidgetExpands.SelectMany(expand => expand.WidgetModels);
                foreach (var widget in allModels)
                {
                    widget.Search(searchTxt);
                }

                foreach (var expand in this.WidgetExpands)
                {
                    expand.SearchText = searchTxt;
                }
            }
        }

        #region ViewModel
        private bool isEmptyHintVisible;
        [XmlIgnore]
        public bool IsEmptyHintVisible
        {
            get { return isEmptyHintVisible; }
            set
            {
                if (isEmptyHintVisible != value)
                {
                    isEmptyHintVisible = value;
                    this.FirePropertyChanged("IsEmptyHintVisible");
                }
            }
        }

        private string textSearch;
        [XmlIgnore]
        public string TextSearch
        {
            get { return textSearch; }
            set
            {
                if (textSearch != value)
                {
                    textSearch = value;
                    this.FirePropertyChanged("InlineContent");
                }
            }
        }

        public void LocalizedTextChanged()
        {
            this.FirePropertyChanged("InlineContent");
        }

        [XmlIgnore]
        public InlineContent InlineContent
        {
            get
            {
                var inlineContent = new InlineContent();
                var resourceStr = Application.Current.FindResource("Libraries_Search_NoResult") as string;
                var splits = resourceStr.Split(new string[] { "{0}" }, StringSplitOptions.RemoveEmptyEntries);
                if (splits != null && splits.Length == 2)
                {
                    inlineContent.Add(new InlineInfo
                    {
                        Text = splits[0]
                    });

                    inlineContent.Add(new InlineInfo
                    {
                        Text = TextSearch,
                        Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x4a, 0x7e, 0xec))
                    });

                    inlineContent.Add(new InlineInfo
                    {
                        Text = splits[1]
                    });
                }

                return inlineContent;
            }
        }

        private DelegateCommand<object> _contextMenuOpenCommand;
        [XmlIgnore]
        public DelegateCommand<object> ContextMenuOpenCommand
        {
            get
            {
                if (_contextMenuOpenCommand == null)
                {
                    _contextMenuOpenCommand = new DelegateCommand<object>(ContextMenuOpenExecute);
                }

                return _contextMenuOpenCommand;
            }
        }

        public void ContextMenuOpenExecute(object cmdParameter)
        {
            var contextMenu = cmdParameter as ContextMenu;
            if (contextMenu != null)
            {
                contextMenu.Items.Clear();

                var downloadLibrary = new MenuItem
                {
                    Header = Application.Current.FindResource("Libraries_ContextMenu_Downloadlibrary") as string,
                    Icon = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Naver.Compass.Module.WidgetLibrary;component/Resources/Images/ico_download.png", UriKind.RelativeOrAbsolute))
                    }
                };
            
                downloadLibrary.Click += downloadLibrary_Click;
                contextMenu.Items.Add(downloadLibrary);

                contextMenu.Items.Add(new Separator { });

                var importToLibrary = new MenuItem
                {
                    Header = Application.Current.FindResource("Libraries_ContextMenu_ImportToLibrary") as string,
                    Icon = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Naver.Compass.Module.WidgetLibrary;component/Resources/Images/ico_import.png", UriKind.RelativeOrAbsolute))
                    }
                };
                importToLibrary.Click += importToLibrary_Click;
                contextMenu.Items.Add(importToLibrary);

                var createLibrary = new MenuItem
                {
                    Header = Application.Current.FindResource("Libraries_ContextMenu_CreateLibrary") as string,
                    Icon = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Naver.Compass.Module.WidgetLibrary;component/Resources/Images/ico_create.png", UriKind.RelativeOrAbsolute))
                    }
                };

                createLibrary.Click += createLibrary_Click;
                contextMenu.Items.Add(createLibrary);

                System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
                {
                    System.Threading.Thread.Sleep(100);
                    contextMenu.Dispatcher.BeginInvoke(new Action(() => { contextMenu.IsOpen = true; }));
                });

                task.Start();
            }
        }

        void downloadLibrary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(CommonDefine.UrlLibaryDownload);
                SendNClick("mml.downloadlibraries");
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine("Open URL error : {0}.", exp.Message);
            }
        }

        private void importToLibrary_Click(object sender, RoutedEventArgs e)
        {
            SendNClick("mml.importtolibrary");
            var dialog = new OpenFileDialog();
            dialog.Filter = CommonDefine.LibraryFilter;
            var rst = dialog.ShowDialog();
            if (rst.HasValue && rst.Value)
            {
                ImportToLibrary(dialog.FileName, dialog.SafeFileName);
            }
        }
        void createLibrary_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string path =Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                path += @"\protoNow.exe";

                //pass create library info and current language.
                string args = "create " + GlobalData.Culture;
                System.Diagnostics.Process.Start(path, args);

                var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                _ListEventAggregator.GetEvent<DisplayAppLoadingEvent>().Publish(true);
                
                SendNClick("mml.createlibrary");
            }
            catch (Exception ex)
            {
                NLogger.Warn("Create Library failed. ex:{0}", ex.ToString());

                MessageBox.Show(GlobalData.FindResource("Error_Create_Library") + ex.Message);
            }
        }

        private void ImportToLibrary(string fileName, string safeFileName, int insertIndex = 1)
        {
            var doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            try
            {
                var libraryGuid = doc.LibraryManager.PeekLibraryGuidFromFile(fileName);
                if (libraryGuid != default(Guid))
                {

                    if (this.WidgetExpands.Any(x => x.LibraryGID == libraryGuid))
                    {
                        var winDuplicate = new DuplicateLibraryWindow();
                        winDuplicate.Owner = Application.Current.MainWindow;
                        winDuplicate.ShowDialog();
                        if (!winDuplicate.Result.HasValue)
                        {
                            return;
                        }
                        else if (winDuplicate.Result.Value)
                        {
                            ///update
                            var matchItem = this.WidgetExpands.Where(x => x.LibraryGID == libraryGuid).FirstOrDefault();
                            if (matchItem != null)
                            {
                                var removeIndex = this.WidgetExpands.IndexOf(matchItem);
                                this.WidgetExpands.Remove(matchItem);
                                doc.LibraryManager.DeleteLibrary(libraryGuid);
                                ImportToLibrary(fileName, safeFileName, removeIndex);
                            }
                        }
                        else
                        {
                            ///as new
                            var copyNewPath = System.IO.Path.Combine(
                                System.Environment.GetEnvironmentVariable("TMP"),
                                string.Format("{0}.libpn", Environment.TickCount));
                            doc.LibraryManager.CreateNewLibraryFile(fileName, copyNewPath);
                            ImportToLibrary(copyNewPath, safeFileName);
                        }
                    }
                    else
                    {
                        var path = System.IO.Path.Combine(
                                this.CreateLibraryFolder(libraryGuid),
                                safeFileName);
                        File.Copy(fileName, path, true);
                        
                        var loadLibrary = doc.LibraryManager.LoadLibrary(path);

                        InsertLibrary(loadLibrary, insertIndex);
                    }

                }
            }
            catch (Exception ex)
            {
                NLogger.Warn("Failed to import library {0},ex:{1}", fileName, ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        private void InsertLibrary(ILibrary library, int insertIndex)
        {
            var expand = new WidgetExpand
            {
                LibraryGID = library.Guid,
                Header = library.Title,
                FileName = library.Name,
                IsCustomWidget = true,
                IsExpand = true,
                ExpandCache = true
            };

            foreach (var customObject in library.CustomObjects)
            {
                var bytes = default(byte[]);
                if (customObject.Icon != null)
                {
                    bytes = new byte[customObject.Icon.Length];
                    customObject.Icon.Read(bytes, 0, bytes.Length);
                }

                var widgetModel = new WidgetModel
                {
                    Id = customObject.Guid,
                    Name = customObject.Name,
                    Icon = bytes != null ? Convert.ToBase64String(bytes) : null,
                    LbrType = "custom"
                };

                expand.WidgetModels.Add(widgetModel);
            }

            this.WidgetExpands.Insert(insertIndex, expand);

            var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _ListEventAggregator.GetEvent<CustomWidgetChangedEvent>().Publish(expand);
        }

        /// <summary>
        /// Run after user click Export To My Library menu.
        /// Save a copy of current library to local path.
        /// </summary>
        /// <returns>saved path</returns>
        public string ExportLibraryFileToLocal()
        {
            if (_document.DocumentType == DocumentType.Standard)
                return string.Empty;

            var safeFileName = string.IsNullOrEmpty(_document.Title) ? CommonDefine.Untitled : _document.Title;

            safeFileName += ".libpn";

            var path = System.IO.Path.Combine(
                                this.CreateLibraryFolder(_document.Guid),
                                safeFileName);

            var library = (_document as ILibrary);
            library.SaveCopyTo(path);
            return path;
        }

        /// <summary>
        /// Run after user click Export To My Library menu.
        /// Load library from local path.
        /// </summary>
        /// <param name="fileName">file saved in local path</param>
        public void LoadLibraryFromPath(string fileName)
        {
            int index = 1;
            var doc = ServiceLocator.Current.GetInstance<IDocumentService>();

             var libraryGuid = doc.LibraryManager.PeekLibraryGuidFromFile(fileName);
             if (libraryGuid != default(Guid))
             {
                 if (this.WidgetExpands.Any(x => x.LibraryGID == libraryGuid))
                 {
                     //if library exists, delete it first.
                     var matchItem = this.WidgetExpands.Where(x => x.LibraryGID == libraryGuid).FirstOrDefault();
                     if (matchItem != null)
                     {
                         index = this.WidgetExpands.IndexOf(matchItem);
                         this.WidgetExpands.Remove(matchItem);
                         doc.LibraryManager.DeleteLibrary(libraryGuid);
                         
                     }
                 }
                 //insert library to My Library
                 var loadLibrary = doc.LibraryManager.LoadLibrary(fileName);
                 InsertLibrary(loadLibrary, index);

                 if(_document.Guid == loadLibrary.Guid)
                 {
                     MessageBox.Show(GlobalData.FindResource("Alert_ExportToMylibrary_Finished"));
                 }
             }

        }

        public void CreateCustomLibrary(Guid libraryId, string libraryName)
        {
            if (string.IsNullOrEmpty(libraryName))
            {
                throw new ArgumentException("libraryName cannot be empty");
            }

            var doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            try
            {
                var iloadLibrary = doc.LibraryManager.GetLibrary(libraryId);
                if (iloadLibrary != null && !this.WidgetExpands.Any(e => e.LibraryGID == libraryId))
                {

                    var path = System.IO.Path.Combine(
                                        this.CreateLibraryFolder(libraryId),
                                        string.Format("{0}.libpn", libraryName));
                    iloadLibrary.Save(path);
                    var expand = new WidgetExpand
                    {
                        LibraryGID = iloadLibrary.Guid,
                        Header = iloadLibrary.Title,
                        FileName = iloadLibrary.Name,
                        IsCustomWidget = true,
                        IsExpand = true,
                        ExpandCache = true
                    };

                    foreach (var customObject in iloadLibrary.CustomObjects)
                    {
                        var bytes = default(byte[]);
                        if (customObject.Icon != null)
                        {
                            bytes = new byte[customObject.Icon.Length];
                            customObject.Icon.Read(bytes, 0, bytes.Length);
                        }

                        var widgetModel = new WidgetModel
                        {
                            Id = customObject.Guid,
                            Name = customObject.Name,
                            Icon = bytes != null ? Convert.ToBase64String(bytes) : null,
                            LbrType = "custom"
                        };

                        expand.WidgetModels.Insert(0, widgetModel);
                    }

                    this.WidgetExpands.Insert(1, expand);
                    var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                    _ListEventAggregator.GetEvent<CustomWidgetChangedEvent>().Publish(expand);

                }
            }
            catch (Exception ex)
            {
                NLogger.Warn("Failed to create custom library. ex:{0}", ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        private string CreateLibraryFolder(Guid guid)
        {
            var path = System.IO.Path.Combine(
                            WidgetGalleryViewModel.Widgetlibraryfolder,
                            string.Format("{0}", guid));
            System.IO.Directory.CreateDirectory(path);
            return path;
        }

        #endregion
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void FirePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        #region Library mode initialization
        IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }
        public void DomLoadedEventHandler(FileOperationType loadType)
        {
            switch (loadType)
            {
                case FileOperationType.Create:
                case FileOperationType.Open:
                    if (_document != null)
                    {
                       if(_document.DocumentType==DocumentType.Library)
                       {
                           //TODO:

                       }
                       else
                       {
                           //TODO:
                       }
                    }
                    break;
            }
        }
        public void  RegisterDomLoadedEvent()
        {
            var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _ListEventAggregator.GetEvent<DomLoadedEvent>().Subscribe(DomLoadedEventHandler, ThreadOption.UIThread);
        }

        /// <summary>
        /// Get NClick info and send it.
        /// </summary>
        /// <param name="panel">object to send nClick when click</param>
        private void SendNClick(string lbrNclickCode)
        {
            if (!string.IsNullOrEmpty(lbrNclickCode))
            {
                ServiceLocator.Current.GetInstance<INClickService>().SendNClick(lbrNclickCode);
                NLogger.Info("Send nclick: {0}", lbrNclickCode);
            }
        }
        #endregion
    }
}
