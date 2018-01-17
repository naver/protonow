using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public class TextAreaWidgetViewModel : WidgetViewModBase
    {
        public TextAreaWidgetViewModel(IWidget widget)
        {
            _model = new TextAreaModel(widget);
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportBorder = false;
            _bSupportBackground = true;
            _bSupportText = true;
            _bSupportTextVerAlign = false;
            _bSupportTextHorAlign = true;
            widgetGID = widget.Guid;
            _bSupportRotate = false;
            _bSupportTextRotate = false;
            Type = ObjectType.TextArea;
        }

        
        #region temporary binding property
        //public string RadioID
        //{
        //    get
        //    {
        //        return widgetGID.ToString();
        //    }
        //}
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
        public bool IsReadOnly
        {
            get
            {
                return (_model as TextAreaModel).IsReadOnly;
            }
            set
            {
                if ((_model as TextAreaModel).IsReadOnly != value)
                {
                    (_model as TextAreaModel).IsReadOnly = value;
                    FirePropertyChanged("IsReadOnly");
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
