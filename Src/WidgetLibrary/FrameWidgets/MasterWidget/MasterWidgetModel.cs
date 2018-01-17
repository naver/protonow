using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Drawing;
namespace Naver.Compass.WidgetLibrary
{
    class MasterWidgetModel : MasterModel
    {
        public MasterWidgetModel(IMaster master, bool isNail)
            : base(master)
        {
            _IsNail = isNail;
            ChildPageID = master.MasterPageGuid;
        }

        #region Private member
        bool _IsNail;
        private ObservableCollection<WidgetPreViewModeBase> _items;
        private IPageView _masterPageView;
        #endregion

        #region Binding Peoperty
        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                LoadAllChildrenWidgets();
                return _items;
            }
        }
        public bool IsLockedToMasterLocation
        {
            get { return _master.IsLockedToMasterLocation; }
            set
            {
                if (_master.IsLockedToMasterLocation != value)
                {
                    _master.IsLockedToMasterLocation = value;
                    _document.IsDirty = true;
                }
            }
        }
        #endregion

        #region Public fucntions
        public Guid ChildPageID{get; set;}
        public void LoadAllChildrenWidgets()
        {
            if (_items == null)
            {
                _items = new ObservableCollection<WidgetPreViewModeBase>();
            }
            _items.Clear();

            _master.MasterPage.Open();
            IPageView view = _master.MasterPage.PageViews.GetPageView(StyleGID);
            if (view == null)
            {
                return;
            }

            foreach (IWidget wdg in _master.MasterPage.Widgets)
            {
                WidgetPreViewModeBase preItem = ReadOnlyWidgetFactory.CreateWidget(wdg, _IsNail);
                preItem.ChangeCurrentPageView(view);
                if (preItem == null)
                {
                    continue;
                }
                _items.Add(preItem);

            }
        }
        override public bool ChangeCurrentStyle(Guid newStyleGid)
        {
            if (base.ChangeCurrentStyle(newStyleGid) == false)
            {
                return false;
            }

            _master.MasterPage.Open();
            IPageView view = _master.MasterPage.PageViews.GetPageView(newStyleGid);
            _masterPageView = view;
            if (view == null || _items == null)
            {
                return false;
            }

            foreach (var item in _items)
            {
                item.ChangeCurrentPageView(view);
            }
            return true;
        }
        public Double MasterLockedLocationX
        {
            get
            {
                return _masterPageView.RegionStyle.X;
            }
            
        }
        public Double MasterLockedLocationY
        {
            get
            {
                return _masterPageView.RegionStyle.Y;
            }

        }
        #endregion
    }
}
