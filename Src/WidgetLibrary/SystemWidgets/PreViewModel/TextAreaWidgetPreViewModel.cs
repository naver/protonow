using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class TextAreaWidgetPreViewModel : WidgetPreViewModeBase
    {
        public TextAreaWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            //Infra Structure
           // _model = new WidgetModel(widget);
            IsImgConvertType = false;           
        }

        #region Initialzie Override
        override protected void IniCreateDataModel(IRegion obj)
        {
            _model = new TextAreaModel(obj as IWidget);
        }
        #endregion

        #region Binding Radio Property
        public bool IsHideBorder
        {
            get
            {
                return (_model as TextAreaModel).IsHideBorder;
            }
            set
            {
                if ((_model as TextAreaModel).IsHideBorder != value)
                {
                    (_model as TextAreaModel).IsHideBorder = value;
                    FirePropertyChanged("IsHideBorder");
                }
            }
        }
        public bool IsDisabled
        {
            get
            {
                return (_model as TextAreaModel).IsDisabled;
            }
            set
            {
                if ((_model as TextAreaModel).IsDisabled != value)
                {
                    (_model as TextAreaModel).IsDisabled = value;
                    FirePropertyChanged("IsDisabled");
                }
            }
        }
        public string HintText
        {
            get
            {
                return (_model as TextAreaModel).HintText;
            }
            set
            {
                if ((_model as TextAreaModel).HintText != value)
                {
                    (_model as TextAreaModel).HintText = value;
                    FirePropertyChanged("HintText");
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
            UpdateBackgroundStyle();
        }
        #endregion 
    }
}
