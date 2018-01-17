using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Naver.Compass.Service.Document
{
    public class DocumentService : IDocumentService
    {
        public void NewDocument(DocumentType type)
        {
            try
            {
                _document = new Document(type);
                _document.HostService = this;
                _document.IsDirty = true;
            }
            catch (Exception)
            {
                _document = null;
                throw;
            }
        }

        public void Open(string fileName)
        {
            if (_document == null)
            {
                NewDocument(DocumentType.Standard);
            }

            try
            {
                _document.Open(fileName);

                _document.IsDirty = false;
            }
            catch (Exception)
            {
                _document.Close();
                _document = null;
                throw;
            }
        }

        public void Save(string fileName, bool saveCopy = false)
        {
            if(_document == null)
            {
                throw new Exception("Document is null!");
            }

            _document.Save(fileName, saveCopy);

            _document.IsDirty = false;
        }

        public void SaveCopyTo(string fileName)
        {
            if (_document == null)
            {
                throw new Exception("Document is null!");
            }

            _document.SaveCopyTo(fileName);
        }

        public void Close()
        {
            if (_document == null)
            {
                throw new Exception("Document is null!");
            }

            _document.Close();

            _document = null;
        }

        public void Dispose()
        {
            if (_document != null)
            {
                _document.Close();
            }

            _libraryManager.Dispose();
        }

        public IDocument Document 
        {
            get { return _document; }
        }

        public ILibraryManager LibraryManager
        {
            get { return _libraryManager; }
        }

        public IStyle WidgetSystemStyle
        {
            get { return _systemStyle; }
        }

        public string ProductName
        {
            get { return @"protoNow"; }
        }

        public string ProductVersion
        {
            get
            {
                if(String.IsNullOrEmpty(_productVersion))
                {
                    try
                    {
                        RegistryKey versionKey = Registry.LocalMachine.OpenSubKey(@"Software\Design Studio");
                        if (versionKey != null)
                        {
                            _productVersion = versionKey.GetValue("CurrentVersion").ToString();
                        }
                    }
                    catch
                    {
                    }
                }

                return _productVersion;
            }
        }

        private string _productVersion;
        private Document _document;
        private LibraryManager _libraryManager = new LibraryManager();
        private Style _systemStyle = new Style("");
    }
}
