
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

namespace Naver.Compass.Module.Property
{
    class TextAreaPropertyViewModel : PropertyViewModelBase
    {

        public TextAreaPropertyViewModel()
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
                IEnumerable<TextAreaWidgetViewModel> AllTextAreas = _VMItems.OfType<TextAreaWidgetViewModel>();
                if (AllTextAreas.Count() < 1)
                {
                    return null;
                }

                string res = AllTextAreas.First().HintText;
                foreach (TextAreaWidgetViewModel item in AllTextAreas)
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
                IEnumerable<TextAreaWidgetViewModel> AllTextAreas = _VMItems.OfType<TextAreaWidgetViewModel>();
                if (AllTextAreas.Count() < 1)
                {
                    return null;
                }

                bool res = AllTextAreas.First().IsDisabled;
                foreach (TextAreaWidgetViewModel item in AllTextAreas)
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
                IEnumerable<TextAreaWidgetViewModel> AllTextAreas = _VMItems.OfType<TextAreaWidgetViewModel>();
                if (AllTextAreas.Count() < 1)
                {
                    return null;
                }

                bool res = AllTextAreas.First().IsHideBorder;
                foreach (TextAreaWidgetViewModel item in AllTextAreas)
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
                IEnumerable<TextAreaWidgetViewModel> AllTextAreas = _VMItems.OfType<TextAreaWidgetViewModel>();
                if (AllTextAreas.Count() < 1)
                {
                    return null;
                }

                bool res = AllTextAreas.First().IsReadOnly;
                foreach (TextAreaWidgetViewModel item in AllTextAreas)
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
        #endregion Binding line Property
    }
}
