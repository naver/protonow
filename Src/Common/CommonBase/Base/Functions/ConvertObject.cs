using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Common.CommonBase
{
    public class ConvertObject
    {
        public static string GetTypeName(ObjectType type)
        {
            switch(type)
            {
                case ObjectType.Image:
                    return FindResource("widgets_Image");
                case ObjectType.Paragraph:
                    return FindResource("widgets_Text");
                case ObjectType.Diamond:
                    return FindResource("widgets_Diamond");
                case ObjectType.Ellipse:
                    return FindResource("widgets_Circle");
                case ObjectType.RoundedRectangle:
                    return FindResource("widgets_RoundedRectangle");
                case ObjectType.Rectangle:
                    return FindResource("widgets_Rectangle");
                case ObjectType.Triangle:
                    return FindResource("widgets_Shape");
                case ObjectType.HotSpot:
                    return FindResource("widgets_Link");
                case ObjectType.VerticalLine:
                    return FindResource("widgets_VerticalLine");
                case ObjectType.HorizontalLine:
                    return FindResource("widgets_HorizontalLine");
                case ObjectType.ListBox:
                    return FindResource("widgets_Listbox");
                case ObjectType.DropList:
                    return FindResource("widgets_Droplist");
                case ObjectType.Button:
                    return FindResource("widgets_Button");
                case ObjectType.Checkbox:
                    return FindResource("widgets_Checkbox");
                case ObjectType.RadioButton:
                    return FindResource("widgets_Radiobutton");
                case ObjectType.TextArea:
                    return FindResource("widgets_Textarea");
                case ObjectType.TextField:
                    return FindResource("widgets_Textfield");
                case ObjectType.DynamicPanel:
                    return FindResource("widgets_SwipeViews");
                case ObjectType.HamburgerMenu:
                    return FindResource("widgets_DrawerMenu");
                case ObjectType.Toast:
                    return FindResource("widgets_Toast");
                case ObjectType.SVG:
                    return FindResource("widgets_SVG");
                case ObjectType.Group:
                    return FindResource("widgets_Group");
                case ObjectType.Master:
                    return FindResource("Master_Title");
                default:
                    return string.Empty;
 
            }
        }


        private static string FindResource(string resourceKey)
        {
            return Application.Current.TryFindResource(resourceKey).ToString();
        }
    }
}
