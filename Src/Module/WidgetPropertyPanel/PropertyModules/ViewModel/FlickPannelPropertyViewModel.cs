using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Controls.Primitives;
using Naver.Compass.Common;

namespace Naver.Compass.Module.Property
{
    class FlickPannelPropertyViewModel : PropertyViewModelBase
    {
       public FlickPannelPropertyViewModel()
        {
            _listNavigation = new List<string>();

            _listNavigation.Add("None");
            //_listNavigation.Add("Number");
            _listNavigation.Add("Dot");

            this.MouseEnterInfoIconCommand = new DelegateCommand<object>(MouseEnterInfoIconExecute);
            this.MouseLeaveInfoIconCommand = new DelegateCommand<object>(MouseLeaveInfoIconExecute);
            this.MouseEnterViewModeCommand = new DelegateCommand<object>(MouseEnterViewModeExecute);
            this.MouseLeaveViewModeCommand = new DelegateCommand<object>(MouseLeaveViewModeExecute);
        }

        #region  private date

        List<string> _listNavigation;
        public DelegateCommand<object> MouseEnterInfoIconCommand { get; private set; }
        public DelegateCommand<object> MouseLeaveInfoIconCommand { get; private set; }
        public DelegateCommand<object> MouseEnterViewModeCommand { get; private set; }
        public DelegateCommand<object> MouseLeaveViewModeCommand { get; private set; }

        #endregion

        #region  Override function

        override protected void OnItemsAdd()
        {
            base.OnItemsAdd();

            //FirePropertyChanged("IsAffrodanceArrow");

            //FirePropertyChanged("IsCirculer");

            FirePropertyChanged("IsAutomatic");

            //FirePropertyChanged("PageList");

            //FirePropertyChanged("IsPageSelectEnable");

            //FirePropertyChanged("StartPage");

            FirePropertyChanged("NaviList");

            FirePropertyChanged("sNavigation");
            FirePropertyChanged("IsNavigation");

        }

        override public void OnPropertyChanged(string args)
        {
            base.OnPropertyChanged(args);
            switch (args)
            {
                //case "IsShowArrow":
                //    {
                //        FirePropertyChanged("IsAffrodanceArrow");
                //    }
                //    break;
                //case "IsCirculer":
                //    {
                //        FirePropertyChanged("IsCirculer");
                //    }
                //    break;
                case "IsAutomatic":
                    {
                        FirePropertyChanged("IsAutomatic");
                    }
                    break;
                //case "StartPage":
                //    {
                //        FirePropertyChanged("StartPage");
                //    }
                //    break;
                case "ShowType":
                    {
                        FirePropertyChanged("sNavigation");
                        FirePropertyChanged("IsNavigation");
                    }
                    break;
                default:
                    break;

            }
        }

        #endregion

        #region Binding property

        public Nullable<bool> IsAffrodanceArrow
        {
            get
            {
                Nullable<bool> sReturn = null;
                foreach (DynamicPanelViewModel wdg in _VMItems)
                {
                    if (wdg != null)
                    {
                        if (sReturn == null)
                        {
                            sReturn = wdg.IsShowArrow;
                        }
                        else if (!sReturn.Equals(wdg.IsShowArrow))
                        {
                            sReturn = null;
                        }
                    }
                }

                return sReturn;
            }
            set
            {
                if (value != null)
                {
                    FlickCommands.ShowArrow.Execute((bool)value, _CmdTarget);
                }
                else
                {
                    FlickCommands.ShowArrow.Execute(false, _CmdTarget);
                }
            }
        }

        public Nullable<bool> IsCirculer
        {
            get
            {
                Nullable<bool> sReturn = null;
                foreach (DynamicPanelViewModel wdg in _VMItems)
                {
                    if (wdg != null)
                    {
                        if (sReturn == null)
                        {
                            sReturn = wdg.IsCirculer;
                        }
                        else if (!sReturn.Equals(wdg.IsCirculer))
                        {
                            sReturn = null;
                        }
                    }
                }

                return sReturn;
            }
            set
            {
                if (value != null)
                {
                    FlickCommands.Circuler.Execute((bool)value,_CmdTarget);
                }
                else
                {
                    FlickCommands.Circuler.Execute(false, _CmdTarget);
                }
            }
        }

