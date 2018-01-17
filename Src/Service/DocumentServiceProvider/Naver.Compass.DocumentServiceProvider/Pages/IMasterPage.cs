using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    public interface IMasterPage : IDocumentPage
    {
        // Return all opened document page which contains masters that master page is this page.
        ReadOnlyCollection<Guid> ActiveConsumerPageGuidList { get; }

        // Return all document page which contains masters which that page is this page.
        // Performance hit point.
        ReadOnlyCollection<Guid> AllConsumerPageGuidList { get; }

        bool IsLockedToMasterLocation { get; set; }
    }
}
