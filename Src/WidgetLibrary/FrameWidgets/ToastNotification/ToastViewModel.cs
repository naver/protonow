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
    public class ToastViewModel : WidgetViewModBase
    {

        public ToastViewModel(IWidget widget)
        {
            _model = new ToastModel(widget,false);            
            _toast = widget as IToast;
            _bSupportBorder = false;
            _bSupportBackground = false;
            _bSupportText = false;
            _bSupportTextVerAlign = false;
            _bSupportTextHorAlign = false;
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = false;
            _bSupportTextRotate = false;

            IsChildPageOpened =(_model as ToastModel).IsAnyChildrenPageOpen();
            widgetGID = widget.Guid;
            Type = ObjectType.Toast;
            _ListEventAggregator.GetEvent<RefreshWidgetChildPageEvent>().Subscribe(RefreshWidgetPageUIHandler);
            _ListEventAggregator.GetEvent<CloseWidgetPageEvent>().Subscribe(CloseWidgetPageHandler);

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
            _ListEventAggregator.GetEvent<OpenWidgetPageEvent>().Publish(_toast);
            IsChildPageOpened = true;
        }

        /// <summary>
        /// Selected page changed in Sitmap or EditorView
        /// For getting current selected widget.
        /// </summary>
        private void RefreshWidgetPageUIHandler(Guid pageGuid)
        {
            if (pageGuid == Guid.Empty || pageGuid != _toast.Guid || _toast.ParentPage.IsOpened == false)
                return;

            FirePropertyChanged("Items");
        }

        private void CloseWidgetPageHandler(IWidget widget)
        {
            if (widget == _toast)
            {
                IsChildPageOpened = false;
            }
        }
        #endregion


        public override double Top
        {
            get
            {
                return base.Top;
            }
            set
            {
                base.Top = value;
                if (value != 0)
                {
                    DisplayPosition = ToastDisplayPosition.UserSetting;
                }
            }
        }

        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                return (_model as ToastModel).Items;
            }
        }

        public override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }
            set
            {
                if (base.IsSelected != value)
                {
                    if (SelectionService.GetCurrentPage() != this.ParentPageVM
                        && value==true)
                    {
                        return;
                    }

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

        public Visibility CloseButtonVisibility
        {
            get
            {
                if (CloseSetting == ToastCloseSetting.CloseButton)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public int ExposureTime
        {
            get
            {
                return (_model as ToastModel).ExposureTime;
            }
            set
            {
                if ((_model as ToastModel).ExposureTime != value)
                {
                    (_model as ToastModel).ExposureTime = value;
                    FirePropertyChanged("ExposureTime");
                }
            }
        }

        public ToastCloseSetting CloseSetting
        {
            get
            {
                return (_model as ToastModel).CloseSetting;
            }
            set
            {
                if((_model as ToastModel).CloseSetting!=value)
                {
                    (_model as ToastModel).CloseSetting = value;
                    FirePropertyChanged("CloseSetting");
                    FirePropertyChanged("CloseButtonVisibility");
                }
            }
        }

        public ToastDisplayPosition DisplayPosition
        {
            get
            {
                return (_model as ToastModel).DisplayPosition;
            }
            set
            {
                if((_model as ToastModel).DisplayPosition!=value)
                {
                    (_model as ToastModel).DisplayPosition = value;
                    FirePropertyChanged("DisplayPosition");
                }
            }
        }

        public IToast Toast
        {
            get
            {
                return _toast;
            }
        }
        #region private member
        private IToast _toast;
        #endregion

    }
}
