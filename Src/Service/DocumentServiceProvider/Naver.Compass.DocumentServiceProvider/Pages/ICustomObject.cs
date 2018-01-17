using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naver.Compass.Service.Document
{
    public interface ICustomObject : IUniqueObject
    {
        ILibrary ParentLibrary { get; }

        string Name { get; set; }
        
        Stream Thumbnail { get; }

        Stream Icon { get; }

        string Tooltip { get; }

        IWidgets Widgets { get; }

        IGroups Groups { get; }

        void Open();
        void Close();
        bool IsOpened { get; }
    }
}
