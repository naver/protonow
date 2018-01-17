using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public class CannotAddMasterException : Exception
    {
        public CannotAddMasterException(string message)
            : base(message)
        {
        }
    }
}
