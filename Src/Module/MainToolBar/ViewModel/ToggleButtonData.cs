using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MainToolBar.ViewModel
{
    public class ToggleButtonData : ControlData
    {
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    FirePropertyChanged("IsChecked");
                }
            }
        }
        private bool _isChecked;
    }
}
