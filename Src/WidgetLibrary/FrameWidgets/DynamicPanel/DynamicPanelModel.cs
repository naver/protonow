using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;

namespace Naver.Compass.WidgetLibrary
{
    public class DynamicPanelModel : WidgetModel
    {
        public DynamicPanelModel(IWidget widget,bool isNail)
            : base(widget)
        {
            _IsNail = isNail;
            _flicking = widget as IDynamicPanel;
            if (_flicking != null && _flicking.PanelStatePages.Count <= 0)
            {
                _flicking.StartPanelStatePage = _flicking.CreatePanelStatePage(@"Panel 01");
                _flicking.CreatePanelStatePage(@"Panel 02");
                _flicking.CreatePanelStatePage(@"Panel 03");
            }
            LoadAllChildrenWidgets();
            return;
        }

        #region pbulic member and functions
        public Guid StartPageGID
        {
            get
            {
                return _flicking.StartPanelStatePage.Guid ;
            }
        }
        public bool IsAnyChildrenPageOpen()
        {
            foreach (IPage page in _flicking.PanelStatePages)
            {
                if (page.IsOpened)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion pbulic member and functions

        #region private member and functions

        bool _IsNail;
        private IDynamicPanel _flicking=null;
        public void LoadAllChildrenWidgets()
        {
            if (items == null)
            {
                items = new ObservableCollection<WidgetPreViewModeBase>();
            }
            items.Clear();

            _flicking.StartPanelStatePage.Open();
            IPageView view = _flicking.StartPanelStatePage.PageViews.GetPageView(StyleGID);
            if (view == null)
            {
                return;
            }
            foreach (IWidget wdg in _flicking.StartPanelStatePage.Widgets)
            {
                WidgetPreViewModeBase preItem = ReadOnlyWidgetFactory.CreateWidget(wdg, _IsNail);
                preItem.ChangeCurrentPageView(view);
                if (preItem == null)
                {
                    continue;
                }
                items.Add(preItem);
            }
        }
        override public bool ChangeCurrentStyle(Guid newStyleGid)
        {
            
            if (base.ChangeCurrentStyle(newStyleGid) == false)
            {
                return false;
            }

            _flicking.StartPanelStatePage.Open();
            IPageView view = _flicking.StartPanelStatePage.PageViews.GetPageView(newStyleGid);
            if (view == null || items == null)
            {
                return false;
            }

            foreach (var item in items)
            {
                item.ChangeCurrentPageView(view);
            }
            return true;
        }
        #endregion private member

        #region public property for binding
        public ObservableCollection<WidgetPreViewModeBase> items;
        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                //LoadAllChildrenWidgets();
                return items;
            }
        }


        public NavigationType ShowType
        {
            get
            {
                return _flicking.NavigationType;
            }
            set
            {
                if (_flicking.NavigationType != value)
                {
                    _flicking.NavigationType = value;                    
                }                
            }
        }
        public Guid StartPage
        {
            get
            {
                return _flicking.StartPanelStatePage.Guid;
            }
            set
            {
                if (_flicking.StartPanelStatePage.Guid != value)
                {
                    foreach (IPage item in _flicking.PanelStatePages)
                    {
                        if (value == item.Guid)
                        {
                            _flicking.StartPanelStatePage = item as IPanelStatePage;
                            break;
                        }
                    }
                }
            }
        }
        public bool IsShowArrow
        {
            get
            {
                return _flicking.ShowAffordanceArrow;
            }
            set
            {
                if (_flicking.ShowAffordanceArrow != value)
                {
                    _flicking.ShowAffordanceArrow = value;
                }
            }
        }
        public bool IsCirculer
        {
            get
            {
                return _flicking.IsCircular;
            }
            set
            {
                if (_flicking.IsCircular != value)
                {
                    _flicking.IsCircular = value;
                }
            }
        }
        public bool IsAutomatic
        {
            get
            {
                return _flicking.IsAutomatic;
            }
            set
            {
                if (_flicking.IsAutomatic != value)
                {
                    _flicking.IsAutomatic = value;
                }
            }
        }

        public DynamicPanelViewMode ViewMode
        {
            get
            {
                return _flicking.ViewMode;
            }
            set
            {
                if (_flicking.ViewMode != value)
                {
                    _flicking.ViewMode = value;
                }
            }
        }

        public int PanelWidth
        {
            get
            {
                return _flicking.PanelWidth;
            }
            set
            {
                if (_flicking.PanelWidth != value)
                {
                    _flicking.PanelWidth = value;
                }
            }
        }

        public double LineWidth
        {
            get
            {
                return _flicking.LineWith;
            }
            set
            {
                if (_flicking.LineWith != value)
                {
                    _flicking.LineWith = value;
                }
            }
        }
        public Stream ImageStream
        {
            set
            {
                return;
                //_flicking.PanelStatePages.FirstOrDefault<IPanelStatePage>(x => x.Guid == _flicking.StartPanelStatePage);
            }
        }
        #endregion private member
    }
}
