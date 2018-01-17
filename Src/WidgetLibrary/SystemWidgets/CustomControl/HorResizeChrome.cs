using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Naver.Compass.WidgetLibrary
{
    public class HorResizeChrome : BaseChrome
    {
        static HorResizeChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HorResizeChrome), new FrameworkPropertyMetadata(typeof(HorResizeChrome)));
        }

        //protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);
        //    object obj = this.DataContext;
        //}
    }
}
