using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service.Document;
using System.Windows.Controls;
using Naver.Compass.Common;
using Naver.Compass.Common.Helper;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.Module
{
    class WidgetListItem : ViewModelBase
    {
        protected WidgetListItem()
        {//for DynamicstatePageitem use
            ParentID = Guid.Empty;
            _Mode = null;
        }

        public WidgetListItem(IRegion data)
        {
            System.Diagnostics.Debug.Assert(data != null);
            ParentID = Guid.Empty;

            _Mode = data;
        }

        #region  Private Data

        protected IRegion _Mode;

        static public IWidget BelongWidget { get; set; }

        public bool InSearch = true;

        private string _searchTxt = "";

        public Guid ParentID { get; set; }

        public IPage ParentPage { get; set; }

        public int Lavel { get; set; }

        static public Guid CurViewID = Guid.Empty;
        //static  public IPageView CurrentView
        // {
        //     get
        //     {
        //         return _curView;
        //     }
        //     set
        //     {
        //         if (!_curView.Guid.Equals(((IPageView)value).Guid))
        //         {
        //             _curView = (IPageView)value;

        //             if (_Mode != null)
        //             {
        //                 UpdateAllFlagByViewChange();
        //             }
        //         }
        //     }
        // }

        #endregion

        #region  Virtual Data

        virtual public Guid WidgetID
        {
            get
            {
                return _Mode.Guid;
            }
            set
            {
                System.Diagnostics.Debug.Assert(false, "Error option to set WidgetID");
            }
        }

        virtual public int zOrder
        {
            get
            {
                return ((IWidget)_Mode).WidgetStyle.Z;
            }
            set { System.Diagnostics.Debug.Assert(false, "Error option to set LostFlag"); }
        }

        virtual public string WidgetName
        {
            get
            {
                if (String.IsNullOrEmpty(_Mode.Name))
                {
                    return "";
                }
                else
                {
                    return _Mode.Name + "  ";
                }
            }
            set
            {
                System.Diagnostics.Debug.Assert(false, "Error option to set WidgetName");
            }
        }

        virtual public string WidgetTypeName
        {
            get
            {
                return "(" + GetWidgetTypeString() + ")";
               // return "(" + GetWidgetTypeString() + "-" + zOrder.ToString() + ")";
            }
        }

        virtual public bool PlaceFlag
        {
            get
            {
                try
                {
                    if (ParentPage != null && ParentPage.PageViews != null)
                    {
                        IPageView CurrentView = ParentPage.PageViews.GetPageView(CurViewID);
                        if (CurrentView != null && CurrentView.Widgets != null && CurrentView.Widgets.Contains(WidgetID))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    NLogger.Info("Get PlaceFlag cause excption:" + ex.Message.ToString());
                    return true;
                }
            }
            set
            {
                System.Diagnostics.Debug.Assert(false, "Error option to set PlaceFlag");
            }
        }

        virtual public bool LostFlag
        {
            get
            {
                if (PlaceFlag)
                {
                    if (ParentPage != null && ParentPage.PageViews != null)
                    {
                        foreach (IPageView childView in ParentPage.PageViews)
                        {
                            if (childView.Widgets.Contains(WidgetID))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                System.Diagnostics.Debug.Assert(false, "Error option to set LostFlag");
            }
        }

        virtual public bool HideFlag
        {
            get
            {
                return !((IWidget)_Mode).GetWidgetStyle(CurViewID).IsVisible;
            }
            set
            {
                System.Diagnostics.Debug.Assert(false, "Error option to set HideFlag");
            }
        }

        virtual public bool InteractiveFlag
        {
            get { return false; }
        }

        virtual public bool DisplayHeadIconFlag
        {
            get { return true; }
        }

        #endregion

        #region Member data

        private List<WidgetListItem> _childrenList;
        public List<WidgetListItem> OrderedChildren
        {
            get
            {
                return _childrenList;
            }
            set
            {
                _childrenList = value;
            }
        }

        private bool _unFilter = true;
        public bool UnFilter
        {
            get
            {
                return _unFilter;
            }
            set
            {
                _unFilter = (bool)value;
                if (!_unFilter)
                {
                    IsSelected = false;
                }
            }
        }

        private ListItemType _itemType = ListItemType.defaultItem;
        public ListItemType ItemType
        {
            get
            {
                return _itemType;
            }
            set
            {
                if (_itemType != (ListItemType)value)
                {
                    _itemType = (ListItemType)value;

                    FirePropertyChanged("ItemType");
                }
            }
        }

        //public ContextMenu CustomContextMenu
        //{
        //    get
        //    {
        //        return GetContentMenu();
        //    }
        //}

        #endregion

        #region Control Data

        bool _bHasChildren = false;
        public bool HasChildren
        {
            get
            {
                return _bHasChildren;
            }
            set
            {
                _bHasChildren = value;
                FirePropertyChanged("HasChildren");
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;

                FirePropertyChanged("IsExpanded");
            }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != (bool)value)
                {
                    _isSelected = (bool)value;

                    IEventAggregator Aggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

                    WidgetSelectionInfo info = new WidgetSelectionInfo();
                    info.WidgetID = WidgetID;
                    info.bSelected = _isSelected;

                    Aggregator.GetEvent<SelectionChangedByItemNotify>().Publish(info);

                }
            }
        }

        private bool _collapseFlag = false;
        public bool CollapseFlag
        {
            get
            {
                return _collapseFlag;
            }
            set
            {
                if (_collapseFlag != (bool)value)
                {
                    _collapseFlag = Convert.ToBoolean(value);

                    FirePropertyChanged("CollapseFlag");
                }
            }
        }

        private bool _OptionFlag = true;
        public bool OptionFlag
        {
            get
            {
               return _OptionFlag  && (PlaceFlag == false);
            }
            set
            {
                if (_OptionFlag != (bool)value)
                {
                    _OptionFlag = Convert.ToBoolean(value);

                    FirePropertyChanged("OptionFlag");
                }
            }
        }

        private bool _enableDropFlag = false;
        public bool EnableDropFlag
        {
            get
            {
                return _enableDropFlag;
            }
            set
            {
                if (_enableDropFlag != (bool)value)
                {
                    _enableDropFlag = Convert.ToBoolean(value);

                    FirePropertyChanged("EnableDropFlag");
                }
            }
        }

        #endregion

        #region Display Data

        public bool InfoMark
        {
            get
            {
                return PlaceFlag || LostFlag;
            }
        }

        #endregion

        #region Public Function

        public void OnSearch(string searchTxt, Stack<WidgetListItem> RecursiveStore)
        {
            _searchTxt = searchTxt;

            string sContent = WidgetName + WidgetTypeName;

            if (string.IsNullOrEmpty(_searchTxt)
                || sContent.ToLower().Contains(_searchTxt.ToLower()))
            {
                this.InSearch = true;
                foreach (var ancestor in RecursiveStore)
                {
                    ancestor.InSearch = true;
                }
            }
            else
            {
                this.InSearch = false;
            }

            RecursiveStore.Push(this);

            if (OrderedChildren != null)
            {
                foreach (var child in OrderedChildren)
                    child.OnSearch(_searchTxt, RecursiveStore);
            }

            RecursiveStore.Pop();
        }

        public void UpdateItemSelectInfo(bool flag)
        {
            if (_isSelected != flag)
            {
                _isSelected = flag;

                FirePropertyChanged("IsSelected");
            }
        }

        public void UpdateTooltip()
        {
            FirePropertyChanged("PlaceFlag");
        }

        public void UpdateHideFlag()
        {
            FirePropertyChanged("HideFlag");
        }

        public void UpdateItemOrder()
        {
            FirePropertyChanged("zOrder");
        }

        public void UpdateWidgetName()
        {
            FirePropertyChanged("WidgetName");
            
        }
        public void UpdateWidgetTypeName()
        {
            FirePropertyChanged("WidgetTypeName");
        }
        virtual public void UpdateAllFlagByViewChange()
        {
            if (ParentPage == null || _Mode == null)
            {
                return;
            }
            FirePropertyChanged("PlaceFlag");
            FirePropertyChanged("LostFlag");
            FirePropertyChanged("HideFlag");
            FirePropertyChanged("OptionFlag");
            FirePropertyChanged("InfoMark");
        }
        #endregion

        #region Private Function
        protected string GetWidgetTypeString()
        {

            switch (((IWidget)_Mode).WidgetType)
            {
                case WidgetType.Shape:
                    return GetShapeTypeString(_Mode as IShape);
                case WidgetType.Image:
                    return GlobalData.FindResource("widgets_Image");
                case WidgetType.DynamicPanel:
                    return GlobalData.FindResource("widgets_SwipeViews");
                case WidgetType.HamburgerMenu:
                    return GlobalData.FindResource("widgets_DrawerMenu");
                case WidgetType.Line:
                    return GetLineTypeString(_Mode as ILine);
                case WidgetType.HotSpot:
                    return GlobalData.FindResource("widgets_Link");
                case WidgetType.TextField:
                    return GlobalData.FindResource("widgets_Textfield");
                case WidgetType.TextArea:
                    return GlobalData.FindResource("widgets_Textarea");
                case WidgetType.DropList:
                    return GlobalData.FindResource("widgets_Droplist");
                case WidgetType.ListBox:
                    return GlobalData.FindResource("widgets_Listbox");
                case WidgetType.Checkbox:
                    return GlobalData.FindResource("widgets_Checkbox");
                case WidgetType.RadioButton:
                    return GlobalData.FindResource("widgets_Radiobutton");
                case WidgetType.Button:
                    return GlobalData.FindResource("widgets_Button");
                case WidgetType.Toast:
                    return GlobalData.FindResource("widgets_Toast");
                case WidgetType.SVG:
                    return GlobalData.FindResource("widgets_SVG");
                default:
                    return "errortype";
            }


        }

        private string GetShapeTypeString(IShape widget)
        {
            if (widget != null)
            {
                switch (widget.ShapeType)
                {
                    case ShapeType.Rectangle:
                        return GlobalData.FindResource("widgets_Rectangle");
                    case ShapeType.RoundedRectangle:
                        return GlobalData.FindResource("widgets_RoundedRectangle");
                    case ShapeType.Ellipse:
                        return GlobalData.FindResource("widgets_Circle");
                    case ShapeType.Diamond:
                        return GlobalData.FindResource("widgets_Diamond");
                    case ShapeType.Triangle:
                        return GlobalData.FindResource("widgets_Triangle");
                    case ShapeType.Paragraph:
                        return GlobalData.FindResource("widgets_Text");
                    default:
                        return "errortype";
                }
            }
            return "errortype";
        }

        private string GetLineTypeString(ILine widget)
        {
            if (widget != null)
            {
                if (widget.Orientation == Naver.Compass.Service.Document.Orientation.Horizontal)
                {
                    return GlobalData.FindResource("widgets_HorizontalLine");
                }
                else
                {
                    return GlobalData.FindResource("widgets_VerticalLine");
                }
            }

            return "errortype";
        }
        #endregion
    }
}