        public Nullable<bool> IsNavigation
        {
            get
            {
                switch (sNavigation)
                {
                    case null:
                        return null;
                    case "None":
                        return false;                        
                    //case NavigationType.Number:
                    //    return "Number";
                    case "Dot":
                        return true;                        
                    default:
                        return true;
                }     

            }
            set
            {
                if (value == null)
                {
                    sNavigation = "None";
                }
                else if(value == true)
                {
                    sNavigation = "Dot";
                }
                else
                {
                    sNavigation = "None";
                }
            }
        }
        public Nullable<bool> IsAutomatic
        {
            get
            {
                Nullable<bool> sReturn = null;
                foreach (DynamicPanelViewModel wdg in _VMItems)
                {
                    if (wdg != null)
                    {
                        if (sReturn == null)
                        {
                            sReturn = wdg.IsAutomatic;
                        }
                        else if (!sReturn.Equals(wdg.IsAutomatic))
                        {
                            sReturn = null;
                        }
                    }
                }

                return sReturn;
            }
            set
            {
                if (value != null)
                {
                    FlickCommands.Automatic.Execute((bool)value, _CmdTarget);
                }
                else
                {
                    FlickCommands.Automatic.Execute(false, _CmdTarget);
                }
            }
        }

        public List<string> PageList
        {
            get
            {
                List<string> listPage = new List<string>();
                if (_VMItems.Count == 1)
                {
                    DynamicPanelViewModel wdg = _VMItems[0] as DynamicPanelViewModel;
                    if (wdg != null)
                    {
                        foreach (DynamicPanelIconNode child in wdg.NavigationChildren)
                        {
                            if (child != null)
                            {
                                listPage.Add(child.Name);
                            }
                        }
                    }
                }

                return listPage;
            }
        }

        public bool IsPageSelectEnable
        {
            get
            {
                if (_VMItems.Count == 1)
                {
                    return true;
                }
                return false;
            }
        }

