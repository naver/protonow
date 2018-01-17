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
    public class MasterPreViewModel 
    {
        public MasterPreViewModel(IMaster widget)           
        {
            //Infra Structure
            _model = new MasterModel(widget,true);
            //IsImgConvertType = false;
            (_model as MasterModel).LoadAllChildrenWidgets();
        }



        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                return (_model as MasterModel).Items;
            }
        }
        MasterModel _model;

        #region Override UpdateWidgetStyle2UI Functions
        //override protected void UpdateWidgetStyle2UI()
        //{
        //    base.UpdateWidgetStyle2UI();
        //    //UpdateTextStyle();
        //    //UpdateFontStyle();
        //    //UpdateBackgroundStyle();
        //}
        #endregion 

    }
}
