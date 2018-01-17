using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MainToolBar.ViewModel
{
    public class ApplicationMenuData : ControlData
    {
        public ApplicationMenuData()
        {
        }

        public bool IsDropDown
        {
            set 
            {
                if (_IsDropDown != value)
                {
                    _IsDropDown = value;
                    FirePropertyChanged("IsDropDown");
                }
            }
            get
            {
                return _IsDropDown;
            }
            
        }

        private bool _IsDropDown;
    }
}
