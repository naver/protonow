using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public class RadioButtonWidgetViewModel : WidgetViewModBase
    {
        public RadioButtonWidgetViewModel(IWidget widget)
        {
            _model = new RadioButtonModel(widget);
            _bSupportBorder = false;
            _bSupportBackground = false;
            _bSupportText = true;
            _bSupportTextVerAlign = true;
            _bSupportTextHorAlign = true;
            widgetGID = widget.Guid;
            Type = ObjectType.RadioButton;
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = false;
            _bSupportTextRotate = false;
        }

        
        #region temporary binding property
        public string RadioID
        {
            get
            {
                return widgetGID.ToString();
            }
        }
        #endregion

        #region Binding Radio Property
        public string RadioGroup
        {
            get
            {
                return (_model as RadioButtonModel).RadioGroup;
            }
            set
            {
                if ((_model as RadioButtonModel).RadioGroup != value)
                {
                    (_model as RadioButtonModel).RadioGroup = value;
                    FirePropertyChanged("RadioGroup");
                    IsShowSelect = !IsShowSelect;
                    IsShowSelect = !IsShowSelect;
                }
            }
        }
        public bool IsShowSelect
        {
            get
            {
                return (_model as RadioButtonModel).IsShowSelect;
            }
            set
            {
                if ((_model as RadioButtonModel).IsShowSelect != value)
                {
                    (_model as RadioButtonModel).IsShowSelect = value;
                    FirePropertyChanged("IsShowSelect");
                }
            }
        }
        public bool IsBtnAlignLeft
        {
            get
            {
                return (_model as RadioButtonModel).IsBtnAlignLeft;
            }
            set
            {
                if ((_model as RadioButtonModel).IsBtnAlignLeft != value)
                {
                    (_model as RadioButtonModel).IsBtnAlignLeft = value;
                    FirePropertyChanged("IsBtnAlignLeft");
                }
            }
        }
        public bool IsDisabled
        {
            get
            {
                return (_model as RadioButtonModel).IsDisabled;
            }
            set
            {
                if ((_model as RadioButtonModel).IsDisabled != value)
                {
                    (_model as RadioButtonModel).IsDisabled = value;
                    FirePropertyChanged("IsDisabled");
                }
            }
        }
        #endregion Binding line Property

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
