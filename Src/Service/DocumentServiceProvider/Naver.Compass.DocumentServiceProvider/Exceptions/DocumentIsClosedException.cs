using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public class DocumentIsClosedException : Exception
    {
        public DocumentIsClosedException(string message)
            : base(message)
        {
        }
    }
}
