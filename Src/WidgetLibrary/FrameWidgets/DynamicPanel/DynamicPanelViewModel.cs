using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using Naver.Compass.Common.Helper;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public class DynamicPanelViewModel : WidgetViewModBase
    {

        public DynamicPanelViewModel(IWidget widget)
        {
            _model = new DynamicPanelModel(widget,false);
            _widget = widget;
            _bSupportBorder = false;
            _bSupportBackground = false;
            _bSupportText = false;
            _bSupportTextVerAlign = false;
            _bSupportTextHorAlign = false;
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = false;
            _bSupportTextRotate = false;

            widgetGID = widget.Guid;
            Type = ObjectType.DynamicPanel;
            IsChildPageOpened = (_model as DynamicPanelModel).IsAnyChildrenPageOpen();

            NavigationChildren = new ObservableCollection<DynamicPanelIconNode>();
            NavigationChildren.CollectionChanged += DynamicChildren_CollectionChanged;
            ViewItems = new ObservableCollection<DynamicPanelIconNode>();
            _ListEventAggregator.GetEvent<RefreshWidgetChildPageEvent>().Subscribe(RefreshWidgetPageUIHandler);
            _ListEventAggregator.GetEvent<CloseWidgetPageEvent>().Subscribe(CloseWidgetPageHandler);
            LoadChildrenIconNode();
        }

        #region Widget Routed Event Handler by Command
        private DelegateCommand<object> _doubleClickCommand = null;
        override public ICommand DoubleClickCommand
        {
            get
            {
                if (_doubleClickCommand == null)
                {
                    _doubleClickCommand = new DelegateCommand<object>(OnDoubleClick);
                }
                return _doubleClickCommand;
            }
        }
        private void OnDoubleClick(object obj)
        {
            OpenDynamicChildPage();
        }
        #endregion

        #region  private member and function
        private IWidget _widget;
        private void OpenDynamicChildPage()
        {
            _ListEventAggregator.GetEvent<OpenWidgetPageEvent>().Publish(_widget);
            IsChildPageOpened = true;
        }
        private void LoadChildrenIconNode()
        {
            NavigationChildren.Clear();
            ViewItems.Clear();
            IDynamicPanel flicking = (_widget as IDynamicPanel);

            double activeWidth = ItemWidth * 0.01 * PanelWidth;
            double sideWidth = (ItemWidth - activeWidth - 2 * LineWidth) / 2;

            int index = 0;
            foreach (IPage item in flicking.PanelStatePages)
            {
                DynamicPanelIconNode childVM = new DynamicPanelIconNode(item,_model.StyleGID);
                //Open all child pages.
                item.Open();
                NavigationChildren.Add(childVM);
                if (flicking.StartPanelStatePage == item)
                {
                    childVM.IsChecked = true;
                }
                
                switch (ViewMode)
                {
                    case DynamicPanelViewMode.Full:
                        if (index == 0)
                        {
                            childVM.PanelWidth = ItemWidth;
                            childVM.LoadAllChildrenWidgets(false);
                            ViewItems.Add(childVM);
                        }
                        break;
                    case DynamicPanelViewMode.Card:
                        if (index == 0)
                        {
                            childVM.PanelWidth = activeWidth; ; // (ItemWidth - activeWidh) / 2;
                            childVM.LineWidth = LineWidth;
                            childVM.LoadAllChildrenWidgets(false);
                            ViewItems.Add(childVM);
                        }
                        else if (index == 1)
                        {
                            childVM.PanelWidth = sideWidth > activeWidth ? activeWidth : sideWidth;
                            childVM.LineWidth = LineWidth;
                            childVM.LoadAllChildrenWidgets(false);
                            ViewItems.Add(childVM);
                        }
                        else if (item == flicking.PanelStatePages.Last())
                        {
                            childVM.PanelWidth = activeWidth;
                            childVM.LineWidth = (activeWidth - sideWidth) * (-1);
                            childVM.LoadAllChildrenWidgets(false);
                            ViewItems.Insert(0, childVM);
                        }
                        break;
                    case DynamicPanelViewMode.Preview:
                        if (index == 0)
                        {
                            childVM.PanelWidth = activeWidth;
                            childVM.LoadAllChildrenWidgets(false);
                            ViewItems.Add(childVM);
                        }
                        else if (index == 1)
                        {
                            childVM.PanelWidth = ItemWidth - activeWidth;
                            childVM.LineWidth = LineWidth;
                            childVM.LoadAllChildrenWidgets(false);
                            ViewItems.Add(childVM);
                        }
                        break;
                    case DynamicPanelViewMode.Scroll:
                        if (index != 0)
                        {
                            childVM.LineWidth = LineWidth;
                        }
                        childVM.PanelWidth = activeWidth;
                        childVM.LoadAllChildrenWidgets(false);
                        ViewItems.Add(childVM);
                        break;
                }
                index++;
            }

            ShowType = flicking.NavigationType;
        }
        #endregion

        #region UI Binding Propery
        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                return (_model as DynamicPanelModel).Items;
            }
        }

        //used to show navigation icons in swipeview
        public ObservableCollection<DynamicPanelIconNode> NavigationChildren { get; set; }


        //used to show views in swipeview widget, including all widgets in the view.
        public ObservableCollection<DynamicPanelIconNode> ViewItems { get; set; }

        public override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }
            set
            {
                if (base.IsSelected != value)
                {
                    if(SelectionService.GetCurrentPage()!=this.ParentPageVM
                        && value==true)
                    {
                        return;
                    }

                    base.IsSelected = value;
                    FirePropertyChanged("DoubleClickVisibility");
                }                
            }
        }

        public Visibility DoubleClickVisibility
        {
            get
            {
                if (IsSelected)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public Visibility NavigationVisibility
        {
            get
            {
                if (ViewMode == DynamicPanelViewMode.Scroll)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }
        #endregion
           
        #region Property Panel Binding Propery
        public NavigationType ShowType
        {
            get
            {
                return (_model as DynamicPanelModel).ShowType;
            }
            set
            {
                if ((_model as DynamicPanelModel).ShowType != value)
                {
                    (_model as DynamicPanelModel).ShowType = value;
                    FirePropertyChanged("ShowType");
                }

                //Update the Icon list UI
                foreach (DynamicPanelIconNode item in NavigationChildren)
                {
                    switch (value)
                    {
                        case NavigationType.None:
                            item.ShowType = 1;
                            break;
                        case NavigationType.Number:
                            item.ShowType = 2;
                            break;
                        default:
                            item.ShowType = 0;
                            break;
                    }
                }
            }
        }        
        public Guid StartPage
        {
            get
            {
                return (_model as DynamicPanelModel).StartPage;
            }
            set
            {

                if ((_model as DynamicPanelModel).StartPage != value)
                {
                    (_model as DynamicPanelModel).StartPage = value;

                    //ReLoade Icons
                    LoadChildrenIconNode();

                    //ReLoad Items
                    (_model as DynamicPanelModel).LoadAllChildrenWidgets();

                    //Fire Property
                    FirePropertyChanged("StartPage");
                }
            }
        }
        public bool IsShowArrow
        {
            get
            {
                return (_widget as IDynamicPanel).ShowAffordanceArrow;
            }
            set
            {
                if ((_widget as IDynamicPanel).ShowAffordanceArrow != value)
                {
                    (_widget as IDynamicPanel).ShowAffordanceArrow = value;
                    FirePropertyChanged("IsShowArrow");
                }
            }
        }
        public bool IsCirculer
        {
            get
            {
                return (_model as DynamicPanelModel).IsCirculer;
            }
            set
            {
                if ((_model as DynamicPanelModel).IsCirculer != value)
                {
                    (_model as DynamicPanelModel).IsCirculer = value;
                    FirePropertyChanged("IsCirculer");
                }
            }
        }
        public bool IsAutomatic
        {
            get
            {
                return (_model as DynamicPanelModel).IsAutomatic;
            }
            set
            {
                if ((_model as DynamicPanelModel).IsAutomatic != value)
                {
                    (_model as DynamicPanelModel).IsAutomatic = value;
                    FirePropertyChanged("IsAutomatic");
                }
            }
        }

        public DynamicPanelViewMode ViewMode
        {
            get
            {
                return (_model as DynamicPanelModel).ViewMode; 
            }
            set
            {
                if ((_model as DynamicPanelModel).ViewMode != value)
                {
                    (_model as DynamicPanelModel).ViewMode = value;
                    FirePropertyChanged("ViewMode");
                    FirePropertyChanged("NavigationVisibility");
                    LoadChildrenIconNode();
                }
            }
        }

        public int PanelWidth
        {
            get
            {
                return (_model as DynamicPanelModel).PanelWidth;
            }
            set
            {
                if ((_model as DynamicPanelModel).PanelWidth != value)
                {
                    (_model as DynamicPanelModel).PanelWidth = value;
                    FirePropertyChanged("PanelWidth");
                    LoadChildrenIconNode();
                }
            }
        }

        public double LineWidth
        {
            get
            {
                return (_model as DynamicPanelModel).LineWidth;
            }
            set
            {
                if ((_model as DynamicPanelModel).LineWidth != value)
                {
                    (_model as DynamicPanelModel).LineWidth = value;
                    FirePropertyChanged("LineWidth");
                    LoadChildrenIconNode();
                }
            }

        }

        public IDynamicPanel DynamicPanel
        {
            get
            {
                return _widget as IDynamicPanel;
            }
        }
        #endregion

        #region Event handler
        private void RefreshWidgetPageUIHandler(Guid pageGuid)
        {
            IPagePropertyData parentPage = ParentPageVM as IPagePropertyData;
            if(parentPage!=null)
            {
                parentPage.SetIsThumbnailUpdate(true);
            }

            if (pageGuid == Guid.Empty || pageGuid != widgetGID || _widget.ParentPage.IsOpened == false)
                return;

            LoadChildrenIconNode();
            
            (_model as DynamicPanelModel).LoadAllChildrenWidgets();
        }

        private void CloseWidgetPageHandler(IWidget widget)
        {
            if (widget == _widget)
            {
                IsChildPageOpened = false;
            }
        }
        private void DynamicChildren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int i = 1;
            foreach (DynamicPanelIconNode item in NavigationChildren)
            {
                item.ShowNumber = i++;
            }
            ShowType = (_widget as IDynamicPanel).NavigationType;
        }
        #endregion

    }
}