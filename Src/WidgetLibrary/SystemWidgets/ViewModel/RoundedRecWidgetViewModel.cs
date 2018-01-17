using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public class RoundedRecWidgetViewModel : RectangleWidgetViewModel
    {
        public RoundedRecWidgetViewModel(IWidget widget)
        {   
            _model = new WidgetModel(widget);
            widgetGID = widget.Guid;
            Type = ObjectType.RoundedRectangle;

            _bSupportCornerRadius = true;
        }


        #region Binding Radio Property
        override public int CornerRadius
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
