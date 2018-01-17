using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class HamburgerMenuButton : Image, IHamburgerMenuButton
    {
        internal HamburgerMenuButton(HamburgerMenu menu, Page parentPage)
            : base(parentPage)
        {
            Debug.Assert(menu != null);

            TagName = "HamburgerMenuButton";

            _menu = menu;

            // Menu button has the same GUID with its parent HamburgerMenu
            Guid = _menu.Guid;
        }

        public IHamburgerMenu ParentMenu
        {
            get { return _menu; }
        }

        private HamburgerMenu _menu;
    }
}
