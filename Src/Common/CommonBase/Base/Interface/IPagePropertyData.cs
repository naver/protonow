using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Naver.Compass.Service.Document;
using System.Collections.ObjectModel;

namespace Naver.Compass.Common.CommonBase
{
    public enum PageType
    {
        NormalPage,
        DynamicPanelPage,
        HamburgerPage,
        ToastPage,
        MasterPage,
    }

    public interface IPagePropertyData
    {
        IInputElement EditorCanvas { get; set; }
        bool IsDirty { get; set; }
        Guid PageGID { get; }
        Rect BoundingRect { get;}
        List<IWidgetPropertyData> GetSelectedwidgets();
        List<IWidgetPropertyData> GetAllWidgets();
        bool CommonEventNotify(string sEvents,object para);
        void EditHanburgerPage();
        void CancelEditHamburgerPage();

        void DuplicateWidgets(object parameter);

        void SetAdaptiveView(Guid id);
        Guid CurAdaptiveViewGID { get;}
        //this is only for dynamic page editor
        IPage ActivePage { get; set; }

        double EditorScale { get; set; }
        PageType PageType { get; }

        // if page is opened/activated by click page in sitemap/master, true
        //if page is opened/activated by click head of page, false
        bool IsNeedReturnFocus { set; get; }

        //save preview image and custom widget icon according to thumbnail information when switching page or save project
        bool GetIsThumbnailUpdate();
        void SetIsThumbnailUpdate(bool bIsDirty);
        bool IsUseThumbnailAsIcon { get; set; }
    }
}
