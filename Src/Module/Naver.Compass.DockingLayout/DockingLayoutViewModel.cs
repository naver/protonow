using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Xceed.Wpf.AvalonDock.Themes;
using Naver.Compass.InfoStructure;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using System.Windows.Controls;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using Naver.Compass.Module;
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using System.Windows;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Service;
using System.Diagnostics;

namespace DockingLayout
{
    public enum Layout_config
    {
        Default,
        Custom
    }
    class DockingLayoutViewModel:ViewModelBase
    {       
        //private IEventAggregator eventAggregation;
        public DockingLayoutViewModel(DockingManager dockManager)
        {
            if (IsInDesignMode)
                return;
            dockingManager = dockManager;


            //Subscribe the Page and Layout Event message
            _ListEventAggregator.GetEvent<OpenNormalPageEvent>().Subscribe(OpenNormalPageEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<OpenMasterPageEvent>().Subscribe(OpenMasterPageEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<OpenWidgetPageEvent>().Subscribe(OpenWidgetPageEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<ClosePageEvent>().Subscribe(ClosePageEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<ChangeLayoutEvent>().Subscribe(ChangeLayoutEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<RenamePageEvent>().Subscribe(RenamePageEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<OpenPanesEvent>().Subscribe(OpenPanesEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<UpdateLanguageEvent>().Subscribe(ChangeLangageHandler);
            _ListEventAggregator.GetEvent<AddNewPageEvent>().Subscribe(AddPageRequestExecute, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<DomLoadedEvent>().Subscribe(DomLoadedEventHandler, ThreadOption.UIThread);

            _ListEventAggregator.GetEvent<WdgMgrChangeSelectionEvent>().Subscribe(WdgMgrChangeSelectionHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<WdgMgrDeleteSelectionEvent>().Subscribe(WdgMgrDeleteSelectioHandler);
            _ListEventAggregator.GetEvent<WdgMgrEditSelectionEvent>().Subscribe(WdgMgrEditSelectioHandler);
            _ListEventAggregator.GetEvent<WdgMgrHideSelectionEvent>().Subscribe(WdgMgrHideSelectioHandler);
            _ListEventAggregator.GetEvent<WdgMgrZorderChangedEvent>().Subscribe(WdgMgrZChangeSelectioHandler);
            _ListEventAggregator.GetEvent<WdgMgrOrderwidgetEvent>().Subscribe(WdgMgrReZOrderSelectionHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<WdgMgrPlacewidgetEvent>().Subscribe(WdgMgrPlacewidgetSelectioHandler);
            _ListEventAggregator.GetEvent<WdgMgrOpenChildWidgetPage>().Subscribe(OpenChildWidgetPageEventHandler, ThreadOption.UIThread);
            this.AddNewPageCommand = new DelegateCommand<object>(AddNewPageExecute);

            var activePane = dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "Sitmap");

            if (GlobalData.IsStandardMode)
            {
                PanePageIcon.Hide();
                PanePageProp.Show();
            }
            else
            {
                PanePageProp.Hide();
                PanePageIcon.Show();
            }


            if (!IsFileExist(Layout_config.Default))
            {
                SaveLayout(Layout_config.Default);
                
                PanePageIcon.Hide();
            }            

            AddPanHidingHandler();

            AddEmptyPage();
        }

        #region private member
        private SubscriptionToken subscriptionToken;
        private DockingManager dockingManager;
        private IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }
        #endregion private member

        #region Other Message handler
        public void ChangeLayoutEventHandler(string strType)
        {
            switch (strType)
            {
                case "SaveCustom":
                    SaveLayout(Layout_config.Custom);
                    break;
                case "LoadDefault":
                    LoadLayout(Layout_config.Default);
                    break;
            }
        }
        public void OpenNormalPageEventHandler(Guid pageGID)
        {
            //remove empty page.
            EditPaneViewModelBase emptyPage = EditPages.FirstOrDefault(a => a is NoPageViewModel);
            if (emptyPage != null)
            {
                EditPages.Remove(emptyPage);
            }

            OpenNormalPage(pageGID, Guid.Empty,false,false);
        }

        public void OpenChildWidgetPageEventHandler(Guid guid)
        {
            ISelectionService _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            List<IWidgetPropertyData> SelectWidgets = _selectionService.GetSelectedWidgets();
            foreach (WidgetViewModBase wdg in SelectWidgets)
            {
                if (wdg.WidgetID.Equals(guid))
                {
                    if (wdg.Type == ObjectType.Master)
                    {
                        OpenMasterPageEventHandler(((EmbedWidgetViewModBase)wdg).EmbedePagetGUID);
                    }
                    else
                    {
                        IWidget widget = wdg.WidgetModel.WdgDom as IWidget;

                        if (widget != null)
                        {
                            OpenWidgetPage(widget, Guid.Empty, false, false);
                        }
                    }

                }
            }


        }

        public void OpenWidgetPageEventHandler(IWidget widget)
        {
            OpenWidgetPage(widget, Guid.Empty,false,false);      
        }
        public void ClosePageEventHandler(Guid pageGID)
        {        
   
            //Close all pages,20140228
            if (pageGID == Guid.Empty)
            {
                foreach (PageEditorViewModel pageItem in EditPages.Where(a=>a is PageEditorViewModel))
                {
                    pageItem.Close(Guid.Empty);
                }
                EditPages.Clear();
                AddEmptyPage();
                return;
            }
            //Close an exist page editor,20140220
            foreach (PageEditorViewModel pageItem in EditPages.Where(a=>a is PageEditorViewModel))
            {
                if (pageItem.ContentId == pageGID.ToString())
                {
                    
                    EditPages.Remove(pageItem);
                    pageItem.Close(pageGID);
                    if (EditPages.Count == 0)
                    {
                        AddEmptyPage();
                    }
                    return;
                }
            }
        }
        private void AddEmptyPage()
        {
            NoPageViewModel empty = new NoPageViewModel();
            empty.IsActive = true;
            EditPages.Add(empty);
        }
        public void RenamePageEventHandler(Guid pageGID)
        {            
            //change the exist page title,20140220
            foreach (EditPaneViewModelBase pageItem in EditPages)
            {                
                if (pageItem.ContentId == pageGID.ToString())
                {
                    PageEditorViewModel page= pageItem as PageEditorViewModel;
                    page.UpdateTitle();
                    //pageItem.IsActive = true;
                    break;
                }
            }                 
        }

        public void OpenPanesEventHandler(ActivityPane pane)
        {
            var activePane = dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == pane.Name);
            if (activePane == null || pane.bFromToolbar == false)
                return;

            if (pane.bOpen)
                activePane.Show();
            else
                activePane.Hide();
        }
        private void ChangeLangageHandler(string str)
        {
            LoadPaneTitle();
        }
        private void LoadPaneTitle()
        {
            if (PaneSitemap != null)
            {
                if (_document != null && _document.DocumentType == DocumentType.Library)
                {
                    PaneSitemap.Title = GlobalData.FindResource("Library_Sitemap_Title");
                }
                else
                {
                    PaneSitemap.Title = GlobalData.FindResource("Sitemap_Title");
                }
            }
            if (PaneWidgets != null)
            {
                PaneWidgets.Title = GlobalData.FindResource("widgets_Title");
            }
            if (PaneInteraction != null)
            {
                PaneInteraction.Title = GlobalData.FindResource("Interaction_Title");
            }
            if (PaneWidgetProp != null)
            {
                PaneWidgetProp.Title = GlobalData.FindResource("WidgetProp_Title");
            }
            if (PanePageProp != null)
            {
                PanePageProp.Title = GlobalData.FindResource("PageProp_Title");
            }
            if (PanePageIcon != null)
            {
                PanePageIcon.Title = GlobalData.FindResource("Icon_Title");
            }

            if (PaneWidgetManager != null)
            {
                PaneWidgetManager.Title = GlobalData.FindResource("ObjectListManager_Title");
            }
            if (PaneMaster != null)
            {
                PaneMaster.Title = GlobalData.FindResource("Master_Title");
            }
            
        }
        #endregion Message handler

        #region Widget Manager Message handler
        //Message from Widget Manager
        private void WdgMgrChangeSelectionHandler(object parameter)
        {
            WidgetSelectionInfoExtra info = (WidgetSelectionInfoExtra)parameter;

            //if (info.IsSwipePanel)
            //{
            //    return; 
            //}

            if(info.pageType==PageType.NormalPage)
            {
                //Open page when the target page is close now
                PageEditorViewModel page = OpenNormalPage(info.PageID, info.WidgetID,info.bSelected,info.IsGroup);
                if (page != null)
                {
                    //Select the target widget  and locate it correctly.
                    page.WdgMgrSetTargeComponent(info.WidgetID, info.bSelected, info.IsGroup);
                    page.SelectTargetComponent();
                }
            }
            else if (info.pageType == PageType.ToastPage || info.pageType == PageType.HamburgerPage )
            {
                //Open page when the target page is close now
                PageEditorViewModel page = OpenWidgetPage(info.BelongWidget, info.WidgetID,info.bSelected,info.IsGroup);
                if (page != null)
                {
                    //Select the target widget  and locate it correctly.
                    page.WdgMgrSetTargeComponent(info.WidgetID, info.bSelected, info.IsGroup);
                    page.SelectTargetComponent();
                }
            }
            else if (info.pageType == PageType.DynamicPanelPage )
            {
                if(info.IsSwipePanel==true && info.bSelected==false)
                {
                    return; 
                }
                else if(info.IsSwipePanel==true && info.bSelected==true)
                {
                    OpenSwipePage(info.BelongWidget,info.PageID, Guid.Empty,true,false);
                    return;
                }

                //Open page when the target page is close now
                PageEditorViewModel page = OpenSwipePage(info.BelongWidget,info.PageID, info.WidgetID,info.bSelected,info.IsGroup);
                if (page != null)
                {
                    //Select the target widget  and locate it correctly.
                    page.WdgMgrSetTargeComponent(info.WidgetID, info.bSelected, info.IsGroup);
                    page.SelectTargetComponent();
                }
            }
            
        }
        private void WdgMgrDeleteSelectioHandler(object parameter)
        {
            WDMgrWidgetDeleteInfo Info = parameter as WDMgrWidgetDeleteInfo;
            if (Info != null)
            {
                ISelectionService selsrv = ServiceLocator.Current.GetInstance<ISelectionService>();
                PageEditorViewModel page = selsrv.GetCurrentPage() as PageEditorViewModel;
                if (page != null && page.ActivePage.Guid == Info.PageID)
                {
                    page.WdgMgrDeleteSelection(Info.WidgetList);
                }
            }
           
        }
        private void WdgMgrReZOrderSelectionHandler(object parameter)
        {
            WDMgrZorderDragChangeInfo info = (WDMgrZorderDragChangeInfo)parameter;
            Guid pageID = info.PageID;
            ISelectionService selsrv = ServiceLocator.Current.GetInstance<ISelectionService>();
            PageEditorViewModel page = selsrv.GetCurrentPage() as PageEditorViewModel;
            if (page != null && page.ActivePage.Guid == pageID)
            {
                page.WdgMgrReZOrderSelection(info.zIndex,info.widgetID);
            }

        }
        private void WdgMgrZChangeSelectioHandler(object parameter)
        {
            WDMgrZorderChangeInfo Info = (WDMgrZorderChangeInfo)parameter;
            Guid pageID = Info.PageID;
            bool bIsForword=Info.bForward;
            ISelectionService selsrv = ServiceLocator.Current.GetInstance<ISelectionService>();
            PageEditorViewModel page = selsrv.GetCurrentPage() as PageEditorViewModel;
            if (page != null && page.ActivePage.Guid == pageID)
            {
                page.WdgMgrZOrderChangeSelection(bIsForword);
            }
        }
        private void WdgMgrPlacewidgetSelectioHandler(object parameter)
        {
            WDMgrPlaceStatusChangeInfo data = (WDMgrPlaceStatusChangeInfo)parameter;

            if (data == null || data.WidgetList.Count == 0)
            {
                return;
            }

            ISelectionService selsrv = ServiceLocator.Current.GetInstance<ISelectionService>();
            PageEditorViewModel page = selsrv.GetCurrentPage() as PageEditorViewModel;
            if (page != null)
            {
                if (data.bPlace)
                {
                    page.WdgMgrPlaceTargets(data.WidgetList);
                }
                else
                {
                    page.UnplaceWidgetsFromView(data.WidgetList);
                }
            }
        }


        private void WdgMgrEditSelectioHandler(object parameter)
        {
            Guid pageID = (Guid)parameter;
            ISelectionService selsrv = ServiceLocator.Current.GetInstance<ISelectionService>();
            PageEditorViewModel page = selsrv.GetCurrentPage() as PageEditorViewModel;
            if (page != null && page.ActivePage.Guid == pageID)
            {
                page.WdgMgrEditSelection();
            }
        }

        private void WdgMgrHideSelectioHandler(object parameter)
        {
            WDMgrHideStatusChangeInfo info = (WDMgrHideStatusChangeInfo)parameter;
            if(info==null)
            {
                return;
            }
            ISelectionService selsrv = ServiceLocator.Current.GetInstance<ISelectionService>();
            PageEditorViewModel page = selsrv.GetCurrentPage() as PageEditorViewModel;
            if (page != null && page.ActivePage.Guid == info.PageID)
            {
                if (info.HideType == WDMgrHideEventEnum.NormalWidget)
                {
                    page.WdgMgrHideSelection(!info.HideFlag);
                }
                else if(info.HideType == WDMgrHideEventEnum.SwipeViewPanel)
                {
                    page.WdgMgrHideAllWidgets(!info.HideFlag);
                    _ListEventAggregator.GetEvent<SwipePanelHidddenEvent>().Publish(parameter);
                }
                else if(info.HideType == WDMgrHideEventEnum.ChildGroup)
                {
                    //
                }
            }
        }
        #endregion Message handler

        #region Private funnctions
        //return value: bool, whether the target page has been open now
        private PageEditorViewModel OpenNormalPage(Guid pageGID, Guid SelWdgGID, bool bIsSelect, bool bIsGroup)
        {
            //Active an exist page editor,20140220
            foreach (EditPaneViewModelBase pageItem in EditPages)
            {
                if (pageItem.ContentId == pageGID.ToString())
                {
                    pageItem.IsActive = true; 
                    pageItem.IsNeedReturnFocus = true;
                    return pageItem as PageEditorViewModel;
                }
            }

            DeselectAllPages();

            //Create a new page editor,20140220

            PageEditorViewModel page = new PageEditorViewModel(pageGID);
            page.WdgMgrSetTargeComponent(SelWdgGID, bIsSelect, bIsGroup);
            page.Open();
            EditPages.Add(page);
            page.IsActive = true;
            page.Update();

            page.IsNeedReturnFocus = true;
            return null;

        }
        public void OpenMasterPageEventHandler(Guid pageGID)
        {
            //Active an exist page editor,20140220
            var exitPage = EditPages.OfType<MasterPageEditorViewModel>().FirstOrDefault(a => a.ContentId == pageGID.ToString());
            if (exitPage != null)
            {
                exitPage.IsActive = true;
                exitPage.IsNeedReturnFocus = true;
                return;
            }

            DeselectAllPages();
            //Create a new page editor,20140220

            var page = new MasterPageEditorViewModel(pageGID);
            page.Open();
            EditPages.Add(page);
            page.IsActive = true;
            page.Update();

            page.IsNeedReturnFocus = true;
        }
            
        private PageEditorViewModel OpenWidgetPage(IWidget widget, Guid SelWdgGID,bool bIsSelect,bool bIsgroup)
        {
            PageEditorViewModel newPage = null;
            ISelectionService _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            IPagePropertyData activePage = _selectionService.GetCurrentPage();
            
            Guid parentGID = widget.ParentPage.Guid;
            switch (widget.WidgetType)
            {
                case WidgetType.DynamicPanel:
                    {
                        //Active an exist page editor,20140220
                        foreach (var pageItem in EditPages.OfType<DynamicPageEditorViewModel>())
                        {
                            if (pageItem.ContentId == widget.Guid.ToString())
                            {
                                pageItem.IsActive = true;
                                return pageItem;
                            }
                        }

                        ////Create a new page editor,20140220
                        newPage = new DynamicPageEditorViewModel(widget);
                        newPage.WdgMgrSetTargeComponent(SelWdgGID, bIsSelect, bIsgroup);
                        break;
                    }
                case WidgetType.Toast:
                    {
                        foreach (var pageItem in EditPages.OfType<ToastPageEditorViewModel>())
                        {
                            if (pageItem.ContentId == widget.Guid.ToString())
                            {
                                pageItem.IsActive = true;
                                return pageItem;
                            }
                        }

                        newPage = new ToastPageEditorViewModel(widget);
                        newPage.WdgMgrSetTargeComponent(SelWdgGID, bIsSelect, bIsgroup);
                        break;
                    }
                case WidgetType.HamburgerMenu:
                    {
                        //Active an exist page editor,20140220
                        foreach (var pageItem in EditPages.OfType<HamburgerPageEditorViewModel>())
                        {
                            if (pageItem.ContentId == widget.Guid.ToString())
                            {
                                pageItem.IsActive = true;
                                return pageItem;
                            }
                        }
                        //Create a new page editor,20140220
                        newPage = new HamburgerPageEditorViewModel(widget);
                        newPage.WdgMgrSetTargeComponent(SelWdgGID, bIsSelect, bIsgroup);
                        break;
                    }

            }

            DeselectAllPages();

            //if child page is not set(value is 100%)
            //Set scale the same as parent page(only create child page, not active child page)
            if (newPage.EditorScale == 1)
            {
                newPage.EditorScale = activePage.EditorScale;
            }
            newPage.Open();
            EditPages.Add(newPage);
            newPage.IsActive = true;
            newPage.Update();
            return null;
        }
        private PageEditorViewModel OpenSwipePage(IWidget widget, Guid childPageGID, Guid SelWdgGID,bool bIsSelect,bool bIsGroup)
        {
            PageEditorViewModel newPage = null;
            ISelectionService _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            IPagePropertyData activePage = _selectionService.GetCurrentPage();

            Guid parentGID = widget.ParentPage.Guid;
            if (widget.WidgetType != WidgetType.DynamicPanel)
                return null;


            //Active an exist page editor,20140220
            DynamicPageEditorViewModel CurrentDynmicPanel = null;
            foreach (var pageItem in EditPages.OfType<DynamicPageEditorViewModel>())
            {
                if (pageItem.ContentId == widget.Guid.ToString())
                {
                    CurrentDynmicPanel = pageItem;
                    if(pageItem.ActivePage.Guid == childPageGID)                    
                    {
                        pageItem.IsActive = true;
                        return pageItem;
                    }
                    break;
                }                        
            }

            //Only Update the target panel
            if (CurrentDynmicPanel!=null)
            {
                //TODO:???????
                CurrentDynmicPanel.IsActive = true;
                foreach (DynamicChildNodViewModel item in CurrentDynmicPanel.DynamicChildren)
                {
                    if (item.GID == childPageGID)
                    {
                        CurrentDynmicPanel.WdgMgrSetTargeComponent(SelWdgGID, bIsSelect, bIsGroup);
                        item.IsChecked = true;
                        break;
                    }
                }
                return null;
            }

            DeselectAllPages();

            //Create a new page with target panel
            newPage = new DynamicPageEditorViewModel(widget, childPageGID);
            newPage.WdgMgrSetTargeComponent(SelWdgGID, bIsSelect, bIsGroup);

            //if child page is not set(value is 100%)
            //Set scale the same as parent page(only create child page, not active child page)
            if (newPage.EditorScale == 1)
            {
                newPage.EditorScale = activePage.EditorScale;
            }
            newPage.Open();
            EditPages.Add(newPage);
            newPage.IsActive = true;
            newPage.Update();
            return null;
        }

        private void DeselectAllPages()
        {
            foreach (EditPaneViewModelBase curItem in EditPages)
            {
                if (curItem.IsSelected == true)
                {
                    curItem.IsSelected = false;
                }
            }
        }

        #endregion Private funnctions

        #region layout function
        public void SaveLayout(Layout_config config)
        {
            try
            {
                if (!Directory.Exists(LayoutFilePath))
                {
                    Directory.CreateDirectory(LayoutFilePath);
                }
                string fileName;
                if (config == Layout_config.Default)
                {
                    fileName = LayoutFilePath + CommonDefine.DefaultConfigFile;
                }
                else
                {
                    fileName = LayoutFilePath + CommonDefine.CustomConfigFile;
                }
                var serializer = new XmlLayoutSerializer(dockingManager);
                using (var stream = new StreamWriter(fileName))
                    serializer.Serialize(stream);

            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }           
        }

        public void LoadLayout(Layout_config config)
        {
            var currentContentsList = dockingManager.Layout.Descendents().OfType<LayoutContent>().Where(c => c.ContentId != null).ToArray();
            try
            {
                
                if (!Directory.Exists(LayoutFilePath))
                {
                    Directory.CreateDirectory(LayoutFilePath);
                }
                string fileName;
                if (config == Layout_config.Default)
                {
                    fileName = LayoutFilePath + CommonDefine.DefaultConfigFile;
                }
                else
                {
                    fileName = LayoutFilePath + CommonDefine.CustomConfigFile;
                }
                if (File.Exists(fileName))
                {
                    var serializer = new XmlLayoutSerializer(dockingManager);
                    using (var stream = new StreamReader(fileName))
                        serializer.Deserialize(stream);
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                AddPanHidingHandler();
                UpdatePaneState();
                LoadPaneTitle();
            }
        }

        private void UpdatePaneState()
        {
            if(PaneSitemap!=null)
            {
                OpenPaneState(PaneSitemap);
            }
            if (PaneWidgets != null)
            {
                OpenPaneState(PaneWidgets);
            }
            if(PaneMaster !=null)
            {
                OpenPaneState(PaneMaster);
            }
            if (PaneInteraction != null)
            {
                OpenPaneState(PaneInteraction);
            }
            if (PaneWidgetProp != null)
            {
                OpenPaneState(PaneWidgetProp);
            }
            if (PanePageProp != null && _document != null && _document.DocumentType == DocumentType.Standard)
            {
                OpenPaneState(PanePageProp);
                PanePageIcon.Hide();
                //AdaptiveModel.GetInstance().IsPagePropOpen = PanePageProp.IsVisible;
                foreach (PageEditorViewModel item in EditPages.OfType<PageEditorViewModel>())
                {
                    if (item == null)
                        continue;
                    item.SetAdaptiveViewVisibility(PanePageProp.IsVisible);
                }
            }
            if (PanePageIcon != null && _document != null && _document.DocumentType == DocumentType.Library)
            {
                OpenPaneState(PanePageIcon);
                PanePageProp.Hide();
            }

            if (PaneWidgetManager != null)
            {
                OpenPaneState(PaneWidgetManager);
            }
        }
        private void OpenPaneState(LayoutAnchorable pane)
        {
            ActivityPane acPane = new ActivityPane();
            acPane.Name = pane.ContentId;
            acPane.bOpen = pane.IsVisible;
            _ListEventAggregator.GetEvent<OpenPanesEvent>().Publish(acPane);
        }

        private bool IsFileExist(Layout_config config)
        {
            string fileName;
            if (config == Layout_config.Default)
            {
                fileName = LayoutFilePath + CommonDefine.DefaultConfigFile;
            }
            else
            {
                fileName = LayoutFilePath + CommonDefine.CustomConfigFile;
            }
            if (File.Exists(fileName))
                return true;
            else
                return false;
        }

        private string LayoutFilePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Compass";
            }
        }

        void AddPanHidingHandler()
        {
            if(PaneSitemap!=null)
            {
                PaneSitemap.IsHiddenChanged += Panels_IsHiddenChanged;
            }
            if (PaneWidgets != null)
            {
                PaneWidgets.IsHiddenChanged += Panels_IsHiddenChanged;
            }
            if (PaneMaster != null)
            {
                PaneMaster.IsHiddenChanged += Panels_IsHiddenChanged;
            }
            if (PaneWidgetProp != null)
            {
                PaneWidgetProp.IsHiddenChanged += Panels_IsHiddenChanged;
            }
            if (PaneInteraction != null)
            {
                PaneInteraction.IsHiddenChanged += Panels_IsHiddenChanged;
            }
            if (PanePageProp != null)
            {
                PanePageProp.IsHiddenChanged += Panels_IsHiddenChanged;
            }
            if (PanePageIcon != null)
            {
                PanePageIcon.IsHiddenChanged += Panels_IsHiddenChanged;
            }

            if (PaneWidgetManager != null)
            {
                PaneWidgetManager.IsHiddenChanged += Panels_IsHiddenChanged;
            }
        }

        void Panels_IsHiddenChanged(object sender, EventArgs e)
        {
            LayoutAnchorable layoutAnchor = (LayoutAnchorable)sender;
            ActivityPane pane = new ActivityPane();
            pane.Name = layoutAnchor.ContentId;
            pane.bOpen = layoutAnchor.IsVisible;
            _ListEventAggregator.GetEvent<OpenPanesEvent>().Publish(pane);
        }

        #region Panes
        private LayoutAnchorable PaneSitemap
        {
            get
            {
                return dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == CommonDefine.PaneSitemap);
            }
        }
        private LayoutAnchorable PaneWidgets
        {
            get
            {
                return dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == CommonDefine.PaneWidgets);
            }
        }
        private LayoutAnchorable PaneInteraction
        {
            get
            {
                return dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == CommonDefine.PaneInteraction);
            }
        }
        private LayoutAnchorable PaneWidgetProp
        {
            get
            {
                return dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == CommonDefine.PaneWidgetProp);
            }
        }
        private LayoutAnchorable PanePageProp
        {
            get
            {
                return dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == CommonDefine.PanePageProp);
            }
        }
        private LayoutAnchorable PanePageIcon
        {
            get
            {
                return dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == CommonDefine.PanePageIcon);
            }
        }

