using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.InfoStructure
{
    public  class WidgetPreViewModeBase : WidgetViewModelDate
    {
        public WidgetPreViewModeBase(IWidget widget)
        {
            IniCreateDataModel(widget);
            IsHiddenInvalid = false;
        }
        public WidgetPreViewModeBase(IMaster master)
        {
            IniCreateDataModel(master);
            IsHiddenInvalid = false;
        }
        #region Initialize Function
        virtual  protected void IniCreateDataModel(IRegion obj)
        {
            if(obj is IWidget)
                _model = new WidgetModel(obj as IWidget);
            else   
                _model = new MasterModel(obj as IMaster);
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

        #region Help Function
        virtual public Rect GetBoundingRectangle(bool isActual = true)
        {
            double x1 = Double.MaxValue;
            double y1 = Double.MaxValue;
            double x2 = Double.MinValue;
            double y2 = Double.MinValue;

            double itemWidth, itemHeight;
            if (isActual)
            {

                itemWidth = this.Raw_ItemWidth;
                itemHeight = this.Raw_ItemHeight;
            }
            else
            {
                itemWidth = this.ItemWidth;
                itemHeight = this.ItemHeight;
            }

            if (this.RotateAngle == 0)
            {
                x1 = this.Raw_Left;
                y1 = this.Raw_Top;
                x2 = this.Raw_Left + itemWidth;
                y2 = this.Raw_Top + itemHeight;
            }
            else
            {
                double x = this.Raw_Left;
                double y = this.Raw_Top;
                double xc = (this.Raw_Left * 2 + itemWidth) / 2;
                double yc = (this.Raw_Top * 2 + itemHeight) / 2;
                double angle = Math.Abs(this.RotateAngle) % 180;
                if (angle > 90)
                {
                    angle = 180 - angle;
                }
                double Kc = Math.Cos(angle * Math.PI / 180);
                double Ks = Math.Sin(angle * Math.PI / 180);

                double xr = xc - (Kc * (xc - x) + Ks * (yc - y));
                double yr = yc - (Ks * (xc - x) + Kc * (yc - y));
                double width = itemWidth + (x - xr) * 2;
                double height = itemHeight + (y - yr) * 2;

                x1 = xr;
                y1 = yr;
                x2 = xr + width;
                y2 = yr + height;
            }
            return new Rect(new System.Windows.Point(x1, y1), new System.Windows.Point(x2, y2));
        }
        #endregion
    }
}
