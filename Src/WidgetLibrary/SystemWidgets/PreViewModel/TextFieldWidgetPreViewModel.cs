using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class TextFieldWidgetPreViewModel : WidgetPreViewModeBase
    {
        public TextFieldWidgetPreViewModel(IWidget widget)
            : base(widget)
        {
            //Infra Structure
           // _model = new WidgetModel(widget);
            IsImgConvertType = false;           
        }

        #region Initialzie Override
        override protected void IniCreateDataModel(IRegion obj)
        {
            _model = new TextFieldModel(obj as IWidget);
        }
        #endregion

        #region temporary binding property
        public string Password
        {
            get
            {
                if (string.IsNullOrEmpty(vTextContent))
                {
                    return null;
                }
                return new String('●', vTextContent.Length);
            }
        }
        public bool IsPasswordEmpty
        {
            get
            {
                return string.IsNullOrEmpty(vTextContent);
            }
        }
        #endregion

        #region  override  binding proprety
        override public string vTextContent
        {
            get
            {
                return base.vTextContent;
            }
            set
            {
                base.vTextContent = value;
                FirePropertyChanged("Password");
                FirePropertyChanged("IsPasswordEmpty");
            }
        }
        #endregion

        #region Binding Text Property
        public bool IsHideBorder
        {
            get
            {
                return (_model as TextFieldModel).IsHideBorder;
            }
            set
            {
                if ((_model as TextFieldModel).IsHideBorder != value)
                {
                    (_model as TextFieldModel).IsHideBorder = value;
                    FirePropertyChanged("IsHideBorder");
                }
            }
        }
        public bool IsDisabled
        {
            get
            {
                return (_model as TextFieldModel).IsDisabled;
            }
            set
            {
                if ((_model as TextFieldModel).IsDisabled != value)
                {
                    (_model as TextFieldModel).IsDisabled = value;
                    FirePropertyChanged("IsDisabled");
                }
            }
        }
        public string HintText
        {
            get
            {
                return (_model as TextFieldModel).HintText;
            }
            set
            {
                if ((_model as TextFieldModel).HintText != value)
                {
                    (_model as TextFieldModel).HintText = value;
                    FirePropertyChanged("HintText");
                }
            }
        }
        public int MaxTextLength
        {
            get{ return 0;            }
        }
        public TextFieldType TextFieldType
        {
            get
            {
                return (_model as TextFieldModel).TextFieldType;
            }
            set
            {
                if ((_model as TextFieldModel).TextFieldType != value)
                {
                    (_model as TextFieldModel).TextFieldType = value;
                    FirePropertyChanged("TextFieldType");
                    FirePropertyChanged("Password");
                    FirePropertyChanged("IsPasswordEmpty");
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
