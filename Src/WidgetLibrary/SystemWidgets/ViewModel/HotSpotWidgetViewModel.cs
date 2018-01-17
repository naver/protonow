using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public class HotSpotWidgetViewModel : WidgetRotateViewModBase
    {
        public HotSpotWidgetViewModel(IWidget widget)
        {
            _model = new WidgetModel(widget);
           _bSupportBorder = false;
           _bSupportBackground = false;
           _bSupportText = false;
           _bSupportTextVerAlign = false;
           _bSupportTextHorAlign = false;

            widgetGID = widget.Guid;
            Type = ObjectType.HotSpot;
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = true;
            _bSupportTextRotate = false;
        }

        #region Override UpdateWidgetStyle2UI Functions
        override protected void UpdateWidgetStyle2UI()
        {
            base.UpdateWidgetStyle2UI();
            //UpdateTextStyle();
            //UpdateFontStyle();
            //UpdateBackgroundStyle();
        }
        #endregion 

    }
}
