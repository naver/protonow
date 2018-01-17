using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.Helper;

namespace Naver.Compass.Module.Property
{
    public class PropertyViewModelBase : ViewModelBase
    {
        public PropertyViewModelBase()
        {
            _VMItems = new List<WidgetViewModBase>();
        }

        #region private member
        protected List<WidgetViewModBase> _VMItems;
        protected static IInputElement _CmdTarget;
        #endregion

        #region Public function

        public static void UpdateCmdTarget(IInputElement cmdTarget)
        {
            if (cmdTarget != null)
            {
                _CmdTarget = cmdTarget;
            }
        }

        public void AddItems(WidgetViewModBase data)
        {
            _VMItems.Add(data);
            OnItemsAdd();
        }

        virtual public void OnPropertyChanged(string args)
        {
            switch (args)
            {
                case "Tooltip":
                    {
                        FirePropertyChanged("Tooltip");
                    }
                    break;
                default:
                    break;

            }
        }

        #endregion

        #region Base binding property
        public string Tooltip
        {
            get
            {
                string sReturn = null;
                foreach (WidgetViewModBase wdg in _VMItems)
                {
                    if (sReturn == null)
                    {
                        sReturn = wdg.Tooltip;
                    }
                    else if (!sReturn.Equals(wdg.Tooltip))
                    {
                        sReturn = "";
                        break;
                    }

                }

                return sReturn;
            }
            set
            {
                foreach (WidgetViewModBase wdg in _VMItems)
                {
                    if (wdg.Tooltip != value)
                    {
                        wdg.Tooltip = value;
                    }
                }

            }
        }

        public IInputElement CmdTarget
        {
            get
            {
                return _CmdTarget;
            }

        }

        public string HeaderString
        {
            get
            {
                return "(" + Convert.ToString(_VMItems.Count) + ")";
            }
        }
        #endregion

        #region  Private function

        virtual protected void OnItemsAdd()
        {
            FirePropertyChanged("Tooltip");
            FirePropertyChanged("HeaderString");
        }

        #endregion

    }
}
