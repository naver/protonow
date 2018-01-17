using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.InfoStructure
{
    public class LineModel : WidgetModel
    {
        public LineModel(IWidget widget):base(widget)
        {
            _line = widget as ILine;
            _Orientation = _line.Orientation;
            return;            
        }

        #region private member
        private ILine _line=null;
        private Orientation _Orientation;
        #endregion private member

        #region Binding Style Property
        public ArrowStyle LineArrowStyle
        {
            get 
            {
                return _style.ArrowStyle;
            }
            set
            {
                if (_style.ArrowStyle != value)
                {
                    _style.ArrowStyle = value;
                    _document.IsDirty = true;
                }                
            }
        }
        #endregion Binding Style Property

        #region Binding Overrid Style Property
        override public double ItemWidth
        {
            get { return base.ItemWidth; }
            set
            {
                if (Orientation.Vertical == _Orientation)
                {
                    base.ItemWidth = 20;
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
                    base.ItemHeight = 20;
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
        #endregion Binding Overrid Style Property

    }
}
