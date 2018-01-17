using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    // A unique region in a page. It could have a name.
    // The region may have different visibility, size, location and rotate degree in different view.
    public interface IRegion : IUniqueObject, INamedObject
    {
        // Return null if the region has been not added to a document yet.
        IDocument ParentDocument { get; }

        // Return null if the region has been not added to a page yet.
        IPage ParentPage { get; }

        // A region may have different child regions in different view.
        IRegions GetChildRegions(Guid viewGuid);

        // Region style in base view.
        IRegionStyle RegionStyle { get; }

        // Get region style with specific adpative view guid.
        IRegionStyle GetRegionStyle(Guid viewGuid);
    }
}