        public string StartPage
        {
            get
            {
                List<string> listPage = new List<string>();
                if (_VMItems.Count == 1)
                {
                    DynamicPanelViewModel wdg = _VMItems[0] as DynamicPanelViewModel;
                    if (wdg != null)
                    {
                        foreach (DynamicPanelIconNode child in wdg.NavigationChildren)
                        {
                            if (child != null && child.PageGID == wdg.StartPage)
                            {
                                return child.Name;
                            }
                        }
                    }
                }

                return null;
            }

            set
            {
                if (value != null)
                {
                    if (_VMItems.Count == 1)
                    {
                        DynamicPanelViewModel wdg = _VMItems[0] as DynamicPanelViewModel;
                        if (wdg != null)
                        {
                            foreach (DynamicPanelIconNode child in wdg.NavigationChildren)
                            {
                                if (child != null && child.Name.Equals(Convert.ToString(value)))
                                {
                                    FlickCommands.StartPage.Execute(child.PageGID, _CmdTarget);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<string> NaviList
        {
            get
            {
                return _listNavigation;
            }
        }

        public string sNavigation
        {
            get
            {
                if (_VMItems.Count == 0)
                {
                    return null;
                }
                else if (_VMItems.Count == 1)
                {
                    DynamicPanelViewModel wdg = _VMItems[0] as DynamicPanelViewModel;
                    if (wdg != null)
                    {
                        //Scroll mode: un check Navigation
                        if (wdg.ViewMode == DynamicPanelViewMode.Scroll)
                            return "None";

                        switch (wdg.ShowType)
                        {
                            case NavigationType.None:
                                return "None";
                            //case NavigationType.Number:
                            //    return "Number";
                            case NavigationType.Dot:
                                return "Dot";
                            default:
                                return "None";
                        }

                    }
                    return null;
                }
                else 
                {
                    NavigationType FirType=(_VMItems[0] as DynamicPanelViewModel).ShowType;
                    foreach (DynamicPanelViewModel item in _VMItems)
                    {
                        if (item.ShowType != FirType)
                        {
                            return null;
                        }
                    }
                    if (FirType == NavigationType.None)
                    {
                        return "None";
                    }
                    else
                    {
                        return "Dot";
                    }
                        
                }
            }

            set
            {
                if (value != null)
                {
                    NavigationType eType = NavigationType.None;
                    switch (Convert.ToString(value))
                    {
                        case "None":
                            eType = NavigationType.None;
                            break;
                        //case "Number":
                        //    eType = NavigationType.Number;
                        //    break;
                        case "Dot":
                             eType = NavigationType.Dot;
                            break;
                    }
                    FlickCommands.Navigation.Execute(eType, _CmdTarget);
                }
            }
        }

        public DynamicPanelViewMode ViewMode
        {
            get
            {
                DynamicPanelViewMode firMode = (_VMItems[0] as DynamicPanelViewModel).ViewMode;
                foreach (DynamicPanelViewModel item in _VMItems)
                {
                    if (item.ViewMode != firMode)
                    {
                        return DynamicPanelViewMode.Full;
                    }
                }
                return firMode;
            }
            set
            {
                FlickCommands.ViewMode.Execute(value, _CmdTarget);
                if (value != DynamicPanelViewMode.Full && PanelWidth == 100)
                {
                    //Set default value to 80 in Scroll/Card/Preview Mode.
                    PanelWidth = 80;
                }
                FirePropertyChanged("ViewMode");
                FirePropertyChanged("IsViewWidthEditable");
                FirePropertyChanged("IsNavigationEnable");
                FirePropertyChanged("IsNavigation");
            }
        }

        public int PanelWidth
        {
            get
            {
                int first = (_VMItems[0] as DynamicPanelViewModel).PanelWidth;
                foreach (DynamicPanelViewModel item in _VMItems)
                {
                    if (item.PanelWidth != first)
                    {
                        return 0;
                    }
                }
                return first;
            }
            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                else if (value >= 100)
                {
                    value = 100;
                    ViewMode = DynamicPanelViewMode.Full;
                }
                   
                FlickCommands.PanelWidth.Execute(value, _CmdTarget);
                FirePropertyChanged("PanelWidth");
            }
        }

        public int LineWidth
        {
            get
            {
                double first = (_VMItems[0] as DynamicPanelViewModel).LineWidth;
                foreach (DynamicPanelViewModel item in _VMItems)
                {
                    if (item.LineWidth != first)
                    {
                        return 0;
                    }
                }
                return int.Parse(first.ToString());
            }
            set
            {
                double newValue;
                if (Double.TryParse(value.ToString(), out newValue))
                {
                    if (newValue >= 0)
                    {
                        FlickCommands.LineWidth.Execute(newValue, _CmdTarget);
                    }
                }
            }
        }

        public bool IsViewWidthEditable
        {
            get
            {
                return (DynamicPanelViewMode.Full != ViewMode);
            }
        }
        public bool IsNavigationEnable
        {
            get
            {
                return (DynamicPanelViewMode.Scroll != ViewMode);
            }
        }
        #endregion

        private void MouseEnterInfoIconExecute(object cmdParameter)
        {
            Popup popup = cmdParameter as Popup;
            popup.IsOpen = true;
        }

        private void MouseLeaveInfoIconExecute(object cmdParameter)
        {
            Popup popup = cmdParameter as Popup;
            popup.IsOpen = false;
        }

        private void MouseEnterViewModeExecute(object cmdParameter)
        {
            GifImage gifImage = cmdParameter as GifImage;
            if(gifImage!=null)
            {
                gifImage.IsAnimating = true;
            }
        }
        private void MouseLeaveViewModeExecute(object cmdParameter)
        {
            GifImage gifImage = cmdParameter as GifImage;
            if (gifImage != null)
            {
                gifImage.IsAnimating = false;
            }
        }
    }
}
