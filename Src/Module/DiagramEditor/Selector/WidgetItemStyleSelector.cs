using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Naver.Compass.InfoStructure;
using Naver.Compass.WidgetLibrary;

namespace Naver.Compass.Module
{

    class WidgetItemStyleSelector : StyleSelector
    {
        public Style WidgetNoRotateItemStyle
        {
            get;
            set;
        }

        public Style WidgetRotateItemStyle
        {
            get;
            set;
        }

        public Style WidgeRounedtRotateItemStyle
        {
            get;
            set;
        }

        public Style GroupItemStyle
        {
            get;
            set;
        }

        public Style VlineItemStyle
        {
            get;
            set;
        }

        public Style HlineItemStyle
        {
            get;
            set;
        }

        public Style HorReiszeItemStyle
        {
            get;
            set;
        }

        public Style MasterItemStyle
        {
            get;
            set;
        }

        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            try
            {
                if (item is MasterWidgetViewModel)
                    return MasterItemStyle;

                if (item is GroupViewModel)
                    return GroupItemStyle;

                if (item is VLineWidgetViewModel)
                    return VlineItemStyle;

                if (item is HLineWidgetViewModel)
                    return HlineItemStyle;

                if (item is CheckBoxWidgetViewModel ||
                            item is RadioButtonWidgetViewModel)
                    return HorReiszeItemStyle;

                if (item is HamburgerMenuViewModel)
                    return WidgetNoRotateItemStyle;

                if (item is RoundedRecWidgetViewModel)
                    return WidgeRounedtRotateItemStyle;

                if (item is WidgetRotateViewModBase)
                    return WidgetRotateItemStyle;
                
                if (item is WidgetViewModBase)
                    return WidgetNoRotateItemStyle;
            }
            catch (System.Exception ex)
            {
                string sz = ex.Message;
            }


            return base.SelectStyle(item, container);
        }

    }
}
