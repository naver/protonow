using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class TextFieldModel : WidgetModel
    {
        public TextFieldModel(IWidget widget)
            : base(widget)
        {
            _textfield = widget as ITextField;
            return;
        }

        #region private member
        private ITextField _textfield = null;
        #endregion private member

        #region public self property for binding
        public bool IsReadOnly
        {
            get
            {
                return _textfield.ReadOnly;
            }
            set
            {
                if (_textfield.ReadOnly != value)
                {
                    _textfield.ReadOnly = value;
                    _document.IsDirty = true;
                }
            }
        }
        public bool IsHideBorder
        {
            get
            {
                return _textfield.HideBorder;
            }
            set
            {
                if (_textfield.HideBorder != value)
                {
                    _textfield.HideBorder = value;
                    _document.IsDirty = true;
                }
            }
        }
        public string HintText
        {
            get
            {
                return _textfield.HintText;
            }
            set
            {
                if (_textfield.HintText != value)
                {
                    _textfield.HintText = value;
                    _document.IsDirty = true;
                }
            }
        }
        public int MaxTextLength 
        {
            get
            {
                return _textfield.MaxLength;
            }
            set
            {
                if (_textfield.MaxLength != value)
                {
                    _textfield.MaxLength = value;
                    _document.IsDirty = true;
                }
            }
        }
        public TextFieldType TextFieldType
        {
            get
            {
                return _textfield.TextFieldType;
            }
            set
            {
                if (_textfield.TextFieldType != value)
                {
                    _textfield.TextFieldType = value;
                    _document.IsDirty = true;
                }
            }
        }
        #endregion public self property for binding
    }
}
