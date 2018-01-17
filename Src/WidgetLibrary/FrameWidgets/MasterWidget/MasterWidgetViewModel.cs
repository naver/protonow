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
    public class MasterWidgetViewModel : EmbedWidgetViewModBase
    {
        public MasterWidgetViewModel(IMaster master)
        {
            _model = new MasterWidgetModel(master, false);
            _bSupportBorder = false;
            _bSupportBackground = false;
            _bSupportText = false;
            _bSupportTextVerAlign = false;
            _bSupportTextHorAlign = false;
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = false;
            _bSupportTextRotate = false;

            widgetGID = master.Guid;
            _masterPageID = master.MasterPageGuid;
            Type = ObjectType.Master;
            
            //MASTER TOD:
            //_ListEventAggregator.GetEvent<RefreshWidgetChildPageEvent>().Subscribe(RefreshWidgetPageUIHandler);

        }

        #region Widget Routed Event Handler by Command
        private DelegateCommand<object> _doubleClickCommand = null;
        override public ICommand DoubleClickCommand
        {
            get
            {
                if (_doubleClickCommand == null)
                {
                    _doubleClickCommand = new DelegateCommand<object>(OnDoubleClick);
                }
                return _doubleClickCommand;
            }
        }
        private void OnDoubleClick(object obj)
        {
            //MASTER TOD:
            Guid masterpageID = (_model as MasterWidgetModel).ChildPageID;
            _ListEventAggregator.GetEvent<OpenMasterPageEvent>().Publish(masterpageID);
            IsChildPageOpened = true;
        }   
        #endregion

        #region private member
        private Guid  _masterPageID;
        #endregion

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
                    if (item.IsShowInPageView2Adaptive == false)
                    {
                        continue;
                    }

                    Rect BoundRect = item.GetBoundingRectangle();
                    double h = BoundRect.Y + BoundRect.Height;
                    height = Math.Max(height,h );
                    Y = Math.Min(Y, BoundRect.Y);
                }
                if (Y == Double.MaxValue)
                    return 0;
                return height-Y;
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
                    if(item.IsShowInPageView2Adaptive==false)
                    {
                        continue;
                    }
                    Rect BoundRect = item.GetBoundingRectangle();
                    double w = BoundRect.X + BoundRect.Width;
                    width = Math.Max(width, w);
                    X = Math.Min(X, BoundRect.X);
                }
                if (X == Double.MaxValue)
                    return 0;
                return width-X;
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
                    if (item.IsShowInPageView2Adaptive == false)
                    {
                        continue;
                    }

                    Rect BoundRect = item.GetBoundingRectangle();
                    X = Math.Min(X, BoundRect.X);   
                    Y = Math.Min(Y, BoundRect.Y);
                }
                return new Thickness(-X,-Y,0,0);
            }
            set
            {
                FirePropertyChanged("CanvasMargin");
                return;
            }
        }
        public  override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }
            set
            {
                if (base.IsSelected != value)
                {
                    base.IsSelected = value;
                    FirePropertyChanged("DoubleClickVisibility");
                }
            }
        }
        public Visibility DoubleClickVisibility
        {
            get
            {
                if (IsSelected)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public override Guid EmbedePagetGUID
        {
            get
            {
                return _masterPageID;
            }  
        }
        public override bool IsLocked
        {
            get { return base.IsLocked; }
            set
            {
                if (_model.IsLocked != value)
                {
                    if (IsLocked2MasterLocation == true && value==false)
                    {
                        return;
                    }

                    _model.IsLocked = value;
                    FirePropertyChanged("IsLocked");
                }
            }
        }
        public override bool CanDragResize 
        {
            get { return false; }
        }
        public bool IsLocked2MasterLocation
        {
            get 
            { 
                return (_model as MasterWidgetModel).IsLockedToMasterLocation;
            }
            set
            {

                if ((_model as MasterWidgetModel).IsLockedToMasterLocation!= value)
                {                    
                    (_model as MasterWidgetModel).IsLockedToMasterLocation = value;
                    FirePropertyChanged("IsLocked2MasterLocation");

                    if (value == true)
                    {
                        IsLocked = true;
                    }
                    else
                    {
                        IsLocked = false;
                    }
                }
            }
        }
        #endregion

       
        #region Public interface         
        public void RefreshUI()
        {
            UpdateMasteLockedPos();
            FirePropertyChanged("Left");
            FirePropertyChanged("Top");
            FirePropertyChanged("ItemHeight");
            FirePropertyChanged("ItemWidth");
            FirePropertyChanged("CanvasMargin");
            FirePropertyChanged("Items");
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
        public double MasterLockedLocationX
        {
            get 
            {
                return (_model as MasterWidgetModel).MasterLockedLocationX;
            }
        }
        public double MasterLockedLocationY
        {
            get
            {
                return (_model as MasterWidgetModel).MasterLockedLocationY;
            }
        }
        public void ForceSetX(Double X)
        {
            _model.Left = X;
            FirePropertyChanged("Left");

        }
        public void ForceSetY(Double Y)
        {
            _model.Top = Y;
            FirePropertyChanged("Top");
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

        #region private functions
        private void UpdateMasteLockedPos()
        {
            if(IsLocked2MasterLocation==false)
            {
                return;
            }

            ForceSetX((_model as MasterWidgetModel).MasterLockedLocationX);
            ForceSetY((_model as MasterWidgetModel).MasterLockedLocationY);
        }
        #endregion
    }
}
