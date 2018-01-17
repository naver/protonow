using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Common.CommonBase
{

    public enum AddWidgetType
    {
        Default,
        DrgAdd,
        DoubleClickAdd,
    }

    public enum SiteMapEventEnum
    {
        Default,
        OpenHomePage,
        OpenEndPage,
        CreateNewPage,
    }
    public enum WDMgrHideEventEnum
    {
        NormalWidget,
        SwipeViewPanel,
        ChildGroup
    }

    public enum ObjectType
    {
        None,

        Group,
        FlowShape,  // See FlowShapeType
        Rectangle,
        RoundedRectangle,
        Ellipse,
        Diamond,
        Triangle,
        Paragraph,

        Image,
        DynamicPanel,
        HamburgerMenu,
        HorizontalLine,
        VerticalLine,
        HotSpot,
        TextField,
        TextArea,
        DropList,
        ListBox,
        Checkbox,
        RadioButton,
        Button,
        Toast,
        SVG,
        Master,
    }

    public  class PublishParameter
    {
        public PublishParameter()
        {
           
        }
      public  string UserID;
      public string UserPassword;
      public string ProjectPassword;
      public bool IsNewProject;
      public bool IsPrivatePublish;
      public string ProjectPath;
      public string RemotePath;
      public string DocGUID;
      public string sShortURL;
    }

    public class PreviewParameter
    {
        public PreviewParameter()
        {
            SavePath = "";
            IsPreviewCurrentPage = false;
            IsBrowerOpen = true;
        }
        public string SavePath;
        public bool IsBrowerOpen;
        public bool IsPreviewCurrentPage;
      
    }
    public class DiffGeneratorParameter
    {
        public DiffGeneratorParameter()
        {
            SavePath = "";
        }
        public string SavePath;
        public List<IDocumentService> Docs;

    }
    public class UploadParameter
    {
        public UploadParameter()
        {

        }
        public string ProjectPassword;
        public bool IsNewProject;
        public string id;
        public string DocGUID;
        public string sShortURL;
        public string sTime;
        public bool IsPublic;
        public string ProjectPath;
        public List<IDocumentService> DocArray;
    }

    public class BorderLineWidthData
    {
        public BorderLineWidthData(Uri uri, int width)
        {
            PngUri = uri;
            Width = width;
            ShowText = false;
        }
        public Uri PngUri { get; set; }
        public int Width { get; set; }
        public bool ShowText { get; set; }
    }

    
    public class BorderLineStyleData
    {
        public BorderLineStyleData(Uri uri, int style)
       {
           PngUri = uri;
           BLStyle = style;
       }
        public Uri PngUri { get; set; }
        public int BLStyle { get; set; }
      
    }

    public class LineArrowStyleDate
    {
        public LineArrowStyleDate(Uri uri, ArrowStyle style)
        {
            PngUri = uri;
            BLStyle = style;
            ShowText = false;
        }
        public Uri PngUri { get; set; }
        public ArrowStyle BLStyle { get; set; }
        public bool ShowText { get; set; }

    }

    public class WidgetTextBase
    {

   }

    public class WidgetText:WidgetTextBase
    {
       public WidgetText()
        {
            FontFamily = "Arial";
            FontSize = 13;
            Color = CommonFunction.CovertColorToInt(System.Windows.Media.Color.FromRgb(0,0,0));
            Bold = false;
            Italic = false;
            Underline = false;
            sText = "";
        }
        public string FontFamily { get; set; }
        public double FontSize { get; set; }
        public int Color { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Underline { get; set; }
        public bool StrikeThough { get; set; }
        public string sText { get; set; }

    }

    public class ViewPortData
    {
        public string Name { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class WidgetSelectionInfo
    {
        public Guid WidgetID { get; set; }
        public bool bSelected { get; set; }
     }

    public class WidgetSelectionInfoExtra
    {
        public Guid WidgetID { get; set; }
        public IWidget BelongWidget { get; set; }
        public Guid PageID { get; set; }
        public PageType pageType { get; set; }
        public bool bSelected { get; set; }
        public bool IsGroup { get; set; }
        public bool IsSwipePanel { get; set; }
    }

    public class WDMgrZorderChangeInfo
    {
        public Guid PageID { get; set; }
        public bool bForward {get;set;}
    }
    public class WDMgrZorderDragChangeInfo
    {
        public Guid PageID { get; set; }
        public Guid widgetID { get; set; }
        public int zIndex { get; set; }
    }

    public class WDMgrWidgetDeleteInfo
    {
        public Guid PageID { get; set; }
        public List<Guid> WidgetList { get; set; }
    }

    public class WDMgrHideStatusChangeInfo
    {
        public Guid PageID { get; set; }
        public Guid ID { get; set; }
        public bool HideFlag { get; set; }
        public WDMgrHideEventEnum HideType { get; set; }
    }

    public class WDMgrPlaceStatusChangeInfo
    {
       public  WDMgrPlaceStatusChangeInfo()
        {
            bPlace = false;
            WidgetList = new List<Guid>();
        }

        public bool bPlace { get; set; }
        public List<Guid> WidgetList { get; set; }
    }
}
