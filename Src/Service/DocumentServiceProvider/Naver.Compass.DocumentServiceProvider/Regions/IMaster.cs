using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IMaster : IRegion, IAnnotatedObject
    {
        IMasterPage MasterPage { get; }

        Guid MasterPageGuid { get; }

        IGroup ParentGroup { get; }

        Guid ParentGroupGuid { get; }

        bool IsLocked { get; set; }

        // Lock to the location in master page.
        bool IsLockedToMasterLocation { get; set; }

        IMasterStyle MasterStyle { get; }

        string MD5 { get; set; }

        IMasterStyle GetMasterStyle(Guid viewGuid);
    }
}
