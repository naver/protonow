using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using System.Windows.Documents;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.Module;
using System.IO;
using Naver.Compass.Service;
using System.Collections.ObjectModel;
using System.Reflection;
using Naver.Compass.Service.WebServer;
using Naver.Compass.Module.Model;
using Naver.Compass.Common.Win32;
using System.Windows.Interop;
using Naver.Compass.Service.Update;
using Naver.Compass.Service.Html;

namespace Naver.Compass.Main
{
    class MainIntegrationViewModel : ViewModelBase
    {
        public MainIntegrationViewModel(MainIntegrationWindow win)
        {
            _mainWindow = win;
            _mainWindow.ContentRendered += _mainWindow_ContentRendered;
            _mainWindow.Closing += _mainWindow_Closing;
            _docService = ServiceLocator.Current.GetInstance<IDocumentService>();
            InitCommand();
        }

       
        private void InitCommand()
        {
            this.NewCommand = new DelegateCommand<object>(NewExecute);
            this.OpenCommand = new DelegateCommand<object>(OpenExecute);
            this.SaveCommand = new DelegateCommand<object>(SaveExecute);
            this.SaveAsCommand = new DelegateCommand<object>(SaveAsExecute);
            this.LanguageSettingCommand = new DelegateCommand<object>(LanguageSettingExecute);
            this.CloseCommand = new DelegateCommand<object>(CloseExecute);
            this.LinkFeedbackCommand = new DelegateCommand<object>(LinkFeedbackExecute);
            this.PublishProject = new DelegateCommand<object>(PublishToDesignStudio);
            this.PreviewProjectCommand = new DelegateCommand<object>(PreviewDocment);
            this.HelpCommand = new DelegateCommand<object>(OnOpenHelpFile);

            //Busy Indicator Single from Prism
            //BusyIndicator = ServiceLocator.Current.GetInstance<BusyIndicatorContext>();
            this.CancelOperationCmd = new DelegateCommand<object>(CancelOperation);
            this.FormatPaintCommand = new DelegateCommand<object>(PaintFormatExcute, CanPaintFormat);

            //Escape key, to close about, welcome window
            this.EscapeCommand = new DelegateCommand<object>(EscapeExecue);

            //Add new page in sitemap, shortcut
            this.NewPageCommand = new DelegateCommand<object>(OnCreateNewPage);
        }
        private void Initialize()
        {
            _ListEventAggregator.GetEvent<FileOperationEvent>().Subscribe(FileOPerationHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<OpenFileEvent>().Subscribe(OpenFileHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<RecoveryFileEvent>().Subscribe(RecoveryFileHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<OpenDialogEvent>().Subscribe(OpenDialogEventHandler);
            _ListEventAggregator.GetEvent<AutoSaveSettingChangedEvent>().Subscribe(AutoSaveSettingChangedHandler);
            _ListEventAggregator.GetEvent<UpdateLanguageEvent>().Subscribe(UpdateLanguageEventHandler);
            _ListEventAggregator.GetEvent<ExportToMYLibraryEvent>().Subscribe(ExportToMyLibraryHandler);
            _ListEventAggregator.GetEvent<DisplayAppLoadingEvent>().Subscribe(DisplayAppLoadingHandler);

            InputShortCut();
            NLogger.Info("MainIntegrationViewModel loaded, try to perform setting of recovery.");
            AutoSaveService.Instance.PerformSetting();
        }

        #region Window operation
      
        void _mainWindow_ContentRendered(object sender, EventArgs e)
        {
            try
            {

                Initialize();

                InitlizeProject();
                
                if(Application.Current.MainWindow is SplashWindow)
                {
                    var win = Application.Current.MainWindow;
                    Application.Current.MainWindow = _mainWindow;
                    win.Close();
                }               

                CheckUpdate();

                //CheckAutoSave();

                InitMessageBetweenApp();
                Win32MsgHelper.PostMessage((IntPtr)Win32MsgHelper.HWND_BROADCAST, endLoading_Message, IntPtr.Zero, IntPtr.Zero);

                if (App.StartType == AppStartType.Normal)
                {
                    bool needShowRecovery = Naver.Compass.Module.Model.AutoSaveService.Instance.AfterSplashScreen();
                    if (needShowRecovery)
                    {
                        var rWin = new DocumentRecoveryWindow();
                        rWin.Owner = Application.Current.MainWindow;
                        bool? rHValue = rWin.ShowDialog();
                        if ((bool)rHValue)
                            return;
                    }
                    _welcomeScreenWindow = new WelcomeScreen();
                    _welcomeScreenWindow.Owner = Application.Current.MainWindow;
                    _welcomeScreenWindow.Closed += _welcomeScreenWindow_Closed;
                    _welcomeScreenWindow.ShowDialog();
                }            
            }
            catch(Exception)
            {
                NLogger.Error("Load main window faild!");
            }
           
        }

        void _welcomeScreenWindow_Closed(object sender, EventArgs e)
        {
            _welcomeScreenWindow = null;
        }

        void _mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (AppExit() == true)
            {

                e.Cancel = false;
                _ListEventAggregator.GetEvent<ChangeLayoutEvent>().Publish("SaveCustom");
                try
                {
                    AutoSaveService.Instance.CloseFile();
                }
                catch (Exception)
                {
                    NLogger.Warn("app window are closed when auto-save service is running ");
                }
                
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Open project or create new default project.
        /// Init language info.
        /// </summary>
        private void InitlizeProject()
        {
            string[] args = Environment.GetCommandLineArgs();
            IEventAggregator listEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            #region double-click to open library or document
            if (App.StartType == AppStartType.OpenFile)
            {
                string InitArgs = args[1];
                if (InitArgs != null && InitArgs.Length > 0)
                {
                    if (listEventAggregator != null)
                    {
                        listEventAggregator.GetEvent<OpenFileEvent>().Publish(InitArgs);
                        return;
                    }
                }
            }
            #endregion
            #region create library
            else if (App.StartType == AppStartType.CreateLibrary)
            {
                IsLibraryEditMode = true;

                if (!string.IsNullOrEmpty(args[2]))
                {
                    GlobalData.Culture = args[2];
                }

                if (listEventAggregator != null)
                {
                    listEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.Create);
                }
            }
            #endregion
            #region edit library
            else if (App.StartType == AppStartType.EditLibrary)
            {
                string InitArgs = args[1];
                if (InitArgs != null && InitArgs.Length > 0)
                {
                    if (listEventAggregator != null)
                    {
                        listEventAggregator.GetEvent<OpenFileEvent>().Publish(InitArgs);
                    }
                }

                if (!string.IsNullOrEmpty(args[3]))
                {
                    GlobalData.Culture = args[3];
                }
                return;
            }
            #endregion


            //if (listEventAggregator != null)
            //{
            //    listEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.Create);
            //}
        }

        private void CheckUpdate()
        {
            // Check udpate after showing main window
            IUpdateService updateService = ServiceLocator.Current.GetInstance<IUpdateService>();
            if (updateService.CheckUpdateAtStart)
            {
                updateService.CheckUpdate();
            }
        }

        private void InitMessageBetweenApp()
        {
            IShareMemoryService ShareMemSrv = ServiceLocator.Current.GetInstance<ShareMemorServiceProvider>();
            ShareMemSrv.Initialize();

            export_Message = Win32MsgHelper.RegisterWindowMessage("EXPORT_CURRENT_TO_MY_LIBRARY");
            language_Message = Win32MsgHelper.RegisterWindowMessage("CHANGE_CURRENT_LANGUAGE");
            endLoading_Message = Win32MsgHelper.RegisterWindowMessage("ENDSHOW_APP_LOADING_IMAGE");
            HwndSource.FromHwnd((new WindowInteropHelper(_mainWindow)).Handle).AddHook(new HwndSourceHook(HandleMessages));

        }
        private static IntPtr HandleMessages(IntPtr handle, Int32 message, IntPtr wParameter, IntPtr lParameter, ref Boolean handled)
        {
            if (message == export_Message)
            {
                //if (Clipboard.ContainsData(@"ProtoNowLibRefreshID"))
                //{
                //    object srcData = Clipboard.GetData(@"ProtoNowLibRefreshID");
                //    if (srcData != null)
                //    {                        
                //         var iEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                //         iEventAggregator.GetEvent<RefreshCustomLibraryEvent>().Publish((string)srcData);
                //    }
                //}


                IShareMemoryService ShareMemSrv = ServiceLocator.Current.GetInstance<ShareMemorServiceProvider>();
                string szPath = ShareMemSrv.GettShareDatePath();
                if (szPath != string.Empty)
                {
                    var iEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                    iEventAggregator.GetEvent<RefreshCustomLibraryEvent>().Publish(szPath);
                }
            }
            else if (message == language_Message)
            {
                IShareMemoryService ShareMemSrv = ServiceLocator.Current.GetInstance<ShareMemorServiceProvider>();
                string lan = ShareMemSrv.GettShareDatePath();
                if (lan != string.Empty && GlobalData.Culture != lan)
                {

                    GlobalData.Culture = lan;
                    var iEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                    iEventAggregator.GetEvent<UpdateLanguageEvent>().Publish(string.Empty);
                }
            }
            else if (message == endLoading_Message)
            {
                var iEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                iEventAggregator.GetEvent<DisplayAppLoadingEvent>().Publish(false);
            }

            return IntPtr.Zero;
        }

        #endregion

        #region ShortCut
        private void InputShortCut()
        {
            FontCommands.Italic.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Control));
            FontCommands.Bold.InputGestures.Add(new KeyGesture(Key.B, ModifierKeys.Control));
            FontCommands.Underline.InputGestures.Add(new KeyGesture(Key.U, ModifierKeys.Control));

            TextCommands.AlignTextTop.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control | ModifierKeys.Shift));
            TextCommands.AlignTextBottom.InputGestures.Add(new KeyGesture(Key.B, ModifierKeys.Control | ModifierKeys.Shift));
            TextCommands.AlignTextMiddle.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control | ModifierKeys.Shift));
            TextCommands.AlignTextLeft.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Control | ModifierKeys.Shift));
            TextCommands.AlignTextCenter.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Shift));
            TextCommands.AlignTextRight.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control | ModifierKeys.Shift));
            TextCommands.UpDownCaseHotKey.InputGestures.Add(new KeyGesture(Key.F3, ModifierKeys.Shift));

            WidgetsCommands.DuplicateWidgets.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
            WidgetsCommands.GroupWidgets.InputGestures.Add(new KeyGesture(Key.G, ModifierKeys.Control));
            WidgetsCommands.UngroupWidgets.InputGestures.Add(new KeyGesture(Key.G, ModifierKeys.Control | ModifierKeys.Shift));

            WidgetsCommands.WidgetsBringFront.InputGestures.Add(new KeyGesture(Key.OemCloseBrackets, ModifierKeys.Control | ModifierKeys.Shift));
            WidgetsCommands.WidgetsBringForward.InputGestures.Add(new KeyGesture(Key.OemCloseBrackets, ModifierKeys.Control));
            WidgetsCommands.WidgetsBringBackward.InputGestures.Add(new KeyGesture(Key.OemOpenBrackets, ModifierKeys.Control));
            WidgetsCommands.WidgetsBringBottom.InputGestures.Add(new KeyGesture(Key.OemOpenBrackets, ModifierKeys.Control | ModifierKeys.Shift));

            WidgetsCommands.WidgetsAlignLeft.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Control | ModifierKeys.Alt));
            WidgetsCommands.WidgetsAlignRight.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control | ModifierKeys.Alt));
            WidgetsCommands.WidgetsAlignCenter.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Alt));
            WidgetsCommands.WidgetsAlignTop.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control | ModifierKeys.Alt));
            WidgetsCommands.WidgetsAlignMiddle.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control | ModifierKeys.Alt));
            WidgetsCommands.WidgetsAlignBottom.InputGestures.Add(new KeyGesture(Key.B, ModifierKeys.Control | ModifierKeys.Alt));


            WidgetsCommands.WidgetsDistributeHorizontally.InputGestures.Add(new KeyGesture(Key.H, ModifierKeys.Control | ModifierKeys.Shift));
            WidgetsCommands.WidgetsDistributeVertically.InputGestures.Add(new KeyGesture(Key.U, ModifierKeys.Control | ModifierKeys.Shift));

            WidgetPropertyCommands.Lock.InputGestures.Add(new KeyGesture(Key.K, ModifierKeys.Control));
            WidgetPropertyCommands.Unlock.InputGestures.Add(new KeyGesture(Key.K, ModifierKeys.Control | ModifierKeys.Shift));


            GridGuideCommands.ShowGrid.InputGestures.Add(new KeyGesture(Key.OemQuotes, ModifierKeys.Control));
            GridGuideCommands.SnapToGrid.InputGestures.Add(new KeyGesture(Key.OemQuotes, ModifierKeys.Control | ModifierKeys.Alt));
            GridGuideCommands.ShowGlobalGuides.InputGestures.Add(new KeyGesture(Key.OemPeriod, ModifierKeys.Control));
            GridGuideCommands.ShowPageGuides.InputGestures.Add(new KeyGesture(Key.OemComma, ModifierKeys.Control));
            GridGuideCommands.SnapToGuides.InputGestures.Add(new KeyGesture(Key.OemComma, ModifierKeys.Control | ModifierKeys.Alt));
            GridGuideCommands.LockGuides.InputGestures.Add(new KeyGesture(Key.OemPeriod, ModifierKeys.Control | ModifierKeys.Alt));

            AppCommands.DefaultStyle.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control|ModifierKeys.Shift));

            EditingCommands.IncreaseFontSize.InputGestures.Add(new KeyGesture(Key.OemPeriod, ModifierKeys.Control | ModifierKeys.Shift));
            EditingCommands.DecreaseFontSize.InputGestures.Add(new KeyGesture(Key.OemComma, ModifierKeys.Control | ModifierKeys.Shift));
            
            EditingCommands.Delete.InputGestures.Add(new KeyGesture(Key.Back));//Mac use
            EditingCommands.Delete.InputGestures.Add(new KeyGesture(Key.Delete));//use system default "Delete" key
            //ApplicationCommands.Delete.InputGestures.Add(new KeyGesture(Key.Back));
        }
        #endregion

        #region Binding Property
        public BusyIndicatorContext BusyIndicator
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BusyIndicatorContext>();
            }
        }
        public DelegateCommand<object> CancelOperationCmd { get; private set; }
        public DelegateCommand<object> EscapeCommand { get; private set; }
        public void CancelOperation(object cmdParameter)
        {
            BusyIndicator.IsContinue = false;
        }

        /// <summary>
        /// Escape key down, close about/wlecomescreen
        /// </summary>
        /// <param name="cmdParameter"></param>
        public void EscapeExecue(object cmdParameter)
        {
            if (isAboutOpen)
            {
                IsAboutOpen = false;
            }
        }

        public string Title
        {
            set
            {
                if (_WindowTitle != value)
                {
                    _WindowTitle = value;
                    FirePropertyChanged("Title");
                }
            }
            get
            {
                return _WindowTitle;
            }
        }

        private bool isAboutOpen;
        public bool IsAboutOpen
        {
            get
            {
                return isAboutOpen;
            }
            set
            {
                if (isAboutOpen != value)
                {
                    isAboutOpen = value;
                    FirePropertyChanged("IsAboutOpen");
                }
            }
        }


        public bool IsHideWelcomOnStart
        {
            get
            {
                return false;
                //return GlobalData.IsHideWelcomOnStart;
            }
            set
            {
                GlobalData.IsHideWelcomOnStart = value;
            }
        }

        public string CurrentVersion
        {
            get
            {
                // Display version format is x.x.x
                string productVersion = ConfigFileManager.CurrentVersion;
                if (String.IsNullOrEmpty(productVersion))
                {
                    return String.Empty;
                }
                else
                {
                    productVersion = productVersion.Substring(0, productVersion.LastIndexOf("."));
                }

                return "Version " + productVersion;
            }
        }

        public Visibility IsShowAppLoading
        {
            get
            {
                return _isShowAppLoading;
            }
            set
            {
                if(_isShowAppLoading!=value)
                {
                    _isShowAppLoading = value;
                    FirePropertyChanged("IsShowAppLoading");
                }
            }
            
        }

        //Mark current mode (Standard/Library)
        public bool IsLibraryEditMode = false;
        #endregion Binding Property

        #region Private Fields
        private static Int32 export_Message;
        private static Int32 language_Message;
        private static Int32 endLoading_Message;
        private String _WindowTitle = PRODUCT_NAME;
        public static readonly string PRODUCT_NAME = "protoNow";
        private const string _openFileFilter = "protoNow Files (*.pn)|*.pn|libpn documents (*.libpn)|*.libpn|All Files (*.*)|*.*";
        private const string _standardDocFilter = "protoNow Files (*.pn)|*.pn";
        private string _previewPath = "";
        private Visibility _isShowAppLoading = Visibility.Collapsed;
        private MainIntegrationWindow _mainWindow;
        private WelcomeScreen _welcomeScreenWindow;
        private IDocumentService _docService = null;
        #endregion 

        #region File operation
        private void OpenFile()
        {
            //Save current project.
            bool bCanceled = false;
            SaveCurrent(ref bCanceled);
            if (bCanceled)
                return;

            //Open a selected file and load data into DOM, then publish model-update message.20140218
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = _openFileFilter;
            if (openFile.ShowDialog() == true)
            {
                try
                {
                    CloseCurrent();
                    _docService.Open(openFile.FileName);

                    if (_docService.Document == null)
                    {
                        _docService.Close();
                        return;
                    }
                    if (_welcomeScreenWindow != null)
                    {
                        _welcomeScreenWindow.Close();
                    }

                    
                    if(_docService.Document.DocumentType == DocumentType.Library)
                    {
                        IsLibraryEditMode = true;
                    }
                    else
                    {
                        IsLibraryEditMode = false;
                    }

                    AddDefaultNote(_docService);

                    ConfigFileManager.InsertFile(openFile.FileName);

                    ClearPreviewFolder();

                    if (_docService.Document != null)
                    {
                        _ListEventAggregator.GetEvent<DomLoadedEvent>().Publish(FileOperationType.Open);
                        _ListEventAggregator.GetEvent<FlashRecentList>().Publish(null);
                    }

                    AutoSaveService.Instance.OpenFile(_docService);
                }
                catch (HigherDocumentVersionException)
                {
                    MessageBox.Show(GlobalData.FindResource("Common_UpdateNotice"), GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void OpenFile(string path)
        {
            //Save current project.
            bool bCanceled = false;
            SaveCloseCurrent(ref bCanceled);
            if (bCanceled)
                return;

            try
            {
                if (!File.Exists(path))
                {
                    ConfigFileManager.RemoveFile(path);
                    _ListEventAggregator.GetEvent<FlashRecentList>().Publish(null);
                    return;
                }
                _docService.Open(path);

                if (_docService.Document == null)
                {
                    _docService.Close();

                    ConfigFileManager.RemoveFile(path);
                    _ListEventAggregator.GetEvent<FlashRecentList>().Publish(null);
                    return;
                }

                if (_docService.Document.DocumentType == DocumentType.Library)
                {
                    IsLibraryEditMode = true;
                }
                else
                {
                    IsLibraryEditMode = false;
                }

                AddDefaultNote(_docService);

                ConfigFileManager.InsertFile(path);

                ClearPreviewFolder();

                if (_docService.Document != null)
                {
                    _ListEventAggregator.GetEvent<FlashRecentList>().Publish(null);
                    _ListEventAggregator.GetEvent<DomLoadedEvent>().Publish(FileOperationType.Open);
                }

                AutoSaveService.Instance.OpenFile(_docService);
            }
            catch (HigherDocumentVersionException)
            {
                ConfigFileManager.RemoveFile(path);
                _ListEventAggregator.GetEvent<FlashRecentList>().Publish(null);

                MessageBox.Show(GlobalData.FindResource("Common_UpdateNotice"), GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                ConfigFileManager.RemoveFile(path);
                _ListEventAggregator.GetEvent<FlashRecentList>().Publish(null);

                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private bool SaveFile()
        {
            NLogger.Info("start to save file...");
            bool res = false;
            IPagePropertyData page = ServiceLocator.Current.GetInstance<SelectionServiceProvider>().GetCurrentPage();
            if (page != null && page.EditorCanvas != null)
            {
                (page as PageEditorViewModel).CreatePreviewImage();
            }

            try
            {
                //save current project, and select a file path if it's a ne project.20140218

                if (_docService.Document != null
                    && !string.IsNullOrEmpty(_docService.Document.Name)
                    && _docService.Document.Name.Contains(AutoSaveService.Instance.Tmp))
                {
                    var beforeGuid = _docService.Document.Guid;
                    string fileName = OpenSaveDialog(_docService.Document);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        SetCustomWdgIcon();
                        _docService.Save(fileName, true);                        

                        NLogger.Info("Save file finished, save copy is true,document name: {0}, document guid: {1}", _docService.Document.Name, _docService.Document.Guid);
                        ConfigFileManager.InsertFile(fileName);
                        if (_docService.Document != null)
                        {
                            _ListEventAggregator.GetEvent<FlashRecentList>().Publish(null);
                            AutoSaveService.Instance.FileSaveAs(beforeGuid, _docService.Document.Guid);
                        }
                        res = true;
                    }
                }
                else if (_docService.Document != null && _docService.Document.IsDirty)
                {
                    if (string.IsNullOrEmpty(_docService.Document.Name))
                    {
                        string fileName = OpenSaveDialog(_docService.Document);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            SetCustomWdgIcon();
                            _docService.Save(fileName);
 

                            NLogger.Info("save file finished, save copy is false1,document name: {0}, document guid: {1}", _docService.Document.Name, _docService.Document.Guid);
                            ConfigFileManager.InsertFile(fileName);
                            if (_docService.Document != null)
                            {
                                _ListEventAggregator.GetEvent<FlashRecentList>().Publish(null);
                            }
                            res = true;
                        }
                    }
                    else
                    {
                        SetCustomWdgIcon();
                        _docService.Save(_docService.Document.Name);                        

                        NLogger.Info("save file finished, save copy is false2, document name: {0}, document guid: {1}", _docService.Document.Name, _docService.Document.Guid);
                        res = true;
                    }

                    AutoSaveService.Instance.ManualSave();
                }
                else
                {
                    if (_docService.Document == null)
                        NLogger.Debug("save skiped: doccument is null");
                    else
                    {
                        NLogger.Info("save skiped:  Document is Dirty? ({0}), Document.Name? ({1}), Document guid? ({2})",
                            _docService.Document.IsDirty,
                            _docService.Document.Name,
                            _docService.Document.Guid);
                        res = true;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                NLogger.Warn("save file failed: {0}", e.Message);
            }
            return res;
        }

        private void SaveFileAs()
        {
            try
            {
                //save current project as another project.20140218
                if (_docService.Document != null)
                {
                    string fileName = OpenSaveDialog(_docService.Document);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var beforeGuid = _docService.Document.Guid;
                        _docService.Save(fileName);

                        NLogger.Info("save file finished, file name: {0}", fileName);
                        ConfigFileManager.InsertFile(fileName);
                        if (_docService.Document != null)
                        {
                            _ListEventAggregator.GetEvent<FlashRecentList>().Publish(null);
                            AutoSaveService.Instance.FileSaveAs(beforeGuid, _docService.Document.Guid);
                        }
                    }
                    else
                    {
                        NLogger.Warn("file name is empty, save as skiped.");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                NLogger.Warn("save file as failed: {0}", e.Message);
            }
        }
        private void CloseProject()
        {
            try
            {
                //Cann't close file when it's generating Html
                var htmlService = ServiceLocator.Current.GetInstance<IHtmlServiceProvider>();
                if (htmlService.IsHtmlGenerating)
                    return;

                //save current project and then close it.20140218
                bool bCanceled = false;
                SaveCloseCurrent(ref bCanceled);

                ClearPreviewFolder();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CreateProject()
        {
            try
            {
                //save and close current project 
                bool bCanceled = false;
                SaveCloseCurrent(ref bCanceled);
                if (bCanceled)
                    return;

                //create a new project, then publish model-update message.20140218
                if (IsLibraryEditMode == false)
                {
                    _docService.NewDocument(DocumentType.Standard);
                }
                else
                {
                    _docService.NewDocument(DocumentType.Library);
                }

                if (_welcomeScreenWindow != null)
                {
                    _welcomeScreenWindow.HideDialog();
                }
                SetDeviceWindow device = new SetDeviceWindow();
                device.Owner = Application.Current.MainWindow;
                bool? bRValue = device.ShowDialog();
                if (bRValue !=null &&(bool)bRValue)
                {
                    if (_welcomeScreenWindow != null)
                    {
                        _welcomeScreenWindow.Close();
                    }
                    AddDefaultNote(_docService);
                    ClearPreviewFolder();
                    _ListEventAggregator.GetEvent<DomLoadedEvent>().Publish(FileOperationType.Create);
                    AutoSaveService.Instance.OpenFile(_docService);
                }
                else
                {
                    if (_welcomeScreenWindow != null)
                    {
                        _welcomeScreenWindow.UnHideDialog();
                    }
                    _docService.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Save and close current project.
        /// </summary>
        /// <param name="_docService"></param>
        /// <returns>false if canceled </returns>
        private void SaveCloseCurrent(ref bool bCanceled)
        {
            SaveCurrent(ref bCanceled);
            if (bCanceled == false)
            {
                CloseCurrent();
            }
        }

        private void SaveCurrent(ref bool bCanceled)
        {
            #region Set EditorCanvas focused
            //when open/close/create, set canvas focus to let other panels lost focus.
            //fix the bug page/widget note can't be saved if close/create/open project after edit note(focus in edit box). 
            IPagePropertyData page = ServiceLocator.Current.GetInstance<SelectionServiceProvider>().GetCurrentPage();
            if (page != null && page.EditorCanvas != null)
            {
                (page as PageEditorViewModel).CreatePreviewImage();
                page.EditorCanvas.Focus();
            }
            #endregion

            if (_docService.Document != null && _docService.Document.IsDirty)
            {
                MessageBoxResult Res = MessageBox.Show(GlobalData.FindResource("Toolbar_SaveProjectAlert"), GlobalData.FindResource("Toolbar_Save"),
                 MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (MessageBoxResult.Cancel == Res)
                {
                    bCanceled = true;
                    NLogger.Info("Save canceled.");
                    return;
                }

                if (MessageBoxResult.Yes == Res)
                {
                    bCanceled = !SaveFile();
                    return;
                }

                if (Res == MessageBoxResult.No)
                {
                    AutoSaveService.Instance.CloseWithoutSave();
                }
                NLogger.Warn("save process skiped,current state:  Document is Dirty? ({0}), Document.Name? ({1}), be canceled? ({2})",
                    _docService.Document.IsDirty,
                    _docService.Document.Name,
                    bCanceled);
            }

        }

        private void CloseCurrent()
        {
            if (_docService.Document != null)
            {
                //Close current project.
                _ListEventAggregator.GetEvent<DomLoadedEvent>().Publish(FileOperationType.Close);
                AutoSaveService.Instance.CloseFile(_docService);
                _docService.Close();

                // Clear the clone cache date in select service when document is closed.
                ISelectionService select = ServiceLocator.Current.GetInstance<ISelectionService>();
                if (select != null)
                {
                    select.ClearCloneCacheData();
                }
            }
        }

        /// <summary>
        /// Show saveFile dialog
        /// </summary>
        /// <returns>filename</returns>
        private string OpenSaveDialog(IDocument doc)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            if(doc.DocumentType==DocumentType.Standard)
            {
                saveFile.Filter = _standardDocFilter;
            }
            else
            {
                saveFile.Filter = CommonDefine.LibraryFilter;
            }
            
            if (saveFile.ShowDialog() == true)
            {
                return saveFile.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region private function 
        private void HTMLExport()
        {
            if (_docService != null && _docService.Document != null)
            {
                PreviewParameter para = new PreviewParameter();
                System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderDialog.Description = GlobalData.FindResource("Toolbar_HtmlDlg_SlectPath");
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    para.SavePath = folderDialog.SelectedPath;

                    //Create the parent folder name : file name or Untitled (If there's no title.) 
                    if (string.IsNullOrEmpty(_docService.Document.Title))
                    {
                        para.SavePath += @"\Untitled";
                    }
                    else
                    {
                        para.SavePath += @"\";
                        para.SavePath += _docService.Document.Title;
                    }


                    para.IsBrowerOpen = false;

                    // Do not generate mobile viewer html files.
                    _docService.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.GenerateMobileFiles = false;
                    _ListEventAggregator.GetEvent<GenerateHTMLEvent>().Publish(para);
                }


            }
        }

        private void PreviewProject(bool CurStart = false)
        {
            try
            {
                // Save html files in temp folder as this is Preview html. 
                // IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();

                //Get save path from project file, it's redundancy now..
                _previewPath = CommonFunction.GetPreViewTempPath();

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
                PreviewParameter para = new PreviewParameter();
                para.SavePath = _previewPath;
                para.IsBrowerOpen = true;
                para.IsPreviewCurrentPage = CurStart;

                if (_docService != null && _docService.Document != null)
                {
                    _docService.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.GenerateMobileFiles = true;
                }
                _ListEventAggregator.GetEvent<GenerateHTMLEvent>().Publish(para);
            }
            catch (Exception ex)
            {

            }

        }

        private void PublishToServer()
        {
            try
            {
                //Save project first before publish.
                if (string.IsNullOrEmpty(_docService.Document.Name))
                {
                    var result = MessageBox.Show(GlobalData.FindResource("Publish_SaveAlert_MSG"), GlobalData.FindResource("Publish_SaveAlert_Tiltle"), MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Cancel)
                        return;                   
                }

                var saveRet = SaveFile();
                if (saveRet == false)
                    return;

                if (_docService.Document != null)
                {
                    NLogger.Debug("Start publish,Name->" + _docService.Document.Name + ";GUID->" + _docService.Document.Guid.ToString());
                    _docService.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.GenerateMobileFiles = true;
                    _ListEventAggregator.GetEvent<PublishHTMLEvent>().Publish(true);
                }
            }
            catch (Exception e)
            {
                NLogger.Debug("PublishToServer error,message is" + e.Message.ToString());
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Add default note, include page and widget note
        /// </summary>
        /// <param name="doc"></param>
        private void AddDefaultNote(IDocumentService doc)
        {
            IAnnotationFieldSet pageNoteFieldSet = doc.Document.PageAnnotationFieldSet;
            if (pageNoteFieldSet.AnnotationFields.Contains("Default") == false)
            {
                pageNoteFieldSet.CreateAnnotationField("Default", AnnotationFieldType.Text);
            }

            IAnnotationFieldSet widgetNoteFieldSet = doc.Document.WidgetAnnotationFieldSet;
            if (widgetNoteFieldSet.AnnotationFields.Contains("Description") == false)
            {
                widgetNoteFieldSet.CreateAnnotationField("Description", AnnotationFieldType.Text);
            }
        }

        private void UpdateWindowTitle()
        {
            string tTtitle = PRODUCT_NAME;
            if (_docService != null)
            {
                if (_docService.Document != null)
                {
                    if (_docService.Document.DocumentType == DocumentType.Library)
                    {
                        tTtitle += " > Library";
                    }

                    tTtitle += " - ";
                    if (String.IsNullOrEmpty(_docService.Document.Title))
                    {
                        tTtitle += "Untitled Document";
                    }
                    else
                    {
                        tTtitle += _docService.Document.Title;
                    }
                }
            }

            Title = tTtitle;
        }

        private void ClearPreviewFolder()
        {
            try
            {
                //Delete preview folder when current document is closed.
                if (Directory.Exists(_previewPath))
                {
                    Directory.Delete(_previewPath, true);
                }
            }
            catch
            {
            }
        }



        #endregion

        #region Command Handler

        public DelegateCommand<object> NewCommand { get; private set; }
        public DelegateCommand<object> OpenCommand { get; private set; }
        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> SaveAsCommand { get; private set; }
        public DelegateCommand<object> CloseCommand { get; private set; }
        public DelegateCommand<object> LanguageSettingCommand { get; private set; }
        public DelegateCommand<object> FormatPaintCommand { get; private set; }

        public DelegateCommand<object> LinkFeedbackCommand { get; private set; }
        public DelegateCommand<object> PublishProject { get; private set; }
        public DelegateCommand<object> PreviewProjectCommand { get; private set; }
        public DelegateCommand<string> OpenRecentCommand { get; private set; }
        public DelegateCommand<object> OpenSampleCommand { get; private set; }
        public DelegateCommand<object> HelpCommand { get; private set; }
        public DelegateCommand<object> NewPageCommand { get; private set; }


        public void NewExecute(object cmdParameter)
        {
            FileOPerationHandler(FileOperationType.Create);
        }
        public void OpenExecute(object cmdParameter)
        {
            FileOPerationHandler(FileOperationType.Open);
        }
        public void SaveExecute(object cmdParameter)
        {
            FileOPerationHandler(FileOperationType.Save);
        }
        public void SaveAsExecute(object cmdParameter)
        {
            FileOPerationHandler(FileOperationType.SaveAs);
        }
        public void LanguageSettingExecute(object cmdParameter)
        {
            _ListEventAggregator.GetEvent<OpenDialogEvent>().Publish(DialogType.LanguageSetting);

        }
        public void CloseExecute(object cmdParameter)
        {
            FileOPerationHandler(FileOperationType.Close);
        }

        public void PublishToDesignStudio(object cmdParameter)
        {
            PublishToServer();
        }

        public void PreviewDocment(object cmdParameter)
        {
            if (cmdParameter != null)
            {
                bool bStarCurPage = Convert.ToBoolean(cmdParameter);
                PreviewProject(bStarCurPage | (!GlobalData.IsDefaultPreview));
            }
        }
        public void LinkFeedbackExecute(object cmdParameter)
        {
            try
            {
                System.Diagnostics.Process.Start(CommonDefine.UrlFeedBack);
                IsAboutOpen = true;
            }
            catch(Exception exp)
            {
                System.Diagnostics.Debug.WriteLine("Open URL error : {0}.", exp.Message);
            }
        }

        private void PaintFormatExcute(object parameter)
        {
            _ListEventAggregator.GetEvent<FormatPaintEvent>().Publish(null);
        }

        private bool CanPaintFormat(object parameter)
        {
            ISelectionService select = ServiceLocator.Current.GetInstance<ISelectionService>();
            if (select.WidgetNumber == 1)
            {
                List<IWidgetPropertyData> allSelects = select.GetSelectedWidgets();

                WidgetViewModBase date = allSelects[0] as WidgetViewModBase;

                if (date != null && date.IsGroup == false)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion Command Handler

        #region VM Message Handler
        public void OnOpenSitemapPage(bool bHome)
        {
            if (bHome)
                PublishEventToSitemap(SiteMapEventEnum.OpenHomePage);
            else
                PublishEventToSitemap(SiteMapEventEnum.OpenEndPage);
        }

        public void OnCreateNewPage(object Parameter)
        {
            PublishEventToSitemap(SiteMapEventEnum.CreateNewPage);
        }

        private void PublishEventToSitemap(SiteMapEventEnum Data)
        {
            if (_docService != null && _docService.Document != null)
            {
                _ListEventAggregator.GetEvent<SiteMapEvent>().Publish(Data);
            }
        }

        private void SetCustomWdgIcon()
        {
            ISelectionService SelectionSrv = ServiceLocator.Current.GetInstance<ISelectionService>();
            if (SelectionSrv != null)
            {
                PageEditorViewModel page = SelectionSrv.GetCurrentPage() as PageEditorViewModel;
                if(page==null)
                {
                    return;
                }

                if(page.IsUseThumbnailAsIcon==true && page.GetIsThumbnailUpdate()==true)
                {
                    page.CreateCustomWidgetIcon(true);                    
                }
            }

        }

        public void FileOPerationHandler(FileOperationType OpType)
        {
            switch (OpType)
            {
                case FileOperationType.Open:
                    OpenFile();
                    break;
                case FileOperationType.Save:
                    SaveFile();
                    break;
                case FileOperationType.Close:
                    CloseProject();
                    break;
                case FileOperationType.Create:
                    CreateProject();
                    break;
                case FileOperationType.SaveAs:
                    SaveFileAs();
                    break;
                case FileOperationType.Publish:
                    PublishToServer();
                    break;
                case FileOperationType.Preview:
                    PreviewProject(!GlobalData.IsDefaultPreview);
                    break;
                case FileOperationType.HTMLExport:
                    HTMLExport();
                    break;
            }

            UpdateWindowTitle();
        }

        public void OpenFileHandler(string parameter)
        {
            if (!String.IsNullOrEmpty(parameter))
            {
                OpenFile(parameter);

                UpdateWindowTitle();
            }
        }

        private void RecoveryFileHandler(object parameter)
        {

            if (parameter is RecoveryFile)
            {
                var programdata = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var tmp = AutoSaveService.Instance.Tmp;
                var toRecovery = parameter as RecoveryFile;
                NLogger.Info("Try to recovery file from recoverylist window.[Guid {0}] [CreateTime {1}] [Type {2}] [Filename {3}] [Location {4}] [FullFilename {5}]",
                toRecovery.Guid,
                toRecovery.CreateTime,
                toRecovery.Type,
                toRecovery.Filename,
                toRecovery.FileType,
                toRecovery.Location,
                toRecovery.GetFullPath());
                try
                {
                    var path = Path.Combine(tmp, Guid.NewGuid().ToString());

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    path = Path.Combine(path, string.Format("{0}{1}", toRecovery.Filename, 
                        string.IsNullOrEmpty(toRecovery.FileType) ? ".pn" : toRecovery.FileType));

                    NLogger.Info("The full path of tmp file is [{0}].Try to copy recovery file to the tmp path.", path);
                    File.Copy(toRecovery.GetFullPath(), path, true);
                    NLogger.Info("Copy recovery file to the tmp path successfully.");
                    OpenFileHandler(path);
                    NLogger.Info("Open the tmp file.");
                }
                catch (Exception ex)
                {
                    NLogger.Warn("Recovery file failed.ex:{0}", ex.ToString());
                }
            }
        }

        public void OnOpenHelpFile(object parameter)
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", CommonDefine.UrlGuide);
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine("Open URL error : {0}.", exp.Message);
            }
        }


        public void OpenDialogEventHandler(DialogType type)
        {
            //if(type == DialogType.AdaptiveView)
            //{
            //    AdaptiveWindow win = new AdaptiveWindow();
            //    win.Owner = Application.Current.MainWindow;
            //    win.ShowDialog();
            //}
            //else 
            if (type == DialogType.AboutPopup)
            {
                IsAboutOpen = true;
            }
        }

        private void UpdateLanguageEventHandler(object obj)
        {
            // Add font family in system style according to variant language, so that new widget can inherited this value.
            if (_docService != null)
            {
                IStyle systemStyle = _docService.WidgetSystemStyle;
                systemStyle.SetStyleProperty(StylePropertyNames.FONT_FAMILY_PROP, CommonFunction.GetDefaultFontNameByLanguage());
            }
        }

        private void ExportToMyLibraryHandler(object obj)
        {
            SetCustomWdgIcon();
        }

        private void DisplayAppLoadingHandler(bool parameter)
        {
            if(parameter)
            {
                IsShowAppLoading = Visibility.Visible;
            }
            else
            {
                IsShowAppLoading = Visibility.Collapsed;
            }
        }

        private void AutoSaveSettingChangedHandler(object obj)
        {
            NLogger.Info("AutoSaveSetting changed, try to re-perform setting of recovery.");
            AutoSaveService.Instance.PerformSetting();
        }

        #endregion VM Message Handler

        #region Exit Application
        //Return value will decide whether the App will exit.
        public bool AppExit()
        {
            try
            {
                ClearPreviewFolder();

                if (_docService.Document == null)
                {
                    //No save operation because there's not any opened project
                    return true;
                }
                else if (_docService.Document.IsDirty == false)
                {
                    //No save operation because there's not any opened project
                    return true;
                }
                else
                {
                    MessageBoxResult Res = MessageBox.Show(GlobalData.FindResource("Toolbar_SaveProjectAlert"), GlobalData.FindResource("Toolbar_Save"),
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if (MessageBoxResult.Yes == Res)
                    {
                        //Save the current project and continue to exit
                        if (string.IsNullOrEmpty(_docService.Document.Name) || _docService.Document.IsDirty == true)
                        {
                            bool res = SaveFile();
                            return res;
                        }
                        return true;
                    }
                    else if (MessageBoxResult.No == Res)
                    {
                        AutoSaveService.Instance.CloseWithoutSave();
                        //Don't save the current project and continue to exit
                        return true;
                    }
                    else
                    {
                        //Don't Exit
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return true;
        }
        #endregion Exit Application

    }
    public class RecentFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
