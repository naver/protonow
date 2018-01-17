using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Service;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;

using UndoCompositeCommand = Naver.Compass.InfoStructure.CompositeCommand;

namespace Naver.Compass.Module
{
    class InteractionTabVM : ViewModelBase
    {
        public InteractionTabVM()
        {
            _ListEventAggregator.GetEvent<SelectionChangeEvent>().Subscribe(SelectionChangeEventHandler);
            _ListEventAggregator.GetEvent<UpdatePageTreeEvent>().Subscribe(UpdatePageTreeEventHandler);
            _ListEventAggregator.GetEvent<UpdateLanguageEvent>().Subscribe(UpdateLanguageHandler);

            _model = PagelistTreeModel.GetInstance();
            LinkPageCommand = new DelegateCommand<object>(LinkPageExecute);
            MouseOverObjectCommand = new DelegateCommand<IUniqueObject>(MouserOverObjectExecute);
            MouseLeaveObjectCommand = new DelegateCommand<object>(MouseLeaveOjectExecute);
            WidgetList = new RangeObservableCollection<WidgetNode>();
        }


        #region Event handler

        /// <summary>
        /// Current seleted widgets changed.
        /// </summary>
        private void SelectionChangeEventHandler(string eventArg)
        {
            // Reload selected widgets and its actions
            try
            {
                LoadActions();
                FirePropertyChanged("IsStandardType");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadActions(): " + ex.Message.ToString());
            }
            
        }

        private void UpdatePageTreeEventHandler(object eventArg)
        {
            if (_selectedWidget == null || _currentPage == null || !IsStandardType)
                return;

            _model.LoadPageTree();
            if (_widgetOpenAction != null && _widgetOpenAction.LinkType == LinkType.LinkToPage)
            {
                SelectPage(_widgetOpenAction.LinkPageGuid);
            }
        }
        private void UpdateLanguageHandler(object eventArg)
        {
            LoadPageWidgets();
        }
        private void LinkPageExecute(object obj)
        {
            PageNode node = obj as PageNode;
            if (node == null)
                return;

            // Selected page changed
            if (_widgetOpenAction != null && _widgetOpenAction.LinkPageGuid != node.Guid)
            {
                if (CurrentUndoManager != null)
                {
                    SelectPageCommand cmd = new SelectPageCommand(this, _widgetOpenAction.LinkPageGuid, node.Guid,
                                                  _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);

                    CurrentUndoManager.Push(cmd);
                }

                _widgetOpenAction.LinkPageGuid = node.Guid;

                Document.IsDirty = true;
            }
        }

        private void MouserOverObjectExecute(IUniqueObject widget)
        {
            if (widget != null)
            {
                _ListEventAggregator.GetEvent<MouseOverInteractionObject>().Publish(widget);
            }
        }
        private void MouseLeaveOjectExecute(object obj)
        {
            _ListEventAggregator.GetEvent<MouseOverInteractionObject>().Publish(null);
        }

        #endregion

        #region Public Binding Properties

        public DelegateCommand<object> LinkPageCommand { get; set; }
        public DelegateCommand<IUniqueObject> MouseOverObjectCommand { get; set; }
        public DelegateCommand<object> MouseLeaveObjectCommand { get; set; }

        public ObservableCollection<PageNode> PageList
        {
            get
            {
                return _model.RootNode.Children;
            }
        }

