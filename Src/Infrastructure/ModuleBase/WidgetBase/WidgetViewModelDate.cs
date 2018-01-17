using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service.Document;

namespace Naver.Compass.InfoStructure
{
    public partial class WidgetViewModelDate : ViewModelBase
    {
        #region Private member
        protected IPageElementDataModel _model = null;
        protected bool _isShowInPageView2Adaptive = false;

        public UIElement _viewInput = null;

        private Guid GetCurrentPageViewGID()
        {
            if (_model == null)
            {
                return Guid.Empty;
            }
            return _model.StyleGID;
        }
        virtual protected void UpdateWidgetStyle2UI()
        {
            UpdateCommonStyle();
        }
        private void UpdateCommonStyle()
        {
            FirePropertyChanged("IsHidden");
            FirePropertyChanged("Left");
            FirePropertyChanged("Top");
            FirePropertyChanged("ItemWidth");
            FirePropertyChanged("ItemHeight");
            FirePropertyChanged("ZOrder");
            FirePropertyChanged("RotateAngle");
            FirePropertyChanged("TextRotate");
            FirePropertyChanged("Opacity");
            FirePropertyChanged("IsFixed");
        }
        protected void UpdateFontStyle()
        {
            FirePropertyChanged("vFontFamily");
            FirePropertyChanged("vFontSize");
            FirePropertyChanged("vFontBold");
            FirePropertyChanged("vFontUnderLine");
            FirePropertyChanged("vFontStrickeThrough");
            FirePropertyChanged("vFontColor");
            FirePropertyChanged("vFontItalic");
        }

        protected void UpdateTextStyle()
        {
            FirePropertyChanged("vTextHorAligen");
            FirePropertyChanged("vTextVerAligen");
        }

        protected void UpdateBackgroundStyle()
        {
            FirePropertyChanged("vBackgroundColor");
            FirePropertyChanged("vBorderLineColor");
            FirePropertyChanged("vBorderlineStyle");
            FirePropertyChanged("vBorderLinethinck");
        }

        #endregion

        #region Public function and property

        public UIElement viewInput
        {
            get { return _viewInput; }
            set { _viewInput = value; }
        }

        public IPageElementDataModel WidgetModel
        {
            get { return _model; }
        }
        public WidgetType WidgetType
        {
            get
            {
                return _model.WdgType;
            }
        }
        public ShapeType shapetype
        {
            get
            {
                return _model.shapeType;
            }
        }
        public void SetStyleAsDefaultStyle()
        {
            _model.SetWidgetStyleAsDefaultStyle(GetCurrentPageViewGID());
        }
        virtual public void ChangeCurrentPageView(IPageView targetPageView)
        {
            if (_model == null || targetPageView==null)
            {
                return ;
            }

            //if (targetPageView.Guid == GetCurrentPageViewGID())
            //{
            //    return ;
            //}     
            
            if(true==targetPageView.Widgets.Contains(_model.Guid))
            {
                IsShowInPageView2Adaptive = true;
            }
            else
            {
                IsShowInPageView2Adaptive = false;
            }
            
            bool bRes=_model.ChangeCurrentStyle(targetPageView.Guid);
            if (bRes == true)
            {
                UpdateWidgetStyle2UI();
            }
        }

        #endregion
        
        #region Binding PageView Switch Peropty
        virtual public bool IsShowInPageView2Adaptive
        {
            get { return _isShowInPageView2Adaptive; }
            set
            {
                if (_isShowInPageView2Adaptive != value)
                {
                    _isShowInPageView2Adaptive=value;
                    FirePropertyChanged("IsShowInPageView2Adaptive");
                }
            }
        }

        #endregion

        #region Binding Common Style Property
        virtual public bool IsHidden
        {
            get { return !_model.IsVisible; }
            set
            {
                if (_model.IsVisible != !value)
                {
                    _model.IsVisible = !value;
                    FirePropertyChanged("IsHidden");
                }
            }
        }

