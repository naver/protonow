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
    public class ToastModel : WidgetModel
    {
        public ToastModel(IWidget widget, bool isNail)
            : base(widget)
        {
            _IsNail = isNail;
            _toast = widget as IToast;
        }

        #region Binding Peoperty
        public override string Tooltip
        {
            get
            {
                return _toast.Tooltip;
            }
            set
            {
                if(_toast.Tooltip!=value)
                {
                    _toast.Tooltip = value;
                    _document.IsDirty = true;
                }
            }
        }
        public int ExposureTime
        {
            get
            {
                return _toast.ExposureTime;
            }
            set
            {
                if (_toast.ExposureTime!=value)
                {
                    _toast.ExposureTime = value;
                    _document.IsDirty = true;
                }
            }
        }

        public ToastCloseSetting CloseSetting
        {
            get
            {
                return _toast.CloseSetting;
            }
            set
            {
                if (_toast.CloseSetting != value)
                {
                    _toast.CloseSetting = value;
                    _document.IsDirty = true;
                }
            }
        }

        public ToastDisplayPosition DisplayPosition
        {
            get
            {
                return _toast.DisplayPosition;
            }
            set
            {
                if(_toast.DisplayPosition!=value)
                {
                    _toast.DisplayPosition = value;
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

            _toast.ToastPage.Open();
            IPageView view = _toast.ToastPage.PageViews.GetPageView(StyleGID);
            if (view == null)
            {
                return ;
            }

            foreach (IWidget wdg in _toast.ToastPage.Widgets)
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

            _toast.ToastPage.Open();
            IPageView view = _toast.ToastPage.PageViews.GetPageView(newStyleGid);
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
        public bool IsAnyChildrenPageOpen()
        {
            return _toast.ToastPage.IsOpened;
        }
        #endregion

        #region Private member
        bool _IsNail;
        private IToast _toast = null;
        private ObservableCollection<WidgetPreViewModeBase> _items;
        #endregion
    }
}
