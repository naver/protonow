using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public class TriangleWidgetViewModel : WidgetMultiTextViewModBase
    {
        public TriangleWidgetViewModel(IWidget widget)
        {   
            _model = new WidgetModel(widget);
            _bSupportBorder = true;
            _bSupportBackground = true;
            _bSupportText = true;
            _bSupportTextVerAlign = true;
            _bSupportTextHorAlign = true;
            widgetGID = widget.Guid;
            Type = ObjectType.Triangle;
            _bSupportGradientBackground = true;
            _bSupportGradientBorderline = false;
            _bSupportRotate = true;
            _bSupportTextRotate = true;
        }

        #region Override UpdateWidgetStyle2UI Functions
        override protected void UpdateWidgetStyle2UI()
        {
            base.UpdateWidgetStyle2UI();
            UpdateTextStyle();
            //UpdateFontStyle();
            UpdateBackgroundStyle();
        }
        #endregion 
    }
}
