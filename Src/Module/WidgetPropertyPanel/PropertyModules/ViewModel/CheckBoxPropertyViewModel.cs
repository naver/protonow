
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Module.Property
{
    class CheckBoxPropertyViewModel : PropertyViewModelBase
    {

        public CheckBoxPropertyViewModel()
        {

        }

        #region Override base function
        protected override void OnItemsAdd()
        {
            base.OnItemsAdd();
            FirePropertyChanged("IsShowSelect");
            FirePropertyChanged("IsBtnAlignLeft");
            FirePropertyChanged("IsDisabled");
        }
        override public void OnPropertyChanged(string args)
        {
            base.OnPropertyChanged(args);
            switch (args)
            {
                case "IsShowSelect":
                    {
                        FirePropertyChanged(args);
                        break;
                    }
                case "IsBtnAlignLeft":
                    {
                        FirePropertyChanged(args);
                        break;
                    }
                case "IsDisabled":
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
        public bool? IsDisabled
        {
            get
            {
                IEnumerable<CheckBoxWidgetViewModel> AllCheckBoxs = _VMItems.OfType<CheckBoxWidgetViewModel>();
                if (AllCheckBoxs.Count() < 1)
                {
                    return null;
                }

                bool res = AllCheckBoxs.First().IsDisabled;
                foreach (CheckBoxWidgetViewModel item in AllCheckBoxs)
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
                IEnumerable<CheckBoxWidgetViewModel> AllCheckBoxs = _VMItems.OfType<CheckBoxWidgetViewModel>();
                if (AllCheckBoxs.Count() < 1)
                {
                    return null;
                }
                
                bool res = AllCheckBoxs.First().IsShowSelect;
                foreach (CheckBoxWidgetViewModel item in AllCheckBoxs)
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
                IEnumerable<CheckBoxWidgetViewModel> AllCheckBoxs = _VMItems.OfType<CheckBoxWidgetViewModel>();
                if (AllCheckBoxs.Count() < 1)
                {
                    return true;                   
                }
                bool res = AllCheckBoxs.First().IsBtnAlignLeft;
                foreach (CheckBoxWidgetViewModel item in AllCheckBoxs)
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
