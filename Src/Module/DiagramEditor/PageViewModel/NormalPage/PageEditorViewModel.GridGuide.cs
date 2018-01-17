using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel
    {

        private void UpdateGridGuideHandler(GridGuideType type)
        {
            if (type == GridGuideType.All)
            {
                FireGrid();
                FireGuide();
            }
        }
        public bool IsGridVisible
        {
            get { return (EditorScale >= 0.5) && GlobalData.IsShowGrid; }
            set
            {
                if (GlobalData.IsShowGrid != value)
                {
                    GlobalData.IsShowGrid = value;
                    FirePropertyChanged("IsGridVisible");
                }
            }
        }

        public bool IsPageGuideVisible
        {
            get { return GlobalData.IsShowPageGuide; }
            set
            {
                if (GlobalData.IsShowPageGuide != value)
                {
                    GlobalData.IsShowPageGuide = value;
                    FirePropertyChanged("IsPageGuideVisible");
                }
            }
        }

        public bool IsGlobalGuideVisible
        {
            get { return GlobalData.IsShowGlobalGuide; }
            set
            {
                if (GlobalData.IsShowGlobalGuide != value)
                {
                    GlobalData.IsShowGlobalGuide = value;
                    FirePropertyChanged("IsGlobalGuideVisible");
                }
            }
        }

        //The value does not matter, just to trigger render
        public bool IsTriggerRender { get; set; }

        private void FireGrid()
        {
            IsTriggerRender = !IsTriggerRender;
            FirePropertyChanged("IsTriggerRender");
            FirePropertyChanged("IsGridVisible");
        }
        private void FireGuide()
        {
            FirePropertyChanged("IsPageGuideVisible");
            FirePropertyChanged("IsGlobalGuideVisible");
        }

    }
}
