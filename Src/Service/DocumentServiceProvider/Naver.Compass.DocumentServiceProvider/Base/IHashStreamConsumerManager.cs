using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    internal interface IHashStreamConsumerManager
    {
        void AddConsumer(IHashStreamConsumer consumer);

        void AddConsumer(string hash, Guid consumerGuid, Guid parentPageGuid);

        void DeleteConsumer(IHashStreamConsumer consumer);

        void DeleteConsumer(Guid consumerGuid);

        void DeleteConsumersInPage(Guid pageGuid);

        bool AnyConsumer(string hash);

        bool AnyActiveConsumer(string hash);

        bool ContainsConsumer(Guid consumerGuid);

        void Clear();
    }
}
