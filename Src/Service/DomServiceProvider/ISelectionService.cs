using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service
{
    public interface ISelectionService
    {
        void RegisterWidget(IWidgetPropertyData wdgVM);
        void RemoveWidget(IWidgetPropertyData wdgVM);

        void RegisterWidgets(List<IWidgetPropertyData> ToRegList);
        void RemoveAllWidgets();

        void RegisterPage(IPagePropertyData pageVM, List<IWidgetPropertyData> ToRemoveList);
        void RemovePage(IPagePropertyData pageVM);

        IPagePropertyData GetCurrentPage();

        void AllowWdgPropertyNotify(bool bIsAllowed);
        void AllowPagePropertyNotify(bool bIsAllowed);
        void UpdateSelectionNotify();

        List<object> GetCloneCacheData();

        void ClearCloneCacheData();
        int GetCopyIndex();
        List<IWidgetPropertyData> GetSelectedWidgets();

        List<Guid> GetSelectedWidgetGUIDs();

        int WidgetNumber{ get; }
    }
}
