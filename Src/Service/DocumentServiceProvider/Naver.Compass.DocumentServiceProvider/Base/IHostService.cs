using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IHostService : IDisposable
    {
        /// <summary>
        /// Set system wide widget style.
        /// Widget style inherited hierarchy :
        /// A --> B means If widget cannot get the specific style value in style A, it will try to get in style B.
        /// Adaptive View Style --> Base View Style --> Default Style --> System Style --> Hard Code style value in WidgetStyle
        /// </summary>
        IStyle WidgetSystemStyle { get; }

        string ProductName { get; }

        string ProductVersion { get; }
    }
}
