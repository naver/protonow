using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using Naver.Compass.InfoStructure;

namespace MainToolBar.ViewModel
{
    public class ControlData : ViewModelBase
    {
        public string Label
        {
            get
            {
                return _label;
            }

            set
            {
                if (_label != value)
                {
                    _label = value;
                    FirePropertyChanged("Label");
                }
            }
        }
        private string _label;

        public bool IsEnable
        {
            get
            {
                return _IsEnable;
            }

            set
            {
                if (_IsEnable != value)
                {
                    _IsEnable = value;
                    FirePropertyChanged("IsEnable");
                }
            }
        }

        private bool _IsEnable;

        public Uri LargeImage
        {
            get
            {
                return _largeImage;
            }

            set
            {
                if (_largeImage != value)
                {
                    _largeImage = value;
                    FirePropertyChanged("LargeImage");
                }
            }
        }
        private Uri _largeImage;

        public Uri SmallImage
        {
            get
            {
                return _smallImage;
            }

            set
            {
                if (_smallImage != value)
                {
                    _smallImage = value;
                    FirePropertyChanged("SmallImage");
                }
            }
        }
        private Uri _smallImage;

        public string ToolTipTitle
        {
            get
            {
                return _toolTipTitle;
            }

            set
            {
                if (_toolTipTitle != value)
                {
                    _toolTipTitle = value;
                    FirePropertyChanged("ToolTipTitle");
                }
            }
        }
        private string _toolTipTitle;

        public string ToolTipDescription
        {
            get
            {
                return _toolTipDescription;
            }

            set
            {
                if (_toolTipDescription != value)
                {
                    _toolTipDescription = value;
                    FirePropertyChanged("ToolTipDescription");
                }
            }
        }
        private string _toolTipDescription;

        public Uri ToolTipImage
        {
            get
            {
                return _toolTipImage;
            }

            set
            {
                if (_toolTipImage != value)
                {
                    _toolTipImage = value;
                    FirePropertyChanged("ToolTipImage");
                }
            }
        }
        private Uri _toolTipImage;

        public string ToolTipFooterTitle
        {
            get
            {
                return _toolTipFooterTitle;
            }

            set
            {
                if (_toolTipFooterTitle != value)
                {
                    _toolTipFooterTitle = value;
                    FirePropertyChanged("ToolTipFooterTitle");
                }
            }
        }
        private string _toolTipFooterTitle;

        public string ToolTipFooterDescription
        {
            get
            {
                return _toolTipFooterDescription;
            }

            set
            {
                if (_toolTipFooterDescription != value)
                {
                    _toolTipFooterDescription = value;
                    FirePropertyChanged("ToolTipFooterDescription");
                }
            }
        }
        private string _toolTipFooterDescription;

        public Uri ToolTipFooterImage
        {
            get
            {
                return _toolTipFooterImage;
            }

            set
            {
                if (_toolTipFooterImage != value)
                {
                    _toolTipFooterImage = value;
                    FirePropertyChanged("ToolTipFooterImage");
                }
            }
        }
        private Uri _toolTipFooterImage;

        public ICommand Command
        {
            get
            {
                return _command;
            }

            set
            {
                if (_command != value)
                {
                    _command = value;
                    FirePropertyChanged("Command");
                }
            }
        }
        private  ICommand _command;

        public  object CommandParameter
        {
            get
            {
                return _commandparameter;
            }

            set
            {
                if (_commandparameter != value)
                {
                    _commandparameter = value;
                    FirePropertyChanged("CommandParameter");
                }
            }
        }
        private static object _commandparameter;

        public IInputElement CmdTarget
        {
            get
            {
                return _cmdtarget;
            }

            set
            {
                if (_cmdtarget != value)
                {
                    _cmdtarget = value;
                    FirePropertyChanged("CmdTarget");
                }
            }
        }

        private IInputElement _cmdtarget;

        public string KeyTip
        {
            get
            {
                return _keyTip;
            }

            set
            {
                if (_keyTip != value)
                {
                    _keyTip = value;
                    FirePropertyChanged("KeyTip");
                }
            }
        }
        private string _keyTip;
    }

    
}