        public bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                if(_canEdit!=value)
                {
                    _canEdit = value;
                    FirePropertyChanged("CanEdit");
                }
            }
        }
        public RangeObservableCollection<WidgetNode> WidgetList { get; set; }

        public bool NoneChecked
        {
            get { return _noneChecked; }
            set
            {
                if (_noneChecked != value)
                {
                    if (CurrentUndoManager != null)
                    {
                        UndoCompositeCommand cmds = new UndoCompositeCommand();

                        NoneLinkCommand noneCmd = new NoneLinkCommand(this, _noneChecked, value,
                                                _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);

                        cmds.AddCommand(noneCmd);

                        if (value)
                        {
                            if (_linkChecked)
                            {
                                ExternalLinkCommand linkCmd = new ExternalLinkCommand(this, true, false,
                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid, _externalLink);

                                cmds.AddCommand(linkCmd);
                            }
                            else if (_pageChecked)
                            {
                                PageLinkCommand pageCmd = new PageLinkCommand(this, true, false,
                                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid,
                                                                    _widgetOpenAction == null ? Guid.Empty : _widgetOpenAction.LinkPageGuid);

                                cmds.AddCommand(pageCmd);
                            }

                            //Uncheck New window
                            if (IsNewWindowChecked)
                            {
                                ShowHideCommand cmd = new ShowHideCommand(this, IsNewWindowChecked, false,
                                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);
                                cmds.AddCommand(cmd);

                                Raw_IsNewWindowChecked = false;
                            }
                        }

                        CurrentUndoManager.Push(cmds);
                    }

                    Raw_NoneChecked = value;
                }
            }
        }

        public bool PageChecked
        {
            get { return _pageChecked; }
            set
            {
                if (_pageChecked != value)
                {
                    if (CurrentUndoManager != null)
                    {
                        UndoCompositeCommand cmds = new UndoCompositeCommand();

                        PageLinkCommand pageCmd = new PageLinkCommand(this, _pageChecked, value,
                                                _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid,
                                                _widgetOpenAction == null ? Guid.Empty : _widgetOpenAction.LinkPageGuid);

                        cmds.AddCommand(pageCmd);

                        if (value)
                        {
                            if (_noneChecked)
                            {
                                NoneLinkCommand noneCmd = new NoneLinkCommand(this, true, false,
                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);

                                cmds.AddCommand(noneCmd);
                            }
                            else if (_linkChecked)
                            {
                                ExternalLinkCommand linkCmd = new ExternalLinkCommand(this, true, false,
                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid, _externalLink);

                                cmds.AddCommand(linkCmd);
                            }

                            //Uncheck New window
                            if(IsNewWindowChecked)
                            {
                                ShowHideCommand cmd = new ShowHideCommand(this, IsNewWindowChecked, false,
                                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);
                                cmds.AddCommand(cmd);

                                Raw_IsNewWindowChecked = false;
                            }
                        }

                        CurrentUndoManager.Push(cmds);

                        
                    }

                    Raw_PageChecked = value;
                }
            }
        }

        public bool LinkChecked
        {
            get { return _linkChecked; }
            set
            {
                if (_linkChecked != value)
                {
                    if (CurrentUndoManager != null)
                    {
                        UndoCompositeCommand cmds = new UndoCompositeCommand();

                        ExternalLinkCommand linkCmd = new ExternalLinkCommand(this, _linkChecked, value,
                                            _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid, _externalLink);

                        cmds.AddCommand(linkCmd);

                        if (value)
                        {
                            if (_noneChecked)
                            {
                                NoneLinkCommand noneCmd = new NoneLinkCommand(this, true, false,
                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);

                                cmds.AddCommand(noneCmd);
                            }
                            else if (_pageChecked)
                            {
                                PageLinkCommand pageCmd = new PageLinkCommand(this, true, false,
                                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid,
                                                                    _widgetOpenAction == null ? Guid.Empty : _widgetOpenAction.LinkPageGuid);

                                cmds.AddCommand(pageCmd);
                            }
                        }

                        CurrentUndoManager.Push(cmds);
                    }

                    Raw_LinkChecked = value;
                }
            }
        }

        public string ExternalLink
        {
            get
            {
                return _externalLink;
            }
            set
            {
                if (_externalLink != value)
                {
                    if (CurrentUndoManager != null)
                    {
                        UrlChangeCommand cmd = new UrlChangeCommand(this, _externalLink, value,
                                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);
                        CurrentUndoManager.Push(cmd);
                    }

                    Raw_ExternalLink = value;
                }
            }
        }

        public bool IsNewWindowChecked
        {
            get
            {
                if (_widgetOpenAction == null)
                    return false;
                return (_widgetOpenAction.OpenIn == ActionOpenIn.NewWindowOrTab);
            }
            set
            {
                bool bOpenInNew = _widgetOpenAction.OpenIn == ActionOpenIn.NewWindowOrTab;
                if (bOpenInNew != value)
                {
                    if (CurrentUndoManager != null)
                    {
                        ShowHideCommand cmd = new ShowHideCommand(this, bOpenInNew, value,
                                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);
                        CurrentUndoManager.Push(cmd);
                    }
                    Raw_IsNewWindowChecked = value;
                }
            }

        }

        public VisibilityType ShowHideType
        {
            get
            {
                if (ShowHideTarget != null)
                {
                    return ShowHideTarget.VisibilityType;
                }
                else
                {
                    return _showHideType;
                }

            }
            set
            {
                if(_showHideType!=value)
                {
                    if (CurrentUndoManager != null)
                    {
                        ShowHideCommand cmd = new ShowHideCommand(this, _showHideType, value,
                                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);
                        CurrentUndoManager.Push(cmd);
                    }
                }
                Raw_ShowHideType = value;
            }
        }

        public ShowHideAnimateType AnimateType
        {
            get
            {
                if (ShowHideTarget != null)
                {
                    return ShowHideTarget.AnimateType;
                }
                else
                {
                    return _showHideAnimateType;
                }
            }
            set
            {
                if (_showHideAnimateType != value)
                {
                    if (CurrentUndoManager != null)
                    {
                        ShowHideCommand cmd = new ShowHideCommand(this, _showHideAnimateType, value,
                                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);
                        CurrentUndoManager.Push(cmd);
                    }
                }
                Raw_AnimateType = value;
            }
        }

        public int AnimateTime
        {
            get
            {
                if (ShowHideTarget != null)
                {
                    return ShowHideTarget.AnimateTime;
                }
                else
                {
                    return _animateTime;
                }
            }
            set
            {
                if (_animateTime != value)
                {
                    if (CurrentUndoManager != null)
                    {
                        ShowHideCommand cmd = new ShowHideCommand(this, _animateTime, value,
                                                                    _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);
                        CurrentUndoManager.Push(cmd);
                    }
                }
                Raw_AnimateTime = value;
            }
        }

        public bool IsCheckAll
        {
            get
            {
                return _isCheckAll;
            }
            set
            {
                if(_isCheckAll!=value)
                {
                   if(CurrentUndoManager!=null)
                   {
                       CheckAllTargetCommand cmd = new CheckAllTargetCommand(this, _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid, value);

                       CurrentUndoManager.Push(cmd);
                   }
                }
                Raw_IsCheckAll = value;
            }
        }

        #endregion

        #region Public Members

        public IInteractionOpenAction WidgetOpenAction
        {
            get { return _widgetOpenAction; }
        }

        public IInteractionShowHideAction WidgetShowHideAction
        {
            get { return _widgetShowHideAction; }
        }


        public void SelectPage(Guid pageGuid)
        {
            if(_widgetOpenAction != null && _widgetOpenAction.LinkPageGuid != pageGuid)
            {
                DeselectPage(_widgetOpenAction.LinkPageGuid);

                _widgetOpenAction.LinkPageGuid = pageGuid;

                Document.IsDirty = true;
            }

            if(pageGuid == Guid.Empty)
            {
                _model.SelectPage(_currentPage.Guid);
            }
            else
            {
                _model.SelectPage(pageGuid);
            }
        }

        public void DeselectPage(Guid pageGuid)
        {
            if (pageGuid != Guid.Empty)
            {
                _model.DeselectPage(pageGuid);
            }
        }

        public void AddTarget(IUniqueObject targetObject)
        {
            if (WidgetShowHideAction != null)
            {
                WidgetShowHideAction.AddTargetObject(targetObject.Guid);

                SetShowHideAction();

                //current action is not "Check All"
                if (!_isCheckAllAction)
                {
                    if (CurrentUndoManager != null) 
                    {  //widget
                        AddTargetCommand cmd = new AddTargetCommand(this, targetObject.Guid, _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);
                        CurrentUndoManager.Push(cmd);
                    }
                   
                    //if all is checked , set IsCheckAll to true.
                    var node = WidgetList.FirstOrDefault(a => a.IsSelected == false);
                    if (node == null)
                    {
                        _isCheckAll = true;
                        FirePropertyChanged("IsCheckAll");
                    }
                }
                FirePropertyChanged("ShowHideType");
                FirePropertyChanged("AnimateType");
                FirePropertyChanged("AnimateTime");
                FirePropertyChanged("IsShowHideEnabled");
            }
        }

        /// <summary>
        /// Undo add every widget in the group to object list.
        /// </summary>
        /// <param name="targetObject"> group</param>
        /// <param name="cmds"></param>
        private void UndoAddGroupTarget(IGroup targetGroup, UndoCompositeCommand cmds)
        {
            foreach (IGroup group in targetGroup.Groups)
            {
                UndoAddGroupTarget(group, cmds);
            }
            foreach (IWidget target in targetGroup.Widgets)
            {
                AddTargetCommand cmd = new AddTargetCommand(this, target.Guid, _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);
                cmds.AddCommand(cmd);
            }
        }
        public void DeleteTaget(IUniqueObject targetObject)
        {
            if (WidgetShowHideAction != null)
            {
                WidgetShowHideAction.DeleteTagetObject(targetObject.Guid);

                //current action is not "Check All"
                if (!_isCheckAllAction)
                {
                    if (CurrentUndoManager != null)
                    {
                        DeleteTargetCommand cmd = new DeleteTargetCommand(this, targetObject.Guid, _selectedWidget == null ? Guid.Empty : _selectedWidget.Guid);
                        CurrentUndoManager.Push(cmd);
                    }

                    SetShowHideAction();

                    // Set IsCheckAll to false if one widget is unchecked.
                    _isCheckAll = false;
                    FirePropertyChanged("IsCheckAll");
                }
                FirePropertyChanged("IsShowHideEnabled");
            }
        }

        public bool Raw_NoneChecked
        {
            set
            {
                if (_noneChecked != value)
                {
                    _noneChecked = value;

                    if (_noneChecked == true)
                    {
                        if (_widgetOpenAction != null && _widgetOpenAction.LinkType!=LinkType.None)
                        {
                            _widgetOpenAction.LinkType = LinkType.None;
                            Document.IsDirty = true;
                        }

                        // Set other radio button to false explicitly, so PageChecked and LinkChecked will not handle undo/redo
                        Raw_PageChecked = Raw_LinkChecked = false;
                    }

                    FirePropertyChanged("NoneChecked");
                    FirePropertyChanged("IsNewWindowEnabled");
                }
            }
        }

        public bool Raw_PageChecked
        {
            set
            {
                if (_pageChecked != value)
                {
                    _pageChecked = value;

                    if (_pageChecked == true)
                    {
                        if (_widgetOpenAction != null && _widgetOpenAction.LinkType!=LinkType.LinkToPage)
                        {
                            _widgetOpenAction.LinkType = LinkType.LinkToPage;

                            if(_widgetOpenAction.LinkPageGuid == Guid.Empty)
                            {
                                _widgetOpenAction.LinkPageGuid = _currentPage.Guid;
                            }

                            SelectPage(_widgetOpenAction.LinkPageGuid);
                            Document.IsDirty = true;
                        }

                        Raw_NoneChecked = Raw_LinkChecked = false;
                    }
                    else
                    {
                        if (_widgetOpenAction != null)
                        {
                            DeselectPage(_widgetOpenAction.LinkPageGuid);
                            _widgetOpenAction.LinkPageGuid = Guid.Empty;
                            Document.IsDirty = true;
                        }
                    }

                    FirePropertyChanged("PageChecked");
                    FirePropertyChanged("IsNewWindowEnabled");
                }
            }
        }

        public bool Raw_LinkChecked
        {
            set
            {
                if (_linkChecked != value)
                {
                    _linkChecked = value;

                    if (_linkChecked == true)
                    {
                        if (_widgetOpenAction != null && _widgetOpenAction.LinkType!=LinkType.LinkToUrl)
                        {
                            _widgetOpenAction.LinkType = LinkType.LinkToUrl;
                            Document.IsDirty = true;
                        }

                        Raw_NoneChecked = Raw_PageChecked = false;
                    }
                    else
                    {
                        Raw_ExternalLink = String.Empty;
                    }

                    FirePropertyChanged("LinkChecked");
                    FirePropertyChanged("IsNewWindowEnabled");
                }
            }
        }

        public string Raw_ExternalLink
        {
            set
            {
                if (_externalLink != value)
                {
                    _externalLink = value;
                    
                    if (_widgetOpenAction != null)
                    {
                        _widgetOpenAction.ExternalUrl = value;
                        Document.IsDirty = true;
                    }

                    FirePropertyChanged("ExternalLink");
                }
            }
        }

        public bool Raw_IsNewWindowChecked
        {
            set
            {
                if ((_widgetOpenAction.OpenIn == ActionOpenIn.NewWindowOrTab)!=value)
                {
                    _widgetOpenAction.OpenIn = value ? ActionOpenIn.NewWindowOrTab : ActionOpenIn.CurrentWindow;
                    Document.IsDirty = true;
                }
                FirePropertyChanged("IsNewWindowChecked");
            }

        }

        public IShowHideActionTarget ShowHideTarget
        {
            get
            {
                if (_widgetShowHideAction == null ||_widgetShowHideAction.TargetObjects.Count <= 0)
                    return null;

                IShowHideActionTarget target = _widgetShowHideAction.TargetObjects[0];

                _showHideAnimateType = target.AnimateType;
                _showHideType = target.VisibilityType;
                _animateTime = target.AnimateTime;

                return target;
            }
        }
        public VisibilityType Raw_ShowHideType
        {
            set
            {
                if(value!=_showHideType)
                {
                    _showHideType = value;
                    _widgetShowHideAction.SetAllVisibilityType(value);
                    Document.IsDirty = true;
                }
                FirePropertyChanged("ShowHideType");
            }
        }

        public ShowHideAnimateType Raw_AnimateType 
        {
            set
            {
                if(_showHideAnimateType!=value)
                {
                    _showHideAnimateType = value;
                    _widgetShowHideAction.SetAllAnimateType(value);
                    Document.IsDirty = true;
                }
                FirePropertyChanged("AnimateType");
            }
        }

        public int Raw_AnimateTime
        {
            set
            {
                if(_animateTime!=value)
                {
                    _animateTime = value;
                    _widgetShowHideAction.SetAllAnimateTime(value);
                    Document.IsDirty = true;
                }
                FirePropertyChanged("AnimateTime");
            }
        }

        public bool Raw_IsCheckAll
        {
            set
            {
                if(_isCheckAll!=value)
                {
                    _isCheckAll = value;

                    _isCheckAllAction = true;
                    foreach (WidgetNode node in WidgetList)
                    {
                        node.IsSelected = value;
                    }
                    _isCheckAllAction = false;
                }
                FirePropertyChanged("IsCheckAll");
            }
        }

        public bool IsNewWindowEnabled
        {
            get
            {
                return _linkChecked;
            }
        }

        public bool IsShowHideEnabled
        {
            get
            {//If check any objects.

                if(null != WidgetList.FirstOrDefault(a => a.IsSelected == true))
                {
                    return true;
                }
                else
                {
                    InitShowHideAction();
                    return false;
                }
            }
        }

        public bool IsStandardType
        {
            get
            {
                if (Document != null)
                    return Document.DocumentType == DocumentType.Standard;
                return true;
            }
        } 

        #endregion

        #region Helper 

        private IDocument Document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        private void DisableInteraction()
        {
            CanEdit = false;
            if (_widgetOpenAction != null)
            {
                DeselectPage(_widgetOpenAction.LinkPageGuid);
            }

            _selectedWidget = null;
            _widgetOpenAction = null;

            Raw_ExternalLink = string.Empty;
            Raw_NoneChecked = Raw_PageChecked = Raw_LinkChecked = false;

            WidgetList.Clear();
            InitShowHideAction();
            FirePropertyChanged("IsNewWindowChecked");
            
        }

        private void InitShowHideAction()
        {
            _showHideType = VisibilityType.Show;
            _showHideAnimateType = ShowHideAnimateType.None;
            _animateTime = 500;
        }

        private void LoadActions()
        {
            List<IWidgetPropertyData> list = SelectionService.GetSelectedWidgets();
            if (_currentPage == null
                || list == null
                || list.Count != 1
                || !(list.ElementAt(0) is WidgetViewModBase)
                || (list.ElementAt(0) as WidgetViewModBase).IsGroup
                || (list.ElementAt(0) as WidgetViewModBase).Type == ObjectType.Master)
            {
                // Only support one selected widget.
                DisableInteraction();
                return;
            }

            IWidget widget = _currentPage.Widgets.GetWidget(list[0].WidgetID);
            if (widget == null || _selectedWidget == widget 
                || widget.WidgetType == WidgetType.HamburgerMenu 
                || widget.WidgetType == WidgetType.DynamicPanel
                || widget.WidgetType == WidgetType.Toast)
            {
                // Selected item is not widget, or the same widget
                DisableInteraction();
                return;
            }

            CanEdit = true;
            // Selected widget changed, get or create action from new widget.
            _widgetOpenAction = null;
            _widgetShowHideAction = null;
            _selectedWidget = widget;

            IInteractionEvent clickEvent = widget.Events[EventType.OnClick];
            if (clickEvent == null)
                return;

            //Create action 
            IInteractionCase clickCase = clickEvent.Cases["clickCase"];
            if (clickCase == null)
            {
                clickCase = clickEvent.CreateCase("clickCase");
            }

            // For now, we only have a open action in exsiting case
            foreach (IInteractionAction action in clickCase.Actions)
            {
                if (action != null)
                {
                    if (action.ActionType == ActionType.OpenAction)
                    {
                        _widgetOpenAction = action as IInteractionOpenAction;
                        LoadWidgetInteraction();
                    }
                    else if (action.ActionType == ActionType.ShowHideAction)
                    {
                        _widgetShowHideAction = action as IInteractionShowHideAction;
                        LoadPageWidgets();                       
                    }
                   
                }
            }

            if (_widgetOpenAction == null)
            {
                _widgetOpenAction = clickCase.CreateAction(ActionType.OpenAction) as IInteractionOpenAction;
                LoadWidgetInteraction();
            }

            if (_widgetShowHideAction == null)
            {
                _widgetShowHideAction = clickCase.CreateAction(ActionType.ShowHideAction) as IInteractionShowHideAction;
                //_widgetShowHideAction.AddTargetObject(widget.Guid); // Add the widget to the target list by default. 
                LoadPageWidgets();
            }
           
           
        }

        private void LoadWidgetInteraction()
        {

            // Cannot get or create action
            if(_widgetOpenAction == null)
            {
                DisableInteraction();
                return;
            }

            if (IsStandardType)
            {
                _model.LoadPageTree();
            }
            
            if (_widgetOpenAction.LinkType == LinkType.None)
            {
                Raw_NoneChecked = true;
            }
            else if (_widgetOpenAction.LinkType == LinkType.LinkToPage)
            {
                Raw_PageChecked = true;
                SelectPage(_widgetOpenAction.LinkPageGuid);
            }
            else if (_widgetOpenAction.LinkType == LinkType.LinkToUrl)
            {
                Raw_LinkChecked = true;
                Raw_ExternalLink = _widgetOpenAction.ExternalUrl;
                Raw_IsNewWindowChecked = (_widgetOpenAction.OpenIn == ActionOpenIn.NewWindowOrTab);
            }

        }

        /// <summary>
        /// load all widgets/groups which is not in a group in current page.
        /// which means,only load top level  widget/group.
        /// </summary>
        public void LoadPageWidgets()
        {

            if (_selectedWidget == null ||_widgetShowHideAction == null)
            {
                DisableInteraction();
                return;
            }

            WidgetList.Clear();

            List<IWidgetPropertyData> allWidgets = SelectionService.GetCurrentPage().GetAllWidgets();
            List<WidgetNode> TargetWidgets = new List<WidgetNode>();
            foreach (IWidgetPropertyData it in allWidgets.Where(x => x.IsGroup == false))
            {
                WidgetViewModBase wdg = it as WidgetViewModBase;
                if(wdg==null)
                {
                    continue;
                }

                // load widget that is not in a group .
                if ((wdg.ParentID == Guid.Empty))
                {
                    bool bChecked = false;
                    IRegion targetObject = wdg.WidgetModel.WdgDom;
                    if (targetObject == null)
                        break;
                    //if the widget is already checked.
                    var selWidget = _widgetShowHideAction.TargetObjects.FirstOrDefault(x => x.Guid == targetObject.Guid);
                    if (selWidget != null)
                    {
                        bChecked = true;
                    }
                    TargetWidgets.Add(new WidgetNode(this, targetObject, wdg.Type, bChecked));
                }
            }

            //load all groups.
            foreach (IGroup group in _currentPage.Groups)
            {
                bool bChecked = false;
                var selGroup = _widgetShowHideAction.TargetObjects.FirstOrDefault(x => x.Guid == group.Guid);
                if (selGroup != null)
                {
                    bChecked = true;
                }
                TargetWidgets.Add(new WidgetNode(this, group, ObjectType.Group, bChecked));
            }

            _isCheckAll = !TargetWidgets.Any(x => x.IsSelected == false);

            WidgetList.AddRange(TargetWidgets);

            FirePropertyChanged("IsCheckAll");
            FirePropertyChanged("ShowHideType");
            FirePropertyChanged("AnimateType");
            FirePropertyChanged("AnimateTime");
            FirePropertyChanged("IsShowHideEnabled");
        }

        public void SetShowHideAction()
        {
            _widgetShowHideAction.SetAllVisibilityType(_showHideType);
            _widgetShowHideAction.SetAllAnimateType(_showHideAnimateType);
            _widgetShowHideAction.SetAllAnimateTime(_animateTime);
        }

        #endregion

        #region Private Fields

        private IPage _currentPage 
        {
            get { return SelectionService.GetCurrentPage().ActivePage; }
        }

        private PagelistTreeModel _model;
        private bool _canEdit;

        private IWidget _selectedWidget;
        private IInteractionOpenAction _widgetOpenAction;
        private IInteractionShowHideAction _widgetShowHideAction;

        private bool _noneChecked;
        private bool _pageChecked;
        private bool _linkChecked;

        private string _externalLink = String.Empty;
        private VisibilityType _showHideType = VisibilityType.Show;
        private ShowHideAnimateType _showHideAnimateType;
        private int _animateTime = 500;
        private bool _isCheckAll;
        private bool _isCheckAllAction;

        #endregion
    }
}
