using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IWidgetDefaultStyleSet
    {
        IDocument ParentDocument { get; }

        IWidgetDefaultStyles WidgetDefaultStyles { get; }

        IWidgetDefaultStyle GetWidgetDefaultStyle(IWidget widget);
    }
}
