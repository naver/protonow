using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Naver.Compass.WidgetLibrary
{
    public class MasterModel
    {
        public MasterModel(IMaster widget, bool isNail)
        {
            _IsNail = isNail;
            _master = widget as IMaster;
        }

        #region Binding Peoperty

        public bool IsVisible
        {
            get { return _style.IsVisible; }
            set
            {
                if (_style.IsVisible != value)
                {
                    _style.IsVisible = value;
                    _document.IsDirty = true;
                }
            }
        }
        public double Left
        {
            get { return _style.X; }
            set
            {
                if (_style.X != value)
                {
                    _style.X = value;
                    _document.IsDirty = true;
                }
            }
        }
        public double Top
        {
            get { return _style.Y; }
            set
            {
                if (_style.Y != value)
                {
                    _style.Y = value;
                    _document.IsDirty = true;
                }
            }
        }
        public double ItemWidth
        {
            get { return _style.Width; }
        }
        public double ItemHeight
        {
            get { return _style.Height; }
        }

        //this is temporary solution for widget manger Zorder change, And TODO:.....
        public int ZOrder
        {
            get { return _style.Z; }
            set
            {
                if (_style.Z != value)
                {
                    _style.Z = value;
                    _document.IsDirty = true;
                }
            }
        }

        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                LoadAllChildrenWidgets();
                return _items;
            }
        }       
        #endregion

        #region Public fucntions
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
                return ;
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
        public bool ChangeCurrentStyle(Guid newStyleGid)
        {

            if (newStyleGid == Guid.Empty)
            {
                _style = _master.MasterStyle;
            }
            else
            {
                _style = _master.GetMasterStyle(newStyleGid);
                if (_style == null)
                {
                    _style = _master.MasterStyle;
                }
            }

            IPageView view = _master.MasterPage.PageViews.GetPageView(newStyleGid);
            if (view == null || _items==null)
            {
                return false;
            }

            foreach (var item in _items)
            {
                item.ChangeCurrentPageView(view);
            }
            return true;
        }
        #endregion

        public IMasterStyle Style
        {
            get { return _style; }
        }
        public Guid StyleGID
        {
            get { return _style.ViewGuid; }
        }

        #region Private member
        protected IMasterStyle _style = null;
        protected IMaster _master = null;
        protected IDocument _document = null;
        bool _IsNail;
        private ObservableCollection<WidgetPreViewModeBase> _items;
        #endregion
    }
}
