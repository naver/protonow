using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naver.Compass.Service.Document
{
    public interface IGroup : IRegion
    {
        IGroup ParentGroup { get; }

        Guid ParentGroupGuid { get; }

        // Top level child group in this group.
        IGroups Groups { get; }

        // Top level child widgets in this group.
        IWidgets Widgets { get; }

        // Top level child masters in this group.
        IMasters Masters { get; }

        // If recursive is false, only check the top level Groups, Widgets and Masters.
        // If recursive is true, it will keep checking in the child groups in Groups recursively 
        // untill all child groups are checked.
        bool IsChild(Guid childGuid, bool recursive);
    }
}