        virtual public double Left
        {
            get { return _model.Left; }
            set
            {
                if (_model.Left != value)
                {
                    if (IsLocked == true)
                    {
                        return;
                    }

                    _model.Left = value;
                    FirePropertyChanged("Left");
                }
            }
        }
        virtual public double Raw_Left
        {
            get
            {
                return Left;
            }
            set
            {
                Left = value;
            }
        }
        virtual public double Top
        {
            get { return _model.Top; }
            set
            {
                if (_model.Top != value)
                {
                    if (IsLocked == true)
                    {
                        return;
                    }

                    _model.Top =value;
                    FirePropertyChanged("Top");
                }
            }
        }

        virtual public double Raw_Top
        {
            get { return Top; }
            set
            {
                Top = value;
            }
        }
        virtual public double ItemWidth
        {
            get { return _model.ItemWidth; }
            set
            {
                if (_model.ItemWidth != value)
                {
                    if(IsLocked==true)
                    {
                        return;
                    }

                    _model.ItemWidth = value;
                    FirePropertyChanged("ItemWidth");
                }
            }
        }
        virtual public double Raw_ItemWidth
        {
            get { return ItemWidth; }
            set
            {
                ItemWidth = value;
            }
        }
        virtual public double ItemHeight
        {
            get { return _model.ItemHeight; }
            set
            {
                if (_model.ItemHeight != value)
                {
                    if (IsLocked == true)
                    {
                        return;
                    }

                    _model.ItemHeight = value;
                    FirePropertyChanged("ItemHeight");
                }
            }
        }

        virtual public double Raw_ItemHeight
        {
            get { return ItemHeight; }
            set
            {
                ItemHeight = value;
            }
        }
        virtual public int ZOrder
        {
            get { return _model.ZOrder; }
            set
            {
                if (_model.ZOrder != value)
                {
                    _model.ZOrder = value;
                    FirePropertyChanged("ZOrder");
                }
            }
        }
        virtual public bool IsActual 
        { 
            get; 
            set; 
        }

        virtual public int CornerRadius
        {
            get;
            set;
        }
        virtual public int RotateAngle
        {
            get 
            {
                if (_model == null)
                {
                    return 0;
                }
                return Convert.ToInt32(_model.RotateAngle); 
            }
            set
            {
                if (_model == null)
                {
                    return ;
                }
                if (_model.RotateAngle != Convert.ToDouble(value))
                {
                    _model.RotateAngle = value;
                    FirePropertyChanged("RotateAngle");
                }
            }
        }
        virtual public int TextRotate
        {
            get 
            {
                if (_model == null)
                {
                    return 0;
                }
                return Convert.ToInt32(_model.TextRotate); 
            }
            set
            {
                if (_model == null)
                {
                    return ;
                }
                if (_model.TextRotate != Convert.ToDouble(value))
                {
                    _model.TextRotate = value;
                    FirePropertyChanged("TextRotate");
                }
            }
        }
        virtual public double Opacity
        {
            get
            {
                return _model.Opacity;
            }
            set
            {
                if (_model.Opacity != value)
                {
                    value = Math.Max(0, value);
                    _model.Opacity = value;
                    FirePropertyChanged("Opacity");
                }
            }
        }
        virtual public bool IsFixed
        {
            get
            {
                return _model.IsFixed;
            }
            set
            {
                if (_model.IsFixed != value)
                {
                    _model.IsFixed = value;
                    FirePropertyChanged("IsFixed");
                }
            }
        }

        virtual public Alignment vTextHorAligen
        {
            get
            {
                if (_model == null)
                    return Alignment.Center;
                return _model.cTextHorAligen;
            }
            set
            {
                if (_model.cTextHorAligen != value)
                {
                    _model.cTextHorAligen = value;
                    FirePropertyChanged("vTextHorAligen");
                }
            }
        }
        virtual public Alignment vTextVerAligen
        {
            get
            {
                if (_model == null)
                    return Alignment.Center;
                return _model.cTextVerAligen;
            }
            set
            {
                if (_model.cTextVerAligen != value)
                {
                    _model.cTextVerAligen = value;
                    FirePropertyChanged("vTextVerAligen");
                }
            }
        }

