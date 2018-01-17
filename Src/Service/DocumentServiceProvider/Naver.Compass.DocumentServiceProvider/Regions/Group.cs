using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class Group : Region, IGroup
    {
        #region Constructors

        internal Group()
            :this(null)
        {
        }

        internal Group(Page parentPage)
            : base(parentPage, "Group")
        {
        }

        #endregion

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            LoadGuidFromChildElementInnerText("ParentGroupGuid", element, ref _parentGroupGuid);

            XmlElement groupsElement = element["GroupsGuid"];
            if (groupsElement != null && groupsElement.ChildNodes.Count > 0)
            {
                XmlNodeList childList = groupsElement.ChildNodes;
                foreach (XmlElement childElement in childList)
                {
                    try
                    {
                        Guid guid = new Guid(childElement.InnerText);
                        if(guid != Guid.Empty)
                        {
                            _groupGuidList.Add(guid);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }

            XmlElement widgetsElement = element["WidgetsGuid"];
            if (widgetsElement != null && widgetsElement.ChildNodes.Count > 0)
            {
                XmlNodeList childList = widgetsElement.ChildNodes;
                foreach (XmlElement childElement in childList)
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
                    }
                }
            }

            XmlElement mastersElement = element["MastersGuid"];
            if (mastersElement != null && mastersElement.ChildNodes.Count > 0)
            {
                XmlNodeList childList = mastersElement.ChildNodes;
                foreach (XmlElement childElement in childList)
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
                    }
                }
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement groupElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(groupElement);

            base.SaveDataToXml(xmlDoc, groupElement);

            if (_parentGroupGuid != Guid.Empty)
            {
                SaveStringToChildElement("ParentGroupGuid", _parentGroupGuid.ToString(), xmlDoc, groupElement);
            }

            XmlElement groupsElement = xmlDoc.CreateElement("GroupsGuid");
            groupElement.AppendChild(groupsElement);
            foreach (Guid guid in _groupGuidList)
            {
                if(guid != Guid.Empty)
                {
                    SaveStringToChildElement("Guid", guid.ToString(), xmlDoc, groupsElement);
                }
            }
            
            XmlElement widgetsElement = xmlDoc.CreateElement("WidgetsGuid");
            groupElement.AppendChild(widgetsElement);
            foreach (Guid guid in _widgetGuidList)
            {
                if (guid != Guid.Empty)
                {
                    SaveStringToChildElement("Guid", guid.ToString(), xmlDoc, widgetsElement);
                }
            }

            XmlElement mastersElement = xmlDoc.CreateElement("MastersGuid");
            groupElement.AppendChild(mastersElement);
            foreach (Guid guid in _masterGuidList)
            {
                if (guid != Guid.Empty)
                {
                    SaveStringToChildElement("Guid", guid.ToString(), xmlDoc, mastersElement);
                }
            }
        }

        #endregion

        #region IRegion

        public override IRegions GetChildRegions(Guid viewGuid)
        {
            Regions regions = new Regions();

            if(Groups != null)
            {
                regions.AddRange(Groups.ToList<IRegion>());
            }

            if(Widgets != null)
            {
                regions.AddRange(Widgets.ToList<IRegion>());
            }

            if(Masters != null)
            {
                regions.AddRange(Masters.ToList<IRegion>());
            }

            return regions;
        }

        public override IRegionStyle RegionStyle
        {
            get
            {
                if(_style == null)
                {
                    Guid baseViewGuid = Guid.Empty;
                    if (ParentDocument != null && ParentDocument.IsOpened)
                    {
                        baseViewGuid = ParentDocument.AdaptiveViewSet.Base.Guid;
                    }
                    _style = new GroupStyle(this, baseViewGuid);
                }

                return _style;
            }
        }

        public override IRegionStyle GetRegionStyle(Guid viewGuid)
        {
            if(viewGuid == Guid.Empty)
            {
                return null;
            }

            if (ParentDocument != null && ParentDocument.IsOpened
                && viewGuid == ParentDocument.AdaptiveViewSet.Base.Guid)
            {
                return RegionStyle;
            }
            else
            {
                if (GroupStyles.ContainsKey(viewGuid))
                {
                    return GroupStyles[viewGuid];
                }
                else if (ParentDocument != null && ParentDocument.IsOpened)
                {
                    IAdaptiveView view = ParentDocument.AdaptiveViewSet.AdaptiveViews[viewGuid];
                    if (view != null)
                    {
                        GroupStyle style = new GroupStyle(this, viewGuid);
                        GroupStyles[viewGuid] = style;
                        return style;
                    }
                }

                return null;
            }
        }

        #endregion

        #region IGroup

        public IGroup ParentGroup
        {
            get 
            {
                if (_parentGroup == null && ParentPage != null)
                {
                    _parentGroup = ParentPage.Groups[_parentGroupGuid] as Group;
                }

                return _parentGroup;
            }

            set 
            { 
                _parentGroup = value as Group;
                if (_parentGroup != null)
                {
                    _parentGroupGuid = _parentGroup.Guid;
                }
                else
                {
                    _parentGroupGuid = Guid.Empty;
                }
            }
        }

        public Guid ParentGroupGuid
        {
            get { return _parentGroupGuid; }
            set
            {
                _parentGroupGuid = value;
                _parentGroup = null;
            }
        }

        public IGroups Groups
        {
            get
            {
                if (_groups == null && ParentPage != null)
                {
                    _groups = new Groups(ParentPage as Page);
                    
                    foreach (Guid guid in _groupGuidList)
                    {
                        Group group = ParentPage.Groups[guid] as Group;
                        if (group != null)
                        {
                            _groups.Add(group.Guid, group);
                        }
                    }
                }

                return _groups;
            }
        }

        public IWidgets Widgets
        {
            get
            {
                if (_widgets == null && ParentPage != null)
                {
                    _widgets = new Widgets(ParentPage as Page);

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
                if (_masters == null && ParentPage != null)
                {
                    _masters = new Masters(ParentPage as Page);

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

        public bool IsChild(Guid childGuid, bool recursive)
        {
            if (_groupGuidList.Contains(childGuid) 
                || _widgetGuidList.Contains(childGuid)
                || _masterGuidList.Contains(childGuid))
            {
                return true;
            }

            if(recursive)
            {
                // Search in child group
                foreach(IGroup group in Groups)
                {
                    if (group.IsChild(childGuid, recursive))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Internal Methods

        internal void AddChildren(List<Guid> guidList)
        {
            if (ParentPage != null && guidList != null)
            {
                foreach (Guid guid in guidList)
                {
                    if (ParentPage.Groups.Contains(guid))
                    {
                        AddGroup(ParentPage.Groups[guid] as Group);
                    }
                    else if (ParentPage.Widgets.Contains(guid))
                    {
                        AddWidget(ParentPage.Widgets[guid] as Widget);
                    }
                    else if (ParentPage.Masters.Contains(guid))
                    {
                        AddMaster(ParentPage.Masters[guid] as Master);
                    }
                }
            }
        }

        internal void AddGroup(Group group)
        {
            if (group == null)
            {
                return;
            }

            group.ParentGroup = this;

            if (!_groupGuidList.Contains(group.Guid)) 
            {
                _groupGuidList.Add(group.Guid);
            }

            if (_groups != null && !_groups.Contains(group.Guid))
            {
                _groups.Add(group.Guid, group);
            }
        }

        internal void RemoveGroup(Group group)
        {
            if (group == null)
            {
                return;
            }

            _groupGuidList.Remove(group.Guid);

            if (_groups != null)
            {
                _groups.Remove(group.Guid);
            }
        }

        internal void AddWidget(Widget widget)
        {
            if (widget == null)
            {
                return;
            }

            widget.ParentGroup = this;

            if (!_widgetGuidList.Contains(widget.Guid))
            {
                _widgetGuidList.Add(widget.Guid);
            }

            if (_widgets != null && !_widgets.Contains(widget.Guid))
            {
                _widgets.Add(widget.Guid, widget);
            }
        }

        internal void RemoveWidget(Widget widget)
        {
            if (widget == null)
            {
                return;
            }

            _widgetGuidList.Remove(widget.Guid);

            if (_widgets != null)
            {
                _widgets.Remove(widget.Guid);
            }
        }

        internal void AddMaster(Master master)
        {
            if (master == null)
            {
                return;
            }

            master.ParentGroup = this;

            if (!_masterGuidList.Contains(master.Guid))
            {
                _masterGuidList.Add(master.Guid);
            }

            if (_masters != null && !_masters.Contains(master.Guid))
            {
                _masters.Add(master.Guid, master);
            }
        }

        internal void RemoveMaster(Master master)
        {
            if (master == null)
            {
                return;
            }

            _masterGuidList.Remove(master.Guid);

            if (_masters != null)
            {
                _masters.Remove(master.Guid);
            }
        }

        // Return a copy list to avoid change the Guid list.
        internal List<Guid> WidgetGuidList
        {
            get { return new List<Guid>(_widgetGuidList); }
            set 
            { 
                _widgetGuidList = value;
                _widgets = null;
            }
        }

        internal List<Guid> MasterGuidList
        {
            get { return new List<Guid>(_masterGuidList); }
            set
            {
                _masterGuidList = value;
                _masters = null;
            }
        }

        internal List<Guid> GroupGuidList
        {
            get { return new List<Guid>(_groupGuidList); }
            set 
            { 
                _groupGuidList = value;
                _groups = null;
            }
        }

        #endregion

        #region  Private Methods

        private Dictionary<Guid, GroupStyle> GroupStyles
        {
            get
            {
                if(_styles == null)
                {
                    _styles = new Dictionary<Guid, GroupStyle>();
                }

                return _styles;
            }
        }
         
        #endregion

        #region Private fields

        private Group _parentGroup;
        private Guid _parentGroupGuid;

        private List<Guid> _groupGuidList = new List<Guid>();
        private List<Guid> _widgetGuidList = new List<Guid>();
        private List<Guid> _masterGuidList = new List<Guid>();

        private Groups _groups;
        private Widgets _widgets;
        private Masters _masters;

        private GroupStyle _style;
        private Dictionary<Guid, GroupStyle> _styles;

        #endregion
    }
}
