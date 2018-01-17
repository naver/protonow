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

namespace Naver.Compass.WidgetLibrary
{
    public class HamburgerMenuViewModel : ImageWidgetViewModel
    {

        public HamburgerMenuViewModel(IWidget widget)
        {
            _model = new HamburgerMenuModel(widget);
            _bSupportBorder = false;
            _bSupportBackground = false;
            _bSupportText = false;
            _bSupportTextVerAlign = false;
            _bSupportTextHorAlign = false;
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = false;
            _bSupportTextRotate = false;

            IsChildPageOpened = (_model as HamburgerMenuModel).IsAnyChildrenPageOpen();
            Widget = widget as IHamburgerMenu;
            Widget.MenuPage.Open();

            _imageStream = (_model as HamburgerMenuModel).ImageStream;
            widgetGID = widget.Guid;
            Type = ObjectType.HamburgerMenu;

            _ListEventAggregator.GetEvent<CloseWidgetPageEvent>().Subscribe(CloseWidgetPageHandler);

        }
        #region Widget Routed Event Handler by Command
        private void CloseWidgetPageHandler(IWidget widget)
        {
            if(widget == Widget)
            {
                IsChildPageOpened = false;
            }
        }
        #endregion
        #region Property
        public override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }
            set
            {
                if(value == false)
                {
                    if(SelectionService.GetCurrentPage()!=null)
                    {
                        SelectionService.GetCurrentPage().CancelEditHamburgerPage();
                    }                    
                }
                Raw_IsSelected = value;
            }
        }
        public override bool Raw_IsSelected
        {
            set
            {
                if (bIsSelected != value)
                {
                    bIsSelected = value;
                    if (bIsSelected == false)
                    {
                        CanEdit = false;
                        IsTarget = false;
                        SelectionService.RemoveWidget(this);
                    }
                    else
                    {
                        SelectionService.RegisterWidget(this);
                    }
                    FirePropertyChanged("IsSelected");
                }
            }
        }

        #endregion

        public void SetEditMode()
        {
            SelectionService.GetCurrentPage().EditHanburgerPage();
        }

        #region Binding Propery
        public double MenuPageLeft
        {
            get
            {
                return (_model as HamburgerMenuModel).MenuPageLeft;
            }
            set
            {
                if ((_model as HamburgerMenuModel).MenuPageLeft != value)
                {
                    (_model as HamburgerMenuModel).MenuPageLeft = value;
                    FirePropertyChanged("MenuPageLeft");
                }
            }
        }

        public double MenuPageTop
        {
            get
            {
                return (_model as HamburgerMenuModel).MenuPageTop;
            }
            set
            {
                if ((_model as HamburgerMenuModel).MenuPageTop != value)
                {
                    (_model as HamburgerMenuModel).MenuPageTop = value;
                    FirePropertyChanged("MenuPageTop");
                }
            }
        }
        public double MenuPageWidth
        {
            get
            {
                return (_model as HamburgerMenuModel).MenuPageWidth;
            }
            set
            {
                if ((_model as HamburgerMenuModel).MenuPageWidth != value)
                {
                    (_model as HamburgerMenuModel).MenuPageWidth = value;
                    FirePropertyChanged("MenuPageWidth");
                }
            }
        }
        public double MenuPageHeight
        {
            get
            {
                return (_model as HamburgerMenuModel).MenuPageHeight;
            }
            set
            {
                if ((_model as HamburgerMenuModel).MenuPageHeight != value)
                {
                    (_model as HamburgerMenuModel).MenuPageHeight = value;
                    FirePropertyChanged("MenuPageHeight");
                }
            }
        }

        public bool IsMenuPageHidden
        {
            get
            {
                return !(_model as HamburgerMenuModel).IsMenuPageVisible;
            }
            set
            {
                if ((_model as HamburgerMenuModel).IsMenuPageVisible != !value)
                {
                    (_model as HamburgerMenuModel).IsMenuPageVisible = !value;
                    FirePropertyChanged("IsMenuPageHidden");
                }
            }
        }

        public HiddenOn HiddenOn
        {
            get
            {
                return (_model as HamburgerMenuModel).HiddenOn;
            }
            set
            {
                if ((_model as HamburgerMenuModel).HiddenOn != value)
                {
                    (_model as HamburgerMenuModel).HiddenOn = value;
                    FirePropertyChanged("HiddenOn");
                }
            }
        }

        public override bool IsFixed
        {
            get
            {
                return (_model as HamburgerMenuModel).IsFixed;
            }

            set
            {
                if ((_model as HamburgerMenuModel).IsFixed != value)
                {
                    (_model as HamburgerMenuModel).IsFixed = value;
                    FirePropertyChanged("IsFixed");
                }
            }
        }
        public IHamburgerMenu Widget { get; set; }
        #endregion

    }
}
