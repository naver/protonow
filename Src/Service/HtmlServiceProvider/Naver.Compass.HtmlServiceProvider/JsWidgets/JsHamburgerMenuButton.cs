using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsHamburgerMenuButton : JsImage
    {
        public JsHamburgerMenuButton(HtmlServiceProvider service, IWidget widget)
            : base(service, widget)
        {
        }
                
        protected override string DefaultResourceName
        {
            get { return @"Naver.Compass.Service.Html.Res.17_Hamburger_Select.svg"; }
        }
    }
}