        virtual public StyleColor vBackgroundColor
        {
            get
            {
                if (_model == null)
                    return new StyleColor(ColorFillType.Solid, -1);
                return _model.cBackgroundColor;
            }
            set
            {
                if (!_model.cBackgroundColor.Equals(value))
                {
                    _model.cBackgroundColor = value;
                    FirePropertyChanged("vBackgroundColor");
                }
            }
        }
        virtual public StyleColor vBorderLineColor
        {
            get
            {
                if (_model == null)
                    return new StyleColor(ColorFillType.Solid, -1);
                return _model.cBorderLineColor;
            }
            set
            {
                if (!_model.cBorderLineColor.Equals(value))
                {
                    _model.cBorderLineColor = value;
                    FirePropertyChanged("vBorderLineColor");
                }
            }
        }
        virtual public LineStyle vBorderlineStyle
        {
            get
            {
                if (_model == null)
                    return LineStyle.None;
                return _model.cBorderlineStyle;
            }
            set
            {
                if (_model.cBorderlineStyle != value)
                {
                    _model.cBorderlineStyle = value;
                    FirePropertyChanged("vBorderlineStyle");
                }
            }
        }
        virtual public double vBorderLinethinck
        {
            get
            {
                if (_model == null)
                    return 1;
                return _model.iBorderLineWidth;
            }
            set
            {
                if (_model.iBorderLineWidth != value)
                {
                    _model.iBorderLineWidth = value;
                    FirePropertyChanged("vBorderLinethinck");
                }
            }
        }

        virtual public string vFontFamily
        {
            get
            {
                if (_model == null)
                    return null;
                return _model.sFontFamily;
            }
            set
            {
                if (_model.sFontFamily != value)
                {
                    _model.sFontFamily = value;
                    FirePropertyChanged("vFontFamily");
                }
            }
        }
        virtual public double vFontSize
        {
            get
            {
                if (_model == null)
                    return 10;
                return _model.dFontSize;
            }
            set
            {
                if (_model.dFontSize != value)
                {
                    _model.dFontSize = value;
                    FirePropertyChanged("vFontSize");
                }
            }
        }
        virtual public bool vFontBold
        {
            get
            {
                if (_model == null)
                    return false;
                return _model.bFontBold;
            }
            set
            {
                if (_model.bFontBold != value)
                {
                    _model.bFontBold = value;
                    FirePropertyChanged("vFontBold");
                }
            }
        }

        virtual public bool vFontUnderLine
        {
            get
            {
                if (_model == null)
                    return false;
                return _model.bFontUnderLine;
            }
            set
            {
                if (_model.bFontUnderLine != value)
                {
                    _model.bFontUnderLine = value;
                    FirePropertyChanged("vFontUnderLine");
                    FirePropertyChanged("FontDecoration");
                }
            }
        }

        virtual public bool vFontStrickeThrough
        {
            get
            {
                if (_model == null)
                    return false;
                return _model.bFontStringThrough;
            }
            set
            {
                if (_model.bFontStringThrough != value)
                {
                    _model.bFontStringThrough = value;
                    FirePropertyChanged("vFontStrickeThrough");
                    FirePropertyChanged("FontDecoration");
                }
            }
        }

        virtual public Dictionary<string, bool> FontDecoration
        {
            get
            {
                Dictionary<string, bool> Value = new Dictionary<string, bool>();

                Value["underline"] = vFontUnderLine;
                Value["strikethrough"] = vFontStrickeThrough;

                return Value;
            }
        }

        virtual public Color vFontColor
        {
            get
            {
                if (_model == null)
                    return Color.FromRgb(0, 0, 0);
                return _model.cFontColor;
            }
            set
            {
                if (_model.cFontColor != value)
                {

                    _model.cFontColor = (Color)value;
                    FirePropertyChanged("vFontColor");
                }
            }
        }

        virtual public bool vFontItalic
        {
            get
            {
                if (_model == null)
                    return false;
                return _model.bFontItalic;
            }
            set
            {
                if (_model.bFontItalic != value)
                {
                    _model.bFontItalic = value;
                    FirePropertyChanged("vFontItalic");
                }
            }
        }

        virtual public TextMarkerStyle vTextBulletStyle
        {
            get
            {
                return _model.MarkerStyle;
            }
            set
            {
                if (_model.MarkerStyle != value)
                {
                    _model.MarkerStyle = (TextMarkerStyle)value;
                    FirePropertyChanged("vTextBulletStyle");
                }
            }
        }

        
        #endregion

