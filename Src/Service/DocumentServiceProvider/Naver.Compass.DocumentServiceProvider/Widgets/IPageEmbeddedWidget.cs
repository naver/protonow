using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    public interface IPageEmbeddedWidget : IWidget
    {
        ReadOnlyCollection<IEmbeddedPage> EmbeddedPages { get; }
    }
}
