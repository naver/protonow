using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.ComponentModel;


namespace Naver.Compass.Module
{
    class GroupListItem : WidgetListItem
    {
        public GroupListItem(IGroup data)
            : base(data)
        {
        }

        override public string WidgetTypeName
        {
            get
            {
                string sTypeName = Naver.Compass.Common.Helper.GlobalData.FindResource("ObjectListManager_FilterMenu_Group");
                return "("+sTypeName+")" ;
               // return "(Group-" + zOrder.ToString() + ")";
            }
        }

        override public bool PlaceFlag
        {
            get
            {
                //System.Diagnostics.Debug.Assert(ParentPage != null, "Count Place Flag->ParentPage is null");

                //IPageView CurrentView = ParentPage.PageViews.GetPageView(CurViewID);
                //if (CurrentView.Groups.Contains(WidgetID))
                //{
                return false;
                //}
                //else
                //{
                //    return true;
                //}
            }
            set
            {
                System.Diagnostics.Debug.Assert(false, "Error option to set PlaceFlag");
            }
        }

        override public bool LostFlag
        {
            get
            {
                //if (PlaceFlag)
                //{
                //    foreach (IPageView childView in ParentPage.PageViews)
                //    {
                //        if (childView.Groups.Contains(WidgetID))
                //        {
                //            return false;
                //        }
                //    }

                //    return true;
                //}
                //else
                //{
                return false;
                //}
            }
            set
            {
                System.Diagnostics.Debug.Assert(false, "Error option to set LostFlag");
            }
        }

        private bool _hideFlag;
        override public bool HideFlag
        {
            get
            {
                return _hideFlag;
            }
            set
            {
                if (_hideFlag != (bool)value)
                {
                    _hideFlag = (bool)value;

                    FirePropertyChanged("HideFlag");
                }
            }

        }

        private int _zorder = 0;
        override public int zOrder
        {
            get
            {
                return _zorder;
            }
            set
            {
                if (_zorder != (int)value)
                {
                    _zorder = (int)value;
                    FirePropertyChanged("zOrder");
                }
            }
        }

        override public bool InteractiveFlag
        {
            get
            {
                return false;
            }
        }

        override public bool DisplayHeadIconFlag
        {
            get
            {
                if (ItemType == ListItemType.GroupChildItem)
                {
                    return false;
                }

                return true;
            }
        }

        override public void UpdateAllFlagByViewChange()
        {
            return;
        }

    }
}
