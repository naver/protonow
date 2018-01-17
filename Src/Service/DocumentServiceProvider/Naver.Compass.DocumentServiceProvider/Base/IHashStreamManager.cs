using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naver.Compass.Service.Document
{
    public interface IHashStreamManager : IDisposable
    {
        IDocument ParentDocument { get; }

        Stream GetStream(string hash);

        string SetStream(Stream stream, string streamType);

        void ClearCachedStream();
    }
}
