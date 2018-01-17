using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum ActionOpenIn
    {
        CurrentWindow,
        NewWindowOrTab,
    }

    public enum LinkType
    {
        None,
        LinkToPage,
        LinkToUrl,
    }

    public interface IInteractionOpenAction : IInteractionAction
    {
        LinkType LinkType { get; set; }

        ActionOpenIn OpenIn { get; set; }

        Guid LinkPageGuid { get; set; }

        string ExternalUrl { get; set; }
    }
}
