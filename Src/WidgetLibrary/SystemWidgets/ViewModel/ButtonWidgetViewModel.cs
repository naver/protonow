using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public class ButtonWidgetViewModel : WidgetViewModBase
    {
        public ButtonWidgetViewModel(IWidget widget)
        {
            _model = new ButtonModel(widget);
           _bSupportBorder = false;
           _bSupportBackground = false;
           _bSupportText = true;
           _bSupportTextVerAlign = false;
           _bSupportTextHorAlign = true;
           _bSupportGradientBackground = false;
           _bSupportGradientBorderline = false;
           _bSupportRotate=false;
           _bSupportTextRotate=false;

            widgetGID = widget.Guid;
            Type = ObjectType.Button;
        }

        #region Binding Button Property
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
