using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Service;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.WidgetLibrary;
using System.Windows.Media;
using System.Windows;
using System.Reflection;
using Naver.Compass.Service.Document;


namespace Naver.Compass.Module
{
    class PropertyPageModel
    {
        private ISelectionService _selectionService;

        private static PropertyPageModel _instance;
        public static PropertyPageModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PropertyPageModel();
            }
            return _instance;
        }
        private PropertyPageModel()
        {
            //Monitor the property change
            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            CmdTarget = null;
        } 
        
        #region Binding Two-way Property Data
        public double Left
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return double.NaN;

                double val = (wdgs[0] as WidgetViewModelDate).Raw_Left;
                foreach (WidgetViewModelDate item in wdgs)
                {
                
                    if (val != item.Raw_Left)
                        return double.NaN;
                }
                
                return  Math.Round(val);
            }
            set
            {
                WidgetPropertyCommands.Left.Execute(value, CmdTarget);                
            }
        }
        public double Top
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return double.NaN;

                double val = (wdgs[0] as WidgetViewModelDate).Raw_Top;

                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.Raw_Top)
                        return double.NaN;
                }

                return  Math.Round(val);
            }
            set
            {
                WidgetPropertyCommands.Top.Execute(value, CmdTarget);    
            }
        }
        public double Width
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return double.NaN;

                double val = (wdgs[0] as WidgetViewModelDate).ItemWidth;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.ItemWidth)
                        return double.NaN;
                }
                return  Math.Round(val);
            }
            set
            {
                WidgetPropertyCommands.Width.Execute(value, CmdTarget);                  
            }
        }
        public double Height
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
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

        public double MenuPageLeft
        {
            get
            {
                IEnumerable<HamburgerMenuViewModel> wdgs = _selectionService.GetSelectedWidgets().OfType<HamburgerMenuViewModel>();
                if (wdgs.Count() == 0)
                    return double.NaN;

                double val = (wdgs.ElementAt(0)).MenuPageLeft;
                foreach (HamburgerMenuViewModel item in wdgs)
                {
                    if (val != item.MenuPageLeft)
                        return double.NaN;
                }


                return Math.Round(val);
            }
            set
            {
                WidgetPropertyCommands.MenuPageLeft.Execute(value, CmdTarget);
            }
        }
        public double MenuPageTop
        {
            get
            {
                IEnumerable<HamburgerMenuViewModel> wdgs = _selectionService.GetSelectedWidgets().OfType<HamburgerMenuViewModel>();
                if (wdgs.Count() == 0)
                    return double.NaN;

                double val = (wdgs.ElementAt(0)).MenuPageTop;
                foreach (HamburgerMenuViewModel item in wdgs)
                {
                    if (val != item.MenuPageTop)
                        return double.NaN;
                }
                return Math.Round(val);
            }
            set
            {
                WidgetPropertyCommands.MenuPageTop.Execute(value, CmdTarget);
            }
        }
        public double MenuPageWidth
        {
            get
            {
                IEnumerable<HamburgerMenuViewModel> wdgs = _selectionService.GetSelectedWidgets().OfType<HamburgerMenuViewModel>();
                if (wdgs.Count() == 0)
                    return double.NaN;

                double val = (wdgs.ElementAt(0)).MenuPageWidth;
                foreach (HamburgerMenuViewModel item in wdgs)
                {
                    if (val != item.MenuPageWidth)
                        return double.NaN;
                }
                return Math.Round(val);
            }
            set
            {
                WidgetPropertyCommands.MenuPageWidth.Execute(value, CmdTarget);
            }
        }
        public double MenuPageHeight
        {
            get
            {
                IEnumerable<HamburgerMenuViewModel> wdgs = _selectionService.GetSelectedWidgets().OfType<HamburgerMenuViewModel>();
                if (wdgs.Count() == 0)
                    return double.NaN;

                double val = (wdgs.ElementAt(0)).MenuPageHeight;
                foreach (HamburgerMenuViewModel item in wdgs)
                {
                    if (val != item.MenuPageHeight)
                        return double.NaN;
                }
                return Math.Round(val);
            }
            set
            {
                WidgetPropertyCommands.MenuPageHeight.Execute(value, CmdTarget);
            }
        }

        public bool? IsMenuPageHidden
        {
            get
            {
                IEnumerable<HamburgerMenuViewModel> wdgs = _selectionService.GetSelectedWidgets().OfType<HamburgerMenuViewModel>();
                if (wdgs.Count() == 0)
                    return null;

                bool val = (wdgs.ElementAt(0)).IsMenuPageHidden;
                foreach (HamburgerMenuViewModel item in wdgs)
                {
                    if (val != item.IsMenuPageHidden)
                        return null;
                }
                return val;
            }
            set
            {
                WidgetPropertyCommands.MenuPageHide.Execute(value, CmdTarget);
            }
        }
        
        public int RotateAngle
        {
            get
            {
                var wdgs = GetEligibleWidgets_Panel(PropertyOption.Option_WidgetRotate);
                if (wdgs.Count == 0)
                    return int.MinValue;

                int val = (wdgs[0] as WidgetViewModelDate).RotateAngle;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.RotateAngle)
                        return int.MinValue;
                }
                return val;
            }
            set
            {
                WidgetPropertyCommands.Rotate.Execute(value, CmdTarget);                 
            }
        }
        public int CornerRadius
        {
            get
            {
                var wdgs = GetEligibleWidgets_Panel(PropertyOption.Option_CornerRadius);
                if (wdgs.Count == 0)
                    return int.MinValue;

                int val = (wdgs[0] as WidgetViewModelDate).CornerRadius;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.CornerRadius)
                        return int.MinValue;
                }
                return val;
            }
            set
            {
                WidgetPropertyCommands.CornerRadius.Execute(value, CmdTarget);
            }
        }
        public int TextRotate
        {
            get
            {
                var wdgs = GetEligibleWidgets_Panel(PropertyOption.Option_TextRotate);
                if (wdgs.Count == 0)
                    return int.MinValue;

                int val = (wdgs[0] as WidgetViewModelDate).TextRotate;
                foreach (WidgetViewModBase item in wdgs)
                {
                    if (item.IsSupportText == false)
                    {
                        return int.MinValue;
                    }
                    if (val != item.TextRotate)
                    {
                        return int.MinValue;
                    }
                }
                return val;
            }
            set
            {
                WidgetPropertyCommands.TextRotate.Execute(value, CmdTarget);
            }
        }
        public double Opacity
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return double.NaN;

                double val = (wdgs[0] as WidgetViewModelDate).Opacity;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.Opacity)
                        return double.NaN;
                }
                return val;
            }
            set
            {
                WidgetPropertyCommands.Opacity.Execute(value, CmdTarget); 
            }
        }
        public bool? IsHidden
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return null;


                bool val = (wdgs[0] as WidgetViewModelDate).IsHidden;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.IsHidden)
                        return null;
                }
                return val;
            }
            set
            {
                WidgetPropertyCommands.Hide.Execute(value, CmdTarget); 
            }
        }

        public bool? IsFixed
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return null;


                bool val = (wdgs[0] as WidgetViewModelDate).IsFixed;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.IsFixed)
                        return null;
                }
                return val;
            }
            set
            {
                WidgetPropertyCommands.IsFixed.Execute(value, CmdTarget);
            }
        }

        public bool IsFixedEnabled
        {
            get
            {
                IPagePropertyData CurPage = _selectionService.GetCurrentPage();
                if(CurPage==null)
                {
                    return false;
                }

                if(CurPage.PageType!=PageType.NormalPage)
                {
                    return false;
                }

                 List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                 var widget = wdgs.FirstOrDefault(a => a.ParentID!=Guid.Empty);
                 if (widget != null)
                     return false;
                 else
                     return true;
            }
        }
        public string Tooltip
        {
            get
            {
                List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
                if (wdgs.Count == 0)
                    return string.Empty;

                string val = (wdgs[0] as WidgetViewModelDate).Tooltip;
                foreach (WidgetViewModelDate item in wdgs)
                {
                    if (val != item.Tooltip)
                        return string.Empty;
                }
                return val;
            }
            set
            {
                WidgetPropertyCommands.Tooltip.Execute(value, CmdTarget); 
            }
        }
        #endregion

        #region Binding Read-Only Property Data    
        //public ImageSource ImgSource
        //{
        //    get
        //    {
        //        List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
        //        if (wdgs.Count != 1)
        //            return null;

        //        ImageWidgetViewModel img = (wdgs[0] as ImageWidgetViewModel);
        //        if (img == null)
        //            return null;
        //        return img.ImgSource;
        //    }
        //}
        #endregion

        #region Public Function and Member
        public IInputElement CmdTarget
        {
            get;
            set;
        }
       
        public int GetWidgetsNumber()
        {
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            return wdgs.Count;
        }        
        public bool IsCanEditImage()
        {
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count != 1)
                return false;

            WidgetViewModBase wdg =wdgs[0] as WidgetViewModBase;
            if (wdg.Type == ObjectType.Image)
            {
                return true;
            }
            return false;
        }
        public bool IsCanEditText()
        {
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return false;

            var val = (wdgs[0] as WidgetViewModBase).Type;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (item.Type == ObjectType.Image
                    ||item.Type == ObjectType.Group
                    || item.Type == ObjectType.HotSpot)
                    return false;
            }
            return true;
        }
        public bool IsCanTooltipText()
        {
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return false;

            var val = (wdgs[0] as WidgetViewModBase).Type;
            foreach (WidgetViewModBase item in wdgs)
            {
                if (item.Type == ObjectType.Group
                    || item.Type == ObjectType.HotSpot 
                    || item.Type == ObjectType.HorizontalLine 
                    || item.Type == ObjectType.VerticalLine)
                    return false;
            }
            return true;
        }
        public bool IsCanRotate()
        {
            return IsSupportProperty_Panel(PropertyOption.Option_WidgetRotate);

            //List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            //if (wdgs.Count == 0)
            //    return false;

            //foreach (WidgetViewModBase item in wdgs)
            //{
            //    if (item.IsSupportRotate == false)
            //    {
            //        return false;
            //    }
            //}
            //return true;
        }

        public bool IsCanCornerRaius()
        {
            return IsSupportProperty_Panel(PropertyOption.Option_CornerRadius);

            //List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            //if (wdgs.Count == 0)
            //    return false;

            //foreach (WidgetViewModBase item in wdgs)
            //{
            //    if ((item is RoundedRecWidgetViewModel)==false)
            //    {
            //        return false;
            //    }
            //}
            //return true;
        }
        public bool IsCanTextRotate()
        {
            return IsSupportProperty_Panel(PropertyOption.Option_TextRotate);

            //List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            //if (wdgs.Count == 0)
            //    return false;

            //foreach (WidgetViewModBase item in wdgs)
            //{
            //    if (item.IsSupportTextRotate == false)
            //    {
            //        return false;
            //    }
            //}
            //return true;
        }

        //if selected widgets all hamburger 
        public bool IsHamburgerWidget()
        {
            List<IWidgetPropertyData> wdgs = _selectionService.GetSelectedWidgets();
            if (wdgs.Count == 0)
                return false;

            foreach (WidgetViewModBase item in wdgs)
            {
                if (!(item is HamburgerMenuViewModel))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region public property for update

        public bool GetPropertyBoolValueForText(string sProperty)
        {
            var ListWidget = GetEligibleWidgets_Panel(PropertyOption.Option_Text);

            bool bReturnValue = false;
            bool bInitValue = true;

            foreach (WidgetViewModBase data in ListWidget)
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
            var ListWidget =GetEligibleWidgets_Panel(PropertyOption.Option_Bullet);

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
                ListWidget = GetEligibleWidgets_Panel(PropertyOption.Option_TextHor);
            }
            else
            {
                ListWidget = GetEligibleWidgets_Panel(PropertyOption.Option_TextVer);
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

        public string GetPropertyFontSizeValue(string sProperty)
        {
            var ListWidget = GetEligibleWidgets_Panel(PropertyOption.Option_Text);
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
            var ListWidget = GetEligibleWidgets_Panel(PropertyOption.Option_Text);

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

        public Color GetFontColorValue()
        {
            Color sReturnValue = new Color();
            var ListWidget = GetEligibleWidgets_Panel(PropertyOption.Option_Text);

            foreach (WidgetViewModBase data in ListWidget)
            {
                sReturnValue = Color.FromArgb(data.vFontColor.A, data.vFontColor.R, data.vFontColor.G, data.vFontColor.B);

                break;
            }

            return sReturnValue;
        }

        public StyleColor GetBackgroundColorValue()
        {
            var ListWidget = GetEligibleWidgets_Panel(PropertyOption.OPtion_BackColor);
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
            var ListWidget = GetEligibleWidgets_Panel(PropertyOption.OPtion_BackColor);;
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
            var sReturnValue = default(StyleColor);
            var ListWidget = GetEligibleWidgets_Panel(PropertyOption.Option_Border);

            foreach (WidgetViewModBase data in ListWidget)
            {
                sReturnValue = data.vBorderLineColor;

                break;
            }

            return sReturnValue;
        }

        public bool GetBackgroundGradientEnable()
        {
            var ListWidget = GetEligibleWidgets_Panel(PropertyOption.OPtion_BackColor); ;

            foreach (WidgetViewModBase data in ListWidget)
            {
                if (data.IsSupportGradientBackground)
                {
                    return true;
                }
            }

            return false;
        }

        public bool GetBorderlineGradientEnable()
        {
            var ListWidget = GetEligibleWidgets_Panel(PropertyOption.Option_Border); ;

            foreach (WidgetViewModBase data in ListWidget)
            {
                if (data.IsSupportGradientBorderline)
                {
                    return true;
                }
            }

            return false;
        }

        public int GetBorderLineValue(string sProperty)
        {
            var ListWidget = GetEligibleWidgets_Panel(PropertyOption.Option_Border); ;
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

        public List<WidgetViewModBase>  GetSelectionWidget()
        {
            List<WidgetViewModBase> Items = new List<WidgetViewModBase>();

             List<IWidgetPropertyData> ListWidget = _selectionService.GetSelectedWidgets();

            foreach (WidgetViewModBase data in ListWidget)
            {
                Items.Add(data);
            }

            return Items;
        }

        public bool IsSupportProperty_Panel(PropertyOption type)
        {
            var allSelects = GetSelectionWidget(); ;
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
                case PropertyOption.Option_WidgetRotate:
                    return allSelects.Exists(k => k.IsSupportRotate == true);
                case PropertyOption.Option_TextRotate:
                    return allSelects.Exists(k => k.IsSupportTextRotate == true);
                case PropertyOption.Option_CornerRadius:
                    return allSelects.Exists(k => k.IsSupportCornerRadius == true);
            }

            System.Diagnostics.Debug.Assert(false, "IsSupportProperty_Panel function error!");
            return false;
        }

        List<WidgetViewModBase> GetEligibleWidgets_Panel(PropertyOption type)
        {
            var ListWidget = GetSelectionWidget();
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
                case PropertyOption.Option_WidgetRotate:
                    return ListWidget.FindAll(k => k.IsSupportRotate == true);
                case PropertyOption.Option_TextRotate:
                    return ListWidget.FindAll(k => k.IsSupportTextRotate == true);
                case PropertyOption.Option_CornerRadius:
                    return ListWidget.FindAll(k => k.IsSupportCornerRadius == true);
               
                default:
                    System.Diagnostics.Debug.Assert(false, "GetEligibleWidgets_Panel function error!");
                    return null;
            }
        }

        #endregion 

    }
}
