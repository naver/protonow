using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;
using System.Windows.Media;
using System.Windows;
using Naver.Compass.Common.Helper;

namespace Naver.Compass.InfoStructure
{

    public class WidgetModel:IPageElementDataModel
    {
        public WidgetModel(IWidget widget)
        {
            _widget = widget;
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            _document = doc.Document;
            _style=widget.WidgetStyle;
       
            //cTextHorAligen = Alignment.Center;
            //cTextVerAligen = Alignment.Center;
        }

        #region Public property and functions
        public IRegion WdgDom
        {
            get { return _widget; }
        }
        public WidgetType WdgType
        {
            get { return _widget.WidgetType; }
        }
        public ShapeType shapeType
        {
            get
            {
                IShape shape = _widget as IShape;
                if (shape != null)
                {
                    return shape.ShapeType;
                }
                else
                {
                    throw new Exception("Not shape type");
                }

            }
        }
        public IRegionStyle Style
        {
            get { return _style; }
        }
        public Guid StyleGID
        {
            get { return _style.ViewGuid; }
        }
        #endregion

        #region public functions
        public virtual bool ChangeCurrentStyle(Guid newStyleGid)
        {
            //if (newStyleGid == StyleGID)
            //{
            //    return false;
            //}

            if (newStyleGid == Guid.Empty)
            {
                _style = _widget.WidgetStyle;
            }
            else
            {
                _style = _widget.GetWidgetStyle(newStyleGid);
                if (_style == null)
                {
                    _style = _widget.WidgetStyle;
                }
            }
            return true;
        }
        public void SetWidgetStyleAsDefaultStyle(Guid StyleGid)
        {
            _widget.SetWidgetStyleAsDefaultStyle(StyleGid);
        }
        public void SerializeObject(ISerializeWriter writer)
        {
            writer.AddWidget(_widget);
        }
        public IRegionStyle GetSpecStyle(Guid gid)
        {
            return _widget.GetWidgetStyle(gid);
        }
        #endregion

        #region Private member
        protected IWidgetStyle _style = null;
        protected IWidget _widget = null;
        protected IDocument _document = null;
        private void ValidateValue(ref double value,double max)
        {
            if (value < CommonDefine.MinWidgetSize)
            {
                value = CommonDefine.MinWidgetSize;
            }
            if (value > max)
            {
                value = max;
            }
        }
        #endregion private member

        #region Binding Style Property
        virtual public bool IsVisible
        {
            get { return _style.IsVisible; }
            set
            {
                if (_style.IsVisible != value)
                {
                    _style.IsVisible = value;
                    _document.IsDirty = true;
                }
            }
        }
        virtual public double Left
        {
            get { return _style.X; }
            set
            {
                if (_style.X != value)
                {
                    _style.X = value;
                    _document.IsDirty = true;
                }
            }
        }
        virtual public double Top
        {
            get { return _style.Y; }
            set
            {
                if (_style.Y != value)
                {
                    _style.Y = value;
                    _document.IsDirty = true;
                }
            }
        }
        virtual public int CornerRadius
        {
            get { return _style.CornerRadius; }
            set
            {
                if (_style.CornerRadius != value)
                {
                    _style.CornerRadius = value;
                    _document.IsDirty = true;
                }
            }
        }
        virtual public double ItemWidth
        {
            get { return _style.Width; }
            set
            {
                if (_style.Width != value)
                {
                    ValidateValue(ref value, CommonDefine.MaxEditorWidth);
                    _style.Width = value;
                    _document.IsDirty = true;
                }
            }
        }
        virtual public double ItemHeight
        {
            get { return _style.Height; }
            set
            {
                if (_style.Height != value)
                {
                    ValidateValue(ref value,CommonDefine.MaxEditorHeight);
                    _style.Height = value;
                    _document.IsDirty = true;
                }
            }
        }

