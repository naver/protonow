using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Naver.Compass.Common.CommonBase
{
    //Font command
    public static class FontCommands
    {
        public static RoutedUICommand Color = new RoutedUICommand();
        public static RoutedUICommand Size = new RoutedUICommand();
        public static RoutedUICommand Family = new RoutedUICommand();
        public static RoutedUICommand Bold = new RoutedUICommand();
        public static RoutedUICommand Italic = new RoutedUICommand();
        public static RoutedUICommand Underline = new RoutedUICommand();
        public static RoutedUICommand Bullet = new RoutedUICommand();
        public static RoutedUICommand Number = new RoutedUICommand();
    }

    //Text command
    public static class TextCommands
    {
        public static RoutedUICommand Strikethrough = new RoutedUICommand();
        public static RoutedUICommand AlignTextTop = new RoutedUICommand();
        public static RoutedUICommand AlignTextBottom = new RoutedUICommand();
        public static RoutedUICommand AlignTextMiddle = new RoutedUICommand();
        public static RoutedUICommand AlignTextLeft = new RoutedUICommand();
        public static RoutedUICommand AlignTextCenter = new RoutedUICommand();
        public static RoutedUICommand AlignTextRight = new RoutedUICommand();
        public static RoutedUICommand OptionCommand = new RoutedUICommand();
        public static RoutedUICommand UpDownCaseHotKey = new RoutedUICommand();
        public static RoutedUICommand UpDownCaseControl = new RoutedUICommand();
    }
    public static class BorderCommands
    {
        public static RoutedUICommand BorderLineColor = new RoutedUICommand();
        public static RoutedUICommand BorderLinePattern = new RoutedUICommand();
        public static RoutedUICommand BorderLineThinck = new RoutedUICommand();
        public static RoutedUICommand BackGround = new RoutedUICommand();
        public static RoutedUICommand LineArrowStyle = new RoutedUICommand();

    }
    public static class WidgetsCommands
    {
        public static RoutedUICommand GroupWidgets = new RoutedUICommand();
        public static RoutedUICommand UngroupWidgets = new RoutedUICommand();
        public static RoutedUICommand DuplicateWidgets = new RoutedUICommand();

        public static RoutedUICommand WidgetsBringFront = new RoutedUICommand();
        public static RoutedUICommand WidgetsBringForward = new RoutedUICommand();

        public static RoutedUICommand WidgetsBringBackward = new RoutedUICommand();
        public static RoutedUICommand WidgetsBringBottom = new RoutedUICommand();

        public static RoutedUICommand WidgetsAlignLeft = new RoutedUICommand();
        public static RoutedUICommand WidgetsAlignRight = new RoutedUICommand();
        public static RoutedUICommand WidgetsAlignCenter = new RoutedUICommand();
        public static RoutedUICommand WidgetsAlignTop = new RoutedUICommand();
        public static RoutedUICommand WidgetsAlignMiddle = new RoutedUICommand();
        public static RoutedUICommand WidgetsAlignBottom = new RoutedUICommand();

        public static RoutedUICommand WidgetsDistributeHorizontally = new RoutedUICommand();
        public static RoutedUICommand WidgetsDistributeVertically = new RoutedUICommand();

        public static RoutedUICommand WidgetsIncreaseWidth = new RoutedUICommand();
        public static RoutedUICommand WidgetsIncreaseHeight = new RoutedUICommand();
        public static RoutedUICommand WidgetsDecreaseWidth = new RoutedUICommand();
        public static RoutedUICommand WidgetsDecreaseHeight = new RoutedUICommand();

    }
    public static class WidgetPropertyCommands
    {
        public static RoutedUICommand Left = new RoutedUICommand();
        public static RoutedUICommand Top = new RoutedUICommand();
        public static RoutedUICommand Width = new RoutedUICommand();
        public static RoutedUICommand Height = new RoutedUICommand();
        public static RoutedUICommand Content = new RoutedUICommand();
        public static RoutedUICommand Name = new RoutedUICommand();
        public static RoutedUICommand Rotate = new RoutedUICommand();
        public static RoutedUICommand TextRotate = new RoutedUICommand();
        public static RoutedUICommand CornerRadius = new RoutedUICommand();
        public static RoutedUICommand Opacity = new RoutedUICommand();
        public static RoutedUICommand Hide = new RoutedUICommand();
        public static RoutedUICommand IsFixed = new RoutedUICommand();
        public static RoutedUICommand Tooltip = new RoutedUICommand();
        public static RoutedUICommand ImportImage = new RoutedUICommand();
        public static RoutedUICommand Slice = new RoutedUICommand();
        public static RoutedUICommand Crop = new RoutedUICommand();

        public static RoutedUICommand Lock = new RoutedUICommand();
        public static RoutedUICommand Unlock = new RoutedUICommand();

        public static RoutedUICommand AllLeft = new RoutedUICommand();
        public static RoutedUICommand AllTop = new RoutedUICommand();
        public static RoutedUICommand AllWidth = new RoutedUICommand();
        public static RoutedUICommand AllHeight = new RoutedUICommand();

        public static RoutedUICommand Enable = new RoutedUICommand();
        public static RoutedUICommand ShowSelect = new RoutedUICommand();
        public static RoutedUICommand ButtonAlign = new RoutedUICommand();        
        public static RoutedUICommand RadioGroup = new RoutedUICommand();

        public static RoutedUICommand HideBorder = new RoutedUICommand();
        public static RoutedUICommand ReadOnly = new RoutedUICommand();
        public static RoutedUICommand HintText = new RoutedUICommand();
        public static RoutedUICommand MaxLength = new RoutedUICommand();
        public static RoutedUICommand TextFieldType = new RoutedUICommand();

        //Hamburger Menu
        public static RoutedUICommand MenuPageLeft = new RoutedUICommand();
        public static RoutedUICommand MenuPageTop = new RoutedUICommand();
        public static RoutedUICommand MenuPageWidth = new RoutedUICommand();
        public static RoutedUICommand MenuPageHeight = new RoutedUICommand();
        public static RoutedUICommand MenuPageHide = new RoutedUICommand();
    }

    //App command
    public static class AppCommands
    {
        public static RoutedUICommand DefaultStyle = new RoutedUICommand();
    }

    public static class GridGuideCommands
    {
        public static RoutedUICommand ShowGrid = new RoutedUICommand();
        public static RoutedUICommand SnapToGrid = new RoutedUICommand();
        public static RoutedUICommand GridSetting = new RoutedUICommand();
        public static RoutedUICommand ShowGlobalGuides = new RoutedUICommand();
        public static RoutedUICommand ShowPageGuides = new RoutedUICommand();
        public static RoutedUICommand SnapToGuides = new RoutedUICommand();
        public static RoutedUICommand LockGuides = new RoutedUICommand();
        public static RoutedUICommand CreateGuides = new RoutedUICommand();
        public static RoutedUICommand DeleteAllGuides = new RoutedUICommand();
        public static RoutedUICommand GuideSetting = new RoutedUICommand();
        public static RoutedUICommand SnapToObject = new RoutedUICommand();
        public static RoutedUICommand ObjectSnapSetting = new RoutedUICommand();

    }

    public static class FlickCommands
    {
        public static RoutedUICommand ShowArrow = new RoutedUICommand();
        public static RoutedUICommand Circuler = new RoutedUICommand();
        public static RoutedUICommand Automatic = new RoutedUICommand();
        public static RoutedUICommand StartPage = new RoutedUICommand();
        public static RoutedUICommand Navigation = new RoutedUICommand();
        public static RoutedUICommand ViewMode = new RoutedUICommand();
        public static RoutedUICommand PanelWidth = new RoutedUICommand();
        public static RoutedUICommand LineWidth = new RoutedUICommand();
    }

    public static class HanburgerCommands
    {
        public static RoutedUICommand HideStyle = new RoutedUICommand();
    }

    public static class ToastCommands
    {
        public static RoutedUICommand ExposureTime = new RoutedUICommand();
        public static RoutedUICommand CloseSetting = new RoutedUICommand();
        public static RoutedUICommand DisplayPosition = new RoutedUICommand();
    }

}
