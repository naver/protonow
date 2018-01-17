using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.InfoStructure
{
    public  class WidgetPreViewModeBase : WidgetViewModelDate
    {
        public WidgetPreViewModeBase(IWidget widget)
        {
            IniCreateDataModel(widget);
            IsHiddenInvalid = false;
        }

        #region Initialize Function
        virtual  protected void IniCreateDataModel(IWidget widget)
        {
            _model = new WidgetModel(widget);
        }
        #endregion

        #region Public Proprerty
        public bool IsImgConvertType 
        { 
            get;
            set;
        }        
        public bool IsHiddenInvalid
        {
            get;
            set;
        }  
        public bool CanEdit
        {
            get
            {
                return false;
            }
            set
            {
                //FirePropertyChanged("CanEdit");
            }
        }
        #endregion

        #region Overrid Proprerty
        override public String Name
        {
            get
            {
                if (IsImgConvertType == true)
                {
                    return _model.Guid.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public override bool IsHidden
        {
            get
            {
                if (IsHiddenInvalid == true)
                {
                    return false;
                }
                return base.IsHidden;
            }
            set
            {
                //FirePropertyChanged("IsHidden");
            }
        }
        #endregion      
    }
}
