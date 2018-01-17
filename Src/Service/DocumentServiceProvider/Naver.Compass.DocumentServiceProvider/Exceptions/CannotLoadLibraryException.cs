using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    class CannotLoadLibraryException: Exception
    {
        public CannotLoadLibraryException(string libraryFileName, string message)
            : base(message)
        {
            LibraryFileName = libraryFileName;
        }

        public string LibraryFileName
        {
            get;
            private set;
        }
    }
}
