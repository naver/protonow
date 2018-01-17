using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using Naver.Compass.Common.Helper;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;
using Naver.Compass.Common.CommonBase;
using System.Collections.ObjectModel;

namespace Naver.Compass.WidgetLibrary
{
    public class MasterWidgetPreViewModel : WidgetPreViewModeBase
    {
        public MasterWidgetPreViewModel(IMaster master)
            : base(master)
        {
            _model = new MasterWidgetModel(master, false);
        }

        #region Public Bingding Property
        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                return (_model as MasterWidgetModel).Items;
            }
        }
        public override double ItemHeight
        {
            get
            {
                double height = 0, Y = Double.MaxValue;
                foreach (WidgetPreViewModeBase item in Items)
                {
                    Rect BoundRect = item.GetBoundingRectangle();
                    double h = BoundRect.Y + BoundRect.Height;
                    height = Math.Max(height, h);
                    Y = Math.Min(Y, BoundRect.Y);
                }
                if (Y == Double.MaxValue)
                    return 0;
                return height - Y;
            }
            set
            {
                FirePropertyChanged("ItemHeight");
                return;
            }
        }
        public override double ItemWidth
        {
            get
            {
                double width = 0, X = Double.MaxValue;
                foreach (WidgetPreViewModeBase item in Items)
                {
                    Rect BoundRect = item.GetBoundingRectangle();
                    double w = BoundRect.X + BoundRect.Width;
                    width = Math.Max(width, w);
                    X = Math.Min(X, BoundRect.X);
                }
                if (X == Double.MaxValue)
                    return 0;
                return width - X;
            }
            set
            {
                FirePropertyChanged("ItemWidth");
                return;
            }
        }
        public Thickness CanvasMargin
        {
            get
            {
                double X = Double.MaxValue, Y = Double.MaxValue;
                foreach (WidgetPreViewModeBase item in Items)
                {
                    Rect BoundRect = item.GetBoundingRectangle();
                    X = Math.Min(X, BoundRect.X);
                    Y = Math.Min(Y, BoundRect.Y);
                }
                return new Thickness(-X, -Y, 0, 0);
            }
            set
            {
                FirePropertyChanged("CanvasMargin");
                return;
            }
        }
        public Visibility DoubleClickVisibility
        {
            get
            {
                return Visibility.Collapsed;
            }
        }
        public override void ChangeCurrentPageView(IPageView targetPageView)
        {
            if (_model == null || targetPageView == null)
            {
                return;
            }

            //if (targetPageView.Guid == GetCurrentPageViewGID())
            //{
            //    return ;
            //}     

            if (true == targetPageView.Masters.Contains(_model.Guid))
            {
                IsShowInPageView2Adaptive = true;
            }
            else
            {
                IsShowInPageView2Adaptive = false;
            }

            bool bRes = _model.ChangeCurrentStyle(targetPageView.Guid);
            if (bRes == true)
            {
                UpdateWidgetStyle2UI();
            }
        }
        #endregion

        #region Override UpdateWidgetStyle2UI Functions
        override protected void UpdateWidgetStyle2UI()
        {
            FirePropertyChanged("CanvasMargin");
            FirePropertyChanged("IsHidden");
            FirePropertyChanged("Left");
            FirePropertyChanged("Top");
            FirePropertyChanged("ItemWidth");
            FirePropertyChanged("ItemHeight");
            FirePropertyChanged("ZOrder");
            FirePropertyChanged("RotateAngle");
            FirePropertyChanged("IsFixed");
            
            //base.UpdateWidgetStyle2UI();
            //UpdateTextStyle();
            //UpdateFontStyle();
            //UpdateBackgroundStyle();
        }
        #endregion 
    }
}
