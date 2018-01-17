using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    // Objects could be serialized by Serializer.
    internal interface ISerializableObject : IUniqueObject
    {
        Guid OriginalGuid { get; set; }

        void UpdateGuid();
    }
}
