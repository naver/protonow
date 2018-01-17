using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public class CannotOpenPageException : Exception
    {
        public CannotOpenPageException(string message)
            : base(message)
        {
        }
    }
}
