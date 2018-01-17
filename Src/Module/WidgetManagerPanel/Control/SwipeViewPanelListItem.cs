using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.ComponentModel;

namespace Naver.Compass.Module
{
    class SwipeViewPanelListItem : WidgetListItem
    {
        private IPanelStatePage _pannelData;

        public SwipeViewPanelListItem(IPanelStatePage data)
        {
            System.Diagnostics.Debug.Assert(data != null);
            _pannelData = data;
            _Mode = null;
        }

        override public Guid WidgetID
        {
            get
            {
                return _pannelData.Guid;
            }
        }

        override public int zOrder
        {
            get
            {
                return 0;
            }
        }

        override public string WidgetName
        {
            get
            {
                return _pannelData.Name;
            }
        }

        override public string WidgetTypeName
        {
            get
            {
                return "";
            }
        }

        override public bool PlaceFlag
        {
            get
            {
                return false;
            }
        }

        private bool _lostFlag;
        override public bool LostFlag
        {
            get
            {
                return false;
            }
        }

        private bool _hide_Flage = false;
        override public bool HideFlag
        {
            get
            {
                return _hide_Flage;
            }
            set
            {
                if (!_hide_Flage.Equals(value))
                {
                    _hide_Flage = (bool)value;
                    FirePropertyChanged("HideFlag");
                }
            }
        }

        override public bool InteractiveFlag
        {
            get { return false; }
        }

        override public bool DisplayHeadIconFlag
        {
            get { return true; }
        }

        override public void UpdateAllFlagByViewChange()
        {
            return;
        }
    }
}
