using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public class CannotDeletePageException : Exception
    {
        public CannotDeletePageException(string message)
            : base(message)
        {
        }
    }
}
