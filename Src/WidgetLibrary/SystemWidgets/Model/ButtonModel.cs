using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class ButtonModel : WidgetModel                    
    {
        public ButtonModel(IWidget widget)
            : base(widget)
        {
            _button = widget as IButton;
            return;            
        }

        #region private member
        private IButton _button = null;
        #endregion private member


        #region public base property for binding
        //public bool IsDisabled
        //{
        //    get
        //    {
        //        return _button.IsDisabled;
        //    }
        //    set
        //    {
        //        if (_button.IsDisabled != value)
        //        {
        //            _button.IsDisabled = value;
        //            _document.IsDirty = true;
        //        }
        //    }
        //}        
        #endregion public base property for binding



    }
}
