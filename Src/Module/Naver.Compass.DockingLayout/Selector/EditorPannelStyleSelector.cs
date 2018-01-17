
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

using Naver.Compass.Module;

namespace DockingLayout
{
    class EditorPannelStyleSelector : StyleSelector
    {
        public Style PageEditorStyle
        {
            get;
            set;
        }

        public Style NoPageViewStyle
        {
            get;
            set;
        }

        public Style DynamicPageEditorStyle
        {
            get;
            set;
        }

        public Style HamburgerPageEditorStyle
        {
            get;
            set;
        }
        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            try
            {
                if (item is DynamicPageEditorViewModel)
                    return DynamicPageEditorStyle;

                if (item is HamburgerPageEditorViewModel)
                    return HamburgerPageEditorStyle;

                if (item is ToastPageEditorViewModel)
                    return HamburgerPageEditorStyle;

                if (item is PageEditorViewModel)
                    return PageEditorStyle;

                if (item is NoPageViewModel)
                    return NoPageViewStyle;
            }
            catch (System.Exception ex)
            {
                string sz = ex.Message;
            }


            return base.SelectStyle(item, container);
        }

    }
}
