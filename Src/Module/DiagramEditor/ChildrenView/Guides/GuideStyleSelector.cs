using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Naver.Compass.Module
{
    class GuideStyleSelector : StyleSelector
    {

        public Style HorizontalGuideStyle
        {
            get;
            set;
        }
        public Style VerticalGuidesStyle
        {
            get;
            set;
        }
        public override Style SelectStyle(object item, DependencyObject container)
        {
            try
            {
                if (item is HorizontalGuideLine)
                    return HorizontalGuideStyle;

                if (item is VerticalGuideLine)
                    return VerticalGuidesStyle;
            }
            catch (System.Exception ex)
            {
                string sz = ex.Message;
            }
            return base.SelectStyle(item, container);
        }
    }
}
