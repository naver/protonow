using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naver.Compass.Service.Document
{
    /*
     * Widgets have different Location, size, style across adaptive view.
     * IPageView is a adaptive view in a page. 
     * You can create/delete/add/place/unplace widgets/groups in this view.
     * Also every view has its own guides.
     * */
    public interface IPageView : IRegion
    {
        IWidgets Widgets { get; }
        IMasters Masters { get; }
        IGroups Groups { get; }
        IGuides Guides { get; }

        IWidget CreateWidget(WidgetType type);
        void DeleteWidget(Guid widgetGuid);
        void AddWidget(IWidget widget);

        // Place/Unplace a widget in this view. Unplace a widget is to make this widget unvisible in this view,
        // and the widget is still in the page.
        void PlaceWidget(Guid widgetGuid);
        void UnplaceWidget(Guid widgetGuid);

        IMaster CreateMaster(Guid masterPageGuid);
        void DeleteMaster(Guid masterGuid);
        void AddMaster(IMaster master);
        IObjectContainer BreakMaster(Guid masterGuid);

        void PlaceMaster(Guid masterGuid);
        void UnplaceMaster(Guid masterGuid);

        IGroup CreateGroup(List<Guid> guidList);
        void Ungroup(Guid groupGuid);
        void DeleteGroup(Guid groupGuid);
        void AddGroup(IGroup group);

        // Add widgets, masters, groups.
        IObjectContainer AddObjects(Stream stream);

        // Add widgets, groups in customObject.
        IObjectContainer AddCustomObject(ICustomObject customObject, double x = 0, double y = 0);

        // Add widgets, groups in masterPage.
        IObjectContainer AddMasterPageObject(Guid masterPageGuid, Guid viewGuid, double x = 0, double y = 0);

        IGuide CreateGuide(Orientation orientation, double x = 0, double y = 0);
        void DeleteGuide(Guid guideGuid);
        void AddGuide(IGuide guide);
    }
}
