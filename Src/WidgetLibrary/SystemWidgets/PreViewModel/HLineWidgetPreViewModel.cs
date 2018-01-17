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
    public class HLineWidgetPreViewModel : WidgetPreViewModeBase
    {
        public HLineWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            //Infra Structure
            IsImgConvertType = true;
            ArrowWidth = 12;
            _renderOffset = 0;
            _renderHeight = 12;
        }

        protected int ArrowWidth;
        protected int _renderOffset;
        private int _renderHeight;

        #region Initialzie Override
        override protected void IniCreateDataModel(IRegion obj)
        {
            _model = new LineModel(obj as IWidget);
        }
        #endregion

        #region Binding line Property
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
                    sRetrunData = String.Format(@"M 0,{1} {0},{1}", ItemWidth, _renderHeight / 2);
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
                    sRetrunData = String.Format(@"M {0},0  {5},{3}  {0},{4} Z M {0},{3} {2},{3}", ArrowWidth, ItemWidth - ArrowWidth, ItemWidth, _renderHeight / 2, _renderHeight, _renderOffset);
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

                return sRetrunData;
            }
        }

        private void countRenderOffset()
        {
            _renderOffset = (int)vBorderLinethinck;
        }
        #endregion Binding line Property

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
