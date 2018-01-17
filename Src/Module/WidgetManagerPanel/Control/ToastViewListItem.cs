using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.ComponentModel;

namespace Naver.Compass.Module
{
    class ToastViewListItem : WidgetListItem
    {
        public ToastViewListItem(IToast data)
            : base(data)
        {
        }

        override public bool InteractiveFlag
        {
            get { return true; }
        }

        override public bool DisplayHeadIconFlag
        {
            get { return true; }
        }

    }
}
