using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Controls.Ribbon;
using System.Windows;

namespace MainToolBar
{
   public class CPSRibbonGallery:RibbonGallery
    {
       protected override void OnSelectionChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            e.Handled = IsExcuteAfterUpdate;
            base.OnSelectionChanged(e);
        }

       public bool IsExcuteAfterUpdate
       {
           get { return (bool)GetValue(IsExcuteAfterUpdateProperty); }
           set { SetValue(IsExcuteAfterUpdateProperty, value); }
       }

       public static readonly DependencyProperty IsExcuteAfterUpdateProperty =
          DependencyProperty.Register(
          "IsExcuteAfterUpdate",
          typeof(bool),
          typeof(CPSRibbonGallery),
          new PropertyMetadata(false));

    }
}