        //this is temporary solution for widget manger Zorder change, And TODO:.....
        public int ZOrder
        {
            get { return _widget.WidgetStyle.Z; }
            set
            {
                if (_widget.WidgetStyle.Z != value)
                {
                    _widget.WidgetStyle.Z = value;
                    _document.IsDirty = true;
                }
            }
        }
        public double RotateAngle
        {
            get { return _style.WidgetRotate; }
            set
            {
                if (_style.WidgetRotate != value)
                {
                    _style.WidgetRotate = value;
                    _document.IsDirty = true;
                }
            }
        }
        public double TextRotate
        {
            get { return _style.TextRotate; }
            set
            {
                if (_style.TextRotate != value)
                {
                    _style.TextRotate = value;
                    _document.IsDirty = true;
                }
            }
        }
        public double Opacity
        {
            get
            {
                return Convert.ToDouble(_style.Opacity) / 100;
            }
            set
            {
                if (_style.Opacity != (value * 100))
                {
                    _style.Opacity = Convert.ToInt32(value * 100);
                    _document.IsDirty = true;
                }
            }
        }
       virtual  public bool IsFixed
        {
            get { return _style.IsFixed; }
            set
            {
                if (_style.IsFixed != value)
                {
                    _style.IsFixed = value;
                    _document.IsDirty = true;
                }
            }
        }

        //===================================================================================
        public StyleColor cBackgroundColor
        {
            get { return _style.FillColor; }
            set
            {

                if (!_style.FillColor.Equals(value))
                {
                    _style.FillColor = value;
                    _document.IsDirty = true;
                }
            }
        }
        public StyleColor cBorderLineColor
        {
            get { return _style.LineColor; }
            set
            {

                if (!_style.LineColor.Equals(value))
                {
                    _style.LineColor = value;
                    _document.IsDirty = true;
                }
            }
        }
        public LineStyle cBorderlineStyle
        {
            get { return _style.LineStyle; }
            set
            {

                if (_style.LineStyle != value)
                {
                    _style.LineStyle = value;
                    _document.IsDirty = true;
                }
            }
        }
        public double iBorderLineWidth
        {
            get { return _style.LineWidth; }
            set
            {
                if (_style.LineWidth != value)
                {
                    _style.LineWidth = value;
                    _document.IsDirty = true;
                }
            }
        }

        public Alignment cTextHorAligen
        {
            get { return _style.HorzAlign; }
            set
            {
                if (_style.HorzAlign != value)
                {
                    _style.HorzAlign = value;
                    _document.IsDirty = true;
                }
            }
        }
        public Alignment cTextVerAligen
        {
            get { return _style.VertAlign; }
            set
            {
                if (_style.VertAlign != value)
                {
                    _style.VertAlign = value;
                    _document.IsDirty = true;
                }
            }
        }

        //Text Property
        public string sFontFamily
        {
            get { return _style.FontFamily; }
            set
            {
                if (_style.FontFamily != value)
                {
                    _style.FontFamily = value;
                    _document.IsDirty = true;
                }
            }
        }
        public double dFontSize
        {
            get { return _style.FontSize; }
            set
            {
                if (_style.FontSize != value)
                {
                    _style.FontSize = value;
                    _document.IsDirty = true;
                }
            }
        }
        public bool bFontBold
        {
            get { return _style.Bold; }
            set
            {
                if (_style.Bold != value)
                {
                    _style.Bold = value;
                    _document.IsDirty = true;
                }
            }
        }
        public bool bFontUnderLine
        {
            get { return _style.Underline; }
            set
            {
                if (_style.Underline != value)
                {
                    _style.Underline = value;
                    _document.IsDirty = true;
                }
            }
        }
        public bool bFontStringThrough
        {
            get { return _style.Strikethrough; }
            set
            {
                if (_style.Strikethrough != value)
                {
                    _style.Strikethrough = value;
                    _document.IsDirty = true;
                }
            }
        }
        public Color cFontColor
        {
            get { return CommonFunction.CovertIntToColor(_style.FontColor.ARGB); }
            set
            {

                if (CommonFunction.CovertIntToColor(_style.FontColor.ARGB) != value)
                {
                    StyleColor color = new StyleColor(ColorFillType.Solid, CommonFunction.CovertColorToInt((Color)value));
                    _style.FontColor = color;
                    _document.IsDirty = true;
                }
            }
        }
        public bool bFontItalic
        {
            get { return _style.Italic; }
            set
            {
                if (_style.Italic != value)
                {
                    _style.Italic = value;
                    _document.IsDirty = true;
                }
            }
        }
        public TextMarkerStyle MarkerStyle
        {
            get { return _style.BulletedList; }
            set
            {
                if (_style.BulletedList != value)
                {
                    _style.BulletedList = value;
                    _document.IsDirty = true;
                }
            }
        }
        //===================================================================================//linearrow

