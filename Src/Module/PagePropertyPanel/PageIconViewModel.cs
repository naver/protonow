using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using Naver.Compass.Common.CommonBase;
using Microsoft.Practices.Prism.Events;
using System.Collections.ObjectModel;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Naver.Compass.Service;
using Naver.Compass.Common.Helper;

namespace Naver.Compass.Module
{
    public class PageIconViewModel:ViewModelBase
    {
        public PageIconViewModel()
        {
            if (this.IsInDesignMode)
                return;

            this.ImportCommand = new DelegateCommand<object>(ImportExecute);
            this.ClearCommand = new DelegateCommand<object>(ClearExecute);

            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler, ThreadOption.UIThread);            
        }

        #region command

        public DelegateCommand<object> ImportCommand { get; private set; }
        public DelegateCommand<object> ClearCommand { get; private set; }

        private void ImportExecute(object obj)
        {
            EditorFocus();
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = CommonDefine.ImageFilter;
            if (openFile.ShowDialog() == false)
            {
                return;
            }

            try
            {
                if (_page != null)
                {
                    Stream oldStream;
                    if (ImgSource != null)
                    {
                        oldStream = new MemoryStream((int)_page.Icon.Length);
                        _page.Icon.Seek(0, SeekOrigin.Begin);
                        _page.Icon.CopyTo(oldStream);
                        oldStream.Seek(0, SeekOrigin.Begin);
                    }
                    else
                    {
                        oldStream = null;
                    }

                    MemoryStream stream = new MemoryStream(File.ReadAllBytes(openFile.FileName));
                    ImportIcon(stream);
                    if (CurrentUndoManager != null)
                    {
                        ImportPageIconCommand imgCmd = new ImportPageIconCommand(this, oldStream, stream);
                        CurrentUndoManager.Push(imgCmd);
                    }
                }
            }
            catch (System.Exception e)
            {
                NLogger.Warn("Import page icon failed: ",e.Message);
            }
        }

        private void ClearExecute(object obj)
        {
            EditorFocus();

            Stream oldStream = GetStream();
            if (oldStream == null)
                return;

            ClearIcon();
            if(CurrentUndoManager!=null)
            {
                ClearPageIconCommand clearCmd = new ClearPageIconCommand(this, oldStream);
                CurrentUndoManager.Push(clearCmd);
            }
        }


        private void SelectionPageChangeHandler(Guid pageGuid)
        {
            if (_document == null || _document.DocumentType == DocumentType.Standard)
                return;

            if (pageGuid == Guid.Empty )
            {
                _page = null;
                FirePropertyChanged("ImgSource");
                FirePropertyChanged("DefaultVisibility");
                return;
            }
            ICustomObjectPage obj = _pageEditor.ActivePage as ICustomObjectPage;
            ICustomObjectPage page = _document.Pages.GetPage(pageGuid) as ICustomObjectPage;
            if (page != null)
            {           
                _page = page;
                _isUseThumbailIcon = page.UseThumbnailAsIcon;
                IsPageIconEnabled = true;
                FirePropertyChanged("IsUseThumbailIcon");
                FirePropertyChanged("ImgSource");
                FirePropertyChanged("DefaultVisibility");
            }
            else
            {
                //Disable Icon set if current page is  Toast/Hamburger menu... 's child page
                IsPageIconEnabled = false;
            }
        }
        #endregion

        #region public member
        public void ClearIcon()
        {
            if (_page != null)
            {
                _page.Icon = null;
            }
            FirePropertyChanged("ImgSource");
            FirePropertyChanged("DefaultVisibility");
        }
        
        public void ImportIcon(Stream iconStream)
        {
            _page.Icon = iconStream;
            _document.IsDirty = true;
            FirePropertyChanged("ImgSource");
            FirePropertyChanged("DefaultVisibility");
        }
        #endregion

        #region private member

        bool _isUseThumbailIcon = true;
        bool _isPageIconEnabled = true;
        IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        ICustomObjectPage _page
        {
            get;
            set;
        }

        IPagePropertyData _pageEditor
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SelectionServiceProvider>().GetCurrentPage();
            }
        }

        /// <summary>
        /// Let canvas editor get focus, or undo/redo will not be activated.
        /// </summary>
        private void EditorFocus()
        {
            IPagePropertyData page = _pageEditor;
            if (page != null && page.EditorCanvas != null)
            {
                page.EditorCanvas.Focus();
            }
        }
        private Stream GetStream()
        {
            Stream oldStream;
            if (ImgSource != null)
            {
                oldStream = new MemoryStream((int)_page.Icon.Length);
                _page.Icon.Seek(0, SeekOrigin.Begin);
                _page.Icon.CopyTo(oldStream);
                oldStream.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                oldStream = null;
            }
            return oldStream;
        }
        #endregion

        #region property

        public bool IsUseThumbailIcon
        {
            get
            {
                return _isUseThumbailIcon;
            }
            set
            {
                if (value != _isUseThumbailIcon)
                {
                    EditorFocus();
                    if (CurrentUndoManager != null)
                    {
                        PropertyChangeCommand cmd = new PropertyChangeCommand(this, "Raw_IsUseThumbailIcon", _isUseThumbailIcon, value);
                        CurrentUndoManager.Push(cmd);
                    }                   

                    Raw_IsUseThumbailIcon = value;
                }
            }
        }

        public bool Raw_IsUseThumbailIcon
        {
            set
            {
                if (_isUseThumbailIcon != value)
                {
                    _isUseThumbailIcon = value;

                    if (_pageEditor != null)
                    {
                        if(!_isUseThumbailIcon)
                        {
                            _page.Icon = null;
                        }
                        _pageEditor.IsUseThumbnailAsIcon = value;
                        _document.IsDirty = true;
                        FirePropertyChanged("IsUseThumbailIcon");
                        FirePropertyChanged("ImgSource");
                        FirePropertyChanged("DefaultVisibility");
                    }
                }
            }
        }
        public ImageSource ImgSource
        {
            get
            {
                if (_page != null && !IsUseThumbailIcon)
                {
                    Stream icon = _page.Icon;
                    if (null == icon)
                    {
                        return null;
                    }
                    else
                    {
                        icon.Seek(0, SeekOrigin.Begin);
                        ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                        return imageSourceConverter.ConvertFrom(icon) as BitmapFrame;
                    }
                }
                return null;
            }
        }

        public Visibility DefaultVisibility
        {
            get
            {
                return ImgSource == null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsPageIconEnabled
        {
            get
            {
                return _isPageIconEnabled;
            }
            set
            {
                if(_isPageIconEnabled!=value)
                {
                    _isPageIconEnabled = value;
                    FirePropertyChanged("IsPageIconEnabled");
                }
            }
        }
        #endregion
    }
}
