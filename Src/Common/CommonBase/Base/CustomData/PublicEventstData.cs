using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Common.CommonBase
{
    //file operation data.
    public enum FileOperationType
    {
        Create,
        Open,
        Save,
        SaveAs,
        Close,
        Publish,
        Preview,
        HTMLExport,
        AutoSave,
    }

    //Close or open panes.
    public class ActivityPane
    {
        public string Name{get;set;}
        public bool bOpen{get;set;}

        //True, if event is sent from toolbar
        public bool bFromToolbar;
    }

    public enum DialogType
    {
        LanguageSetting,
        PageNoteField,
        AdaptiveView,
        AboutPopup,
        WelcomeScreen
    }

    public enum GridGuideType
    {
        All,//grid and guide
        //Grid,
        Guide
    }

    public enum TBUpdateType
    {
        Default,
        FormatPaint,
        Ohters
    }

    public class PageInfo
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }

        public bool IsInPopup { get; set; }
    }

    public enum GuideType
    {
        Local,
        Global
    }

    public class GuideInfo
    {
        public GuideInfo(GuideType type, IGuide guide)
        {
            Type = type;
            Guide = guide;
        }
        public GuideType Type { get; set; }
        public IGuide Guide { get; set; }
    }

    public enum AdaptiveLoadType
    {
        Load,
        Edit
    }

    public enum StringStatus
    {
        AllSmall = 0,
        First,
        Middle,
        FirstAndMiddle,
        AllUplow
    }

    public enum PropertyOption
    {
        OPtion_Default,
        Option_Border,
        Option_Text,
        Option_LineArrow,
        Option_Bullet,
        Option_TextHor,
        Option_TextVer,
        OPtion_BackColor,
        Option_WidgetRotate,
        Option_CornerRadius,
        Option_TextRotate
    }

}