        //Now unused
        //public bool Baseline { get; set; }
        //public bool Overline { get; set; }
        //public bool BulletedList { get; set; }
        //public int CornerRadius { get; set; }
        #endregion

        #region Binding common property
        public string ItemContent
        {
            get { return _widget.Text; }
            set
            {
                if (_widget.Text != value)
                {
                    _widget.Text = value;
                    _document.IsDirty = true;
                }
            }
        }
        public string DisplayName
        {
            get { return _widget.Name; }
            set
            {
                if (_widget.Name != value)
                {
                    _widget.Name = value;
                    _document.IsDirty = true;
                }
            }
        }
        virtual public string Tooltip
        {
            get { return _widget.Tooltip; }
            set
            {
                if (_widget.Tooltip != value)
                {
                    _widget.Tooltip = value;
                    _document.IsDirty = true;
                }
            }
        }
        public Guid Guid
        {
            get { return _widget.Guid; }
        }
        public bool IsLocked
        {
            get { return _widget.IsLocked; }
            set
            {
                if (_widget.IsLocked != value)
                {
                    _widget.IsLocked = value;
                    _document.IsDirty = true;
                }
            }
        }
        public bool IsDisabled
        {
            get
            {
                return _widget.IsDisabled;
            }
            set
            {
                if (_widget.IsDisabled != value)
                {
                    _widget.IsDisabled = value;
                    _document.IsDirty = true;
                }
            }
        }

        public string sTextContent
        {
            get { return _widget.Text; }
            set
            {
                if (_widget.Text != value)
                {
                    _widget.Text = value;
                    _document.IsDirty = true;
                }
            }
        }
        public string sRichTextContent
        {
            get { return _widget.RichText; }
            set
            {
                if (_widget.RichText != value)
                {
                    _widget.RichText = value;
                    _document.IsDirty = true;
                }
            }
        }
        public string InputSimpleText
        {

            set
            {
                _widget.SetRichText(Convert.ToString(value));
            }
        }
        #endregion Binding common property

        #region widget common readonly property
        public Guid RealParentGroupGID
        {
            get { return _widget.ParentGroupGuid; }
        }
        public IGroup RealParentGroup
        {
            get { return _widget.ParentGroup; }
        }
        #endregion
    }

    public class MasterModel : IPageElementDataModel
    {
        public MasterModel(IMaster master)
        {
            _master = master;
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            _document = doc.Document;
            _style = master.MasterStyle;
        }

        #region widget common readonly property
        public IRegion WdgDom
        {
            get { return _master; }
        }
        public IRegionStyle Style
        {
            get { return _style; }
        }
        public Guid StyleGID
        {
            get { return _style.ViewGuid; }
        }
        public Guid Guid
        {
            get { return _master.Guid; }
        }
        public Guid RealParentGroupGID
        {
            get { return _master.ParentGroupGuid; }
        }
        public IGroup RealParentGroup
        {
            get { return _master.ParentGroup; }
        }
        #endregion

