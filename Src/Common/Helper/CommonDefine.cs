using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Naver.Compass.Common.Helper
{
    public class CommonDefine
    {
        //max size define of editor canvers
        public const int MaxEditorWidth = 25000;
        public const int MaxEditorHeight = 25000;

        // Minimum of page resolution size
        public const int MinResolutionSize = 0;

        // Minimum of widget size
        public const int MinWidgetSize = 1;

        // Minimum of widget location
        public const int MinWidgetLocation = int.MinValue;

        // Maximum of opacity
        public const int MaxOpacity = 100;
        public const int MinOpacity = 0;

        //Margin for snap to guide/grid
        public const double SnapMargin = 5;

        //Device resolution
        public const string PresetOff = "Off";

        public const string PCRetina = "PC web  2560 ";
        public const string PC1280 = "PC web  1280 ";
        public const string PC1024 = "PC web  1024 ";

        public const string IPhone6Plus = "iPhone 6S, 6 Plus ";
        public const string IPhone6 = "iPhone 6S, 6 ";
        public const string IPhone5 = "iPhone 5S, 5 ";
        public const string GalaxyS6 = "Galaxy S6, Note5, 4 ";
        public const string GalaxyS5 = "Galaxy S5, S4, Note 3 ";
        public const string GalaxyS3 = "Galaxy S3, Note 2 ";
        public const string GoogleNexus5 = "Nexus 5 ";
        public const string LGOptimusG3 = "Optimus G3 ";

        public const string IPadPro = "iPad Pro ";
        public const string IpadMini2 = "iPad Air2, Air, mini 4, 2 ";
        public const string IpadMini1 = "iPad 2, 1, mini ";
        public const string GalaxyTabS2 = "Galaxy Tab S2, S ";
        public const string GalaxyTabA = "Galaxy Tab A, SPen ";
        public const string GoogleNexus7 = "Nexus 7 ";

        public const string Watch42mm = "Watch 42mm ";
        public const string Watch38mm = "Watch 38mm ";
        
        public const string UserSetting = "User Setting";

        //Hotkey contextmenu

        public const string KeyPaste = "Ctrl+V";
        public const string KeySelectAll = "Ctrl+A";
        public const string KeyCut = "Ctrl+X";
        public const string KeyCopy = "Ctrl+C";

        public const string KeyGroup = "Ctrl+G";
        public const string KeyUnGroup = "Ctrl+Shift+G";

        public const string KeyBringFront = "Ctrl+Shift+]";
        public const string KeySendBack = "Ctrl+Shift+[";
        public const string KeyBringForward = "Ctrl+]";
        public const string KeySendBackward = "Ctrl+[";

        public const string KeyAlignLeft = "Ctrl+Alt+L";
        public const string KeyAlignCenter = "Ctrl+Alt+C";
        public const string KeyAlignRight = "Ctrl+Alt+R";
        public const string KeyAlignTop = "Ctrl+Alt+T";
        public const string KeyAlignMiddle = "Ctrl+Alt+M";
        public const string KeyAlignBottom = "Ctrl+Alt+B";

        public const string KeyDistributeHori = "Ctrl+Shift+H";
        public const string KeyDistributeVert = "Ctrl+Shift+U";

        public const string KeyShowGrid = "Ctrl+'";
        public const string KeySnaptoGrid = "Ctrl+Alt+'";
        public const string KeyShowGlobalGuide = "Ctrl+.";
        public const string KeyShowPageGuide = "Ctrl+,";
        public const string KeySnaptoGuide = "Ctrl+Alt+,";
        public const string KeyLockGuide = "Ctrl+Alt+.";


        //Panel flag, used for show/hide panel in menu
        public const string PaneSitemap = "Sitmap";
        public const string PaneWidgets = "Widgets";
        public const string PanePageProp = "PageProperty";
        public const string PaneInteraction = "Interaction";
        public const string PaneWidgetProp = "WidgetProperty";
        public const string PanePageIcon = "PageIcon";
        public const string PaneWidgetManager = "WidgetManager";
        public const string PaneMaster = "Masters";

        //change layout file name to XXX.config_01 to implement setting new height for right panel
        //XXX.config will be deleted later
        //Dock panel Layout files
        public const string DefaultConfigFile = @"\Compass_DefaultLayout.config_07";
        public const string CustomConfigFile = @"\Compass_CustomLayout.config_07";

        //Please configure your URLs
        public const string UrlFeedBack = "";
        public const string UrlGuide = "";
        public const string UrlLibaryDownload = "";

        public const string UrlTableUI = "";
        public const string UrlTemplateUI = "";
        public const string UrlUIGuide = "";

        public const string ImageFilter = "All Image Files (*.png;*.bmp;*.jpg;*.jpeg;*.gif;*.ico)|*.png;*.bmp;*.jpg;*.jpeg;*.gif;*.ico|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|GIF (*.gif)|*.gif|ICO (*.ico)|*.ico";

        public const string Untitled = "untitled";

        public const string LibraryFilter = @"libpn documents (*.libpn)|*.libpn";

        public static SolidColorBrush StandardWindowBarColor = new SolidColorBrush(Color.FromArgb(0xff, 0x4a, 0x7e, 0xec));
        public static SolidColorBrush LibraryWindowBarColor = new SolidColorBrush(Color.FromArgb(0xff, 0x59, 0x69, 0x80));
        public static SolidColorBrush StandardWindowBorderColor = new SolidColorBrush(Color.FromArgb(0xff, 0x3d, 0x67, 0xbf));
        public static SolidColorBrush LibraryWindowBorderColor = new SolidColorBrush(Color.FromArgb(0xff, 0x49, 0x56, 0x69));
    }
}
