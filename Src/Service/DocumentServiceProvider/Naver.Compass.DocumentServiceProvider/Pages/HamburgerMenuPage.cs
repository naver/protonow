using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class HamburgerMenuPage : EmbeddedPage, IHamburgerMenuPage
    {
        internal HamburgerMenuPage(HamburgerMenu menu, string pageName)
            : base("HamburgerMenuPage", pageName)
        {
            Debug.Assert(menu != null);
            _menu = menu;

            InitializeBasePageView();
        }

        public override IPageEmbeddedWidget ParentWidget
        {
            get { return ParentMenu; }
        }

        public IHamburgerMenu ParentMenu
        {
            get { return _menu; }
        }

        private HamburgerMenu _menu;
    }
}
