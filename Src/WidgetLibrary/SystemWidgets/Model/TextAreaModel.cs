using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class TextAreaModel : WidgetModel                    
    {
        public TextAreaModel(IWidget widget)
            : base(widget)
        {
            _textarea = widget as ITextArea;
            return;            
        }

        #region private member
        private ITextArea _textarea = null;
        #endregion private member

        #region public self property for binding     
        public bool IsReadOnly
        {
            get
            {                
                return _textarea.ReadOnly;
            }
            set
            {
                if (_textarea.ReadOnly != value)
                {
                    _textarea.ReadOnly = value;
                    _document.IsDirty = true;
                }  
            }
        }
        public bool IsHideBorder
        {
            get
            {                
                return _textarea.HideBorder;
            }
            set
            {
                if (_textarea.HideBorder != value)
                {
                    _textarea.HideBorder = value;
                    _document.IsDirty = true;
                }
            }
        }
        public string HintText
        {
            get
            {
                return _textarea.HintText;
            }
            set
            {
                if (_textarea.HintText != value)
                {
                    _textarea.HintText = value;
                    _document.IsDirty = true;
                }
            }
        }        
        #endregion public self property for binding       

    }
}
