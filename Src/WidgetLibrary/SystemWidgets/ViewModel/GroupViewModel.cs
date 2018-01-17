using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System.Windows.Media;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{                              
    public class GroupViewModel : WidgetViewModBase
    {
        public GroupViewModel(IGroup group)
        {
            _ExternalGroup = group;
            IsGroup = true;
            CanEdit = true;
            status = GroupStatus.UnSelect;
            _widgetChildrens = new List<WidgetViewModBase>();
            ZOrder = 9999;
            ShowGroupBorder = Visibility.Hidden;
            widgetGID = group.Guid;
            Type = ObjectType.Group;

            _bSupportBorder = true;
            _bSupportBackground = true;

            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            _document = doc.Document;
        }

        #region  Public functions
        public bool IsChild(Guid gid, bool bIsGroup)
        {
            if (bIsGroup == true)
            {
                return IsChildGroup(gid, _ExternalGroup);
            }
            else
            {
                return IsChildWidget(gid, _ExternalGroup);
            }
        }

        public void AddChild(WidgetViewModBase item)
        {
            if (item == null)
                return;
            _widgetChildrens.Add(item);
        }

        public void Refresh()
        {
            FirePropertyChanged("Left");
            FirePropertyChanged("Top");
            FirePropertyChanged("ItemWidth");
            FirePropertyChanged("ItemHeight");
            FirePropertyChanged("IsLocked");
            FirePropertyChanged("RotateAngle");
        }

        public void RefreshProperty(string sPropertyName)
        {
            FirePropertyChanged(sPropertyName);
        }
        public void Move(double OffsetX, double OffsetY)
        {
            foreach (WidgetViewModBase item in _widgetChildrens)
            {
                item.Top += OffsetX;
                item.Left += OffsetY;
            }
            Refresh();
        }

        public List<WidgetViewModBase> GetEligibleWidgetsByProperty(string sProperty)
        {
            switch (sProperty)
            {
                case "vTextContent":
                case "vFontFamily":
                case "vFontSize":
                case "vFontBold":
                case "vFontItalic":
                case "vFontUnderLine":
                case "vFontStrickeThrough":
                case "vFontColor":
                    return GetEligibleWidgets_Group(PropertyOption.Option_Text);
                case "vTextHorAligen":
                    return GetEligibleWidgets_Group(PropertyOption.Option_TextHor);
                case "vTextVerAligen":
                    return GetEligibleWidgets_Group(PropertyOption.Option_TextVer);
                case "vBackgroundColor":
                    return GetEligibleWidgets_Group(PropertyOption.OPtion_BackColor);
                case "vBorderLinethinck":
                case "vBorderLineColor":
                case "vBorderlineStyle":
                    return GetEligibleWidgets_Group(PropertyOption.Option_Border);
                case "LineArrowStyle":
                    return GetEligibleWidgets_Group(PropertyOption.Option_LineArrow);
                case "vTextBulletStyle":
                    return GetEligibleWidgets_Group(PropertyOption.Option_Bullet);
                case "CornerRadius":
                    return GetEligibleWidgets_Group(PropertyOption.Option_CornerRadius);
                case "TextRotate":
                    return GetEligibleWidgets_Group(PropertyOption.Option_TextRotate);
                case "RotateAngle":
                    return GetEligibleWidgets_Group(PropertyOption.Option_WidgetRotate);
                default:
                    return WidgetChildren;
            }
        }

        public GroupStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                if (status != value)
                {
                    //var visibleWidget = _widgetChildrens.FirstOrDefault(x => x.IsShowInPageView2Adaptive == true);
                    //if (visibleWidget == null)
                    //{
                    //    //if all widgets is upplace mode in group, unselect group.
                    //    status = GroupStatus.UnSelect;
                    //    IsShowInPageView2Adaptive = false;
                    //}
                    //else
                    //{
                    //    status = value;
                    //    IsShowInPageView2Adaptive = true;
                    //}
                    
                    //bool bis=this.IsShowInPageView2Adaptive;
                    status = value;                    
                    if (status == GroupStatus.UnSelect)
                    {
                        IsSelected = false;
                        ShowGroupBorder = Visibility.Hidden;
                        SelectionService.RemoveWidget(this);
                        ShowHiddenChildren(false);
                    }
                    else if (status == GroupStatus.Selected)
                    {
                        IsSelected = true;
                        ShowGroupBorder = Visibility.Hidden;
                        SelectionService.RegisterWidget(this);
                        ShowHiddenChildren(true);

                        //Change size in child adaptive view
                        //need to refresh size when back to base.
                        Refresh();
                    }
                    else
                    {
                        IsSelected = false;
                        ShowGroupBorder = Visibility.Visible;
                        SelectionService.RemoveWidget(this);
                        ShowHiddenChildren(false);

                        //Refresh();
                    }
                    FirePropertyChanged("IsShowInPageView2Adaptive");
                }
            }
        }
        public IGroups GroupChildren
        {
            get
            {
                return _ExternalGroup.Groups;
            }

        }
        public IGroup ExternalGroup
        {
            get { return _ExternalGroup; }
        }
        public List<WidgetViewModBase> WidgetChildren
        {
            get
            {
                return _widgetChildrens;
            }

        }
        public void DeSelectAllChildren()
        {
            foreach (WidgetViewModBase it in _widgetChildrens)
            {
                it.IsSelected = false;
            }
        }

        /// <summary>
        /// Show border of hidden children in a group, just show the border
        /// </summary>
        /// <param name="bShow">be shown or not</param>
        public void ShowHiddenChildren(bool bShow)
        {
            foreach (WidgetViewModBase it in _widgetChildrens)
            {
                it.ShowBorderInGroup(bShow);
            }
        }
        #endregion

        #region  Pirvtae functions
        private GroupStatus status;
        private IGroup _ExternalGroup;
        private Visibility _showGroupBorder = Visibility.Visible;
        private List<WidgetViewModBase> _widgetChildrens;
        private IDocument _document = null;
        private bool IsChildGroup(Guid gid, IGroup group)
        {
            if (group == null)
                return false;

            if (group.Guid == gid)
            {
                return true;
            }

            if (group.Groups == null || group.Groups.Count <= 0)
            {
                return false;
            }

            foreach (IGroup item in group.Groups)
            {
                if (true == IsChildGroup(gid, item))
                    return true;
                else
                    continue;
            }
            return false;
        }
        private bool IsChildWidget(Guid gid, IGroup group)
        {
            if (group == null)
                return false;
            return group.IsChild(gid, true);

            //foreach (IWidget item in group.Widgets)
            //{
            //    if (item.Guid == gid)
            //    {
            //        return true;
            //    }
            //}


            //if (group.Groups == null || group.Groups.Count <= 0)
            //{
            //    return false;
            //}

            //foreach (IGroup item in group.Groups)
            //{
            //    if (true == IsChildWidget(gid, item))
            //        return true;
            //    else
            //        continue;
            //}
            //return false;
        }
        public override Rect GetBoundingRectangle(bool isActual=true)
        {
            double x1 = Double.MaxValue;
            double y1 = Double.MaxValue;
            double x2 = Double.MinValue;
            double y2 = Double.MinValue;

            foreach (WidgetViewModBase item in _widgetChildrens)
            {

                double itemLeft = item.Raw_Left;
                double itemTop = item.Raw_Top;
                double itemWidth = item.Raw_ItemWidth;
                double itemHeight = item.Raw_ItemHeight;

                if (item.RotateAngle == 0)
                {
                    x1 = Math.Min(itemLeft, x1);
                    y1 = Math.Min(itemTop, y1);

                    x2 = Math.Max(itemLeft + itemWidth, x2);
                    y2 = Math.Max(itemTop + itemHeight, y2);
                }
                else
                {
                    double x = itemLeft;
                    double y = itemTop;

                    double xc = (itemLeft * 2 + itemWidth) / 2;
                    double yc = (itemTop * 2 + itemHeight) / 2;
                    double angle = Math.Abs(item.RotateAngle) % 180;
                    if (angle > 90)
                    {
                        angle = 180 - angle;
                    }
                    double Kc = Math.Cos(angle * Math.PI / 180);
                    double Ks = Math.Sin(angle * Math.PI / 180);

                    double xr = xc - (Kc * (xc - x) + Ks * (yc - y));
                    double yr = yc - (Ks * (xc - x) + Kc * (yc - y));

                    double width = itemWidth + (x - xr) * 2;
                    double height = itemHeight + (y - yr) * 2;
                    x1 = Math.Min(xr, x1);
                    y1 = Math.Min(yr, y1);

                    x2 = Math.Max(xr + width, x2);
                    y2 = Math.Max(yr + height, y2);
                }

            }
            return new Rect(new System.Windows.Point(x1, y1), new System.Windows.Point(x2, y2));
        }
        private string GetGroupFamily()
        {
            string sValue = "";
         
            foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
            {
                if (!string.IsNullOrEmpty(sValue) && sValue != item.vFontFamily)
                {
                    sValue = "";
                    break;
                }
                else
                {
                    sValue = item.vFontFamily;
                }
            }

            return sValue;
        }
        private double GetGroupFontSize()
        {
            double dValue = double.MinValue;
            foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
            {
                if (dValue != double.MinValue && dValue != item.vFontSize)
                {
                    dValue = -1;
                    break;
                }
                else
                {
                    dValue = item.vFontSize;
                }
            }

            return dValue;
        }

        private TextMarkerStyle GetBulletStyle()
        {

            foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Bullet))
            {
                if (item.vTextBulletStyle != TextMarkerStyle.Disc)
                {
                    return TextMarkerStyle.None;
                }
            }

            return TextMarkerStyle.Disc;
        }
        private bool GetGroupFontBoldFlag()
        {
            bool bReturnValue = false;
            bool bInitValue = true;
            foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
            {
                if (bInitValue != item.vFontBold)
                {
                    bReturnValue = false;
                    break;
                }
                else
                {
                    bReturnValue = true;
                }
            }

            return bReturnValue;
        }
        private bool GetGroupFontItalicFlag()
        {
            bool bReturnValue = false;
            bool bInitValue = true;
            foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
            {
                if (bInitValue != item.vFontItalic)
                {
                    bReturnValue = false;
                    break;
                }
                else
                {
                    bReturnValue = true;
                }
            }

            return bReturnValue;
        }
        private bool GetGroupFontUnderlineFlag()
        {
            bool bReturnValue = false;
            bool bInitValue = true;
            foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
            {
                if (bInitValue != item.vFontUnderLine)
                {
                    bReturnValue = false;
                    break;
                }
                else
                {
                    bReturnValue = true;
                }
            }

            return bReturnValue;
        }
        private bool GetGroupFontStrikthoughFlag()
        {
            bool bReturnValue = false;
            bool bInitValue = true;
            foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
            {
                if (bInitValue != item.vFontStrickeThrough)
                {
                    bReturnValue = false;
                    break;
                }
                else
                {
                    bReturnValue = true;
                }
            }

            return bReturnValue;
        }
        private Color GetGroupFontColor()
        {
            List<WidgetViewModBase> tList = GetEligibleWidgets_Group(PropertyOption.Option_Text);
            if (tList.Count < 1)
            {
                return Color.FromArgb((Byte)255, (Byte)255, (Byte)255, (Byte)255);
            }

            Color RetunColor = tList[0].vFontColor;
            foreach (WidgetViewModBase item in tList)
            {
                if (RetunColor != item.vFontColor)
                {
                    RetunColor = Color.FromArgb((Byte)255, (Byte)255, (Byte)255, (Byte)255);
                    break;
                }
            }
            return RetunColor;
        }
        private Alignment GetTextVerFlag()
        {
            Alignment tAlign = Alignment.None;
            foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_TextVer))
            {
                if (tAlign == Alignment.None)
                {
                    tAlign = item.vTextVerAligen;
                }
                else if (tAlign != item.vTextVerAligen)
                {
                    tAlign = Alignment.None;
                }
            }

            return tAlign;
        }
        private Alignment GetTextHorFlag()
        {
            Alignment tAlign = Alignment.None;
            foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_TextHor))
            {
                if (tAlign == Alignment.None)
                {
                    tAlign = item.vTextHorAligen;
                }
                else if (tAlign != item.vTextHorAligen)
                {
                    tAlign = Alignment.None;
                }
            }

            return tAlign;
        }

        private ArrowStyle GetChildArrowStyle()
        {
            List<WidgetViewModBase> tList = GetEligibleWidgets_Group(PropertyOption.Option_LineArrow);
            ArrowStyle style = ArrowStyle.None;
            bool bFist = false;
            foreach (WidgetLineViewModeBase item in tList)
            {
                if (!bFist)
                {
                    bFist = true;
                    style = item.LineArrowStyle;
                }
                else if (style != item.LineArrowStyle)
                {
                    style = ArrowStyle.None;
                }
            }

            return style;
        }

        private  List<WidgetViewModBase> GetEligibleWidgets_Group(PropertyOption type)
        {
                switch (type)
                {
                    case PropertyOption.Option_Border:
                        return WidgetChildren.FindAll(k => k.IsSupportBorber == true);
                    case PropertyOption.Option_Text:
                        return WidgetChildren.FindAll(k => k.IsSupportText == true);
                    case PropertyOption.Option_LineArrow:
                        return WidgetChildren.FindAll(k => k.IsSupportArrowStyle == true);
                    case PropertyOption.Option_TextHor:
                        return WidgetChildren.FindAll(k => k.IsSupportTextHorAlign == true);
                    case PropertyOption.Option_TextVer:
                        return WidgetChildren.FindAll(k => k.IsSupportTextVerAlign == true);
                    case PropertyOption.Option_Bullet:
                        return WidgetChildren.FindAll(k => k.IsSupportTextRotate == true);
                    case PropertyOption.OPtion_BackColor:
                        return WidgetChildren.FindAll(k => k.IsSupportBackground == true);
                    case PropertyOption.Option_TextRotate:
                        return WidgetChildren.FindAll(k => k.IsSupportTextRotate == true);
                    case PropertyOption.Option_WidgetRotate:
                        return WidgetChildren.FindAll(k => k.IsSupportRotate == true);
                    case PropertyOption.Option_CornerRadius:
                        return WidgetChildren.FindAll(k => k.IsSupportCornerRadius == true);
                    default:
                        System.Diagnostics.Debug.Assert(false, "GetEligibleWidgets_Group function error!");
                        return null;
                }
        }
        #endregion

        #region  Binding Group Common Property(Override)
        override public double Left
        {
            get { return GetBoundingRectangle().Left; }
            set
            {
                double change = value - GetBoundingRectangle().Left;
                if (change != 0)
                {
                    foreach (WidgetViewModBase it in _widgetChildrens)
                    {
                        it.Raw_Left = it.Left + change;
                    }
                    FirePropertyChanged("Left");
                    FirePropertyChanged("ItemWidth");
                }

            }
        }
        override public double Top
        {
            get { return GetBoundingRectangle().Top; }
            set
            {
                double change = value - GetBoundingRectangle().Top;
                if (change != 0)
                {
                    foreach (WidgetViewModBase it in _widgetChildrens)
                    {
                        it.Raw_Top = it.Top + change;
                    }
                    FirePropertyChanged("Top");
                    FirePropertyChanged("ItemHeight");
                }
            }
        }
        override public double ItemWidth
        {
            get { return GetBoundingRectangle().Width; }
            set
            {
                double actual = Math.Max(value,8);
                double scale = actual / GetBoundingRectangle().Width;
                double groupLeft = GetBoundingRectangle().Left;

                foreach (WidgetViewModBase item in _widgetChildrens)
                {
                    if (item.ParentID != widgetGID)
                        continue;

                    Rect BoundRect = item.GetBoundingRectangle();

                    double ItemLeft = BoundRect.Left;
                    double deltaX = (ItemLeft - groupLeft) * (scale - 1);
                    //double deltaW = BoundRect.Width * scale;
                    BoundRect.X += deltaX;
                    BoundRect.Width *= scale;

                    Rect realRec = item.RevertBoundingRectangle(BoundRect);
                    item.Top = realRec.Top;
                    item.Left = realRec.Left;
                    item.ItemWidth = realRec.Width;
                    item.ItemHeight = realRec.Height;
                }
                FirePropertyChanged("ItemWidth");
                FirePropertyChanged("Left");
                
            }
        }
        override public double ItemHeight
        {
            get { return GetBoundingRectangle().Height; }
            set
            {
                double actual = Math.Max(value, 8);
                double scale = actual / GetBoundingRectangle().Height;
                double grouptop = GetBoundingRectangle().Top;

                foreach (WidgetViewModBase item in _widgetChildrens)
                {
                    if (item.ParentID != widgetGID)
                        continue;

                    Rect BoundRect = item.GetBoundingRectangle();
                    double ItemTop = BoundRect.Top;
                    double deltaY = (ItemTop - grouptop) * (scale - 1);
                    BoundRect.Y += deltaY;
                    BoundRect.Height *= scale;

                    Rect realRec = item.RevertBoundingRectangle(BoundRect);
                    item.Top = realRec.Top;
                    item.Left = realRec.Left;
                    item.ItemWidth = realRec.Width;
                    item.ItemHeight = realRec.Height;
                }
                FirePropertyChanged("ItemHeight");
                FirePropertyChanged("Top");
            }
        }

        override public string Name
        {
            get { return _ExternalGroup.Name; }
            set
            {
                if (_ExternalGroup.Name != value)
                {
                    _ExternalGroup.Name = value;
                    FirePropertyChanged("Name");
                    FirePropertyChanged("DisplayName");
                    _document.IsDirty = true;
                }
            }
        }

        override  public string DisplayName
        {
            get { return _ExternalGroup.Name; }
            protected set 
            {
                FirePropertyChanged("DisplayName");
            }
        }

        override public Guid WidgetID
        {
            get { return widgetGID; }
        }
        override public bool IsHidden
        {
            get
            {
                foreach (WidgetViewModBase it in _widgetChildrens)
                {
                    if (it.IsHidden == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            set
            {
                foreach (WidgetViewModBase it in _widgetChildrens)
                {
                    it.IsHidden = value;
                }
                FirePropertyChanged("IsHidden");
            }
        }
        override public bool IsFixed
        {
            get
            {
                foreach (WidgetViewModBase it in _widgetChildrens)
                {
                    if (it.IsFixed == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            set
            {
                foreach (WidgetViewModBase it in _widgetChildrens)
                {
                    it.IsFixed = value;
                }
                FirePropertyChanged("IsFixed");
            }
        }

        override public bool IsSelected
        {
            get { return bIsSelected; }
            set
            {
                if (bIsSelected != value)
                {
                    bIsSelected = value;
                    if (bIsSelected == false)
                    {
                        SelectionService.RemoveWidget(this);
                    }
                    else
                    {
                        SelectionService.RegisterWidget(this);
                    }
                }
                FirePropertyChanged("IsSelected");
            }
        }
        override public int ZOrder
        {
            get 
            {
                return WidgetChildren.Max(a => a.ZOrder);
            }
            set
            {
                
            }
        }

        //Group border when child in group is selected.
        public Visibility ShowGroupBorder
        {
            get { return _showGroupBorder; }
            set
            {
                if (_showGroupBorder != value)
                {
                    _showGroupBorder = value;
                    FirePropertyChanged("ShowGroupBorder");
                }
            }
        }
        public override bool IsLocked
        {
            get
            {
                foreach (WidgetViewModBase item in _widgetChildrens)
                {
                    if (item.IsLocked == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            set
            {
                foreach (WidgetViewModBase item in _widgetChildrens)
                {
                    if (item.IsLocked != value)
                    {
                        item.IsLocked = value;
                    }
                }
                FirePropertyChanged("IsLocked");
            }
        }
        public override double Opacity
        {
            get
            {
                double value = double.NaN;
                if (_widgetChildrens.Count > 0)
                {
                    value = _widgetChildrens[0].Opacity;

                    foreach (WidgetViewModBase item in _widgetChildrens)
                    {
                        if (item.Opacity != value)
                        {
                            return double.NaN;
                        }
                    }
                }
                return value;
            }
            set
            {
                foreach (WidgetViewModBase item in _widgetChildrens)
                {
                    if (item.Opacity != value)
                    {
                        item.Opacity = value;
                    }
                }
                FirePropertyChanged("Opacity");
            }
        }
        override public int TextRotate
        {
            get
            {
                var widgetlist = GetEligibleWidgets_Group(PropertyOption.Option_TextRotate);
                int value = int.MinValue;
                if (widgetlist.Count > 0)
                {
                    value = widgetlist[0].TextRotate;
                    foreach (WidgetViewModBase item in widgetlist)
                    {
                        if (item.TextRotate != value)
                        {
                            return int.MinValue;
                        }
                    }
                }
                return value;
            }
            set
            {
                 var widgetlist = GetEligibleWidgets_Group(PropertyOption.Option_TextRotate);
                foreach (WidgetViewModBase item in widgetlist)
                {
                    if (item.TextRotate != value)
                    {
                        item.TextRotate = value;
                    }
                }
                FirePropertyChanged("TextRotate");
            }
        }
        override public int RotateAngle
        {
            get
            {
                var widgetlist = GetEligibleWidgets_Group(PropertyOption.Option_WidgetRotate);

                int value = int.MinValue;
                if (widgetlist.Count > 0)
                {
                    value = widgetlist[0].RotateAngle;
                    foreach (WidgetViewModBase item in widgetlist)
                    {
                        if (item.RotateAngle != value)
                        {
                            return int.MinValue;
                        }
                    }
                }
                return value;
            }
            set
            {
                bool bIsRefresh = false;

                var widgetlist = GetEligibleWidgets_Group(PropertyOption.Option_WidgetRotate);

                foreach (WidgetViewModBase item in widgetlist)
                {
                    if (item.RotateAngle != value)
                    {
                        item.RotateAngle = value;
                        bIsRefresh = true;
                    }
                }
                if (bIsRefresh == true)
                {
                    Refresh();
                }
                FirePropertyChanged("RotateAngle");
            }
        }

        override public int CornerRadius
        {
            get
            {
                var widgetlist = GetEligibleWidgets_Group(PropertyOption.Option_CornerRadius);

                int value = int.MinValue;
                if (widgetlist.Count > 0)
                {
                    value = widgetlist[0].CornerRadius;
                    foreach (WidgetViewModBase item in widgetlist)
                    {
                        if (item.CornerRadius != value)
                        {
                            return int.MinValue;
                        }
                    }
                }
                return value;
            }
            set
            {
                var widgetlist = GetEligibleWidgets_Group(PropertyOption.Option_CornerRadius);
                foreach (WidgetViewModBase item in widgetlist)
                {
                    if (item.CornerRadius != value)
                    {
                        item.CornerRadius = value;
                    }
                }
                FirePropertyChanged("CornerRadius");
            }
        }
        #endregion

        #region Binding Group Font And Text Property(Override)

        override public ArrowStyle LineArrowStyle
        {
            get
            {
                return GetChildArrowStyle();
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_LineArrow))
                {
                    System.Diagnostics.Debug.Assert(item is WidgetLineViewModeBase, "Set Group child ArrowStyle error");

                    ((WidgetLineViewModeBase)item).LineArrowStyle = (ArrowStyle)value;

                }
                FirePropertyChanged("LineArrowStyle");
            }
        }

        //Font family
        override public string vFontFamily
        {
            get
            {
                return GetGroupFamily();
            }
            set
            {
                if (GetGroupFamily() != value.ToString())
                {
                    foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
                    {
                        if (item.vFontFamily != value.ToString())
                        {
                            item.vFontFamily = value.ToString();
                        }
                    }
                    FirePropertyChanged("vFontFamily");
                }
            }
        }
        //Font size
        override public double vFontSize
        {
            get
            {
                return GetGroupFontSize();
            }
            set
            {
                if (GetGroupFontSize() != value)
                {
                    foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
                    {
                        if (item.vFontSize != value)
                        {
                            item.vFontSize = value;
                        }
                    }
                    FirePropertyChanged("vFontSize");
                }

            }
        }
        //Font bold style
        override public bool vFontBold
        {
            get
            {
                return GetGroupFontBoldFlag();
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
                {
                    if (item.vFontBold != value)
                    {
                        item.vFontBold = value;
                    }
                    FirePropertyChanged("vFontBold");
                }
            }
        }
        //// Font Decorations style
        // override public Dictionary<int, bool> uFontDecorations
        // {

        //     set
        //     {
        //         Dictionary<int, bool> tStyles = value as Dictionary<int, bool>;

        //         foreach (WidgetViewModBase item in _widgetChildrens)
        //         {
        //             item.uFontDecorations = tStyles;
        //         }
        //         FirePropertyChanged("uFontDecorations");
        //     }
        // }
        override public bool vFontUnderLine
        {
            get
            {
                return GetGroupFontUnderlineFlag();
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
                {
                    if (item.vFontUnderLine != value)
                    {
                        item.vFontUnderLine = value;
                    }
                }
                FirePropertyChanged("vFontUnderLine");
            }
        }
        //Font stringthroug style
        override public bool vFontStrickeThrough
        {
            get
            {
                return GetGroupFontStrikthoughFlag();
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
                {
                    if (item.vFontStrickeThrough != value)
                    {
                        item.vFontStrickeThrough = value;
                    }
                }
                FirePropertyChanged("vFontStrickeThrough");
            }
        }

        //TextBulletStyle
        override public TextMarkerStyle vTextBulletStyle
        {
            get
            {
                return GetBulletStyle();
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Bullet))
                {
                    if (item.vTextBulletStyle != (TextMarkerStyle)value)
                    {
                        item.vTextBulletStyle = (TextMarkerStyle)value;
                    }
                    FirePropertyChanged("vTextBulletStyle");
                }
            }
        }
        //Font color
        override public Color vFontColor
        {
            get
            {
                return GetGroupFontColor();
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
                {
                    if (item.vFontColor != value)
                    {
                        item.vFontColor = value;
                    }
                }
                FirePropertyChanged("vFontColor");
            }
        }
        //Font italic style
        override public bool vFontItalic
        {
            get
            {
                return GetGroupFontItalicFlag();
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Text))
                {
                    if (item.vFontItalic != value)
                    {
                        item.vFontItalic = value;
                    }
                }
                FirePropertyChanged("vFontItalic");
            }
        }
        //Background color
        override public StyleColor vBackgroundColor
        {
            get
            {

                List<WidgetViewModBase> tList = GetEligibleWidgets_Group(PropertyOption.OPtion_BackColor);
                //return Color.White;
                if (tList.Count < 1)
                {
                    return new StyleColor(ColorFillType.Solid, -1);
                }

                var RetunColor = tList[0].vBackgroundColor;
                foreach (WidgetViewModBase item in tList)
                {
                    if (!RetunColor.Equals(item.vBackgroundColor))
                    {
                        RetunColor = new StyleColor(ColorFillType.Solid, -1);
                        break;
                    }
                }
                return RetunColor;
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.OPtion_BackColor))
                {
                    if (!item.vBackgroundColor.Equals(value))
                    {
                        if (value.FillType == ColorFillType.Solid || (value.FillType == ColorFillType.Gradient && item.IsSupportGradientBackground))
                        {
                            item.vBackgroundColor = value;
                        }
                    }
                }
                FirePropertyChanged("vBackgroundColor");
            }
        }
        //BorderLine color
        override public StyleColor vBorderLineColor
        {
            get
            {
                List<WidgetViewModBase> tList = GetEligibleWidgets_Group(PropertyOption.Option_Border);
                //return Color.White;
                if (tList.Count < 1)
                {
                    return new StyleColor(ColorFillType.Solid, -1);
                }

                var RetunColor = tList[0].vBorderLineColor;
                foreach (WidgetViewModBase item in tList)
                {
                    if (!RetunColor.Equals(item.vBorderLineColor))
                    {
                        RetunColor = new StyleColor(ColorFillType.Solid, -1);
                        break;
                    }
                }
                return RetunColor;
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Border))
                {
                    if (!item.vBorderLineColor.Equals(value))
                    {
                        if (value.FillType == ColorFillType.Solid || (value.FillType == ColorFillType.Gradient && item.IsSupportGradientBorderline))
                        {
                            item.vBorderLineColor = value;
                        }
                    }
                }
                FirePropertyChanged("vBorderLineColor");
            }
        }
        override public LineStyle vBorderlineStyle
        {
            get
            {
                LineStyle rStyle = LineStyle.None;

                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Border))
                {
                    if (rStyle == LineStyle.None)
                    {
                        rStyle = item.vBorderlineStyle;
                    }
                    else if (rStyle != item.vBorderlineStyle)
                    {
                        rStyle = LineStyle.None;
                        break;
                    }
                    else
                    {
                        rStyle = item.vBorderlineStyle;
                    }
                }

                return rStyle;
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Border))
                {
                    if (item.vBorderlineStyle != value)
                    {
                        item.vBorderlineStyle = value;
                    }
                }
                FirePropertyChanged("vBorderlineStyle");
            }
        }
        override public double vBorderLinethinck
        {
            get
            {
                double rValue = -2;
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Border))
                {
                    if (rValue == -2)
                    {
                        rValue = item.vBorderLinethinck;
                    }
                    else if (rValue != item.vBorderLinethinck)
                    {
                        rValue = -2;
                        break;
                    }
                    else
                    {
                        rValue = item.vBorderLinethinck;
                    }
                }
                return rValue;
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_Border))
                {
                    if (item.vBorderLinethinck != value)
                    {
                        item.vBorderLinethinck = value;
                    }
                }
                FirePropertyChanged("vBorderLinethinck");
            }
        }
        //Text horizontal style
        override public Alignment vTextHorAligen
        {
            get
            {
                return GetTextHorFlag();
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_TextHor))
                {
                    item.vTextHorAligen = value;
                }
                FirePropertyChanged("vTextHorAligen");
            }
        }
        //Text vertical style
        override public Alignment vTextVerAligen
        {
            get
            {
                return GetTextVerFlag();
            }
            set
            {
                foreach (WidgetViewModBase item in GetEligibleWidgets_Group(PropertyOption.Option_TextVer))
                {
                    item.vTextVerAligen = value;
                }
                FirePropertyChanged("vTextVerAligen");
            }
        }

       
        #endregion

        #region  Override Property and function

        override public bool IsSupportText
        {
            get
            {
                return _widgetChildrens.Exists(k => k.IsSupportText == true);
            }
        }
        override public  bool IsSupportGradientBackground
        {
            get
            {
                return _widgetChildrens.Exists(k => k.IsSupportGradientBackground == true);
            }
        }
        override public bool IsSupportGradientBorderline
        {
            get
            {
                return _widgetChildrens.Exists(k => k.IsSupportGradientBorderline == true);
            }
        }

        override public bool IsSupportTextVerAlign
        {
            get 
            {
                return _widgetChildrens.Exists(k => k.IsSupportTextVerAlign == true);
            }
        }
        override public bool IsSupportTextHorAlign
        {
            get
            {
                return _widgetChildrens.Exists(k => k.IsSupportTextHorAlign == true);
            }
        }
        override public bool IsSupportRotate
        {
            get
            {
                return _widgetChildrens.Exists(k => k.IsSupportRotate == true);
            }
        }
        override public bool IsSupportTextRotate
        {
            get
            {
                return _widgetChildrens.Exists(k => k.IsSupportTextRotate == true);
            }
        }

        override public bool IsSupportArrowStyle
        {
            get
            {
                return _widgetChildrens.Exists(k => k.IsSupportArrowStyle == true);
            }
        }

        override public bool IsSupportBackground
        {
            get
            {
                return _widgetChildrens.Exists(k => k.IsSupportBackground == true);
            }
        }

        override public bool IsSupportBorber
        {
            get
            {
                return _widgetChildrens.Exists(k => k.IsSupportBorber == true);
            }
        }

        public override bool IsSupportCornerRadius
        {
            get
            {
                return _widgetChildrens.Exists(k => k.IsSupportCornerRadius == true);
            }
        }
        #endregion

        #region Override UpdateWidgetStyle2UI Functions
        override public void ChangeCurrentPageView(IPageView targetPageView)
        {
            FirePropertyChanged("IsShowInPageView2Adaptive");
            if (IsShowInPageView2Adaptive == true)
            {
                UpdateWidgetStyle2UI();
            }
        }
        override protected void UpdateWidgetStyle2UI()
        {
            base.UpdateWidgetStyle2UI();
            //UpdateTextStyle();
            //UpdateFontStyle();
            //UpdateBackgroundStyle();            
        }

        bool _IsGroupShowInView2Adaptive;
        public bool IsGroupShowInView2Adaptive
        {
            get
            {
                return _IsGroupShowInView2Adaptive;
            }
            set
            {
                if(_IsGroupShowInView2Adaptive!=value)
                {
                    _IsGroupShowInView2Adaptive=value;
                    FirePropertyChanged("IsGroupShowInView2Adaptive");
                }
            }
        }
        override public bool IsShowInPageView2Adaptive
        {
            get
            { 
                //return _isShowInPageView2Adaptive;
                var visibleWidget = 
                    _widgetChildrens.FirstOrDefault(x => x.IsShowInPageView2Adaptive == true);
                if (visibleWidget == null)
                {
                    IsGroupShowInView2Adaptive = false;
                    return false;
                }
                IsGroupShowInView2Adaptive = true;
                return true;
            }
            set
            {
                IsGroupShowInView2Adaptive = value;
                FirePropertyChanged("IsShowInPageView2Adaptive");
            }
        }
        #endregion 

        #region  Pirvtae Children Operation Help functions
        private bool GetTargetChildGroup(Guid gid, IGroup group, ref IGroup TargetGroup)
        {
            if (group == null)
                return false;

            if (group.Guid == gid)
            {
                TargetGroup = group;
                return true;
            }

            if (group.Groups == null || group.Groups.Count <= 0)
            {
                return false;
            }

            foreach (IGroup item in group.Groups)
            {
                if (true == GetTargetChildGroup(gid, item, ref TargetGroup))
                {
                    TargetGroup = item;
                    return true;
                }
                else
                {
                    continue;
                }

            }
            return false;
        }
        public List<IZOrderTopChildObj> GetSpecificTopLevelZOrderChildren(Guid groupGId)
        {
            IGroup TargetGroup = null;
            bool bRes=GetTargetChildGroup(groupGId, _ExternalGroup, ref TargetGroup);
            if (bRes == false || TargetGroup==null)
            {
                return null;
            }

            //Add child group object
            List<IZOrderTopChildObj> Objs=new List<IZOrderTopChildObj>();
            foreach(IGroup itm in TargetGroup.Groups)
            {
               Objs.Add( new ZOrderTopChildGroup(itm,_widgetChildrens));
            }

            //Add child widget or Master object
            foreach(WidgetViewModBase item in _widgetChildrens)
            {
                if(item.RealParentGroupGID==TargetGroup.Guid)
                {
                     Objs.Add( new ZOrderTopChildWidget(item));
                }            
            }
            return Objs;
        }
        public List<WidgetViewModBase> GetSpecificGroupAllChildren(Guid realGroupGId)
        {
            IGroup TargetGroup = null;
            List<WidgetViewModBase> children = new List<WidgetViewModBase>();
            bool bRes = GetTargetChildGroup(realGroupGId, _ExternalGroup, ref TargetGroup);
            if (bRes == false || TargetGroup == null)
            {
                return children;
            }

            foreach (WidgetViewModBase item in _widgetChildrens)
            {
                if (TargetGroup.IsChild(item.WidgetID,true)==true)
                {
                    children.Add(item);
                }
            }
            return children;
        }
        #endregion
    }

    public interface IZOrderTopChildObj
    {
        int ZOrdder{get;}
        int Count{get;}
        int ZOrderDel { get; set; }
        List<WidgetViewModBase> GetChildren();
        bool IsSelected{get;}
        void IncreaseZOrder(int nNum);
        void DecreaseZOrder(int nNum);    
    }

    public class ZOrderTopChildGroup:IZOrderTopChildObj
    {
        #region Constructor
        public ZOrderTopChildGroup(IGroup childGroup,List<WidgetViewModBase> AllWidgets)
        {
            _widgetChildrens = new List<WidgetViewModBase>();
            ZOrderDel = 0;

            foreach(WidgetViewModBase item in AllWidgets)
            {
                if(childGroup.IsChild(item.WidgetID,true))
                {

                    _widgetChildrens.Add(item);
                }
            }
        
        }
        #endregion Constructor

        #region  Pirvtae functions
        private List<WidgetViewModBase> _widgetChildrens;
        #endregion

        #region Interface
        public int ZOrdder
        {
            get
            {
                return _widgetChildrens.Max(a => a.ZOrder);
            }
        }
        public int Count
        {
            get
            {
                return _widgetChildrens.Count();
            }
        }
        public int ZOrderDel { get; set; }
        public bool IsSelected
        {
            get
            {
                return false;
            }
        }
        public List<WidgetViewModBase> GetChildren()
        {
            return _widgetChildrens;          
        }
        public void IncreaseZOrder(int nNum)
        {
            foreach (WidgetViewModBase item in _widgetChildrens)
            {
                item.ZOrder = item.ZOrder + nNum;
            }
            ZOrderDel = ZOrderDel + nNum;
        }
        public void DecreaseZOrder(int nNum)
        {
            foreach (WidgetViewModBase item in _widgetChildrens)
            {
                item.ZOrder = item.ZOrder - nNum;
            }
            ZOrderDel = ZOrderDel - nNum;
        }
        #endregion Interface

    }
    public class ZOrderTopChildWidget : IZOrderTopChildObj
    {
        #region Constructor
        public ZOrderTopChildWidget(WidgetViewModBase wdg)
        {
            _widgetChild = wdg;
            ZOrderDel = 0;
        }
        #endregion Constructor

        #region  Pirvtae functions
        private WidgetViewModBase _widgetChild;
        #endregion

        #region Interface
        public int ZOrdder
        {
            get
            {
                return _widgetChild.ZOrder;
            }
        }
        public int Count
        {
            get
            {
                return 1;
            }
        }
        public int ZOrderDel { get; set; }
        public bool IsSelected
        {
            get
            {
                return _widgetChild.IsSelected;
            }
        }
        public List<WidgetViewModBase> GetChildren()
        {
            List<WidgetViewModBase> children = new List<WidgetViewModBase>();
            children.Add(_widgetChild);
            return children;
        }
        public void IncreaseZOrder(int nNum)
        {
            _widgetChild.ZOrder = _widgetChild.ZOrder + nNum;
            ZOrderDel = ZOrderDel + nNum;
        }
        public void DecreaseZOrder(int nNum)
        {
            _widgetChild.ZOrder = _widgetChild.ZOrder - nNum;
            ZOrderDel = ZOrderDel - nNum;
        }
        #endregion Interface

    }
    public class ZOrderTopChildMask : IZOrderTopChildObj
    {
        #region Constructor
        public ZOrderTopChildMask(WidgetViewModBase wdg)
        {
            _widgetChild = wdg;
            ZOrderDel = 0;
        }
        #endregion Constructor

        #region  Pirvtae functions
        private WidgetViewModBase _widgetChild;
        #endregion

        #region Interface
        public int ZOrdder
        {
            get
            {
                return _widgetChild.ZOrder;
            }
        }
        public int Count
        {
            get
            {
                return 1;
            }
        }
        public int ZOrderDel { get; set; }
        public bool IsSelected
        {
            get
            {
                return _widgetChild.IsSelected;
            }
        }
        public List<WidgetViewModBase> GetChildren()
        {
            List<WidgetViewModBase> children = new List<WidgetViewModBase>();
            children.Add(_widgetChild);
            return children;
        }
        public void IncreaseZOrder(int nNum)
        {

            _widgetChild.ZOrder = _widgetChild.ZOrder + nNum;
            ZOrderDel = ZOrderDel + nNum;
        }
        public void DecreaseZOrder(int nNum)
        {
            _widgetChild.ZOrder = _widgetChild.ZOrder - nNum;
            ZOrderDel = ZOrderDel - nNum;
        }
        #endregion Interface
    }
}
