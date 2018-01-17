using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class TriangleWidgetPreViewModel : WidgetRichPreViewModeBase
    {
        public TriangleWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            //Infra Structure
            //_model = new WidgetModel(widget);
            IsImgConvertType = true;    
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
