using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.InfoStructure;
using System.Diagnostics;

namespace Naver.Compass.Module.PreviewModule
{
    //This Canvas is just for Preview and Converting Image
    public class PreviewPageCanvas : Canvas
    {      
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();
            try
            {
                if (Children.Count<=0)
                {
                    size.Width = 0;
                    size.Height = 0;
                    return size;
                }
                foreach (UIElement element in Children)
                {
                    double left = Canvas.GetLeft(element);
                    double top = Canvas.GetTop(element);
                    left = double.IsNaN(left) ? 0 : left;
                    top = double.IsNaN(top) ? 0 : top;

                    element.Measure(constraint);

                    Size desiredSize = element.DesiredSize;
                    if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                    {
                        size.Width = Math.Max(size.Width, left + desiredSize.Width);
                        size.Height = Math.Max(size.Height, top + desiredSize.Height);
                    }
                    //Debug.WriteLine("@@@@@ Single SIZE: W:" + size.Width+ " H:"+ size.Height);

                }
            
                // add some extra mar
                size.Width += 10;
                size.Height += 10;
                //Debug.WriteLine("@@@@@@@@@@ Measure SIZE: W:" + size.Width + " H:" + size.Height);
                return size;
            }
            catch
            {
                size.Width = 0;
                size.Height = 0;
                return size;
            }            
        }
    }
}
