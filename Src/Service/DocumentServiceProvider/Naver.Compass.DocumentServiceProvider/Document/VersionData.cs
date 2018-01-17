using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Win32;
using System.Reflection;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    // Save file version in a sole xml file, so we can check the version without loading 
    // large size Document.xml.
    internal class VersionData : XmlDocumentObject
    {
        static VersionData()
        {
            _currentAssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public VersionData(Document document)
            : base("Version")
        {
            Debug.Assert(document != null);
            _ownerDocument = document;

            UpdateToCurrentVersion();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            if(!LoadStringFromChildElementInnerText("FileVersion", element, ref _fileVersion))
            {
                throw new XmlException("Bad version information. Cannot load FileVersion element!");
            }

            LoadStringFromChildElementInnerText("CreatedAssemblyVersion", element, ref _createdAssemblyVersion);
            LoadStringFromChildElementInnerText("CreatedProductName", element, ref _createdProductName);
            LoadStringFromChildElementInnerText("CreatedProductVersion", element, ref _createdProductVersion);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            xmlDoc.AppendChild(element);

            // Always save current version information
            SaveStringToChildElement("FileVersion", THIS_FILE_VERSION, xmlDoc, element);
            SaveStringToChildElement("CreatedAssemblyVersion", CurrentAssemblyVersion, xmlDoc, element);
            SaveStringToChildElement("CreatedProductName", CurrentProductName, xmlDoc, element);
            SaveStringToChildElement("CreatedProductVersion", CurrentProductVersion, xmlDoc, element);
        }

        #endregion

        public void UpdateToCurrentVersion()
        {
            _fileVersion = THIS_FILE_VERSION;
            _createdAssemblyVersion = CurrentAssemblyVersion;
            _createdProductName = CurrentProductName;
            _createdProductVersion = CurrentProductVersion;
        }

        public void Clear()
        {
            _fileVersion = "";
            _createdAssemblyVersion = "";
            _createdProductName = "";
            _createdProductVersion = "";
        }

        public string FileVersion
        {
            get { return _fileVersion; }
        }

        public string CreatedAssemblyVersion
        {
            get { return _createdAssemblyVersion; }
        }

        public string CreatedProductName
        {
            get { return _createdProductName; }
        }

        public string CreatedProductVersion
        {
            get { return _createdProductVersion; }
        }

        public string CurrentAssemblyVersion
        {
            get { return _currentAssemblyVersion; }
        }

        public string CurrentProductName
        {
            get
            {
                if (_ownerDocument != null && _ownerDocument.HostService != null)
                {
                    return  _ownerDocument.HostService.ProductName;
                }

                return "";
            }
        }

        public string CurrentProductVersion
        {
            get
            {
                if (_ownerDocument != null && _ownerDocument.HostService != null)
                {
                    return _ownerDocument.HostService.ProductVersion;
                }

                return "";
            }
        }

        private Document _ownerDocument;

        private string _fileVersion;
        private string _createdProductName;
        private string _createdProductVersion;
        private string _createdAssemblyVersion;

        private static string _currentAssemblyVersion;

        // Version format: major.minor.build.revision
        public const string THIS_FILE_VERSION = @"9.0.0.1";
    }
}
