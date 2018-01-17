
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
    class RadioButtonPropertyViewModel : PropertyViewModelBase
    {

        public RadioButtonPropertyViewModel()
        {
        }

        private void SetGroupNameCombox()
        {
            RadioGroups.Clear();
            IEnumerable<RadioButtonWidgetViewModel> radios = _VMItems.OfType<RadioButtonWidgetViewModel>();
            foreach (RadioButtonWidgetViewModel item in radios)
            {
                string name=item.RadioGroup;
                if(string.IsNullOrEmpty(name)||RadioGroups.Contains(name))
                {
                    continue;
                }
                RadioGroups.Add(name);
            }
        }
        #region Override base function
        protected override void OnItemsAdd()
        {
            base.OnItemsAdd();
            SetGroupNameCombox();
            FirePropertyChanged("IsShowSelect");
            FirePropertyChanged("IsBtnAlignLeft");
            FirePropertyChanged("IsDisabled");
            FirePropertyChanged("RadioGroup");

        }
        override public void OnPropertyChanged(string args)
        {
            base.OnPropertyChanged(args);
            switch (args)
            {  
                case "IsShowSelect":
                case "IsBtnAlignLeft":
                case "IsDisabled":
                    {
                        FirePropertyChanged(args);
                        break;
                    }
                case "RadioGroup":
                    {
                        SetGroupNameCombox();
                        FirePropertyChanged(args);
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region temporary binding property
        public ObservableCollection<string> _radioGroups;
        public ObservableCollection<string> RadioGroups
        {
            get
            {
                if (_radioGroups == null)
                {
                    _radioGroups = new ObservableCollection<string>();
                }
                return _radioGroups;
            }

        }
        #endregion

        #region Binding line Property
        public string RadioGroup
        {
            get
            {
                IEnumerable<RadioButtonWidgetViewModel> radios = _VMItems.OfType<RadioButtonWidgetViewModel>();
                if (radios.Count() < 1)
                {
                    return null;
                }

                string res = radios.First().RadioGroup;
                foreach (RadioButtonWidgetViewModel item in radios)
                {
                    if (res != item.RadioGroup)
                    {
                        return null;
                    }
                }
                return res;
            }
            set
            {
                if (RadioGroup == value)
                    return;

                List<object> param = new List<object>();                   
                param.Insert(0, value);
                param.Insert(1, _VMItems);
                WidgetPropertyCommands.RadioGroup.Execute(param, CmdTarget);
                FirePropertyChanged("RadioGroup");
            }
        }
        public bool? IsDisabled
        {
            get
            {
                IEnumerable<RadioButtonWidgetViewModel> AllCheckBoxs = _VMItems.OfType<RadioButtonWidgetViewModel>();
                if (AllCheckBoxs.Count() < 1)
                {
                    return null;
                }

                bool res = AllCheckBoxs.First().IsDisabled;
                foreach (RadioButtonWidgetViewModel item in AllCheckBoxs)
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
                //foreach (CheckBoxWidgetViewModel item in _VMItems.OfType<CheckBoxWidgetViewModel>())
                //{
                //    if (value == true)
                //    {
                //        item.IsShowSelect = true;
                //    }
                //    else if (value == false)
                //    {
                //        item.IsShowSelect = false;
                //    }
                //}
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
        public bool? IsShowSelect
        {
            get
            {
                IEnumerable<RadioButtonWidgetViewModel> AllCheckBoxs = _VMItems.OfType<RadioButtonWidgetViewModel>();
                if (AllCheckBoxs.Count() < 1)
                {
                    return null;
                }

                bool res = AllCheckBoxs.First().IsShowSelect;
                foreach (RadioButtonWidgetViewModel item in AllCheckBoxs)
                {
                    if (res != item.IsShowSelect)
                    {
                        return null;
                    }
                }
                return res;

            }
            set
            {
                //foreach (CheckBoxWidgetViewModel item in _VMItems.OfType<CheckBoxWidgetViewModel>())
                //{
                //    if (value == true)
                //    {
                //        item.IsShowSelect = true;
                //    }
                //    else if (value == false)
                //    {
                //        item.IsShowSelect = false;
                //    }
                //}
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
                WidgetPropertyCommands.ShowSelect.Execute(param, CmdTarget);
                FirePropertyChanged("IsShowSelect");

            }
        }
        public bool IsBtnAlignLeft
        {
            get
            {
                IEnumerable<RadioButtonWidgetViewModel> AllCheckBoxs = _VMItems.OfType<RadioButtonWidgetViewModel>();
                if (AllCheckBoxs.Count() < 1)
                {
                    return true;
                }
                bool res = AllCheckBoxs.First().IsBtnAlignLeft;
                foreach (RadioButtonWidgetViewModel item in AllCheckBoxs)
                {
                    if (res != item.IsBtnAlignLeft)
                    {
                        return true;
                    }
                }
                return res;
            }
            set
            {
                //foreach (CheckBoxWidgetViewModel item in _VMItems.OfType<CheckBoxWidgetViewModel>())
                //{
                //    if (value == true)
                //    {
                //        item.IsBtnAlignLeft = true;
                //    }  
                //    else if(value == false)
                //    {
                //        item.IsBtnAlignLeft = false;
                //    }  
                //}
                List<object> param = new List<object>();
                param.Insert(0, value);
                param.Insert(1, _VMItems);
                WidgetPropertyCommands.ButtonAlign.Execute(param, CmdTarget);
                //FirePropertyChanged("IsBtnAlignLeft");

            }
        }
        #endregion Binding line Property
    }
}
