using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class RadioButtonWidgetPreViewModel : WidgetPreViewModeBase
    {
        public RadioButtonWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            //Infra Structure
           // _model = new WidgetModel(widget);
            IsImgConvertType = false;           
        }

        #region Initialzie Override
        override protected void IniCreateDataModel(IRegion obj)
        {
            _model = new RadioButtonModel(obj as IWidget);
        }
        #endregion

        #region temporary binding property
        public string RadioID
        {
            get
            {
                return WidgetID.ToString();
            }
        }
        #endregion

        #region Binding Radio Property
        public string RadioGroup
        {
            get
            {
                return null;
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
