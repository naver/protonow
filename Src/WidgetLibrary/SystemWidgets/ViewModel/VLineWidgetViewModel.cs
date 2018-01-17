using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public class VLineWidgetViewModel : WidgetLineViewModeBase
    {

        private int _renderWidth;
        private int _renderOffset;
        public VLineWidgetViewModel(IWidget widget):base(widget)
        {   
            _bSupportBorder = true;
            _bSupportBackground = false;
            _bSupportText = false;
            _bSupportTextVerAlign = false;
            _bSupportTextHorAlign = false;
            widgetGID = widget.Guid;
            Type = ObjectType.VerticalLine;
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = true;
            _bSupportTextRotate = false;
            _bSupportArrowStyle = true;

            _renderWidth = 12;

            _renderOffset = 0;
        }


        override public ArrowStyle LineArrowStyle
        {
            get
            {
                return base.LineArrowStyle;
            }
            set
            {
                if (base.LineArrowStyle != (ArrowStyle)value)
                {
                    base.LineArrowStyle = (ArrowStyle)value;
                    FirePropertyChanged("PathDataMain");
                    FirePropertyChanged("PathDataExtern");
                }
            }
        }

        public override double Raw_ItemWidth
        {
            get
            {
                return vBorderLinethinck;
            }
            set
            {
                base.Raw_ItemWidth = value;
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

        public override double Left
        {
            get
            {

                return base.Left;
            }
            set
            {

                base.Left = value - (ItemWidth - vBorderLinethinck) / 2;
                FirePropertyChanged("Left");
            }
        }

        public override double Raw_Left
        {
            get
            {
                return base.Left + (ItemWidth - vBorderLinethinck) / 2;
            }
            set
            {
                base.Left = value;
                FirePropertyChanged("Left");
            }
        }

        override public double vBorderLinethinck
        {
            get { return base.vBorderLinethinck; }
            set
            {
                if (base.vBorderLinethinck != Convert.ToDouble(value))
                {
                    base.Left += (Convert.ToDouble(value) - base.vBorderLinethinck) / 2;

                    base.vBorderLinethinck = Convert.ToDouble(value);
                    
                    FirePropertyChanged("Left");
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
                    sRetrunData = String.Format(@"M {1},0 {1},{0}", ItemHeight , _renderWidth / 2);
                }
                else if (LineArrowStyle == ArrowStyle.ArrowArrow)
                {
                    sRetrunData = String.Format(@"M 0,{0}  {1},{5}  {2},{0} Z M {1},{0} {1},{3}  M 0,{3}  {1},{4}  {2},{3} Z", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight - _renderOffset, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.NoneArrow)
                {
                    sRetrunData = String.Format(@" M {1},0 {1},{3}  M 0,{3}  {1},{4}  {2},{3} Z", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight-_renderOffset);
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
                    sRetrunData = String.Format(@"M {1},0 {1},0 M 0,{3}  {1},{4}  {2},{3}", ArrowWidth, _renderWidth / 2, _renderWidth, ItemHeight - ArrowWidth, ItemHeight-_renderOffset);
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
        }
      
    }
}
