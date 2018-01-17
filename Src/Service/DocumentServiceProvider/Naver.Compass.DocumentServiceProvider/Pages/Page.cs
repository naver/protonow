using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace Naver.Compass.Service.Document
{
    internal abstract class Page : XmlElementObject, IPage, ISerializableObject
    {
        internal Page(string tagName)
            : base(tagName)
        {

        }

        #region XmlDataFileObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            Guid guid = Guid.Empty;
            if (!LoadElementGuidAttribute(element, ref guid))
            {
                throw new ArgumentException("Cannot get page guid from page element.");
            }

            PageData.Guid = guid;

            string name = "";
            LoadStringFromChildElementInnerText("Name", element, ref name);
            PageData.Name = name;
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement pageElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(pageElement);

            SaveElementGuidAttribute(pageElement, PageData.Guid);
            SaveStringToChildElement("Name", PageData.Name, xmlDoc, pageElement);
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            lock(this)
            {
                // Page is disposed only then the document is closed, so we don't need to save data here, 
                // as the document is closing, document will dispose embadded pages as well.

                if (_thumbnail != null)
                {
                    _thumbnail.Dispose();
                    _thumbnail = null;
                }

                _isOpened = false;
            }
        }

        #endregion

        #region IPage

        public string MD5 { get; set; }

        public Guid Guid
        {
            get { return PageData.Guid; }
            set { PageData.Guid = value; }
        }

        public string Name
        {
            get { return PageData.Name; }
            set { PageData.Name = value; }
        }

        public int Zoom
        {
            get { return PageData.Zoom; }
            set { PageData.Zoom = value; }
        }

        public IAnnotation Annotation
        {
            get { return PageData.Annotation; }
        }

        public void Open()
        {
            lock (this)
            {
                if (!_isOpened)
                {
                    // Page must be added to a document before it can be opened.
                    Document document = ParentDocument as Document;
                    if (document == null)
                    {
                        throw new CannotOpenPageException("The document is null.");
                    }

                    // The working directory was created when page added to the document.
                    if(_workingDirectory == null)
                    {
                        throw new CannotOpenPageException("Cannot get page working directory.");
                    }

                    string xmlFileName = Path.Combine(_workingDirectory.FullName, Document.PAGE_FILE_NAME);
                    if (PageData.IsCleared && File.Exists(xmlFileName))
                    {
                        PageData.Load(xmlFileName);
                    }

                    SyncPageDataWithDocument(document);

                    // Widgets and masters are loaded to document here.
                    foreach (Widget widget in PageData.Widgets)
                    {
                        widget.OnAddToDocument();
                        widget.UpdateActions(); // Update action, remove invalid page link
                    }

                    foreach (Master master in PageData.Masters)
                    {
                        master.OnAddToDocument();
                    }

                    // Make sure page instance is set in the AllPages when the page is opened.
                    document.AllPages[Guid] = this; 

                    _isOpened = true;
                }
            }
        }

        public virtual void Close()
        {
            lock (this)
            {
                if (_isOpened)
                {
                    // Save data to file.
                    Save();

                    // Close parent page doesn't effect widget embadded page. Embedded page is a standalone page.

                    foreach(Master master  in PageData.Masters)
                    {
                        MasterPage masterPage = master.MasterPage as MasterPage;
                        if (masterPage != null)
                        {
                            masterPage.RemoveActiveConsumerPage(Guid);
                        }
                    }

                    _isOpened = false;
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

        public IWidget CreateWidget(WidgetType widgetType)
        {
            CheckOpen();

            Widget widget = WidgetFactory.CreateWidget(this, widgetType);
            AddWidget(widget);

            return widget;
        }

        public void DeleteWidget(Guid widgetGuid)
        {
            CheckOpen();

            Widget widget = PageData.Widgets[widgetGuid] as Widget;

            if (widget == null)
            {
                return;
            }

            DeleteWidgetInternal(widget);

            UpdateDeletedWidgetParentGroup(widget);
        }

        public virtual void AddWidget(IWidget widget)
        {
            Widget widgetObject = widget as Widget;
            if (widgetObject == null)
            {
                throw new ArgumentNullException("widget");
            }

            CheckOpen();
            
            if (!PageData.Widgets.Contains(widgetObject.Guid))
            {
                // Add widget in page 
                PageData.Widgets.Add(widgetObject.Guid, widgetObject);

                widgetObject.ParentPage = this;

                // Add the widget to its parent group if parent group is already in this page.
                if (PageData.Groups.Contains(widgetObject.ParentGroupGuid))
                {
                    Group parentGroup = PageData.Groups.Get(widgetObject.ParentGroupGuid);
                    parentGroup.AddWidget(widgetObject);
                }
                else
                {
                    // Set its parent group to null
                    widgetObject.ParentGroup = null;
                }

                widgetObject.OnAddToDocument();
            }
        }

        public IMaster CreateMaster(Guid masterPageGuid)
        {
            CheckOpen();

            MasterPage masterPage = null;
            if (ParentDocument != null && ParentDocument.MasterPages.Contains(masterPageGuid))
            {
                masterPage = ParentDocument.MasterPages[masterPageGuid] as MasterPage;
            }

            if(masterPage == null)
            {
                throw new CannotFindMasterPageException("Cannot find the corresponding master page!");
            }

            Master master = new Master(masterPage, this);
            master.IsLockedToMasterLocation = masterPage.IsLockedToMasterLocation;
            AddMaster(master);

            return master;
        }

        public void DeleteMaster(Guid masterGuid)
        {
            CheckOpen();

            Master master = PageData.Masters[masterGuid] as Master;

            if (master == null)
            {
                return;
            }

            DeleteMasterInternal(masterGuid, false);

            UpdateDeletedMasterParentGroup(master);
        }

        public virtual void AddMaster(IMaster master)
        {
            Master masterObject = master as Master;
            if (masterObject == null)
            {
                throw new ArgumentNullException("master");
            }

            CheckOpen();

            MasterPage masterPage = null;
            if (ParentDocument != null && ParentDocument.MasterPages.Contains(masterObject.MasterPageGuid))
            {
                masterPage = ParentDocument.MasterPages[masterObject.MasterPageGuid] as MasterPage;
            }

            if (masterPage == null)
            {
                throw new CannotFindMasterPageException("Cannot find the corresponding master page!");
            }

            if (!PageData.Masters.Contains(masterObject.Guid))
            {
                // Add master in page 
                PageData.Masters.Add(masterObject.Guid, masterObject);

                masterObject.ParentPage = this;

                // Add the master to its parent group if parent group is already in this page.
                if (PageData.Groups.Contains(masterObject.ParentGroupGuid))
                {
                    Group parentGroup = PageData.Groups.Get(masterObject.ParentGroupGuid);
                    parentGroup.AddMaster(masterObject);
                }
                else
                {
                    // Set its parent group to null
                    masterObject.ParentGroup = null;
                }

                masterObject.OnAddToDocument();
            }
        }

        public IObjectContainer BreakMaster(Guid masterGuid)
        {
            CheckOpen();

            Master master = PageData.Masters[masterGuid] as Master;

            if (master == null)
            {
                throw new ArgumentException("masterGuid");
            }
          
            Document document = ParentDocument as Document;

            MasterPage masterPage = document.MasterPages[master.MasterPageGuid] as MasterPage;
            if (masterPage == null)
            {
                throw new CannotFindMasterPageException("Cannot find the corresponding master page!");
            }

            masterPage.Open();

            Serializer container = new Serializer();

            Serializer writer = new Serializer(document.WorkingDirectoryGuid, Guid.Empty, document.WorkingImagesDirectory);
            writer.AddMasterPage(masterPage);

            Serializer reader = new Serializer(writer.WriteToStream());
            reader.ReadAllFromStream();

            MasterPage newMasterPage = reader.MasterPageList[0] as MasterPage;
            if (newMasterPage.Widgets.Count <= 0)
            {
                // If there is no any widgets, return an empty container.
                return container;
            }

            newMasterPage.UpdateAllGuids();

            try
            {
                foreach (Widget widget in newMasterPage.Widgets)
                {
                    AddWidget(widget);

                    container.AddWidget(widget);

                    // Save embedded page data.
                    if (widget is PageEmbeddedWidget)
                    {
                        PageEmbeddedWidget embeddedWidget = widget as PageEmbeddedWidget;
                        foreach (EmbeddedPage embeddedPage in embeddedWidget.EmbeddedPages)
                        {
                            // Save embedded page.
                            embeddedPage.Save();
                        }
                    }
                }

                foreach (Group group in newMasterPage.Groups)
                {
                    AddGroup(group);

                    container.AddGroup(group);
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);

                // Remove objects already added in this page.
                foreach (Widget widget in container.WidgetList)
                {
                    DeleteWidget(widget.Guid);
                }

                foreach (Group group in container.GroupList)
                {
                    DeleteGroup(group.Guid);
                }

                container.Clear();
            }

            // At this time, the widgets in the newMasterPage have the location in master page.
            // We have to adjust widgets location to master location if it is NOT locked to master location.
            // NOTE: We can only get the widget style value via view guid after it is added to the document.
            if (!master.IsLockedToMasterLocation)
            {
                foreach (PageView pageView in newMasterPage.PageViews)
                {
                    pageView.PageViewStyle.X = master.GetMasterStyle(pageView.Guid).X;
                    pageView.PageViewStyle.Y = master.GetMasterStyle(pageView.Guid).Y;
                }
            }

            // Delete master from this page
            DeleteMaster(masterGuid);

            // Place widgets on page view according to master page
            foreach (PageView pageView in PageData.PageViews)
            {
                PageView masterPageView = newMasterPage.PageViews[pageView.Guid] as PageView;
                foreach(Guid widgetGuid in masterPageView.WidgetGuidList)
                {
                    pageView.PlaceWidget(widgetGuid);
                }
            }

            return container;
        }
       
        public IGroup CreateGroup(List<Guid> guidList)
        {
            CheckOpen();

            if (guidList == null || guidList.Count < 2)
            {
                throw new ArgumentException("A group must contain two or more objects!");
            }

            Group group = new Group(this);
            group.AddChildren(guidList);
            PageData.Groups.Add(group.Guid, group);

            return group;
        }

        public void Ungroup(Guid groupGuid)
        {
            CheckOpen();

            Group group = PageData.Groups[groupGuid] as Group;
            if (group != null)
            {
                // We don't change anything in the removed group as it must contain child and parent object info,
                // this way, we can restore the removed group correctly.

                // Remove group from page.
                PageData.Groups.Remove(groupGuid);

                // Move child to new parent group.
                Group parentGroup = group.ParentGroup as Group;
                foreach (Group subGroup in group.Groups)
                {
                    if (parentGroup != null)
                    {
                        parentGroup.AddGroup(subGroup);
                    }
                    else
                    {
                        subGroup.ParentGroup = null;
                    }
                }

                foreach (Widget widget in group.Widgets)
                {
                    if (parentGroup != null)
                    {
                        parentGroup.AddWidget(widget);
                    }
                    else
                    {
                        widget.ParentGroup = null;
                    }
                }

                foreach (Master master in group.Masters)
                {
                    if (parentGroup != null)
                    {
                        parentGroup.AddMaster(master);
                    }
                    else
                    {
                        master.ParentGroup = null;
                    }
                }

                // The parent group of groupGuid will always have 2 or more child after ungroup groupGuid,
                // so just remove the groupGuid from its parent group.
                if (parentGroup != null)
                {
                    parentGroup.RemoveGroup(group);
                }
            }
        }

        public void DeleteGroup(Guid groupGuid)
        {
            CheckOpen();

            // Delete the group and its child from this page.
            Group group = PageData.Groups[groupGuid] as Group;

            if (group == null)
            {
                return;
            }

            DeleteGroupInternal(groupGuid);

            UpdateDeletedGroupParentGroup(group);
        }
        
        public void AddGroup(IGroup group)
        {
            CheckOpen();

            // Add the group and its child to this page
            Group groupToAdd = group as Group;

            if (groupToAdd != null && !PageData.Groups.Contains(groupToAdd.Guid))
            {
                PageData.Groups.Add(groupToAdd.Guid, groupToAdd);

                groupToAdd.ParentPage = this;

                // Add to its parent group if parent group is in page.
                if (PageData.Groups.Contains(groupToAdd.ParentGroupGuid))
                {
                    Group parentGroup = PageData.Groups.Get(groupToAdd.ParentGroupGuid);
                    parentGroup.AddGroup(groupToAdd);
                }
                else
                {
                    groupToAdd.ParentGroup = null;
                }

                foreach (Group childGroup in groupToAdd.Groups)
                {
                    AddGroup(childGroup);

                    Group oldParentGroup = childGroup.ParentGroup as Group;
                    if (oldParentGroup != null && oldParentGroup != groupToAdd)
                    {
                        oldParentGroup.RemoveGroup(childGroup);
                    }
                    childGroup.ParentGroup = groupToAdd;
                }

                foreach (Widget widget in groupToAdd.Widgets)
                {
                    AddWidget(widget);

                    Group oldParentGroup = widget.ParentGroup as Group;
                    if (oldParentGroup != null && oldParentGroup != groupToAdd)
                    {
                        oldParentGroup.RemoveWidget(widget);
                    }
                    widget.ParentGroup = groupToAdd;
                }

                foreach (Master master in groupToAdd.Masters)
                {
                    AddMaster(master);

                    Group oldParentGroup = master.ParentGroup as Group;
                    if (oldParentGroup != null && oldParentGroup != groupToAdd)
                    {
                        oldParentGroup.RemoveMaster(master);
                    }
                    master.ParentGroup = groupToAdd;
                }
            }
        }

        public IObjectContainer AddObjects(Stream stream)
        {
            CheckOpen();

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            Serializer container = new Serializer();

            Serializer reader = new Serializer(stream);
            reader.ReadAllFromStream();

            List<Widget> widgetList = reader.Widgets.Values.ToList<Widget>();
            List<Master> masterList = reader.Masters.Values.ToList<Master>();
            List<Group> groupList = reader.Groups.Values.ToList<Group>();

            Document document = ParentDocument as Document;
            if (document.WorkingDirectoryGuid != reader.WorkingDirectoryGuid)
            {
                Dictionary<Guid, IObjectContainer> newTargets = new Dictionary<Guid, IObjectContainer>();

                // Break away all masters
                foreach (Master master in masterList)
                {
                    IObjectContainer objects = reader.GetObjectsInAttachedMasterPageCurrentView(master.MasterPageGuid);
                    newTargets[master.Guid] = objects;

                    // Update Guids
                    Page.UpdateObjectGuids(objects);

                    Group parentGroup = null;
                    if (master.ParentGroupGuid != Guid.Empty)
                    {
                        parentGroup = groupList.FirstOrDefault<IGroup>(x => x.Guid == master.ParentGroupGuid) as Group;
                        if (parentGroup != null)
                        {
                            // Remove this master from its parent group.
                            parentGroup.RemoveMaster(master);
                        }
                    }

                    // Ajust broken away widgets location
                    double deltaX = 0;
                    double deltaY = 0;
                    IMasterStyle masterStyle = reader.GetMasterBaseStyle(master.Guid);
                    if (masterStyle != null)
                    {
                        deltaX = masterStyle.X;
                        deltaY = masterStyle.Y;
                    }
                    else
                    {
                        deltaX = master.MasterStyle.X;
                        deltaY = master.MasterStyle.Y;
                    }

                    foreach (Widget widget in objects.WidgetList)
                    {
                        if (!widgetList.Any<Widget>( x => x.Guid == widget.Guid))
                        {
                            widgetList.Add(widget);

                            if (widget.ParentGroup == null && parentGroup != null)
                            {
                                // Add widget created by breaking master away to parent group.
                                parentGroup.AddWidget(widget);
                            }

                            WidgetStyle newBaseStyle = reader.GetWidgetBaseStyle(widget.Guid);
                            if (newBaseStyle != null)
                            {
                                newBaseStyle.X += deltaX;
                                newBaseStyle.Y += deltaY;
                            }
                            else
                            {
                                widget.WidgetStyle.X += deltaX;
                                widget.WidgetStyle.Y += deltaY;
                            }
                        }
                    }

                    foreach (Group group in objects.GroupList)
                    {
                        if (!groupList.Any<Group>(x => x.Guid == group.Guid))
                        {
                            groupList.Add(group);
                            if (group.ParentGroup == null && parentGroup != null)
                            {
                                // Add group created by breaking master away to parent group.
                                parentGroup.AddGroup(group);
                            }
                        }
                    }
                }

                foreach (Widget widget in widgetList)
                {
                    // Update action which has the master as a target.
                    widget.UpdateActions(newTargets);
                }

                // Remove all masters
                masterList.Clear();
            }

            // Rebuild widget base style
            foreach (Widget widget in widgetList)
            {
                widget.RebuildStyleChain(reader.GetWidgetBaseStyle(widget.Guid));
            }

            // Rebuild widget base style
            foreach (Master master in masterList)
            {
                master.RebuildStyleChain(reader.GetMasterBaseStyle(master.Guid));
            }

            // Update objects guid
            UpdateObjectGuids(widgetList, masterList, groupList);

            try
            {
                foreach (Widget widget in widgetList)
                {
                    AddWidget(widget);

                    container.AddWidget(widget);

                    // Save embedded page data.
                    if (widget is PageEmbeddedWidget)
                    {
                        PageEmbeddedWidget embeddedWidget = widget as PageEmbeddedWidget;
                        foreach (EmbeddedPage embeddedPage in embeddedWidget.EmbeddedPages)
                        {
                            // Save embedded page.
                            embeddedPage.Save();
                        }
                    }
                }

                foreach (Master master in masterList)
                {
                    AddMaster(master);

                    container.AddMasterInternal(master);
                }

                foreach (Group group in groupList)
                {
                    AddGroup(group);

                    container.AddGroup(group);
                }

                // Update interactions in widgets when all widgets have been added to the document.
                foreach (Widget widget in PageData.Widgets)
                {
                    widget.UpdateActions();
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);

                // Remove objects already added in this page.
                foreach (Widget widget in container.WidgetList)
                {
                    DeleteWidget(widget.Guid);
                }

                foreach (Master master in container.MasterList)
                {
                    DeleteMaster(master.Guid);
                }

                foreach (Group group in container.GroupList)
                {
                    DeleteGroup(group.Guid);
                }

                container.Clear();
            }

            return container;
        }

        public IObjectContainer AddCustomObject(ICustomObject customObject)
        {
            if (customObject == null)
            {
                throw new ArgumentNullException("customObject");
            }

            CheckOpen();

            CustomObjectPage objectPage = customObject as CustomObjectPage;

            Serializer container = new Serializer();

            // Update parent library LastAccessTime
            objectPage.UpdateLastAccessTime();

            Serializer reader = new Serializer(objectPage.SerializeStream);
            reader.ReadAllFromStream();

            DocumentPage page = reader.CustomObjectPageList[0] as DocumentPage;
            if (page.Widgets.Count <= 0)
            {
                // If there is no any widgets in this custom object, return an empty container.
                return container;
            }

            foreach (Widget widget in page.Widgets)
            {
                widget.RebuildStyleChain(reader.GetWidgetBaseStyle(widget.Guid));
            }

            page.UpdateAllGuids();

            try
            {
                foreach (Widget widget in page.Widgets)
                {
                    AddWidget(widget);

                    container.AddWidget(widget);

                    // Save embedded page data.
                    if (widget is PageEmbeddedWidget)
                    {
                        PageEmbeddedWidget embeddedWidget = widget as PageEmbeddedWidget;
                        foreach (EmbeddedPage embeddedPage in embeddedWidget.EmbeddedPages)
                        {
                            // Save embedded page.
                            embeddedPage.Save();
                        }
                    }
                }

                foreach (Group group in page.Groups)
                {
                    AddGroup(group);

                    container.AddGroup(group);
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine(exp.Message);

                // Remove objects already added in this page.
                foreach (Widget widget in container.WidgetList)
                {
                    DeleteWidget(widget.Guid);
                }

                foreach (Group group in container.GroupList)
                {
                    DeleteGroup(group.Guid);
                }

                container.Clear();
            }

            return container;
        }

        public IObjectContainer AddMasterPageObject(Guid masterPageGuid, Guid viewGuid)
        {
            if (masterPageGuid == Guid.Empty)
            {
                throw new ArgumentException("masterPageGuid");
            }

            if (viewGuid == Guid.Empty)
            {
                throw new ArgumentException("viewGuid");
            }

            CheckOpen();
            
            Document document = ParentDocument as Document;

            MasterPage masterPage = document.MasterPages[masterPageGuid] as MasterPage;
            if(masterPage == null)
            {
                throw new CannotFindMasterPageException("Cannot find the corresponding master page!");
            }

            masterPage.Open();

            Serializer container = new Serializer();
            Serializer writer = new Serializer(document.WorkingDirectoryGuid, viewGuid, document.WorkingImagesDirectory);

            PageView pageView = masterPage.PageViews[viewGuid] as PageView;
            if (pageView != null)
            {
                foreach (Widget widget in pageView.Widgets)
                {
                    writer.AddWidget(widget);
                }

                foreach (Group group in pageView.Groups)
                {
                    writer.AddGroup(group);
                }
            }
            else
            {
                foreach (Widget widget in masterPage.Widgets)
                {
                    writer.AddWidget(widget);
                }

                foreach (Group group in masterPage.Groups)
                {
                    writer.AddGroup(group);
                }
            }

            return AddObjects(writer.WriteToStream());
        }

        public abstract IDocument ParentDocument { get; set; }

        public Stream Thumbnail
        {
            get 
            {
                if(_thumbnail != null)
                {
                    return _thumbnail.DataStream;
                }

                return null;
            }

            set 
            { 
                if(_thumbnail != null)
                {
                    _thumbnail.DataStream = value; 
                }
                else
                {
                    if (value != null)
                    {
                        _thumbnail = new StreamFileObject(null);
                        _thumbnail.DataStream = value;
                    }
                }
            }
        }

        public IWidgets Widgets
        {
            get { return PageData.Widgets; }
        }

        public IMasters Masters
        {
            get { return PageData.Masters; }
        }

        public IGroups Groups
        {
            get { return PageData.Groups; }
        }

        public IPageViews PageViews
        {
            get { return PageData.PageViews; }
        }

        /* This is very ugly!!!!
         * I readlly hate to add this method in IPage interface, but other Dev insist it.
         * [DOM] level should provide all datas in file, but it should not provide all needs by [UI],
         * if so, the methods in DOM interface will be fat.
         * 
         * At the beginning, we design protoNow to a mutili-level hierarchy:
         *       [UI]
         *       [VM]
         *       [M] 
         *       [DOM]
         * 
         * [DOM] should focus on file format, provide all data to [M] efficently. But [DOM] should not 
         * provide all data structure [VM] or [UI] need, this is done by [M], especially on-fly data. 
         * 
         * [UI] should bind tightly to [VM], then all [UI] functions should be done with [VM]. Changes in [DOM]
         * should affect [UI] and [VM] on few codes, or better on no codes. 
         * 
         * The data structure [VM] or [UI] needed should provided by [M], [M] is an adapter, 
         * it will provide the data structure that [VM] or [M] needs based on the data from [DOM]. 
         * 
         * In other words, changes in [DOM] should mainly affect [M]. 
         * 
         * I take some examples to describe this.
         * 1. Page
         * [PageVM]->[PageM]->[IPage].
         * [ObjectVM]->[ObjectM]
         * 
         *                   [ObjectVM]
         *                   /   |   \
         *                  /    |    \
         *         [GroupVM][MasterVM][ImageVM][ShapeVM]...
         * 
         * [PageVM] have a Collection<ObjectVM>, 
         * 
         * A [ObjectVMFactory] will load or create ObjectVM and provide to [PageVM] to add in Collection<ObjectVM>.
         * Then any other fuctions should operate on  Collection<ObjectVM> or ObjectVM.  
         * 
         * 2. WidgetManager panel.
         *                 
         *                  [ NodeVM ]
         *                 /    |    \
         *                /     |     \
         * [DocumentPageNodeVM] [EmbeddedPageNodeVM] [GroupNodeVM] [SimpleWidgetNodeVM] [PageEmbeddedWidgetNodeVM] [MasterNodeVM]..
         * 
         * Any operation in tree is based on NodeVM.
         * 
         * We introduce multi-level design, but don't use it in the right way. Level will isolate changes, we don't use it to decouple, 
         * [UI][VM][M][DOM] all bind with each other, this is not MVC, MVM, whatever.
         * */
        public IRegions WidgetsAndMasters
        {
            get { return PageData.WidgetsAndMasters; }
        }

        #endregion

        #region ISerializableObject

        public Guid OriginalGuid
        {
            get;
            set;
        }

        public void UpdateGuid()
        {
            OriginalGuid = PageData.Guid;
            PageData.Guid = Guid.NewGuid();
        }

        #endregion

        #region  Events

        /*
         * When a page is added to the document, the page data maybe haven't been loaded.
         * 1. Create page working directory.
         * 2. Create thumbnail stream file in page working directory.
         * 3. Add page to document AllPages collection.
         * 4. Try to broadcast add event to all widgets and masters in this page if it has widgets.
         * */
        internal virtual void OnAddToDocument()
        {
            // Here, the parent document must exist.
            Document document = ParentDocument as Document;
            if(document == null)
            {
                throw new CannotAddPageException("Failed to add page to document, the document is null.");
            }

            // Create page working directory.
            InitializeWorkingDirectory();

            if(_workingDirectory == null)
            {
                throw new CannotAddPageException("Failed to add page to document, the page working directory is null.");
            }

            // Create thumbnail
            if(_thumbnail == null)
            {
                _thumbnail = new StreamFileObject(Path.Combine(_workingDirectory.FullName, Document.PAGE_THUMBNAIL_FILE_NAME));
            }
            else
            {
                if (string.IsNullOrEmpty(_thumbnail.StreamFileName))
                {
                    _thumbnail.StreamFileName = Path.Combine(Path.Combine(_workingDirectory.FullName, Document.PAGE_THUMBNAIL_FILE_NAME));
                }
            }

            // Sync paga data if it is opend, if not, sync the data when opening the page.
            if(IsOpened)
            {
                SyncPageDataWithDocument(document);

                // Broadcast add event to all widgets and maste if the page is opned.
                foreach (Widget widget in PageData.Widgets)
                {
                    widget.OnAddToDocument();
                }

                foreach(Master master in PageData.Masters)
                {
                    master.OnAddToDocument();
                }
            }

            // Set the page to the AllPages.
            document.AllPages[Guid] = this;
        }

        internal abstract void OnDeleteFromDocument();

        internal void OnAddAdaptiveView(AdaptiveView view)
        {
            if (!PageData.PageViews.Contains(view.Guid))
            {
                PageView pageView = new PageView(this, view.Guid);
                PageData.PageViews.Add(pageView);

                foreach (Widget widget in PageData.Widgets)
                {
                    widget.OnAddAdaptiveView(view);
                }

                foreach (Master master in PageData.Masters)
                {
                    master.OnAddAdaptiveView(view);
                }

                // Place widgets and masters in parent view to the newly added view.
                IAdaptiveView parentView = view.ParentView;
                PageView parentPageView = PageData.PageViews[parentView.Guid] as PageView;
                if (parentPageView != null)
                {
                    foreach (Guid guid in parentPageView.WidgetGuidList)
                    {
                        pageView.PlaceWidget(guid);
                    }

                    foreach (Guid guid in parentPageView.MasterGuidList)
                    {
                        pageView.PlaceMaster(guid);
                    }
                }
            }
        }

        internal void OnDeleteAdaptiveView(AdaptiveView view)
        {
            OnDeleteAdaptiveView(view.Guid);
        }

        internal void OnDeleteAdaptiveView(Guid viewGuid)
        {
            PageView pageView = PageData.PageViews[viewGuid] as PageView;
            PageData.PageViews.Remove(pageView);

            foreach (Widget widget in PageData.Widgets)
            {
                widget.OnDeleteAdaptiveView(viewGuid);
            }

            foreach (Master master in PageData.Masters)
            {
                master.OnDeleteAdaptiveView(viewGuid);
            }
        }

        internal void OnChangeAdaptiveViewParent(AdaptiveView view, AdaptiveView oldParentView)
        {
            // Todo: add codes if we can change parent of adaptive views.
            return;
        }

        internal void OnDeletePageAnnotationField(string fieldName)
        {
            PageData.Annotation.TextValues.Remove(fieldName);
        }

        internal void OnDeleteMasterPage(MasterPage masterPage)
        {
            // Remove masters which master page doesn't exist.
            List<Guid> deleteMasterGuidList = new List<Guid>();
            foreach (Master master in PageData.Masters)
            {
                if (master.MasterPageGuid == master.MasterPageGuid)
                {
                    deleteMasterGuidList.Add(master.Guid);
                }
            }

            foreach (Guid guid in deleteMasterGuidList)
            {
                DeleteMasterInternal(guid, true);
            }
        }

        #endregion

        #region Internal Methods

        // Data in Page.xml file.
        internal abstract PageData PageData { get; }

        // Becareful what you are doing.
        internal void ForceToSetPageOpened()
        {
            lock (this)
            {
                _isOpened = true;
            }
        }

        internal DirectoryInfo WorkingDirectory
        {
            get { return _workingDirectory; }
        }

        internal virtual void Save()
        {
            // Save Page.xml
            PageData.Save(Path.Combine(_workingDirectory.FullName, Document.PAGE_FILE_NAME));
         
            // Save thumbnail.
            if (_thumbnail != null)
            {
                _thumbnail.SaveStreamToFile();
            }
        }

        // Update page guid, widgets guid, groups guid and target guid in action.
        internal void UpdateAllGuids()
        {
            // We can only update guids if it has not been added to a document.
            if(ParentDocument != null)
            {
                throw new Exception("Cannot update guids.");
            }

            // Update this page guid.
            UpdateGuid();

            // Update all widgets, masters and groups guids.
            List<Widget> widgetList = PageData.Widgets.ToList<Widget>();
            List<Master> masterList = PageData.Masters.ToList<Master>();
            List<Group> groupList = PageData.Groups.ToList<Group>();
            UpdateObjectGuids(widgetList, masterList, groupList);

            // Here the Widgets, Groups in PageData is still used old Guid. Update them with new Guid.
            PageData.Widgets.Clear();
            PageData.Masters.Clear();
            PageData.Groups.Clear();

            foreach(Widget widget in widgetList)
            {
                PageData.Widgets.Add(widget.Guid, widget);
            }

            foreach (Master master in masterList)
            {
                PageData.Masters.Add(master.Guid, master);
            }

            foreach(Group group in groupList)
            {
                PageData.Groups.Add(group.Guid, group);
            }

            // Update widgets and masters guid in page view.
            foreach(PageView pageView in PageData.PageViews)
            {
                List<Guid> newWidgetGuidList = new List<Guid>();
                foreach(Widget widget in PageData.Widgets)
                {
                    if (pageView.WidgetGuidList.Contains(widget.OriginalGuid))
                    {
                        newWidgetGuidList.Add(widget.Guid);
                    }
                }

                pageView.WidgetGuidList = newWidgetGuidList;

                List<Guid> newMasterGuidList = new List<Guid>();
                foreach (Master master in PageData.Masters)
                {
                    if (pageView.MasterGuidList.Contains(master.OriginalGuid))
                    {
                        newMasterGuidList.Add(master.Guid);
                    }
                }

                pageView.MasterGuidList = newMasterGuidList;
            }

            // Update interactions in widgets.
            foreach (Widget widget in PageData.Widgets)
            {
                widget.UpdateActions();
            }
        }

        internal static void UpdateObjectGuids(IObjectContainer container)
        {
            List<Widget> widgetList = new List<Widget>(container.WidgetList.OfType<Widget>());
            List<Master> masterList = new List<Master>(container.MasterList.OfType<Master>());
            List<Group> groupList = new List<Group>(container.GroupList.OfType<Group>());

            UpdateObjectGuids(widgetList, masterList, groupList);
        }

        internal static void UpdateObjectGuids(List<Widget> widgetList, List<Master> masterList, List<Group> groupList)
        {
            // Update all group guids.
            foreach (Group group in groupList)
            {
                group.UpdateGuid();
            }

            // Update parent group guids as all groups have been updated guids.
            foreach (Group group in groupList)
            {
                if (group.ParentGroupGuid != Guid.Empty)
                {
                    group.ParentGroup = groupList.FirstOrDefault<Group>(x => x.OriginalGuid == group.ParentGroupGuid);
                }
            }

            // Update widgets' guid and their parent group guid. 
            foreach (Widget widget in widgetList)
            {
                widget.UpdateAllGuids();

                if (widget.ParentGroupGuid != Guid.Empty)
                {
                    Group parentGroup = groupList.FirstOrDefault<Group>(x => x.OriginalGuid == widget.ParentGroupGuid);
                    if (parentGroup != null)
                    {
                        widget.ParentGroupGuid = parentGroup.Guid;
                    }
                    else
                    {
                        widget.ParentGroupGuid = Guid.Empty;
                    }
                }
            }

            // Update masters' guid and their parent group guid. 
            foreach (Master master in masterList)
            {
                master.UpdateGuid();

                if (master.ParentGroupGuid != Guid.Empty)
                {
                    Group parentGroup = groupList.FirstOrDefault<Group>(x => x.OriginalGuid == master.ParentGroupGuid);
                    if (parentGroup != null)
                    {
                        master.ParentGroupGuid = parentGroup.Guid;
                    }
                    else
                    {
                        master.ParentGroupGuid = Guid.Empty;
                    }
                }
            }

            // Update gourp children guid list.
            foreach (Group group in groupList)
            {
                List<Guid> newWidgetGuidList = new List<Guid>();
                foreach (Guid oldGuid in group.WidgetGuidList)
                {
                    Widget childWidget = widgetList.FirstOrDefault<Widget>(x => x.OriginalGuid == oldGuid);
                    if (childWidget != null)
                    {
                        newWidgetGuidList.Add(childWidget.Guid);

                        // Update parent object and parent Guid.
                        childWidget.ParentGroup = group;
                    }
                }
                group.WidgetGuidList = newWidgetGuidList;

                List<Guid> newMasterGuidList = new List<Guid>();
                foreach (Guid oldGuid in group.MasterGuidList)
                {
                    Master childMaster = masterList.FirstOrDefault<Master>(x => x.OriginalGuid == oldGuid);
                    if (childMaster != null)
                    {
                        newMasterGuidList.Add(childMaster.Guid);

                        // Update parent object and parent Guid.
                        childMaster.ParentGroup = group;
                    }
                }
                group.MasterGuidList = newMasterGuidList;

                List<Guid> newGroupGuidList = new List<Guid>();
                foreach (Guid oldGuid in group.GroupGuidList)
                {
                    Group childGroup = groupList.FirstOrDefault<Group>(x => x.OriginalGuid == oldGuid);
                    if (childGroup != null)
                    {
                        newGroupGuidList.Add(childGroup.Guid);
                        childGroup.ParentGroup = group;
                    }
                }
                group.GroupGuidList = newGroupGuidList;
            }
        }

        internal virtual void UpdateActions()
        {
            foreach (Widget widget in PageData.Widgets)
            {
                widget.UpdateActions();
            }
        }

        internal void DeleteMasterInternal(Guid masterGuid, bool isMasterPageDeleted)
        {
            Master master = PageData.Masters[masterGuid] as Master;
            
            // Unplace this widget from all page views
            foreach (PageView pageView in PageData.PageViews)
            {
                pageView.UnplaceMaster(masterGuid);
            }

            PageData.Masters.Remove(masterGuid);

            // Delete widget from document if the doucment is null.
            if (ParentDocument != null)
            {
                master.OnDeleteFromDocument(isMasterPageDeleted);
            }
        }

        internal void DeleteAllMasters()
        {
            List<Master> deleteMasterList = PageData.Masters.ToList<Master>();
            foreach (Master master in deleteMasterList)
            {
                DeleteMasterInternal(master.Guid, true);
            }
        }

        #endregion

        #region Protected Methods

        protected void InitializeBasePageView()
        {
            if (ParentDocument != null)
            {
                if (!PageData.PageViews.Contains(ParentDocument.AdaptiveViewSet.Base.Guid))
                {
                    // If there is no page view for current document base view,
                    // rebuild the page views.
                    PageData.PageViews.Clear();

                    // Create Base view.
                    PageView pageBaseView = new PageView(this, ParentDocument.AdaptiveViewSet.Base.Guid);
                    PageData.PageViews.Add(pageBaseView);

                    // Create child view.
                    foreach (AdaptiveView adaptiveView in ParentDocument.AdaptiveViewSet.AdaptiveViews)
                    {
                        if (!PageData.PageViews.Contains(adaptiveView.Guid))
                        {
                            PageView pageView = new PageView(this, adaptiveView.Guid);
                            PageData.PageViews.Add(pageView);
                        }
                    }

                    // Place all widgets in base view and all its child view.
                    foreach (Widget widget in PageData.Widgets)
                    {
                        // Make widget all created in base view.
                        widget.CreatedViewGuid = pageBaseView.Guid;

                        // Initialize embedded page as well.
                        IPageEmbeddedWidget embeddedWidget = widget as IPageEmbeddedWidget;
                        if (embeddedWidget != null)
                        {
                            foreach (EmbeddedPage page in embeddedWidget.EmbeddedPages)
                            {
                                page.InitializeBasePageView();
                            }
                        }

                        pageBaseView.PlaceWidgetInternal(widget, true);
                    }
                }
            }
        }

        protected abstract void InitializeWorkingDirectory();

        #endregion

        #region Private Methods

        private void CheckOpen()
        {
            if (!IsOpened)
            {
                throw new PageIsClosedException("Page is closed.");
            }
        }

        private void DeleteWidgetInternal(Widget widget)
        {
            // Unplace this widget from all page views
            foreach (PageView pageView in PageData.PageViews)
            {
                pageView.UnplaceWidget(widget.Guid);
            }

            // Delete widget from document if the doucment is null.
            if (ParentDocument != null)
            {
                widget.OnDeleteFromDocument(false);
            }

            PageData.Widgets.Remove(widget.Guid);
        }

        private void DeleteGroupInternal(Guid groupGuid)
        {
            Group group = PageData.Groups[groupGuid] as Group;
            PageData.Groups.Remove(group.Guid);

            // DeleteGroup() -> UpdateDeletedGroupParentGroup() will change collection
            // group.GroupGuidList or group.Groups.
            // Call group.Groups() once so group will hold deleted groups object. They can be added to page again.
            List<Guid> subGroupGuidList = group.Groups.Select<IGroup, Guid>(x => x.Guid).ToList<Guid>();
            foreach (Guid guid in subGroupGuidList)
            {
                DeleteGroupInternal(guid);
            }

            foreach (Widget widget in group.Widgets)
            {
                DeleteWidgetInternal(widget);
            }

            foreach (Master master in group.Masters)
            {
                DeleteMasterInternal(master.Guid, false);
            }
        }

        private void UpdateDeletedWidgetParentGroup(Widget deletedWidget)
        {
            Group parentGroup = deletedWidget.ParentGroup as Group;
            if (parentGroup != null)
            {
                // If there is only one subgroup in this group and there is no widget in this group, ungroup this group.
                // Or if there is only one widget in this group and there is no subgroup in this group, ungroup this group.
                if ((parentGroup.Widgets.Count + parentGroup.Groups.Count + parentGroup.Masters.Count) <= 2)
                {
                    DeleteParentGroup(parentGroup, deletedWidget.Guid);
                }
                else
                {
                    parentGroup.RemoveWidget(deletedWidget);
                }
            }
        }

        private void UpdateDeletedMasterParentGroup(Master deletedMaster)
        {
            Group parentGroup = deletedMaster.ParentGroup as Group;
            if (parentGroup != null)
            {
                // If there is only one subgroup in this group and there is no widget in this group, ungroup this group.
                // Or if there is only one widget in this group and there is no subgroup in this group, ungroup this group.
                if ((parentGroup.Widgets.Count + parentGroup.Groups.Count + parentGroup.Masters.Count) <= 2)
                {
                    DeleteParentGroup(parentGroup, deletedMaster.Guid);
                }
                else
                {
                    parentGroup.RemoveMaster(deletedMaster);
                }
            }
        }

        private void UpdateDeletedGroupParentGroup(Group deletedGroup)
        {
            Group parentGroup = deletedGroup.ParentGroup as Group;
            if (parentGroup != null)
            {
                // If there is only one subgroup in this group and there is no widget in this group, ungroup this group.
                // Or if there is only one widget in this group and there is no subgroup in this group, ungroup this group.
                if ((parentGroup.Widgets.Count + parentGroup.Groups.Count + parentGroup.Masters.Count) <= 2)
                {
                    DeleteParentGroup(parentGroup, deletedGroup.Guid);
                }
                else
                {
                    parentGroup.RemoveGroup(deletedGroup);
                }
            }
        }

        // Delete parent group which only contains one child after a child is deleted.
        private void DeleteParentGroup(Group parentGroup, Guid deletedChildGuid)
        {
            // We don't change anything in the removed parentGroup as it must contain child and parent object info,
            // this way, we can restore the removed parentGroup correctly.

            // Remove the parent group from page as it will only has one child.
            PageData.Groups.Remove(parentGroup.Guid);

            // Remove parent group from grandgroup if it already in page.
            Group grandGroup = parentGroup.ParentGroup as Group;
            if (grandGroup != null && PageData.Groups.Contains(grandGroup.Guid))
            {
                grandGroup.RemoveGroup(parentGroup);
            }

            // Move child to grand group
            foreach (Group group in parentGroup.Groups)
            {
                // Don't move the deleted child.
                if (group.Guid != deletedChildGuid)
                {
                    if (grandGroup != null && PageData.Groups.Contains(grandGroup.Guid))
                    {
                        grandGroup.AddGroup(group);
                    }
                    else
                    {
                        group.ParentGroup = null;
                    }
                }
            }

            foreach (Widget widget in parentGroup.Widgets)
            {
                // Don't move the deleted child.
                if (widget.Guid != deletedChildGuid)
                {
                    if (grandGroup != null && PageData.Groups.Contains(grandGroup.Guid))
                    {
                        grandGroup.AddWidget(widget);
                    }
                    else
                    {
                        widget.ParentGroup = null;
                    }
                }
            }

            foreach (Master master in parentGroup.Masters)
            {
                // Don't move the deleted child.
                if (master.Guid != deletedChildGuid)
                {
                    if (grandGroup != null && PageData.Groups.Contains(grandGroup.Guid))
                    {
                        grandGroup.AddMaster(master);
                    }
                    else
                    {
                        master.ParentGroup = null;
                    }
                }
            }
        }

        /*
         * Synchronize page data with the document:
         * 1. Match page view to adaptive view in document.
         * 2. Match page annotation field in document.
         * 3. Remove masters which master page doesn't exist.
         * */
        private void SyncPageDataWithDocument(Document document)
        {
            // Remove invalid adaptive view data
            List<Guid> deleteViewGuidList = new List<Guid>();
            foreach (PageView view in PageData.PageViews)
            {
                if (view.Guid != document.AdaptiveViewSet.Base.Guid
                    && !document.AdaptiveViewSet.AdaptiveViews.Contains(view.Guid))
                {
                    deleteViewGuidList.Add(view.Guid);
                }
            }

            foreach (Guid guid in deleteViewGuidList)
            {
                OnDeleteAdaptiveView(guid);
            }

            // Initialize page view for document base view.
            InitializeBasePageView();

            // Add new adaptive view data.
            List<AdaptiveView> addViewGuidList = new List<AdaptiveView>();
            foreach (AdaptiveView view in document.AdaptiveViewSet.AdaptiveViews)
            {
                if (!PageData.PageViews.Contains(view.Guid))
                {
                    OnAddAdaptiveView(view);
                }
            }

            // Remove invalid page Annotation data.
            if (!PageData.Annotation.IsEmpty)
            {
                List<string> deleteFieldList = new List<string>();
                foreach (string field in PageData.Annotation.TextValues.Keys)
                {
                    if (!document.PageAnnotationFieldSet.AnnotationFields.Contains(field))
                    {
                        deleteFieldList.Add(field);
                    }
                }

                foreach (string field in deleteFieldList)
                {
                    PageData.Annotation.TextValues.Remove(field);
                }
            }

            // Remove masters which master page doesn't exist.
            List<Guid> deleteMasterGuidList = new List<Guid>();
            foreach (Master master in PageData.Masters)
            {
                if (master != null && !document.MasterPages.Contains(master.MasterPageGuid))
                {
                    deleteMasterGuidList.Add(master.Guid);
                }
            }

            foreach(Guid guid in deleteMasterGuidList)
            {
                DeleteMasterInternal(guid, true);
            }
        }

        #endregion

        #region Protected Fields

        protected DirectoryInfo _workingDirectory;

        protected StreamFileObject _thumbnail;

        protected bool _isOpened = false;  // The page is closed when it is created.

        #endregion
    }
}
