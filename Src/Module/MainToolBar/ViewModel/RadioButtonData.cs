using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;

namespace MainToolBar.ViewModel
{
    public class RadioButtonData : ControlData
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

    public class RibbonBarData:ControlData
    {

        private Brush _BackGround;

        public Brush BackGround
        {
            get
            {
                return _BackGround;
            }

            set
            {
                if (_BackGround != value)
                {
                    _BackGround = value;
                    FirePropertyChanged("BackGround");
                }
            }
        }


        private Brush _BorderBrush;

        public Brush BorderBrush
        {
            get
            {
                return _BorderBrush;
            }

            set
            {
                if (_BorderBrush != value)
                {
                    _BorderBrush = value;
                    FirePropertyChanged("BorderBrush");
                }
            }
        }

    }
}
