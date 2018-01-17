using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using System.IO;
using Microsoft.Win32.SafeHandles;
using ICSharpCode.SharpZipLib.Zip;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    internal class Document : IDocument, ILibrary
    {
        internal Document(DocumentType type)
        {
            _documentData = new DocumentData(this, type); 

            _versionData = new VersionData(this);

            _imagesData = new ImagesData(this);
                        
            // Initialize working directory so that we can save page data and stream data in working directory,
            // even if we don't save this document.
            InitializeWorkingDirectory();

            _isOpened = true; // Document is opened once it is created.
        }

        #region IDisposable

        public void Dispose()
        {
            Close();
        }

        #endregion

        #region IDocument
        public string TimeStamp
        {
            get { return _documentData.TimeStamp; }
        }

        public Guid Guid
        {
            get { return _documentData.Guid; }
            set { _documentData.Guid = value; }
        }

        public string Name
        {
            get
            {
                // Refer to DSTUDIO-1714
                // Open, Save, Close will change this property.
                lock (this)
                {
                    return _fileName;
                }
            }

            set { throw new NotSupportedException("Cannot change the document name."); }
        }

        public DocumentType DocumentType
        {
            // We need document type when loading document page, so we don't check open here.
            get { return _documentData.DocumentType; }
            set { _documentData.DocumentType = value; }
        }

        public string FileVersion
        {
            get { return _versionData.FileVersion; }
        }

        public string CreatedAssemblyVersion
        {
            get { return _versionData.CreatedAssemblyVersion; }
        }

        public string CreatedProductName
        {
            get { return _versionData.CreatedProductName; }
        }

        public string CreatedProductVersion
        {
            get { return _versionData.CreatedProductVersion; }
        }

        public string ThisVersion
        {
            get { return VersionData.THIS_FILE_VERSION; }
        }

        public string Title
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_fileName))
                {
                    try
                    {
                        return Path.GetFileNameWithoutExtension(_fileName);
                    }
                    catch (Exception exp)
                    {
                        Debug.WriteLine(exp.Message);
                    }
                }

                return "";
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                // Easy to debug
                if (_isDirty != value)
                {
                    _isDirty = value;
                }
            }
        }

        public bool IsOpened
        {
            get
            {
                lock (this)
                {
                    return _isOpened;
                }
            }
        }

        public IHostService HostService
        {
            get { return _service; }
            set { _service = value; }
        }

        public IPages Pages
        {
            get { return _documentData.Pages; }
        }

        public IMasterPages MasterPages
        {
            get { return _documentData.MasterPages; }
        }

        public IAnnotationFieldSet PageAnnotationFieldSet
        {
            get { return _documentData.PageAnnotationFieldSet; }
        }

        public IAnnotationFieldSet WidgetAnnotationFieldSet
        {
            get { return _documentData.WidgetAnnotationFieldSet; }
        }

        public IWidgetDefaultStyleSet WidgetDefaultStyleSet
        {
            get { return _documentData.WidgetDefaultStyleSet; }
        }

        public IGuides GlobalGuides
        {
            get { return _documentData.GlobalGuides; }
        }

        public IGeneratorConfigurationSet GeneratorConfigurationSet
        {
            get { return _documentData.GeneratorConfigurationSet; }
        }

        public IAdaptiveViewSet AdaptiveViewSet
        {
            get { return _documentData.AdaptiveViewSet; }
        }

        public IDeviceSet DeviceSet
        {
            get { return _documentData.DeviceSet; }
        }

        public IDocumentSettings DocumentSettings
        {
            get { return _documentData.DocumentSettings; }
        }


        public IDocumentPage CreatePage(string pageName)
        {
            CheckOpen();

            DocumentPage page = null;
            if (_documentData.DocumentType == DocumentType.Library)
            {
                page = new CustomObjectPage(this, pageName);
            }
            else
            {
                page = new StandardPage(this, pageName);
            }

            AddPage(page);

            return page;
        }

        public void DeletePage(Guid pageGuid)
        {
            CheckOpen();

            Page page = _documentData.Pages.Get(pageGuid);
            if (page != null)
            {
                page.OnDeleteFromDocument();

                _documentData.Pages.Remove(pageGuid);
            }
        }

        public void AddPage(IDocumentPage page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            CheckOpen();

            DocumentPage pageToAdd = page as DocumentPage;
            if (pageToAdd != null && !_documentData.Pages.Contains(pageToAdd.Guid))
            {
                _documentData.Pages.Add(pageToAdd.Guid, pageToAdd);

                pageToAdd.ParentDocument = this;

                pageToAdd.OnAddToDocument();
            }
        }

        public IDocumentPage DuplicatePage(Guid pageGuid)
        {
            CheckOpen();

            if (!Pages.Contains(pageGuid))
            {
                throw new ArgumentException("pageGuid");
            }

            Serializer writer = new Serializer(_workingDirectoryGuid, Guid.Empty, _workingImagesDirectory);
            if (_documentData.DocumentType == DocumentType.Library)
            {
                CustomObjectPage sourcePage = Pages[pageGuid] as CustomObjectPage;
                writer.AddCustomObjectPage(sourcePage);
            }
            else
            {
                StandardPage sourcePage = Pages[pageGuid] as StandardPage;
                writer.AddStandardPage(sourcePage);
            }

            IDocumentPage newPage = null;
            using (Stream stream = writer.WriteToStream())
            {
                IObjectContainer container = AddPages(stream);
                if (_documentData.DocumentType == DocumentType.Library)
                {
                    if (container.CustomObjectPageList.Count > 0)
                    {
                        newPage = container.CustomObjectPageList[0];
                    }
                }
                else
                {
                    if (container.StandardPageList.Count > 0)
                    {
                        newPage = container.StandardPageList[0];
                    }
                }
            }

            return newPage;
        }

        public IObjectContainer AddPages(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            CheckOpen();

            Serializer container = new Serializer();

            Serializer reader = new Serializer(stream);
            reader.ReadAllFromStream();
            
            List<IDocumentPage> pages = new List<IDocumentPage>();
            if (_documentData.DocumentType == DocumentType.Library)
            {
                pages.AddRange(reader.CustomObjectPageList);
            }
            else
            {
                pages.AddRange(reader.StandardPageList);
            }

            foreach (DocumentPage page in pages)
            {
                // They are from the different document
                if (reader.WorkingDirectoryGuid != _workingDirectoryGuid)
                {
                    List<IRegion> regionList = new List<IRegion>();
                    regionList.AddRange(page.Widgets);
                    regionList.AddRange(page.Masters);
                    List<IRegion> newOrderList = new List<IRegion>(regionList.OrderBy(x => x.RegionStyle.Z));

                    Dictionary<Guid, IObjectContainer> newTargets = new Dictionary<Guid, IObjectContainer>();

                    // Break away all masters
                    foreach (Master master in page.Masters)
                    {
                        IObjectContainer objects = reader.GetObjectsInAttachedMasterPage(master.MasterPageGuid);
                        newTargets[master.Guid] = objects;

                        // Update Guids as there may be many masters were created from the same master page.
                        Page.UpdateObjectGuids(objects);

                        Group parentGroup = master.ParentGroup as Group;
                        if (parentGroup != null)
                        {
                            // Remove this master from its parent group.
                            parentGroup.RemoveMaster(master);
                        }

                        // Ajust broken away widgets location
                        double deltaX = master.MasterStyle.X;
                        double deltaY = master.MasterStyle.Y;

                        // Replace master with its broken widgets.
                        int masterIndex = newOrderList.IndexOf(master);
                        newOrderList.InsertRange(masterIndex, objects.WidgetList);
                        newOrderList.Remove(master);

                        foreach(Widget widget in objects.WidgetList)
                        {
                            if (!page.PageData.Widgets.Contains(widget.Guid))
                            {
                                page.PageData.Widgets.Add(widget.Guid, widget);

                                widget.ParentPage = page; // Set parent page.

                                if (widget.ParentGroup == null && parentGroup != null)
                                {
                                    // Add widget created by breaking master away to parent group if it is not in a group.
                                    parentGroup.AddWidget(widget);
                                }

                                widget.WidgetStyle.X += deltaX;
                                widget.WidgetStyle.Y += deltaY;
                            }
                        }

                        foreach (Group group in objects.GroupList)
                        {
                            if (!page.PageData.Groups.Contains(group.Guid))
                            {
                                page.PageData.Groups.Add(group.Guid, group);

                                group.ParentPage = page;

                                if (group.ParentGroup == null && parentGroup != null)
                                {
                                    // Add group created by breaking master away to parent group.
                                    parentGroup.AddGroup(group);
                                }
                            }
                        }
                    }

                    foreach (Widget widget in page.Widgets)
                    {
                        // Rebuild widget base style
                        widget.RebuildStyleChain(reader.GetWidgetBaseStyle(widget.Guid));

                        // Update action which has the master as a target.
                        widget.UpdateActions(newTargets);
                    }

                    // Delete all masters
                    page.DeleteAllMasters();

                    // Reset all widgets Zorder
                    int index = 0;
                    foreach (Widget widget in newOrderList)
                    {
                        widget.WidgetStyle.Z = index;
                        index++;
                    }
                }

                // Update all guids in this new page.
                page.UpdateAllGuids();

                // Add this page to the document.
                AddPage(page);

                // Save embedded page data to the working directory.
                foreach (PageEmbeddedWidget embeddedWidget in page.Widgets.OfType<PageEmbeddedWidget>())
                {
                    foreach (EmbeddedPage embeddedPage in embeddedWidget.EmbeddedPages)
                    {
                        // Save embedded page and close it.
                        embeddedPage.Close();
                    }
                }

                // Save page data to the working directory and close it.
                page.Close();

                container.AddPageInternal(page);
            }

            return container;
        }

        public IMasterPage CreateMasterPage(ISerializeWriter writer, string pageName, Stream thumbnail)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            CheckOpen();

            IMasterPage newMasterPage = CreateMasterPage(pageName);

            newMasterPage.Open();

            PageView baseView = newMasterPage.PageViews[AdaptiveViewSet.Base.Guid] as PageView;
            IObjectContainer container = baseView.AddObjects(writer.WriteToStream());
            
            // Adjust widgets position from coordinate (0,0);
            baseView.PageViewStyle.X = 0;
            baseView.PageViewStyle.Y = 0;

            if (thumbnail != null)
            {
                newMasterPage.Thumbnail = thumbnail;
            }

            newMasterPage.Close(); 

            return newMasterPage;
        }

        public IMasterPage CreateMasterPage(string pageName)
        {
            CheckOpen();

            MasterPage page = new MasterPage(this, pageName);
            AddMasterPage(page);

            return page;
        }

        public void DeleteMasterPage(Guid pageGuid)
        {
            CheckOpen();

            MasterPage masterPage = _documentData.MasterPages.Get(pageGuid);
            if (masterPage != null)
            {
                masterPage.OnDeleteFromDocument();

                _documentData.MasterPages.Remove(pageGuid);
            }
        }

        public void AddMasterPage(IMasterPage page)
        {
            CheckOpen();

            MasterPage pageToAdd = page as MasterPage;
            if (pageToAdd != null && !_documentData.MasterPages.Contains(pageToAdd.Guid))
            {
                _documentData.MasterPages.Add(pageToAdd.Guid, pageToAdd);

                pageToAdd.ParentDocument = this;

                pageToAdd.OnAddToDocument();
            }
        }

        public IMasterPage DuplicateMasterPage(Guid pageGuid)
        {
            CheckOpen();

            if (!MasterPages.Contains(pageGuid))
            {
                throw new ArgumentException("pageGuid");
            }

            Serializer writer = new Serializer(_workingDirectoryGuid, Guid.Empty, _workingImagesDirectory);
            MasterPage sourcePage = MasterPages[pageGuid] as MasterPage;
            writer.AddMasterPage(sourcePage);

            IMasterPage newPage = null;
            using (Stream stream = writer.WriteToStream())
            {
                IObjectContainer container = AddMasterPages(stream);
                if (container.MasterPageList.Count > 0)
                {
                    newPage = container.MasterPageList[0];
                }
            }

            newPage.IsLockedToMasterLocation = sourcePage.IsLockedToMasterLocation;

            return newPage;
        }
        
        public IObjectContainer AddMasterPages(Stream stream)
        {
            CheckOpen();

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            Serializer container = new Serializer();

            Serializer reader = new Serializer(stream);
            reader.ReadAllFromStream();

            foreach (MasterPage page in reader.MasterPageList)
            {
                if(reader.WorkingDirectoryGuid != _workingDirectoryGuid)
                {
                    // They are from different document.
                    foreach(Widget widget in page.Widgets)
                    {
                        widget.RebuildStyleChain(reader.GetWidgetBaseStyle(widget.Guid));
                    }
                }

                page.UpdateAllGuids();
                
                AddMasterPage(page);

                // Save page data to the working directory.
                page.Save();

                // Save embedded page data to the working directory.
                foreach (PageEmbeddedWidget embeddedWidget in page.Widgets.OfType<PageEmbeddedWidget>())
                {
                    foreach (EmbeddedPage embeddedPage in embeddedWidget.EmbeddedPages)
                    {
                        // Save embedded page.
                        embeddedPage.Save();
                    }
                }

                container.AddPageInternal(page);
            }

            return container;
        }

        public IGuide CreateGlobalGuide(Orientation orientation, double x = 0, double y = 0)
        {
            CheckOpen();

            Guide guide = new Guide(orientation, x, y);
            _documentData.GlobalGuides.Add(guide.Guid, guide);
            return guide;
        }

        public void DeleteGlobalGuide(Guid guideGuid)
        {
            CheckOpen();

            _documentData.GlobalGuides.Remove(guideGuid);
        }

        public void AddGlobalGuide(IGuide guide)
        {
            CheckOpen();

            Guide guideObject = guide as Guide;
            if (guideObject != null && !_documentData.GlobalGuides.Contains(guideObject.Guid))
            {
                _documentData.GlobalGuides.Add(guideObject.Guid, guideObject);
            }
        }

        public ISerializeWriter CreateSerializeWriter(Guid currentViewGuid)
        {
            return new Serializer(_workingDirectoryGuid, currentViewGuid, _workingImagesDirectory);
        }

        public ISerializeReader CreateSerializeReader(Stream stream)
        {
            return new Serializer(stream);
        }
      
        #endregion

        #region Document Internal Methods

        internal void Open(string fileName)
        {
            lock (this)
            {
                if (!File.Exists(fileName))
                {
                    throw new ArgumentException("The specified file doesn't exist!");
                }

                // Clear the previous document data and unlock previous file, working directory.
                Close();

                _fileName = fileName;

                // Initialize working directory
                InitializeWorkingDirectory();

                // Unzip the file to working directory.
                UnzipFile();

                // Load Version.xml
                _versionData.Load(Path.Combine(_workingDirectory.FullName, VERSION_FILE_NAME));

                // Check if we can open the specified file. Currently we cannot open higher Major version file.
                Version fileVersion = new Version(_versionData.FileVersion);
                Version thisVersion = new Version(VersionData.THIS_FILE_VERSION);
                if (fileVersion.Major > thisVersion.Major)
                {
                    // We cannot open file which major number is greater than THIS_FILE_VERSION 
                    Close();
                    throw new HigherDocumentVersionException(VersionData.THIS_FILE_VERSION, _versionData.FileVersion,
                        "Cannot open higher version file! The current DOM version is " + VersionData.THIS_FILE_VERSION + " .");
                }
                else if (fileVersion.Major < thisVersion.Major)
                {
                    // Todo: Major update should notify user to save. For now, we update silently.
                    // Major update if needed.
                    LegacyDocumentManager legacyManager = new LegacyDocumentManager(this);
                    if (legacyManager.UpdateToThisFileVersion())
                    {
                        // Document data is updated in the working directory, save the new file data in file.
                        // Zip working directory to file.
                        ZipFile(_fileName, false);
                    }
                }

                // Load Images.Xml
                _imagesData.Load(Path.Combine(_workingDirectory.FullName, IMAGES_FILE_NAME));

                // Load Document.xml
                _documentData.Load(Path.Combine(_workingDirectory.FullName, DOCUMENT_FILE_NAME));

                // If the document is a standard document, lock the file so that the file cannot be modified by other process.
                // Do not lock library document, it can be modified by multiple process.
                if (_documentData.DocumentType == DocumentType.Standard)
                {
                    LockFile();
                }

                // Load all pages (including embedded pages) guids from _workingPagesDirectory 
                // so that we know all pages guid in this document.
                LoadAllPagesGuids();

                _isOpened = true;
            }
        }

        public void Save(string fileName, bool saveCopy = false)
        {
            CheckOpen();

            lock (this)
            {
                // Initialize working directory and lock it.
                InitializeWorkingDirectory();

                if (String.IsNullOrEmpty(_fileName))
                {
                    // This is a new document.
                    _fileName = fileName;
                }
                else
                {
                    // This document was opened or saved before.

                    // Save to a new file, unlock the previous file first.
                    if (_fileName != fileName)
                    {
                        UnlockFile();
                        _fileName = fileName;

                        if (!saveCopy)
                        {
                            // Create a new guid for the new file if it is not a copy. We just update the document guid for now.
                            _documentData.Guid = Guid.NewGuid();
                        }
                    }
                }

                // Save Version.xml
                _versionData.Save(Path.Combine(_workingDirectory.FullName, VERSION_FILE_NAME));

                // Save Document.xml
                _documentData.Save(Path.Combine(_workingDirectory.FullName, DOCUMENT_FILE_NAME));      

                // Save Images.xml
                _imagesData.Save(Path.Combine(_workingDirectory.FullName, IMAGES_FILE_NAME));

                // Save opened pages
                List<Page> allPages = new List<Page>(_allPages.Values);
                foreach (Page page in allPages)
                {
                    if (page != null && page.IsOpened)
                    {
                        page.Save();
                    }
                }

                // Unlock file temporarily so that we can zip working directory to this file.
                UnlockFile();

                // Zip working directory to file.
                ZipFile(_fileName, true);

                if (DocumentType == DocumentType.Standard)
                {
                    // Lock the file after saving if it is a standard document.
                    LockFile();
                }
            }
        }

        public void SaveCopyTo(string fileName)
        {
            CheckOpen();

            lock (this)
            {
                if (_workingDirectory != null)
                {
                    // Save Version.xml
                    _versionData.Save(Path.Combine(_workingDirectory.FullName, VERSION_FILE_NAME));

                    _documentData.Save(Path.Combine(_workingDirectory.FullName, DOCUMENT_FILE_NAME));

                    // Save Images.xml
                    _imagesData.Save(Path.Combine(_workingDirectory.FullName, IMAGES_FILE_NAME));

                    // Save opened pages.
                    List<Page> allPages = new List<Page>(_allPages.Values);
                    foreach (Page page in allPages)
                    {
                        if (page != null && page.IsOpened)
                        {
                            page.Save();
                        }
                    }

                    // Zip working directory to file.
                    ZipFile(fileName, true);
                }
            }
        }

        internal void Close()
        {
            lock (this)
            {
                if (_isOpened)
                {
                    // Close all pages.
                    try
                    {
                        foreach (Page page in _allPages.Values)
                        {
                            // Only dispose document page, embedded page will be disposed by its parent document page.
                            if (page != null && page is DocumentPage)
                            {
                                page.Dispose();
                            }
                        }
                    }
                    catch(Exception exp)
                    {
                        Debug.WriteLine(exp.Message);
                    }
                    UnlockFile();
                    _documentData = new DocumentData(this, DocumentType.Standard);
                    _versionData = new VersionData(this);
                    _allPages.Clear();
                    _fileName = null;
                    _isDirty = false;
                    _isOpened = false;
                }
                ///out of _isOpened  in order to clear work directory when unzip exception.
                UninitializeWorkingDirectory();
            }
        }

        internal Guid WorkingDirectoryGuid
        {
            get { return _workingDirectoryGuid; }
        }

        internal DirectoryInfo WorkingDirectory
        {
            get { return _workingDirectory; }
        }

        internal DirectoryInfo WorkingPagesDirectory
        {
            get { return _workingPagesDirectory; }
        }

        internal DirectoryInfo WorkingImagesDirectory
        {
            get { return _workingImagesDirectory; }
        }

        internal DocumentData DocumentData
        {
            get { return _documentData; }
        }

        internal VersionData VersionData
        {
            get { return _versionData; }
        }

        internal ImagesData ImagesData
        {
            get { return _imagesData; }
        }

        public IHashStreamManager ImagesStreamManager
        {
            get 
            {
                if(_imagesStreamManager == null && _workingImagesDirectory != null)
                {
                    _imagesStreamManager = new HashStreamManager(this);
                }

                return _imagesStreamManager; 
            }
        }

        // All pages in this document, including embedded pages.
        internal Dictionary<Guid, Page> AllPages
        {
            get { return _allPages; }
        }

        #endregion

        #region Events

        internal void OnAddAdaptiveView(AdaptiveView view)
        {
            foreach (Page page in _allPages.Values)
            {
                if (page != null && page.IsOpened)
                {
                    page.OnAddAdaptiveView(view);
                }
            }
        }

        internal void OnDeleteAdaptiveView(AdaptiveView view)
        {
            foreach (Page page in _allPages.Values)
            {
                if (page != null && page.IsOpened)
                {
                    page.OnDeleteAdaptiveView(view);
                }
            }
        }

        internal void OnChangeAdaptiveViewParent(AdaptiveView view, AdaptiveView oldParentView)
        {
            foreach (Page page in _allPages.Values)
            {
                if (page != null && page.IsOpened)
                {
                    page.OnChangeAdaptiveViewParent(view, oldParentView);
                }
            }
        }

        #endregion

        #region ILibrary

        public ReadOnlyCollection<ICustomObject> CustomObjects 
        {
            get
            {
                UpdateLastAccessTime();

                if (_documentData.DocumentType == DocumentType.Library)
                {
                    return new ReadOnlyCollection<ICustomObject>(_documentData.Pages.OfType<CustomObjectPage>().ToList<ICustomObject>());
                }
                else
                {
                    // Return a empty collection if this documen is not a library.
                    return new ReadOnlyCollection<ICustomObject>(new List<ICustomObject>());
                }
            }
        }

        public ICustomObject GetCustomObject(Guid objectGuid)
        {
            UpdateLastAccessTime();

            if (_documentData.DocumentType == DocumentType.Library)
            {
                return _documentData.Pages[objectGuid] as CustomObjectPage;
            }
            else
            {
                return null;
            }
        }

        public ICustomObject AddCustomObject(ISerializeWriter writer, string objectName, Stream icon, Stream thumbnail)
        {
            CheckOpen();

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (_documentData.DocumentType == DocumentType.Library)
            {
                return LibraryManager.CreateCustomObject(this, writer, objectName, icon, thumbnail);
            }
            else
            {
                return null;
            }
        }

        public void DeleteCustomObject(Guid objectGuid)
        {
            DeletePage(objectGuid);

            // Delete TreeNode
            ITreeNode node = _documentData.DocumentSettings.LayoutSetting.PageTree;
            DeleteTreeNode(node, objectGuid);
        }

        #endregion

        #region Library Internal Methods

        internal void UpdateLastAccessTime()
        {
            LastAccessTime = DateTime.Now.Ticks;
        }

        internal long LastAccessTime
        {
            get;
            set;
        }

        #endregion

        #region Private Methods

        private void CheckOpen()
        {
            if (!IsOpened)
            {
                throw new DocumentIsClosedException("Document is closed.");
            }
        }

        private void InitializeWorkingDirectory()
        {
            if (_workingDirectory == null)
            {
                // Create a temp unique directory as working directory(This directory typically resides in C:\Users\Victor\AppData\Local\Temp)
                DirectoryInfo info = new DirectoryInfo(Path.Combine(Path.GetTempPath(), TEMP_FOLDER_NAME));
                if (!info.Exists)
                {
                    info.Create();
                }

                _workingDirectoryGuid = Guid.NewGuid();

                // Create document working directory.
                _workingDirectory = info.CreateSubdirectory(_workingDirectoryGuid.ToString());

                 // Create pages sub directory.
                _workingPagesDirectory = _workingDirectory.CreateSubdirectory(PAGES_FOLDER_NAME);

                // Create images sub directory.
                _workingImagesDirectory = _workingDirectory.CreateSubdirectory(IMAGES_FOLDER_NAME);

                // Lock working directory so that the directory cannot be modified by other process.
                LockWorkingDirectory();
            }
        }

        private void UninitializeWorkingDirectory()
        {
            if (_imagesStreamManager != null)
            {
                _imagesStreamManager.Dispose();
                _imagesStreamManager = null;
            }

            UnlockWorkingDirectory();
            try
            {
                if (_workingDirectory != null)
                {
                    _workingDirectory.Delete(true);
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine(exp.Message);
            }

            _workingDirectory = null;
            _workingPagesDirectory = null;
            _workingImagesDirectory = null;
            _workingDirectoryGuid = Guid.Empty;
        }

        private void LockFile()
        {
            // Lock the zipped file, other process only can ready it.

            UnlockFile();
            _fileSafeHandle = Win32Wrapper.CreateFile(_fileName, Win32Wrapper.DESIRED_ACCESS_GENERIC_WRITE, Win32Wrapper.SHARE_MODE_READ,
                                                      IntPtr.Zero, Win32Wrapper.OPEN_EXISTING, 0, IntPtr.Zero);
            if (_fileSafeHandle.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), _fileName); ;
            }
        }

        private void UnlockFile()
        {
            if (_fileSafeHandle != null && !_fileSafeHandle.IsClosed)
            {
                _fileSafeHandle.Close();
                _fileSafeHandle = null;
            }
        }

        // Lock working directory so that the directory cannot be modified by other process.
        private void LockWorkingDirectory()
        {
            // Lock the working directory, other process CAN NOT access it. Use Win32Wrapper.SHARE_MODE_NONE, not Win32Wrapper.SHARE_MODE_READ.
            // This way, we prevent third-party software(like anti-virus, or delete manually by people) from accessing the document working 
            // folder and it can not delete any files and sub directories.

            UnlockWorkingDirectory();
            _directorySafeHandle = Win32Wrapper.CreateFile(_workingDirectory.FullName, Win32Wrapper.DESIRED_ACCESS_GENERIC_WRITE,
                                                           Win32Wrapper.SHARE_MODE_NONE/*Win32Wrapper.SHARE_MODE_READ*/ , IntPtr.Zero,
                                                           Win32Wrapper.OPEN_EXISTING, Win32Wrapper.FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero);
            if (_directorySafeHandle.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), _workingDirectory.FullName);
            }
        }

        private void UnlockWorkingDirectory()
        {
            if (_directorySafeHandle != null && !_directorySafeHandle.IsClosed)
            {
                _directorySafeHandle.Close();
                _directorySafeHandle = null;
            }
        }

        /*
         * [pages] + [A]
         *         + [B]
         *         + [C]
         *         + [D]
         * To remove C and D from the zip file, directory filter is "-C;-D;".
         * 
         * The following expression includes all file name ending in '.dat' with the exception of 'dummy.dat'
         *  "+\.dat$;-^dummy\.dat$"
         * */
        private void ZipFile(string fileName, bool filter)
        {
            // Filter pages which are not in the document. filter pattern is like "\\page_guid$" which is that a directory full
            // name is end with \page_guid.
            string directoryFilter = "";

            // Filter files.
            string fileFilter = "";

            if (filter)
            {
                // Filter page directories.
                foreach(DirectoryInfo pageDir in _workingPagesDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        Guid guid = new Guid(pageDir.Name);

                        if (!_allPages.ContainsKey(guid))
                        {
                            directoryFilter += @"-\\";
                            directoryFilter += pageDir.Name;
                            directoryFilter += @"$;";
                        }
                        else
                        {
                            // Check embedded pages.
                            foreach (DirectoryInfo childPageDir in pageDir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                            {
                                if (string.CompareOrdinal(childPageDir.Name, IMAGES_FOLDER_NAME) == 0)
                                {
                                    continue;
                                }

                                Guid childGuid = new Guid(childPageDir.Name);

                                if (!_allPages.ContainsKey(childGuid))
                                {
                                    directoryFilter += @"-\\";
                                    directoryFilter += childPageDir.Name;
                                    directoryFilter += @"$;";
                                }
                            }
                        }
                    }
                    catch(Exception exp)
                    {
                        Debug.WriteLine(exp.Message);

                        // Also filter the directory which name is not Guid. 
                        // "pages" directory only has page guid sub directories.
                        directoryFilter += @"-\\";
                        directoryFilter += pageDir.Name;
                        directoryFilter += @"$;";
                    }
                }

                // Filter image files.
                foreach(FileInfo imageFile in _workingImagesDirectory.EnumerateFiles())
                {
                    string hash = Path.GetFileNameWithoutExtension(imageFile.Name);
                    if (!_imagesData.AnyConsumer(hash))
                    {
                        fileFilter += @"-\\";
                        fileFilter += hash;
                        fileFilter += @"\.*;";
                    }
                }
            }

            UnlockWorkingDirectory(); // Unlock the document working directory so that sharpzip can zip this folder.

            _fastZip.CreateZip(fileName, _workingDirectory.FullName, true, fileFilter, directoryFilter);

            LockWorkingDirectory(); // Lock the document working directory again.
        }

        private void UnzipFile()
        {
            // Unzip file to working directory.
            _fastZip.ExtractZip(_fileName, _workingDirectory.FullName, "");
        }

        private void LoadAllPagesGuids()
        {
            if(_workingPagesDirectory != null)
            {
                foreach (DirectoryInfo pageDir in _workingPagesDirectory.EnumerateDirectories("*", SearchOption.AllDirectories))
                {
                    Guid guid;
                    if(Guid.TryParse(pageDir.Name, out guid))
                    {
                        // Only add the key, the value is null untill you open the page.
                        if(!_allPages.ContainsKey(guid))
                        {
                            _allPages.Add(guid, null);
                        }
                    }
                }
            }
        }

        private void DeleteTreeNode(ITreeNode node, Guid guid)
        {
            if (node.NodeType == TreeNodeType.Page
                && node.AttachedObject != null
                && node.AttachedObject.Guid == guid)
            {
                node.RemoveMe();
            }
            else
            {
                foreach (ITreeNode childNode in node.ChildNodes)
                {
                    DeleteTreeNode(childNode, guid);
                }
            }
        }

        #endregion

        #region Private Fields

        private IHostService _service;
        private bool _isDirty;
        private string _fileName;
        private bool _isOpened;

        private FastZip _fastZip = new FastZip();
        private Guid _workingDirectoryGuid;
        private DirectoryInfo _workingDirectory;
        private DirectoryInfo _workingPagesDirectory;
        private DirectoryInfo _workingImagesDirectory;
        private SafeFileHandle _fileSafeHandle;
        private SafeFileHandle _directorySafeHandle;
        
        private DocumentData _documentData;
        private VersionData _versionData;
        private ImagesData _imagesData;

        private HashStreamManager _imagesStreamManager;
                
        // All pages in this document, the page could be null if it is an embedded page and it is closed.
        private Dictionary<Guid, Page> _allPages = new Dictionary<Guid, Page>();

        #endregion

        #region Constants

        public const string TEMP_FOLDER_NAME = @"Compass";

        public const string DOCUMENT_FILE_NAME = @"Document.xml";
        public const string VERSION_FILE_NAME = @"Version.xml";
        public const string PAGE_FILE_NAME = @"Page.xml";
        public const string IMAGES_FILE_NAME = @"Images.xml";

        public const string PAGE_THUMBNAIL_FILE_NAME = "Thumbnail.png";
        public const string LIBRARY_ICON_FILE_NAME = "Icon.png";

        public const string PAGES_FOLDER_NAME = @"pages";
        public const string IMAGES_FOLDER_NAME = @"images";

        public const string STANDARD_FILE_SUFFIX = @".pn";
        public const string LIBRARY_FILE_SUFFIX = @".libpn";

        #endregion
    }
}
