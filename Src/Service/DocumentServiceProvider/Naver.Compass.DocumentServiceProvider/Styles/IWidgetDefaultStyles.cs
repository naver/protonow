using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public interface IWidgetDefaultStyles : IEnumerable<IWidgetDefaultStyle>
    {
        IWidgetDefaultStyle GetWidgetDefaultStyle(string name);

        bool Contains(string name);

        int Count { get; }

        IWidgetDefaultStyle this[string name] { get; }
    }
}
