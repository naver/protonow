using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naver.Compass.Service.Document
{
    public interface IStreamWidget : IWidget, IHashStreamConsumer
    {
        Stream DataStream { get; set; }
    }
}
