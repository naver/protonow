using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module.Styles
{
    class StylePageViewModel : ViewModelBase
    {
        public StylePageViewModel()
        {
            _ListEventAggregator.GetEvent<SelectionChangeEvent>().Subscribe(SelectionChangeEventHandler);
            _model = PropertyPageModel.GetInstance();
        }

        private void SelectionChangeEventHandler(string EventArg)
        {
            FirePropertyChanged("CanEdit");
            FirePropertyChanged("IsCommonWidget");
            FirePropertyChanged("IsHamburgerWidget");
        }

        public bool IsCommonWidget
        {
            get
            {
                return (_model.IsHamburgerWidget() == false);
            }
        }

        public bool IsHamburgerWidget
        {
            get
            {
                return _model.IsHamburgerWidget();
            }
        }
        public bool CanEdit
        {
            get
            {
                return _model.GetWidgetsNumber() > 0;
            }
        }
        PropertyPageModel _model;

    }
}
