using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IListItem
    {
        string TextValue { get; set; }
        bool IsSelected { get; set; }
    }

    public interface IListBase : IWidget
    {
        List<IListItem> Items { get; set; }

        IListItem CreateItem(string textValue);
    }
}
