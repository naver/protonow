using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public class HigherDocumentVersionException : Exception
    {
        public HigherDocumentVersionException(string thisVersion, string fileVersion, string message)
            : base(message)
        {
            ThisVersion = thisVersion;
            FileVersion = fileVersion;
        }

        public string ThisVersion { get; private set; }
        public string FileVersion { get; private set; }
    }
}
