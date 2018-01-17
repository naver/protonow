using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class CustomObjectPage : DocumentPage, ICustomObjectPage, ICustomObject
    {
        internal CustomObjectPage(Document document, string pageName)
            : base("CustomObjectPage", document)
        {            
            _pageData = new CustomObjectPageData(this, "CustomObjectPage");
            _pageData.Name = pageName;

            InitializeBasePageView();
        }

        public override void AddMaster(IMaster master)
        {
            throw new CannotAddMasterException("Cannot add Master to an CustomObjectPage.");
        }

        #region ICustomObjectPage

        public bool UseThumbnailAsIcon
        {
            get { return _pageData.UseThumbnailAsIcon; }
            set { _pageData.UseThumbnailAsIcon = value; }
        }

        public Stream Icon
        {
            get 
            {
                if(_icon != null)
                {
                    return _icon.DataStream; 
                }

                return null;
            }

            set 
            {
                if (_icon != null)
                {
                    _icon.DataStream = value;
                }
                else
                {
                    if (value != null)
                    {
                        _icon = new StreamFileObject(null);
                        _icon.DataStream = value;
                    }
                }
            }
        }

        public string Tooltip
        {
            get { return _pageData.Tooltip; }
            set { _pageData.Tooltip = value; }
        }

        public override void Dispose()
        {
            lock (this)
            {
                base.Dispose();

                if (_serializeStream != null)
                {
                    _serializeStream.Dispose();
                    _serializeStream = null;
                }

                if (_icon != null)
                {
                    _icon.Dispose();
                    _icon = null;
                }
            }
        }

        #endregion

        #region Page 

        internal override void Save()
        {
            base.Save();

            if (_icon != null)
            {
                _icon.SaveStreamToFile();
            }
        }
        
        internal override PageData PageData
        {
            get { return _pageData; }
        }

        internal override void OnAddToDocument()
        {
            base.OnAddToDocument();

            if(_icon == null)
            {
                _icon = new StreamFileObject(Path.Combine(_workingDirectory.FullName, Document.LIBRARY_ICON_FILE_NAME));
            }
            else
            {
                if (string.IsNullOrEmpty(_icon.StreamFileName))
                {
                    _icon.StreamFileName = Path.Combine(Path.Combine(_workingDirectory.FullName, Document.LIBRARY_ICON_FILE_NAME));
                }
            }
        }

        #endregion

        #region CustomObject

        public ILibrary ParentLibrary
        {
            get { return _document; }
        }

        internal Stream SerializeStream
        {
            get
            {
                if (_serializeStream == null)
                {
                    Guid workingDirectoryGuid = Guid.Empty;
                    DirectoryInfo imagesDir = null;
                    if (_document != null)
                    {
                        workingDirectoryGuid = _document.WorkingDirectoryGuid;
                        imagesDir = _document.WorkingImagesDirectory;
                    }

                    Serializer writer = new Serializer(workingDirectoryGuid, Guid.Empty, imagesDir);
                    writer.AddCustomObjectPage(this);
                    _serializeStream = writer.WriteToStream();
                }

                return _serializeStream;
            }
            set
            {
                _serializeStream = value;
            }
        }

        internal void UpdateLastAccessTime()
        {
            if(_document != null)
            {
                _document.UpdateLastAccessTime();
            }
        }

        #endregion
        
        private CustomObjectPageData _pageData;
        private StreamFileObject _icon;
        private Stream _serializeStream;
    }
}
