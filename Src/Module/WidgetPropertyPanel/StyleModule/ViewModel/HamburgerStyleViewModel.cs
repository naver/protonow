using Microsoft.Practices.Prism.Events;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module.Styles
{
    class HamburgerStyleViewModel:ViewModelBase
    {
        public HamburgerStyleViewModel()
        {
            if (this.IsInDesignMode)
                return;
            _model = PropertyPageModel.GetInstance();
            _ListEventAggregator.GetEvent<SelectionChangeEvent>().Subscribe(SelectionChangeEventHandler);
            _propertyChangeToken =_ListEventAggregator.GetEvent<SelectionPropertyChangeEvent>().Subscribe(SelectionPropertyEventHandler);
            _ListEventAggregator.GetEvent<EnableFirePositionInRibbonEvent>().Subscribe(EnableFirePositionHandler);
        }

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

        public string MenuPageLeft
        {
            get
            {
                if (double.IsNaN(_model.MenuPageLeft))
                    return null;
                return _model.MenuPageLeft.ToString();
            }
            set
            {
                double newValue;
                if (ValidateValue(value, CommonDefine.MaxEditorWidth, CommonDefine.MinWidgetLocation, out newValue))
                {
                    if (_model.MenuPageLeft != newValue)
                    {
                        _model.MenuPageLeft = newValue;
                        FirePropertyChanged("MenuPageLeft");
                        //ApplicationCommands.Cut.Execute(null, Application.Current.MainWindow);
                    }
                }
            }
        }
        public string MenuPageTop
        {
            get
            {
                if (double.IsNaN(_model.MenuPageTop))
                    return null;
                return _model.MenuPageTop.ToString();
            }
            set
            {
                double newValue;
                if (ValidateValue(value, CommonDefine.MaxEditorHeight, CommonDefine.MinWidgetLocation, out newValue))
                {
                    if (_model.MenuPageTop != newValue)
                    {
                        _model.MenuPageTop = newValue;
                        FirePropertyChanged("MenuPageTop");

                    }
                }
            }
        }
        public string MenuPageWidth
        {
            get
            {
                if (double.IsNaN(_model.MenuPageWidth))
                    return null;
                return _model.MenuPageWidth.ToString();
            }
            set
            {
                double newValue;
                if (ValidateValue(value, CommonDefine.MaxEditorWidth, CommonDefine.MinWidgetSize, out newValue))
                {
                    if (_model.MenuPageWidth != newValue)
                    {
                        _model.MenuPageWidth = newValue;
                        FirePropertyChanged("MenuPageWidth");
                    }
                }
            }
        }
        public string MenuPageHeight
        {
            get
            {
                if (double.IsNaN(_model.MenuPageHeight))
                    return null;
                return _model.MenuPageHeight.ToString();
            }
            set
            {
                double newValue;
                if (ValidateValue(value, CommonDefine.MaxEditorHeight, CommonDefine.MinWidgetSize, out newValue))
                {
                    if (_model.MenuPageHeight != newValue)
                    {
                        _model.MenuPageHeight = newValue;
                        FirePropertyChanged("MenuPageHeight");
                    }
                }
            }
        }

        public bool? IsMenuPageHidden
        {
            get { return _model.IsMenuPageHidden; }
            set
            {
                if (_model.IsMenuPageHidden != value)
                {
                    _model.IsMenuPageHidden = value;
                    FirePropertyChanged("IsMenuPageHidden");
                }
            }
        }

        #endregion

        private void EnableFirePositionHandler(bool enable)
        {
            _isFirePositionEnabled = enable;
        }
        private void SelectionPropertyEventHandler(string EventArg)
        {
            if (_model.IsHamburgerWidget())
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
                    case "MenuPageLeft":
                        {
                            FirePropertyChanged("MenuPageLeft");
                            break;
                        }
                    case "MenuPageTop":
                        {
                            FirePropertyChanged("MenuPageTop");
                            break;
                        }
                    case "MenuPageWidth":
                        {
                            FirePropertyChanged("MenuPageWidth");
                            break;
                        }
                    case "MenuPageHeight":
                        {
                            FirePropertyChanged("MenuPageHeight");
                            break;
                        }
                    case "IsMenuPageHidden":
                        {
                            FirePropertyChanged("IsMenuPageHidden");
                            break;
                        }
                }
            }
        }
        private void SelectionChangeEventHandler(string EventArg)
        {
            if(_model.IsHamburgerWidget())
            {
                _propertyChangeToken = _ListEventAggregator.GetEvent<SelectionPropertyChangeEvent>().Subscribe(SelectionPropertyEventHandler);
                FirePropertyChanged("Top");
                FirePropertyChanged("Left");
                FirePropertyChanged("Width");
                FirePropertyChanged("Height");
                FirePropertyChanged("IsHidden");
                FirePropertyChanged("IsFixed");
                FirePropertyChanged("MenuPageLeft");
                FirePropertyChanged("MenuPageTop");
                FirePropertyChanged("MenuPageWidth");
                FirePropertyChanged("MenuPageHeight");
                FirePropertyChanged("IsMenuPageHidden");
            }
            else
            {
                //left/top change will cost a lot time when move widget
                //so unsubscribe selection property change event if no hamburger selected.
                _ListEventAggregator.GetEvent<SelectionPropertyChangeEvent>().Unsubscribe(_propertyChangeToken);
            }

        }

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

        private PropertyPageModel _model;
        private bool _isFirePositionEnabled = true;
        private SubscriptionToken _propertyChangeToken;

    }
}
