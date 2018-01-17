using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naver.Compass.Service.Document
{
    public enum HiddenOn
    {
        Left,
        Right
    }

    public interface IHamburgerMenu : IPageEmbeddedWidget
    {
        IHamburgerMenuPage MenuPage { get; }

        IHamburgerMenuButton MenuButton { get; }

        HiddenOn HiddenOn { get; set; }
    }
}
