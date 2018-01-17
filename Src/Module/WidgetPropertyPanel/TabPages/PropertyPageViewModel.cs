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

namespace Naver.Compass.Module
{
    class PropertyPageViewModel : ViewModelBase
    {
        #region Constructor
        public PropertyPageViewModel()
        {
            if (this.IsInDesignMode)
                return;
            _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>(); ;
            _ListEventAggregator.GetEvent<SelectionChangeEvent>().Subscribe(SelectionChangeEventHandler);
            _ListEventAggregator.GetEvent<SelectionPropertyChangeEvent>().Subscribe(SelectionPropertyEventHandler);
            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeEventHandler);

            _model = new PropertyPageModel();
            this.ImportImgCommand = new DelegateCommand<object>(ImportImgCommandHandler);
            this.ClearImgCommand = new DelegateCommand<object>(ClearImgCommandHandler);

            fontSizeList = new List<int>();
            fontSizeList.Add(8);
            fontSizeList.Add(9);
            fontSizeList.Add(10);
            fontSizeList.Add(11);
            fontSizeList.Add(12);
            fontSizeList.Add(14);
            fontSizeList.Add(16);
            fontSizeList.Add(18);
            fontSizeList.Add(20);
            fontSizeList.Add(22);
            fontSizeList.Add(24);
            fontSizeList.Add(28);
            fontSizeList.Add(36);
            fontSizeList.Add(48);
            fontSizeList.Add(72);

        }
        #endregion Constructor

        #region private Function and Member
        //protected ISelectionService _selectionService;

        private bool ValidateValue(string value, double max, double min, out double newValue)
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

        #endregion

        #region Data Change Trigger Handler
        PropertyPageModel _model;
        private void SelectionPropertyEventHandler(string EventArg)
        {
            switch(EventArg)
            {
                case "Top":
                    {
                        FirePropertyChanged("Top");
                        break;
                    }
                case "Left":
                    {
                        FirePropertyChanged("Left");
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
                case "vTextContent":
                    {
                        FirePropertyChanged("Content");
                        break;
                    }
                case "RotateAngle":
                    {
                        FirePropertyChanged("RotateAngle");
                        break;
                    }
                //case "Name":
                //    {
                //        FirePropertyChanged("Name");
                //        break;
                //    }

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
                case "ImgSource":
                    {
                        FirePropertyChanged("ImgSource");
                        break;
                    }
                case "Tooltip":
                    {
                        FirePropertyChanged("Tooltip");
                        break;
                    }
                default:
                    return;
            }
 


        }
        private void SelectionPageChangeEventHandler(Guid EventArg)
        {
            FirePropertyChanged("Top");
            FirePropertyChanged("Left");
            FirePropertyChanged("Width");
            FirePropertyChanged("Height");     
            FirePropertyChanged("Content");
            FirePropertyChanged("RotateAngle"); 
           // FirePropertyChanged("Name");
           // FirePropertyChanged("Type");
            FirePropertyChanged("Opacity");
            FirePropertyChanged("IsHidden");
            FirePropertyChanged("ImgSource");
            FirePropertyChanged("Tooltip");

            ISelectionService _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
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
        }
        private void SelectionChangeEventHandler(string EventArg)
        {
            FirePropertyChanged("Top");
            FirePropertyChanged("Left");
            FirePropertyChanged("Width");
            FirePropertyChanged("Height");
            FirePropertyChanged("Content");
            FirePropertyChanged("RotateAngle");
           // FirePropertyChanged("Name");
           // FirePropertyChanged("Type");
            FirePropertyChanged("Opacity");
            FirePropertyChanged("IsHidden");
            FirePropertyChanged("ImgSource");
            FirePropertyChanged("Tooltip");
            
            if (_model.GetWidgetsNumber() <= 0)
            {
                CanEdit = false;
                return;
            }
            else
            {
                CanEdit = true;
            }

            if (_model.IsCanRotate())
            {
                CanRotatEdit = true;
            }
            else
            {
                CanRotatEdit = false;
            }

            if (_model.IsCanEditImage())
            {
                CanEditImg = true;
            }
            else
            {
                CanEditImg = false;
            }          

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
                int newValue;
                if (ValidateValue(value, Int32.MaxValue, Int32.MinValue, out newValue))
                {
                    if (_model.RotateAngle != newValue)
                    {
                        _model.RotateAngle = newValue;
                        FirePropertyChanged("RotateAngle");
                    }
                }
            }
        }
        public string Content
        {
            get { return _model.Content; }
            set
            {
                if (_model.Content != value)
                {
                    _model.Content = value;
                    FirePropertyChanged("Content");
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
        public string Tooltip
        {
            get { return _model.Tooltip; }
            set
            {
                if (_model.Tooltip != value)
                {
                    _model.Tooltip = value;
                    FirePropertyChanged("Tooltip");
                }
            }
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
                canEdit=value;
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
        #endregion

        #endregion

        #region Text Binding Propery


        public ICommand SelectFontCommand
        {
            get
            {
                return EditingCommands.ToggleBold;
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
                if(selectedFont!=value)
                {
                    selectedFont = value;
                    FirePropertyChanged("SelectedFont");
                }
            }
        }

        private List<int> fontSizeList;
        public List<int> FontSizeList
        {
            get
            {
                return fontSizeList;
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
        public ImageSource ImgSource
        {
            get
            {
                return _model.ImgSource;
            }
        }
        #endregion

        #region Command Handler
        public DelegateCommand<object> ImportImgCommand { get; private set; }
        public DelegateCommand<object> ClearImgCommand { get; private set; }
        private void ImportImgCommandHandler(object cmdParameter)
        {
            _model.ImportImg();
        }     
        private void ClearImgCommandHandler(object cmdParameter)
        {
            _model.ClearImg();
        }

        #endregion

    }
}
