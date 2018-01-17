using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;
using MainToolBar.Common;
using System.Windows.Documents;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Naver.Compass.InfoStructure;
using System.Windows.Input;
using System.Windows.Data;
using Naver.Compass.Module;
using Naver.Compass.Common.Helper;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.MainToolBar.Module;
using System.Globalization;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;
using Naver.Compass.Service;
using Naver.Compass.Service.Update;
using Naver.Compass.Common;
using System.Windows.Interop;
using Naver.Compass.Common.Win32;
using System.Diagnostics;
using System.IO;

namespace MainToolBar.ViewModel
{

    partial class RibbonViewModel : ViewModelBase
    {

        public RibbonViewModel()
        {
            // _ListEventAggregator.GetEvent<OpenDialogEvent>().Subscribe(ChangeLanEventHandler);
            _ListEventAggregator.GetEvent<FlashRecentList>().Subscribe(FlashRecentFile);
            _model = new RibbonModel();
            _ListEventAggregator.GetEvent<DomLoadedEvent>().Subscribe(DomLoadedEventHandler, ThreadOption.UIThread);

            _doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            _updateService = ServiceLocator.Current.GetInstance<IUpdateService>();
            _updateService.UpdateProgressChanging += UpdateService_ProgressChanging;
            

            InitCommandBinding();
            InitToolbarViewData();

            //Register the Selection Service Handler
            _ListEventAggregator.GetEvent<SelectionChangeEvent>().Subscribe(SelectionChangeEventHandler);
            _ListEventAggregator.GetEvent<SelectionPropertyChangeEvent>().Subscribe(SelectionPropertyEventHandler);
            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeEventHandler);
            _ListEventAggregator.GetEvent<OpenPanesEvent>().Subscribe(OpenPanesEventHandler);

