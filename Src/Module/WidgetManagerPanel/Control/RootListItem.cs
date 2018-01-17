using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.ComponentModel;

namespace Naver.Compass.Module
{
    class RootListItem : WidgetListItem
    {
        private IPage _Page = null;
        public RootListItem()
        {
            _Mode = null;
        }
        private Guid _guid = Guid.Empty;
        override public Guid WidgetID
        {
            get
            {
                return _guid;
            }
            set
            {
                if (!_guid.Equals(value))
                {
                    _guid = (Guid)value;

                    FirePropertyChanged("WidgetID");
                }
            }
        }

        private string _Name = "";
        override public string WidgetName
        {
            get
            {
                return _Name;
            }
            set
            {
                if (!_Name.Equals(value))
                {
                    _Name = (string)value;

                    FirePropertyChanged("WidgetName");
                }
            }
        }

        override public string WidgetTypeName
        {
            get
            {
                return "";
            }
        }
    }
}
