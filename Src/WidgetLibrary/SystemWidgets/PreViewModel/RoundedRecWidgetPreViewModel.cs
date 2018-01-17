using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class RoundedRecWidgetPreViewModel : RectangleWidgetPreViewModel
    {
        public RoundedRecWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            IsImgConvertType = true;   
        }


        #region Binding Radio Property
        public int CornerRadius
        {
            get
            {
                return _model.CornerRadius;
            }
            set
            {
                if (_model.CornerRadius != value)
                {
                    _model.CornerRadius = value;
                    FirePropertyChanged("CornerRadius");
                }
            }
        }
        #endregion

        #region Override UpdateWidgetStyle2UI Functions
        override protected void UpdateWidgetStyle2UI()
        {
            base.UpdateWidgetStyle2UI();
            UpdateTextStyle();
            //UpdateFontStyle();
            UpdateBackgroundStyle();
            FirePropertyChanged("CornerRadius");
        }
        #endregion 
    }
}
