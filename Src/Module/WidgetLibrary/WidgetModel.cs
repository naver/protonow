using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Naver.Compass.Module
{
    public class WidgetModel : ICustomWidget, INotifyPropertyChanged
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LbrType { get; set; }
        public string Type { get; set; }
        public string Icon { get; set; }
        public string SvgIcon { get; set; }
        public string LocalizedName { get; set; }
        public string ToolTip { get; set; }
        public string[] Tags { get; set; }
        public string NClickCode { get; set; }

        public WidgetModel()
        {
            this.FavouriteCommand = new DelegateCommand<object>(FavouriteExecute);
            this.ClickTypeCommand = new DelegateCommand<object>(ClickTypeExecute);
            this.DeleteCustomObjectCommand = new DelegateCommand<object>(DeleteCustomObjectExecute);
            this.NameChangedCommand = new DelegateCommand<object>(NameChangedExecute);
            Tags = new string[] { };
            Name = string.Empty;
        }

        # region View Model
        public void LocalizedTextChanged()
        {
            this.FirePropertyChanged("InlineContent");
        }

        public WidgetModelType EnumType
        {
            get
            {
                WidgetModelType result = (WidgetModelType)(-1);
                Enum.TryParse<WidgetModelType>(Type, out result);
                return result;
            }
        }

        [XmlIgnore]
        public string LocalizedText
        {
            get
            {
                if (!string.IsNullOrEmpty(LocalizedName))
                {
                    var resourceStr = Application.Current.FindResource(LocalizedName) as string;
                    if (!string.IsNullOrEmpty(resourceStr))
                    {
                        return resourceStr;
                    }
                    else
                    {
                        return LocalizedName;
                    }
                }
                else
                {
                    return Name;
                }
            }
        }

        [XmlIgnore]
        public InlineContent InlineContent
        {
            get
            {
                var localizedTxt = LocalizedText;
                var inlineContent = new InlineContent();
                if (string.IsNullOrEmpty(_searchTxt))
                {
                    ///empty search keyword
                    inlineContent.Add(new InlineInfo { Text = localizedTxt });
                    return inlineContent;
                }

                if (this.LocalizedText.ToLower().Contains(_searchTxt.ToLower()))
                {
                    /// keyword match name
                    var replacedinput = Regex.Replace(
                        localizedTxt,
                        _searchTxt,
                        "\u0000" + "$0" + "\u0000",
                        RegexOptions.IgnoreCase);
                    var splits = replacedinput.Split(new string[] { "\u0000" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var split in splits)
                    {
                        var inlineinfo = new InlineInfo { Text = split };
                        if (split.Equals(this._searchTxt, StringComparison.OrdinalIgnoreCase))
                        {
                            inlineinfo.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x56, 0x9d, 0xe5));
                        }

                        inlineContent.Add(inlineinfo);
                    }

                    return inlineContent;
                }

                /// keyword match tags
                var matchTag = this.Tags.FirstOrDefault(t => t.Equals(_searchTxt, StringComparison.OrdinalIgnoreCase));
                if (matchTag != null)
                {
                    inlineContent.Add(
                        new InlineInfo
                        {
                            Text = this.LocalizedText
                        });
                }

                return inlineContent;
            }
        }

        private string _searchTxt;
        public void Search(string searchTxt)
        {
            _searchTxt = searchTxt;
            IsEditable = string.IsNullOrEmpty(searchTxt);

            if (string.IsNullOrEmpty(_searchTxt)
                || this.LocalizedText.ToLower().Contains(_searchTxt.ToLower())
                || this.Tags.Any(t => t.Equals(_searchTxt, StringComparison.OrdinalIgnoreCase)))
            {
                this.IsVisible = true;
                this.FirePropertyChanged("InlineContent");
            }
            else
            {
                this.IsVisible = false;
            }
        }

        [XmlIgnore]
        public bool IsSvg
        {
            get
            {
                return EnumType == WidgetModelType.svg;
            }
        }

        private ImageSource _imageSource;
        [XmlIgnore]
        public ImageSource ImageSource
        {
            get
            {
                try
                {
                    if (_imageSource != null)
                    {
                        return _imageSource;
                    }

                    if (!string.IsNullOrEmpty(SvgIcon))
                    {
                        var imgSource = new Uri(WidgetGalleryViewModel.packPath + SvgIcon, UriKind.RelativeOrAbsolute);
                        var imgStreamInfo = Application.GetResourceStream(imgSource);
                        var imgStream = (imgStreamInfo != null) ? imgStreamInfo.Stream : null;
                        var settings = new WpfDrawingSettings();
                        settings.CultureInfo = settings.NeutralCultureInfo;
                        if (imgStream != null)
                        {
                            using (imgStreamInfo.Stream)
                            {
                                using (FileSvgReader reader = new FileSvgReader(settings))
                                {
                                    var drawGroup = reader.Read(imgStreamInfo.Stream);
                                    if (drawGroup != null)
                                    {
                                        _imageSource = new DrawingImage(drawGroup);
                                        return _imageSource;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var base64 = Convert.FromBase64String(Icon);
                        var ms = new MemoryStream(base64);
                        var imageSource = new BitmapImage();
                        imageSource.BeginInit();
                        imageSource.StreamSource = ms;
                        imageSource.EndInit();
                        _imageSource = imageSource;
                        return _imageSource;
                    }

                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        private bool isEditable = true;
        [XmlIgnore]
        public bool IsEditable
        {
            get { return isEditable; }
            set
            {
                if (isEditable != value)
                {
                    isEditable = value;
                    this.FirePropertyChanged("IsEditable");
                }
            }
        }

        [XmlIgnore]
        public bool IsInEditMode
        {
            get;
            set;
        }

        private bool isFavourite;
        [XmlIgnore]
        public bool IsFavourite
        {
            get { return isFavourite; }
            set
            {
                if (isFavourite != value)
                {
                    isFavourite = value;
                    this.FirePropertyChanged("IsFavourite");
                }
            }
        }

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
        public bool IsClickonFavourite { get; set; }
        [XmlIgnore]
        public DelegateCommand<object> FavouriteCommand { get; private set; }
        [XmlIgnore]
        public DelegateCommand<object> ClickTypeCommand { get; private set; }
        [XmlIgnore]
        public DelegateCommand<object> DeleteCustomObjectCommand { get; private set; }
        [XmlIgnore]
        public DelegateCommand<object> NameChangedCommand { get; private set; }

        public void FavouriteExecute(object cmdParameter)
        {
            var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _ListEventAggregator.GetEvent<WidgetFavouriteEvent>().Publish(this);
        }

        public void DeleteCustomObjectExecute(object cmdParameter)
        {
            if (cmdParameter is WidgetExpand)
            {
                var expand = cmdParameter as WidgetExpand;
                expand.DeleteCustomObject(this);
            }
        }

        /// <summary>
        /// Mouse enter/ leave to set IsClickonFavourite
        /// </summary>
        /// <param name="cmdparameter">1: click on favourite icon 0: click on widget icon</param>
        public void ClickTypeExecute(object cmdparameter)
        {
            if (cmdparameter == null)
                return;
            if ("1" == cmdparameter.ToString())
            {
                IsClickonFavourite = true;
            }
            else
            {
                IsClickonFavourite = false;
            }

        }

        public void NameChangedExecute(object cmdparameter)
        {
            var paramters = cmdparameter as object[];
            if (paramters != null && paramters.Length == 2 && paramters[0] is WidgetExpand && paramters[1] is CustomEventArgs<string>)
            {
                var expand = paramters[0] as WidgetExpand;
                var eventArgs = paramters[1] as CustomEventArgs<string>;

                var doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                try
                {
                    var iloadLibrary = doc.LibraryManager.GetLibrary(expand.LibraryGID, expand.FileName);
                    if (iloadLibrary != null)
                    {
                        var customObject = iloadLibrary.GetCustomObject(this.Id);
                        if (customObject != null)
                        {
                            customObject.Name = eventArgs.Item1;
                            iloadLibrary.Save(expand.FileName);
                        }
                    }


                    this.Name = eventArgs.Item1;
                    var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
                    _ListEventAggregator.GetEvent<CustomWidgetChangedEvent>().Publish(expand);
                }
                catch (Exception ex)
                {
                    NLogger.Warn("Failed to rename custom objec {0},ex:{1}", this.Id, ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        internal WidgetModel Clone()
        {
            return (WidgetModel)this.MemberwiseClone();
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