            _ListEventAggregator.GetEvent<CheckUpdateCompletedEvent>().Subscribe(CheckUpdateCompletedHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<UpdateProcessEvent>().Subscribe(UpdateProcessHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<OpenDialogEvent>().Subscribe(OpenDialogEventHandler);
            _ListEventAggregator.GetEvent<UpdateLanguageEvent>().Subscribe(UpdateLanguageHandler);
            _ListEventAggregator.GetEvent<RecoveryDocumentOpenEvent>().Subscribe(DocumentRecoveryAction);

            _ListEventAggregator.GetEvent<TBUpdateEvent>().Subscribe(UpdateToolbarUI);

            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();

            _ListEventAggregator.GetEvent<EnableFirePositionInRibbonEvent>().Subscribe(EnableFirePositionHandler);
        }

        private void InitCommandBinding()
        {
            this.NewDocmentCommand = new DelegateCommand<object>(NewAction);
            this.OpenDocmentCommand = new DelegateCommand<object>(OpenAction);
            this.SaveDocmentCommand = new DelegateCommand<object>(SaveAction, CanDocmentOpen);
            this.CloseDocmentCommand = new DelegateCommand<object>(CloseAction, CanDocmentOpen);
            this.PreviewDocmentCommand = new DelegateCommand<object>(PreviewExecuted, CanDocmentOpen);
            this.PublishDocmentCommand = new DelegateCommand<object>(PublishAction, CanPublish);
            this.AdativeSettingCommand = new DelegateCommand<object>(AdativeSettingAction);
            this.CloseWindowCommand = new DelegateCommand<object>(CloseWindowExecure);
            this.LanguageSettingCommand = new DelegateCommand<object>(LanguageSettingAction);
            this.SaveAsDocmentCommand = new DelegateCommand<object>(SaveAsAction, CanDocmentOpen);
            this.ExportToMyLibraryCommand = new DelegateCommand<object>(ExportToMyLibraryAction, CanDocmentOpen);
            this.HTMLExportCommand = new DelegateCommand<object>(HtmlExortAction, CanDocmentOpen);
            this.AutoSaveCommand = new DelegateCommand<object>(AutoSaveAction, CanDocmentOpen);
            this.DocumentRecoveryCommand = new DelegateCommand<object>(DocumentRecoveryAction);
            this.CheckPanesCommand = new DelegateCommand<object>(CheckPanesAction);
            this.CheckPreviewCommand = new DelegateCommand<object>(CheckPreviewAction);
            this.ResetViewsCommand = new DelegateCommand<object>(ResetViewsAction);
            this.AboutDSCommand = new DelegateCommand<object>(AboutDSAction);
            this.WelcomeScreenCommand = new DelegateCommand<object>(WelcomeScreenAction);
            this.TipsCommand = new DelegateCommand<object>(TipsAction);
            this.CheckUpdateCommand = new DelegateCommand<object>(CheckUpdateAction);
            this.OpenGridGuideCommand = new DelegateCommand<object>(OpenGridGuideAction);
            this.OpenRecentCommand = new DelegateCommand<object>(OpenRecentExecute);
            this.ClearRecentCommand = new DelegateCommand<object>(ClearRecentExecute);
            this.FormatPaintCommand = new DelegateCommand<object>(PaintFormatExcute, CanPaintFormat);
            this.VersioningCommand = new DelegateCommand<object>(VersioningExecute);
            this.InstallUpdateCommand = new DelegateCommand<object>(InstallUpdateExecute);
        }

        #region Private Data

        private const string HelpFooterTitle = "Press F1 for more help.";
        private static object _lockObject = new object();
        private static Dictionary<string, ControlData> _dataCollection = new Dictionary<string, ControlData>();

        // Store any data that doesnt inherit from ControlData
        private static Dictionary<string, object> _miscData = new Dictionary<string, object>();
        protected static RibbonModel _model;
        private static bool beOpenDocument;

        private static Dictionary<string, bool> _ToolbarCheckData = new Dictionary<string, bool>();
        private static Dictionary<string, string> _ToolbarView = new Dictionary<string, string>();

        private ISelectionService _selectionService;
        private bool _isFirePositionEnabled = true;
        private bool _isCheckUpdateFromMenu = false;

        private IDocumentService _doc = null;
        private IUpdateService _updateService;

        private bool _isUpdateProgressShow;
        private double _updateProcess;
        private string _targetVersion;
        private bool _isRunUpdateEnabled;

        #endregion Data

        #region Property

        public static Dictionary<string, ControlData> DataCollection
        {
            get { return _dataCollection; }
        }

        private bool isSitemapOpen = true;
        public bool IsSitemapOpen
        {
            get
            {
                return isSitemapOpen;
            }
            set
            {
                if (isSitemapOpen != value)
                {
                    isSitemapOpen = value;
                    FirePropertyChanged("IsSitemapOpen");
                }
            }
        }

        private bool isWidgetsOpen = true;
        public bool IsWidgetsOpen
        {
            get
            {
                return isWidgetsOpen;
            }
            set
            {
                if (isWidgetsOpen != value)
                {
                    isWidgetsOpen = value;
                    FirePropertyChanged("IsWidgetsOpen");
                }
            }
        }

        private bool isMastersOpen = true;
        public bool IsMasterOpen
        {
            get
            {
                return isMastersOpen;
            }
            set
            {
                if (isMastersOpen != value)
                {
                    isMastersOpen = value;
                    FirePropertyChanged("IsMasterOpen");
                }
            }
        }

        private bool isInteractionOpen = true;
        public bool IsInteractionOpen
        {
            get
            {
                return isInteractionOpen;
            }
            set
            {
                if (isInteractionOpen != value)
                {
                    isInteractionOpen = value;
                    FirePropertyChanged("IsInteractionOpen");
                }
            }
        }
        private bool isWidgetPropOpen = true;
        public bool IsWidgetPropOpen
        {
            get
            {
                return isWidgetPropOpen;
            }
            set
            {
                if (isWidgetPropOpen != value)
                {
                    isWidgetPropOpen = value;
                    FirePropertyChanged("IsWidgetPropOpen");
                }
            }
        }

        private bool isPagePropOpen = true;
        public bool IsPagePropOpen
        {
            get
            {
                return isPagePropOpen;
            }
            set
            {
                if (isPagePropOpen != value)
                {
                    isPagePropOpen = value;
                    FirePropertyChanged("IsPagePropOpen");
                }
            }
        }

        private bool isPageIconOpen = false;
        public bool IsPageIconOpen
        {
            get
            {
                return isPageIconOpen;
            }
            set
            {
                if (isPageIconOpen != value)
                {
                    isPageIconOpen = value;
                    FirePropertyChanged("IsPageIconOpen");
                }
            }
        }

        private bool isPageWidgetManagerOpen = false;
        public bool IsPageWidgetManagerOpen
        {
            get
            {
                return isPageWidgetManagerOpen;
            }
            set
            {
                if (isPageWidgetManagerOpen != value)
                {
                    isPageWidgetManagerOpen = value;
                    FirePropertyChanged("IsPageWidgetManagerOpen");
                }
            }
        }

        public bool IsExport2ImageEnabled
        {
            get { return beOpenDocument; }
        }

        public bool IsExport2Image
        {
            get
            {
                if (_doc.Document != null)
                {
                    return _doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.ExportType == ExportType.ImageFile;
                }
                return false;
                
            }
            set
            {
                if (_doc.Document!= null)
                {
                    if (value)
                    {
                        _doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.ExportType = ExportType.ImageFile;
                    }
                    else
                    {
                        _doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.ExportType = ExportType.Data;
                    }
                    _doc.Document.IsDirty = true;
                }
            }
        }

        public bool IsUpdateProgressShow
        {
            get
            {
                return _isUpdateProgressShow;
            }
            set
            {
                if(_isUpdateProgressShow!=value)
                {
                    _isUpdateProgressShow = value;
                    FirePropertyChanged("IsUpdateProgressShow");
                }
            }
        }

        public double UpdateProgress
        {
            get
            {
                return _updateProcess;
            }
            set
            {
                if(_updateProcess!=value)
                {
                    _updateProcess = value;
                    FirePropertyChanged("UpdateProgress");
                }
            }
        }

        public string TargetVersion
        {
            get
            {
                return _targetVersion;
            }
            set
            {
                if(_targetVersion!=value)
                {
                    _targetVersion = value;
                    FirePropertyChanged("TargetVersion");
                }
            }
        }

        public bool IsRunUpdateEnabled
        {
            get
            {
                return _isRunUpdateEnabled;
            }
            set
            {
                if(_isRunUpdateEnabled!=value)
                {
                    _isRunUpdateEnabled = value;
                    FirePropertyChanged("IsRunUpdateEnabled");
                }
            }
        }

        #endregion

        #region Command Define
        public DelegateCommand<object> NewDocmentCommand { get; private set; }
        public DelegateCommand<object> OpenDocmentCommand { get; private set; }
        public DelegateCommand<object> SaveDocmentCommand { get; private set; }
        public DelegateCommand<object> SaveAsDocmentCommand { get; private set; }
        public DelegateCommand<object> ExportToMyLibraryCommand { get; private set; }
        public DelegateCommand<object> HTMLExportCommand { get; private set; }
        public DelegateCommand<object> AutoSaveCommand { get; private set; }
        public DelegateCommand<object> DocumentRecoveryCommand { get; private set; }
        public DelegateCommand<object> PreviewDocmentCommand { get; private set; }
        public DelegateCommand<object> PublishDocmentCommand { get; private set; }
        public DelegateCommand<object> AdativeSettingCommand { get; private set; }
        public DelegateCommand<object> CloseDocmentCommand { get; private set; }
        public DelegateCommand<object> CloseWindowCommand { get; private set; }
        public DelegateCommand<object> LanguageSettingCommand { get; private set; }
        public DelegateCommand<object> CheckPanesCommand { get; private set; }
        public DelegateCommand<object> CheckPreviewCommand { get; private set; }
        public DelegateCommand<object> ResetViewsCommand { get; private set; }
        public DelegateCommand<object> OpenRecentCommand { get; private set; }
        public DelegateCommand<object> ClearRecentCommand { get; private set; }
        public DelegateCommand<object> AboutDSCommand { get; private set; }
        public DelegateCommand<object> WelcomeScreenCommand { get; private set; }
        public DelegateCommand<object> TipsCommand { get; private set; }
        public DelegateCommand<object> CheckUpdateCommand { get; private set; }
        public DelegateCommand<object> OpenGridGuideCommand { get; private set; }
        public DelegateCommand<object> FormatPaintCommand { get; private set; }

        public DelegateCommand<object> VersioningCommand { get; private set; }
        public DelegateCommand<object> InstallUpdateCommand { get; private set; }

        #endregion

        #region Command handle function

        private void OpenAction(object parameter)
        {
            _ListEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.Open);
        }

