using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;

namespace Naver.Compass.Module
{
    partial class WidgetManagerViewMode
    {

        private Dictionary<ListItemType, bool> _filterType = new Dictionary<ListItemType, bool>();

        private void InitFilterList()
        {
            SetValueToDictionary(true);
        }

        private bool IsShowAll()
        {
            foreach (bool value in _filterType.Values)
            {
                if (!value)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetValueToDictionary(bool value)
        {
            _filterType[ListItemType.DynamicPanelItem] = value;
            _filterType[ListItemType.MasterItem] = value;
            _filterType[ListItemType.MenuItem] = value;
            _filterType[ListItemType.GroupItem] = value;
            _filterType[ListItemType.ToastItem] = value;
            _filterType[ListItemType.defaultItem] = value;
            _filterType[ListItemType.DynamicPanelStateItem] = value;
        }

        private bool GetgroupFlag(WidgetListItem groupitem)
        {
            return _filterType[ListItemType.GroupItem]; ;

            //if (!_filterType[ListItemType.GroupItem])
            //{
            //    foreach (WidgetListItem child in groupitem.OrderedChildren)
            //    {
            //        if (_filterType[child.ItemType])
            //        {
            //            return true;
            //        }
            //    }

            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
        }

        private void SetFilterFlag(List<WidgetListItem> list)
        {
            foreach (WidgetListItem item in list)
            {
                if (item.ItemType == ListItemType.DynamicPanelStateItem && item.OrderedChildren != null)
                {
                    SetFilterFlag(item.OrderedChildren);
                }
                if (item.ItemType == ListItemType.GroupItem || item.ItemType == ListItemType.GroupChildItem)
                {
                    SetGroupFilterFlag(item);
                }
                else
                {
                    item.UnFilter = _filterType[item.ItemType];
                }
            }

        }

        private void SetGroupFilterFlag(WidgetListItem GroupItem)
        {
            GroupItem.UnFilter = GetgroupFlag(GroupItem);

            foreach (WidgetListItem child in GroupItem.OrderedChildren)
            {
                if (child.ItemType == ListItemType.GroupItem || child.ItemType == ListItemType.GroupChildItem)
                {
                    SetGroupFilterFlag(child);
                }
                else
                {
                    child.UnFilter = GroupItem.UnFilter;
                }
            }
        }


        private void OnChangeFilter()
        {
            UpdateUIData();
        }

        #region Menu Command

        public bool _IsOpen = false;
        public bool IsFilterMenuOpen
        {
            get
            {
                return _IsOpen;
            }
            set
            {
                if ((bool)value != _IsOpen)
                {
                    _IsOpen = (bool)value;
                    FirePropertyChanged("IsFilterMenuOpen");
                }

            }
        }

        public bool IsShowAllType
        {
            get
            {
                return IsShowAll();
            }
            set
            {
                bool vale = ((bool)value);
                if (IsShowAllType != vale)
                {
                    SetValueToDictionary(vale);

                    OnChangeFilter();

                    FirePropertyChanged("IsShowGroup");
                    FirePropertyChanged("IsShowMaster");
                    FirePropertyChanged("IsShowSwipViews");
                    FirePropertyChanged("IsShowDrawMenu");
                    FirePropertyChanged("IsShowToast");
                    FirePropertyChanged("IsShowObjects");
                }
            }
        }

        public bool IsShowGroup
        {
            get
            {
                return _filterType[ListItemType.GroupItem];
            }
            set
            {
                if (_filterType[ListItemType.GroupItem] != (bool)value)
                {
                    _filterType[ListItemType.GroupItem] = (bool)value;

                    OnChangeFilter();

                    FirePropertyChanged("IsShowAllType");
                }
            }
        }

        public bool IsShowMaster
        {
            get
            {
                return _filterType[ListItemType.MasterItem];
            }
            set
            {
                if (_filterType[ListItemType.MasterItem] != (bool)value)
                {
                    _filterType[ListItemType.MasterItem] = (bool)value;

                    OnChangeFilter();

                    FirePropertyChanged("IsShowAllType");
                }
            }
        }

        public bool IsShowSwipViews
        {
            get
            {
                return _filterType[ListItemType.DynamicPanelItem];
            }
            set
            {
                if (_filterType[ListItemType.DynamicPanelItem] != (bool)value)
                {
                    _filterType[ListItemType.DynamicPanelItem] = (bool)value;

                    OnChangeFilter();
                    FirePropertyChanged("IsShowAllType");
                }
            }
        }

        public bool IsShowDrawMenu
        {
            get
            {
                return _filterType[ListItemType.MenuItem];
            }
            set
            {
                if (_filterType[ListItemType.MenuItem] != (bool)value)
                {
                    _filterType[ListItemType.MenuItem] = (bool)value;

                    OnChangeFilter();

                    FirePropertyChanged("IsShowAllType");
                }
            }
        }

        public bool IsShowToast
        {
            get
            {
                return _filterType[ListItemType.ToastItem];
            }
            set
            {
                if (_filterType[ListItemType.ToastItem] != (bool)value)
                {
                    _filterType[ListItemType.ToastItem] = (bool)value;

                    OnChangeFilter();

                    FirePropertyChanged("IsShowAllType");
                }
            }
        }

        public bool IsShowObjects
        {
            get
            {
                return _filterType[ListItemType.defaultItem];
            }
            set
            {
                if (_filterType[ListItemType.defaultItem] != (bool)value)
                {
                    _filterType[ListItemType.defaultItem] = (bool)value;

                    OnChangeFilter();

                    FirePropertyChanged("IsShowAllType");
                }
            }
        }

        #endregion
    }
}
