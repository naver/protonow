using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.ComponentModel;
using Naver.Compass.Common.Helper;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Module
{
    class MasterListItem : WidgetListItem
    {

        public MasterListItem(IMaster data)
            : base(data)
        {
        }


        override public int zOrder
        {
            get
            {
                return ((IMaster)_Mode).MasterStyle.Z;
            }
            set { System.Diagnostics.Debug.Assert(false, "Error option to set LostFlag"); }
        }

        override public string WidgetTypeName
        {
            get
            {
                string sTittle = GlobalData.FindResource("Master_Title"); ;
                return "(" +sTittle + ")";

               // return "(" + sTittle + "-" + zOrder.ToString() + ")";
            }
        }

        override public bool PlaceFlag
        {
            get
            {
                try
                {
                    if (ParentPage != null)
                    {
                        IPageView CurrentView = ParentPage.PageViews.GetPageView(CurViewID);
                        if (CurrentView != null && CurrentView.Masters.Contains(WidgetID))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    NLogger.Info("Get PlaceFlag cause excption:" + ex.Message.ToString());
                    return true;
                }
            }
        }

        override public bool LostFlag
        {
            get
            {
                if (PlaceFlag)
                {
                    if (ParentPage != null && ParentPage.PageViews != null)
                    {
                        foreach (IPageView childView in ParentPage.PageViews)
                        {
                            if (childView.Masters.Contains(WidgetID))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        override public bool HideFlag
        {
            get
            {
                System.Diagnostics.Debug.Assert(ParentPage != null, "Count Place Flag->ParentPage is null");

                IPageView CurrentView = ParentPage.PageViews.GetPageView(CurViewID);

                System.Diagnostics.Debug.Assert(CurrentView != null, "Count Place Flag->CurrentView is null");

                return !((IMaster)_Mode).GetMasterStyle(CurrentView.Guid).IsVisible;
            }
        }

        override public bool InteractiveFlag
        {
            get { return true; }
        }

        override public bool DisplayHeadIconFlag
        {
            get { return true; }
        }

    }
}
