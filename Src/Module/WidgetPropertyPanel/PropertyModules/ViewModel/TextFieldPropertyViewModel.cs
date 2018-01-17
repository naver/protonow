
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Naver.Compass.Common.CommonBase;
using System.Collections.ObjectModel;
using Naver.Compass.Common.Helper;

namespace Naver.Compass.Module.Property
{
    class TextFieldPropertyViewModel : PropertyViewModelBase
    {

        public TextFieldPropertyViewModel()
        {
        }

        #region Override base function
        protected override void OnItemsAdd()
        {
            base.OnItemsAdd();
            FirePropertyChanged("HintText");
            FirePropertyChanged("IsHideBorder");
            FirePropertyChanged("IsDisabled");
            FirePropertyChanged("IsReadOnly");
            FirePropertyChanged("TextFieldType");
            FirePropertyChanged("MaxTextLength");
        }
        override public void OnPropertyChanged(string args)
        {
            base.OnPropertyChanged(args);
            switch (args)
            {
                case "HintText":
                case "IsHideBorder":
                case "IsDisabled":
                case "IsReadOnly":
                case "TextFieldType":
                case "MaxTextLength":
                    {
                        FirePropertyChanged(args);
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region Binding line Property
        public string HintText
        {
            get
            {
                IEnumerable<TextFieldWidgetViewModel> AllTextFields = _VMItems.OfType<TextFieldWidgetViewModel>();
                if (AllTextFields.Count() < 1)
                {
                    return null;
                }

                string res = AllTextFields.First().HintText;
                foreach (TextFieldWidgetViewModel item in AllTextFields)
                {
                    if (res != item.HintText)
                    {
                        return null;
                    }
                }
                return res;
            }
            set
            {
                if (HintText == value)
                    return;

                List<object> param = new List<object>();
                param.Insert(0, value);
                param.Insert(1, _VMItems);
                WidgetPropertyCommands.HintText.Execute(param, CmdTarget);
                FirePropertyChanged("HintText");
            }
        }
        public bool? IsDisabled
        {
            get
            {
                IEnumerable<TextFieldWidgetViewModel> AllTextFields = _VMItems.OfType<TextFieldWidgetViewModel>();
                if (AllTextFields.Count() < 1)
                {
                    return null;
                }

                bool res = AllTextFields.First().IsDisabled;
                foreach (TextFieldWidgetViewModel item in AllTextFields)
                {
                    if (res != item.IsDisabled)
                    {
                        return null;
                    }
                }
                return res;

            }
            set
            {
                List<object> param = new List<object>();
                if (value == null)
                {
                    param.Insert(0, false);
                }
                else
                {
                    param.Insert(0, value);
                }

                param.Insert(1, _VMItems);
                WidgetPropertyCommands.Enable.Execute(param, CmdTarget);
                FirePropertyChanged("IsDisabled");

            }
        }
        public bool? IsHideBorder
        {
            get
            {
                IEnumerable<TextFieldWidgetViewModel> AllTextFields = _VMItems.OfType<TextFieldWidgetViewModel>();
                if (AllTextFields.Count() < 1)
                {
                    return null;
                }

                bool res = AllTextFields.First().IsHideBorder;
                foreach (TextFieldWidgetViewModel item in AllTextFields)
                {
                    if (res != item.IsHideBorder)
                    {
                        return null;
                    }
                }
                return res;

            }
            set
            {
                List<object> param = new List<object>();
                if (value == null)
                {
                    param.Insert(0, false);
                }
                else
                {
                    param.Insert(0, value);
                }

                param.Insert(1, _VMItems);
                WidgetPropertyCommands.HideBorder.Execute(param, CmdTarget);
                FirePropertyChanged("IsHideBorder");

            }
        }
        public bool? IsReadOnly
        {
            get
            {
                IEnumerable<TextFieldWidgetViewModel> AllTextFields = _VMItems.OfType<TextFieldWidgetViewModel>();
                if (AllTextFields.Count() < 1)
                {
                    return null;
                }

                bool res = AllTextFields.First().IsReadOnly;
                foreach (TextFieldWidgetViewModel item in AllTextFields)
                {
                    if (res != item.IsReadOnly)
                    {
                        return null;
                    }
                }
                return res;

            }
            set
            {
                List<object> param = new List<object>();
                if (value == null)
                {
                    param.Insert(0, false);
                }
                else
                {
                    param.Insert(0, value);
                }

                param.Insert(1, _VMItems);
                WidgetPropertyCommands.ReadOnly.Execute(param, CmdTarget);
                FirePropertyChanged("IsReadOnly");

            }
        }
        //public int MaxTextLength
        public string MaxTextLength
        {
            get
            {
                IEnumerable<TextFieldWidgetViewModel> AllTextFields = _VMItems.OfType<TextFieldWidgetViewModel>();
                if (AllTextFields.Count() < 1)
                {
                    return null;
                }

                int res = AllTextFields.First().MaxTextLength;
                foreach (TextFieldWidgetViewModel item in AllTextFields)
                {
                    if (res != item.MaxTextLength)
                    {
                        return null;
                    }
                }
                return res.ToString();
                //return Math.Round(val);
            }
            set
            {
                int newValue;
                if (ValidateValue(value, 100000, 1, out newValue))
                {
                    List<object> param = new List<object>();
                    param.Insert(0, newValue);
                    param.Insert(1, _VMItems);
                    WidgetPropertyCommands.MaxLength.Execute(param, CmdTarget);
                    //FirePropertyChanged("TextFieldType");
                }                
            }
        }
        public TextFieldType TextFieldType
        {
            get
            {
                IEnumerable<TextFieldWidgetViewModel> AllTextFields = _VMItems.OfType<TextFieldWidgetViewModel>();
                if (AllTextFields.Count() < 1)
                {
                    return TextFieldType.Null;
                }

                TextFieldType res = AllTextFields.First().TextFieldType;
                foreach (TextFieldWidgetViewModel item in AllTextFields)
                {
                    if (res != item.TextFieldType)
                    {
                        return TextFieldType.Null;
                    }
                }
                return res;
            }
            set
            {
                List<object> param = new List<object>();
                if (value == TextFieldType.Null)
                {
                    return;
                }

                param.Insert(0, value);
                param.Insert(1, _VMItems);
                WidgetPropertyCommands.TextFieldType.Execute(param, CmdTarget);
                //FirePropertyChanged("TextFieldType");
            }
        }
        #endregion Binding line Property

        #region Scale Setting
        public Dictionary<TextFieldType, string> TextTypeDic
        {
            get
            {
                return new Dictionary<TextFieldType, string>()
                {
                    {TextFieldType.Text,"Text"},
                    {TextFieldType.Password,"Password"},
                    {TextFieldType.Email,"Email"},
                    {TextFieldType.Number,"Number"},
                    {TextFieldType.Tel,"Tel"},
                    {TextFieldType.Url,"Url"},
                    {TextFieldType.Search,"Search"},
                    {TextFieldType.File,"File"},
                    {TextFieldType.Month,"Month"},
                    {TextFieldType.Time,"Time"}
                };
            }
        }
        #endregion

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
    }
}
