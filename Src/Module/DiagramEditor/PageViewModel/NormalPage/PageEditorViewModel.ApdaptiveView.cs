using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Common.Helper;
using System.Windows.Controls;
using System.Diagnostics;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel
    {
        #region Adaptive View Delegate Command
        public DelegateCommand<object> OpenAdaptiveCommand { get; private set; }
        public DelegateCommand<object> CheckAdaptiveCommand { get; private set; }
        #endregion

        #region Adaptive view binding data
        public ObservableCollection<AdaptivVieweNode> AdaptiveViewsList
        {
            get
            {
                return _adaptiveModel.AdaptiveViewsList;
            }
            set
            {
                _adaptiveModel.AdaptiveViewsList = value;
            }
        }
        public Visibility AdaptiveSetVisibility
        {
            get
            {
                if (_document == null)
                    return Visibility.Visible;

                return _document.DocumentType == DocumentType.Library ? 
                    Visibility.Collapsed : Visibility.Visible;
            }
        }

        virtual public double EditorScale
        {
            get 
            {
                return _model.EditorScale;
            }
            set
            {
                if (_model.EditorScale != value)
                {
                    if (_model.EditorScale < 1 && value >= 1)
                    {
                        _ListEventAggregator.GetEvent<EditorScaleChangeEvent>().Publish(true);
                    }
                    else if (_model.EditorScale >= 1 && value < 1)
                    {
                        _ListEventAggregator.GetEvent<EditorScaleChangeEvent>().Publish(false);
                    }
                    _model.EditorScale = value;

                    FirePropertyChanged("EditorScale");
                    FirePropertyChanged("AdaptiveWidth");
                    FirePropertyChanged("DeviceBoxWidth");
                    FirePropertyChanged("DeviceBoxHeight");
                    FirePropertyChanged("IsGridVisible");
                }
            }
        }

        private int adaptiveWidth;
        public int AdaptiveWidth
        {
            get 
            {
                return Convert.ToInt32(adaptiveWidth * EditorScale); 
            }
            set
            {
                if (adaptiveWidth != value)
                {
                    adaptiveWidth = value;
                    FirePropertyChanged("AdaptiveWidth");
                }
            }
        }

        private int adaptiveHeight;
        public int AdaptiveHeight
        {
            get 
            { 
                return Convert.ToInt32(adaptiveHeight * EditorScale);
            }
            set
            {
                if (adaptiveHeight != value)
                {
                    adaptiveHeight = value;
                    FirePropertyChanged("AdaptiveHeight");
                }
            }
        }

        private string adaptiveName;
        public string AdaptiveName
        {
            get { return adaptiveName; }
            set
            {
                if (adaptiveName != value)
                {
                    adaptiveName = value;
                    //BaseView.Name = value;
                    FirePropertyChanged("AdaptiveName");
                }
            } 
        }

        private Visibility _deviceVisibility;
        public Visibility DeviceVisibility
        {
            get
            {
                return _deviceVisibility;
            }
            set
            {
                if (_deviceVisibility != value)
                {
                    _deviceVisibility = value;
                    FirePropertyChanged("DeviceVisibility");
                }
            }

        }
        public bool IsAdaptivePanelOpened
        {
            get
            {
                return _adaptiveModel.IsAdaptiveOpen;
            }
            set
            {
                _adaptiveModel.IsAdaptiveOpen = value;
                FirePropertyChanged("IsAdaptivePanelOpened");
            }
        }

        public Visibility IsAdaptiveListVisible
        {
            get
            {
                if (_adaptiveModel.AdaptiveViewsList.Count > 0)
                    return Visibility.Visible;
                else return Visibility.Collapsed;
            }
        }

        #endregion
        
        #region Adaptive view  public function
        public void SetAdaptiveViewVisibility(bool IsVisible)
        {
            _adaptiveModel.IsPagePropOpen = IsVisible;
        }

        virtual protected void CheckAdaptiveView(Guid guid)
        {
            AdaptivVieweNode node = null;
            //check the same view and uncheck others
            foreach (var item in _adaptiveModel.AdaptiveViewsList)
            {
                if (item.Guid == guid)
                {
                    node = item;
                    item.IsChecked = true;
                    _curAdaptiveViewGID = item.Guid;
                    _model.SetActivePageView(_curAdaptiveViewGID);
                }
                else
                    item.IsChecked = false;
            }

            if (node != null)
            {
                //Update Canvas and Adaptive UI
                UpdateAdaptiveView(node);
                UpdatePageView(node.Guid);

                _adaptiveModel.PageAdaptiveList.Add(this.PageGID, node);
            }

            //refresh guide when change adaptive view.
            _ListEventAggregator.GetEvent<UpdateGridGuide>().Publish(GridGuideType.Guide);


            //keep the checked adaptive vew in model, loading it when open page next time
            AdaptivVieweNode adapnode = _adaptiveModel.PageAdaptiveList.FirstOrDefault(x => x.Key == this.PageGID).Value;
            //if the view already exists, remove it first.
            if (adapnode != null)
            {
                _adaptiveModel.PageAdaptiveList.Remove(this.PageGID);
            }

        }
        #endregion

        #region Adaptive view  private function
        private void OpenAdaptveExecute(object obj)
        {
            //_ListEventAggregator.GetEvent<OpenDialogEvent>().Publish(DialogType.AdaptiveView);
            OpenSetAdaptiveWindow();
        }
        private void OpenSetAdaptiveWindow()
        {
            SetAdaptiveView win = new SetAdaptiveView();
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }

        /// <summary>
        /// check one adaptive view 
        /// </summary>
        /// <param name="obj">the adaptive view ndoe</param>
        private void CheckAdaptiveExecute(object obj)
        {
            AdaptivVieweNode node = obj as AdaptivVieweNode;
            if (node == null)
                return;

            //Click the same node again
            if (node.IsChecked == false)
            {
                node.IsChecked = true;
                return;
            }

            CheckAdaptiveView(node.Guid);
            EditorCanvas.Focus();

        }

        /// <summary>
        /// Load adaptive views form document.
        /// </summary>
        /// <param name="obj"></param>
        private void LoadAdaptiveViewsHandler(AdaptiveLoadType type)
        {
            //If edit adaptive view in set adaptive widow, clear all undo operation.
            if (type == AdaptiveLoadType.Edit)
            {
                UndoManager.Clear();
            }
            _adaptiveModel.AdaptiveViewsList.Clear();
            if (_document != null)
            {
                if (_document.AdaptiveViewSet.Base.ChildViews.Count > 0)
                {
                    // Load adaptive view only if base has child views
                    LoadAdaptiveView(null, _document.AdaptiveViewSet.Base);

                    if (_curAdaptiveViewGID == Guid.Empty)
                    {
                        //Initial loading
                        CheckBaseNode();
                    }
                    else
                    {
                        AdaptivVieweNode checkNode = _adaptiveModel.AdaptiveViewsList.FirstOrDefault(x => x.Guid == _curAdaptiveViewGID);
                        if (checkNode == null)
                        {
                            //Current adaptive-view has be removed
                            CheckBaseNode();
                        }
                        else
                        {
                            //Keep original adaptive-view
                            checkNode.IsChecked = true;
                            _curAdaptiveViewGID = checkNode.Guid;
                            UpdateAdaptiveView(checkNode);
                        }
                    }
                }
                else 
                {
                    if (_curAdaptiveViewGID == Guid.Empty)
                    {
                        //Current adaptive-view has be removed
                        CheckBaseNode();        
                    }  
                    else if (_curAdaptiveViewGID != _document.AdaptiveViewSet.Base.Guid)
                    {
                        //Only base adaptive view
                        CheckBaseNode();
                    }          
                }
            }

            FirePropertyChanged("AdaptiveViewsList");
            FirePropertyChanged("IsAdaptiveListVisible");
        }

        /// <summary>
        /// Check Base Adaptive when load.
        /// </summary>
        private void CheckBaseNode()
        {
            if (_document == null)
                return;

            AdaptivVieweNode baseNode = _adaptiveModel.AdaptiveViewsList.FirstOrDefault(x => x.Name == @"Base");
            if (baseNode != null)
            {
                baseNode.IsChecked = true;
                baseNode.IsRoot = true;
                _curAdaptiveViewGID = baseNode.Guid;
            }
            else
            {
                //Hide adaptive box, check a initial node
                baseNode = new AdaptivVieweNode();

                _curAdaptiveViewGID = _document.AdaptiveViewSet.Base.Guid;
                //refresh base view in case delete all other views.
            }
            
            UpdateAdaptiveView(baseNode);
            UpdatePageView(_curAdaptiveViewGID);
            _model.SetActivePageView(_curAdaptiveViewGID);
        }
        private void LoadAdaptiveView(AdaptivVieweNode parent, IAdaptiveView curView)
        {
            //==============load current view================//

            //Set all Condition to LessOrEqual
            if (curView.Condition != AdaptiveViewCondition.LessOrEqual)
            {
                curView.Condition = AdaptiveViewCondition.LessOrEqual;
            }
            AdaptivVieweNode adaptive = new AdaptivVieweNode(curView);
            adaptive.ParentNode = parent;

            if (parent != null)
            {
                parent.Add(adaptive);
            }
            
            _adaptiveModel.AdaptiveViewsList.Add(adaptive);

            //==============load children views================//
            foreach (IAdaptiveView view in curView.ChildViews)
            {
                LoadAdaptiveView(adaptive, view);
            }
        }
        private void UpdateAdaptiveView(AdaptivVieweNode node)
        {
            if (node == null)
                return;

            AdaptiveWidth = node.Width;
            AdaptiveHeight = node.Height;
            AdaptiveName = node.Name;
        }


        private void UpdatePageView(Guid viewGuid)
        {
            IPageViews allViews = _model.PageViews;
            if (allViews == null)
                return;
            IPageView targetView = allViews.GetPageView(viewGuid);
            if (targetView == null)
                return;

            _selectionService.RemoveAllWidgets();
            DeselectAll();

            //first,update the widget common style property
            foreach (WidgetViewModBase it in Items.Where(c => c.IsGroup == false))
            {
                it.ChangeCurrentPageView(targetView);
            }
            //then,update the group common style property
            foreach (WidgetViewModBase it in Items.Where(c => c.IsGroup == true))
            {
                it.ChangeCurrentPageView(targetView);
            }

            _ListEventAggregator.GetEvent<UpdateAdaptiveView>().Publish(viewGuid);
        }
        private void OpenAdaptiveViewSettingHandler(DialogType type)
        {
            if(DialogType.AdaptiveView==type)
            {
                OpenSetAdaptiveWindow();
            }
        }
        #endregion

        #region Device

        private void InitDeviceView()
        {
            _deviceModel = DeviceModel.GetInstance();
            _deviceModel.LoadDevices();

            CheckDevices();
            CheckDeviceCommand = new DelegateCommand<object>(CheckDeviceExecute);
            DeviceOffOnCommand = new DelegateCommand<object>(DeviceOffOnExecute);
            AutoAdjustScaleCommand = new DelegateCommand<object>(AutoAdjustScaleExecute);
            PagePropOffOnCommand = new DelegateCommand<object>(PagePropOffOnExecute);
            ExchangeWidthHeightCommand = new DelegateCommand<object>(ExchangeWidthHeightExecute);
            EditDeviceCommand = new DelegateCommand<object>(EditDevideExecute);

            //_ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler);
            _ListEventAggregator.GetEvent<OpenPanesEvent>().Subscribe(OpenPanesEventHandler);
            _ListEventAggregator.GetEvent<UpdateLanguageEvent>().Subscribe(UpdateLanguageEventHandler);

        }
        public DelegateCommand<object> CheckDeviceCommand { get; private set; }
        public DelegateCommand<object> DeviceOffOnCommand { get; private set; }
        public DelegateCommand<object> PagePropOffOnCommand { get; private set; }
        public DelegateCommand<object> ExchangeWidthHeightCommand { get; private set; }
        public DelegateCommand<object> EditDeviceCommand { get; private set; }
        public DelegateCommand<object> AutoAdjustScaleCommand { get; private set; }

        public ObservableCollection<DeviceNode> DevicesList
        {
            get
            {
                return _deviceModel.DeviceList;
            }
        }

        public IDeviceSet DeviceSet
        {
            get
            {
                if (_document!=null&&_document.DeviceSet != null)
                {
                    return _document.DeviceSet;
                }
                return null;
            }
        }

        public IAdaptiveViews AdaptiveViews
        {
            get
            {
                if (_document != null && _document.AdaptiveViewSet != null)
                    return _document.AdaptiveViewSet.AdaptiveViews;
                return null;
            }
        }
        public bool IsPagePropOn
        {
            get
            {
                return _adaptiveModel.IsPagePropOpen;
            }
            set
            {
                _adaptiveModel.IsPagePropOpen = value;
                FirePropertyChanged("IsPagePropOn");
            }
        }
        public bool IsDeviceOn
        {
            get
            {
                return DeviceSet.IsVisible;
            }
            set
            {
                if (DeviceSet.IsVisible != value)
                {
                    DeviceSet.IsVisible = value;
                    FirePropertyChanged("IsDeviceOn");
                    _document.IsDirty = true;
                }
            }
        }

        private int editableWidth;
        //Binding with Width Edit-box
        public int EditableWidth
        {
            get 
            {
                return editableWidth; 
            }
            set
            {
                if (editableWidth != value)
                {
                    editableWidth = value;
                    if (bUserSetting)
                    {
                        DeviceNode checkedItem = DevicesList.FirstOrDefault(x => x.IsChecked == true);
                        if (checkedItem != null)
                        {
                            checkedItem.IsChecked = false;
                        }
                        SelectedDevice = DevicesList.FirstOrDefault(x => x.Name == CommonDefine.UserSetting);
                        SelectedDevice.IsChecked = true;
                        SelectedDevice.Width = value;
                        selectedDevice.Height = EditableHeight;

                        UpdateDeviceBox();
                        _document.IsDirty = true;
                    }
                    FirePropertyChanged("EditableWidth");
                    FirePropertyChanged("DeviceBoxWidth");
                }
            }
        }

        private int editableHeight;
        //Binding with Height Edit-box
        public int EditableHeight
        {
            get
            {
                return editableHeight;
            }
            set
            {
                if (editableHeight != value)
                {
                    editableHeight = value;
                    if (bUserSetting)
                    {
                        DeviceNode checkedItem = DevicesList.FirstOrDefault(x => x.IsChecked == true);
                        if (checkedItem != null)
                        {
                            checkedItem.IsChecked = false;
                        }

                        SelectedDevice = DevicesList.FirstOrDefault(x => x.Name == CommonDefine.UserSetting);
                        SelectedDevice.IsChecked = true;
                        SelectedDevice.Height = value;
                        SelectedDevice.Width = EditableWidth;

                        //show usersetting resolution only when width != 0 and height !=0 
                        UpdateDeviceBox();
                        _document.IsDirty = true;
                    }
                    FirePropertyChanged("EditableHeight");
                    FirePropertyChanged("DeviceBoxHeight");

                }
            }
        }

        private DeviceNode selectedDevice;
        public DeviceNode SelectedDevice
        {
            get
            {
                return selectedDevice;
            }
            set
            {
                if(selectedDevice!=value)
                {
                    selectedDevice = value;
                    FirePropertyChanged("DeviceBoxName");
                }
            }
        }

        public int DeviceBoxWidth
        {
            get
            {
                return Convert.ToInt32(EditableWidth * EditorScale);
            }
        }

        public int DeviceBoxHeight
        {
            get
            {
                return Convert.ToInt32(EditableHeight * EditorScale);
            }
        }

        public string DeviceBoxName
        {
            get
            {
                if (selectedDevice != null)
                    return selectedDevice.NameWidthSize;
                return string.Empty;
            }
        }

        private void CheckDeviceExecute(object obj)
        {
            DeviceNode node = obj as DeviceNode;
            if (node == null)
                return;

            //Check the same item, keep it.
            if(node.IsChecked == false)
            {
                node.IsChecked = true;
                return;
            }
            SelectedDevice = node;
            //unchecked other menuitem
            DeviceNode checkedItem = DevicesList.FirstOrDefault(x => x.IsChecked == true && x != node);
            if(checkedItem!=null)
            {
                checkedItem.IsChecked = false;
            }
            //Check current node
            CheckDeviceView(node);

            _document.IsDirty = true;
        }

        private void DeviceOffOnExecute(object obj)
        {
            IsDeviceOn = !IsDeviceOn;
            UpdateDeviceBox();
        }

        private void AutoAdjustScaleExecute(object obj)
        {
            AutoAdjustScale();
        }

        private void UpdateDeviceBox()
        {
            if (IsDeviceOn && (DeviceBoxWidth != 0.0 && DeviceBoxHeight != 0.0))
            {
                DeviceVisibility = Visibility.Visible;
            }
            else
            {
                DeviceVisibility = Visibility.Collapsed;
            }
        }

        private void PagePropOffOnExecute(object obj)
        {
            
            ActivityPane pane = new ActivityPane();
            if (_document != null && _document.DocumentType == DocumentType.Library)
            {
                pane.Name = CommonDefine.PanePageIcon;
            }
            else
            {
                pane.Name = CommonDefine.PanePageProp;
            }

            pane.bOpen = !IsPagePropOn;
            pane.bFromToolbar = true;
            _ListEventAggregator.GetEvent<OpenPanesEvent>().Publish(pane);
        }
        private void ExchangeWidthHeightExecute(object obj)
        {
            if (null == SelectedDevice)
            {
                return;
            }

            try
            {
                if(DeviceBoxName != CommonDefine.UserSetting)
                {
                    bUserSetting = false;
                }
                int item = EditableWidth;
                EditableWidth = EditableHeight;
                EditableHeight = item;

                SelectedDevice.Width = EditableWidth;
                SelectedDevice.Height = EditableHeight;

                bUserSetting = true;
                _document.IsDirty = true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
        }

        private void EditDevideExecute(object obj)
        {
            EditDeviceWindow device = new EditDeviceWindow();
            device.Owner = Application.Current.MainWindow;
            device.ShowDialog();
        }

        /// <summary>
        /// Select one device in device menu
        /// </summary>
        /// <param name="view"></param>
        private void CheckDeviceView(DeviceNode view)
        {
            if (view == null)
                return;
            bUserSetting = false;
            EditableWidth = view.Width;
            EditableHeight = view.Height;
            bUserSetting = true;

            SelectedDevice = view;
            UpdateDeviceBox();
        }

        public void CheckDevices()
        {
            //_deviceModel.LoadDevices();

            DeviceNode selectedItem = DevicesList.FirstOrDefault(x => x.IsChecked == true);
            //Current select is null, select off item.
            if(selectedItem == null)
            {
                selectedItem = DevicesList.FirstOrDefault(x => x.Name == CommonDefine.PresetOff);
                if (selectedItem != null)
                {
                    selectedItem.IDevice.IsChecked = true;
                }
            }
            FirePropertyChanged("IsDeviceOn");
            CheckDeviceView(selectedItem);
        }

        private void OpenPanesEventHandler(ActivityPane pane)
        {
            if (_document != null && _document.DocumentType == DocumentType.Library)
            {
                if (pane.Name == CommonDefine.PanePageIcon)
                {
                    IsPagePropOn = pane.bOpen;
                }
            }
            else
            {
                if (pane.Name == CommonDefine.PanePageProp)
                {
                    IsPagePropOn = pane.bOpen;
                }
            }
 
        }

        protected virtual void UpdateLanguageEventHandler(string str)
        {
            //FirePropertyChanged("SelectedDevice");
            FirePropertyChanged("DeviceBoxName");
        }

        //if usersetting width height changed by user set width box or height box.
        private bool bUserSetting = true;
        private DeviceModel _deviceModel;
        private AdaptiveModel _adaptiveModel;
        protected IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        #endregion

        #region Scale Setting
        public Dictionary<double, string> ScaleDic
        {
            get
            {
                return new Dictionary<double, string>()
                {
                    {0.1,"10%"},
                    {0.25,"25%"},
                    {0.33,"33%"},
                    {0.5,"50%"},
                    {0.65,"65%"},
                    {0.8,"80%"},
                    {1,"100%"},
                    {1.25,"125%"},
                    {1.5,"150%"},
                    {2,"200%"},
                    {3,"300%"},
                    {4,"400%"}
                };
            }

        }
        #endregion
    }
}
