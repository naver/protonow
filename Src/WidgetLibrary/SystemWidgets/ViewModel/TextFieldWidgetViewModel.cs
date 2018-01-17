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
    public class TextFieldWidgetViewModel : WidgetViewModBase
    {
        public TextFieldWidgetViewModel(IWidget widget)
        {
            _model = new TextFieldModel(widget);
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
            Type = ObjectType.TextField;
        }

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
        public bool IsReadOnly
        {
            get
            {
                return (_model as TextFieldModel).IsReadOnly;
            }
            set
            {
                if ((_model as TextFieldModel).IsReadOnly != value)
                {
                    (_model as TextFieldModel).IsReadOnly = value;
                    FirePropertyChanged("IsReadOnly");
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
            get
            {
                return (_model as TextFieldModel).MaxTextLength;
            }
            set
            {
                if ((_model as TextFieldModel).MaxTextLength != value)
                {
                    (_model as TextFieldModel).MaxTextLength = value;
                    if (string.IsNullOrEmpty(vTextContent) == false)
                    {
                        int nLength = vTextContent.Length;
                        if (nLength > MaxTextLength)
                        {
                            vTextContent = vTextContent.Substring(0, MaxTextLength);
                        }
                    }
                    FirePropertyChanged("MaxTextLength");
                }
            }
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