        private void OpenRecentExecute(object parameter)
        {
            _ListEventAggregator.GetEvent<OpenFileEvent>().Publish(parameter as string);

        }
        private void ClearRecentExecute(object parameter)
        {
            ConfigFileManager.RemoveAllFile();
            _ListEventAggregator.GetEvent<FlashRecentList>().Publish(null);
        }

        private void NewAction(object parameter)
        {
            _ListEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.Create);
        }

        private void SaveAction(object parameter)
        {
            _ListEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.Save);
        }

        private void SaveAsAction(object parameter)
        {
            _ListEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.SaveAs);
        }

        /// <summary>
        /// 1.Export current to local path.
        /// 2.Send message to other program to refresh library.
        /// </summary>
        /// <param name="pararmeter"></param>
        private void ExportToMyLibraryAction(object pararmeter)
        {
            if (_doc.Document.DocumentType != DocumentType.Library)
                return;

            _ListEventAggregator.GetEvent<ExportToMYLibraryEvent>().Publish(null);
               
        }
        private void HtmlExortAction(object parameter)
        {
            _ListEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.HTMLExport);
        }
        private void AutoSaveAction(object parameter)
        {
            var winSetting = new AutoSaveSettingWindow();
            winSetting.Owner = Application.Current.MainWindow;

            winSetting.ShowDialog();
        }

        private void DocumentRecoveryAction(object obj)
        {
            var documentRecovery = new DocumentRecoveryWindow();
            documentRecovery.Owner = Application.Current.MainWindow;

            documentRecovery.ShowDialog();
        }
        private void PreviewExecuted(object parameter)
        {
            _ListEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.Preview);
        }

        private bool CanDocmentOpen(object parameter)
        {
            return beOpenDocument;
        }
        private bool CanPublish(object parameter)
        {
            return beOpenDocument && _doc.Document.DocumentType == DocumentType.Standard;
        }

        private void CloseAction(object parameter)
        {
            _ListEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.Close);
        }

        private void LanguageSettingAction(object parameter)
        {

            LanguageSettingWindow winSetting = new LanguageSettingWindow();
            winSetting.Owner = Application.Current.MainWindow;

            winSetting.ShowDialog();
        }

        private void CheckPreviewAction(object parameter)
        {
            if (parameter != null)
            {
                bool isDefaultPreview = Convert.ToBoolean(parameter);

                //if (isDefaultPreview != GlobalData.IsDefaultPreview)
                //{
                    GlobalData.IsDefaultPreview = isDefaultPreview;

                    FirePropertyChanged("IsPreviewDefault");
                    FirePropertyChanged("IsPreviewCurPage");

                    _ListEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.Preview);
                //}
            }
        }

        private void CheckPanesAction(object parameter)
        {
            string str = parameter as string;
            ActivityPane pane = new ActivityPane();
            pane.Name = str;
            switch (str)
            {
                case CommonDefine.PaneSitemap:
                    pane.bOpen = IsSitemapOpen;
                    break;
                case CommonDefine.PaneWidgets:
                    pane.bOpen = IsWidgetsOpen;
                    break;
                case CommonDefine.PaneMaster:
                    pane.bOpen = IsMasterOpen;
                    break;
                case CommonDefine.PaneWidgetProp:
                    pane.bOpen = IsWidgetPropOpen;
                    break;
                case CommonDefine.PanePageProp:
                    pane.bOpen = IsPagePropOpen;
                    break;
                case CommonDefine.PaneInteraction:
                    pane.bOpen = IsInteractionOpen;
                    break;
                case CommonDefine.PanePageIcon:
                    pane.bOpen = IsPageIconOpen;
                    break;
                case CommonDefine.PaneWidgetManager:
                    pane.bOpen = IsPageWidgetManagerOpen;
                    break;
            }
            pane.bFromToolbar = true;
            _ListEventAggregator.GetEvent<OpenPanesEvent>().Publish(pane);
        }
        private void ResetViewsAction(object parameter)
        {
            _ListEventAggregator.GetEvent<ChangeLayoutEvent>().Publish("LoadDefault");
            IsPageWidgetManagerOpen = IsSitemapOpen = isMastersOpen = IsWidgetPropOpen = IsWidgetsOpen = IsInteractionOpen = true;
            if (_doc.Document != null && _doc.Document.DocumentType == DocumentType.Library)
            {
                IsPageIconOpen = true;
            }
            else
            {
                IsPagePropOpen = true;
            }
            IsShowToolbar = true;
        }

        private void AboutDSAction(object parameter)
        {
            _ListEventAggregator.GetEvent<OpenDialogEvent>().Publish(DialogType.AboutPopup);
        }
        private void WelcomeScreenAction(object parameter)
        {
            _ListEventAggregator.GetEvent<OpenDialogEvent>().Publish(DialogType.WelcomeScreen);
        }

        private void TipsAction(object parameter)
        {
            try
            {
                System.Diagnostics.Process.Start(CommonDefine.UrlGuide);
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine("Open URL error : {0}.", exp.Message);
            }
        }
        private void CheckUpdateAction(object parameter)
        {
            _isCheckUpdateFromMenu = true;
            if (_updateService.IsBusy)
            {
                return;
            }

            _updateService.CheckUpdate(false);

            CheckUpdateWindow win = new CheckUpdateWindow();
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }
        private void CloseWindowExecure(object parameter)
        {
            Application.Current.MainWindow.Close();
        }

        private bool PreviewCanExecute(object parameter)
        {
            return false;
        }

        private void PublishAction(object parameter)
        {
            _ListEventAggregator.GetEvent<FileOperationEvent>().Publish(FileOperationType.Publish);
        }

        private void AdativeSettingAction(object parameter)
        {
            _ListEventAggregator.GetEvent<OpenDialogEvent>().Publish(DialogType.AdaptiveView);
        }

        private void GenerateHTMLExecuted(object parameter)
        {
            GenerateHTML win = new GenerateHTML();
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }

        private bool GenerateHTMLCanExecute()
        {
            return beOpenDocument;
        }

        private void PaintFormatExcute(object parameter)
        {
            if (IsFomratCheck)
            {
                _ListEventAggregator.GetEvent<CancelFormatPaintEvent>().Publish(false);
            }
            else
            {
                _ListEventAggregator.GetEvent<FormatPaintEvent>().Publish(null);
            }

        }

        private bool CanPaintFormat(object parameter)
        {
            if (_selectionService.WidgetNumber == 1)
            {
                List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

                WidgetViewModBase date = allSelects[0] as WidgetViewModBase;

                if (date != null && date.IsGroup == false)
                {
                    return true;
                }
            }
            return false;
        }


        private void VersioningExecute(object parameter)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Naver.Compass.Differ.exe";
            if(File.Exists(path))
            {
                Process.Start(path, GlobalData.Culture);
            }                
        }

        private void InstallUpdateExecute(object parameter)
        {
            _updateService.RunUpdatePackage();
        }

        private void StyChangeHandler(string EventArg)
        {
            if (EventArg == null)
            {
                return;
            }
            if (EventArg.Equals("vFontFamily", StringComparison.Ordinal))
            {
                FontFamilyControlData cData = _dataCollection["Font Face"] as FontFamilyControlData;
                var fontfamilyname = _model.GetPropertyFontFamilyValue("vFontFamily");
                if (!string.IsNullOrEmpty(fontfamilyname))
                {
                    //cData.SelectedFamily = cData.FontFamilies.FirstOrDefault(x => x.Name == fontfamilyname);
                    cData.SetSelectedFamily(cData.FontFamilies.FirstOrDefault(x => x.Name == fontfamilyname));
                }
                else
                {
                    cData.SelectedFamily = null;
                }
            }
            else if (EventArg.Equals("vFontSize", StringComparison.Ordinal))
            {
                FontSizeControlData fData = _dataCollection["Font Size"] as FontSizeControlData;
                var vFontSize = _model.GetPropertyFontSizeValue("vFontSize");

                var fsize = 0d;

                if (double.TryParse(vFontSize, out fsize))
                {
                  
                    fData.SetSelectedSize(fData.FontSizes.FirstOrDefault(x => x.FontSize == fsize));
                }
                else
                {
                    fData.SelectedSize = null;
                }

                fData.DisplaySize = vFontSize;      
            }

        }

        private void UpdateArrowStyleButtonParameter()
        {
            SplitMenuItemData ButtonData = _dataCollection["LineArrowStyle"] as SplitMenuItemData;
            {
                if (ButtonData != null)
                {
                    ArrowStyle styleVaule = _model.GetLineArrowStyleValue();

                    if (styleVaule != ArrowStyle.Default)
                    {
                        GalleryData<LineArrowStyleDate> BLData = _dataCollection["LineArrowStyleGallery"] as GalleryData<LineArrowStyleDate>;
                        if (BLData != null)
                        {
                            foreach (LineArrowStyleDate tData in BLData.CategoryDataCollection[0].GalleryItemDataCollection)
                            {
                                if (tData.BLStyle == styleVaule)
                                {
                                    ButtonData.CommandParameter = tData;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateBorderLineWidthButtonParameter()
        {
            SplitMenuItemData ButtonData = _dataCollection["BorderLineType"] as SplitMenuItemData;
            if (ButtonData != null)
            {
                int iVaule = _model.GetBorderLineValue("vBorderLinethinck");
                if (iVaule >= 0)
                {
                    GalleryData<BorderLineWidthData> BLData = _dataCollection["BorderLineType Gallery"] as GalleryData<BorderLineWidthData>;
                    if (BLData != null)
                    {
                        foreach (BorderLineWidthData tData in BLData.CategoryDataCollection[0].GalleryItemDataCollection)
                        {
                            if (tData.Width == iVaule)
                            {
                                ButtonData.CommandParameter = tData;
                            }
                        }
                    }
                }
            }
        }
        private void UpdateBorderLineStyleButtonParameter()
        {
            SplitMenuItemData ButtonData = _dataCollection["BorderLinePattern"] as SplitMenuItemData;
            if (ButtonData != null)
            {
                int iVaule = _model.GetBorderLineValue("vBorderlineStyle");
                if (iVaule < 0)
                {
                    ButtonData.CommandParameter = null;
                }
                else
                {
                    GalleryData<BorderLineStyleData> uData = _dataCollection["BorderLinePattern Gallery"] as GalleryData<BorderLineStyleData>;
                    if (uData != null)
                    {
                        foreach (BorderLineStyleData tData in uData.CategoryDataCollection[0].GalleryItemDataCollection)
                        {
                            if (tData.BLStyle == iVaule)
                            {
                                ButtonData.CommandParameter = tData;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateArrowStyleSelectItem()
        {
            GalleryData<LineArrowStyleDate> LAData = _dataCollection["LineArrowStyleGallery"] as GalleryData<LineArrowStyleDate>;
            if (LAData != null)
            {
                ArrowStyle styleVaule = _model.GetLineArrowStyleValue();

                if (styleVaule == ArrowStyle.Default)
                {
                    LAData.SelectedItem = null;
                }
                else
                {
                    foreach (LineArrowStyleDate tData in LAData.CategoryDataCollection[0].GalleryItemDataCollection)
                    {
                        if (tData.BLStyle == styleVaule)
                        {
                            LAData.SelectedItem = tData;

                            break;
                        }
                    }
                }
            }
        }
        private void UpdateBorderLineWidthSelectedItem()
        {
            GalleryData<BorderLineWidthData> BLData = _dataCollection["BorderLineType Gallery"] as GalleryData<BorderLineWidthData>;
            if (BLData != null)
            {
                int iVaule = _model.GetBorderLineValue("vBorderLinethinck");
                if (iVaule < 0)
                {
                    BLData.SelectedItem = null;
                }
                else
                {
                    foreach (BorderLineWidthData tData in BLData.CategoryDataCollection[0].GalleryItemDataCollection)
                    {
                        if (tData.Width == iVaule)
                        {
                            BLData.SelectedItem = tData;
                        }
                    }
                }
            }
        }
        private void UpdateBorderLineStyleSelectedItem()
        {
            GalleryData<BorderLineStyleData> uData = _dataCollection["BorderLinePattern Gallery"] as GalleryData<BorderLineStyleData>;
            if (uData != null)
            {
                int iVaule = _model.GetBorderLineValue("vBorderlineStyle");
                if (iVaule < 0)
                {
                    uData.SelectedItem = null;
                }
                else
                {
                    foreach (BorderLineStyleData tData in uData.CategoryDataCollection[0].GalleryItemDataCollection)
                    {
                        if (tData.BLStyle == iVaule)
                        {
                            uData.SelectedItem = tData;
                        }
                    }

                }
            }
        }
        private void UpdateSelectedItem()
        {
            GalleryData<StyleColor> Cdata = _dataCollection["Background Color Gallery"] as GalleryData<StyleColor>;
            if (Cdata != null)
            {
                Cdata.SelectedItem = BackgroundColorView;
                Cdata.GradientEnable = BackgroundGradientEnable;
            }

            Cdata = _dataCollection["Font Color Gallery"] as GalleryData<StyleColor>;
            if (Cdata != null)
            {
                Cdata.SelectedItem = FontColorView.ToStyleColor();
            }

            Cdata = _dataCollection["BorderLine Color Gallery"] as GalleryData<StyleColor>;
            if (Cdata != null)
            {
                Cdata.SelectedItem = BorderLineColorView;
                Cdata.GradientEnable = BorderlineGradientEnable;
            }

            UpdateBorderLineStyleSelectedItem();

            UpdateBorderLineWidthSelectedItem();

            UpdateArrowStyleSelectItem();
        }

        private void CanSupportProperty()
        {
            bool IsSupportText = _model.IsSupportProperty_Toolbar(PropertyOption.Option_Text);
            bool IsSupportBorder = _model.IsSupportProperty_Toolbar(PropertyOption.Option_Border);
            bool IsSupportBackground = _model.IsSupportProperty_Toolbar(PropertyOption.OPtion_BackColor);
            bool IsSupportArrowStyle = _model.IsSupportProperty_Toolbar(PropertyOption.Option_LineArrow);

            ComboBoxData cData = _dataCollection["Font Face"] as ComboBoxData;
            cData.IsEnable = IsSupportText;

            cData = _dataCollection["Font Size"] as ComboBoxData;
            cData.IsEnable = IsSupportText;

            SplitFontColorButton spcData = _dataCollection["Font Color"] as SplitFontColorButton;
            spcData.IsEnable = IsSupportText;

            SplitMenuItemData spData = _dataCollection["BorderLineType"] as SplitMenuItemData;
            spData.IsEnable = IsSupportBorder;

            spData = _dataCollection["BorderLinePattern"] as SplitMenuItemData;
            spData.IsEnable = IsSupportBorder;

            spData = _dataCollection["LineArrowStyle"] as SplitMenuItemData;
            spData.IsEnable = IsSupportArrowStyle;

            SplitBackgroundColorButton sbBKData = _dataCollection["BackColor"] as SplitBackgroundColorButton;
            sbBKData.IsEnable = IsSupportBackground;

            SplitBordlineColorButton sbBLData = _dataCollection["BorderLineColor"] as SplitBordlineColorButton;
            sbBLData.IsEnable = IsSupportBorder;


        }

        private void SetStyleCmdTarget(IInputElement target)
        {
            //GalleryData<FontFamily> GDate = _dataCollection["Font Face Gallery"] as GalleryData<FontFamily>;
            //if (GDate != null)
            //{
            //    GDate.CmdTarget = target;
            //}

            //GalleryData<double?> dDate = _dataCollection["Font Size Gallery"] as GalleryData<double?>;

            //if (dDate != null)
            //{
            //    dDate.CmdTarget = target;
            //}


            GalleryData<Brush> Bdata = _dataCollection["Background Color Gallery"] as GalleryData<Brush>;
            if (Bdata != null)
            {
                Bdata.CmdTarget = target;
            }

            Bdata = _dataCollection["BorderLine Color Gallery"] as GalleryData<Brush>;
            if (Bdata != null)
            {
                Bdata.CmdTarget = target;
            }

            SplitBackgroundColorButton sbBKData = _dataCollection["BackColor"] as SplitBackgroundColorButton;
            if (sbBKData != null)
            {
                sbBKData.CmdTarget = target;
            }

            SplitBordlineColorButton sbBLData = _dataCollection["BorderLineColor"] as SplitBordlineColorButton;
            if (sbBLData != null)
            {
                sbBLData.CmdTarget = target;
            }

            SplitMenuItemData spData = _dataCollection["BorderLineType"] as SplitMenuItemData;
            if (spData != null)
            {
                spData.CmdTarget = target;
            }

            GalleryData<BorderLineWidthData> BLDate = _dataCollection["BorderLineType Gallery"] as GalleryData<BorderLineWidthData>;
            if (BLDate != null)
            {
                BLDate.CmdTarget = target;
            }

            spData = _dataCollection["BorderLinePattern"] as SplitMenuItemData;
            if (spData != null)
            {
                spData.CmdTarget = target;
            }

            GalleryData<BorderLineStyleData> uDate = _dataCollection["BorderLinePattern Gallery"] as GalleryData<BorderLineStyleData>;
            if (uDate != null)
            {
                uDate.CmdTarget = target;
            }

            spData = _dataCollection["LineArrowStyle"] as SplitMenuItemData;
            if (spData != null)
            {
                spData.CmdTarget = target;
            }

            GalleryData<LineArrowStyleDate> LADate = _dataCollection["LineArrowStyleGallery"] as GalleryData<LineArrowStyleDate>;
            if (LADate != null)
            {
                LADate.CmdTarget = target;
            }
        }

        private void UpdateFormatPaint()
        {
            FirePropertyChanged("IsFomratCheck");
        }

        private void UpdateService_ProgressChanging(object sender, EventArgs e)
        {
            var updateService = sender as IUpdateService;
            if (updateService == null)
                return;

            if (updateService.UpdateProcess < 0)
            {//Before/after download update package, or download failed.
                IsUpdateProgressShow = false;
                return;
            }

            if (updateService.UpdateProcess > 0 && IsUpdateProgressShow == false)
            {//Download update package starting. 
                IsUpdateProgressShow = true;
                TargetVersion = updateService.UpdateInfo.TargetVersion;
            }

            if (updateService.State == UpdateServiceState.UpdatePackageDownloaded)
            {//Download update package finished.
                IsRunUpdateEnabled = true;
            }
            UpdateProgress = updateService.UpdateProcess;
        }
        #endregion

        #region  Event Handler

        private void OpenPanesEventHandler(ActivityPane pane)
        {
            switch (pane.Name)
            {
                case CommonDefine.PaneSitemap:
                    IsSitemapOpen = pane.bOpen;
                    break;
                case CommonDefine.PaneWidgets:
                    IsWidgetsOpen = pane.bOpen;
                    break;
                case CommonDefine.PaneMaster:
                    IsMasterOpen = pane.bOpen;
                    break;
                case CommonDefine.PaneWidgetProp:
                    IsWidgetPropOpen = pane.bOpen;
                    break;
                case CommonDefine.PaneInteraction:
                    IsInteractionOpen = pane.bOpen;
                    break;
                case CommonDefine.PanePageProp:
                    IsPagePropOpen = pane.bOpen;
                    break;
                case CommonDefine.PanePageIcon:
                    IsPageIconOpen = pane.bOpen;
                    break;
                case CommonDefine.PaneWidgetManager:
                    IsPageWidgetManagerOpen = pane.bOpen;
                    break;
            }
        }
        private void SelectionChangeEventHandler(string EventArg)
        {
            FirePropertyChanged("IsBoldCheck");
            FirePropertyChanged("IsItalicCheck");
            FirePropertyChanged("IsUnderlineCheck");
            FirePropertyChanged("IsBulletCheck");
            FirePropertyChanged("IsStrikeThroughCheck");
            FirePropertyChanged("IsTxtAlignTop");
            FirePropertyChanged("IsTxtAlignMiddle");
            FirePropertyChanged("IsTxtAlignBottom");
            FirePropertyChanged("IsTxtAlignLeft");
            FirePropertyChanged("IsTxtAlignCenter");
            FirePropertyChanged("IsTxtAlignRight");

            //Fire Property for Biding
            FirePropertyChanged("Top");
            FirePropertyChanged("Left");
            FirePropertyChanged("Width");
            FirePropertyChanged("Height");

            //Fire IsEnable Property
            FirePropertyChanged("CanLocationEdit");

            UpdateSelectedItem();
            StyChangeHandler("vFontFamily");
            StyChangeHandler("vFontSize");

            CanSupportProperty();
        }

        private void SelectionPropertyEventHandler(string EventArg)
        {
            switch (EventArg)
            {
                case "Top":
                    {
                        if (_isFirePositionEnabled)
                        {
                            FirePropertyChanged("Top");
                        }
                        break;
                    }
                case "Left":
                    {
                        if (_isFirePositionEnabled)
                        {
                            FirePropertyChanged("Left");
                        }
                        break;
                    }
                case "ItemWidth":
                    {
                        FirePropertyChanged("Width");
                        break;
                    }
                case "ItemHeight":
                    {
                        FirePropertyChanged("Height");
                        break;
                    }
                case "vFontBold":
                    {
                        FirePropertyChanged("IsBoldCheck");
                        break;
                    }
                case "vFontItalic":
                    {
                        FirePropertyChanged("IsItalicCheck");
                        break;
                    }

                case "vFontUnderLine":
                    {
                        System.Diagnostics.Debug.WriteLine("vFontUnderLine handler");
                        FirePropertyChanged("IsUnderlineCheck");
                        break;
                    }

                case "vTextBulletStyle":
                    {
                        FirePropertyChanged("IsBulletCheck");
                        break;
                    }

                case "vFontStrickeThrough":
                    {
                        FirePropertyChanged("IsStrikeThroughCheck");
                        break;
                    }
                case "vFontColor":
                    {
                        //FirePropertyChanged("FontColorView");
                        //var stackTrace = new System.Diagnostics.StackTrace();
                        //var undo = stackTrace.GetFrames().Where(f => f.GetMethod().Name == "Undo" || f.GetMethod().Name == "Redo");

                        GalleryData<StyleColor> Bdata = _dataCollection["Font Color Gallery"] as GalleryData<StyleColor>;
                        if (Bdata != null)
                        {
                            Bdata.SelectedItem = FontColorView.ToStyleColor();
                        }
                        //if (undo.Count() == 0)
                        //{
                        //    SplitMenuItemData spData = _dataCollection["Font Color"] as SplitMenuItemData;
                        //    if (spData != null)
                        //    {
                        //        //spData.CommandParameter = FontColorView;
                        //    }
                        //}
                        break;
                    }
                case "vBorderLineColor":
                    {
                        FirePropertyChanged("BorderLineColorView");
                        //var stackTrace = new System.Diagnostics.StackTrace();
                        //var undo = stackTrace.GetFrames().Where(f => f.GetMethod().Name == "Undo" || f.GetMethod().Name == "Redo");

                        GalleryData<StyleColor> Bdata = _dataCollection["BorderLine Color Gallery"] as GalleryData<StyleColor>;
                        if (Bdata != null)
                        {
                            Bdata.SelectedItem = BorderLineColorView;
                        }

                        //if (undo.Count() == 0)
                        //{
                        //    SplitButtonData sbData = _dataCollection["BorderLineColor"] as SplitButtonData;
                        //    if (sbData != null)
                        //    {
                        //        sbData.CommandParameter = BorderLineColorView;
                        //    }
                        //}
                        break;
                    }
                case "vBackgroundColor":
                    {
                        FirePropertyChanged("BackgroundColorView");
                        //var stackTrace = new System.Diagnostics.StackTrace();
                        //var undo = stackTrace.GetFrames().Where(f => f.GetMethod().Name == "Undo" || f.GetMethod().Name == "Redo");

                        GalleryData<StyleColor> Bdata = _dataCollection["Background Color Gallery"] as GalleryData<StyleColor>;
                        if (Bdata != null)
                        {
                            Bdata.SelectedItem = BackgroundModifyColorView;
                        }
                        //if (undo.Count() == 0)
                        //{

                        //    SplitBackgroundColorButton sbData = _dataCollection["BackColor"] as SplitBackgroundColorButton;
                        //    if (sbData != null)
                        //    {
                        //        sbData.CommandParameter = BackgroundModifyColorView;
                        //    }
                        //}
                        break;
                    }
                case "vTextHorAligen":
                    {
                        FirePropertyChanged("IsTxtAlignLeft");
                        FirePropertyChanged("IsTxtAlignCenter");
                        FirePropertyChanged("IsTxtAlignRight");
                        break;
                    }
                case "vTextVerAligen":
                    {
                        FirePropertyChanged("IsTxtAlignTop");
                        FirePropertyChanged("IsTxtAlignMiddle");
                        FirePropertyChanged("IsTxtAlignBottom");
                        break;
                    }
                case "vBorderlineStyle":
                    {
                        UpdateBorderLineStyleSelectedItem();
                        UpdateBorderLineStyleButtonParameter();
                        break;
                    }
                case "vBorderLinethinck":
                    {
                        UpdateBorderLineWidthSelectedItem();
                        UpdateBorderLineWidthButtonParameter();
                        break;
                    }
                case "LineArrowStyle":
                    {
                        UpdateArrowStyleSelectItem();
                        UpdateArrowStyleButtonParameter();
                        break;
                    }
                default:
                    StyChangeHandler(EventArg);
                    return;
            }



        }

        private void EnableFirePositionHandler(bool enable)
        {
            _isFirePositionEnabled = enable;
        }
        private void SelectionPageChangeEventHandler(Guid EventArg)
        {
            FirePropertyChanged("IsBoldCheck");
            FirePropertyChanged("IsItalicCheck");
            FirePropertyChanged("IsUnderlineCheck");
            FirePropertyChanged("IsStrikeThroughCheck");
            FirePropertyChanged("IsTxtAlignTop");
            FirePropertyChanged("IsTxtAlignMiddle");
            FirePropertyChanged("IsTxtAlignBottom");
            FirePropertyChanged("IsTxtAlignLeft");
            FirePropertyChanged("IsTxtAlignCenter");
            FirePropertyChanged("IsTxtAlignRight");
            //Fire Property for Biding
            FirePropertyChanged("Top");
            FirePropertyChanged("Left");
            FirePropertyChanged("Width");
            FirePropertyChanged("Height");

            //Fire IsEnable Property
            FirePropertyChanged("CanLocationEdit");

            //Fire Color 
            FirePropertyChanged("BackgroundColorView");
            // FirePropertyChanged("FontColorView");
            FirePropertyChanged("BorderLineColorView");

            StyChangeHandler("vFontSize");
            StyChangeHandler("vFontFamily");
            CanSupportProperty();

            ISelectionService _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            IPagePropertyData page = _selectionService.GetCurrentPage();
            if (page == null)
            {
                return;
            }
            if (CmdTarget == page.EditorCanvas)
            {
                return;
            }
            CmdTarget = _selectionService.GetCurrentPage().EditorCanvas;

            SetStyleCmdTarget(CmdTarget);
        }

        public void DomLoadedEventHandler(FileOperationType loadType)
        {
            switch (loadType)
            {
                case FileOperationType.Create:
                case FileOperationType.Open:
                    beOpenDocument = true;
                    if (_doc != null && _doc.Document != null)
                    {
                        if (_doc.Document.DocumentType == DocumentType.Library)
                        {
                            IsPageIconOpen = true;
                        }
                        else
                        {
                            IsPageIconOpen = false;
                        }
                    }
                    break;
                case FileOperationType.Close:
                    beOpenDocument = false;
                    break;
            }

            FirePropertyChanged("IsExport2ImageEnabled");
            FirePropertyChanged("IsExport2Image");
            FirePropertyChanged("LibraryVisibility");
            FirePropertyChanged("DocumentVisibility");

            RefreshCommands();
        }

        private void RefreshCommands()
        {
            this.SaveDocmentCommand.RaiseCanExecuteChanged();
            this.SaveAsDocmentCommand.RaiseCanExecuteChanged();
            this.HTMLExportCommand.RaiseCanExecuteChanged();
            this.AutoSaveCommand.RaiseCanExecuteChanged();
            this.DocumentRecoveryCommand.RaiseCanExecuteChanged();
            this.CloseDocmentCommand.RaiseCanExecuteChanged();
            this.PreviewDocmentCommand.RaiseCanExecuteChanged();
            this.PublishDocmentCommand.RaiseCanExecuteChanged();
        }

        private void FlashRecentFile(object parameter)
        {
            FirePropertyChanged("RecentFile");
        }

        public void CheckUpdateCompletedHandler(string parameter)
        {
            IUpdateInfo updateInfo = _updateService.UpdateInfo;
            if (_updateService.IsAutoCheckUpdate && updateInfo.HasError == false && updateInfo.NeedToUpdate)
            {
                CheckUpdateWindow win = new CheckUpdateWindow();
                win.Owner = Application.Current.MainWindow;
                win.Closed += win_Closed;
                win.ShowDialog();
            }
        }

        /// <summary>
        /// Set foucus of mainwindow in case it is set back when update window closed.
        /// </summary>
        void win_Closed(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Activate();
        }


        public void UpdateProcessHandler(string parameter)
        {
            IUpdateService updateService = ServiceLocator.Current.GetInstance<IUpdateService>();
            IUpdateInfo updateInfo = updateService.UpdateInfo;

            if (updateInfo.HasError)
            {
                MessageBoxResult resault = MessageBox.Show(GlobalData.FindResource("Update_Update_Failed")
                    + updateInfo.Message, GlobalData.FindResource("Update_Update"), MessageBoxButton.OK);
            }
        }

        public void OpenDialogEventHandler(DialogType type)
        {
            if (type == DialogType.LanguageSetting)
            {
                LanguageSettingAction(new object());
            }
        }

        public void UpdateToolbarUI(TBUpdateType type)
        {
            switch (type)
            {
                case TBUpdateType.FormatPaint:
                    UpdateFormatPaint();
                    break;
                default:
                    break;
            }
        }

        public void ResetFontsizeValue()
        {
            FontSizeControlData fData = _dataCollection["Font Size"] as FontSizeControlData;

            if (fData != null)
            {
                fData.ReUpDateFontsize();
            }
        }
       

        #endregion
    }
}