        #region Private member
        protected IMasterStyle _style = null;
        protected IMaster _master = null;
        protected IDocument _document = null;
        #endregion private member

        #region Binding Style Property
        public bool IsVisible
        {
            get { return _style.IsVisible; }
            set
            {
                if (_style.IsVisible != value)
                {
                    _style.IsVisible = value;
                    _document.IsDirty = true;
                }
            }
        }
        public double Left
        {
            get { return _style.X; }
            set
            {
                if (_style.X != value)
                {
                    _style.X = value;
                    _document.IsDirty = true;
                }
            }
        }
        public double Top
        {
            get { return _style.Y; }
            set
            {
                if (_style.Y != value)
                {
                    _style.Y = value;
                    _document.IsDirty = true;
                }
            }
        }
        public int ZOrder
        {
            get { return _master.MasterStyle.Z; }
            set
            {
                if (_master.MasterStyle.Z != value)
                {
                    _master.MasterStyle.Z = value;
                    _document.IsDirty = true;
                }
            }
        }
        #endregion

        #region Binding Common Property
        public string DisplayName
        {
            get { return _master.Name; }
            set
            {
                if (_master.Name != value)
                {
                    _master.Name = value;
                    _document.IsDirty = true;
                }
            }
        }
        public bool IsLocked
        {
            get { return _master.IsLocked; }
            set
            {
                if (_master.IsLocked != value)
                {
                    _master.IsLocked = value;
                    _document.IsDirty = true;
                }
            }
        }
        public bool IsFixed
        {
            get { return _style.IsFixed; }
            set
            {
                if (_style.IsFixed != value)
                {
                    _style.IsFixed = value;
                    _document.IsDirty = true;
                }
            }
        }
        #endregion Binding common property

        #region public functions
        virtual public bool ChangeCurrentStyle(Guid newStyleGid)
        {
            if (newStyleGid == Guid.Empty)
            {
                _style = _master.MasterStyle;
            }
            else
            {
                _style = _master.GetMasterStyle(newStyleGid);
                if (_style == null)
                {
                    _style = _master.MasterStyle;
                }
            }
            return true;
        }
        public void SetWidgetStyleAsDefaultStyle(Guid StyleGid)
        {
            return;
        }
        public void SerializeObject(ISerializeWriter writer)
        {
            writer.AddMaster(_master);
        }
        public IRegionStyle GetSpecStyle(Guid gid)
        {
            return _master.GetMasterStyle(gid);
        }
        #endregion



        #region invalid property for master object
        public string ItemContent { get; set; }
        public string Tooltip { get; set; }
        public bool IsDisabled { get; set; }
        public string sTextContent { get; set; }
        public string sRichTextContent { get; set; }
        public string InputSimpleText { set{} }
        public StyleColor cBackgroundColor { get; set; }
        public StyleColor cBorderLineColor { get; set; }
        public LineStyle cBorderlineStyle { get; set; }
        public double iBorderLineWidth { get; set; }
        public Alignment cTextHorAligen { get; set; }
        public Alignment cTextVerAligen { get; set; }
        public string sFontFamily { get; set; }
        public double dFontSize { get; set; }
        public bool bFontBold { get; set; }
        public bool bFontUnderLine { get; set; }
        public bool bFontStringThrough { get; set; }
        public Color cFontColor { get; set; }
        public bool bFontItalic { get; set; }
        public TextMarkerStyle MarkerStyle { get; set; }
        public WidgetType WdgType { get{return WidgetType.None;} }
        public ShapeType shapeType { get { return ShapeType.None; } }
        public int CornerRadius { get; set; }
        public double ItemWidth { get; set; }
        public double ItemHeight { get; set; }
        public double RotateAngle { get; set; }
        public double TextRotate { get; set; }
        public double Opacity { get; set; }
        #endregion

    }
}
