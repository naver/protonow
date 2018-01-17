using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Naver.Compass.WidgetLibrary
{
    public class DroplistWidgetViewModel : ListBaseWidgetViewModel
    {
        public DroplistWidgetViewModel(IWidget widget)
            : base(widget)
        {
            Type = ObjectType.DropList;
        }

        #region Override UpdateWidgetStyle2UI Functions
        override protected void UpdateWidgetStyle2UI()
        {
            base.UpdateWidgetStyle2UI();
            UpdateTextStyle();
            UpdateFontStyle();
            UpdateBackgroundStyle();
        }
        #endregion 

    }
}
