using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service;
using Naver.Compass.Common.CommonBase;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.InfoStructure;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Naver.Compass.Service.Document;

namespace Naver.Compass.MainToolBar.Module
{
    class RibbonModel
    {
        public RibbonModel()
        {
            _Selection = ServiceLocator.Current.GetInstance<ISelectionService>();

            _Painter = ServiceLocator.Current.GetInstance<IFormatPainterService>();
        }

        #region private member

        ISelectionService _Selection = null;

        IFormatPainterService _Painter = null;

        #endregion

        #region public property for update

        public bool GetPropertyBoolValueForText(string sProperty)
        {
            bool bReturnValue = false;
            bool bInitValue = true;

            foreach (WidgetViewModBase data in GetEligibleWidgets_Toolbar(PropertyOption.Option_Text))
            {
                Type iType = data.GetType();
                PropertyInfo propertyInfo = iType.GetProperty(sProperty);

                if (propertyInfo == null)
                {
                    bReturnValue = false;
                    break;
                }

                //one value false->all set false
                if (bInitValue != Convert.ToBoolean(propertyInfo.GetValue(data, null)))
                {
                    bReturnValue = false;
                    break;
                }
                else
                {
                    bReturnValue = true;
                }

            }

            return bReturnValue;
        }

        public bool GetBulletBoolValue()
        {
            var ListWidget = GetEligibleWidgets_Toolbar(PropertyOption.Option_Bullet);

            if (ListWidget.Count < 1)
            {
                return false;
            }

            bool bReturnValue = true;

            foreach (WidgetViewModBase data in ListWidget)
            {
                Type iType = data.GetType();
                PropertyInfo propertyInfo = iType.GetProperty("vTextBulletStyle");

                if (propertyInfo == null)
                {
                    bReturnValue = false;
                    break;
                }

                TextMarkerStyle style = (TextMarkerStyle)(propertyInfo.GetValue(data, null));

                bReturnValue &= (style == TextMarkerStyle.Disc);

            }

            return bReturnValue;
        }

        public int GetAlignPropertyBoolValue(string sProperty)
        {
            List<WidgetViewModBase> ListWidget;

            if (sProperty.Equals("vTextHorAligen"))
            {
                ListWidget = GetEligibleWidgets_Toolbar(PropertyOption.Option_TextHor);
            }
            else
            {
                ListWidget = GetEligibleWidgets_Toolbar(PropertyOption.Option_TextVer);
            }
            int iReValue = -2;
            foreach (WidgetViewModBase data in ListWidget)
            {
                Type iType = data.GetType();
                PropertyInfo propertyInfo = iType.GetProperty(sProperty);

                if (propertyInfo == null)
                {
                    iReValue = -2;
                    break;
                }

                if (iReValue == -2)
                {
                    iReValue = Convert.ToInt32(propertyInfo.GetValue(data, null));
                }
                else if (iReValue != Convert.ToInt32(propertyInfo.GetValue(data, null)))
                {
                    iReValue = -2;
                    break;
                }
                else
                {
                    iReValue = Convert.ToInt32(propertyInfo.GetValue(data, null));
                }
            }

            return iReValue;
        }

        public int GetBorderLineValue(string sProperty)
        {
            var ListWidget = GetEligibleWidgets_Toolbar(PropertyOption.Option_Border);
            int iReValue = -1;
            foreach (WidgetViewModBase data in ListWidget)
            {
                Type iType = data.GetType();
                PropertyInfo propertyInfo = iType.GetProperty(sProperty);

                if (propertyInfo == null)
                {
                    iReValue = -1;
                    break;
                }

                if (iReValue == -1)
                {
                    iReValue = Convert.ToInt32(propertyInfo.GetValue(data, null));
                }
                else if (iReValue != Convert.ToInt32(propertyInfo.GetValue(data, null)))
                {
                    iReValue = -1;
                    break;
                }
                else
                {
                    iReValue = Convert.ToInt32(propertyInfo.GetValue(data, null));
                }
            }

            return iReValue;
        }

        public Color GetFontColorValue()
        {
            Color sReturnValue = new Color();
            var ListWidget = GetEligibleWidgets_Toolbar(PropertyOption.Option_Text);

            foreach (WidgetViewModBase data in ListWidget)
            {
                sReturnValue = Color.FromArgb(data.vFontColor.A, data.vFontColor.R, data.vFontColor.G, data.vFontColor.B);

                break;
            }

            return sReturnValue;
        }

        public StyleColor GetBackgroundColorValue()
        {
            var ListWidget = GetEligibleWidgets_Toolbar(PropertyOption.OPtion_BackColor);
            if (ListWidget.Count() == 0) return default(StyleColor);
            StyleColor sReturnValue = ListWidget.First().vBackgroundColor;

            foreach (var data in ListWidget)
            {
                if (!sReturnValue.Equals(data.vBackgroundColor))
                {
                    return new StyleColor(ColorFillType.Solid, -1);
                }
            }

            return sReturnValue;
        }

