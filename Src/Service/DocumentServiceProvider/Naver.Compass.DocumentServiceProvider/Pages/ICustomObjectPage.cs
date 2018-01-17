using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naver.Compass.Service.Document
{
    // A page which represent a custom object
    public interface ICustomObjectPage : IDocumentPage
    {
        bool UseThumbnailAsIcon { get; set; }
        Stream Icon { get; set; }
        string Tooltip { get; set; }
    }
}
