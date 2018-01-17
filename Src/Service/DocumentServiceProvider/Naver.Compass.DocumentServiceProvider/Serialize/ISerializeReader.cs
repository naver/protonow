using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    // Read from a stream and unserialize them. 
    // You also can get widget guid list or type list without fully loading and unserializing all data.
    public interface ISerializeReader : IObjectContainer
    {
        void ReadAllFromStream();

        ReadOnlyCollection<Guid> PeekWidgetGuidList();

        ReadOnlyCollection<WidgetType> PeekWidgetTypeList();

        ReadOnlyCollection<Guid> PeekMasterGuidList();

        bool ContainsMaster { get; }
    }
}