        public StyleColor GetBackgroundColorModifyValue()
        {
            var ListWidget = GetEligibleWidgets_Toolbar(PropertyOption.OPtion_BackColor);
            if (ListWidget.Count() == 0) return default(StyleColor);
            StyleColor sReturnValue = ListWidget.First().vBackgroundColor;

            foreach (WidgetViewModBase data in ListWidget)
            {
                if (!sReturnValue.Equals(data.vBackgroundColor))
                {
                    return data.vBackgroundColor.FillType == ColorFillType.Gradient ? data.vBackgroundColor : sReturnValue;
                }
                else
                {
                    sReturnValue = data.vBackgroundColor;
                }
            }

            return sReturnValue;
        }

        public StyleColor GetBorderlineColorValue()
        {
            StyleColor sReturnValue = default(StyleColor);
            var ListWidget = GetEligibleWidgets_Toolbar(PropertyOption.Option_Border);

            foreach (WidgetViewModBase data in ListWidget)
            {
                sReturnValue = data.vBorderLineColor;

                break;
            }

            return sReturnValue;
        }

        public ArrowStyle GetLineArrowStyleValue()
        {
            ArrowStyle sReturnValue = ArrowStyle.Default;
            var ListWidget = GetEligibleWidgets_Toolbar(PropertyOption.Option_LineArrow);

            foreach (WidgetViewModBase data in ListWidget)
            {
                if (data is WidgetLineViewModeBase)
                {
                    if (sReturnValue == ArrowStyle.Default)
                    {
                        sReturnValue = ((WidgetLineViewModeBase)data).LineArrowStyle;
                    }
                    else if (sReturnValue != ((WidgetLineViewModeBase)data).LineArrowStyle)
                    {
                        sReturnValue = ArrowStyle.Default;
                        break;
                    }
                }
            }

            return sReturnValue;
        }

        public bool GetBackgroundGradientEnable()
        {
            List<IWidgetPropertyData> ListWidget = _Selection.GetSelectedWidgets();

            foreach (WidgetViewModBase data in ListWidget)
            {
                if(data.IsSupportGradientBackground)
                {
                    return true;
                }
            }

            return false;
        }

        public bool GetBorderlineGradientEnable()
        {
            List<IWidgetPropertyData> ListWidget = _Selection.GetSelectedWidgets();

            foreach (WidgetViewModBase data in ListWidget)
            {
                if (data.IsSupportGradientBorderline)
                {
                    return true;
                }
            }

            return false;
        }

        public string GetPropertyFontSizeValue(string sProperty)
        {
            var ListWidget = GetEligibleWidgets_Toolbar(PropertyOption.Option_Text);
            double iReValue = double.MinValue;
            foreach (WidgetViewModBase data in ListWidget)
            {
                Type iType = data.GetType();
                PropertyInfo propertyInfo = iType.GetProperty(sProperty);

                if (propertyInfo == null)
                {
                    iReValue = double.MinValue;
                    break;
                }

                if (iReValue == double.MinValue)
                {
                    iReValue = Convert.ToDouble(propertyInfo.GetValue(data, null));
                }
                else if (iReValue != Convert.ToDouble(propertyInfo.GetValue(data, null)))
                {
                    iReValue = double.MinValue;
                    break;
                }
                else
                {
                    iReValue = Convert.ToDouble(propertyInfo.GetValue(data, null));
                }
            }

            return (iReValue < 0) ? "" : iReValue.ToString("0");
        }
        public string GetPropertyFontFamilyValue(string sProperty)
        {
            string sReturnValue = "install";
            var ListWidget =GetEligibleWidgets_Toolbar(PropertyOption.Option_Text);

            foreach (WidgetViewModBase data in ListWidget)
            {
                Type iType = data.GetType();
                PropertyInfo propertyInfo = iType.GetProperty(sProperty);

                if (propertyInfo == null)
                {
                    sReturnValue = "";
                    break;
                }

                if (String.Compare(sReturnValue, "install") == 0)
                {
                    sReturnValue = Convert.ToString(propertyInfo.GetValue(data, null));
                }
                else if (sReturnValue != Convert.ToString(propertyInfo.GetValue(data, null)))
                {
                    sReturnValue = "";
                    break;
                }
            }

            return sReturnValue == "install" ? "" : sReturnValue;
        }

        public List<string> GetRecentFiles()
        {
            return ConfigFileManager.RecentFiles();
        }

        public bool GetPaintFormatState()
        {
            return _Painter.GetMode();
        }

