using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class SVGModel : WidgetModel
    {
        public SVGModel(IWidget widget)
            : base(widget)
        {
            _svg = widget as ISvg;
            return;
        }

        #region private member
        private ISvg _svg = null;

        #endregion private member

        #region public base property for binding
        virtual public Stream SVGStream
        {
            get
            {
                return _svg.XmlStream;
            }
            set
            {
                _svg.XmlStream = value;
                _document.IsDirty = true;
            }
        }

        public Stream SVGUpdateStream
        {
            get
            {
                return _svg.XmlStream;
            }
            set
            {
                _svg.XmlStream = value;
            }
        }
        #endregion private member
    }
}
