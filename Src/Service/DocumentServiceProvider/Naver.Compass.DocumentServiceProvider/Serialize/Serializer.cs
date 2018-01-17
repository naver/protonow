using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml;
using System.IO.Compression;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    /*
     * Xml schema of Serializer:
     * 
     * <Serializer FileVersion = "x.x.x.x">
     *     
     *     <WorkingDirectoryGuid>x-x-x-x</WorkingDirectoryGuid>
     *     <CurrentViewGuid>x-x-x-x</CurrentViewGuid>
     *   
     *     <WidgetList>
     *         <Shape>...</Shape>
     *         <Image>...</Image>
     *         ...
     *     </WidgetList>
     *   
     *     <MasterList>
     *         <Master>...</Master>
     *         ...
     *     </MasterList>
     *   
     *     <GroupList>
     *         <Group>...</Group>
     *         ...
     *     </GroupList>
     *   
     *     <StandardPageList>
     *         <StandardPage>...</StandardPage>
     *         ...
     *     </StandardPageList>
     *     
     *     <CustomObjectPageList>
     *         <CustomObjectPage>...</CustomObjectPage>
     *         ...
     *     </CustomObjectPageList>
     *     
     *     <MasterPageList>
     *         <MasterPage>...</MasterPage>
     *         ...
     *     </MasterPageList>
     * 
     *     <AttachedMasterPageList>
     *         <MasterPage>...</MasterPage>
     *         ...
     *     </AttachedMasterPageList>
     *     
     *     <WidgetBaseStyleList>
     *         <WidgetBaseStyle  Guid = "[widget.Guid]">
     *             <WidgetStyle>...</WidgetStyle>
     *         </WidgetBaseStyle>
     *         ...
     *     </WidgetBaseStyleList>
     *   
     *     <MasterBaseStyleList>
     *         <MasterBaseStyle  Guid = "[master.Guid]">
     *             <MasterStyle>...</MasterStyle>
     *         </MasterBaseStyle>
     *         ...
     *     </MasterBaseStyleList>
     *   
     *     <StreamList>
     *         <Stream Hash = "[Stream hash value]">...</Stream>
     *         ...
     *     </StreamList>
     *     
     *     <EmbeddedPageList>
     *         <HamburgerMenuPage>...</HamburgerMenuPage>
     *         ...
     *     </EmbeddedPageList>
     *     
     *     <ThumbnailList>
     *         <Thumbnail Guid = "[page.Guid]">...</Thumbnail>
     *         ...
     *     </ThumbnailList>
     *     
     *     <IconList>
     *         <Icon Guid = "[page.Guid]">...</Icon>
     *         ...
     *     </IconList>
     *     
     * </Serializer>
     * */

    internal class Serializer : XmlDocumentObject, ISerializeReader, ISerializeWriter
    {
        #region Constructors

        // Constructor for container.
        internal Serializer()
            : this(Guid.Empty, null, Guid.Empty, null)
        {
        }

        // Constructor for reader.
        internal Serializer(Stream stream)
            : this(Guid.Empty, stream, Guid.Empty, null)
        {
        }

        // Constructor for writer.
        internal Serializer(Guid workingDirectoryGuid, Guid currentViewGuid, DirectoryInfo imagesDir)
            : this(workingDirectoryGuid, null, currentViewGuid, imagesDir)
        {
        }

        internal Serializer(Guid workingDirectoryGuid, Stream stream, Guid currentViewGuid, DirectoryInfo imagesDir)
            : base("Serializer")
        {
            _workingDirectoryGuid = workingDirectoryGuid;
            _currentViewGuid = currentViewGuid;
            _imagesDir = imagesDir;

            if (stream != null)
            {
                stream.Position = 0;
                
                using (MemoryStream deflateDecompressedStream = new MemoryStream())
                {
                    using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true))
                    {
                        deflateStream.CopyTo(deflateDecompressedStream);
                    }

                    deflateDecompressedStream.Position = 0;
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(deflateDecompressedStream);
                    _documentElement = xmlDoc.DocumentElement;
                }
            }
        }

        #endregion

        #region XmlDocumentObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            // Restore document element which is cleared in Clear().
            _documentElement = element;

            string version = "";
            LoadElementStringAttribute(element, "FileVersion", ref version);
            if (string.CompareOrdinal(version, VersionData.THIS_FILE_VERSION) != 0)
            {
                // Cannot handle different version serailized stream.
                return;
            }

            LoadGuidFromChildElementInnerText("WorkingDirectoryGuid", element, ref _workingDirectoryGuid);
            LoadGuidFromChildElementInnerText("CurrentViewGuid", element, ref _currentViewGuid);

            XmlElement widgetListElement = element[WIDGET_LIST_TAGNAME];
            XmlElement masterListElement = element[MASTER_LIST_TAGNAME];
            XmlElement groupListElement = element[GROUP_LIST_TAGNAME];

            XmlElement standardPageListElement = element[STANDARD_PAGE_LIST_TAGNAME];
            XmlElement customObjectPageListElement = element[CUSTOM_OBJECT_PAGE_LIST_TAGNAME];
            XmlElement masterPageListElement = element[MASTER_PAGE_LIST_TAGNAME];

            XmlElement attachedMasterPageListElement = element[ATTACHED_MASTER_PAGE_LIST_TAGNAME];

            XmlElement widgetBaseStyleListElement = element[WIDGET_BASE_STYLE_LIST_TAGNAME];
            XmlElement masterBaseStyleListElement = element[MASTER_BASE_STYLE_LIST_TAGNAME];

            XmlElement embeddedPageListElement = element[EMBEDDED_PAGE_LIST_TAGNAME];
            XmlElement streamListElement = element[STREAM_LIST_TAGNAME];
            XmlElement thumbnailListElement = element[THUMBNAIL_LIST_TAGNAME];
            XmlElement iconListElement = element[ICON_LIST_TAGNAME];

            if (_workingDirectoryGuid == Guid.Empty 
                || widgetListElement == null || masterListElement == null || groupListElement == null
                || standardPageListElement == null || customObjectPageListElement == null
                || masterPageListElement == null || attachedMasterPageListElement == null || embeddedPageListElement == null
                || widgetBaseStyleListElement == null || masterBaseStyleListElement == null
                || streamListElement == null || thumbnailListElement == null || iconListElement == null)
            {
                // _currentViewGuid could be Guid.Empty when copy pages.
                // This is invalid stream.
                return;
            }

            // Load and cache attachment elements first
            foreach (XmlElement childElement in attachedMasterPageListElement.ChildNodes)
            {
                Guid guid = Guid.Empty;
                if (LoadElementGuidAttribute(childElement, ref guid))
                {
                    _attachedMasterPageXmlElements.Add(guid, childElement);
                }
            }

            foreach (XmlElement childElement in widgetBaseStyleListElement.ChildNodes)
            {
                Guid guid = Guid.Empty;
                if (LoadElementGuidAttribute(childElement, ref guid))
                {
                    _widgetBaseStyleXmlElements.Add(guid, childElement);
                }
            }

            foreach (XmlElement childElement in masterBaseStyleListElement.ChildNodes)
            {
                Guid guid = Guid.Empty;
                if (LoadElementGuidAttribute(childElement, ref guid))
                {
                    _masterBaseStyleXmlElements.Add(guid, childElement);
                }
            }

            foreach (XmlElement childElement in embeddedPageListElement.ChildNodes)
            {
                Guid guid = Guid.Empty;
                if (LoadElementGuidAttribute(childElement, ref guid))
                {
                    _embeddedPageXmlElements.Add(guid, childElement);
                }
            }

            foreach (XmlElement childElement in streamListElement.ChildNodes)
            {
                string hash = "";
                LoadElementStringAttribute(childElement, "Hash", ref hash);
                if (!String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(childElement.InnerText))
                {
                    MemoryStream stream = new MemoryStream(Convert.FromBase64String(childElement.InnerText));
                    _hashStreams.Add(hash, stream);
                }
            }

            foreach (XmlElement childElement in thumbnailListElement.ChildNodes)
            {
                Guid guid = Guid.Empty;
                if (LoadElementGuidAttribute(childElement, ref guid))
                {
                    _thumbnailXmlElements.Add(guid, childElement);
                }
            }

            foreach (XmlElement childElement in iconListElement.ChildNodes)
            {
                Guid guid = Guid.Empty;
                if (LoadElementGuidAttribute(childElement, ref guid))
                {
                    _iconXmlElements.Add(guid, childElement);
                }
            }

            // Load widgets
            foreach (XmlElement widgetElement in widgetListElement.ChildNodes)
            {
                Widget widget = WidgetFactory.CreateWidget(null, widgetElement.Name) as Widget;
                if (widget == null)
                {
                    // Continue to load other widgets if something wrong happenned
                    continue;
                }

                widget.LoadDataFromXml(widgetElement);

                if (widget is StreamWidget)
                {
                    StreamWidget streamWidget = widget as StreamWidget;
                    if (_hashStreams.ContainsKey(streamWidget.Hash))
                    {
                        streamWidget.DataStream = GetStream(streamWidget.Hash);
                    }
                }
                else if (widget is PageEmbeddedWidget)
                {
                    if (widget is HamburgerMenu)
                    {
                        HamburgerMenu menu = widget as HamburgerMenu;
                        StreamWidget streamWidget = menu.MenuButton as StreamWidget;
                        if (_hashStreams.ContainsKey(streamWidget.Hash))
                        {
                            streamWidget.DataStream = GetStream(streamWidget.Hash);
                        }
                    }

                    PageEmbeddedWidget embeddedWidget = widget as PageEmbeddedWidget;
                    foreach (EmbeddedPage embeddedPage in embeddedWidget.EmbeddedPages)
                    {
                        if (_embeddedPageXmlElements.ContainsKey(embeddedPage.Guid))
                        {
                            // Embedded page doesn't have PageEmbeddedWidget.
                            embeddedPage.PageData.LoadDataFromXml(_embeddedPageXmlElements[embeddedPage.Guid]);
                            embeddedPage.ForceToSetPageOpened();
                            LoadFullPageData(embeddedPage);
                        }
                    }
                }

                _widgets[widget.Guid] = widget;
            }

            foreach (XmlElement masterElement in masterListElement.ChildNodes)
            {
                Master master = new Master(null, null);
                master.LoadDataFromXml(masterElement);
                _masters[master.Guid] = master;
            }

            foreach (XmlElement groupElement in groupListElement.ChildNodes)
            {
                Group group = new Group(null);
                group.LoadDataFromXml(groupElement);
                _groups[group.Guid] = group;
            }

            foreach (XmlElement pageElement in standardPageListElement.ChildNodes)
            {
                StandardPage page = new StandardPage(null, "");
                page.PageData.LoadDataFromXml(pageElement);
                page.ForceToSetPageOpened();
                LoadFullPageData(page);
                _standardPages[page.Guid] = page;
            }

            foreach (XmlElement pageElement in customObjectPageListElement.ChildNodes)
            {
                CustomObjectPage page = new CustomObjectPage(null, "");
                page.PageData.LoadDataFromXml(pageElement);
                page.ForceToSetPageOpened();
                LoadFullPageData(page);
                _customObjectPages[page.Guid] = page;
            }

            foreach (XmlElement pageElement in masterPageListElement.ChildNodes)
            {
                MasterPage page = new MasterPage(null, "");
                page.PageData.LoadDataFromXml(pageElement);
                page.ForceToSetPageOpened();
                LoadFullPageData(page);
                _masterPages[page.Guid] = page;
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            _hashStreams.Clear();
            
            XmlElement documentElement = xmlDoc.CreateElement(TagName);
            xmlDoc.AppendChild(documentElement);
            SaveElementStringAttribute(documentElement, "FileVersion", VersionData.THIS_FILE_VERSION);

            SaveStringToChildElement("WorkingDirectoryGuid", _workingDirectoryGuid.ToString(), xmlDoc, documentElement);
            SaveStringToChildElement("CurrentViewGuid", _currentViewGuid.ToString(), xmlDoc, documentElement);

            XmlElement widgetListElement = xmlDoc.CreateElement(WIDGET_LIST_TAGNAME);
            documentElement.AppendChild(widgetListElement);

            XmlElement masterListElement = xmlDoc.CreateElement(MASTER_LIST_TAGNAME);
            documentElement.AppendChild(masterListElement);

            XmlElement groupListElement = xmlDoc.CreateElement(GROUP_LIST_TAGNAME);
            documentElement.AppendChild(groupListElement);

            XmlElement standardPageListElement = xmlDoc.CreateElement(STANDARD_PAGE_LIST_TAGNAME);
            documentElement.AppendChild(standardPageListElement);

            XmlElement customObjectPageListElement = xmlDoc.CreateElement(CUSTOM_OBJECT_PAGE_LIST_TAGNAME);
            documentElement.AppendChild(customObjectPageListElement);

            XmlElement masterPageListElement = xmlDoc.CreateElement(MASTER_PAGE_LIST_TAGNAME);
            documentElement.AppendChild(masterPageListElement);

            // Attachments
            XmlElement attachedMasterPageListElement = xmlDoc.CreateElement(ATTACHED_MASTER_PAGE_LIST_TAGNAME);
            documentElement.AppendChild(attachedMasterPageListElement);

            XmlElement widgetBaseStyleListElement = xmlDoc.CreateElement(WIDGET_BASE_STYLE_LIST_TAGNAME);
            documentElement.AppendChild(widgetBaseStyleListElement);

            XmlElement masterBaseStyleListElement = xmlDoc.CreateElement(MASTER_BASE_STYLE_LIST_TAGNAME);
            documentElement.AppendChild(masterBaseStyleListElement);

            XmlElement embeddedPageListElement = xmlDoc.CreateElement(EMBEDDED_PAGE_LIST_TAGNAME);
            documentElement.AppendChild(embeddedPageListElement);

            XmlElement thumbnailListElement = xmlDoc.CreateElement(THUMBNAIL_LIST_TAGNAME);
            documentElement.AppendChild(thumbnailListElement);

            XmlElement iconListElement = xmlDoc.CreateElement(ICON_LIST_TAGNAME);
            documentElement.AppendChild(iconListElement);

            // Save widgets
            foreach (Widget widget in _widgets.Values)
            {
                widget.SaveDataToXml(xmlDoc, widgetListElement);

                // Save new base view style.
                SaveWidgetBaseStyle(xmlDoc, documentElement, widget);

                // Save stream of stream widgets.
                if (widget is StreamWidget)
                {
                    StreamWidget streamWidget = widget as StreamWidget;
                    SetStream(streamWidget.Hash, streamWidget.DataStream as MemoryStream);
                }
                else if (widget is PageEmbeddedWidget)
                {
                    // Save button image.
                    if (widget is HamburgerMenu)
                    {
                        HamburgerMenu menu = widget as HamburgerMenu;
                        StreamWidget streamWidget = menu.MenuButton as StreamWidget;
                        SetStream(streamWidget.Hash, streamWidget.DataStream as MemoryStream);
                    }

                    // Save child pages.
                    PageEmbeddedWidget embeddedWidget = widget as PageEmbeddedWidget;

                    foreach (EmbeddedPage embeddedPage in embeddedWidget.EmbeddedPages)
                    {
                        SaveEmbeddedPageElement(xmlDoc, documentElement, embeddedPage);
                    }
                }
            }

            // Save masters.
            foreach (Master master in _masters.Values)
            {
                master.SaveDataToXml(xmlDoc, masterListElement);

                SaveMasterBaseStyle(xmlDoc, documentElement, master);
            }

            // Save groups
            foreach (Group group in _groups.Values)
            {
                group.SaveDataToXml(xmlDoc, groupListElement);
            }

            // Save standard pages.
            foreach (StandardPage page in _standardPages.Values)
            {
                SavePageElement(xmlDoc, documentElement, standardPageListElement, page);
            }

            // Save custom object pages.
            foreach (CustomObjectPage page in _customObjectPages.Values)
            {
                SavePageElement(xmlDoc, documentElement, customObjectPageListElement, page);
            }

            // Save master pages.
            foreach (MasterPage page in _masterPages.Values)
            {
                SavePageElement(xmlDoc, documentElement, masterPageListElement, page);
            }

            // Save attached master pages.
            foreach (MasterPage page in _attachedMasterPages.Values)
            {
                SavePageElement(xmlDoc, documentElement, attachedMasterPageListElement, page);
            }

            // Finally, save stream
            SaveStreamListElement(xmlDoc, documentElement);
        }

        #endregion

        #region IObjectContainer

        public ReadOnlyCollection<IWidget> WidgetList
        {
            get
            {
                return new ReadOnlyCollection<IWidget>(_widgets.Values.ToList<IWidget>());
            }
        }

        public ReadOnlyCollection<IMaster> MasterList
        {
            get
            {
                return new ReadOnlyCollection<IMaster>(_masters.Values.ToList<IMaster>());
            }
        }

        public ReadOnlyCollection<IGroup> GroupList
        {
            get
            {
                return new ReadOnlyCollection<IGroup>(_groups.Values.ToList<IGroup>());
            }
        }

        public ReadOnlyCollection<IStandardPage> StandardPageList
        {
            get
            {
                return new ReadOnlyCollection<IStandardPage>(_standardPages.Values.ToList<IStandardPage>());
            }
        }

        public ReadOnlyCollection<ICustomObjectPage> CustomObjectPageList
        {
            get
            {
                return new ReadOnlyCollection<ICustomObjectPage>(_customObjectPages.Values.ToList<ICustomObjectPage>());
            }
        }

        public ReadOnlyCollection<IMasterPage> MasterPageList
        {
            get
            {
                return new ReadOnlyCollection<IMasterPage>(_masterPages.Values.ToList<IMasterPage>());
            }
        }

        #endregion

        #region ISerializeReader

        public void ReadAllFromStream()
        {
            if (_documentElement != null)
            {
                LoadDataFromXml(_documentElement);
            }
        }

        public ReadOnlyCollection<Guid> PeekWidgetGuidList()
        {
            List<Guid> widgetGuidList = new List<Guid>();

            if (_documentElement != null)
            {
                XmlElement widgetListElement = _documentElement[WIDGET_LIST_TAGNAME];
                if (widgetListElement != null)
                {
                    foreach (XmlElement childElement in widgetListElement.ChildNodes)
                    {
                        Guid guid = Guid.Empty;
                        if (LoadElementGuidAttribute(childElement, ref guid))
                        {
                            widgetGuidList.Add(guid);
                        }
                    }
                }
            }

            return new ReadOnlyCollection<Guid>(widgetGuidList);
        }

        public ReadOnlyCollection<WidgetType> PeekWidgetTypeList()
        {
            List<WidgetType> widgetTypeList = new List<WidgetType>();

            if (_documentElement != null)
            {
                XmlElement widgetListElement = _documentElement[WIDGET_LIST_TAGNAME];
                if (widgetListElement != null)
                {
                    foreach (XmlElement childElement in widgetListElement.ChildNodes)
                    {
                        WidgetType type = WidgetFactory.NameToWidgetType(childElement.Name);
                        if (type != WidgetType.None)
                        {
                            widgetTypeList.Add(type);
                        }
                    }
                }
            }

            return new ReadOnlyCollection<WidgetType>(widgetTypeList);
        }

        public ReadOnlyCollection<Guid> PeekMasterGuidList()
        {
            List<Guid> masterGuidList = new List<Guid>();

            if (_documentElement != null)
            {
                XmlElement masterListElement = _documentElement[MASTER_LIST_TAGNAME];
                if (masterListElement != null)
                {
                    foreach (XmlElement childElement in masterListElement.ChildNodes)
                    {
                        Guid guid = Guid.Empty;
                        if (LoadElementGuidAttribute(childElement, ref guid))
                        {
                            masterGuidList.Add(guid);
                        }
                    }
                }
            }

            return new ReadOnlyCollection<Guid>(masterGuidList);
        }

        public bool ContainsMaster
        {
            get
            {
                if (_documentElement != null)
                {
                    XmlElement masterListElement = _documentElement[MASTER_LIST_TAGNAME];
                    if (masterListElement != null && masterListElement.ChildNodes.Count > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

        #region ISerializeWriter

        public void AddPage(IDocumentPage page)
        {
            if (page is ICustomObjectPage)
            {
                AddCustomObjectPage(page as CustomObjectPage);
            }
            else if (page is IStandardPage)
            {
                AddStandardPage(page as StandardPage);
            }
            else if (page is IMasterPage)
            {
                AddMasterPage(page as MasterPage);
            }
        }

        public void AddWidget(IWidget widget)
        {
            Widget widgetToAdd = widget as Widget;
            if (widgetToAdd == null)
            {
                throw new ArgumentNullException("widget");
            }

            if (!_widgets.ContainsKey(widgetToAdd.Guid))
            {
                _widgets.Add(widgetToAdd.Guid, widgetToAdd);

                // Also add parent group but don't add other children in parent group.
                if (widgetToAdd.ParentGroup != null)
                {
                    AddGroupInternal(widgetToAdd.ParentGroup as Group, false);
                }
            }
        }

        public void AddMaster(IMaster master)
        {
            Master masterToAdd = master as Master;
            if (masterToAdd == null)
            {
                throw new ArgumentNullException("master");
            }

            // Also add masters master pages.
            if (masterToAdd.MasterPage == null)
            {
                throw new CannotFindMasterPageException("Cannot find the corresponding master page.");
            }

            AddMasterInternal(masterToAdd);

            // Add associated master page.
            AddAttachedMasterPage(masterToAdd.MasterPage);
        }

        public void AddGroup(IGroup group)
        {
            Group groupToAdd = group as Group;
            if(groupToAdd == null)
            {
                throw new ArgumentNullException("group");
            }

            // Also add all its children.
            AddGroupInternal(groupToAdd, true);
        }

        public void AddStandardPage(IStandardPage page)
        {
            StandardPage pageToAdd = page as StandardPage;
            if (pageToAdd == null)
            {
                throw new ArgumentNullException("page");
            }

            if(!_standardPages.ContainsKey(pageToAdd.Guid))
            {
                _standardPages.Add(pageToAdd.Guid, pageToAdd);

                bool isOpened = pageToAdd.IsOpened;

                // Open the page to get masters in this page
                if (!isOpened)
                {
                    pageToAdd.Open();
                }

                // Also add masters master pages. For now, only StandardPage has masters.
                foreach(Master master in pageToAdd.Masters)
                {
                    if (master == null || master.MasterPage == null)
                    {
                        throw new CannotFindMasterPageException("Cannot find the corresponding master page.");
                    }

                    AddAttachedMasterPage(master.MasterPage);
                }

                if(!isOpened)
                {
                    pageToAdd.Close();
                }
            }
        }

        public void AddCustomObjectPage(ICustomObjectPage page)
        {
            CustomObjectPage pageToAdd = page as CustomObjectPage;
            if (pageToAdd == null)
            {
                throw new ArgumentNullException("page");
            }

            if (!_customObjectPages.ContainsKey(pageToAdd.Guid))
            {
                _customObjectPages.Add(pageToAdd.Guid, pageToAdd);
            }
        }

        public void AddMasterPage(IMasterPage page)
        {
            MasterPage pageToAdd = page as MasterPage;
            if (pageToAdd == null)
            {
                throw new ArgumentNullException("page");
            }

            if (!_masterPages.ContainsKey(pageToAdd.Guid))
            {
                _masterPages.Add(pageToAdd.Guid, pageToAdd);
            }
        }

        public Stream WriteToStream()
        {
            MemoryStream deflateCompressedStream = new MemoryStream();

            using (MemoryStream outStream = new MemoryStream())
            {
                Save(outStream);

                using (DeflateStream deflateStream = new DeflateStream(deflateCompressedStream, CompressionMode.Compress, true))
                {
                    outStream.Position = 0;
                    outStream.CopyTo(deflateStream);
                }
            }

            return deflateCompressedStream;
        }

        #endregion

        #region Internal Methods

        internal void Clear()
        {
            _documentElement = null;
            _workingDirectoryGuid = Guid.Empty;
            _currentViewGuid = Guid.Empty;

            _widgets.Clear();
            _masters.Clear();
            _groups.Clear();

            _standardPages.Clear();
            _customObjectPages.Clear();
            _masterPages.Clear();

            _attachedMasterPages.Clear();

            _widgetBaseStyleXmlElements.Clear();
            _masterBaseStyleXmlElements.Clear();

            _attachedMasterPageXmlElements.Clear();
            _embeddedPageXmlElements.Clear();
            
            _thumbnailXmlElements.Clear();
            _iconXmlElements.Clear();

            _hashStreams.Clear();
        }

        internal void AddMasterInternal(Master master)
        {
            if (!_masters.ContainsKey(master.Guid))
            {
                _masters.Add(master.Guid, master);

                // Also add parent group but don't add other children in parent group.
                if (master.ParentGroup != null)
                {
                    AddGroupInternal(master.ParentGroup as Group, false);
                }
            }
        }

        internal void AddGroupInternal(Group group, bool addChildren)
        {
            if (!_groups.ContainsKey(group.Guid))
            {
                _groups.Add(group.Guid, group);
            }

            if (addChildren)
            {
                foreach (Widget widget in group.Widgets)
                {
                    AddWidget(widget);
                }

                foreach (Master master in group.Masters)
                {
                    AddMaster(master);
                }

                foreach (Group childGroup in group.Groups)
                {
                    AddGroupInternal(childGroup, true);
                }
            }
        }

        internal void AddPageInternal(Page page)
        {
            if (page is CustomObjectPage)
            {
                _customObjectPages.Add(page.Guid, page as CustomObjectPage);
            }
            else if (page is StandardPage)
            {
                _standardPages.Add(page.Guid, page as StandardPage);
            }
            else if (page is MasterPage)
            {
                _masterPages.Add(page.Guid, page as MasterPage);
            }
        }

        internal Guid WorkingDirectoryGuid
        {
            get { return _workingDirectoryGuid; }
        }

        internal Guid CurrentViewGuid
        {
            get { return _currentViewGuid; }
        }

        internal Dictionary<Guid, Widget> Widgets
        {
            get { return _widgets; }
        }

        internal Dictionary<Guid, Master> Masters
        {
            get { return _masters; }
        }

        internal Dictionary<Guid, Group> Groups
        {
            get { return _groups; }
        }

        internal Dictionary<Guid, StandardPage> StandardPages
        {
            get { return _standardPages; }
        }

        internal Dictionary<Guid, CustomObjectPage> CustomObjectPages
        {
            get { return _customObjectPages; }
        }

        internal Dictionary<Guid, MasterPage> MasterPages
        {
            get { return _masterPages; }
        }

        internal Dictionary<Guid, MasterPage> AttachedMasterPages
        {
            get { return _attachedMasterPages; }
        }

        internal WidgetStyle GetWidgetBaseStyle(Guid widgetGuid)
        {
            if(_widgetBaseStyleXmlElements.ContainsKey(widgetGuid))
            {
                XmlElement baseStyleElement = _widgetBaseStyleXmlElements[widgetGuid];
                WidgetStyle style = new WidgetStyle();
                style.LoadDataFromXml(baseStyleElement[style.TagName]);
                return style;
            }

            return null;
        }

        internal MasterStyle GetMasterBaseStyle(Guid masterGuid)
        {
            if (_masterBaseStyleXmlElements.ContainsKey(masterGuid))
            {
                XmlElement baseStyleElement = _masterBaseStyleXmlElements[masterGuid];
                MasterStyle style = new MasterStyle();
                style.LoadDataFromXml(baseStyleElement[style.TagName]);
                return style;
            }

            return null;
        }

        internal MemoryStream GetStream(string hash)
        {
            if (!String.IsNullOrEmpty(hash) && _hashStreams.ContainsKey(hash))
            {
                return _hashStreams[hash];                
            }

            return null;
        }

        internal void SetStream(string hash, MemoryStream stream)
        {
            if (String.IsNullOrEmpty(hash))
            {
                return;
            }

            _hashStreams[hash] = stream;
        }

        internal MemoryStream GetPageThumbnail(Guid pageGuid)
        {
            if (_thumbnailXmlElements.ContainsKey(pageGuid))
            {
                XmlElement streamElement = _thumbnailXmlElements[pageGuid];
                return new MemoryStream(Convert.FromBase64String(streamElement.InnerText));
            }

            return null;
        }

        internal MemoryStream GetPageIcon(Guid pageGuid)
        {
            if (_iconXmlElements.ContainsKey(pageGuid))
            {
                XmlElement streamElement = _iconXmlElements[pageGuid];
                return new MemoryStream(Convert.FromBase64String(streamElement.InnerText));
            }

            return null;
        }

        internal IObjectContainer GetObjectsInAttachedMasterPage(Guid masterPageGuid)
        {
            Serializer container = new Serializer();

            if (_attachedMasterPageXmlElements.ContainsKey(masterPageGuid))
            {
                XmlElement masterPageElement = _attachedMasterPageXmlElements[masterPageGuid];
                MasterPage page = new MasterPage(null, "");
                page.PageData.LoadDataFromXml(masterPageElement);
                page.ForceToSetPageOpened();
                LoadFullPageData(page);

                foreach(Widget widget in page.Widgets)
                {
                    container.AddWidget(widget);
                }
            }

            return container;
        }

        internal IObjectContainer GetObjectsInAttachedMasterPageCurrentView(Guid masterPageGuid)
        {
            Serializer container = new Serializer();

            if (_attachedMasterPageXmlElements.ContainsKey(masterPageGuid))
            {
                XmlElement masterPageElement = _attachedMasterPageXmlElements[masterPageGuid];
                MasterPage page = new MasterPage(null, "");
                page.PageData.LoadDataFromXml(masterPageElement);
                page.ForceToSetPageOpened();
                LoadFullPageData(page);
                
                PageView pageView = page.PageViews[_currentViewGuid] as PageView;
                if (pageView != null)
                {
                    foreach (Widget widget in pageView.Widgets)
                    {
                        container.AddWidget(widget);
                    }                                       
                }
            }

            return container;
        }

        #endregion

        #region Private Methods
                
        private void AddAttachedMasterPage(IMasterPage page)
        {
            MasterPage pageToAdd = page as MasterPage;
            if (pageToAdd == null)
            {
                throw new ArgumentNullException("page");
            }

            if (!_attachedMasterPages.ContainsKey(pageToAdd.Guid))
            {
                _attachedMasterPages.Add(pageToAdd.Guid, pageToAdd);
            }
        }

        private void LoadFullPageData(Page page)
        {
            foreach (Widget widget in page.Widgets)
            {
                if (widget is StreamWidget)
                {
                    StreamWidget streamWidget = widget as StreamWidget;
                    if(_hashStreams.ContainsKey(streamWidget.Hash))
                    {
                        streamWidget.DataStream = GetStream(streamWidget.Hash);
                    }
                }
                else if (widget is PageEmbeddedWidget)
                {
                    if (widget is HamburgerMenu)
                    {
                        HamburgerMenu menu = widget as HamburgerMenu;
                        StreamWidget streamWidget = menu.MenuButton as StreamWidget;
                        if (_hashStreams.ContainsKey(streamWidget.Hash))
                        {
                            streamWidget.DataStream = GetStream(streamWidget.Hash);
                        }
                    }

                    PageEmbeddedWidget embeddedWidget = widget as PageEmbeddedWidget;
                    foreach (EmbeddedPage embeddedPage in embeddedWidget.EmbeddedPages)
                    {
                        if (_embeddedPageXmlElements.ContainsKey(embeddedPage.Guid))
                        {
                            // Embedded page doesn't have PageEmbeddedWidget.
                            embeddedPage.PageData.LoadDataFromXml(_embeddedPageXmlElements[embeddedPage.Guid]);
                            embeddedPage.ForceToSetPageOpened();
                            LoadFullPageData(embeddedPage);
                        }
                    }
                }
            }

            if (_thumbnailXmlElements.ContainsKey(page.Guid))
            {
                page.Thumbnail = GetPageThumbnail(page.Guid);
            }

            if (page is CustomObjectPage && _iconXmlElements.ContainsKey(page.Guid))
            {
                CustomObjectPage objPage = page as CustomObjectPage;
                objPage.Icon = GetPageIcon(page.Guid);
            }
        }

        private void SaveWidgetBaseStyle(XmlDocument xmlDoc, XmlElement documentElement, Widget widget)
        {
            // Save widget style value in current page view or created page view. In other words, 
            // the widget base style here is the widget look in current view or created view.
            if (widget.WidgetStyles.Count > 0)
            {
                WidgetStyle viewStyle = null;
                if (_currentViewGuid != Guid.Empty)
                {
                    // Use current view style value as base style.
                    viewStyle = widget.GetWidgetStyle(_currentViewGuid) as WidgetStyle;

                }
                else
                {
                    // If current view is empty, use the created view style value as base style.
                    viewStyle = widget.GetWidgetStyle(widget.CreatedViewGuid) as WidgetStyle;
                }

                if (viewStyle != null)
                {
                    WidgetStyle baseStyle = new WidgetStyle();
                    WidgetStyle.CopyWidgetStyle(viewStyle, baseStyle);

                    XmlElement baseStyleElement = xmlDoc.CreateElement(WIDGET_BASE_STYLE_TAGNAME);
                    documentElement[WIDGET_BASE_STYLE_LIST_TAGNAME].AppendChild(baseStyleElement);
                    SaveElementGuidAttribute(baseStyleElement, widget.Guid); // Element guid is master guid.
                    baseStyle.SaveDataToXml(xmlDoc, baseStyleElement);
                }
            }
        }

        private void SaveMasterBaseStyle(XmlDocument xmlDoc, XmlElement documentElement, Master master)
        {
            // Save master style value in current page view or created page view. In other works, 
            // the master base style here is the widget look in current view or created view.
            if (master.MasterStyles.Count > 0)
            {
                MasterStyle viewStyle = null;
                if (_currentViewGuid != Guid.Empty)
                {
                    // Use current view style value as base style.
                    viewStyle = master.GetMasterStyle(_currentViewGuid) as MasterStyle;

                }
                else
                {
                    // If current view is empty, use the created view style value as base style.
                    viewStyle = master.GetMasterStyle(master.CreatedViewGuid) as MasterStyle;
                }

                if (viewStyle != null)
                {
                    MasterStyle baseStyle = new MasterStyle();
                    MasterStyle.CopyMasterStyle(viewStyle, baseStyle);

                    XmlElement baseStyleElement = xmlDoc.CreateElement(MASTER_BASE_STYLE_TAGNAME);
                    documentElement[MASTER_BASE_STYLE_LIST_TAGNAME].AppendChild(baseStyleElement);
                    SaveElementGuidAttribute(baseStyleElement, master.Guid); // Element guid is master guid.
                    baseStyle.SaveDataToXml(xmlDoc, baseStyleElement);
                }
            }
        }

        private void SaveThumbnailElement(XmlDocument xmlDoc, XmlElement documentElement, Guid guid, MemoryStream stream)
        {
            if (stream == null)
            {
                return;
            }

            XmlElement streamElement = xmlDoc.CreateElement(THUMBNAIL_TAGNAME);
            documentElement[THUMBNAIL_LIST_TAGNAME].AppendChild(streamElement);

            SaveElementGuidAttribute(streamElement, guid);

            stream.Position = 0;
            streamElement.InnerText = Convert.ToBase64String(stream.ToArray());
            stream.Position = 0;
        }

        private void SaveIconElement(XmlDocument xmlDoc, XmlElement documentElement, Guid guid, MemoryStream stream)
        {
            if (stream == null)
            {
                return;
            }

            XmlElement streamElement = xmlDoc.CreateElement(ICON_TAGNAME);
            documentElement[ICON_LIST_TAGNAME].AppendChild(streamElement);

            SaveElementGuidAttribute(streamElement, guid);

            stream.Position = 0;
            streamElement.InnerText = Convert.ToBase64String(stream.ToArray());
            stream.Position = 0;
        }

        private void SaveEmbeddedPageElement(XmlDocument xmlDoc, XmlElement documentElement, EmbeddedPage page)
        {
            if (page.IsOpened)
            {
                page.PageData.SaveDataToXml(xmlDoc, documentElement[EMBEDDED_PAGE_LIST_TAGNAME]);

                foreach (Widget widget in page.Widgets)
                {
                    SaveWidgetBaseStyle(xmlDoc, documentElement, widget);

                    if (widget is StreamWidget)
                    {
                        StreamWidget streamWidget = widget as StreamWidget;
                        SetStream(streamWidget.Hash, streamWidget.DataStream as MemoryStream);
                    }
                }
            }
            else
            {
                // Page is not opened, load data from files.
                string pageDir = Path.Combine(page.WorkingDirectory.FullName);
                string pageXmlFileName = Path.Combine(pageDir, Document.PAGE_FILE_NAME);
                if (File.Exists(pageXmlFileName))
                {
                    XmlDocument pageXmlDoc = new XmlDocument();
                    pageXmlDoc.Load(pageXmlFileName);

                    XmlNode importNode = xmlDoc.ImportNode(pageXmlDoc.DocumentElement, true);
                    documentElement[EMBEDDED_PAGE_LIST_TAGNAME].AppendChild(importNode);

                    XmlElement pageElement = pageXmlDoc.DocumentElement;
                    foreach (XmlElement widgetElement in pageElement["Widgets"].ChildNodes)
                    {
                        if (string.CompareOrdinal(widgetElement.Name, "Image") == 0
                            || string.CompareOrdinal(widgetElement.Name, "Svg") == 0)
                        {
                            string hash = widgetElement["Hash"] != null ? widgetElement["Hash"].InnerText : "";
                            SetStream(hash, null);
                        }
                    }
                }
            }

            //Save thumbnail
            SaveThumbnailElement(xmlDoc, documentElement, page.Guid, page.Thumbnail as MemoryStream);
        }

        private void SaveEmbeddedPagesDirectory(XmlDocument xmlDoc, XmlElement documentElement, Page parentPage)
        {           
            foreach (DirectoryInfo childPageDir in parentPage.WorkingDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
            {
                Guid guid;
                if (Guid.TryParse(childPageDir.Name, out guid))
                {
                    Document document = parentPage.ParentDocument as Document;

                    if (document.AllPages.ContainsKey(guid))
                    {
                        string pageDir = Path.Combine(childPageDir.FullName);
                        string pageXmlFileName = Path.Combine(pageDir, Document.PAGE_FILE_NAME);
                        if (File.Exists(pageXmlFileName))
                        {
                            XmlDocument pageXmlDoc = new XmlDocument();
                            pageXmlDoc.Load(pageXmlFileName);

                            XmlNode importNode = xmlDoc.ImportNode(pageXmlDoc.DocumentElement, true);
                            documentElement[EMBEDDED_PAGE_LIST_TAGNAME].AppendChild(importNode);

                            XmlElement pageElement = pageXmlDoc.DocumentElement;
                            foreach (XmlElement widgetElement in pageElement["Widgets"].ChildNodes)
                            {
                                if (string.CompareOrdinal(widgetElement.Name, "Image") == 0
                                    || string.CompareOrdinal(widgetElement.Name, "Svg") == 0)
                                {
                                    string hash = widgetElement["Hash"] != null ? widgetElement["Hash"].InnerText : "";
                                    SetStream(hash, null);
                                }
                            }

                            // Save thumbnail
                            string thumbnailFileName = Path.Combine(pageDir, Document.PAGE_THUMBNAIL_FILE_NAME);
                            if (File.Exists(thumbnailFileName))
                            {
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    using (FileStream fileStream = new FileStream(thumbnailFileName, FileMode.Open, FileAccess.Read))
                                    {
                                        fileStream.CopyTo(stream);
                                        SaveThumbnailElement(xmlDoc, documentElement, guid, stream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SavePageElement(XmlDocument xmlDoc, XmlElement documentElement, XmlElement pageListElement, Page page)
        {
            // If page data is not cleared, save data in memory.
            if (!page.PageData.IsCleared)
            {
                page.PageData.SaveDataToXml(xmlDoc, pageListElement);

                foreach (Widget widget in page.Widgets)
                {
                    SaveWidgetBaseStyle(xmlDoc, documentElement, widget);

                    if (widget is StreamWidget)
                    {
                        StreamWidget streamWidget = widget as StreamWidget;
                        SetStream(streamWidget.Hash, streamWidget.DataStream as MemoryStream);
                    }
                    else if (widget is PageEmbeddedWidget)
                    {
                        if (widget is HamburgerMenu)
                        {
                            HamburgerMenu menu = widget as HamburgerMenu;
                            StreamWidget streamWidget = menu.MenuButton as StreamWidget;
                            SetStream(streamWidget.Hash, streamWidget.DataStream as MemoryStream);
                        }

                        PageEmbeddedWidget embeddedWidget = widget as PageEmbeddedWidget;

                        foreach (EmbeddedPage embeddedPage in embeddedWidget.EmbeddedPages)
                        {
                            SaveEmbeddedPageElement(xmlDoc, documentElement, embeddedPage);
                        }
                    }
                }

                foreach (Master master in page.Masters)
                {
                    SaveMasterBaseStyle(xmlDoc, documentElement, master);
                }
            }
            else
            {
                // Page data is cleared, load data from disk.
                string pageDir = Path.Combine(page.WorkingDirectory.FullName);
                string pageXmlFileName = Path.Combine(pageDir, Document.PAGE_FILE_NAME);
                if (File.Exists(pageXmlFileName))
                {
                    XmlDocument pageXmlDoc = new XmlDocument();
                    pageXmlDoc.Load(pageXmlFileName);

                    XmlNode importNode = xmlDoc.ImportNode(pageXmlDoc.DocumentElement, true);
                    pageListElement.AppendChild(importNode);

                    XmlElement pageElement = pageXmlDoc.DocumentElement;
                    foreach (XmlElement widgetElement in pageElement["Widgets"].ChildNodes)
                    {
                        if (string.CompareOrdinal(widgetElement.Name, "Image") == 0
                            || string.CompareOrdinal(widgetElement.Name, "Svg") == 0)
                        {
                            string hash = widgetElement["Hash"] != null ? widgetElement["Hash"].InnerText : "";
                            SetStream(hash, null);
                        }
                        else if (string.CompareOrdinal(widgetElement.Name, "HamburgerMenu") == 0)
                        {
                            // HamburgerMenu has a HamburgerMenuButton
                            XmlElement menuButtonElement = widgetElement["HamburgerMenuButton"];
                            string hash = menuButtonElement["Hash"] != null ? menuButtonElement["Hash"].InnerText : "";
                            SetStream(hash, null);
                        }
                    }

                    SaveEmbeddedPagesDirectory(xmlDoc, documentElement, page);
                }
            }

            //Save thumbnail
            SaveThumbnailElement(xmlDoc, documentElement, page.Guid, page.Thumbnail as MemoryStream);

            // Save icon if it is a custom object page.
            if (page is CustomObjectPage)
            {
                CustomObjectPage objectPage = page as CustomObjectPage;
                SaveIconElement(xmlDoc, documentElement, objectPage.Guid, objectPage.Icon as MemoryStream);
            }
        }

        private void SaveStreamListElement(XmlDocument xmlDoc, XmlElement documentElement)
        {
            XmlElement streamListElement = xmlDoc.CreateElement(STREAM_LIST_TAGNAME);
            documentElement.AppendChild(streamListElement);

            foreach(KeyValuePair<string, MemoryStream> pair in _hashStreams)
            {
                XmlElement streamElement = xmlDoc.CreateElement(STREAM_TAGNAME);
                documentElement[STREAM_LIST_TAGNAME].AppendChild(streamElement);

                SaveElementStringAttribute(streamElement, "Hash", pair.Key);

                if (pair.Value != null)
                {
                    pair.Value.Position = 0;
                    streamElement.InnerText = Convert.ToBase64String(pair.Value.ToArray());
                    pair.Value.Position = 0;
                }
                else if (_imagesDir != null)
                {
                    // Load from image file
                    FileInfo[] files = _imagesDir.GetFiles(pair.Key + "*");
                    if (files.Length > 0)
                    {
                        try
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                using (FileStream fileStream = new FileStream(files[0].FullName, FileMode.Open, FileAccess.Read))
                                {
                                    fileStream.CopyTo(stream);
                                    stream.Position = 0;
                                    streamElement.InnerText = Convert.ToBase64String(stream.ToArray());
                                    stream.Position = 0;
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            Debug.WriteLine(exp.Message);
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Fields

        // Current document working directory Guid, we have a new guid once we new or open a document.
        // This Guid likes a session ID, we can use it to tell if the serialized stream is from current 
        // document in current session. 
        // In current design, we treat the serialized stream as the one from the same document if the guid is the equal.
        private Guid _workingDirectoryGuid;

        private XmlElement _documentElement;  // For reader.
        private Guid _currentViewGuid;  // For writer.
        private DirectoryInfo _imagesDir; // For writer.

        private Dictionary<Guid, Widget> _widgets = new Dictionary<Guid, Widget>();
        private Dictionary<Guid, Master> _masters = new Dictionary<Guid, Master>();
        private Dictionary<Guid, Group> _groups = new Dictionary<Guid, Group>();

        private Dictionary<Guid, StandardPage> _standardPages = new Dictionary<Guid, StandardPage>();
        private Dictionary<Guid, CustomObjectPage> _customObjectPages = new Dictionary<Guid, CustomObjectPage>();
        private Dictionary<Guid, MasterPage> _masterPages = new Dictionary<Guid, MasterPage>();

        // Master pages which are added because of the masters.
        private Dictionary<Guid, MasterPage> _attachedMasterPages = new Dictionary<Guid, MasterPage>(); 

        // Only used in reader
        private Dictionary<Guid, XmlElement> _widgetBaseStyleXmlElements = new Dictionary<Guid, XmlElement>();
        private Dictionary<Guid, XmlElement> _masterBaseStyleXmlElements = new Dictionary<Guid, XmlElement>();

        private Dictionary<Guid, XmlElement> _attachedMasterPageXmlElements = new Dictionary<Guid, XmlElement>();
        private Dictionary<Guid, XmlElement> _embeddedPageXmlElements = new Dictionary<Guid, XmlElement>();

        private Dictionary<string, MemoryStream> _hashStreams = new Dictionary<string, MemoryStream>();
        private Dictionary<Guid, XmlElement> _thumbnailXmlElements = new Dictionary<Guid, XmlElement>();
        private Dictionary<Guid, XmlElement> _iconXmlElements = new Dictionary<Guid, XmlElement>();

        #endregion

        #region Constants

        public const string WIDGET_LIST_TAGNAME = @"WidgetList";
        public const string MASTER_LIST_TAGNAME = @"MasterList";
        public const string GROUP_LIST_TAGNAME = @"GroupList";

        public const string STANDARD_PAGE_LIST_TAGNAME = @"StandardPageList";
        public const string CUSTOM_OBJECT_PAGE_LIST_TAGNAME = @"CustomObjectPageList";
        public const string MASTER_PAGE_LIST_TAGNAME = @"MasterPageList";

        // Additional list
        public const string ATTACHED_MASTER_PAGE_LIST_TAGNAME = @"AttachedMasterPageList";

        public const string EMBEDDED_PAGE_LIST_TAGNAME = @"EmbeddedPageList";

        public const string WIDGET_BASE_STYLE_LIST_TAGNAME = @"WidgetBaseStyleList";
        public const string WIDGET_BASE_STYLE_TAGNAME = @"WidgetBaseStyle";

        public const string MASTER_BASE_STYLE_LIST_TAGNAME = @"MasterBaseStyleList";
        public const string MASTER_BASE_STYLE_TAGNAME = @"MasterBaseStyle";

        public const string STREAM_LIST_TAGNAME = @"StreamList";
        public const string STREAM_TAGNAME = @"Stream";

        public const string THUMBNAIL_LIST_TAGNAME = @"ThumbnailList";
        public const string THUMBNAIL_TAGNAME = @"Thumbnail";

        public const string ICON_LIST_TAGNAME = @"IconList";
        public const string ICON_TAGNAME = @"Icon";

        #endregion
    }
}
