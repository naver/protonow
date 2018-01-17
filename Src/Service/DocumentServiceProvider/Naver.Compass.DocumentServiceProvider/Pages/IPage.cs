using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Naver.Compass.Service.Document
{
    public interface IPage : IUniqueObject, INamedObject, IAnnotatedObject, IDisposable
    {
        void Open();
        void Close();
        bool IsOpened { get; }

        /*
         * IPageEmbeddedWidget(Interactive UI widgets: Toast, HamburgerMenu/DrawerMenu, DynamicPanel/SwipView ) cannot be added 
         * to EmbeddedPage(Child Page).
         * 
         * IPageEmbeddedWidget -> EmbeddedPage - No
         *                     -> Other pages  - Yes
         * */
        IWidget CreateWidget(WidgetType widgetType);
        void DeleteWidget(Guid widgetGuid);
        void AddWidget(IWidget widget);

        /*
         * Master cannot be added to CustomObjectPage(Page in Library document), MasterPage, EmbeddedPage(Child Page).
         * 
         * Master -> StandardPage(Page in Standard document)    - Yes
         *        -> CustomObjectPage(Page in Library document) - No (Master is broken away)
         *        -> MasterPage                                 - No
         *        -> EmbeddedPage                               - No
         * */
        IMaster CreateMaster(Guid masterPageGuid);
        void DeleteMaster(Guid masterGuid);
        void AddMaster(IMaster master);

        // Break master, the broken master will be deleted and new objects(widgets and groups) in returned 
        // IObjectContainer will be added to this page.
        IObjectContainer BreakMaster(Guid masterGuid);

        IGroup CreateGroup(List<Guid> guidList);
        void Ungroup(Guid groupGuid);
        void DeleteGroup(Guid groupGuid);
        void AddGroup(IGroup group);

        IObjectContainer AddObjects(Stream stream);
        IObjectContainer AddCustomObject(ICustomObject customObject);
        IObjectContainer AddMasterPageObject(Guid masterPageGuid, Guid viewGuid);

        IDocument ParentDocument { get; }

        Stream Thumbnail { get; set; }

        IWidgets Widgets { get; }

        IMasters Masters { get; }

        IGroups Groups { get; }

        IPageViews PageViews { get; }

        IRegions WidgetsAndMasters { get; }

        int Zoom { get; set; }

        string MD5 { get; set; }
    }
}
