using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Naver.Compass.Service.Document
{

    public interface ISvg : IStreamWidget
    {
        Stream XmlStream { get; set; }
    }
}
