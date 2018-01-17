using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class RadioButtonModel : WidgetModel                    
    {
        public RadioButtonModel(IWidget widget)
            : base(widget)
        {
            _radiobutton = widget as IRadioButton;
            return;            
        }

        #region Private member
        private IRadioButton _radiobutton = null;
        #endregion private member

        #region Public self property for binding
        public string RadioGroup
        {
            get
            {
                return _radiobutton.RadioGroup;
            }
            set
            {
                if (_radiobutton.RadioGroup != value)
                {
                    _radiobutton.RadioGroup = value;
                    _document.IsDirty = true;
                }
            }
        }
        public bool IsShowSelect
        {
            get
            {
                return _radiobutton.IsSelected;
            }
            set
            {
                if (_radiobutton.IsSelected != value)
                {
                    _radiobutton.IsSelected = value;
                    _document.IsDirty = true;
                }  
            }
        }
        public bool IsBtnAlignLeft
        {
            get
            {
                if (_radiobutton.AlignButton == AlignButton.Left)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                AlignButton realvalue;
                if (value==true)
                {
                    realvalue = AlignButton.Left;
                }
                else
                {
                    realvalue = AlignButton.Right;
                }

                if (_radiobutton.AlignButton != realvalue)
                {
                    _radiobutton.AlignButton = realvalue;
                    _document.IsDirty = true;
                }
            }
        }        
        #endregion public self property for binding

        #region Override Binding Propery
        //Radio button Height is auto-size, so dirty setting will be removed
        override public double ItemHeight
        {
            get { return base.ItemHeight; }
            set
            {
                if (base.ItemHeight!= value)
                {
                    (base.Style as IWidgetStyle).Height= value;
                    //MASTER TOD:
                }
            }
        }
        #endregion
    }
}
