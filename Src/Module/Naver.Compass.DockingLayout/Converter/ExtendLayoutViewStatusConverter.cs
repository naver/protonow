using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout;

namespace Naver.Compass.Module.Converter
{
    public class ExtendLayoutViewStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Length == 2 && values[0] is LayoutDocument)
            {
                var doc = values[0] as LayoutDocument;
                if (doc.Content == values[1])
                {
                    ///the extend page is selected
                    return 1;
                }

                if (doc.Parent != null
                        && doc.Parent.Children != null
                        && doc.Parent.ChildrenCount != 0
                        && (doc.Content is DynamicPageEditorViewModel
                            || doc.Content is HamburgerPageEditorViewModel
                            || doc.Content is ToastPageEditorViewModel)
                        )
                {
                    ILayoutElement parentIDoc = null;
                    try
                    {
                        parentIDoc = doc.Parent.Children.ToList().Where(c =>
                                    {
                                        if (c is LayoutDocument)
                                        {
                                            var cld = c as LayoutDocument;
                                            if (cld.Content is PageEditorViewModel)
                                            {
                                                var cpevm = cld.Content as PageEditorViewModel;
                                                if (cpevm.items != null)
                                                {
                                                    return cpevm.items.ToList().Any(i => i.widgetGID.ToString() == doc.ContentId);
                                                }
                                            }
                                        }

                                        return false;
                                    }).FirstOrDefault();
                    }
                    catch
                    {
                    }
                    var parentDoc = parentIDoc as LayoutDocument;
                    if (parentDoc != null && parentDoc.Content == values[1])
                    {
                        ///parent page is selected
                        return 2;
                    }
                }
            }

            ///normal status
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
