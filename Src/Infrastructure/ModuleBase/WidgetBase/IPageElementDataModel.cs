using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.CommonBase;
using System.Windows.Media;
using System.Windows;

namespace Naver.Compass.InfoStructure
{
    public interface IPageElementDataModel
    {

        #region readonly property
        IRegion WdgDom { get; }
        WidgetType WdgType { get; }
        ShapeType shapeType { get; }
        IRegionStyle Style { get; }
        Guid StyleGID { get; }
        Guid Guid { get; }
        Guid RealParentGroupGID { get; }
        IGroup RealParentGroup { get; }
        #endregion
        
        #region Public functions       
        bool ChangeCurrentStyle(Guid newStyleGid);
        void SetWidgetStyleAsDefaultStyle(Guid StyleGid);
        void SerializeObject(ISerializeWriter writer);
        IRegionStyle GetSpecStyle(Guid gid);
        #endregion

        #region Binding Style Property
        bool IsVisible { get; set; }
        double Left { get; set; }
        double Top { get; set; }
        int CornerRadius { get; set; }
        double ItemWidth { get; set; }
        double ItemHeight { get; set; }
        int ZOrder { get; set; }
        double RotateAngle { get; set; }
        double TextRotate { get; set; }
        double Opacity { get; set; }
        bool IsFixed { get; set; }

        //===================================================================================
        StyleColor cBackgroundColor { get; set; }
        StyleColor cBorderLineColor { get; set; }
        LineStyle cBorderlineStyle { get; set; }
        double iBorderLineWidth { get; set; }
        Alignment cTextHorAligen { get; set; }
        Alignment cTextVerAligen { get; set; }
        string sFontFamily { get; set; }
        double dFontSize { get; set; }
        bool bFontBold { get; set; }
        bool bFontUnderLine { get; set; }
        bool bFontStringThrough { get; set; }
        Color cFontColor { get; set; }
        bool bFontItalic { get; set; }
        TextMarkerStyle MarkerStyle { get; set; }
        #endregion

        #region Binding common property
        string ItemContent { get; set; }
        string DisplayName { get; set; }
        string Tooltip { get; set; }
        bool IsLocked { get; set; }
        bool IsDisabled { get; set; }
        string sTextContent { get; set; }
        string sRichTextContent { get; set; }
        string InputSimpleText { set; }
        #endregion Binding common property
    }
}