        private LayoutAnchorable PaneWidgetManager
        {
            get
            {
                return dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == CommonDefine.PaneWidgetManager);
            }
        }

        private LayoutAnchorable PaneMaster
        {
            get
            {
                return dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == CommonDefine.PaneMaster);
            }
        }
        #endregion
        #endregion layout function

        #region Binding Property

        private Theme _dockThemes;
        public Theme DockThemes
        {
            get
            {
                if(_dockThemes==null)
                {
                    _dockThemes = new AeroTheme();

                }
                return _dockThemes;
            }


        }

        //use base class<EditPaneViewModleBase> is for support multi-type Editor except for page,20140216
        //ObservableCollection<PageEditorViewModel> _Pages = null;
        ObservableCollection<EditPaneViewModelBase> _Pages = null;
        public ObservableCollection<EditPaneViewModelBase> EditPages
        {
            get
            {
                if (_Pages == null)
                    _Pages = new ObservableCollection<EditPaneViewModelBase>();
                return _Pages;
            }
        }

        public Visibility LibraryVisibility
        {
            get
            {
                if (_document != null && _document.DocumentType == DocumentType.Library)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public Visibility DocumentVisibility
        {
            get
            {
                if (_document != null && _document.DocumentType == DocumentType.Library)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }
        #endregion Binding Property

        public DelegateCommand<object> AddNewPageCommand { get; private set; }
        private void AddNewPageExecute(object obj)
        {
            _ListEventAggregator.GetEvent<AddNewPageEvent>().Publish(Guid.Empty);
        }
        private void AddPageRequestExecute(Guid cmdParameter)
        {
            if (cmdParameter != Guid.Empty)
            {
                OpenNormalPageEventHandler(cmdParameter);
            }
        }
        private void DomLoadedEventHandler(FileOperationType loadType)
        {
            switch (loadType)
            {
                case FileOperationType.Create:
                case FileOperationType.Open:
                    LoadPaneTitle();
                    if (_document != null)
                    {
                        if (_document.DocumentType == DocumentType.Library)
                        {
                            PanePageIcon.Show();
                            PanePageProp.Hide();
                        }
                        else
                        {
                            PanePageIcon.Hide();
                            PanePageProp.Show();
                        }
                        FirePropertyChanged("LibraryVisibility");
                        FirePropertyChanged("DocumentVisibility");
                        UpdateWindowStyle();
                    }
                    break;
                case FileOperationType.Close:
                    
                    break;
            }
        }

        private void UpdateWindowStyle()
        {
            try
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc != null && doc.Document != null && doc.Document.DocumentType == DocumentType.Library)
                {
                    Application.Current.Resources["WindowTitleColor"] = CommonDefine.LibraryWindowBarColor;
                    Application.Current.Resources["WindowBorderColor"] = CommonDefine.LibraryWindowBorderColor;
                }
                else
                {
                    Application.Current.Resources["WindowTitleColor"] = CommonDefine.StandardWindowBarColor;
                    Application.Current.Resources["WindowBorderColor"] = CommonDefine.StandardWindowBorderColor;

                }
            }
            catch
            {

            }

        }
    }
}
