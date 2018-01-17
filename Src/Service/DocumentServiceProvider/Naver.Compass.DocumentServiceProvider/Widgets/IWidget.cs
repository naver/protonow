using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naver.Compass.Service.Document
{
    public enum WidgetType
    {
        None,

        FlowShape,  // See FlowShapeType
        Shape,      // See ShapeType

        Image,
        DynamicPanel,
        HamburgerMenu,
        Line,
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
    }

    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public interface IWidget : IRegion, IAnnotatedObject, IInteractiveObject
    {
        WidgetType WidgetType { get; }

        IGroup ParentGroup { get; }

        Guid ParentGroupGuid { get; }

        bool IsDisabled { get; set; }

        bool IsLocked { get; set; }
 
        string Text { get; set; }

        string RichText { get; set; }

        string Tooltip { get; set; }

        string MD5 { get; set; }

        // Style of base view.
        IWidgetStyle WidgetStyle { get; }

        IWidgetStyle GetWidgetStyle(Guid viewGuid);

        // Build a rich text with input simpleText and style, if style is null, using widget's own style
        void SetRichText(string simpleText, IWidgetStyle style = null);

        // Set exact view style if the input adaptive view guid is valid
        void SetWidgetStyleAsDefaultStyle(Guid viewGuid);
    }
}
