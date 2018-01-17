using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class VLineWidgetPreViewModel : WidgetPreViewModeBase
    {
        public VLineWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            IsImgConvertType = true;
            ArrowWidth = 12;
            _renderOffset = 0;
            _renderWidth = 12;
        }

        protected int ArrowWidth;
        protected int _renderOffset;
        private int _renderWidth;

        #region Initialzie Override
        override protected void IniCreateDataModel(IRegion obj)
        {
            _model = new LineModel(obj as IWidget);
        }
        #endregion

        public ArrowStyle LineArrowStyle
        {
            get
            {
                return (_model as LineModel).LineArrowStyle;
            }
            set
            {
                if ((_model as LineModel).LineArrowStyle != value)
                {
                    (_model as LineModel).LineArrowStyle = value;
                    FirePropertyChanged("LineArrowStyle");

                    FirePropertyChanged("PathDataMain");
                    FirePropertyChanged("PathDataExtern");
                }
            }
        }

        override public double ItemHeight
        {
            get { return base.ItemHeight; }
            set
            {
                if (base.ItemHeight != Convert.ToDouble(value))
                {
                    base.ItemHeight = Convert.ToDouble(value);

                    FirePropertyChanged("PathDataMain");
                    FirePropertyChanged("PathDataExtern");
                }
            }
        }

        override public double vBorderLinethinck
        {
            get { return base.vBorderLinethinck; }
            set
            {
                if (base.vBorderLinethinck != Convert.ToDouble(value))
                {
                    base.vBorderLinethinck = Convert.ToDouble(value);

                    FirePropertyChanged("PathDataMain");
                    FirePropertyChanged("PathDataExtern");
                }
            }
        }

        public string PathDataMain
        {
            get
            { 
                string sRetrunData = "";

                if (vBorderLinethinck < 1)
                {
                    return sRetrunData;
                }

                countRenderOffset();
                ArrowWidth = 12;
                ArrowWidth = ArrowWidth + _renderOffset;
               
                if (LineArrowStyle == ArrowStyle.None)
                {
                    sRetrunData = String.Format(@"M {1},0 {1},{0}", ItemHeight, _renderWidth / 2);
                }
                else if (LineArrowStyle == ArrowStyle.ArrowArrow)
                {
                    sRetrunData = String.Format(@"M 0,{0}  {1},{5}  {2},{0} Z M {1},{0} {1},{3}  M 0,{3}  {1},{4}  {2},{3} Z", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight - _renderOffset, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.NoneArrow)
                {
                    sRetrunData = String.Format(@" M {1},0 {1},{3}  M 0,{3}  {1},{4}  {2},{3} Z", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight - _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.ArrowNone)
                {
                    sRetrunData = String.Format(@"M 0,{0}  {1},{5}  {2},{0} Z M {1},{0} {1},{4}", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.OpenOpen)
                {
                    sRetrunData = String.Format(@"M 0,{0}  {1},{5}  {2},{0}  M {1},{5} {1},{4}  M 0,{3}  {1},{4}  {2},{3} ", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight - _renderOffset, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.NoneOpen)
                {
                    sRetrunData = String.Format(@"M {1},0 {1},{4}  M 0,{3}  {1},{4}  {2},{3} ", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight - _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.OpenNone)
                {
                    sRetrunData = String.Format(@"M 0,{0}  {1},{5}  {2},{0} M {1},{5} {1},{4}", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight - _renderOffset, _renderOffset);
                }
                return sRetrunData;
            }
        }

        public string PathDataExtern
        {
            get
            {
                string sRetrunData = "";

                if (vBorderLinethinck < 1)
                {
                    return sRetrunData;
                }

                countRenderOffset();
                ArrowWidth = 12;
                ArrowWidth = ArrowWidth + _renderOffset;
                if (LineArrowStyle == ArrowStyle.ArrowArrow)
                {
                    sRetrunData = String.Format(@"M 0,{0}  {1},{5}  {2},{0} Z M 0,{3}  {1},{4}  {2},{3} Z", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight - _renderOffset, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.NoneArrow)
                {
                    sRetrunData = String.Format(@"M {1},0 {1},0 M 0,{3}  {1},{4}  {2},{3} Z", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight - _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.ArrowNone)
                {
                    sRetrunData = String.Format(@"M 0,{0}  {1},{5}  {2},{0} Z M {1},{4} {1},{4}", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.OpenOpen)
                {
                    sRetrunData = String.Format(@"M 0,{0}  {1},{5}  {2},{0} M 0,{3}  {1},{4}  {2},{3} ", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight - _renderOffset, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.NoneOpen)
                {
                    sRetrunData = String.Format(@"M {1},0 {1},0 M 0,{3}  {1},{4}  {2},{3}", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight - _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.OpenNone)
                {
                    sRetrunData = String.Format(@"M 0,{0}  {1},{5} {2},{0} M {1},{4} {1},{4}", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight - _renderOffset, _renderOffset);
                }

                return sRetrunData;
            }
        }

        private void countRenderOffset()
        {
            _renderOffset = (int)vBorderLinethinck;
            //if (vBorderLinethinck == 1)
            //    _renderOffset = 1;
            //else if (vBorderLinethinck == 2)
            //    _renderOffset = 2;
            //else if (vBorderLinethinck == 3)
            //    _renderOffset = 3;
            //else if (vBorderLinethinck == 4)
            //    _renderOffset = 4;
            //else if (vBorderLinethinck == 5)
            //    _renderOffset = 5;
            //else
            //    _renderOffset = 0;
        }

        #region Override Functions
        override protected void UpdateWidgetStyle2UI()
        {
            base.UpdateWidgetStyle2UI();
            UpdateBackgroundStyle();
            FirePropertyChanged("LineArrowStyle");

            FirePropertyChanged("PathDataMain");
            FirePropertyChanged("PathDataExtern");
        }
        #endregion
    }
}
