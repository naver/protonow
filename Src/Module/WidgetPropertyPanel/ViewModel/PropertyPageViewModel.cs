using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using System.Windows.Input;
using System.Windows.Documents;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Media;
using Naver.Compass.Common.Helper;
using System.Windows;
using MainToolBar.ViewModel;
using Naver.Compass.Service.Document;
using Naver.Compass.Common;

namespace Naver.Compass.Module
{
    public partial class PropertyPageViewModel : ViewModelBase
    {
        #region Constructor
        public PropertyPageViewModel()
        {
            if (this.IsInDesignMode)
                return;
            _ListEventAggregator.GetEvent<SelectionChangeEvent>().Subscribe(SelectionChangeEventHandler);
            _ListEventAggregator.GetEvent<SelectionPropertyChangeEvent>().Subscribe(SelectionPropertyEventHandler);
            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeEventHandler);

            _model = PropertyPageModel.GetInstance();

            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            _ListEventAggregator.GetEvent<EnableFirePositionInRibbonEvent>().Subscribe(EnableFirePositionHandler);
        }
        #endregion Constructor

        #region private Function and Member
        private bool _isFirePositionEnabled = true;
        //protected ISelectionService _selectionService;
        protected bool ValidateValue(string value, double max, double min, out double newValue)
        {
            if (Double.TryParse(value, out newValue))
            {
                if (newValue < min)
                {
                    newValue = min;
                }

                if (newValue > max)
                {
                    newValue = max;
                }

                return true;
            }

            return false;
        }
        private bool ValidateValue(string value, int max, int min, out int newValue)
        {
            if (Int32.TryParse(value, out newValue))
            {
                if (newValue < min)
                {
                    newValue = min;
                }

                if (newValue > max)
                {
                    newValue = max;
                }

                return true;
            }

            return false;
        }

        private int AdjustAngleValue(string value)
        {
            int newValue = 0;
            if (Int32.TryParse(value, out newValue))
            {
                if (newValue < 0)
                {
                    newValue = newValue % 360 + 360;
                }
                else if (newValue >= 360)
                {
                    newValue = newValue % 360;
                }

                return newValue;
            }

            return 0;
        }

        private void UpdateEnableUI()
        {

            FirePropertyChanged("IsBoldCheck");
            FirePropertyChanged("IsItalicCheck");
            FirePropertyChanged("IsBulletCheck");
            FirePropertyChanged("IsUnderlineCheck");
            FirePropertyChanged("IsStrikeThroughCheck");
            FirePropertyChanged("IsTxtAlignTop");
            FirePropertyChanged("IsTxtAlignMiddle");
            FirePropertyChanged("IsTxtAlignBottom");
            FirePropertyChanged("IsTxtAlignLeft");
            FirePropertyChanged("IsTxtAlignCenter");
            FirePropertyChanged("IsTxtAlignRight");


            if (_model.GetWidgetsNumber() <= 0)
            {
                CanEdit = false;
                return;
            }
            else
            {
                CanEdit = true;
            }

            CanRotatEdit = _model.IsCanRotate();
            CanTextRotateEdit = _model.IsCanTextRotate();
            CanEditImg = _model.IsCanEditImage();
            CanTextEdit = _model.IsCanEditText();
            CanTooltipEdit = _model.IsCanTooltipText();
            CanCornerRaius = _model.IsCanCornerRaius();
        }
        #endregion

        #region Data Change Trigger Handler
        PropertyPageModel _model;

        ISelectionService _selectionService;

