using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    // A page which can be embedded in a widget which can embed pages.
    public interface IEmbeddedPage : IPage
    {
        IPageEmbeddedWidget ParentWidget { get; }
    }
}
