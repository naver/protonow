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
    public class DynamicPanelPreViewModel : WidgetPreViewModeBase
    {

        public DynamicPanelPreViewModel(IWidget widget)
            : base(widget)
        {
            _model = new DynamicPanelModel(widget,true);
            NavigationChildren = new ObservableCollection<DynamicPanelIconNode>();
            ViewItems = new ObservableCollection<DynamicPanelIconNode>();
            _widget = widget as IDynamicPanel;
            IsImgConvertType = false;
            LoadChildrenIconNode();
            (_model as DynamicPanelModel).LoadAllChildrenWidgets();
        }
        public ICommand DoubleClickCommand
        {
            get
            {
                return null;
            }
        }


        #region  private member and function
        private IDynamicPanel _widget;
        private void LoadChildrenIconNode()
        {
            NavigationChildren.Clear();
            ViewItems.Clear();
            IDynamicPanel flicking = (_widget as IDynamicPanel);

            double activeWidth = ItemWidth * 0.01 * _widget.PanelWidth;
            double sideWidth = (ItemWidth - activeWidth - 2 * _widget.LineWith) / 2;

            int index = 0;
            foreach (IPage item in flicking.PanelStatePages)
            {
                DynamicPanelIconNode childVM = new DynamicPanelIconNode(item, _model.StyleGID);
                NavigationChildren.Add(childVM);
                if (flicking.StartPanelStatePage == item)
                {
                    childVM.IsChecked = true;
                }

                switch (_widget.ViewMode)
                {
                    case DynamicPanelViewMode.Full:
                        if (index == 0)
                        {
                            childVM.PanelWidth = ItemWidth;
                            childVM.LoadAllChildrenWidgets(true);
                            ViewItems.Add(childVM);
                        }
                        break;
                    case DynamicPanelViewMode.Card:
                        if (index == 0)
                        {
                            childVM.PanelWidth = activeWidth; ; // (ItemWidth - activeWidh) / 2;
                            childVM.LineWidth = _widget.LineWith;
                            childVM.LoadAllChildrenWidgets(true);
                            ViewItems.Add(childVM);
                        }
                        else if (index == 1)
                        {
                            childVM.PanelWidth = sideWidth;
                            childVM.LineWidth = _widget.LineWith;
                            childVM.LoadAllChildrenWidgets(true);
                            ViewItems.Add(childVM);
                        }
                        else if (item == flicking.PanelStatePages.Last())
                        {
                            childVM.PanelWidth = ItemWidth;
                            childVM.LineWidth = (ItemWidth - sideWidth) * (-1);
                            childVM.LoadAllChildrenWidgets(true);
                            ViewItems.Insert(0, childVM);
                        }
                        break;
                    case DynamicPanelViewMode.Preview:
                        if (index == 0)
                        {
                            childVM.PanelWidth = activeWidth;
                            childVM.LoadAllChildrenWidgets(true);
                            ViewItems.Add(childVM);
                        }
                        else if (index == 1)
                        {
                            childVM.PanelWidth = ItemWidth - activeWidth;
                            childVM.LineWidth = _widget.LineWith;
                            childVM.LoadAllChildrenWidgets(true);
                            ViewItems.Add(childVM);
                        }
                        break;
                    case DynamicPanelViewMode.Scroll:
                        if (index != 0)
                        {
                            childVM.LineWidth = _widget.LineWith;
                        }
                        childVM.PanelWidth = activeWidth;
                        childVM.LoadAllChildrenWidgets(true);
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
        public ObservableCollection<DynamicPanelIconNode> NavigationChildren { get; set; }

        //Views shown in widget.
        public ObservableCollection<DynamicPanelIconNode> ViewItems { get; set; }
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
        #endregion

        #region Override UpdateWidgetStyle2UI Functions
        override protected void UpdateWidgetStyle2UI()
        {
            base.UpdateWidgetStyle2UI();
            //UpdateTextStyle();
            //UpdateFontStyle();
            //UpdateBackgroundStyle();
        }
        #endregion 


    }
}