        private void EnableFirePositionHandler(bool enable)
        {
            _isFirePositionEnabled = enable;
        }
        private void SelectionPropertyEventHandler(string EventArg)
        {

            switch (EventArg)
            {
                case "Top":
                    {
                        if (_isFirePositionEnabled)
                        {
                            FirePropertyChanged("Top");
                        }
                        break;
                    }
                case "Left":
                    {
                        if (_isFirePositionEnabled)
                        {
                            FirePropertyChanged("Left");
                        }
                        break;
                    }
                case "ItemWidth":
                    {
                        FirePropertyChanged("Width");
                        break;
                    }
                case "ItemHeight":
                    {
                        FirePropertyChanged("Height");
                        break;
                    }
                case "RotateAngle":
                    {
                        FirePropertyChanged("RotateAngle");
                        break;
                    }
                case "CornerRadius":
                    {
                        FirePropertyChanged("CornerRadius");
                        break;
                    }
                case "Opacity":
                    {
                        FirePropertyChanged("Opacity");
                        break;
                    }
                case "IsHidden":
                    {
                        FirePropertyChanged("IsHidden");
                        break;
                    }
                case "IsFixed":
                    {
                        FirePropertyChanged("IsFixed");
                        break;
                    }
                case "vFontBold":
                    {
                        FirePropertyChanged("IsBoldCheck");
                        break;
                    }
                case "vFontItalic":
                    {
                        FirePropertyChanged("IsItalicCheck");
                        break;
                    }
                case "vTextBulletStyle":
                    {
                        FirePropertyChanged("IsBulletCheck");
                        break;
                    }

                case "vFontUnderLine":
                    {
                        FirePropertyChanged("IsUnderlineCheck");
                        break;
                    }
                case "vFontStrickeThrough":
                    {
                        FirePropertyChanged("IsStrikeThroughCheck");
                        break;
                    }
                case "vFontColor":
                    {
                        FirePropertyChanged("FontColorView");
                        //var stackTrace = new System.Diagnostics.StackTrace();
                        //var undo = stackTrace.GetFrames().Where(f => f.GetMethod().Name == "Undo" || f.GetMethod().Name == "Redo");

                        GalleryData<StyleColor> Bdata = _dataCollection["Font Color Gallery"] as GalleryData<StyleColor>;
                        if (Bdata != null)
                        {
                            Bdata.SelectedItem = FontColorView.ToStyleColor();
                        }

                        //if (undo.Count() == 0)
                        //{
                        //    SplitMenuItemData spData = _dataCollection["Font Color"] as SplitMenuItemData;
                        //    if (spData != null)
                        //    {
                        //        //spData.CommandParameter = FontColorView;
                        //    }
                        //}
                        break;
                    }
                case "vBorderLineColor":
                    {
                        FirePropertyChanged("BorderLineColorView");
                        //var stackTrace = new System.Diagnostics.StackTrace();
                        //var undo = stackTrace.GetFrames().Where(f => f.GetMethod().Name == "Undo" || f.GetMethod().Name == "Redo");

                        GalleryData<StyleColor> Bdata = _dataCollection["BorderLine Color Gallery"] as GalleryData<StyleColor>;
                        if (Bdata != null)
                        {
                            Bdata.SelectedItem = BorderLineColorView;
                        }

                        //if (undo.Count() == 0)
                        //{
                        //    SplitButtonData sbData = _dataCollection["BorderLineColor"] as SplitButtonData;
                        //    if (sbData != null)
                        //    {
                        //        sbData.CommandParameter = BorderLineColorView;
                        //    }
                        //}
                        break;
                    }
                case "vBackgroundColor":
                    {
                        FirePropertyChanged("BackgroundColorView");
                        //var stackTrace = new System.Diagnostics.StackTrace();
                        //var undo = stackTrace.GetFrames().Where(f => f.GetMethod().Name == "Undo" || f.GetMethod().Name == "Redo");

                        GalleryData<StyleColor> Bdata = _dataCollection["Background Color Gallery"] as GalleryData<StyleColor>;
                        if (Bdata != null)
                        {
                            Bdata.SelectedItem = BackgroundModifyColorView;
                        }
                        
                        //if (undo.Count() == 0)
                        //{

                        //    SplitBackgroundColorButton sbData = _dataCollection["BackColor"] as SplitBackgroundColorButton;
                        //    if (sbData != null)
                        //    {
                        //        sbData.CommandParameter = BackgroundModifyColorView;
                        //    }
                        //}

                        break;
                    }
                case "vTextHorAligen":
                    {
                        FirePropertyChanged("IsTxtAlignLeft");
                        FirePropertyChanged("IsTxtAlignCenter");
                        FirePropertyChanged("IsTxtAlignRight");
                        break;
                    }
                case "vTextVerAligen":
                    {
                        FirePropertyChanged("IsTxtAlignTop");
                        FirePropertyChanged("IsTxtAlignMiddle");
                        FirePropertyChanged("IsTxtAlignBottom");
                        break;
                    }
                case "TextRotate":
                    {
                        FirePropertyChanged("TextRotate");
                        break;
                    }
                case "vBorderlineStyle":
                    {
                        UpdateBorderLineStyleSelectedItem();
                        UpdateBorderLineStyleButtonParameter();
                        break;
                    }
                case "vBorderLinethinck":
                    {
                        UpdateBorderLineWidthSelectedItem();
                        UpdateBorderLineWidthButtonParameter();
                        break;
                    }
                case "NailStream":
                case "Tooltip":
                case "IsShowSelect":
                case "IsBtnAlignLeft":
                case "IsDisabled":
                case "RadioGroup":
                    {
                        UpdateWidgetProperty(EventArg);
                        break;
                    }
                default:
                    StyChangeHandler(EventArg);
                    return;
            }

        }
        private void SelectionPageChangeEventHandler(Guid EventArg)
        {
            FirePropertyChanged("Top");
            FirePropertyChanged("Left");
            FirePropertyChanged("Width");
            FirePropertyChanged("Height");
            FirePropertyChanged("RotateAngle");
            FirePropertyChanged("CornerRadius");
            FirePropertyChanged("Opacity");
            FirePropertyChanged("IsHidden");
            FirePropertyChanged("IsFixed");
            FirePropertyChanged("IsFixedEnabled");
            FirePropertyChanged("TextRotate");

            //Fire Color 
            //FirePropertyChanged("BackgroundColorView");
            ////FirePropertyChanged("BackgroundColor32");
            //FirePropertyChanged("FontColorView");
            //FirePropertyChanged("BorderLineColorView");
            GalleryData<StyleColor> cdata = _dataCollection["Background Color Gallery"] as GalleryData<StyleColor>;
            if (cdata != null)
            {
                cdata.SelectedItem = BackgroundColorView;
            }

            var Bdata = _dataCollection["Font Color Gallery"] as GalleryData<Brush>;
            if (Bdata != null)
            {
                Bdata.SelectedItem = FontColorView;
            }

            cdata = _dataCollection["BorderLine Color Gallery"] as GalleryData<StyleColor>;
            if (cdata != null)
            {
                cdata.SelectedItem = BorderLineColorView;
            }

            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            IPagePropertyData page = _selectionService.GetCurrentPage();
            if (page == null)
            {
                return;
            }
            if (CmdTarget == page.EditorCanvas)
            {
                return;
            }
            CmdTarget = _selectionService.GetCurrentPage().EditorCanvas;
          
            SetStyleCmdTarget(_selectionService.GetCurrentPage().EditorCanvas);

            UpdateCmdTarget(CmdTarget);

        }
        private void SelectionChangeEventHandler(string EventArg)
        {
            FirePropertyChanged("Top");
            FirePropertyChanged("Left");
            FirePropertyChanged("Width");
            FirePropertyChanged("Height");
            FirePropertyChanged("RotateAngle");
            FirePropertyChanged("CornerRadius");
            FirePropertyChanged("Opacity");
            FirePropertyChanged("IsHidden");
            FirePropertyChanged("IsFixed");
            FirePropertyChanged("IsFixedEnabled");
            FirePropertyChanged("TextRotate");

            UpdateSelectedItem();
            UpdateEnableUI();
            StyChangeHandler("vFontFamily");
            StyChangeHandler("vFontSize");
            CanSupportProperty();

            UpdatePropertyView();
        }