        #region Binding Common Widget Property
        public string ItemContent
        {
            get
            {
                if (_model != null)
                {
                    return _model.ItemContent; 
                }
                else
                {
                    return string.Empty;
                }
                
            }
            set
            {
                if (_model == null)
                {
                    return ;
                }
                if (_model.ItemContent != value)
                {
                    _model.ItemContent = value;
                    FirePropertyChanged("ItemContent");
                }
            }
        }
        virtual public string Name
        {
            get 
            {
                if (_model == null)
                {
                    return null;
                }
                return _model.DisplayName; 
            }
            set
            {
                if (_model == null)
                {
                    return ;
                }
                if (_model.DisplayName != value)
                {
                    _model.DisplayName = value;
                    FirePropertyChanged("DisplayName");
                }
            }
        }
        virtual public Guid WidgetID
        {
            get { return _model.Guid; }
        }
        virtual public string Tooltip
        {
            get
            {
                if (_model == null)
                {
                    return null;
                }
                return _model.Tooltip;
            }
            set
            {
                if (_model == null)
                {
                    return;
                }
                if (_model.Tooltip != value)
                {
                    _model.Tooltip = value;
                    FirePropertyChanged("Tooltip");
                }
            }
        }
        virtual public bool IsLocked
        {
            get { return _model.IsLocked; }
            set
            {
                if (_model.IsLocked != value)
                {
                    _model.IsLocked = value;
                    FirePropertyChanged("IsLocked");
                }
            }
        }
        #region Binding Text Property, Move it to derived class later
        virtual public string vTextContent
        {
            get
            {
                if (_model == null)
                    return null;
                if (string.IsNullOrEmpty(_model.sTextContent))
                    return null;
                return _model.sTextContent;
            }
            set
            {
                if (_model.sTextContent != value)
                {
                    CompositeCommand cmds = new CompositeCommand();
                    PropertyChangeCommand cmd = new PropertyChangeCommand(this, "Raw_TextContent", _model.sTextContent, value);

                    Raw_TextContent = value;

                    if (CurrentUndoManager != null)
                    {
                        cmds.DeselectAllWidgetsFirst();
                        cmds.AddCommand(cmd);
                        CurrentUndoManager.Push(cmds);
                    }
                    FirePropertyChanged("vTextContent");
                }
            }
        }
        // Workaround for text undo/redo, vTextContent is bond to control and it update automatically 
        // when you change it via double click. Add  Raw_TextContent to set text without handling undo/redo.
        public string Raw_TextContent
        {
            set
            {
                _model.sTextContent = value;
                FirePropertyChanged("vTextContent");
            }
        }
        #endregion
        #endregion 

        #region widget common readonly property
        public Guid RealParentGroupGID
        {
            get
            {
                if (_model != null)
                {
                    return _model.RealParentGroupGID; 
                }
                else
                {
                    return Guid.Empty;
                }
                
            }
           
        }
        #endregion

        #region Binding UI Operation Property
        private bool _isCtrlPressed = false;
        public bool IsCtrlPressed
        {
            get { return _isCtrlPressed; }
            set
            {
                if (_isCtrlPressed != value)
                {
                    _isCtrlPressed = value;
                    FirePropertyChanged("IsCtrlPressed");
                }
            }
        }

        private bool _isShiftPressed = false;
        public bool IsShiftPressed
        {
            get
            {
                return _isShiftPressed;
            }
            set
            {
                if(_isShiftPressed!=value)
                {
                    _isShiftPressed = value;
                    FirePropertyChanged("IsShiftPressed");
                }
            }
        }

        private Visibility _isResing = Visibility.Collapsed;
        public Visibility IsResing
        {
            get
            {
                return _isResing;
            }
            set
            {
                if (_isResing != value)
                {
                    _isResing = value;
                    FirePropertyChanged("IsResing");
                }
            }
        }
        #endregion
        
        #region Undo/Redo
        public PropertyMementos CreateNewPropertyMementos()
        {
            _mementos = new PropertyMementos();
            return _mementos;
        }
        public PropertyMementos PropertyMementos
        {
            get { return _mementos; }
        }
        private PropertyMementos _mementos;
        #endregion        

    }
}
