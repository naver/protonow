
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
    class ButtonPropertyViewModel : PropertyViewModelBase
    {

        public ButtonPropertyViewModel()
        {

        }

        #region Override base function
        protected override void OnItemsAdd()
        {
            base.OnItemsAdd();
            FirePropertyChanged("IsDisabled");
        }
        override public void OnPropertyChanged(string args)
        {
            base.OnPropertyChanged(args);
            switch (args)
            {
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
                IEnumerable<ButtonWidgetViewModel> AllButtons = _VMItems.OfType<ButtonWidgetViewModel>();
                if (AllButtons.Count() < 1)
                {
                    return null;
                }

                bool res = AllButtons.First().IsDisabled;
                foreach (ButtonWidgetViewModel item in AllButtons)
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
        #endregion Binding line Property
    }
}
