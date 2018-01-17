using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsWidgetFactory
    {

        private static JsWidget CreateJsWdg(HtmlServiceProvider service, IWidget widget)
        {
            switch(widget.WidgetType)
            {
                case WidgetType.FlowShape:
                case WidgetType.Shape:
                    {
                        return new JsShape(service, widget);
                    }
                case WidgetType.Image:
                    {
                        return new JsImage(service, widget);
                    }
                case WidgetType.TextField:
                case WidgetType.TextArea:
                    {
                        return new JsTextBase(service, widget);
                    }
                case WidgetType.DropList:
                case WidgetType.ListBox:
                    {
                        return new JsListBase(service, widget);
                    }
                case WidgetType.Checkbox:
                case WidgetType.RadioButton:
                    {
                        return new JsSelectionButton(service, widget);
                    }
                case WidgetType.Button:
                    {
                        return new JsSimpleTextWidget(service, widget);
                    }
                case WidgetType.SVG:
                    {
                        return new JsSvg(service, widget);
                    }
                case WidgetType.DynamicPanel:
                    {
                        return new JsDynamicPanel(service, widget);
                    }
                case WidgetType.HamburgerMenu:
                    {
                        return new JsHamburgerMenu(service, widget);
                    }
                case WidgetType.Toast:
                    {
                        return new JsToast(service, widget);
                    }
                case WidgetType.Line:
                    {
                        return new JsLine(service, widget);
                    }
                default:
                    return new JsWidget(service, widget);
            }
        }


        public static JsWidget CreateJsWidget(HtmlServiceProvider service, IWidget widget,bool ByMD5)
        {
            JsWidget wdg = CreateJsWdg(service, widget);
            wdg.IsSetMD5 = ByMD5;
            return wdg;
        }
    }
}
