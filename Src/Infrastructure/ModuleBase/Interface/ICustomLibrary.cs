using Naver.Compass.Common;
using Naver.Compass.Common.CommonBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Naver.Compass.InfoStructure
{
    public interface IMyLibrary
    {
        void CreateCustomLibrary(Guid libraryId,string libraryName);
    }

    public interface ICustomLibrary
    {
        Guid LibraryGID { get; set; }
        string Header { get; set; }
        string TabType { get; set; }
        string FileName { get; set; }
        //RangeObservableCollection<ICustomWidget> WidgetModels { get; set; }
        IEnumerable<ICustomWidget> GetAllCustomWidgets();
        void Refresh();

        bool IsCustomWidget { get; set; }
        bool IsExpand { get; set; }
        bool IsVisible { get; set; }
        string SearchText { get; set; }
        void AddCustomObject(Guid customObjId);
    }
}
