using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Naver.Compass.Module
{
    public class WidgetExpand : ICustomLibrary, INotifyPropertyChanged
    {
        public Guid LibraryGID { get; set; }
        private string _header;
        public string Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    this.FirePropertyChanged("Header");
                }
            }
        }
        public string FileName { get; set; }

        [XmlIgnore]
        private bool isFavoriteHintVisible;
        [XmlIgnore]
        public bool IsFavoriteHintVisible
        {
            get { return isFavoriteHintVisible; }
            set
            {
                if (isFavoriteHintVisible != value)
                {
                    isFavoriteHintVisible = value;
                    this.FirePropertyChanged("IsFavoriteHintVisible");
                }
            }
        }

        [XmlIgnore]
        private bool isLibraryHintVisible;
        [XmlIgnore]
        public bool IsLibraryHintVisible
        {
            get { return isLibraryHintVisible; }
            set
            {
                if (isLibraryHintVisible != value)
                {
                    isLibraryHintVisible = value;
                    this.FirePropertyChanged("IsLibraryHintVisible");
                }
            }
        }

        private object _itemChangedInfo;
        [XmlIgnore]
        public object ItemChangedInfo
        {
            get { return _itemChangedInfo; }
            set
            {
                if (_itemChangedInfo != value)
                {
                    _itemChangedInfo = value;
                    this.FirePropertyChanged("ItemChangedInfo");
                }
            }
        }


        public string Raw_FileName
        {
            get
            {
                if (string.IsNullOrEmpty(FileName))
                {
                    return FileName;
                }

                if (FileName.StartsWith(WidgetGalleryViewModel.Widgetlibraryfolder))
                {
                    return FileName;
                }
                else
                {
                    return Path.Combine(WidgetGalleryViewModel.Widgetlibraryfolder, FileName);
                }
            }
        }
        public RangeObservableCollection<WidgetModel> WidgetModels { get; set; }
        public WidgetExpand()
        {
            WidgetModels = new RangeObservableCollection<WidgetModel>();
        }

        public IEnumerable<ICustomWidget> GetAllCustomWidgets()
        {
            return WidgetModels.Cast<ICustomWidget>();
        }

        # region View Model
        private bool isCustomWidget;
        [XmlIgnore]
        public bool IsCustomWidget
        {
            get { return isCustomWidget; }
            set
            {
                if (isCustomWidget != value)
                {
                    isCustomWidget = value;
                    this.FirePropertyChanged("IsCustomWidget");
                }
            }
        }

        private bool isExpand;
        [XmlIgnore]
        public bool IsExpand
        {
            get { return isExpand; }
            set
            {
                if (isExpand != value)
                {
                    isExpand = value;
                    this.FirePropertyChanged("IsExpand");
                    var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                    _ListEventAggregator.GetEvent<LibraryExpandChangedEvent>().Publish(this);
                }
            }
        }
        [XmlIgnore]
        public bool ExpandCache { get; set; }

        private bool isVisible;
        [XmlIgnore]
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    this.FirePropertyChanged("IsVisible");
                }
            }
        }

        [XmlIgnore]
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    this.FirePropertyChanged("SearchText");
                }
            }
        }

        private double _settingOpacity = 0;
        [XmlIgnore]
        public double SettingOpacity
        {
            get
            {
                return _settingOpacity;
            }
            set
            {
                if (_settingOpacity != value)
                {
                    _settingOpacity = value;
                    this.FirePropertyChanged("SettingOpacity");
                }
            }
        }

        [XmlIgnore]
        public string TabType { get; set; }


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

        private DelegateCommand<object> _showSettingCommand;
        [XmlIgnore]
        public DelegateCommand<object> ShowSettingCommand
        {
            get
            {
                if(_showSettingCommand == null)
                {
                    _showSettingCommand = new DelegateCommand<object>(ShowSettingExecute);
                }
                return _showSettingCommand;
            }
        }

        private DelegateCommand<object> _hideSettingCommand;
        [XmlIgnore]
        public DelegateCommand<object> HideSettingCommand
        {
            get
            {
                if (_hideSettingCommand == null)
                {
                    _hideSettingCommand = new DelegateCommand<object>(HideSettingExecute);
                }
                return _hideSettingCommand;
            }
        }

        public void ContextMenuOpenExecute(object cmdParameter)
        {
            if (cmdParameter is object[])
            {
                var paramters = cmdParameter as object[];
                if (paramters.Length == 2
                    && paramters[0] is ContextMenu
                    && paramters[1] is System.Windows.Input.MouseButtonEventArgs)
                {
                    var contextMenu = paramters[0] as ContextMenu;
                    var mbea = paramters[1] as System.Windows.Input.MouseButtonEventArgs;
                    if (contextMenu != null && mbea.Source is StackPanel)
                    {
                        contextMenu.Items.Clear();
                        if (IsCustomWidget)
                        {
                            var exportLibrary = new MenuItem { Header = Application.Current.FindResource("Libraries_ContextMenu_ExportToLibrary") as string };
                            exportLibrary.Click += exportLibrary_Click;
                            contextMenu.Items.Add(exportLibrary);

                            var renameLibrary = new MenuItem { Header = Application.Current.FindResource("Libraries_ContextMenu_RenameLibrary") as string };
                            renameLibrary.Click += renameLibrary_Click;
                            contextMenu.Items.Add(renameLibrary);

                            var editLibrary = new MenuItem { Header = Application.Current.FindResource("Libraries_ContextMenu_EditLibrary") as string };
                            editLibrary.Click += editLibrary_Click;
                            contextMenu.Items.Add(editLibrary);

                            var refreshLibrary = new MenuItem { Header = Application.Current.FindResource("Libraries_ContextMenu_RefreshLibrary") as string };
                            refreshLibrary.Click += refreshLibrary_Click;
                            contextMenu.Items.Add(refreshLibrary);

                            var resetLibrary = new MenuItem { Header = Application.Current.FindResource("Libraries_ContextMenu_ResetLibrary") as string };
                            if (this.WidgetModels.Count == 0)
                            {
                                resetLibrary.IsEnabled = false;
                            }

                            resetLibrary.Click += resetLibrary_Click;
                            contextMenu.Items.Add(resetLibrary);

                            var deleteLibrary = new MenuItem { Header = Application.Current.FindResource("Libraries_ContextMenu_DeleteFromMyLibrary") as string };
                            deleteLibrary.Click += deleteLibrary_Click;
                            contextMenu.Items.Add(deleteLibrary);

                        }
                        else
                        {
                            var resetLibrary = new MenuItem { Header = Application.Current.FindResource("Libraries_ContextMenu_ResetLibrary") as string };
                            if (this.WidgetModels.Count == 0)
                            {
                                resetLibrary.IsEnabled = false;
                            }

                            resetLibrary.Click += resetFavorite_Click;
                            contextMenu.Items.Add(resetLibrary);
                        }

                        Task task = new Task(() =>
                        {
                            System.Threading.Thread.Sleep(100);
                            contextMenu.Dispatcher.BeginInvoke(new Action(() => { contextMenu.IsOpen = true; }));
                        });

                        task.Start();
                    }
                }
            }
        }

        void ShowSettingExecute(object cmdParameter)
        {
            SettingOpacity = 1;
        }
        void HideSettingExecute(object cmdParameter)
        {
            SettingOpacity = 0;
        }
        void deleteLibrary_Click(object sender, RoutedEventArgs e)
        {
            var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _ListEventAggregator.GetEvent<DeleteLibraryWidgetEvent>().Publish(this);
        }

        void resetLibrary_Click(object sender, RoutedEventArgs e)
        {
            var doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            try
            {
                var iloadLibrary = doc.LibraryManager.GetLibrary(this.LibraryGID, this.FileName);
                if (iloadLibrary != null)
                {
                    foreach (var widgetModel in this.WidgetModels.ToList())
                    {
                        iloadLibrary.DeleteCustomObject(widgetModel.Id);
                        this.WidgetModels.Remove(widgetModel);
                    }

                    iloadLibrary.Save(this.FileName);
                    var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                    _ListEventAggregator.GetEvent<CustomWidgetChangedEvent>().Publish(this);
                    this.FireItemChangedInfo();
                }
            }
            catch (Exception ex)
            {
                NLogger.Warn("resetLibrary_Click->Failed to delete custom object to library " + this.Header + ",ex:" + ex.Message); 

                MessageBox.Show(GlobalData.FindResource("Error_Delete_CustomObject"));
            }
        }

        void resetFavorite_Click(object sender, RoutedEventArgs e)
        {
            var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _ListEventAggregator.GetEvent<ResetFavouriteEvent>().Publish(this);
            this.FireItemChangedInfo();
        }

        void refreshLibrary_Click(object sender, RoutedEventArgs e)
        {
            this.Refresh();
        }

        private void exportLibrary_Click(object sender, RoutedEventArgs e)
        {
            this.Export();
        }

        private void renameLibrary_Click(object sender, RoutedEventArgs e)
        {
            var renameWindow = new RenameWindow();
            renameWindow.NewName = this.Header;
            renameWindow.Owner = Application.Current.MainWindow;
            renameWindow.ShowDialog();
            if (renameWindow.Result.HasValue && renameWindow.Result.Value)
            {
                this.Header = renameWindow.NewName;
            }
        }

        private void editLibrary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                path += @"\protoNow.exe";

                //Solve the space problem:arguments will be splited by space in Process.Start()
                var filename = "\"" + this.FileName + "\"";

                filename += " edit ";

                filename += GlobalData.Culture;

                System.Diagnostics.Process.Start(path,filename);

                var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                _ListEventAggregator.GetEvent<DisplayAppLoadingEvent>().Publish(true);
            }
            catch (Exception ex)
            {
                NLogger.Warn("Edit Library failed. ex:{0}", ex.ToString());

            }
        }
        private void Export()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = CommonDefine.LibraryFilter;
            dialog.FileName = this.Header;
            var rst = dialog.ShowDialog();
            if (rst.HasValue && rst.Value)
            {
                var doc = ServiceLocator.Current.GetInstance<IDocumentService>();

                try
                {
                    var iLibrary = doc.LibraryManager.GetLibrary(this.LibraryGID);
                    if (iLibrary == null)
                    {
                        File.Copy(this.FileName, dialog.FileName, true);
                    }
                    else
                    {
                        iLibrary.SaveCopyTo(dialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn("Export library failed. ex:{0}", ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void AddCustomObject(Guid customObjectGuid)
        {
            var doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            try
            {
                var iloadLibrary = doc.LibraryManager.GetLibrary(this.LibraryGID, this.FileName);
                ICustomObject customObject = iloadLibrary.GetCustomObject(customObjectGuid);
                if (customObject != null)
                {

                    var bytes = default(byte[]);
                    if (customObject.Icon != null)
                    {
                        bytes = new byte[customObject.Icon.Length];
                        customObject.Icon.Read(bytes, 0, bytes.Length);
                    }

                    this.WidgetModels.Insert(0, new WidgetModel
                    {
                        Id = customObject.Guid,
                        Name = customObject.Name,
                        Icon = bytes != null ? Convert.ToBase64String(bytes) : null,
                        LbrType = "custom"
                    });
                    
                    iloadLibrary.Save(this.FileName);
                    this.IsExpand = true;
                    this.ExpandCache = true;
                    var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                    _ListEventAggregator.GetEvent<CustomWidgetChangedEvent>().Publish(this);
                    this.FireItemChangedInfo();
                }

            }
            catch (Exception ex)
            {
                NLogger.Warn("Failed to add custom objec to library {0},ex:{1}", this.Header, ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        internal void DeleteCustomObject(WidgetModel widgetModel)
        {
            var doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            try
            {
                var iloadLibrary = doc.LibraryManager.GetLibrary(this.LibraryGID, this.FileName);
                if (iloadLibrary != null)
                {
                    iloadLibrary.DeleteCustomObject(widgetModel.Id);
                    iloadLibrary.Save(this.FileName);
                    this.WidgetModels.Remove(widgetModel);
                    var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                    _ListEventAggregator.GetEvent<CustomWidgetChangedEvent>().Publish(this);
                    this.FireItemChangedInfo();
                }
            }
            catch (Exception ex)
            {
                NLogger.Warn("Failed to delete custom objec to library {0},ex:{1}", this.Header, ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        public void Refresh()
        {
            var doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            try
            {
                var loadLibrary = doc.LibraryManager.LoadLibrary(this.FileName);
                if (loadLibrary != null && loadLibrary.CustomObjects != null && loadLibrary.CustomObjects.Count > 0)
                {
                    this.WidgetModels.Clear();
                    foreach (var customObject in loadLibrary.CustomObjects)
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

                        this.WidgetModels.Add(widgetModel);
                    }

                    /// Notify to refresh ui.
                    var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                    _ListEventAggregator.GetEvent<CustomWidgetChangedEvent>().Publish(this);
                    this.FireItemChangedInfo();
                }
                else
                {
                    ///No custom objects exist in the library, remove it
                    this.RemoveThis();
                }
            }
            catch (Exception ex)
            {
                NLogger.Warn("Reload library file failed. ex :{0} \r\nIt means that current file has been removed or destroyed. Remove the library from xml.", ex.ToString());
                this.RemoveThis();
            }
        }

        private void RemoveThis()
        {
            var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _ListEventAggregator.GetEvent<DeleteLibraryWidgetEvent>().Publish(this);
        }

        public void FireItemChangedInfo()
        {
            ItemChangedInfo = new object();
        }

        internal WidgetExpand Clone()
        {
            var cloneObj = (WidgetExpand)this.MemberwiseClone();
            cloneObj.WidgetModels = new RangeObservableCollection<WidgetModel>();
            cloneObj.WidgetModels.AddRange(this.WidgetModels.Select(x=>x.Clone()));
            return cloneObj;
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
    }
}
