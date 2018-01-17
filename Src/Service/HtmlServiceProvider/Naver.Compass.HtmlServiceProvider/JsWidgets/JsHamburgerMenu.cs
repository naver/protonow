using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsHamburgerMenu : JsWidget
    {
        public JsHamburgerMenu(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }

        protected override void AppendSpecificTypeProperties(StringBuilder builder)
        {
            IHamburgerMenu menu = _widget as IHamburgerMenu;
            builder.AppendFormat("\"direction\":\"{0}\",", menu.HiddenOn.ToString().ToLower());

            bool isClosedPage = false;
            if (!menu.MenuPage.IsOpened)
            {
                isClosedPage = true;
                menu.MenuPage.Open();
            }


            if (IsSetMD5 == true)
            {
                builder.AppendFormat("\"Content\":\"{0}\",", menu.MenuPage.MD5);
            }
            builder.Append("\"menuWidgets\":[");
            foreach (IWidget widget in menu.MenuPage.Widgets)
            {
                JsWidget jsWidget = JsWidgetFactory.CreateJsWidget(_service, widget, IsSetMD5);
                builder.Append(jsWidget.ToString());
                builder.Append(",");
            }

            if (isClosedPage)
            {
                menu.MenuPage.Close();
            }

            JsHelper.RemoveLastComma(builder);
            builder.Append("],");

            // menu button
            IHamburgerMenuButton button = menu.MenuButton;
            JsHamburgerMenuButton jsButton = new JsHamburgerMenuButton(_service, button);
            builder.Append("\"buttonWidget\":");
            builder.Append(jsButton.ToString());
            builder.Append(",");
        }
    }
}
