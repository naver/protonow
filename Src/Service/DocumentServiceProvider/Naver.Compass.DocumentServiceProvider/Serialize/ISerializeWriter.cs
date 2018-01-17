using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naver.Compass.Service.Document
{
    // Add and serialise objects and write them to a stream.
    public interface ISerializeWriter : IObjectContainer
    {
        void AddWidget(IWidget widget);
        void AddMaster(IMaster master);
        void AddGroup(IGroup group);

        void AddPage(IDocumentPage page);
        void AddStandardPage(IStandardPage page);
        void AddCustomObjectPage(ICustomObjectPage page);
        void AddMasterPage(IMasterPage page);

        Stream WriteToStream();
    }
}
