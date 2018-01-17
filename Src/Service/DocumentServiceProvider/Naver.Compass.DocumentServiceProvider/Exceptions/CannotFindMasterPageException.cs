using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public class CannotFindMasterPageException : Exception
    {
        public CannotFindMasterPageException(string message)
            : base(message)
        {
        }
    }
}
