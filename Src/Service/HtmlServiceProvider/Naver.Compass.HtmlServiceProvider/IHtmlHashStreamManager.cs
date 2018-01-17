using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naver.Compass.Service
{
    public interface IHtmlHashStreamManager 
    {
        // Change working directory will clear consumers data.
        string WorkingDirectory { get; set; }

        string GetConsumerStreamHash(Guid consumerGuid, Guid viewGuid);

        Stream GetConsumerStream(Guid consumerGuid, Guid viewGuid);

        string SetConsumerStream(Guid consumerGuid, Guid viewGuid, Stream stream, string streamType);
    }
}
