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
    public class MasterViewModel : WidgetViewModBase
    {

        public MasterViewModel(IMaster master)
        {
            _masterModel = new MasterModel(master, false);
            _master = master as IMaster;
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
            WidgetTypeName = @"Master";
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
            //_ListEventAggregator.GetEvent<OpenWidgetPageEvent>().Publish(_master);
            IsChildPageOpened = true;
        }

        /// <summary>
        /// Selected page changed in Sitmap or EditorView
        /// For getting current selected widget.
        /// </summary>
        private void RefreshWidgetPageUIHandler(Guid pageGuid)
        {
            if (pageGuid == Guid.Empty || pageGuid != _master.Guid || _master.ParentPage.IsOpened == false)
                return;

            FirePropertyChanged("Items");
        }

        private void CloseWidgetPageHandler(IWidget widget)
        {
            if (widget == _master)
            {
                IsChildPageOpened = false;
            }
        }
        #endregion



        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                return (_masterModel as MasterModel).Items;
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

        public IMaster Master
        {
            get
            {
                return _master;
            }
        }
        #region private member
        private IMaster _master;
        private MasterModel _masterModel;
        #endregion

    }
}
