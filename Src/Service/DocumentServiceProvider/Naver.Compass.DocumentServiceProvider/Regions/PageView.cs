using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class PageView : Region, IPageView
    {
        #region Constructors

        internal PageView(Page parentPage, Guid viewGuid)
            : base(parentPage, "PageView")
        {
            // PageView must exist with its parent page.
            Debug.Assert(parentPage != null);

            _viewGuid = viewGuid;
            _guides = new Guides();
        }

        #endregion

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            // Do not save guid and name, so we don't call base.LoadDataFromXml() here.
            CheckTagName(element); 

            LoadGuidFromChildElementInnerText("AdaptiveViewGuid", element, ref _viewGuid);

            XmlElement widgetsElement = element["WidgetsGuid"];
            if (widgetsElement != null && widgetsElement.ChildNodes.Count > 0)
            {
                foreach (XmlElement childElement in widgetsElement.ChildNodes)
                {
                    try
                    {
                        Guid guid = new Guid(childElement.InnerText);
                        if (guid != Guid.Empty)
                        {
                            _widgetGuidList.Add(guid);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        continue;
                    }
                }
            }

            XmlElement mastersElement = element["MastersGuid"];
            if (mastersElement != null && mastersElement.ChildNodes.Count > 0)
            {
                foreach (XmlElement childElement in mastersElement.ChildNodes)
                {
                    try
                    {
                        Guid guid = new Guid(childElement.InnerText);
                        if (guid != Guid.Empty)
                        {
                            _masterGuidList.Add(guid);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        continue;
                    }
                }
            }

            // Load Guides
            XmlElement guidesElement = element["Guides"];
            if (guidesElement != null)
            {
                _guides.LoadDataFromXml(guidesElement);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement viewElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(viewElement);

            SaveStringToChildElement("AdaptiveViewGuid", _viewGuid.ToString(), xmlDoc, viewElement);

            XmlElement widgetsElement = xmlDoc.CreateElement("WidgetsGuid");
            viewElement.AppendChild(widgetsElement);

            foreach (Guid guid in _widgetGuidList)
            {
                if (guid != Guid.Empty)
                {
                    SaveStringToChildElement("Guid", guid.ToString(), xmlDoc, widgetsElement);
                }
            }

            XmlElement mastersElement = xmlDoc.CreateElement("MastersGuid");
            viewElement.AppendChild(mastersElement);

            foreach (Guid guid in _masterGuidList)
            {
                if (guid != Guid.Empty)
                {
                    SaveStringToChildElement("Guid", guid.ToString(), xmlDoc, mastersElement);
                }
            }

            _guides.SaveDataToXml(xmlDoc, viewElement);
        }

        #endregion

        #region IRegion

        public override Guid Guid
        {
            get { return _viewGuid; }
            set { throw new NotSupportedException("Cannot change the guid of page view!"); }
        }

        public override string Name
        {
            get
            {
                if(ParentDocument != null && ParentDocument.IsOpened)
                {
                    if (_viewGuid == ParentDocument.AdaptiveViewSet.Base.Guid)
                    {
                        return ParentDocument.AdaptiveViewSet.Base.Name;
                    }
                    else
                    {
                        IAdaptiveView view = ParentDocument.AdaptiveViewSet.AdaptiveViews[_viewGuid];
                        if (view != null)
                        {
                            return view.Name;
                        }
                    }
                }

                return String.Empty;
            }
            set { throw new NotSupportedException("Cannot change the name of page view!"); }
        }

        public override IRegions GetChildRegions(Guid viewGuid)
        {
            Regions regions = new Regions();

            if (viewGuid == _viewGuid)
            {
                if(Widgets != null)
                {
                    regions.AddRange(Widgets.ToList<IRegion>());
                }

                if (Masters != null)
                {
                    regions.AddRange(Masters.ToList<IRegion>());
                }
            }

            return regions;
        }

        public override IRegionStyle RegionStyle
        {
            get { return PageViewStyle; }
        }

        public override IRegionStyle GetRegionStyle(Guid viewGuid)
        {
            if (viewGuid != _viewGuid)
            {
                return null;
            }

            return PageViewStyle;
        }

        #endregion

        #region IPageView

        public IWidgets Widgets
        {
            get
            {
                if (_widgets == null)
                {
                    _widgets = new Widgets(ParentPage as Page);
                }

                if (_widgets.Count <= 0 && _widgetGuidList.Count > 0)
                {
                    foreach (Guid guid in _widgetGuidList)
                    {
                        Widget widget = ParentPage.Widgets[guid] as Widget;
                        if (widget != null)
                        {
                            _widgets.Add(widget.Guid, widget);
                        }
                    }
                }

                return _widgets;
            }
        }

        public IMasters Masters
        {
            get
            {
                if (_masters == null)
                {
                    _masters = new Masters(ParentPage as Page);
                }

                if (_masters.Count <= 0 && _masterGuidList.Count > 0)
                {
                    foreach (Guid guid in _masterGuidList)
                    {
                        Master master = ParentPage.Masters[guid] as Master;
                        if (master != null)
                        {
                            _masters.Add(master.Guid, master);
                        }
                    }
                }

                return _masters;
            }
        }

        public IGroups Groups
        {
            get
            {
                // Return parent group of placed widgets 
                Groups groups = new Groups(ParentPage as Page);
                if (_widgetGuidList.Count > 0)
                {
                    foreach (Guid guid in _widgetGuidList)
                    {
                        IWidget widget = ParentPage.Widgets[guid];
                        if (widget != null && widget.ParentGroup != null)
                        {
                            groups.Add(widget.ParentGroup.Guid, widget.ParentGroup as Group);
                        }
                    }
                }

                return groups;
            }
        }

        public IGuides Guides
        {
            get { return _guides; }
        }

        // Create a widget in a page view:
        // Add it to page and place it in the page view and its child page view.
        public IWidget CreateWidget(WidgetType type)
        {
            Widget widget = ParentPage.CreateWidget(type) as Widget;
            widget.CreatedViewGuid = _viewGuid;

            PlaceWidgetInternal(widget, true);
            return widget;
        }

        // Delete a widget in a page view:
        // Delete the widget from page, in other words, remove it from all views.
        public void DeleteWidget(Guid widgetGuid)
        {
            ParentPage.DeleteWidget(widgetGuid);
        }

        public void AddWidget(IWidget widget)
        {
            ParentPage.AddWidget(widget);
            PlaceWidgetInternal(widget as Widget, true);
        }

        public void PlaceWidget(Guid widgetGuid)
        {
            Widget widget = ParentPage.Widgets[widgetGuid] as Widget;
            PlaceWidgetInternal(widget, false);
        }

        public void UnplaceWidget(Guid widgetGuid)
        {
            // Just unplace in current view.
            _widgetGuidList.Remove(widgetGuid);

            if (_widgets != null)
            {
                _widgets.Remove(widgetGuid);
            }
        }

        public IMaster CreateMaster(Guid masterPageGuid)
        {
            Master master = ParentPage.CreateMaster(masterPageGuid) as Master;
            master.CreatedViewGuid = _viewGuid;

            PlaceMasterInternal(master, true);
            return master;
        }
        
        public void DeleteMaster(Guid masterGuid)
        {
            ParentPage.DeleteMaster(masterGuid);
        }
        
        public void AddMaster(IMaster master)
        {
            ParentPage.AddMaster(master);
            PlaceMasterInternal(master as Master, true);
        }

        public void PlaceMaster(Guid masterGuid)
        {
            Master master = ParentPage.Masters[masterGuid] as Master;
            PlaceMasterInternal(master, false);
        }
        
        public void UnplaceMaster(Guid masterGuid)
        {
            // Just unplace in current view.
            _masterGuidList.Remove(masterGuid);

            if (_masters != null)
            {
                _masters.Remove(masterGuid);
            }
        }

        public IObjectContainer BreakMaster(Guid masterGuid)
        {
            return ParentPage.BreakMaster(masterGuid);
        }

        public IGroup CreateGroup(List<Guid> guidList)
        {
            return ParentPage.CreateGroup(guidList);
        }

        public void Ungroup(Guid groupGuid)
        {
            ParentPage.Ungroup(groupGuid);
        }

        public void DeleteGroup(Guid groupGuid)
        {
            ParentPage.DeleteGroup(groupGuid);
        }

        public void AddGroup(IGroup group)
        {
            ParentPage.AddGroup(group);
        }

        public IObjectContainer AddObjects(Stream stream)
        {
            IObjectContainer container = ParentPage.AddObjects(stream);
            foreach(Widget widget in container.WidgetList)
            {
                // Set the created view as the current view because this is the first time when the widget
                // is added in the current document.
                widget.CreatedViewGuid = _viewGuid;

                // Set this flag to false. PlaceWidgetInternal method will set this flag base on checking
                // if this is base view.
                widget.HasBeenPlacedInBaseView = false;

                PlaceWidgetInternal(widget, true);
            }

            foreach (Master master in container.MasterList)
            {
                master.CreatedViewGuid = _viewGuid;

                master.HasBeenPlacedInBaseView = false;

                PlaceMasterInternal(master, true);
            }

            return container;
        }

        public IObjectContainer AddCustomObject(ICustomObject customObject, double x = 0, double y = 0)
        {
            IObjectContainer container = ParentPage.AddCustomObject(customObject);
            foreach (Widget widget in container.WidgetList)
            {
                // Set the created view as the current view because this is the first time when the widget
                // is added in the current document.
                widget.CreatedViewGuid = _viewGuid;

                // Set this flag to false. PlaceWidgetInternal method will set this flag base on checking
                // if this is base view.
                widget.HasBeenPlacedInBaseView = false;

                // Get widget style in this view.
                IWidgetStyle widgetViewStyle = widget.GetWidgetStyle(_viewGuid);

                // Widgets in library should have same relative location, so make them have the same delta.
                widgetViewStyle.X += x;
                widgetViewStyle.Y += y;

                if (widget is IHamburgerMenu)
                {
                    IHamburgerMenu menu = widget as IHamburgerMenu;
                    IHamburgerMenuButton button = menu.MenuButton;

                    IWidgetStyle buttonViewStyle = button.GetWidgetStyle(_viewGuid);
                    buttonViewStyle.X += x;
                    buttonViewStyle.Y += y;
                }
                
                PlaceWidgetInternal(widget, true);
            }

            // Custom object doesn't contain masters, so we don't handle container.MasterList.

            return container;
        }

        public IObjectContainer AddMasterPageObject(Guid masterPageGuid, Guid viewGuid, double x = 0, double y = 0)
        {
            IObjectContainer container = ParentPage.AddMasterPageObject(masterPageGuid, viewGuid);

            IMasterPage masterPage = ParentDocument.MasterPages[masterPageGuid];
            
            masterPage.Open();

            PageView masterPageView = masterPage.PageViews[viewGuid] as PageView;
            if (masterPageView == null)
            {
                masterPageView = masterPage.PageViews[ParentDocument.AdaptiveViewSet.Base.Guid] as PageView;
            }

            double xDelta = masterPageView.PageViewStyle.X - x;
            double yDelta = masterPageView.PageViewStyle.Y - y;

            foreach (Widget widget in container.WidgetList)
            {
                // Set the created view as the current view because this is the first time when the widget
                // is added in the current document.
                widget.CreatedViewGuid = _viewGuid;

                // Set this flag to false. PlaceWidgetInternal method will set this flag base on checking
                // if this is base view.
                widget.HasBeenPlacedInBaseView = false;

                // Get widget style in this view.
                IWidgetStyle widgetViewStyle = widget.GetWidgetStyle(_viewGuid);

                // Widgets in master page should have same relative location, so make them have the same delta.
                widgetViewStyle.X -= xDelta;
                widgetViewStyle.Y -= yDelta;

                if (widget is IHamburgerMenu)
                {
                    IHamburgerMenu menu = widget as IHamburgerMenu;
                    IHamburgerMenuButton button = menu.MenuButton;

                    IWidgetStyle buttonViewStyle = button.GetWidgetStyle(_viewGuid);
                    buttonViewStyle.X -= xDelta;
                    buttonViewStyle.Y -= yDelta;
                }

                PlaceWidgetInternal(widget, true);
            }

            // Master Page doesn't contain masters, so we don't handle container.MasterList.

            return container;
        }

        public IGuide CreateGuide(Orientation orientation, double x = 0, double y = 0)
        {
            Guide guide = new Guide(orientation, x, y);
            _guides.Add(guide.Guid, guide);
            return guide;
        }

        public void DeleteGuide(Guid guideGuid)
        {
            _guides.Remove(guideGuid);
        }

        public void AddGuide(IGuide guide)
        {
            Guide guideObject = guide as Guide;
            if (guideObject != null && !_guides.Contains(guideObject.Guid))
            {
                _guides.Add(guideObject.Guid, guideObject);
            }
        }

        #endregion

        #region Internal Methods

        internal void PlaceWidgetInternal(Widget widget, bool placeToChild)
        {
            if (widget != null && !_widgetGuidList.Contains(widget.Guid)
                && ParentDocument != null && ParentDocument.IsOpened)
            {
                _widgetGuidList.Add(widget.Guid);

                if (_widgets != null)
                {
                    _widgets.Add(widget.Guid, widget as Widget);
                }

                // If this widget is never placed in base view before.
                if(widget.HasBeenPlacedInBaseView == false )
                {
                    // If this view is base view, set the flag
                    if(_viewGuid == ParentDocument.AdaptiveViewSet.Base.Guid)
                    {
                        widget.HasBeenPlacedInBaseView = true;

                        // If the widget is not created in base view, set the create view style as the base view style.
                        // If the widget is IPageEmbeddedWidget, we don't update the child widgets style.
                        if (widget.CreatedViewGuid != ParentDocument.AdaptiveViewSet.Base.Guid)
                        {
                            WidgetStyle createdViewStyle = widget.GetWidgetStyle(widget.CreatedViewGuid) as WidgetStyle;
                            WidgetStyle baseViewStyle = widget.WidgetStyle as WidgetStyle;
                            if(createdViewStyle != null)
                            {
                                Style.CopyStyle(createdViewStyle, baseViewStyle, null, null);
                            }
                        }
                    }
                }

                // Place to child page view.
                if (placeToChild)
                {
                    IAdaptiveView view = null;
                    if(_viewGuid == ParentDocument.AdaptiveViewSet.Base.Guid)
                    {
                        view = ParentDocument.AdaptiveViewSet.Base;
                    }
                    else
                    {
                        view = ParentDocument.AdaptiveViewSet.AdaptiveViews[_viewGuid];
                    }

                    if (view != null)
                    {
                        foreach (AdaptiveView childView in view.ChildViews)
                        {
                            PageView childPageView = ParentPage.PageViews[childView.Guid] as PageView;
                            if (childPageView != null)
                            {
                                childPageView.PlaceWidgetInternal(widget, true);
                            }
                        }
                    }
                }
            }
        }


        internal void PlaceMasterInternal(Master master, bool placeToChild)
        {
            if (master != null && !_masterGuidList.Contains(master.Guid)
                && ParentDocument != null && ParentDocument.IsOpened)
            {
                _masterGuidList.Add(master.Guid);

                if (_masters != null)
                {
                    _masters.Add(master.Guid, master as Master);
                }

                // If this master is never placed in base view before.
                if (master.HasBeenPlacedInBaseView == false)
                {
                    // If this view is base view, set the flag
                    if (_viewGuid == ParentDocument.AdaptiveViewSet.Base.Guid)
                    {
                        master.HasBeenPlacedInBaseView = true;

                        // If the widget is not created in base view, set the create view style as the base view style.
                        // If the widget is IPageEmbeddedWidget, we don't update the child widgets style.
                        if (master.CreatedViewGuid != ParentDocument.AdaptiveViewSet.Base.Guid)
                        {
                            MasterStyle createdViewStyle = master.GetMasterStyle(master.CreatedViewGuid) as MasterStyle;
                            MasterStyle baseViewStyle = master.MasterStyle as MasterStyle;
                            if (createdViewStyle != null)
                            {
                                Style.CopyStyle(createdViewStyle, baseViewStyle, null, null);
                            }
                        }
                    }
                }

                // Place to child page view.
                if (placeToChild)
                {
                    IAdaptiveView view = null;
                    if (_viewGuid == ParentDocument.AdaptiveViewSet.Base.Guid)
                    {
                        view = ParentDocument.AdaptiveViewSet.Base;
                    }
                    else
                    {
                        view = ParentDocument.AdaptiveViewSet.AdaptiveViews[_viewGuid];
                    }

                    if(view != null)
                    {
                        foreach (AdaptiveView childView in view.ChildViews)
                        {
                            PageView childPageView = ParentPage.PageViews[childView.Guid] as PageView;
                            if (childPageView != null)
                            {
                                childPageView.PlaceMasterInternal(master, true);
                            }
                        }
                    }
                }
            }
        }

        internal List<Guid> WidgetGuidList
        {
            get { return _widgetGuidList; }
            set 
            { 
                _widgetGuidList = value;
                _widgets = null;
            }
        }

        internal List<Guid> MasterGuidList
        {
            get { return _masterGuidList; }
            set
            {
                _masterGuidList = value;
                _masters = null;
            }
        }

        internal PageViewStyle PageViewStyle
        {
            get
            {
                if (_style == null)
                {
                    _style = new PageViewStyle(this);
                }

                return _style;
            }
        }
        
        #endregion

        #region Private Fields

        private Guid _viewGuid;

        private Widgets _widgets;
        private List<Guid> _widgetGuidList = new List<Guid>();

        private Masters _masters;
        private List<Guid> _masterGuidList = new List<Guid>();

        private Guides _guides;

        private PageViewStyle _style;

        #endregion
    }
}
