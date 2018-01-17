using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;
using MainToolBar.Common;
using System.Windows.Documents;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Naver.Compass.InfoStructure;
using System.Windows.Input;
using System.Windows.Data;
using Naver.Compass.Module;
using Naver.Compass.Common.Helper;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.MainToolBar.Module;
using System.Globalization;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;


namespace MainToolBar.ViewModel
{

    partial class RibbonViewModel
    {
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

        public bool CanLocationEdit
        {
            get
            {
                return _model.CanLocationEdit;
            }
        }

        public bool IsLockRatio
        {
            get
            {
                return GlobalData.IsLockRatio;
            }
            set
            {
                GlobalData.IsLockRatio = value;
            }
        }
        #endregion

        #region Validation Base
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


    }
}

