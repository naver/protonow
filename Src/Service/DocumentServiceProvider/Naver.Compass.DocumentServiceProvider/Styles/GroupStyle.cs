using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class GroupStyle : RegionStyle
    {
        internal GroupStyle(Group ownerGroup, Guid viewGuid)
            : base(viewGuid, "GroupStyle")
        {
            // GroupStyle must exist with its owner.
            Debug.Assert(ownerGroup != null);
            _ownerGroup = ownerGroup;
        }

        public override IRegion OwnerRegion
        {
            get { return _ownerGroup; }
        }

        private Group _ownerGroup;
    }
}
