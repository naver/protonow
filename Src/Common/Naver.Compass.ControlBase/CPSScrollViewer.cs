using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Naver.Compass.Common
{
   public class CPSScrollViewer : ScrollViewer
    {

       public CPSScrollViewer()
       {
           this.PreviewMouseWheel += CPSScrollViewer_PreviewMouseWheel;
       }

       /// <summary>
       /// Shift + mouse wheel to scroll horizontally .
       /// </summary>
       void CPSScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
       {
           ScrollViewer scrollview = sender as ScrollViewer;

           if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
           {
               if (e.Delta > 0)
                   scrollview.LineLeft();
               else
                   scrollview.LineRight();
               e.Handled = true;
           }   
       }
       protected override void OnKeyDown(KeyEventArgs e)
       {
           if (e.Key == Key.Home || e.Key == Key.End)
           {
               if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift
                   && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
               {
                   e.Handled = true;
                   return;
               }

           }

           base.OnKeyDown(e);
       }
       
    }
}
