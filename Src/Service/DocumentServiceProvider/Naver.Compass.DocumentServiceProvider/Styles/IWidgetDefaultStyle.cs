using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Service.Document
{
    public interface IWidgetDefaultStyle : IStyle, INamedObject
    {
        IWidgetDefaultStyleSet WidgetDefaultStyleSet { get; }
    }
}
