using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    // A container for widgets, masters, groups and pages.
    public interface IObjectContainer
    {
        ReadOnlyCollection<IWidget> WidgetList { get; }
        ReadOnlyCollection<IMaster> MasterList { get; }
        ReadOnlyCollection<IGroup> GroupList { get; }

        ReadOnlyCollection<IStandardPage> StandardPageList { get; }
        ReadOnlyCollection<ICustomObjectPage> CustomObjectPageList { get; }
        ReadOnlyCollection<IMasterPage> MasterPageList { get; }
    }
}
