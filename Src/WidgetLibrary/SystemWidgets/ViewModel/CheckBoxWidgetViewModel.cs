using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public class CheckBoxWidgetViewModel : WidgetViewModBase
    {
        public CheckBoxWidgetViewModel(IWidget widget)
        {
            _model = new CheckBoxModel(widget);
            _bSupportBorder = false;
            _bSupportBackground = false;
            _bSupportText = true;
            _bSupportTextVerAlign = true;
            _bSupportTextHorAlign = true;
            widgetGID = widget.Guid;
            Type = ObjectType.Checkbox;
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = false;
            _bSupportTextRotate = false;
        }

        #region Binding line Property
        public bool IsShowSelect
        {
            get
            {
                return (_model as CheckBoxModel).IsShowSelect;
            }
            set
            {
                if ((_model as CheckBoxModel).IsShowSelect != value)
                {
                    (_model as CheckBoxModel).IsShowSelect = value;
                    FirePropertyChanged("IsShowSelect");
                }
            }
        }
        public bool IsBtnAlignLeft
        {
            get
            {
                return (_model as CheckBoxModel).IsBtnAlignLeft;
            }
            set
            {
                if ((_model as CheckBoxModel).IsBtnAlignLeft != value)
                {
                    (_model as CheckBoxModel).IsBtnAlignLeft = value;
                    FirePropertyChanged("IsBtnAlignLeft");
                }
            }
        }
        public bool IsDisabled
        {
            get
            {
                return (_model as CheckBoxModel).IsDisabled;
            }
            set
            {
                if ((_model as CheckBoxModel).IsDisabled != value)
                {
                    (_model as CheckBoxModel).IsDisabled = value;
                    FirePropertyChanged("IsDisabled");
                }
            }
        }
        #endregion Binding line Property

        #region Override Functions
        override public double ItemWidth
        {
            get { return _model.ItemWidth; }
            set
            {
                double newValue = 16;
                if (value > 16)
                {
                    newValue = value;
                }

                if (_model.ItemWidth != newValue)
                {
                    _model.ItemWidth = newValue;
                    FirePropertyChanged("ItemWidth");
                }
            }
        }
        #endregion 

        #region Override UpdateWidgetStyle2UI Functions
        override protected void UpdateWidgetStyle2UI()
        {
            base.UpdateWidgetStyle2UI();
            UpdateTextStyle();
            UpdateFontStyle();
            //UpdateBackgroundStyle();
        }    
        #endregion 
    }
}
