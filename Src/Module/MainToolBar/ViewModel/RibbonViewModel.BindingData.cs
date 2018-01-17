using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Naver.Compass.Common.Helper;
using System.Windows;

namespace MainToolBar.ViewModel
{

    partial class RibbonViewModel
    {
        #region Check Data Value

        public bool IsBoldCheck
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetPropertyBoolValueForText("vFontBold");
                }

                return false;
            }
        }

        public bool IsItalicCheck
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetPropertyBoolValueForText("vFontItalic");
                }

                return false;
            }
        }

        public bool IsUnderlineCheck
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetPropertyBoolValueForText("vFontUnderLine");
                }

                return false;
            }
        }

        public bool IsStrikeThroughCheck
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetPropertyBoolValueForText("vFontStrickeThrough");
                }

                return false;
            }
        }

        public bool IsBulletCheck
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBulletBoolValue();
                }

                return false;
            }

        }

        public bool IsTxtAlignLeft
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextHorAligen") == 0);
                }

                return false;
            }
        }

        public bool IsTxtAlignCenter
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextHorAligen") == 1);
                }

                return false;
            }
        }

        public bool IsTxtAlignRight
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextHorAligen") == 2);
                }

                return false;
            }
        }

        public bool IsTxtAlignTop
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextVerAligen") == 3);
                }

                return false;
            }
        }

        public bool IsTxtAlignMiddle
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextVerAligen") == 1);
                }

                return false;
            }
        }

        public bool IsTxtAlignBottom
        {
            get
            {
                if (_model != null)
                {
                    return (_model.GetAlignPropertyBoolValue("vTextVerAligen") == 4);
                }

                return false;
            }
        }

        public Brush FontColorView
        {
            get
            {
                if (_model != null)
                {
                    return (new SolidColorBrush(_model.GetFontColorValue()));
                }
                return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }

        }

        public StyleColor BackgroundModifyColorView
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBackgroundColorModifyValue();
                }

                return new StyleColor(ColorFillType.Solid, -1);
            }
        }

        public StyleColor BackgroundColorView
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBackgroundColorValue();
                }

                return new StyleColor(ColorFillType.Solid, -1);
            }

        }

        public StyleColor BorderLineColorView
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBorderlineColorValue();
                }
                return new StyleColor(ColorFillType.Solid, -1);
            }

        }

        public bool BackgroundGradientEnable
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBackgroundGradientEnable();
                }
                return false;
            }
        }

        public bool BorderlineGradientEnable
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetBorderlineGradientEnable();
                }
                return false;
            }
        }

        public string Visibility_Toolbar
        {
            get
            {
                if (IsShowToolbar)
                {
                    return @"Visible";
                }

                return @"Collapsed";
            }
        }

        public bool IsFomratCheck
        {
            get
            {
                if (_model != null)
                {
                    return _model.GetPaintFormatState();
                }
                return false;
            }
        }

        public string IncreaseTooltip
        {
            get
            {
                return GlobalData.FindResource("TB_Tooltip_IncreaseFontSize") + " (Ctrl+Shift+>)";
            }
        }

        public string DecreaseTooltip
        {
            get
            {
                return GlobalData.FindResource("TB_Tooltip_DecreaseFontSize") + " (Ctrl+Shift+<)";
            }
        }

        public Visibility LibraryVisibility
        {
            get
            {
                if (_doc.Document != null && _doc.Document.DocumentType == DocumentType.Library)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public Visibility DocumentVisibility
        {
            get
            {
                if (_doc.Document != null && _doc.Document.DocumentType == DocumentType.Library)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }
        #endregion
    }
}
