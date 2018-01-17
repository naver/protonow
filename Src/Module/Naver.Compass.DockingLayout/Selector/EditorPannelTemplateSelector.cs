using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using Naver.Compass.Module;

namespace DockingLayout
{
    class EditorPannelTemplateSelector : DataTemplateSelector
    {
        public EditorPannelTemplateSelector()
        {

        }

        public DataTemplate PageEditorTemplate
        {
            get;
            set;
        }

        public DataTemplate NoPageViewTemplate
        {
            get;
            set;
        }

        public DataTemplate DynamicPageEditorTemplate
        {
            get;
            set;
        }

        public DataTemplate HamburgerPageEditorTemplate
        {
            get;
            set;
        }
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is DynamicPageEditorViewModel)
                return DynamicPageEditorTemplate;

            if (item is HamburgerPageEditorViewModel)
                return HamburgerPageEditorTemplate;

            if (item is ToastPageEditorViewModel)
                return HamburgerPageEditorTemplate;

            //DynamicPageEditorViewModel HamburgerPageEditorViewModel ToastPageEditorViewModel is derived from PageEditorViewModel
            //So PageEditorViewModel is last one
            if (item is PageEditorViewModel)
                return PageEditorTemplate;

            if (item is NoPageViewModel)
                return NoPageViewTemplate;

            return base.SelectTemplate(item, container);
        }

    }
}
