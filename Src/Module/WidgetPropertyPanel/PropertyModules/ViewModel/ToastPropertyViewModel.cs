using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.Helper;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Module.Property
{
    class ToastPropertyViewModel  : PropertyViewModelBase
    {
        public ToastPropertyViewModel()
        {
            _ListEventAggregator.GetEvent<SelectionPropertyChangeEvent>().Subscribe(SelectionPropertyEventHandler);

        }

        #region Binding property
        public string ExposureTime
        {
            get
            {
                int exposureTime = (_VMItems[0] as ToastViewModel).ExposureTime;
                foreach(ToastViewModel wdg in _VMItems)
                {
                    if(wdg!=null)
                    {
                        if(wdg.ExposureTime!=exposureTime)
                        {
                            return null;
                        }
                    }
                }
                return exposureTime.ToString();
            }
            set
            {
                if (value != _exposureTime && (int.Parse(value)>=0))
                {
                    _exposureTime = value.ToString();
                    ToastCommands.ExposureTime.Execute(int.Parse(value), _CmdTarget);
                    FirePropertyChanged("ExposureTime");
                }
            }
        }

        public ToastCloseSetting CloseSetting
        {
            get
            {
                ToastCloseSetting closeSetting = (_VMItems[0] as ToastViewModel).CloseSetting;
                foreach (ToastViewModel wdg in _VMItems)
                {
                    if (wdg != null)
                    {
                        if (wdg.CloseSetting != closeSetting)
                        {
                            return ToastCloseSetting.ExposureTime;
                        }
                    }
                }
                return closeSetting;
            }
            set
            {
                ToastCommands.CloseSetting.Execute(value, _CmdTarget);

                FirePropertyChanged("CloseSetting");
            }
        }

        public ToastDisplayPosition DisplayPosition
        {
            get
            {
                ToastDisplayPosition dispalyPosition = (_VMItems[0] as ToastViewModel).DisplayPosition;
                foreach (ToastViewModel wdg in _VMItems)
                {
                    if (wdg != null)
                    {
                        if (wdg.DisplayPosition != dispalyPosition)
                        {
                            return ToastDisplayPosition.Top;
                        }
                    }
                }
                return dispalyPosition;
            }
            set
            {
                ToastCommands.DisplayPosition.Execute(value, _CmdTarget);

                FirePropertyChanged("DisplayPosition");
            }
        }
        #endregion

        private void SelectionPropertyEventHandler(string EventArg)
        {
            switch (EventArg)
            {
                case "DisplayPosition":
                    {
                        FirePropertyChanged("DisplayPosition");
                        break;
                    }
            }

        }

       #region private member
       //keep exposure time in memory
       static private string _exposureTime;
       private ToastCloseSetting _closeSeting;
       private ToastDisplayPosition _displayPosition;
       #endregion

    }
}
