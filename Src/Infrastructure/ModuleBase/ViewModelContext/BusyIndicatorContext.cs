using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.InfoStructure
{
    public class BusyIndicatorContext : ViewModelBase
    {

        //Title
        private string _title = null;
        public string Content
        {
            get 
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    FirePropertyChanged("Content");
                }
            }
        }

        //Is Show
        private bool _bIsShow = false;
        public bool IsShow
        {
            get
            {
                return _bIsShow; 
            }
            set
            {
                if (_bIsShow != value)
                {
                    _bIsShow = value;
                    if (_bIsShow == false)
                    {
                        _bIsContinue = true;
                    }
                    FirePropertyChanged("IsShow");
                }
            }
        }

        //Progress Number
        private int _progress = 70;
        public int Progress
        {
            get
            {
                return _progress; 
            }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    FirePropertyChanged("Progress");
                }
            }
        }

        //Is Show
        private bool _bIsContinue = true;
        public bool IsContinue
        {
            get
            {
                return _bIsContinue;
            }
            set
            {
                if (_bIsContinue != value)
                {
                    _bIsContinue = value;
                    FirePropertyChanged("IsContinue");
                }
            }
        }

        //Can Stop
        private bool _bCanStop = true;
        public bool CanStop
        {
            get
            {
                return _bCanStop;
            }
            set
            {
                if (_bCanStop != value)
                {
                    _bCanStop = value;
                    ShowLoading = !_bCanStop;
                    FirePropertyChanged("CanStop");
                }
            }
        }

        private bool _bShowLoading = false;
        public bool ShowLoading
        {
            get
            {
                return _bShowLoading;
            }
            set
            {
                if (_bShowLoading != value)
                {
                    _bShowLoading = value;
                    FirePropertyChanged("ShowLoading");
                }
            }
        }

    }
}
