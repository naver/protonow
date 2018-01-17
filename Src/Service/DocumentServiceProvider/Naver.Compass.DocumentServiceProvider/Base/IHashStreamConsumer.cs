using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IHashStreamConsumer : IUniqueObject
    {
        string Hash { get; set; }

        string StreamType { get; set; }

        Guid ParentPageGuid { get; }
    }
}
