using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    // A library is a collection of custom objects.
    public interface ILibrary : IUniqueObject, INamedObject, IDisposable
    {
        string Title { get; }
                
        ReadOnlyCollection<ICustomObject> CustomObjects { get; }

        ICustomObject GetCustomObject(Guid objectGuid);

        ICustomObject AddCustomObject(ISerializeWriter writer, string objectName, Stream icon, Stream thumbnail);

        void DeleteCustomObject(Guid objectGuid);

        void Save(string fileName, bool saveCopy = false);

        void SaveCopyTo(string fileName);
    }
}
