using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public class CannotAddPageException : Exception
    {
        public CannotAddPageException(string message)
            : base(message)
        {
        }
    }
}
