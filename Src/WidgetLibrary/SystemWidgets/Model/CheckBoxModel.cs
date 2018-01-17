using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class CheckBoxModel : WidgetModel                    
    {
        public CheckBoxModel(IWidget widget)
            : base(widget)
        {
            _chekcbox = widget as ICheckbox;
            return;            
        }

        #region Private member
        private ICheckbox _chekcbox = null;
        #endregion private member

        #region Public self property for binding
        public bool IsShowSelect
        {
            get
            {
                return _chekcbox.IsSelected;
            }
            set
            {
                if (_chekcbox.IsSelected != value)
                {
                    _chekcbox.IsSelected = value;
                    _document.IsDirty = true;
                }  
            }
        }
        public bool IsBtnAlignLeft
        {
            get
            {
                if(_chekcbox.AlignButton==AlignButton.Left)
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

                if (_chekcbox.AlignButton != realvalue)
                {
                    _chekcbox.AlignButton = realvalue;
                    _document.IsDirty = true;
                }
            }
        }        
        #endregion public base property for binding

        #region Override Binding Propery
        //Checkbox Height is auto-size, so dirty setting will be removed
        override public double ItemHeight
        {
            get { return base.ItemHeight; }
            set
            {
                if (base.ItemHeight!= value)
                {
                    (base.Style as IWidgetStyle).Height= value;
                }
            }
        }
        #endregion

    }
}
