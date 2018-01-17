using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class ButtonWidgetPreViewModel : WidgetPreViewModeBase
    {
        public ButtonWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            //Infra Structure
           // _model = new WidgetModel(widget);
            IsImgConvertType = false;           
        }

        #region Initialzie Override
        override protected void IniCreateDataModel(IRegion obj)
        {
            _model = new ButtonModel(obj as IWidget);
        }
        #endregion

        #region Binding Propery
        public bool IsDisabled
        {
            get
            {
                return (_model as ButtonModel).IsDisabled;
            }
            set
            {
                if ((_model as ButtonModel).IsDisabled != value)
                {
                    (_model as ButtonModel).IsDisabled = value;
                    FirePropertyChanged("IsDisabled");
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
