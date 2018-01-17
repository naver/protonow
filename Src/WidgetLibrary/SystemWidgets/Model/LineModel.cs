using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class LineModel : WidgetModel
    {
        public LineModel(IWidget widget)
            : base(widget)
        {
            _line = widget as ILine;
            _Orientation = _line.Orientation;
            return;
            
        }

        #region private member
        private ILine _line=null;
        private Orientation _Orientation;
        #endregion private member

        #region public base property for binding
        public ArrowStyle LineArrowStyle
        {
            get 
            {
                return _line.WidgetStyle.ArrowStyle;
            }
            set
            {
                if (_line.WidgetStyle.ArrowStyle != value)
                {
                    _line.WidgetStyle.ArrowStyle = value;
                    _document.IsDirty = true;
                }                
            }
        }
        override public double ItemWidth
        {
            get { return base.ItemWidth; }
            set
            {
                if (Orientation.Vertical == _Orientation)
                {
                    base.ItemWidth = 10;
                }
                else
                {
                    if (base.ItemWidth != value)
                    {
                        base.ItemWidth = value;
                        _document.IsDirty = true;
                    }
                }
                
            }
        }
        override public double ItemHeight
        {
            get { return base.ItemHeight; }
            set
            {
                if (Orientation.Horizontal == _Orientation)
                {
                    base.ItemHeight = 10;
                }
                else
                {
                    if (base.ItemHeight != value)
                    {
                        base.ItemHeight = value;
                        _document.IsDirty = true;
                    }
                }

            }
        }
        #endregion private member
    }
}