        #endregion

        #region Common Binding Propery

        #region Command Target
        //public IInputElement _cmdTarget = null;
        public IInputElement CmdTarget
        {
            get
            {
                return _model.CmdTarget;
            }
            set
            {
                if (_model.CmdTarget != value)
                {
                    _model.CmdTarget = value;
                    FirePropertyChanged("CmdTarget");
                }
            }
        }
        #endregion

        #region  Widget Property
        public string Left
        {
            get
            {
                if (double.IsNaN(_model.Left))
                    return null;
                return _model.Left.ToString();
            }
            set
            {
                double newValue;
                if (ValidateValue(value, CommonDefine.MaxEditorWidth, CommonDefine.MinWidgetLocation, out newValue))
                {
                    if (_model.Left != newValue)
                    {
                        _model.Left = newValue;
                        FirePropertyChanged("Left");
                        //ApplicationCommands.Cut.Execute(null, Application.Current.MainWindow);
                    }
                }
            }
        }
        public string Top
        {
            get
            {
                if (double.IsNaN(_model.Top))
                    return null;
                return _model.Top.ToString();
            }
            set
            {
                double newValue;
                if (ValidateValue(value, CommonDefine.MaxEditorHeight, CommonDefine.MinWidgetLocation, out newValue))
                {
                    if (_model.Top != newValue)
                    {
                        _model.Top = newValue;
                        FirePropertyChanged("Top");

                    }
                }
            }
        }
        public string Width
        {
            get
            {
                if (double.IsNaN(_model.Width))
                    return null;
                return _model.Width.ToString();
            }
            set
            {
                double newValue;
                if (ValidateValue(value, CommonDefine.MaxEditorWidth, CommonDefine.MinWidgetSize, out newValue))
                {
                    if (_model.Width != newValue)
                    {
                        _model.Width = newValue;
                        FirePropertyChanged("Width");
                    }
                }
            }
        }
        public string Height
        {
            get
            {
                if (double.IsNaN(_model.Height))
                    return null;
                return _model.Height.ToString();
            }
            set
            {
                double newValue;
                if (ValidateValue(value, CommonDefine.MaxEditorHeight, CommonDefine.MinWidgetSize, out newValue))
                {
                    if (_model.Height != newValue)
                    {
                        _model.Height = newValue;
                        FirePropertyChanged("Height");
                    }
                }
            }
        }
        public string RotateAngle
        {
            get
            {
                if (_model.RotateAngle == int.MinValue)
                    return null;
                return _model.RotateAngle.ToString();
            }
            set
            {
                int newValue = AdjustAngleValue(value);
                if (_model.RotateAngle != newValue)
                {
                    _model.RotateAngle = newValue;
                    FirePropertyChanged("RotateAngle");
                }
            }
        }
        public string CornerRadius
        {
            get
            {
                if (_model.CornerRadius == int.MinValue)
                    return null;
                return _model.CornerRadius.ToString();
            }
            set
            {
                
                double validValue=0;
                if (ValidateValue(value, 10000, 0, out validValue))
                {
                    int newValue = Convert.ToInt32(validValue);
                    if (_model.CornerRadius != newValue)
                    {
                        _model.CornerRadius = newValue;
                        FirePropertyChanged("CornerRadius");
                    }
                }


            }
        }
        public string TextRotate
        {
            get
            {
                if (_model.TextRotate == int.MinValue)
                    return null;
                return _model.TextRotate.ToString();
            }
            set
            {
                int newValue = AdjustAngleValue(value);
                if (_model.TextRotate != newValue)
                {
                    _model.TextRotate = newValue;
                    FirePropertyChanged("TextRotate");
                }
            }
        }

