using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialWidgetStyle
    {
        [NonSerialized]
        private IWidgetStyle _style;

        #region public interface functions
        public SerialWidgetStyle(IWidgetStyle style,bool bIsPlaced)
        {
            _style = style;
            IsPlaced = bIsPlaced;
            refresh();
        }
        public void refresh(IWidgetStyle style,bool bIsPlaced)
        {
            _style = style;
            IsPlaced = bIsPlaced;
            refresh();
        }
        #endregion

        #region private functions
        private void refresh()
        {
            IsVisible = _style.IsVisible;
            X=_style.X;
            Y = _style.Y;
            Height = _style.Height;
            Width = _style.Width;
            Z = _style.Z;
            IsFixed = _style.IsFixed;
            WidgetRotate = _style.WidgetRotate;
            TextRotate = _style.TextRotate;
            FontFamily = _style.FontFamily;
            FontSize = _style.FontSize;
            Bold = _style.Bold;
            Italic = _style.Italic;
            Underline = _style.Underline;
            Baseline = _style.Baseline;
            Overline = _style.Overline;
            Strikethrough = _style.Strikethrough;
            FontColor = _style.FontColor;
            BulletedList = _style.BulletedList;
            LineColor = _style.LineColor;
            LineWidth = _style.LineWidth;
            LineStyle = _style.LineStyle;
            ArrowStyle = _style.ArrowStyle;
            CornerRadius = _style.CornerRadius;
            FillColor = _style.FillColor;
            Opacity = _style.Opacity;
            HorzAlign = _style.HorzAlign;
            VertAlign = _style.VertAlign;

            //chcekbox and radiobutton is auto-height control, so the height property needn't be compared, excluding from md5...
            if(_style.OwnerWidget is IRadioButton ||
                _style.OwnerWidget is ICheckbox)
            {
                Height = 0;
            }
        }
        #endregion

        #region Serialize Data
        public bool IsVisible;
        public double X;
        public double Y;
        public double Height;
        public double Width;
        public int Z;
        public bool IsFixed;
        public double WidgetRotate;
        public double TextRotate;
        public string FontFamily;
        public double FontSize;
        public bool Bold;
        public bool Italic;
        public bool Underline;
        public bool Baseline;
        public bool Overline;
        public bool Strikethrough;
        public StyleColor FontColor;
        public TextMarkerStyle BulletedList;
        public StyleColor LineColor;
        public double LineWidth;
        public LineStyle LineStyle;
        public ArrowStyle ArrowStyle;
        public int CornerRadius;
        public StyleColor FillColor;
        public int Opacity;
        public Alignment HorzAlign;
        public Alignment VertAlign;
        public bool IsPlaced;
        #endregion
    }

    internal class SerialWidgetStyle2
    {
        [NonSerialized]
        private IWidgetStyle _style;

        public SerialWidgetStyle2(IWidgetStyle style)
        {
            _style = style;
        }


        public bool IsVisible
        {
            get { return _style.IsVisible; }
        }
        public double X
        {
            get { return _style.X; }
        }
        public double Y
        {
            get { return _style.Y; }
        }




        public double Height
        {
            get { return _style.Height; }
        }
        public double Width
        {
            get { return _style.Width; }
        }
        public int Z
        {
            get { return _style.Z; }
        }

        public bool IsFixed
        {
            get { return _style.IsFixed; }
        }
        public double WidgetRotate
        {
            get { return _style.WidgetRotate; }
        }
        public double TextRotate
        {
            get { return _style.TextRotate; }
        }
        public string FontFamily
        {
            get { return _style.FontFamily; }
        }
        public double FontSize
        {
            get { return _style.FontSize; }
        }
        public bool Bold
        {
            get { return _style.Bold; }
        }
        public bool Italic
        {
            get { return _style.Italic; }
        }
        public bool Underline
        {
            get { return _style.Underline; }
        }
        public bool Baseline
        {
            get { return _style.Baseline; }
        }
        public bool Overline
        {
            get { return _style.Overline; }
        }
        public bool Strikethrough
        {
            get { return _style.Strikethrough; }
        }
        public StyleColor FontColor
        {
            get { return _style.FontColor; }
        }
        public TextMarkerStyle BulletedList
        {
            get { return _style.BulletedList; }
        }
        public StyleColor LineColor
        {
            get { return _style.LineColor; }
        }
        public double LineWidth
        {
            get { return _style.LineWidth; }
        }
        public LineStyle LineStyle
        {
            get { return _style.LineStyle; }
        }
        public ArrowStyle ArrowStyle
        {
            get { return _style.ArrowStyle; }
        }
        public int CornerRadius
        {
            get { return _style.CornerRadius; }
        }
        public StyleColor FillColor
        {
            get { return _style.FillColor; }
        }
        public int Opacity
        {
            get { return _style.Opacity; }
        }
        public Alignment HorzAlign
        {
            get { return _style.HorzAlign; }
        }
        public Alignment VertAlign
        {
            get { return _style.VertAlign; }
        }

    }
}
