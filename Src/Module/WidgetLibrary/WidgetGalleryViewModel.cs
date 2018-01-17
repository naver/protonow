using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.Common.Win32;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Naver.Compass.Service.CustomLibrary;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace Naver.Compass.Module
{
    public class WidgetGalleryViewModel : ViewModelBase
    {
        public const string packPath = @"pack://application:,,,/Naver.Compass.Module.WidgetLibrary;component/Resources/";
        public static string Widgetlibraryfolder;
        private const string uixmlPath = packPath + "ui.xml";
        private const string iconxmlPath = packPath + "icon.xml";
        private const string uiLib = "UI > ";
        private const string iconLib = "Icon > ";
        private const string myLib = "MY > ";
        public WidgetTab UIWidgetLibraryTab { get; set; }
        public WidgetTab ICONWidgetLibraryTab { get; set; }
        public WidgetTab MyWidgetLibraryTab { get; set; }

        public WidgetTab SearchResultTab { get; set; }
        public DelegateCommand<object> WidgetSearchChangedCommand { get; private set; }
        public DelegateCommand<object> TabIndexChangedCommand { get; private set; }

        private const string MutexFavourite = "MutexFavourite_WidgetLibiary";
        private const string MutexCustom = "MutexCustom_WidgetLibiary";
        private Mutex SyncFavourite;
        private Mutex SyncCustom;
        private string FavouriteFileXmlPath;
        private string CustomFileXmlPath;
        private const string FavouriteExpandHeader = "Favorites";
        private FileChangedWatcher _favouriteWatcher;
        public WidgetGalleryViewModel()
        {
            ///create my tab
            MyWidgetLibraryTab = new WidgetTab();
            SearchResultTab = new WidgetTab();
            MyWidgetLibraryTab.RegisterDomLoadedEvent();
            ICustomLibraryService CustomLibraryService;
            CustomLibraryService = ServiceLocator.Current.GetInstance<CustomLibraryServiceProvider>();
            CustomLibraryService.RegisterMyLibrary(MyWidgetLibraryTab);
            MyWidgetLibraryTab.WidgetExpands.CollectionChanged += MyWidget_CollectionChanged;

            MyWidgetLibraryTab.WidgetExpands.Add(new WidgetExpand { Header = FavouriteExpandHeader });

            ///events
            _ListEventAggregator.GetEvent<WidgetFavouriteEvent>().Subscribe(WidgetFavouriteHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<ResetFavouriteEvent>().Subscribe(ResetFavouriteHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<UpdateLanguageEvent>().Subscribe(UpdateLanguagesHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<CustomWidgetChangedEvent>().Subscribe(CustomWidgetChangedHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<DeleteLibraryWidgetEvent>().Subscribe(DeleteCustomLibraryHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<LibraryExpandChangedEvent>().Subscribe(LibraryExpandChangedEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<DomLoadedEvent>().Subscribe(DomLoadedEventHandler, ThreadOption.UIThread);

            _ListEventAggregator.GetEvent<ExportToMYLibraryEvent>().Subscribe(ExportToMyLibraryAction);
            _ListEventAggregator.GetEvent<RefreshCustomLibraryEvent>().Subscribe(RefreshCustomLibraryAction);

            this.WidgetSearchChangedCommand = new DelegateCommand<object>(WidgetSearchChangedExecute);
            this.TabIndexChangedCommand = new DelegateCommand<object>(TabIndexChangedExecute);

            ///set delay timer
            this._inputDelayTimer = new DispatcherTimer();
            this._inputDelayTimer.Interval = TimeSpan.FromMilliseconds(500);
            this._inputDelayTimer.Tick += _inputDelayTimer_Tick;

            this.InitializeInfo();
            Application.Current.Exit += Current_Exit;
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            SaveExpanderStatus();
        }

        void MyWidget_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ICustomLibraryService CustomLibraryService;
            CustomLibraryService = ServiceLocator.Current.GetInstance<CustomLibraryServiceProvider>();
            CustomLibraryService.RegisterLibraies(MyWidgetLibraryTab.WidgetExpands.Cast<ICustomLibrary>());
        }

        private void InitializeInfo()
        {
            /// initialize folder
            var programdata = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.Create);
            Widgetlibraryfolder = Path.Combine(programdata, @"Design Studio\Widget library");
            if (!Directory.Exists(Widgetlibraryfolder))
            {
                Directory.CreateDirectory(Widgetlibraryfolder);
            }

            /// initialize favourite
            FavouriteFileXmlPath = Path.Combine(Widgetlibraryfolder, "favourite_library.xml");
            _favouriteWatcher = new FileChangedWatcher(Widgetlibraryfolder, "favourite_library.xml");
            _favouriteWatcher.FileChanged += favouriteWatcher_FileChanged;
            try
            {
                SyncFavourite = Mutex.OpenExisting(MutexFavourite);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                SyncFavourite = new Mutex(false, MutexFavourite);
            }

            /// initialize custom
            CustomFileXmlPath = Path.Combine(Widgetlibraryfolder, "custom_library.xml");
            try
            {
                SyncCustom = Mutex.OpenExisting(MutexCustom);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                SyncCustom = new Mutex(false, MutexCustom);
            }

            /// ui and icon tab
            var serializer = new XmlSerializer(typeof(WidgetTab));
            var uiStreamInfo = Application.GetResourceStream(new Uri(uixmlPath, UriKind.RelativeOrAbsolute));
            UIWidgetLibraryTab = serializer.Deserialize(uiStreamInfo.Stream) as WidgetTab;

            var iconStreamInfo = Application.GetResourceStream(new Uri(iconxmlPath, UriKind.RelativeOrAbsolute));
            ICONWidgetLibraryTab = serializer.Deserialize(iconStreamInfo.Stream) as WidgetTab;

            ///load favourite widgets
            this.LoadFavouriteWidget();

            ///load custom widgets
            LoadCustomWidget();

            InitialExpandInfo();

            ///initial ui status
            PerformSearch();
        }

        private void InitialExpandInfo()
        {
            var expandInfo = GlobalData.LibrariesExpanded;
            try
            {
                if (!string.IsNullOrEmpty(expandInfo))
                {
                    var t = expandInfo
                        .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(e => e.ToCharArray().Select(b => b == '0' ? false : true))
                        .ToList();
                    if (t.Count == 3 &&
                        t[0].Count() == UIWidgetLibraryTab.WidgetExpands.Count &&
                        t[1].Count() == ICONWidgetLibraryTab.WidgetExpands.Count &&
                        t[2].Count() == MyWidgetLibraryTab.WidgetExpands.Count)
                    {
                        var uiexpand = t[0].GetEnumerator();
                        foreach (var expand in UIWidgetLibraryTab.WidgetExpands)
                        {
                            uiexpand.MoveNext();
                            expand.IsExpand = uiexpand.Current;
                            expand.ExpandCache = expand.IsExpand;
                            expand.TabType = uiLib;
                        }

                        var iconexpand = t[1].GetEnumerator();
                        foreach (var expand in ICONWidgetLibraryTab.WidgetExpands)
                        {
                            iconexpand.MoveNext();
                            expand.IsExpand = iconexpand.Current;
                            expand.ExpandCache = expand.IsExpand;
                            expand.TabType = iconLib;
                        }

                        var myexpand = t[2].GetEnumerator();
                        foreach (var expand in MyWidgetLibraryTab.WidgetExpands)
                        {
                            myexpand.MoveNext();
                            expand.IsExpand = myexpand.Current;
                            expand.ExpandCache = expand.IsExpand;
                            expand.TabType = myLib;
                        }

                        return;
                    }
                }
            }
            catch
            {
            }

            foreach (var expand in UIWidgetLibraryTab.WidgetExpands)
            {
                expand.IsExpand = true;
                expand.ExpandCache = true;
                expand.TabType = uiLib;
            }

            bool bfirst = true;
            foreach (var expand in ICONWidgetLibraryTab.WidgetExpands)
            {
                if (bfirst)
                {
                    expand.IsExpand = true;
                    expand.ExpandCache = true;
                    bfirst = false;
                }
                expand.TabType = iconLib;
            }

            foreach (var expand in MyWidgetLibraryTab.WidgetExpands)
            {
                expand.IsExpand = true;
                expand.ExpandCache = true;
                expand.TabType = myLib;
            }
        }

        private void SaveExpanderStatus()
        {
            try
            {
                var str1 = string.Join(string.Empty, UIWidgetLibraryTab.WidgetExpands.Select(e => e.IsExpand ? '1' : '0'));
                var str2 = string.Join(string.Empty, ICONWidgetLibraryTab.WidgetExpands.Select(e => e.IsExpand ? '1' : '0'));
                var str3 = string.Join(string.Empty, MyWidgetLibraryTab.WidgetExpands.Select(e => e.IsExpand ? '1' : '0'));
                GlobalData.LibrariesExpanded = str1 + "|" + str2 + "|" + str3;
            }
            catch (Exception)
            {

            }

        }

        private DispatcherTimer _inputDelayTimer;
        private string _textSearch = string.Empty;
        private IEnumerable<WidgetModel> _cacheWidgets;
        private Timer _widgetLoaderTimer;
        private int _lastIndex;
        public Visibility DocTypeVisibility
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc.Document == null || doc.Document.DocumentType == DocumentType.Standard)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public bool IsOnSearch
        {
            get
            {
                return !string.IsNullOrEmpty(this._textSearch);
            }
        }

        public void TabIndexChangedExecute(object cmdParameter)
        {
            if (cmdParameter is object[])
            {
                var cmdparameters = cmdParameter as object[];
                if (cmdparameters.Length == 2 && cmdparameters[1] is SelectionChangedEventArgs)
                {
                    var selectionChangedE = cmdparameters[1] as SelectionChangedEventArgs;
                    if (selectionChangedE.Source is TabControl)
                    {
                        var tabcontrol = selectionChangedE.Source as TabControl;
                        var index = tabcontrol.SelectedIndex;
                        if (index == 1)
                        {
                            if (string.IsNullOrEmpty(this._textSearch))
                            {
                                var openedExpand = ICONWidgetLibraryTab.WidgetExpands.FirstOrDefault(expand => expand.IsExpand);
                                if (openedExpand != null)
                                {
                                    _cacheWidgets = openedExpand.WidgetModels.ToList().AsEnumerable();
                                    openedExpand.WidgetModels.Clear();

                                    var appendWidget = _cacheWidgets.Except(openedExpand.WidgetModels).Take(50);
                                    openedExpand.WidgetModels.AddRange(appendWidget);
                                    _widgetLoaderTimer = new Timer(WidgetLoaderTimerTick, openedExpand, 1500, Timeout.Infinite);

                                    //var leftrange = _cacheWidgets;
                                    //var resetEvent = new AutoResetEvent(false);
                                    //ThreadPool.QueueUserWorkItem(o =>
                                    //{
                                    //    while (leftrange.Count() > 0)
                                    //    {
                                    //        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, (Action)(() =>
                                    //        {
                                    //            openedExpand.WidgetModels.AddRange(leftrange.Take(1));
                                    //            openedExpand.FireItemChangedInfo();
                                    //            resetEvent.Set();
                                    //        }));

                                    //        resetEvent.WaitOne();
                                    //        leftrange = leftrange.Skip(1);

                                    //    }
                                    //});

                                }
                            }
                        }
                        else if (_lastIndex == 1)
                        {
                            var openedExpand = ICONWidgetLibraryTab.WidgetExpands.FirstOrDefault(expand => expand.IsExpand);
                            if (openedExpand != null && _cacheWidgets != null)
                            {
                                ThreadPool.QueueUserWorkItem(obj =>
                                {
                                    if (_widgetLoaderTimer != null)
                                    {
                                        _widgetLoaderTimer.Dispose();
                                        _widgetLoaderTimer = null;
                                    }

                                    Thread.Sleep(200);
                                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
                                    {
                                        var appendWidget = _cacheWidgets.Except(openedExpand.WidgetModels);
                                        openedExpand.WidgetModels.AddRange(appendWidget);
                                    }));
                                });
                            }
                        }

                        _lastIndex = index;
                    }
                }
            }
        }

        private void WidgetLoaderTimerTick(object objState)
        {
            var openedExpand = objState as WidgetExpand;
            if (openedExpand != null && _cacheWidgets != null)
            {
                if (_widgetLoaderTimer != null)
                {
                    _widgetLoaderTimer.Dispose();
                    _widgetLoaderTimer = null;
                }

                var appendWidget = _cacheWidgets.Except(openedExpand.WidgetModels);
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
                {
                    openedExpand.WidgetModels.AddRange(appendWidget);
                    openedExpand.FireItemChangedInfo();
                }));
            }
        }

        //private void WidgetLoaderTimerTick(object objState)
        //{
        //    var openedExpand = objState as WidgetExpand;
        //    _widgetLoaderTimer.Dispose();
        //    _widgetLoaderTimer = null;
        //    var appendWidget = _cacheWidgets.Except(openedExpand.WidgetModels).Take(80);
        //    Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
        //    {
        //        openedExpand.WidgetModels.AddRange(appendWidget);
        //        if (appendWidget.Count() >= 80)
        //        {
        //            _widgetLoaderTimer = new Timer(WidgetLoaderTimerTick, openedExpand, 1000, Timeout.Infinite);
        //        }
        //    }));
        //}

        public void WidgetSearchChangedExecute(object cmdParameter)
        {
            if (cmdParameter is object[])
            {
                var cmdparameters = cmdParameter as object[];
                if (cmdparameters.Length == 2 && cmdparameters[1] is TextChangedEventArgs)
                {
                    var textChangedE = cmdparameters[1] as TextChangedEventArgs;
                    if (textChangedE.Source is TextBox)
                    {
                        this._textSearch = (textChangedE.Source as TextBox).Text.Trim();
                        if (!this._textSearch.Any(c => c < 0x20 || c > 0x7e) && this._textSearch.Length < 2)
                        {
                            this._textSearch = string.Empty;
                        }

                        if (this._inputDelayTimer.IsEnabled)
                        {
                            this._inputDelayTimer.Stop();
                        }

                        this._inputDelayTimer.Start();
                    }
                }

            }
        }

        private string _lastSearchText = string.Empty;
        private void _inputDelayTimer_Tick(object sender, EventArgs e)
        {
            this._inputDelayTimer.Stop();
            if (_lastSearchText != this._textSearch)
            {
                this.PerformSearch();
                _lastSearchText = this._textSearch;
            }
        }

        private bool _isInitialized;
        private void PerformSearch()
        {
            if (SearchResultTab != null && SearchResultTab.WidgetExpands.Count > 0)
            {
                SearchResultTab.WidgetExpands.Clear();
            }

            FirePropertyChanged("IsOnSearch");
            if (!string.IsNullOrEmpty(_textSearch))
            {
                var uiexpands = new RangeObservableCollection<WidgetExpand>();
                foreach (var ex in UIWidgetLibraryTab.WidgetExpands)
                {
                    uiexpands.Add(ex.Clone());
                }

                var iconexpands = new RangeObservableCollection<WidgetExpand>();
                foreach (var ex in ICONWidgetLibraryTab.WidgetExpands)
                {
                    iconexpands.Add(ex.Clone());
                }

                var myexpands = new RangeObservableCollection<WidgetExpand>();
                foreach (var ex in MyWidgetLibraryTab.WidgetExpands)
                {
                    myexpands.Add(ex.Clone());
                }

                SearchResultTab.WidgetExpands.AddRange(uiexpands);
                SearchResultTab.WidgetExpands.AddRange(iconexpands);
                SearchResultTab.WidgetExpands.AddRange(myexpands);
                SearchResultTab.PerformSearch(_textSearch);
                FixUiStatus(SearchResultTab);
                //Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
                //{
                //    foreach (var expand in ICONWidgetLibraryTab.WidgetExpands)
                //    {
                //        expand.FireItemChangedInfo();
                //    }
                //}));
            }
            else
            {
                if (_isInitialized)
                {
                    return;
                }

                _isInitialized = true;
                UIWidgetLibraryTab.PerformSearch(_textSearch);
                MyWidgetLibraryTab.PerformSearch(_textSearch);
                FixUiStatus(UIWidgetLibraryTab);
                FixUiStatus(MyWidgetLibraryTab);
                foreach (var expand in ICONWidgetLibraryTab.WidgetExpands)
                {
                    expand.IsExpand = expand.ExpandCache;
                    expand.IsVisible = true;
                }

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
                {
                    var clone = ICONWidgetLibraryTab.WidgetExpands.ToArray();
                    ICONWidgetLibraryTab.WidgetExpands.Clear();
                    ICONWidgetLibraryTab.WidgetExpands.AddRange(clone);
                    ICONWidgetLibraryTab.PerformSearch(_textSearch);
                    FixUiStatus(ICONWidgetLibraryTab);
                }));
            }

        }


        private void FixUiStatus(WidgetTab tabModel)
        {
            foreach (var expand in tabModel.WidgetExpands)
            {
                FixStatusOfExpand(expand);
            }

            if (!tabModel.WidgetExpands.Any(expand => expand.IsVisible))
            {
                tabModel.IsEmptyHintVisible = true;
            }
            else
            {
                tabModel.IsEmptyHintVisible = false;
            }
        }

        private void FixStatusOfExpand(WidgetExpand expand)
        {
            if (string.IsNullOrEmpty(_textSearch))
            {
                /// normal condition
                expand.IsExpand = expand.ExpandCache;
                expand.IsVisible = true;
                if (expand.TabType != null && expand.Header.StartsWith(expand.TabType))
                {
                    expand.Header = expand.Header.Substring(expand.TabType.Length);
                }
            }
            else
            {
                /// search condition
                if (expand.WidgetModels.Any(wm => wm.IsVisible))
                {
                    if (string.IsNullOrEmpty(_lastSearchText))
                    {
                        expand.ExpandCache = expand.IsExpand;
                    }

                    expand.IsExpand = true;
                    expand.IsVisible = true;
                    if (expand.TabType != null && !expand.Header.StartsWith(expand.TabType))
                    {
                        expand.Header = expand.TabType + expand.Header;
                    }
                }
                else
                {
                    expand.IsVisible = false;
                }
            }

            if (expand.WidgetModels.Any(wm => wm.IsVisible))
            {
                expand.IsFavoriteHintVisible = false;
                expand.IsLibraryHintVisible = false;
            }
            else
            {
                if (expand.Header == FavouriteExpandHeader)
                {
                    expand.IsFavoriteHintVisible = true;
                }
                else
                {
                    expand.IsLibraryHintVisible = true;
                }
            }
        }

        void favouriteWatcher_FileChanged(object sender, FileSystemEventArgs e)
        {
            LoadFavouriteWidget();
        }

        private void LoadFavouriteWidget()
        {
            try
            {
                SyncFavourite.WaitOne();
                using (var rdr = new StreamReader(FavouriteFileXmlPath))
                {
                    var serializer = new XmlSerializer(typeof(Guid[]));
                    var favouriteIds = (Guid[])serializer.Deserialize(rdr);
                    var allWidgets = UIWidgetLibraryTab.WidgetExpands.SelectMany(we => we.WidgetModels)
                        .Concat(ICONWidgetLibraryTab.WidgetExpands.SelectMany(we => we.WidgetModels));
                    var favouriteWidgets = allWidgets
                        .Where(x => favouriteIds.Contains(x.Id))
                        .OrderBy(x => Array.IndexOf(favouriteIds, x.Id));

                    var expandFavourite = MyWidgetLibraryTab.WidgetExpands[0];
                    expandFavourite.WidgetModels.Clear();

                    foreach (var w in allWidgets)
                    {
                        w.IsFavourite = false;
                    }

                    foreach (var w in favouriteWidgets)
                    {
                        ;
                        w.IsFavourite = true;
                        expandFavourite.WidgetModels.Add(w);
                    }
                }
            }
            catch { }
            finally
            {
                SyncFavourite.ReleaseMutex();
            }
        }

        private void SaveFavouriteWidget()
        {
            try
            {
                SyncFavourite.WaitOne();
                var expandFavourite = MyWidgetLibraryTab.WidgetExpands[0];
                var favouriteIds = expandFavourite.WidgetModels.Select(vm => vm.Id).ToArray();

                using (var rdr = new StreamWriter(FavouriteFileXmlPath))
                {

                    var serializer = new XmlSerializer(typeof(Guid[]));
                    serializer.Serialize(rdr, favouriteIds);
                }

                _favouriteWatcher.lastwrite = DateTime.Now;
            }
            catch { }
            finally
            {
                SyncFavourite.ReleaseMutex();
            }
        }

        private void LoadCustomWidget()
        {
            try
            {
                SyncCustom.WaitOne();
                using (var rdr = new StreamReader(CustomFileXmlPath))
                {
                    var serializer = new XmlSerializer(typeof(List<WidgetExpand>));
                    var customExpands = (List<WidgetExpand>)serializer.Deserialize(rdr);
                    MyWidgetLibraryTab.WidgetExpands.RemoveRange(MyWidgetLibraryTab.WidgetExpands.Where(e => e.IsCustomWidget));
                    foreach (var expand in customExpands)
                    {
                        expand.IsCustomWidget = true;
                        MyWidgetLibraryTab.WidgetExpands.Add(expand);
                    }
                }
            }
            catch { }
            finally
            {
                SyncCustom.ReleaseMutex();
            }
        }

        private void SaveCustomWidget()
        {
            try
            {
                SyncCustom.WaitOne();
                var customTab = MyWidgetLibraryTab.WidgetExpands.Where(e => e.IsCustomWidget).ToList();

                using (var rdr = new StreamWriter(CustomFileXmlPath))
                {

                    var serializer = new XmlSerializer(typeof(List<WidgetExpand>));
                    serializer.Serialize(rdr, customTab);
                }
            }
            catch { }
            finally
            {
                SyncCustom.ReleaseMutex();
            }
        }

        public void WidgetFavouriteHandler(object switchFavour)
        {
            var expandFavourite = MyWidgetLibraryTab.WidgetExpands[0];
            if (switchFavour is WidgetModel)
            {
                var favourWidget = switchFavour as WidgetModel;

                if (favourWidget.IsFavourite)
                {
                    favourWidget.IsFavourite = false;

                    if (expandFavourite.WidgetModels.Contains(favourWidget))
                    {
                        expandFavourite.WidgetModels.Remove(favourWidget);
                    }
                }
                else
                {
                    favourWidget.IsFavourite = true;
                    if (!expandFavourite.WidgetModels.Contains(favourWidget))
                    {
                        expandFavourite.WidgetModels.Insert(0, favourWidget);
                    }
                }

                #region update favourite icon in search result tab.
                foreach (var item in SearchResultTab.WidgetExpands)
                {
                    var favourite = item.WidgetModels.FirstOrDefault(x => x.Id == favourWidget.Id);
                    if (favourite != null)
                    {
                        favourite.IsFavourite = favourWidget.IsFavourite;
                        break;
                    }
                }
                #endregion

                CustomWidgetChangedHandler(expandFavourite);
                SaveFavouriteWidget();
                expandFavourite.SearchText = Guid.NewGuid().ToString();
            }
        }

        public void ResetFavouriteHandler(object obj)
        {
            var expandFavourite = MyWidgetLibraryTab.WidgetExpands[0];
            foreach (var widget in expandFavourite.WidgetModels)
            {
                widget.IsFavourite = false;
            }

            expandFavourite.WidgetModels.Clear();
            MyWidgetLibraryTab.PerformSearch(_textSearch);
            var expand = expandFavourite;
            if (expand == null) return;
            if (expand.WidgetModels.Any(wm => wm.IsVisible))
            {
                expand.IsExpand = true;
                expand.IsVisible = true;
                if (expand.Header == FavouriteExpandHeader)
                {
                    expand.IsFavoriteHintVisible = false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_textSearch))
                {
                    expand.IsVisible = true;
                    expand.IsExpand = true;
                    if (expand.Header == FavouriteExpandHeader)
                    {
                        expand.IsFavoriteHintVisible = true;
                    }
                }
                else
                {
                    expand.IsVisible = false;
                    expand.IsExpand = false;
                }
            }
            SaveFavouriteWidget();
        }

        private void UpdateLanguagesHandler(string obj)
        {
            var widgets = UIWidgetLibraryTab.WidgetExpands.SelectMany(we => we.WidgetModels).Where(w => !string.IsNullOrEmpty(w.LocalizedName));
            widgets.Concat(ICONWidgetLibraryTab.WidgetExpands.SelectMany(we => we.WidgetModels).Where(w => !string.IsNullOrEmpty(w.LocalizedName)));
            widgets.Concat(MyWidgetLibraryTab.WidgetExpands.SelectMany(we => we.WidgetModels).Where(w => !string.IsNullOrEmpty(w.LocalizedName)));
            foreach (var widget in widgets)
            {
                widget.LocalizedTextChanged();
            }

            UIWidgetLibraryTab.LocalizedTextChanged();
            ICONWidgetLibraryTab.LocalizedTextChanged();
            MyWidgetLibraryTab.LocalizedTextChanged();
        }

        private void CustomWidgetChangedHandler(object obj)
        {
            //Display added custom widget in MY tab.
            MyWidgetLibraryTab.PerformSearch(string.Empty);
            var expand = obj as WidgetExpand;
            if (expand == null) return;
            FixStatusOfExpand(expand);

            if (!MyWidgetLibraryTab.WidgetExpands.Any(e => e.IsVisible))
            {
                MyWidgetLibraryTab.IsEmptyHintVisible = true;
            }
            else
            {
                MyWidgetLibraryTab.IsEmptyHintVisible = false;
            }

            SaveCustomWidget();
        }

        private void DeleteCustomLibraryHandler(object obj)
        {
            var expand = obj as WidgetExpand;
            if (expand != null && MyWidgetLibraryTab.WidgetExpands.Contains(expand))
            {
                var filename = expand.FileName;
                MyWidgetLibraryTab.WidgetExpands.Remove(expand);
                SaveCustomWidget();
                try
                {
                    var doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                    doc.LibraryManager.DeleteLibrary(expand.LibraryGID);
                    System.IO.Directory.Delete(System.IO.Path.Combine(
                        WidgetGalleryViewModel.Widgetlibraryfolder,
                            string.Format("{0}", expand.LibraryGID)), true);
                }
                catch (Exception)
                {
                }
            }
        }

        private void LibraryExpandChangedEventHandler(object obj)
        {
            if (obj is WidgetExpand)
            {
                var expand = (obj as WidgetExpand);
                if (string.IsNullOrEmpty(_textSearch))
                {
                    /// normal condition
                    expand.ExpandCache = expand.IsExpand;
                    
                    NLogger.Debug("{0} expandcache : {1}", expand.Header, expand.ExpandCache);
                    if (ICONWidgetLibraryTab.WidgetExpands.Contains(expand) && expand.IsExpand)
                    {
                        /// if the expand is in the icon tab
                        /// and the expand is opened
                        /// then close all other expands in icon tab
                        foreach (var exp in ICONWidgetLibraryTab
                            .WidgetExpands
                            .Where(e => e.IsExpand)
                            .Except(new List<WidgetExpand> { expand }))
                        {
                            exp.IsExpand = false;
                        }
                    }
                }
            }
        }

        private void DomLoadedEventHandler(FileOperationType loadType)
        {
            switch (loadType)
            {
                case FileOperationType.Create:
                case FileOperationType.Open:
                    FirePropertyChanged("DocTypeVisibility");
                    break;
            }
        }

        private void ExportToMyLibraryAction(object obj)
        {
            var filePath = MyWidgetLibraryTab.ExportLibraryFileToLocal();
            if (filePath != null)
            {
                //Clipbord mode
                //var data = new DataObject();
                //data.SetData(@"ProtoNowLibRefreshID", filePath);
                //Int32 m_Message = Win32MsgHelper.RegisterWindowMessage("EXPORT_CURRENT_TO_MY_LIBRARY");
                //try
                //{
                //    Clipboard.SetDataObject(data);
                //}
                //catch
                //{
                //}
                //finally
                //{
                //    Win32MsgHelper.PostMessage((IntPtr)Win32MsgHelper.HWND_BROADCAST, m_Message, IntPtr.Zero, IntPtr.Zero);
                //}


                //SharedMemory Mode
                Int32 m_Message = Win32MsgHelper.RegisterWindowMessage("EXPORT_CURRENT_TO_MY_LIBRARY");
                try
                {
                    IShareMemoryService ShareMemSrv = ServiceLocator.Current.GetInstance<ShareMemorServiceProvider>();
                    ShareMemSrv.SetShareDate(filePath);
                }
                catch
                {
                }
                finally
                {
                    Win32MsgHelper.PostMessage((IntPtr)Win32MsgHelper.HWND_BROADCAST, m_Message, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        private void RefreshCustomLibraryAction(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            MyWidgetLibraryTab.LoadLibraryFromPath(fileName);
        }
    }
}
