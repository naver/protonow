using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naver.Compass.Service.Document
{
    internal static class WidgetFactory
    {
        internal static Widget CreateWidget(Page parentPage, WidgetType widgetType)
        {
            switch (widgetType)
            {
                case WidgetType.FlowShape:
                    return new FlowShape(parentPage, FlowShapeType.None);

                case WidgetType.Shape:
                    return new Shape(parentPage, ShapeType.None);
                
                case WidgetType.Image:
                    return new Image(parentPage);

                case WidgetType.DynamicPanel:
                    return new DynamicPanel(parentPage);

                case WidgetType.HamburgerMenu:
                    return new HamburgerMenu(parentPage);

                case WidgetType.Toast:
                    return new Toast(parentPage);

                case WidgetType.Line:
                    return new Line(parentPage);

                case WidgetType.HotSpot:
                    return new HotSpot(parentPage);

                case WidgetType.TextField:
                    return new TextField(parentPage);

                case WidgetType.TextArea:
                    return new TextArea(parentPage);

                case WidgetType.DropList:
                    return new Droplist(parentPage);

                case WidgetType.ListBox:
                    return new ListBox(parentPage);

                case WidgetType.Checkbox:
                    return new Checkbox(parentPage);

                case WidgetType.RadioButton:
                    return new RadioButton(parentPage);

                case WidgetType.Button:
                    return new Button(parentPage);

                case WidgetType.SVG:
                    return new Svg(parentPage);

                default:
                    return null;
            }
        }

        internal static Widget CreateWidget(Page parentPage, string name)
        {
            WidgetType type = NameToWidgetType(name);

            if(type == WidgetType.None)
            {
                return null;
            }

            return CreateWidget(parentPage, type);
        }

        internal static WidgetType NameToWidgetType(string name)
        {
            string key = name.ToLower();
            if (_typeMap.ContainsKey(key))
            {
                return _typeMap[key];
            }

            return WidgetType.None;
        }

        private static readonly Dictionary<string, WidgetType> _typeMap = new Dictionary<string,WidgetType>()
        { 
            {"flowshape", WidgetType.FlowShape},
            {"shape", WidgetType.Shape},
            {"image", WidgetType.Image},
            {"dynamicpanel", WidgetType.DynamicPanel},
            {"hamburgermenu", WidgetType.HamburgerMenu},
            {"toast", WidgetType.Toast},
            {"line", WidgetType.Line},
            {"hotspot", WidgetType.HotSpot},
            {"textfield", WidgetType.TextField},
            {"textarea", WidgetType.TextArea},
            {"droplist", WidgetType.DropList},
            {"listbox", WidgetType.ListBox},
            {"checkbox", WidgetType.Checkbox},
            {"radiobutton", WidgetType.RadioButton},
            {"button", WidgetType.Button},
            {"svg", WidgetType.SVG},
        };
    }
}
