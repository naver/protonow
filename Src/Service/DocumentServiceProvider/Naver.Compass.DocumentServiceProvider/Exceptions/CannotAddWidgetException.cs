using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public class CannotAddWidgetException : Exception
    {
        public CannotAddWidgetException(string message)
            : base(message)
        {
        }
    }
}
