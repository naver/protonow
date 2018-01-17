using System;
using System.Windows;
using System.Windows.Controls;

namespace Naver.Compass.Module
{
    // Implements ItemsControl for ToolboxItems    
    public class Toolbox : ItemsControl
    {
        // Defines the ItemHeight and ItemWidth properties of
        // the WrapPanel used for this Toolbox
        //public Size ItemSize
        //{
        //    get { return itemSize; }
        //    set { itemSize = value; }
        //}
        //private Size itemSize = new Size(75, 45);
        private readonly Style CustomWS;
        private readonly Style SystemWS;
        public Toolbox()
        {
            var themeStyles = new ResourceDictionary
            {
                Source = new Uri(
                        @"/Naver.Compass.Module.WidgetLibrary;component/Resources/ToolboxItem.xaml",
                        UriKind.Relative)
            };

            CustomWS = themeStyles["CustomWidgetStyle"] as Style;
            SystemWS = themeStyles["SystemWidgetStyle"] as Style;
        }

        // Creates or identifies the element that is used to display the given item.        
        protected override DependencyObject GetContainerForItemOverride()
        {
            var toolboxItem = new ToolboxItem();
            if (this.DataContext is WidgetExpand)
            {
                var expand = this.DataContext as WidgetExpand;

                if (expand.IsCustomWidget)
                {
                    toolboxItem.Style = CustomWS;
                }
                else
                {
                    toolboxItem.Style = SystemWS;
                }
            }

            return toolboxItem;
        }

        // Determines if the specified item is (or is eligible to be) its own container.        
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ToolboxItem);
        }
    }
}
