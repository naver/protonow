using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;
using System.Windows.Media;

namespace Naver.Compass.WidgetLibrary
{
    public class HLineWidgetViewModel : WidgetLineViewModeBase
    {

        private int _renderHeight;

        private int _renderOffset;

        public HLineWidgetViewModel(IWidget widget):base(widget)
        {
            _bSupportBorder = true;
            _bSupportBackground = false;
            _bSupportText = false;
            _bSupportTextVerAlign = false;
            _bSupportTextHorAlign = false;
            _bSupportArrowStyle = true;
            widgetGID = widget.Guid;
            Type = ObjectType.HorizontalLine;
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = true;
            _bSupportTextRotate = false;

            _renderHeight = 12;

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
                base.LineArrowStyle = (ArrowStyle)value;
                FirePropertyChanged("PathDataMain");
                FirePropertyChanged("PathDataExtern");
            }
        }

        override public double ItemWidth
        {
            get { return base.ItemWidth; }
            set
            {
                base.ItemWidth = Convert.ToDouble(value);

                FirePropertyChanged("PathDataMain");
                FirePropertyChanged("PathDataExtern");
            }
        }

        public override double Raw_ItemHeight
        {
            get
            {
                return vBorderLinethinck;
            }
            set
            {
                base.Raw_ItemHeight = value;
            }
        }
        public override double Top
        {
            get
            {

                return base.Top;
            }
            set
            {
                base.Top = value - (ItemHeight - vBorderLinethinck) / 2;
                FirePropertyChanged("Top");
            }
        }

        public override double Raw_Top
        {
            get
            {
                return base.Top + (ItemHeight - vBorderLinethinck) / 2;
            }
            set
            {
                base.Top = value;
                FirePropertyChanged("Top");
            }
        }

       
        override public double vBorderLinethinck
        {
            get { return base.vBorderLinethinck; }
            set
            {
                if (base.vBorderLinethinck != Convert.ToDouble(value))
                {
                    base.Top += (Convert.ToDouble(value) - base.vBorderLinethinck) / 2;

                    base.vBorderLinethinck = Convert.ToDouble(value);

                    FirePropertyChanged("Top");
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
                    return sRetrunData ;
                }

                countRenderOffset();
                ArrowWidth = 12;
                ArrowWidth = ArrowWidth + _renderOffset;

                if (LineArrowStyle == ArrowStyle.None)
                {
                    sRetrunData = String.Format(@"M 0,{1} {0},{1}", ItemWidth, _renderHeight/2);
                }
                else if (LineArrowStyle == ArrowStyle.ArrowArrow)
                {
                    sRetrunData = String.Format(@"M {0},0  {5},{3}  {0},{4} Z M {0},{3} {1},{3}  M {1},0  {2},{3}  {1},{4} Z", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth - _renderOffset, _renderHeight / 2, _renderHeight, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.NoneArrow)
                {
                    sRetrunData = String.Format(@"M 0,{3} {1},{3} M {1},0  {2},{3}  {1},{4} Z", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth - _renderOffset, _renderHeight / 2, _renderHeight);
                }
                else if (LineArrowStyle == ArrowStyle.ArrowNone)
                {
                    sRetrunData = String.Format(@"M {0},0  {5},{3}  {0},{4} Z M {0},{3} {2},{3}", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth , _renderHeight / 2, _renderHeight, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.OpenOpen)
                {
                    sRetrunData = String.Format(@"M {0},0  {5},{3}  {0},{4} M {5},{3} {2},{3} M {1},0  {2},{3}  {1},{4} ", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth - _renderOffset, _renderHeight / 2, _renderHeight, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.NoneOpen)
                {
                    sRetrunData = String.Format(@"M 0,{3} {2},{3} M {1},0  {2},{3}  {1},{4}", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth - _renderOffset, _renderHeight / 2, _renderHeight);
                }
                else if (LineArrowStyle == ArrowStyle.OpenNone)
                {
                    sRetrunData = String.Format(@"M {0},0  {5},{3}  {0},{4} M {5},{3} {2},{3}", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth, _renderHeight / 2, _renderHeight, _renderOffset);
                }
                return sRetrunData;
            }
        }

        public string PathDataExtern
        {
            get
            {
                string sRetrunData="";

                if (vBorderLinethinck < 1)
                {
                    return sRetrunData;
                }

                countRenderOffset();
                ArrowWidth = 12;
                ArrowWidth = ArrowWidth + _renderOffset;
               
                if (LineArrowStyle == ArrowStyle.ArrowArrow)
                {
                    sRetrunData = String.Format(@"M {0},0  {5},{3}  {0},{4} Z  M {1},0  {2},{3}  {1},{4} Z", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth - _renderOffset, _renderHeight / 2, _renderHeight, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.NoneArrow)
                {
                    sRetrunData = String.Format(@"M 0,{3} 0,{3} M {1},0  {2},{3}  {1},{4} Z", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth - _renderOffset, _renderHeight / 2, _renderHeight);
                }
                else if (LineArrowStyle == ArrowStyle.ArrowNone)
                {
                    sRetrunData = String.Format(@"M {0},0  {5},{3}  {0},{4} Z M {2},{3} {2},{3}", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth, _renderHeight / 2, _renderHeight, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.OpenOpen)
                {
                    sRetrunData = String.Format(@"M {0},0  {5},{3}  {0},{4} M {1},0  {2},{3}  {1},{4} ", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth - _renderOffset, _renderHeight / 2, _renderHeight, _renderOffset);
                }
                else if (LineArrowStyle == ArrowStyle.NoneOpen)
                {
                    sRetrunData = String.Format(@"M 0,{3} 0,{3} M {1},0  {2},{3}  {1},{4}", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth - _renderOffset, _renderHeight / 2, _renderHeight);
                }
                else if (LineArrowStyle == ArrowStyle.OpenNone)
                {
                    sRetrunData = String.Format(@"M {0},0  {5},{3}  {0},{4} M {2},{3} {2},{3}", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth, _renderHeight / 2, _renderHeight, _renderOffset);
                }

                if (vBorderLinethinck < 1)
                {
                    sRetrunData = "";
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
