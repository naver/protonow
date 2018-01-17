using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
//using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Events;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;

using UndoCompositeCommand = Naver.Compass.InfoStructure.CompositeCommand;
using System.Diagnostics;
using System.Windows.Resources;
using Naver.Compass.Service.CustomLibrary;
using Naver.Compass.Common.Helper;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel : EditPaneViewModelBase,
        ICommandSink, IPagePropertyData, IGroupOperation, ISupportUndo
    {
        #region Constructor
        public PageEditorViewModel(Guid pageGID)
        {
            //This is for normal page
            InitializePage(pageGID);
            InitializeCommonData();
             _copyList=new List<List<Guid>>();
        }
        public PageEditorViewModel()
        {
            //this is for inherit usage            
            _copyList = new List<List<Guid>>();
        }
        #endregion Constructor

        #region Private member
        protected PageEditorModel _model;
        protected BusyIndicatorContext _busyIndicator;
        protected Guid _pageGID;
        protected CommandSink _commandSink;
        protected ISelectionService _selectionService;
        protected SubscriptionToken AddNewWidgetSubToken;
        protected PageType _pageType = PageType.NormalPage;

        protected double widgetoffsetX = 0;
        protected double widgetoffsetY = 0;
        protected double countoffsetY = 0;
        protected Guid _curAdaptiveViewGID;
        protected bool _isThumbnailUpdate = true;

        protected int _copyTime;
        protected Guid _copyGID;
        protected List< List<Guid> > _copyList=null;
        #endregion Private member

        #region Model Data Binding Property

        internal PageEditorModel PageEditorModel
        {
            get { return _model; }
        }

        public Guid CurAdaptiveViewGID
        {
            get { return _curAdaptiveViewGID; }
        }

        //Is Data Changed
        public bool IsDirty
        {
            get { return _model.IsDirty; }
            set
            {
                if (_model.IsDirty != value)
                {
                    _model.IsDirty = value;
                    FirePropertyChanged("IsDirty");
                }
            }
        }
        //Page ID
        public Guid PageGID
        {
            get { return _pageGID; }
        }

        virtual public IPage ActivePage
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc.Document != null)
                {
                    return doc.Document.Pages.GetPage(_pageGID);

                }
                return null;
            }
            set
            {

            }
        }

        public PageType PageType
        {
            get { return _pageType; }
        }

        public List<IWidgetPropertyData> GetSelectedwidgets()
        {
            return _selectionService.GetSelectedWidgets();
        }

        public List<IWidgetPropertyData> GetAllWidgets()
        {
            return Items.ToList<IWidgetPropertyData>();
        }

        //All Widgets in the Page;
        public ObservableCollection<WidgetViewModBase> items;
        public ObservableCollection<WidgetViewModBase> Items
        {
            get
            {
                if (items == null)
                {
                    items = new ObservableCollection<WidgetViewModBase>();
                    items.CollectionChanged -= items_CollectionChanged;
                    //Add widget from DOM to UI
                    //AsyncLoadAllWidgets(true);                    
                    return items;
                }
                return items;
            }
        }
        public void items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StartMsgVisibility = Visibility.Collapsed;
            _isThumbnailUpdate = true;
            _ListEventAggregator.GetEvent<WidgetsNumberChangedEvent>().Publish(PageGID);
        }


        IInputElement _editorCanvas = null;
        public IInputElement EditorCanvas
        {
            get { return _editorCanvas; }
            set
            {
                if (_editorCanvas != value)
                {
                    _editorCanvas = value;
                    FirePropertyChanged("EditorCanvas");
                }
            }
        }

        private double _displayX;
        public double DisplayX
        {
            set
            {
                if (_displayX != value)
                {
                    _displayX = value;
                }
            }
        }

        private double _displayY;
        public double DisplayY
        {
            set
            {
                if (_displayY != value)
                {
                    _displayY = value;
                }
            }
        }

        private double _displayWidth;
        public double DisplayWidth
        {
            set
            {
                if (_displayWidth != value)
                {
                    _displayWidth = value;
                }
            }
        }

        private double _displayHeight;
        public double DisplayHeight
        {
            set
            {
                if (_displayHeight != value)
                {
                    _displayHeight = value;
                }
            }
        }

        private Cursor _cursor;
        public Cursor CanvasCursor
        {
            get
            {
                return _cursor;
            }
            set
            {

                if (_cursor == null || !_cursor.Equals(value))
                {
                    _cursor = (Cursor)value;
                    FirePropertyChanged("CanvasCursor");
                }
            }
        }

        private bool _isFormatCursor = false;
        public bool IsFormatCursor
        {
            get
            {
                return _isFormatCursor;
            }
            set
            {

                if (_isFormatCursor != (bool)value)
                {
                    _isFormatCursor = (bool)value;
                    FirePropertyChanged("IsFormatCursor");
                }
            }
        }

        //Get selected widgets bounding rect.
        public Rect BoundingRect
        {
            get
            {
                double left = GetLeftMost();
                double top = GetTopMost();
                double width = GetRightMost() - left;
                double height = GetBottomMost() - top;
                return new Rect(left, top, width, height);
            }
        }

        #region Mouse over interaction objects
        private Thickness _objectPosition = new Thickness(0, 0, 0, 0);
        //Position of mouser over object in interaction
        public Thickness InteractionObjectPosition
        {
            get
            {
                return _objectPosition;
            }
            set
            {
                if (_objectPosition != value)
                {
                    _objectPosition = value;
                    FirePropertyChanged("InteractionObjectPosition");
                }
            }
        }

        private double _objectWidth;
        public double InteractionObjectWidth
        {
            get
            {
                return _objectWidth;
            }
            set
            {
                if (_objectWidth != value)
                {
                    _objectWidth = value;
                    FirePropertyChanged("InteractionObjectWidth");
                    FirePropertyChanged("InteractionObjectCenterX");
                }
            }
        }

        private double _objectHeight;
        public double InteractionObjectHeight
        {
            get
            {
                return _objectHeight;
            }
            set
            {
                if (_objectHeight != value)
                {
                    _objectHeight = value;
                    FirePropertyChanged("InteractionObjectHeight");
                    FirePropertyChanged("InteractionObjectCenterY");
                }
            }
        }
        public double InteractionObjectCenterX
        {
            get
            {
                return _objectWidth * 0.5;
            }
        }
        public double InteractionObjectCenterY
        {
            get
            {
                return _objectHeight * 0.5;
            }
        }

        private double _objectRotateAngle;
        public double InteractionObjectRotateAngle
        {
            get
            {
                return _objectRotateAngle;
            }
            set
            {
                if (_objectRotateAngle != value)
                {
                    _objectRotateAngle = value;
                    FirePropertyChanged("InteractionObjectRotateAngle");
                }
            }
        }
        #endregion

        #endregion Model Data Binding Property

        #region Page ui binding property
        protected Visibility _startMsgVisibility = Visibility.Collapsed;
        public virtual Visibility StartMsgVisibility
        {
            get { return Visibility.Collapsed; ; }
            set
            {
                if (_startMsgVisibility != value)
                {
                    _startMsgVisibility = value;
                    FirePropertyChanged("StartMsgVisibility");
                }
            }
        }
        public virtual String StartMsg
        {
            get
            {
                return "이 영역에 마스터를 만들어 보세요";
            }

        }
        #endregion

        #region Async Function--Load Widgets
        protected async void AsyncLoadAllWidgets(bool bIsAutoScale=false)
        {

            //Application.Current.MainWindow.IsEnabled = false;
            _busyIndicator.Progress = 100;
            _busyIndicator.CanStop = false;
            _busyIndicator.IsShow = true;

            //load page data and create all images
            await Task.Factory.StartNew(LoadWidgets);
            await Task.Factory.StartNew(LoadAllGroups);
            if(bIsAutoScale==true)
            {
                AutoAdjustScale();
            }

            items.CollectionChanged += items_CollectionChanged;
            _busyIndicator.Progress = 100;
            _busyIndicator.IsShow = false;
            if (_targetLoadingWdgGID != Guid.Empty)
            {
                SelectTargetComponent();
                _targetLoadingWdgGID = Guid.Empty;
            }

            if(this.PageType!= PageType.NormalPage)
            {
                _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Publish(this.PageGID);
            }

        }
        virtual protected void LoadWidgets()
        {
            if (_model.Widgets == null)
            {
                return;
            }

            //Async Add widget from DOM to UI
            foreach (IRegion it in _model.Widgets)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
                {

                    WidgetViewModBase vmItem = WidgetFactory.CreateWidget(it);
                    vmItem.ChangeCurrentPageView(_model.ActivePageView);
                    if (vmItem == null)
                    {
                        return;
                    }
                    Items.Add(vmItem);
                    vmItem.ParentPageVM = this;

                }));
            }
        }
        private void LoadAllGroups()
        {
            if (_model.Groups == null || _model.Groups.Count <= 0)
            {
                return;
            }

            //Create the external Groups
            List<WidgetViewModBase> GroupVMs = new List<WidgetViewModBase>();
            foreach (IGroup it in _model.Groups)
            {
                if (it.ParentGroup != null)
                    continue;
                WidgetViewModBase vmItem = WidgetFactory.CreateGroup(it);
                if (vmItem == null)
                {
                    return;
                }
                vmItem.IsGroup = true;
                GroupVMs.Add(vmItem);
            }
            if (GroupVMs.Count <= 0)
            {
                return;
            }

            //Initialize all external groups and all group's children.
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
            {

                foreach (WidgetViewModBase wdg in Items)
                {
                    foreach (GroupViewModel group in GroupVMs)
                    {
                        if (true == group.IsChild(wdg.widgetGID, false))
                        {
                            wdg.ParentID = group.widgetGID;
                            wdg.IsGroup = false;
                            group.AddChild(wdg);
                            continue;
                        }
                    }
                }
            }));

            //UI Render the Groups
            foreach (GroupViewModel group in GroupVMs)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
                {
                    group.Status = GroupStatus.UnSelect;
                    Items.Add(group);
                    group.Refresh();
                }));
            }


        }
        #endregion Function--Load Widgets

        #region Public Function
        virtual public void Open()
        {
            try
            {
                _model.OpenPage();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"));
                NLogger.Error("Open page failed: " + e.Message);
            }
        }
        virtual public void Close(Guid PageGID)
        {
            //Create the Preview nail image
            //CreatePreviewImage();
            try
            {
                //Close Child Page
                foreach (WidgetViewModBase item in Items.Where(a => (a is ToastViewModel || a is HamburgerMenuViewModel || a is DynamicPanelViewModel) && a.IsChildPageOpened == false))
                {
                    if (item is ToastViewModel)
                    {
                        IPage page = (item as ToastViewModel).Toast.ToastPage;
                        if (page.IsOpened)
                        {
                            page.Close();
                        }
                    }
                    else if (item is HamburgerMenuViewModel)
                    {
                        IPage page = (item as HamburgerMenuViewModel).Widget.MenuPage;
                        if (page.IsOpened)
                        {
                            page.Close();
                        }
                    }
                    else
                    {
                        foreach (IPage page in (item as DynamicPanelViewModel).DynamicPanel.PanelStatePages)
                        {
                            if (page.IsOpened)
                            {
                                page.Close();
                            }
                        }
                    }
                }

                //Close Current page
                _model.ClosePage();

                //CreateCustomWidgetIcon(true);
            }
            catch
            {
               // Logger.fatal("close page error:null reference");
            }

        }
        public void Update()
        {
            UpdateTitle();
            UpdateContent();
        }
        virtual public void UpdateTitle()
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null)
            {
                IPage pageItem = doc.Document.Pages.GetPage(_pageGID);
                if (pageItem != null)
                {
                    Title = pageItem.Name;
                }
            }
            else
            {
                Title = _pageGID.ToString();
            }
        }
        public void UpdateContent()
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null)
            {

            }
        }
        public void AddWidgetItem(WidgetViewModBase it)
        {
            // Add to DOM
            _model.AddClonedItem2Dom(it.WidgetModel.WdgDom);
            Items.Add(it); 
        }
        public void DeleteItem(WidgetViewModBase it)
        {
            // Delete widget or group in VM and DOM
            if (it.IsGroup == false)
            {
                Guid gid = it.widgetGID;
                
                if ((it is DynamicPanelViewModel) || (it is HamburgerMenuViewModel) || (it is ToastViewModel))
                {
                    _ListEventAggregator.GetEvent<ClosePageEvent>().Publish(it.widgetGID);
                }
                _model.RemoveWidgetFromDom(it);
                Items.Remove(it);
            }
            else
            {
                // Delete group will delete all subgroup and widgets in this group.
                _model.DeleteGroup(it.widgetGID);

                foreach (WidgetViewModBase target in (it as GroupViewModel).WidgetChildren)
                {
                    if (target.ParentID == it.widgetGID)
                    {
                        items.Remove(target);
                        if ((target is DynamicPanelViewModel) || (target is HamburgerMenuViewModel))
                        {
                            _ListEventAggregator.GetEvent<ClosePageEvent>().Publish(target.widgetGID);
                        }
                    }
                }

                items.Remove(it);

                
            }
        }
        /// <summary>
        /// Delete all old widgets , and then create all new widgets
        /// </summary>
        /// <param name="oldWidgets">Old widgets to be replaced</param>
        /// <param name="newWidgets">New widgets to replaced with</param>
        public void ReplaceWidget(List<WidgetViewModBase> oldWidgets, List<WidgetViewModBase> newWidgets)
        {
            if (oldWidgets.Any(w => w.IsGroup)
                || newWidgets.Any(w => w.IsGroup)
                || newWidgets.Any(w => w.ParentID != Guid.Empty))
            {
                ///old widget cannot be a group item
                ///and new widget cannot be a group item nor in a group
                return;
            }

            var cmds = new UndoCompositeCommand();
            ///to remove old widgets before
            foreach (var widget in oldWidgets)
            {
                /// TODO: At present, the item in group cannot be deleted. Update this when this feature is implemented
                if (widget.ParentID == Guid.Empty)
                {
                    var delCmd = new DeleteWidgetCommand(this, widget);
                    cmds.AddCommand(delCmd);
                    Items.Remove(widget);
                    _model.RemoveWidgetFromDom(widget);
                }
            }

            foreach (var widget in newWidgets)
            {
                widget.ZOrder = Items.Count - Items.OfType<GroupViewModel>().Count<GroupViewModel>();
                var crtCmd = new CreateWidgetCommand(this, widget);
                cmds.AddCommand(crtCmd);

                widget.ChangeCurrentPageView(_model.ActivePageView);
                widget.IsSelected = true;
                Items.Add(widget);
            }

            //Re-order all widgets' Z-order
            ReOrderAllWidgets(cmds);

            if (cmds.Count > 0)
            {
                cmds.DeselectAllWidgetsFirst();
                _undoManager.Push(cmds);
            }
        }
        /// <summary>
        /// Set widgets' adaptive view of  child page the same as widget itself (hamburger/flicking panel/toast).
        /// </summary>
        public void SetDefaultAdaptive()
        {
            IPagePropertyData page = SelectionService.GetCurrentPage();
            if (page != null && (this.PageType != PageType.NormalPage))
            {
                _curAdaptiveViewGID = page.CurAdaptiveViewGID;
                //_model.OpenPage();
                _model.SetActivePageView(_curAdaptiveViewGID);
            }
        }
        public void SetAdaptiveView(Guid id)
        {
            if (id == _curAdaptiveViewGID)
            {
                return;
            }
            _curAdaptiveViewGID = id;
            CheckAdaptiveView(id);
        }
        public void PlaceWidgets2View(List<Guid> widgetlist)
        {
            foreach (Guid item in widgetlist)
            {
                WidgetViewModBase targetWidget = Items.FirstOrDefault(a => a.WidgetID == item);

                //Change widget Status.
                if (targetWidget == null)
                    break;

                if (targetWidget.IsGroup)
                {
                    (targetWidget as GroupViewModel).Status = GroupStatus.Selected;
                }
                else
                {
                    targetWidget.IsSelected = true;
                }

                //Place widget
                PlaceWidget(targetWidget);

                //group external border show
                if (targetWidget.RealParentGroupGID != Guid.Empty)
                {                    
                    GroupViewModel group = Items.FirstOrDefault(a => a.WidgetID == targetWidget.ParentID) as GroupViewModel;
                    if (group != null)
                    {
                        group.Status = GroupStatus.Edit;
                    }  
                }                
            }
        }
        public void UnplaceWidgetsFromView(List<Guid> widgetlist)
        {
            foreach (Guid item in widgetlist)
            {
                WidgetViewModBase targetWidget = Items.FirstOrDefault(a => a.WidgetID == item);

                //Change widget Status.
                if (targetWidget == null)
                    break;
                //Place widget
                UnplaceWidget(targetWidget);

                if (targetWidget.IsGroup)
                {
                    (targetWidget as GroupViewModel).Status = GroupStatus.UnSelect;
                }
                else
                {
                    targetWidget.IsSelected = false;
                }
            }
        }
        public void PlaceWidget(WidgetViewModBase it)
        {
            if (it.IsGroup)
            {
                GroupViewModel group = it as GroupViewModel;
                foreach (var item in group.WidgetChildren)
                {
                    PlaceWidget(item);
                }
                group.IsGroupShowInView2Adaptive = true;
            }
            else
            {
                _model.PlaceWidget2View(it.WidgetModel.WdgDom);
                it.ChangeCurrentPageView(_model.ActivePageView);
            }
        }
        public void UnplaceWidget(WidgetViewModBase it)
        {
            if (it.IsGroup)
            {
                GroupViewModel group = it as GroupViewModel;
                foreach (var item in group.WidgetChildren)
                {
                    UnplaceWidget(item);
                }
                (it as GroupViewModel).IsGroupShowInView2Adaptive = false;
            }
            else
            {
                _model.UnplaceWidgetFromView(it.WidgetModel.WdgDom);
                it.ChangeCurrentPageView(_model.ActivePageView);
            }

        }
        public bool GetIsThumbnailUpdate()
        {
            return _isThumbnailUpdate;
        }
        public void SetIsThumbnailUpdate(bool bIsDirty)
        {
            _isThumbnailUpdate = bIsDirty;
        }
        public void AutoAdjustScale()
        {
            double scaleH = 1d, scaleW = 1d; ;

            //No Device List
            if (EditableHeight==0|| EditableWidth==0)
            {
                double actulaH = this.GetActualPageHeight();
                double actulaW = this.GetActualPageWidth();
                if(actulaH<=0 || actulaW<=0)
                {
                    EditorScale = 1;
                    return;
                }

                if (_displayHeight < actulaH )
                {
                    scaleH = (_displayHeight - 20) / actulaH;
                }
                if (_displayWidth < actulaW)
                {
                    scaleW = (_displayWidth - 20) / actulaW;
                }                
                EditorScale = Math.Min(scaleH, scaleW);
                return;
            }

            //Device list is not empty            
            if(_displayHeight< EditableHeight && _displayHeight>0)
            {
                scaleH = (_displayHeight-20) / EditableHeight;
            }
            if(_displayWidth< EditableWidth && _displayWidth > 0)
            {
                scaleW = (_displayWidth - 20) / EditableWidth;
            }
            EditorScale = Math.Min(scaleH, scaleW);

        }
        public void InitialLoadWidget()
        {
            //Add widget from DOM to UI
            AsyncLoadAllWidgets(true);
        }



        #endregion Public Function

        #region Page Event Subscriber Handler
        //Close Command
        private DelegateCommand<object> _closeCommand = null;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new DelegateCommand<object>(OnClose, CanClose);
                }
                return _closeCommand;
            }
        }
        private bool CanClose(object obj)
        {
            return true;
        }
        private void OnClose(object obj)
        {
            _ListEventAggregator.GetEvent<ClosePageEvent>().Publish(_pageGID);
        }

        //Active Command
        private DelegateCommand<object> _activeCommand = null;
        public ICommand ActiveCommand
        {
            get
            {
                if (_activeCommand == null)
                {
                    _activeCommand = new DelegateCommand<object>(OnActived);
                }
                return _activeCommand;
            }
        }
        private void OnActived(object obj)
        {
            //_ListEventAggregator.GetEvent<ClosePageEvent>().Publish(_pageGID);
            //IsSelected = true;
            IsActive = true;
            if (EditorCanvas != null)
            {
                EditorCanvas.Focus();
            }

        }

        private void OnNewWidgetHandler(object obj)
        {
            DataObject data = obj as DataObject;
            if (data != null)
            {
                AddItemFromDataobject(data);
            }
        }

        //Select all widgets Command
        public DelegateCommand<object> _selectAllCommand = null;
        public ICommand SelectAllCommand
        {
            get
            {
                if (_selectAllCommand == null)
                {
                    _selectAllCommand = new DelegateCommand<object>(OnSelectAll);
                }
                return _selectAllCommand;
            }
        }
        private void OnSelectAll(object obj)
        {
            List<IWidgetPropertyData> SelWidgets = new List<IWidgetPropertyData>();
            foreach (WidgetViewModBase wdg in Items)
            {
                if (wdg.ParentID == Guid.Empty && wdg.IsShowInPageView2Adaptive == true)
                {
                    SelWidgets.Add(wdg);
                }
            }
            _selectionService.RegisterWidgets(SelWidgets);

            foreach (WidgetViewModBase wdg in Items)
            {
                if (wdg.ParentID != Guid.Empty)
                {
                    wdg.IsSelected = false;
                }
                else if (wdg.IsShowInPageView2Adaptive == true)
                {
                    if (wdg.IsGroup == false)
                    {
                        wdg.IsSelected = true;
                    }
                    else
                    {
                        (wdg as GroupViewModel).Status = GroupStatus.Selected;
                    }
                }
            }
        }

        public DelegateCommand<object> _arrowKeyUpCommand = null;
        public DelegateCommand<object> ArrowKeyUpCommand
        {
            get
            {
                if (_arrowKeyUpCommand == null)
                {
                    _arrowKeyUpCommand = new DelegateCommand<object>(OnKeyUp);
                }
                return _arrowKeyUpCommand;
            }
        }
        private void OnKeyUp(object obj)
        {
            //Ctrl Key
            KeyEventArgs e = obj as KeyEventArgs;
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                ProcessKeyDown(ModifierKeys.Control, false);
                return;
            }

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                ProcessKeyDown(ModifierKeys.Shift, false);
            }

            //Arrow Key
            if (_arrowKeyDownTargetWidgetList.Count > 0)
            {
                UndoCompositeCommand cmds = new UndoCompositeCommand();
                foreach (WidgetViewModBase item in _arrowKeyDownTargetWidgetList)
                {
                    item.PropertyMementos.SetPropertyNewValue("Left", item.Left);
                    item.PropertyMementos.SetPropertyNewValue("Top", item.Top);

                    PropertyChangeCommand cmd = new PropertyChangeCommand(item, item.PropertyMementos);
                    cmds.AddCommand(cmd);
                }

                if (_arrowKeyDownUpdateGroupList.Count > 0)
                {
                    List<Guid> updateGroupList = new List<Guid>(_arrowKeyDownUpdateGroupList);
                    UpdateGroupCommand cmd = new UpdateGroupCommand(this, updateGroupList);
                    cmds.AddCommand(cmd);
                }

                PushToUndoStack(cmds);
            }
            _arrowKeyDownTargetWidgetList.Clear();
            _arrowKeyDownUpdateGroupList.Clear();


        }

        public DelegateCommand<object> _arrowKeyDownCommand = null;
        public DelegateCommand<object> ArrowKeyDownCommand
        {
            get
            {
                if (_arrowKeyDownCommand == null)
                {
                    _arrowKeyDownCommand = new DelegateCommand<object>(OnKeyDown);
                }
                return _arrowKeyDownCommand;
            }
        }
        private void OnKeyDown(object obj)
        {
            //Ctrl Key
            KeyEventArgs e = obj as KeyEventArgs;
            if ((e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl))
            {
                ProcessKeyDown(ModifierKeys.Control, true);
                return;
            }

            if ((e.Key == Key.LeftShift || e.Key == Key.RightShift))
            {
                ProcessKeyDown(ModifierKeys.Shift, true);
                return;
            }

            //Arrow Key
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                ProcessAdjustWidgetsSize(obj);
            }
            else
            {
                ProcessMovewidgets(obj);
            }
        }
        private void ProcessKeyDown(ModifierKeys keyType, bool bIsCtrlPressed)
        {
            foreach (WidgetViewModBase wdg in Items)
            {
                if (wdg.IsSelected == true && wdg.IsLocked == false && wdg.IsGroup == false)
                {
                    if (keyType == ModifierKeys.Control)
                    {
                        wdg.IsCtrlPressed = bIsCtrlPressed;
                    }
                    else if (keyType == ModifierKeys.Shift)
                    {
                        wdg.IsShiftPressed = bIsCtrlPressed;
                    }
                }
                else if (wdg.IsSelected == true && wdg.IsLocked == false && wdg.IsGroup == true)
                {
                    GroupViewModel gVM = wdg as GroupViewModel;
                    if (gVM == null)
                    {
                        return;
                    }
                    foreach (WidgetViewModBase child in gVM.WidgetChildren)
                    {
                        if (keyType == ModifierKeys.Control)
                        {
                            child.IsCtrlPressed = bIsCtrlPressed;
                        }
                        else if (keyType == ModifierKeys.Shift)
                        {
                            child.IsShiftPressed = bIsCtrlPressed;
                        }
                    }
                }
            }
        }
        private void ProcessMovewidgets(object obj)
        {
            KeyEventArgs e = obj as KeyEventArgs;

            bool handled = false;
            switch (e.Key)
            {
                case Key.Left:
                    {
                        MoveSelectedWidget(-1, 0, e.IsRepeat);
                        handled = true;
                        break;
                    }
                case Key.Right:
                    {
                        MoveSelectedWidget(1, 0, e.IsRepeat);
                        handled = true;
                        break;
                    }
                case Key.Up:
                    {
                        MoveSelectedWidget(0, -1, e.IsRepeat);
                        handled = true;
                        break;
                    }
                case Key.Down:
                    {
                        MoveSelectedWidget(0, 1, e.IsRepeat);
                        handled = true;
                        break;
                    }
                default:
                    break;
            }

            if (handled && _selectionService.WidgetNumber > 0)
            {
                // Arrow key down has been handled.
                e.Handled = handled;
            }
        }
        private void ProcessAdjustWidgetsSize(object obj)
        {
            KeyEventArgs e = obj as KeyEventArgs;

            switch (e.Key)
            {
                case Key.Left:
                    WidgetsDecreaseWidthCommandHandler(!e.IsRepeat);
                    e.Handled = true;
                    break;
                case Key.Right:
                    WidgetsIncreaseWidthCommandHandler(!e.IsRepeat);
                    e.Handled = true;
                    break;
                case Key.Up:
                    WidgetsIncreaseHeightCommandHandler(!e.IsRepeat);
                    e.Handled = true;
                    break;
                case Key.Down:
                    WidgetsDecreaseHeightCommandHandler(!e.IsRepeat);
                    e.Handled = true;
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }
        private void MoveSelectedWidget(int offsetX, int offsetY, bool isRepeat)
        {
            bool pushToUndoStack = (isRepeat == false || (isRepeat == true && _arrowKeyDownTargetWidgetList.Count <= 0));

            foreach (WidgetViewModBase wdg in Items)
            {


                if (wdg.IsSelected == true && wdg.IsLocked == false)
                {
                    if (pushToUndoStack)
                    {
                        wdg.CreateNewPropertyMementos();

                        wdg.PropertyMementos.AddPropertyMemento(new PropertyMemento("Left", wdg.Raw_Left, wdg.Raw_Left));
                        wdg.PropertyMementos.AddPropertyMemento(new PropertyMemento("Top", wdg.Raw_Top, wdg.Raw_Top));

                        _arrowKeyDownTargetWidgetList.Add(wdg);

                        if (wdg.IsGroup)
                        {
                            _arrowKeyDownUpdateGroupList.Add(wdg.WidgetID);
                        }
                        else if (wdg.ParentID != Guid.Empty)
                        {
                            _arrowKeyDownUpdateGroupList.Add(wdg.ParentID);
                        }
                    }

                    wdg.Raw_Left = wdg.Left + offsetX;
                    wdg.Raw_Top = wdg.Top + offsetY;
                }
            }

            foreach (Guid guid in _arrowKeyDownUpdateGroupList)
            {
                UpdateGroup(guid);
            }
        }
        private List<WidgetViewModBase> _arrowKeyDownTargetWidgetList = new List<WidgetViewModBase>();
        private List<Guid> _arrowKeyDownUpdateGroupList = new List<Guid>();

        //Right clicked event handler for set context menu
        public DelegateCommand<object> _rightClickedCommand = null;
        public DelegateCommand<object> SetContextMenuCommand
        {
            get
            {
                if (_rightClickedCommand == null)
                {
                    _rightClickedCommand = new DelegateCommand<object>(OnPreSetContextMenu);
                }
                return _rightClickedCommand;
            }
        }

        #endregion Page Event Subscriber Handler

        #region EditPaneViewModelBase Override function
        public bool CommonEventNotify(string sEvents, object para)
        {
            if (sEvents.CompareTo("FormatPaint") == 0)
            {

                _ListEventAggregator.GetEvent<CancelFormatPaintEvent>().Publish(para);
            }
            return false;
        }
        //Drag object to add widget.
        override protected void OnItemAdded(object obj)
        {
            DropInfo info = obj as DropInfo;
            if (info == null)
                return;

            //Drag file to work area.
            if (info.e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])info.e.Data.GetData(DataFormats.FileDrop);
                try
                {
                    int i = 0;
                    foreach (string path in files)
                    {
                        if (CommonFunction.IsImageFilePath(path))
                        {
                            WidgetViewModBase wdg = AddWidgetItem(WidgetType.Image, ShapeType.None, info.position.X + i * 20, info.position.Y + i * 20, 100, 100);
                            ImageWidgetViewModel imgWdg = wdg as ImageWidgetViewModel;
                            if (imgWdg == null)
                            {
                                return;
                            }
                            imgWdg.ImportImg(path);
                            i++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return;
                }
            }            
            else
            {
                AddObjectsFromData(info.e.Data, AddWidgetType.DrgAdd, info.position);
            }
        }
        // double click to add widget.
        virtual protected void AddItemFromDataobject(DataObject data)
        {
            //Deselect all in current page
            DeselectAll();


            System.Windows.Point pt = new System.Windows.Point();
            AddObjectsFromData(data, AddWidgetType.DoubleClickAdd, pt);
        }

        private void OnItemListDelete(List<WidgetViewModBase> listInfo, UndoCompositeCommand cmds = null)
        {
            // obj is null means this is called by RemoveItemCommand, so we handle undo/redo here.
            bool bAddToUndo = false;
            if(cmds == null)
            {
                cmds = new UndoCompositeCommand();
                bAddToUndo = true;
            }
            DeleteWidgetInGroupCommand delWdgInGrpCmd = new DeleteWidgetInGroupCommand();

            foreach (WidgetViewModBase it in listInfo)
            {
                // Delete top widgets or groups
                if (it.ParentID == Guid.Empty)
                {
                    if (!it.IsGroup)
                    {
                        DeleteWidgetCommand cmd = new DeleteWidgetCommand(this, it);
                        cmds.AddCommand(cmd);
                    }
                    else
                    {
                        DeleteGroupCommand cmd = new DeleteGroupCommand(this, it as GroupViewModel);
                        cmds.AddCommand(cmd);
                    }

                    DeleteItem(it);
                }
                else
                {
                    // Delete widgets in group
                    GroupViewModel groupVM = items.OfType<GroupViewModel>().Where(c => c.WidgetID == it.ParentID).FirstOrDefault<GroupViewModel>();
                    if (groupVM != null)
                    {
                        IGroup parentGroup = groupVM.ExternalGroup;
                        if (parentGroup == null)
                        {
                            return;
                        }

                        DeleteWidgetInGroupSubCommand cmd = new DeleteWidgetInGroupSubCommand(this, it);
                        delWdgInGrpCmd.AddCommand(cmd);

                        it.ParentID = Guid.Empty;
                        DeleteItem(it);

                        if (_model.Groups.Contains(parentGroup.Guid))
                        {
                            // The old group still exists, recreate this group
                            CreateGroupRender(parentGroup);
                        }
                        else
                        {
                            // The old group doesn't exist in DOM, which means the old group is split to its child groups or widgets.

                            // Set the rest widgets VM ParentID to Guid.Empty.
                            foreach (WidgetViewModBase childWdgItem in groupVM.WidgetChildren)
                            {
                                childWdgItem.ParentID = Guid.Empty;
                            }

                            // Delete old group VM.
                            groupVM.IsSelected = false;
                            _selectionService.RemoveWidget(groupVM);
                            Items.Remove(groupVM);

                            // Then create sub group if necessary
                            foreach (IGroup childGroup in parentGroup.Groups)
                            {
                                CreateGroupRender(childGroup);
                            }
                        }
                    }
                }
            }

            // There is deleting widget in group undo command
            if (delWdgInGrpCmd.Count > 0)
            {
                cmds.AddCommand(delWdgInGrpCmd);
            }
            
            if (cmds.Count > 0)
            {
                //Re-order all widgets' Z-order
                ReOrderAllWidgets(cmds);

                cmds.DeselectAllWidgetsFirst();

                if (bAddToUndo)
                {
                    _undoManager.Push(cmds);
                }
            }
        }
        override protected void OnItemRemoved(object obj )
        {
            if (obj == null)
            {
                RemoveSelectedWidget();
            }
            else
            {
                WidgetViewModBase wdgVM = obj as WidgetViewModBase;
                if (wdgVM == null)
                {
                    return;
                }
                //Remove item from selection service
                wdgVM.IsSelected = false;

                //Remove it from dom    
                DeleteItem(wdgVM);

                // obj is not null means this is called by CutCommand, we have to handle redo/undo in CutCommanddHandler.
            }
        }

        private void RemoveSelectedWidget(UndoCompositeCommand  cmds = null)
        {
            List<WidgetViewModBase> ToRemoveList = new List<WidgetViewModBase>();
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.IsSelected == true)
                {
                    //Remove item from selection service
                    wdgItem.IsSelected = false;

                    //construct the  widget list to be removed
                    ToRemoveList.Add(wdgItem);
                }
            }

            OnItemListDelete(ToRemoveList, cmds);
        }
        override protected void OnPannelSelected(bool bIsSelected)
        {
            if(_model.IsPageOpen()==false)
            {
                if (AddNewWidgetSubToken != null)
                {
                    _ListEventAggregator.GetEvent<NewWidgetEvent>().Unsubscribe(AddNewWidgetSubToken);
                    AddNewWidgetSubToken = null;
                }
                return;
            }
            //Update the selection service data
            if (bIsSelected == true)
            {
                //Re-load the selected widget when page is re-selected
                List<IWidgetPropertyData> ToAddList = new List<IWidgetPropertyData>();
                foreach (WidgetViewModBase wdgItem in Items)
                {
                    if (wdgItem.IsSelected == true)
                    {
                        ToAddList.Add(wdgItem);
                    }
                    if (wdgItem is ImageWidgetViewModel)
                    {
                        (wdgItem as ImageWidgetViewModel).UnloadImage(false);
                    }
                }
                //Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
                //{
                //    foreach (WidgetViewModBase wdgItem in Items)
                //    {
                //        if (wdgItem is ImageWidgetViewModel)
                //        {
                //            (wdgItem as ImageWidgetViewModel).UnloadImage(false);
                //        }
                //    }
                //}));

                //register the selected page
                _selectionService.RegisterPage(this, ToAddList);

                //load adaptivePanel state
                IsAdaptivePanelOpened = _adaptiveModel.IsAdaptiveOpen;

                if (AddNewWidgetSubToken == null)
                {
                    AddNewWidgetSubToken = _ListEventAggregator.GetEvent<NewWidgetEvent>().Subscribe(OnNewWidgetHandler);
                }
            }
            else
            {

                SaveEditingWidget();
                CancelEditHamburgerPage();

                //Create the Preview nail image and custom widget icon
                if(this.PageType==PageType.NormalPage)
                {
                    CreatePreviewImage();
                    if (_document!=null)
                    {
                        if (_document.DocumentType == DocumentType.Library)
                        {
                            if (IsUseThumbnailAsIcon == true)
                            {
                                CreateCustomWidgetIcon(true);
                            }                            
                        }
                    }
                    _isThumbnailUpdate = false;
                }


                _selectionService.RemovePage(this);

                if (AddNewWidgetSubToken != null)
                {
                    _ListEventAggregator.GetEvent<NewWidgetEvent>().Unsubscribe(AddNewWidgetSubToken);
                    AddNewWidgetSubToken = null;
                }

                //set widget re-selecting invalid when switch to a exist page.
                //it's a illegal function, maybe a restore is necessary.........
               

                bool bIsContainerImg = false;
                foreach (WidgetViewModBase it in items)
                {
                    //set widget re-selecting invalid when switch to a exist page.
                    //it's a illegal function, maybe a restore is necessary.........

                    if (it.IsGroup)
                    {
                        (it as GroupViewModel).Status = GroupStatus.UnSelect;
                    }
                    else
                    {
                        it.IsSelected = false;
                    }                    

                    if (it is ImageWidgetViewModel)
                    {
                        (it as ImageWidgetViewModel).UnloadImage(true);
                        bIsContainerImg = true;
                    }
                }
                if (bIsContainerImg == true)
                {
                    (_editorCanvas as DesignerCanvas).UpdateLayout();
                }
                //this.items.Clear();
            }

            SetDefaultAdaptive();
            //Keep the selection UI

        }
        #endregion

        #region Private function
        private void SaveEditingWidget()
        {
            bool bHaswidgetInEditMode = false;
            foreach (WidgetViewModBase wdgItem in Items)
            {
                if (wdgItem.CanEdit == true)
                {
                    bHaswidgetInEditMode = true;
                    break;
                }
            }

            if (bHaswidgetInEditMode)
            {
                EditorCanvas.Focus();
            }

        }
        private WidgetViewModBase AddWidgetItem(WidgetType type, ShapeType flowType, double x, double y, int w, int h)
        {
            IWidget widget = _model.AddWidgetItem2Dom(type, flowType, x, y, w, h);
            if (widget == null)
                return null;
            widget.WidgetStyle.Z = Items.Count - Items.OfType<GroupViewModel>().Count<GroupViewModel>();
            WidgetViewModBase widgetVM = InsertWidget2Canvas(widget);
            if (widgetVM == null)
            {
                return widgetVM;
            }
            widgetVM.ParentPageVM = this;

            // Undo/Redo
            UndoCompositeCommand cmds = new UndoCompositeCommand();
            cmds.DeselectAllWidgetsFirst();
            cmds.AddCommand(new CreateWidgetCommand(this, widgetVM));
            cmds.MoveFocusToEditCanvasAtEnd();
            _undoManager.Push(cmds);
            return widgetVM;
        }
        private WidgetViewModBase AddMasterItem(Guid masterPageGuid, double x, double y, UndoCompositeCommand cmds = null)
        {

            IMaster master = _model.AddWidgetItem2Dom(masterPageGuid, x, y);
            if (master == null)
                return null;

            master.MasterStyle.Z = Items.Count - Items.OfType<GroupViewModel>().Count<GroupViewModel>();
            WidgetViewModBase widgetVM = InsertWidget2Canvas(master);
            if (widgetVM == null)
            {
                return widgetVM;
            }
            widgetVM.ParentPageVM = this;

            // Undo/Redo
            if(cmds == null)
            {
                cmds = new UndoCompositeCommand();
                cmds.DeselectAllWidgetsFirst();
                cmds.AddCommand(new CreateWidgetCommand(this, widgetVM));
                cmds.MoveFocusToEditCanvasAtEnd();
                _undoManager.Push(cmds);
            }
            else
            {
                //Convert to master undo command.
                cmds.DeselectAllWidgetsFirst();
                cmds.AddCommand(new CreateWidgetCommand(this, widgetVM));
                cmds.MoveFocusToEditCanvasAtEnd();
            }
            
            return widgetVM;
        }
        private WidgetViewModBase AddLineWidgetItem(Orientation lineType, double x, double y, int w, int h)
        {
            IWidget widget = _model.AddLineItem2Dom(lineType, x, y, w, h);
            if (widget == null)
                return null;
            widget.WidgetStyle.Z = Items.Count - Items.OfType<GroupViewModel>().Count<GroupViewModel>();
            WidgetViewModBase widgetVM = InsertWidget2Canvas(widget);
            if (widgetVM == null)
            {
                return widgetVM;
            }

            // Undo/Redo
            UndoCompositeCommand cmds = new UndoCompositeCommand();
            cmds.DeselectAllWidgetsFirst();
            cmds.AddCommand(new CreateWidgetCommand(this, widgetVM));
            cmds.MoveFocusToEditCanvasAtEnd();
            _undoManager.Push(cmds);
            return widgetVM;
        }
        private WidgetViewModBase InsertWidget2Canvas(IRegion widget,bool isSelected=true)
        {
            WidgetViewModBase vmItem = WidgetFactory.CreateWidget(widget);
            vmItem.ChangeCurrentPageView(_model.ActivePageView);
            if (vmItem == null)
            {
                return null;
            }
            Items.Add(vmItem);
            vmItem.ParentPageVM = this;

            Guid id = vmItem.RealParentGroupGID;
            if(id==Guid.Empty)
            {
                vmItem.IsSelected = isSelected;
            }            
            return vmItem;
        }
        private GroupViewModel InsertGrou2pCanvas(IGroup group, int ZBase, object positionOffset)
        {
            List<IRegion> children = new List<IRegion>();
            GetWidgetChildren(group, children);
            if (children.Count < 1)
            {
                return null;
            }

            System.Windows.Point offset;
            if (positionOffset == null)
            {
                offset = new System.Windows.Point(20, 20);
            }
            else
            {
                offset = (System.Windows.Point)positionOffset;
            }

            //Create Groups
            GroupViewModel vmGroup = WidgetFactory.CreateGroup(group) as GroupViewModel;
            vmGroup.IsGroup = true;

            //Create children widgets
            foreach (IWidget item in children)
            {
                IWidgetStyle wdgStyle = item.GetWidgetStyle(_curAdaptiveViewGID);
                if (wdgStyle == null)
                {
                    continue;
                }
                if (item.WidgetType == WidgetType.HamburgerMenu)
                {
                    IWidgetStyle buttonStyle = (item as IHamburgerMenu).MenuButton.GetWidgetStyle(_curAdaptiveViewGID);
                    buttonStyle.X += offset.X * _copyTime;
                    buttonStyle.Y += offset.Y * _copyTime;
                }
                else
                {
                    wdgStyle.X += offset.X * _copyTime;
                    wdgStyle.Y += offset.Y * _copyTime;
                }
                wdgStyle.Z += ZBase;
                WidgetViewModBase vmChild = InsertWidget2Canvas(item);
                if (vmChild == null)
                {
                    continue;
                }

                vmChild.IsGroup = false;
                vmChild.ParentID = group.Guid;
                vmChild.IsSelected = false;
                vmGroup.AddChild(vmChild);
            }

            // Fire event after child is added.
            vmGroup.Status = GroupStatus.Selected;

            //render UI group
            Items.Add(vmGroup);
            vmGroup.Refresh();

            return vmGroup;
        }       


        private void DeselectAll()
        {
            //Deselect all in current page
            foreach (WidgetViewModBase wdg in Items)
            {
                if (wdg.IsGroup == false)
                {
                    wdg.IsSelected = false;                
                }
                else
                {
                    (wdg as GroupViewModel).Status = GroupStatus.UnSelect;
                }
            }
        }
        private void GetWidgetPos(ref System.Windows.Point pt, int width, int height, AddWidgetType itype)
        {
            if (itype == AddWidgetType.DoubleClickAdd)
            {
                pt = GetCenterOfCanvasOffset(width, height, widgetoffsetX, widgetoffsetY);

                widgetoffsetX += 5;
                widgetoffsetY += 5;
                countoffsetY += 5;

                if (widgetoffsetY > 300)
                {
                    widgetoffsetX = 110 * (countoffsetY / 300);
                    widgetoffsetY = 0;
                }
            }

        }
        private void CommonAddWidget(string xamlString, System.Windows.Point pt, AddWidgetType itype)
        {
            if (xamlString == "lbw.rectangle")
            {
                GetWidgetPos(ref pt, 100, 100, itype);
                AddWidgetItem(WidgetType.Shape,
                    ShapeType.Rectangle, pt.X, pt.Y, 100, 100);
            }
            else if (xamlString == "lbw.roundedrectangle")
            {
                GetWidgetPos(ref pt, 100, 100, itype);
                AddWidgetItem(WidgetType.Shape,
                    ShapeType.RoundedRectangle, pt.X, pt.Y, 120, 60);
            }
            else if (xamlString == "lbw.diamond")
            {
                GetWidgetPos(ref pt, 100, 100, itype);
                AddWidgetItem(WidgetType.Shape,
                    ShapeType.Diamond, pt.X, pt.Y, 100, 100);
            }
            //else if (xamlString.Contains("wButton.png"))
            //{
            //    GetWidgetPos(ref pt, 100, 50, itype);
            //    AddWidgetItem(WidgetType.Button,
            //        ShapeType.None, pt.X, pt.Y, 100, 50);
            //}
            else if (xamlString == "lbw.circle")
            {
                GetWidgetPos(ref pt, 100, 100, itype);
                AddWidgetItem(WidgetType.Shape,
                    ShapeType.Ellipse, pt.X, pt.Y, 100, 100);
            }
            else if (xamlString == "lbw.link")
            {
                GetWidgetPos(ref pt, 100, 100, itype);
                AddWidgetItem(WidgetType.HotSpot,
                    ShapeType.None, pt.X, pt.Y, 100, 100);
            }
            else if (xamlString == "lbw.text")
            {
                GetWidgetPos(ref pt, 200, 100, itype);
                AddWidgetItem(WidgetType.Shape,
                    ShapeType.Paragraph, pt.X, pt.Y, 200, 100);
            }
            else if (xamlString == "lbw.horizontalline")
            {
                GetWidgetPos(ref pt, 200, 20, itype);
                AddLineWidgetItem(Orientation.Horizontal, pt.X, pt.Y, 200, 20);
            }
            else if (xamlString == "lbw.verticalline")
            {
                GetWidgetPos(ref pt, 20, 200, itype);
                AddLineWidgetItem(Orientation.Vertical, pt.X, pt.Y, 20, 200);
            }
            else if (xamlString == "lbw.triangle")
            {
                GetWidgetPos(ref pt, 100, 100, itype);
                AddWidgetItem(WidgetType.Shape,
                    ShapeType.Triangle, pt.X, pt.Y, 100, 100);
            }
            else if (xamlString == "lbw.listbox")
            {
                GetWidgetPos(ref pt, 200, 100, itype);
                AddWidgetItem(WidgetType.ListBox,
                    ShapeType.None, pt.X, pt.Y, 200, 100);
            }
            else if (xamlString == "lbw.droplist")
            {
                GetWidgetPos(ref pt, 200, 22, itype);
                AddWidgetItem(WidgetType.DropList,
                    ShapeType.None, pt.X, pt.Y, 200, 22);
            }
            else if (xamlString == "lbw.htmlbutton")
            {
                GetWidgetPos(ref pt, 100, 30, itype);
                AddWidgetItem(WidgetType.Button,
                    ShapeType.None, pt.X, pt.Y, 100, 30);
            }
            else if (xamlString == "lbw.checkbox")
            {
                GetWidgetPos(ref pt, 120, 16, itype);
                AddWidgetItem(WidgetType.Checkbox,
                    ShapeType.None, pt.X, pt.Y, 120, 16);
            }
            else if (xamlString == "lbw.radiobutton")
            {
                GetWidgetPos(ref pt, 120, 16, itype);
                AddWidgetItem(WidgetType.RadioButton,
                    ShapeType.None, pt.X, pt.Y, 120, 16);
            }
            else if (xamlString == "lbw.textarea")
            {
                GetWidgetPos(ref pt, 200, 100, itype);
                AddWidgetItem(WidgetType.TextArea,
                    ShapeType.None, pt.X, pt.Y, 200, 100);
            }
            else if (xamlString == "lbw.textfield")
            {
                GetWidgetPos(ref pt, 200, 25, itype);
                AddWidgetItem(WidgetType.TextField,
                    ShapeType.None, pt.X, pt.Y, 200, 25);
            }
            else if (xamlString == "lbw.hamburgermenu")
            {
                GetWidgetPos(ref pt, 37, 37, itype);
                AddWidgetItem(WidgetType.HamburgerMenu,
                    ShapeType.None, pt.X, pt.Y, 37, 37);
            }
            else if (xamlString == "lbw.swipeviews")
            {
                GetWidgetPos(ref pt, 420, 250, itype);
                AddWidgetItem(WidgetType.DynamicPanel,
                    ShapeType.None, pt.X, pt.Y, 420, 250);
            }
            else if (xamlString == "lbw.toastnotification")
            {//Toast default is on the top, so y is 0
                GetWidgetPos(ref pt, 298, 146, itype);
                AddWidgetItem(WidgetType.Toast,
                    ShapeType.None, pt.X, pt.Y, 298, 146);
            }
            else
            {
                GetWidgetPos(ref pt, 100, 100, itype);
                //newItem.Content = content;
                AddWidgetItem(WidgetType.Image, ShapeType.None, pt.X, pt.Y, 100, 100);
            }
        }


        // Add all widgets and groups in a container to page  
        private void AddObjects2Page0(IObjectContainer container, Guid adaptiveGuid, object parameter = null)
        {
            if (container == null || container.WidgetList.Count < 1)
            {
                return;
            }

            //Re-Order all the new pure Widgets(except for Groups)
            int nZbase = Items.Count - Items.OfType<GroupViewModel>().Count<GroupViewModel>();
            foreach (IWidget item in container.WidgetList.OrderBy(s => s.WidgetStyle.Z))
            {
                IWidgetStyle copyStyle = item.GetWidgetStyle(adaptiveGuid);
                if (copyStyle != null)
                {
                    item.WidgetStyle.X = copyStyle.X;
                    item.WidgetStyle.Y = copyStyle.Y;
                }                
                item.WidgetStyle.Z = nZbase;
                nZbase++;
            }

            //DeSelect all in current page
            DeselectAll();

            Naver.Compass.InfoStructure.CompositeCommand cmds = new Naver.Compass.InfoStructure.CompositeCommand();
            //Run Paste common widget
            foreach (IWidget newItem in container.WidgetList)
            {
                if (newItem != null && newItem.ParentGroup == null)
                {
                    //Render the UI
                    WidgetViewModBase widgetVM = PasteWidgetItem(newItem, 0, parameter);
                    CreateWidgetCommand cmd = new CreateWidgetCommand(this, widgetVM);
                    cmds.AddCommand(cmd);
                }
            }

            //Run Paste group
            foreach (IGroup newGroup in container.GroupList)
            {
                if (newGroup != null && newGroup.ParentGroup == null)
                {
                    //Render the UI
                    GroupViewModel groupVM = PasteGroupItem(newGroup, 0, parameter);

                    if (groupVM != null)
                    {
                        PasteGroupCommand cmd = new PasteGroupCommand(this, groupVM);
                        cmds.AddCommand(cmd);
                    }
                }
            }
            PushToUndoStack(cmds);
            IsDirty = true;
        }
        private List<WidgetViewModBase> AddObjects2Page2(IObjectContainer container,
            Guid adaptiveGuid, object posOffSet, bool bIsCopy, int baseZOrder = -1, Naver.Compass.InfoStructure.CompositeCommand cmds=null)
        {
            if (container == null ) 
            {
                return null;
            }
            
            int nZbase;
            if (baseZOrder <0)
            {
                nZbase = Items.Count - Items.OfType<GroupViewModel>().Count<GroupViewModel>();
            }
            else
            {
                nZbase = baseZOrder;
            }

            List<WidgetViewModBase> AllVms=CreateVMObjects(container);
            if (AllVms.Count<1)
            {
                return null;
            }
            List<WidgetViewModBase> AllObjs = 
                AllVms.Where(r => r.IsGroup == false).OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();
            
            //DeSelect all in current page
            DeselectAll();

            //Initialize the current all object vm
            InitializeNewObjectsPos(AllObjs, posOffSet, bIsCopy);

            //Re-Order all the new pure Widgets(except for Groups)
            InitializeNewObjectsZorder(AllObjs, adaptiveGuid, nZbase);

            if (cmds==null)
            {
                cmds = new Naver.Compass.InfoStructure.CompositeCommand();
            }
           
            //modify the original objects ZOrder
            ChangeOldObjectsZOrder(baseZOrder, AllObjs.Count,cmds);
            
            //Insert All children(including the group/widget/master) vm into canvas
            InsertAllVm2Canvas(AllVms,cmds);

            PushToUndoStack(cmds);
            IsDirty = true;

            //return new object collections
            return AllVms;
            
        }
        //initialize the position/zorder/adpativeView
        private void InitializeNewObjectsPos(List<WidgetViewModBase> AllObjs,object positionOffset,bool bIsCopy)
        {
            System.Windows.Point offset;
            if(bIsCopy==true)
            {
                if (positionOffset == null)
                {
                    offset = new System.Windows.Point(20 * _copyTime, 20 * _copyTime);
                }
                else
                {
                    offset = (System.Windows.Point)positionOffset;
                }
            }
            else
            {

                if (positionOffset == null)
                {
                    offset = new System.Windows.Point(0, 0);
                }
                else
                {
                    offset = (System.Windows.Point)positionOffset;
                }
            }

            foreach(WidgetViewModBase item in AllObjs)
            {
                item.ChangeCurrentPageView(_model.ActivePageView);
                item.Raw_Left = item.Left + offset.X;
                item.Raw_Top = item.Top + offset.Y;
            }
            //return InsertWidget2Canvas(newItem);
        }
        private void InitializeNewObjectsZorder(List<WidgetViewModBase> AllObjs,Guid adaptiveGuid, int nZOrderBase)
        {
            int nZbase = nZOrderBase;
            foreach (WidgetViewModBase it in AllObjs)
            {
                //IRegionStyle copyStyle = it.WidgetModel.GetSpecStyle(adaptiveGuid);
                //if (copyStyle != null)
                //{
                //    it.Left = copyStyle.X;
                //    it.Top = copyStyle.Y;
                //}
                it.ZOrder = nZbase;
                nZbase++;
            }   
        }
        private void ChangeOldObjectsZOrder(int ZBase, int offset, Naver.Compass.InfoStructure.CompositeCommand cmds)
        {
            if (ZBase < 0 || offset<=1)
            { 
                return;
            }

            List<WidgetViewModBase> targets =
                Items.Where(r => r.IsGroup == false && r.ZOrder > ZBase).ToList<WidgetViewModBase>();
            foreach(WidgetViewModBase it in targets)
            {
                CreatePropertyChangeUndoCommand(cmds, it, "ZOrder", it.ZOrder, it.ZOrder + offset);
                it.ZOrder = it.ZOrder + offset;
            }

        }
        //insert all objects into canvas
        private void InsertAllVm2Canvas(List<WidgetViewModBase> AllObjs, Naver.Compass.InfoStructure.CompositeCommand cmds)
        {

            foreach (WidgetViewModBase item in AllObjs)
            {
                item.ParentPageVM = this;
                Items.Add(item);
                
                if(item.IsGroup==false)
                {
                    if (item.RealParentGroupGID == Guid.Empty)
                    {
                        item.IsSelected = true;
                        CreateWidgetCommand cmd = new CreateWidgetCommand(this, item);
                        cmds.AddCommand(cmd);
                    }
                    else
                    {
                        item.IsSelected = false;
                    }
                }
                else
                {
                    GroupViewModel group = item as GroupViewModel;
                    group.Status = GroupStatus.Selected;
                    group.Refresh();

                    PasteGroupCommand cmd = new PasteGroupCommand(this, group);
                    cmds.AddCommand(cmd);
                }

            }       
        }
        private void AddObjectsFromData(IDataObject data,AddWidgetType addType,System.Windows.Point pt)
        {
            if (data.GetDataPresent("SVG_ITEM"))
            {
                Uri svgUri = data.GetData("SVG_ITEM") as Uri;
                Stream svgStream = Application.GetResourceStream(svgUri).Stream;
                if (svgStream == null)
                {
                    return;
                }

                if (addType == AddWidgetType.DoubleClickAdd)
                {
                    GetWidgetPos(ref pt, 150, 150, AddWidgetType.DoubleClickAdd);
                }

                WidgetViewModBase wdg = AddWidgetItem(WidgetType.SVG, ShapeType.None, pt.X, pt.Y, 150, 150);
                SVGWidgetViewModel svgWdg = wdg as SVGWidgetViewModel;
                if (svgWdg != null)
                {
                    svgWdg.ImportSvg(svgStream);
                    //Set default name for SVG Icon
                    int start = svgUri.AbsolutePath.LastIndexOf("/") + 1;
                    svgWdg.Name = Uri.UnescapeDataString(svgUri.AbsolutePath.Substring(start, svgUri.AbsolutePath.Length - start - 4));

                    svgWdg.IsSelected = false;
                    svgWdg.IsSelected = true;
                }
            }
            else if (data.GetDataPresent("DESIGNER_ITEM"))
            {
                string xamlString = data.GetData("DESIGNER_ITEM") as string;
                if (String.IsNullOrEmpty(xamlString))
                {
                    return;
                }
                if(addType==AddWidgetType.DoubleClickAdd)
                {
                    pt = new System.Windows.Point(100, 100);
                    CommonAddWidget(xamlString, pt, AddWidgetType.DoubleClickAdd);
                }
                else
                {
                    CommonAddWidget(xamlString, pt, AddWidgetType.DrgAdd);
                }
                
            }
            else if (data.GetDataPresent("CUSTOM_ITEM"))
            {
                try
                {
                    ICustomObject customObject = GetCustomObjectFromData(data.GetData("CUSTOM_ITEM"));
                    if (customObject != null)
                    {
                        #region donn't add widget if it's a empty widget.
                        customObject.Open();
                        bool hasChildren = customObject.Groups.Count > 0 || customObject.Widgets.Count > 0;
                        customObject.Close();
                        if (hasChildren == false)
                            return;
                        #endregion

                        //Default position is (0,0) for List UI
                        var containData = data.GetData("CUSTOM_ITEM") as Tuple<Guid, string, Guid, string>;
                        if(containData.Item4 == "List UI")
                        {
                            pt.X = 0;
                            pt.Y = 0;
                        }
                        else
                        {
                            //get all widgets' bouding in a custom object.
                            Rect bouding = GetCustomObjectSize(customObject);
                            if (addType == AddWidgetType.DoubleClickAdd)
                            {
                                GetWidgetPos(ref pt, (int)bouding.Width, (int)bouding.Height, AddWidgetType.DoubleClickAdd);
                                if (pt.X < 0) pt.X = 0;
                                if (pt.Y < 0) pt.Y = 0;
                            }

                        }

                        IObjectContainer container = _model.AddCustomObject(customObject, pt.X, pt.Y);
                        _selectionService.AllowWdgPropertyNotify(false);
                        AddObjects2Page2(container, _curAdaptiveViewGID,null,false);
                        _selectionService.AllowWdgPropertyNotify(true);
                        _selectionService.UpdateSelectionNotify();
                    }
                }
                catch
                {
                    _selectionService.AllowWdgPropertyNotify(true);
                }
                finally
                {
                    _selectionService.AllowWdgPropertyNotify(true);
                }
                
            }
            else if(data.GetDataPresent("MASTER_ITEM"))
            {
                if( _document.DocumentType==DocumentType.Standard)
                {
                    Guid guid = (Guid)data.GetData("MASTER_ITEM");
                    AddMasterItem(guid, pt.X, pt.Y);
                }
                else
                {
                    Guid guid = (Guid)data.GetData("MASTER_ITEM");
                    IObjectContainer container = _model.AddMasterPageObject(guid, pt.X, pt.Y);
                    _selectionService.AllowWdgPropertyNotify(false);
                    AddObjects2Page2(container, _curAdaptiveViewGID,null,false);
                    _selectionService.AllowWdgPropertyNotify(true);
                    _selectionService.UpdateSelectionNotify();
                }
                
            }
        }
        protected ICustomObject GetCustomObjectFromData(object dataObject)
        {
            var data = dataObject as Tuple<Guid, string, Guid, string>;
            IDocumentService DocService = ServiceLocator.Current.GetInstance<IDocumentService>();
            ILibrary libraryGet = DocService.LibraryManager.GetLibrary(data.Item1, data.Item2);
            ICustomObject customObject = null;
            if (libraryGet != null)
            {
                customObject = libraryGet.GetCustomObject(data.Item3);
                if (customObject != null)
                {
                    return customObject;
                }
            }

            if (customObject == null)
            {
                // If we cannot get the specific custom object, there is something wrong about this custom library.
                // Refresh this custom library
                ICustomLibraryService customLibraryService = ServiceLocator.Current.GetInstance<ICustomLibraryService>();
                ICustomLibrary customLibrary = customLibraryService.GetAllCustomLibraies().FirstOrDefault<ICustomLibrary>(x => x.LibraryGID == data.Item1);
                if (customLibrary != null)
                {
                    customLibrary.Refresh();
                }
            }

            return null;
        }
        private Rect GetCustomObjectSize(ICustomObject customObject)
        {
            customObject.Open();

            double minLeft = double.MaxValue;
            double minTop = double.MaxValue; 
            double maxRight = double.MinValue;
            double maxBottom = double.MinValue;

            foreach (IWidget element in customObject.Widgets)
            {
                double left, top,right, bottom;
                if (element is IHamburgerMenu)
                {
                    left = (element as IHamburgerMenu).MenuButton.WidgetStyle.X;
                    top = (element as IHamburgerMenu).MenuButton.WidgetStyle.Y;
                    right = left + (element as IHamburgerMenu).MenuButton.WidgetStyle.Width;
                    bottom = top + (element as IHamburgerMenu).MenuButton.WidgetStyle.Height;
                }
                else
                {
                    left = element.WidgetStyle.X;
                    top = element.WidgetStyle.Y;
                    right = left + element.WidgetStyle.Width;
                    bottom = top + element.WidgetStyle.Height;
                }

                minLeft = Math.Min(minLeft, left);
                minTop = Math.Min(minTop, top);
                maxRight = Math.Max(maxRight, right);
                maxBottom = Math.Max(maxBottom, bottom);
            }
            customObject.Close();

            return new Rect(minLeft, minTop, maxRight - minLeft, maxBottom - minTop);
        }
        #endregion

        #region ToolBar feature functions;

        private bool IsSupportProperty(PropertyOption type)
        {
            List<WidgetViewModBase> allSelects = _selectionService.GetSelectedWidgets().OfType<WidgetViewModBase>().ToList<WidgetViewModBase>();
            switch (type)
            {
                case PropertyOption.Option_Border:
                    return allSelects.Exists(k => k.IsSupportBorber == true);
                case PropertyOption.OPtion_BackColor:
                    return allSelects.Exists(k => k.IsSupportBackground == true);
                case PropertyOption.Option_Bullet:
                    return allSelects.Exists(k => k.IsSupportTextRotate == true);
                case PropertyOption.Option_LineArrow:
                    return allSelects.Exists(k => k.IsSupportArrowStyle == true);
                case PropertyOption.Option_Text:
                    return allSelects.Exists(k => k.IsSupportText == true);
                case PropertyOption.Option_TextHor:
                    return allSelects.Exists(k => k.IsSupportTextHorAlign == true);
                case PropertyOption.Option_TextVer:
                    return allSelects.Exists(k => k.IsSupportTextVerAlign == true);
                case PropertyOption.Option_WidgetRotate:
                    return allSelects.Exists(k => k.IsSupportRotate == true);
                case PropertyOption.Option_TextRotate:
                    return allSelects.Exists(k => k.IsSupportTextRotate == true);
                case PropertyOption.Option_CornerRadius:
                    return allSelects.Exists(k => k.IsSupportCornerRadius == true);
            }
            System.Diagnostics.Debug.Assert(false, "IsSupportProperty PageVM error!");
            return false;
        }

        List<IWidgetPropertyData> GetEligibleWidgets(PropertyOption type)
        {
            List<IWidgetPropertyData> WidgetList = _selectionService.GetSelectedWidgets();

            switch (type)
            {
                case PropertyOption.Option_Border:
                    return WidgetList.FindAll(k =>((WidgetViewModBase)k).IsSupportBorber == true);
                case PropertyOption.Option_Text:
                    return WidgetList.FindAll(k => ((WidgetViewModBase)k).IsSupportText == true);
                case PropertyOption.Option_LineArrow:
                    return WidgetList.FindAll(k => ((WidgetViewModBase)k).IsSupportArrowStyle == true);
                case PropertyOption.Option_TextHor:
                    return WidgetList.FindAll(k => ((WidgetViewModBase)k).IsSupportTextHorAlign == true);
                case PropertyOption.Option_TextVer:
                    return WidgetList.FindAll(k => ((WidgetViewModBase)k).IsSupportTextVerAlign == true);
                case PropertyOption.Option_Bullet:
                    return WidgetList.FindAll(k => ((WidgetViewModBase)k).IsSupportTextRotate == true);
                case PropertyOption.OPtion_BackColor:
                    return WidgetList.FindAll(k => ((WidgetViewModBase)k).IsSupportBackground == true);
                case PropertyOption.Option_WidgetRotate:
                    return WidgetList.FindAll(k => ((WidgetViewModBase)k).IsSupportRotate == true);
                case PropertyOption.Option_TextRotate:
                    return WidgetList.FindAll(k => ((WidgetViewModBase)k).IsSupportTextRotate == true);
                case PropertyOption.Option_CornerRadius:
                    return WidgetList.FindAll(k => ((WidgetViewModBase)k).IsSupportCornerRadius == true);
                default:
                    System.Diagnostics.Debug.Assert(false, "GetEligibleWidgets_Group function error!");
                    return null;
            }

           
        }

        private int GetSelectWidgetsNum()
        {
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();
            return allSelects.Count;
        }

        private int GetUnlockedWidgetsNum()
        {
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();
            return allSelects.Count;
        }
        private Rect GetTargetRect(List<IWidgetPropertyData> allSelects)
        {
            var targetObject = allSelects.FirstOrDefault(a => a.IsTarget == true) as WidgetViewModBase;
            if (targetObject == null)
            {
                targetObject = allSelects.FirstOrDefault(b => b.ZOrder == allSelects.Min(a => a.ZOrder)) as WidgetViewModBase;
            }
            return targetObject.GetBoundingRectangle();
        }
        private double GetLeftMost()
        {
            double dLeft = double.MaxValue;
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();

            foreach (WidgetViewModBase item in allSelects)
            {
                double actualvalue = item.GetBoundingRectangle().Left;
                if (actualvalue < dLeft)
                {
                    dLeft = actualvalue;
                }
            }

            return dLeft == double.MaxValue ? 0 : dLeft;
        }
        private double GetRightMost()
        {
            double dRight = double.MinValue;
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();

            foreach (WidgetViewModBase item in allSelects)
            {
                double actualvalue = item.GetBoundingRectangle().Right;
                if (actualvalue > dRight)
                {
                    dRight = actualvalue;
                }
            }

            return dRight == double.MinValue ? 0 : dRight;
        }
        private double GetTopMost()
        {
            double dTop = double.MaxValue;
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();

            foreach (WidgetViewModBase item in allSelects)
            {
                double actualvalue = item.GetBoundingRectangle().Top;
                if (actualvalue < dTop)
                {
                    dTop = actualvalue;
                }
            }

            return dTop == double.MaxValue ? 0 : dTop;
        }
        private double GetBottomMost()
        {
            double dBottom = double.MinValue;
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();

            foreach (WidgetViewModBase item in allSelects)
            {
                double actualvalue = item.GetBoundingRectangle().Bottom;
                if (actualvalue > dBottom)
                {
                    dBottom = actualvalue;
                }
            }

            return dBottom == double.MinValue ? 0 : dBottom;
        }
        private double GetCenterMost()
        {
            double dminLeft = double.MaxValue;
            double dMaxRight = double.MinValue;

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();
            foreach (WidgetViewModBase item in allSelects)
            {
                Rect rect = item.GetBoundingRectangle();
                if (rect.Left < dminLeft)
                {
                    dminLeft = rect.Left;
                }

                if (rect.Right > dMaxRight)
                {
                    dMaxRight = rect.Right;
                }
            }
            double dCenter = (dMaxRight + dminLeft) / 2;
            return dCenter;
        }
        private double GetMiddleMost()
        {
            double dminTop = double.MaxValue;
            double dMaxBottom = double.MinValue;

            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets().Where(a => a.IsLocked == false).ToList();
            foreach (WidgetViewModBase item in allSelects)
            {
                Rect rect = item.GetBoundingRectangle();
                if (rect.Top < dminTop)
                {
                    dminTop = rect.Top;
                }

                if (rect.Bottom > dMaxBottom)
                {
                    dMaxBottom = rect.Bottom;
                }
            }

            double dMiddle = (dminTop + dMaxBottom) / 2;
            return dMiddle;
        }
        private int CompareByLeft(IWidgetPropertyData tdata1, IWidgetPropertyData tdata2)
        {
            if (tdata1 == null)
            {
                if (tdata2 == null)
                {
                    return 0;
                }

                return 1;
            }

            if (tdata2 == null)
            {
                return -1;
            }

            WidgetViewModBase item1 = tdata1 as WidgetViewModBase;
            WidgetViewModBase item2 = tdata2 as WidgetViewModBase;
            return item1.GetBoundingRectangle().Left.CompareTo(item2.GetBoundingRectangle().Left); ;
        }
        private int CompareByTop(IWidgetPropertyData tdata1, IWidgetPropertyData tdata2)
        {
            if (tdata1 == null)
            {
                if (tdata2 == null)
                {
                    return 0;
                }

                return 1;
            }

            if (tdata2 == null)
            {
                return -1;
            }

            WidgetViewModBase item1 = tdata1 as WidgetViewModBase;
            WidgetViewModBase item2 = tdata2 as WidgetViewModBase;
            return item1.GetBoundingRectangle().Top.CompareTo(item2.GetBoundingRectangle().Top);
        }
        private void ReOrderAllWidgets(UndoCompositeCommand undoCompositeCommand)
        {
            int count = Items.Count - Items.OfType<GroupViewModel>().Count<GroupViewModel>();
            List<WidgetViewModBase> allWidget = Items.ToList<WidgetViewModBase>().Where(c => c.IsGroup == false).ToList<WidgetViewModBase>();
            allWidget = allWidget.OrderBy(s => s.ZOrder).ToList<WidgetViewModBase>();
            //set
            for (int i = 0; i < allWidget.Count; i++)
            {
                if (undoCompositeCommand != null)
                {
                    CreatePropertyChangeUndoCommand(undoCompositeCommand, allWidget[i], "ZOrder", allWidget[i].ZOrder, i);
                }

                allWidget[i].ZOrder = i;
            }
        }


        private double GetActualPageHeight()
        {
            double dBottom = 0;
            List<WidgetViewModBase> allWidget = Items.ToList<WidgetViewModBase>().Where(c => c.IsGroup == false).ToList<WidgetViewModBase>();

            foreach (WidgetViewModBase item in allWidget)
            {
                double actualvalue = item.GetBoundingRectangle().Bottom;
                if (actualvalue > dBottom)
                {
                    dBottom = actualvalue;
                }
            }

            return dBottom == double.MinValue ? 0 : dBottom;
        }
        private double GetActualPageWidth()
        {
            double dRight = 0;
            List<WidgetViewModBase> allWidget = Items.ToList<WidgetViewModBase>().Where(c => c.IsGroup == false).ToList<WidgetViewModBase>();

            foreach (WidgetViewModBase item in allWidget)
            {
                double actualvalue = item.GetBoundingRectangle().Right;
                if (actualvalue > dRight)
                {
                    dRight = actualvalue;
                }
            }

            return dRight == double.MinValue ? 0 : dRight;
        }
        #endregion

        #region Undo/Redo

        public UndoManager UndoManager
        {
            get { return _undoManager; }
            set { _undoManager = value; }
        }

        // Every page has an individual undo/redo stack
        protected UndoManager _undoManager = new UndoManager();

        #endregion
    }
}
