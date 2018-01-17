using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Collections.ObjectModel;

namespace Naver.Compass.WidgetLibrary
{
    public class ToastPreViewModel : WidgetPreViewModeBase
    {
        public ToastPreViewModel(IWidget widget)
            : base(widget)
        {
            //Infra Structure
            _model = new ToastModel(widget,true);
            IsImgConvertType = false;
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

        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                return (_model as ToastModel).Items;
            }
        }

        #region Override UpdateWidgetStyle2UI Functions
        override protected void UpdateWidgetStyle2UI()
        {
            base.UpdateWidgetStyle2UI();
            //UpdateTextStyle();
            //UpdateFontStyle();
            //UpdateBackgroundStyle();
        }
        #endregion 

    }
}
