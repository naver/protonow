using Naver.Compass.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DockingLayout
{
    class EditorHeaderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PageEditorHeaderTemplate
        {
            get;
            set;
        }

        public DataTemplate NoPageHeaderTemplate
        {
            get;
            set;
        }

        public DataTemplate DynamicPageHeaderTemplate
        {
            get;
            set;
        }

        public DataTemplate HamburgerPageHeaderTemplate
        {
            get;
            set;
        }
        public DataTemplate MasterPageHeaderTemplate
        {
            get;
            set;
        }
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var doc = item as Xceed.Wpf.AvalonDock.Layout.LayoutDocument;

            if (doc != null)
            {
                if (doc.Content is MasterPageEditorViewModel)
                    return MasterPageHeaderTemplate;

                if (doc.Content is DynamicPageEditorViewModel)
                    return DynamicPageHeaderTemplate;

                if (doc.Content is HamburgerPageEditorViewModel)
                    return HamburgerPageHeaderTemplate;

                if (doc.Content is ToastPageEditorViewModel)
                    return HamburgerPageHeaderTemplate;

                if (doc.Content is PageEditorViewModel)
                    return PageEditorHeaderTemplate;

                if (doc.Content is NoPageViewModel)
                    return NoPageHeaderTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
