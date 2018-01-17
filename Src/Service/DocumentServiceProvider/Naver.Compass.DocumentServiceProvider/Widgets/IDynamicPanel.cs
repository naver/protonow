using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    public enum DynamicPanelViewMode
    {
        Full,
        Card,
        Preview,
        Scroll
    }


    public enum NavigationType
    {
        None,
        Dot,
        Number
    }

    public interface IDynamicPanel : IPageEmbeddedWidget
    {
        IPanelStatePage StartPanelStatePage { get; set; }

        bool IsCircular { get; set; }

        bool IsAutomatic { get; set; }

        int AutomaticIntervalTime { get; set; }

        int DurationTime { get; set; }

        NavigationType NavigationType { get; set; }

        bool ShowAffordanceArrow { get; set; }

        DynamicPanelViewMode ViewMode { get; set; }

        int PanelWidth { get; set; }

        double LineWith { get; set; }

        List<IPanelStatePage> PanelStatePages { get; }

        IPanelStatePage CreatePanelStatePage(string name);

        // Add to the end if index is less than 0.-or-index is greater than PanelStatePages.Count.
        void AddPanelStatePage(IPanelStatePage page, int index);

        IPanelStatePage GetPanelStatePage(Guid pageGuid);

        void DeletePanelStatePage(Guid pageGuid);

        bool MovePanelStatePage(IPanelStatePage page, int delta);

        bool MovePanelStatePageTo(IPanelStatePage page, int index);

        int IndexOf(IPanelStatePage page);
    }
}
