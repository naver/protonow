using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class CheckBoxWidgetPreViewModel : WidgetPreViewModeBase
    {
        public CheckBoxWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            //Infra Structure
           // _model = new WidgetModel(widget);
            IsImgConvertType = false;           
        }

        #region Initialzie Override
        override protected void IniCreateDataModel(IRegion obj)
        {
            _model = new CheckBoxModel(obj as IWidget);
        }
        #endregion

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