        public bool IsSupportProperty_Toolbar(PropertyOption type)
        {
            List<WidgetViewModBase> allSelects = _Selection.GetSelectedWidgets().OfType<WidgetViewModBase>().ToList<WidgetViewModBase>();
            switch (type)
            {
                case PropertyOption.Option_Border:
                    return allSelects.Exists(k => k.IsSupportBorber == true);
                case PropertyOption.OPtion_BackColor:
                    return allSelects.Exists(k => k.IsSupportBackground == true);
                case PropertyOption.Option_Bullet:
                    return allSelects.Exists(k => k.IsSupportTextRotate == true);
                case PropertyOption.Option_LineArrow:
                    return allSelects.Exists(k => k.IsSupportArrowStyle == true);
                case PropertyOption.Option_Text:
                    return allSelects.Exists(k => k.IsSupportText == true);
                case PropertyOption.Option_TextHor:
                    return allSelects.Exists(k => k.IsSupportTextHorAlign == true);
                case PropertyOption.Option_TextVer:
                    return allSelects.Exists(k => k.IsSupportTextVerAlign == true);
                case PropertyOption.Option_TextRotate:
                    return allSelects.Exists(k => k.IsSupportTextRotate == true);
                case PropertyOption.Option_WidgetRotate:
                    return allSelects.Exists(k => k.IsSupportRotate == true);
                case PropertyOption.Option_CornerRadius:
                    return allSelects.Exists(k => k.IsSupportCornerRadius == true);
            }

            System.Diagnostics.Debug.Assert(false, "IsSupportProperty_Toolbar type error");
            return false;
        }

        List<WidgetViewModBase> GetEligibleWidgets_Toolbar(PropertyOption type)
        {
            var ListWidget = _Selection.GetSelectedWidgets().OfType<WidgetViewModBase>().ToList <WidgetViewModBase>();
            switch (type)
            {
                case PropertyOption.Option_Border:
                    return ListWidget.FindAll(k => k.IsSupportBorber == true);
                case PropertyOption.Option_Text:
                    return ListWidget.FindAll(k => k.IsSupportText == true);
                case PropertyOption.Option_LineArrow:
                    return ListWidget.FindAll(k => k.IsSupportArrowStyle == true);
                case PropertyOption.Option_TextHor:
                    return ListWidget.FindAll(k => k.IsSupportTextHorAlign == true);
                case PropertyOption.Option_TextVer:
                    return ListWidget.FindAll(k => k.IsSupportTextVerAlign == true);
                case PropertyOption.Option_Bullet:
                    return ListWidget.FindAll(k => k.IsSupportTextRotate == true);
                case PropertyOption.OPtion_BackColor:
                    return ListWidget.FindAll(k => k.IsSupportBackground == true);
                case PropertyOption.Option_TextRotate:
                    return ListWidget.FindAll(k => k.IsSupportTextRotate == true);
                case PropertyOption.Option_WidgetRotate:
                    return ListWidget.FindAll(k => k.IsSupportRotate == true);
                case PropertyOption.Option_CornerRadius:
                    return ListWidget.FindAll(k => k.IsSupportCornerRadius == true);
                default:
                    System.Diagnostics.Debug.Assert(false, "GetEligibleWidgets_Toolbar function error!");
                    return null;
            }
        }

        #endregion

        #region Public Function and Member
        public IInputElement CmdTarget
        {
            get;
            set;
        }
        private double left;
        public double Left
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _Selection.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return double.NaN;

                double val = (wdgs[0] as WidgetViewModelDate).Raw_Left;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.Raw_Left)
                        val = Math.Min(val, item.Left);
                }
                left = val;
                return Math.Round(val);
            }
            set
            {
                if (value == left)
                    return;
                WidgetPropertyCommands.AllLeft.Execute(value - left, CmdTarget);
            }
        }

        private double top;
        public double Top
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _Selection.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return double.NaN;

                double val = (wdgs[0] as WidgetViewModelDate).Raw_Top;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.Raw_Top)
                        val = Math.Min(val, item.Top);
                }
                top = val;
                return Math.Round(val);
            }
            set
            {
                if (value == top)
                    return;
                WidgetPropertyCommands.AllTop.Execute(value - top, CmdTarget);
            }
        }

        private double width;
        public double Width
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _Selection.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return double.NaN;

                double val = (wdgs[0] as WidgetViewModelDate).ItemWidth;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.ItemWidth)
                        return double.NaN;
                }
                return Math.Round(val);
            }
            set
            {
                WidgetPropertyCommands.Width.Execute(value, CmdTarget); 
            }
        }

        private double height;
        public double Height
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _Selection.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return double.NaN;

                double val = (wdgs[0] as WidgetViewModelDate).ItemHeight;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.ItemHeight)
                        return double.NaN;
                }
                return Math.Round(val);
            }
            set
            {
                WidgetPropertyCommands.Height.Execute(value, CmdTarget);   
            }
        }

        public bool CanLocationEdit
        {
            get
            {
                return _Selection.WidgetNumber > 0;
            }
        }
        #endregion
    }
}
