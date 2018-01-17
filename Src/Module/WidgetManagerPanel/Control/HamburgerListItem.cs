using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.ComponentModel;

namespace Naver.Compass.Module
{
    class HamburgerListItem : WidgetListItem
    {
        public HamburgerListItem(IHamburgerMenu data)
            : base(data)
        {
        }

        override public bool HideFlag
        {
            get
            {
                return !((IHamburgerMenu)_Mode).MenuButton.GetWidgetStyle(CurViewID).IsVisible;
            }
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