        public bool? IsHidden
        {
            get { return _model.IsHidden; }
            set
            {
                if (_model.IsHidden != value)
                {
                    _model.IsHidden = value;
                    FirePropertyChanged("IsHidden");
                }
            }
        }

        public bool? IsFixed
        {
            get { return _model.IsFixed; }
            set
            {
                if (_model.IsFixed != value)
                {
                    _model.IsFixed = value;
                    FirePropertyChanged("IsFixed");
                }
            }
        }

        virtual public bool IsFixedEnabled
        {
            get { return _model.IsFixedEnabled; }
        }

        #endregion

        #region Enable Mark
        public bool canEdit = false;
        public bool CanEdit
        {
            get
            {
                return canEdit;
            }
            set
            {
                canEdit = value;
                FirePropertyChanged("CanEdit");
            }
        }
        public bool canRotateEdit = true;
        public bool CanRotatEdit
        {
            get
            {
                return canRotateEdit;
            }
            set
            {
                canRotateEdit = value;
                FirePropertyChanged("CanRotatEdit");
            }
        }
        public bool canCornerRaius = false;
        public bool CanCornerRaius
        {
            get
            {
                return canCornerRaius;
            }
            set
            {
                canCornerRaius = value;
                FirePropertyChanged("CanCornerRaius");
            }
        }
        public bool canEditImg = true;
        public bool CanEditImg
        {
            get
            {
                return canEditImg;
            }
            set
            {
                canEditImg = value;
                FirePropertyChanged("CanEditImg");
            }
        }
        public bool canTextEdit = true;
        public bool CanTextEdit
        {
            get
            {
                return canTextEdit;
            }
            set
            {
                canTextEdit = value;
                FirePropertyChanged("CanTextEdit");
            }
        }
        public bool canTooltipEdit = true;
        public bool CanTooltipEdit
        {
            get
            {
                return canTooltipEdit;
            }
            set
            {
                canTooltipEdit = value;
                FirePropertyChanged("CanTooltipEdit");
            }
        }
        public bool canTextRotateEdit = true;
        public bool CanTextRotateEdit
        {
            get
            {
                return canTextRotateEdit;
            }
            set
            {
                canTextRotateEdit = value;
                FirePropertyChanged("CanTextRotateEdit");
            }
        }
        #endregion

        #endregion

        #region Text Binding Propery


        public ICommand SelectFontCommand
        {
            get
            {
                //return EditingCommands.ToggleBold;
                   return FontCommands.Bold;
            }
        }

        private string selectedFont;
        public string SelectedFont
        {
            get
            {
                return selectedFont;
            }
            set
            {
                if (selectedFont != value)
                {
                    selectedFont = value;
                    FirePropertyChanged("SelectedFont");
                }
            }
        }



        #endregion

        #region Border Binding Propery

        #endregion

        #region Image Property
        public string Opacity
        {
            get
            {
                if (double.IsNaN(_model.Opacity))
                    return null;
                double opacity = _model.Opacity * 100;
                return opacity.ToString();
            }
            set
            {
                double newValue;
                if (ValidateValue(value, CommonDefine.MaxOpacity, CommonDefine.MinOpacity, out newValue))
                {
                    newValue = newValue / 100;
                    if (_model.Opacity != newValue)
                    {
                        _model.Opacity = newValue;
                        FirePropertyChanged("Opacity");
                    }
                }
            }
        }

        #endregion
    }
}
