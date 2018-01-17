using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using Naver.Compass.Common.Helper;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public class DynamicPanelIconNode : ViewModelBase
    {
        public DynamicPanelIconNode(IPage childPage, Guid guid)
        {
            _styleGuid = guid;
            _page = childPage;
            isChecked = false;
        }
        #region Private Functions and Properties
        private IPage _page;
        private Guid _styleGuid;
        #endregion

        #region Public Binding Propery
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if(isChecked!=value)
                {
                    isChecked = value;
                    FirePropertyChanged("IsChecked");
                }
            }

        }

        private int _showNumber;
        public int ShowNumber
        {
            get { return _showNumber; }
            set
            {
                if (_showNumber != value)
                {
                    _showNumber = value;
                    FirePropertyChanged("ShowNumber");
                }
            }
        }

        private int _showType;
        public int ShowType
        {
            get { return _showType; }
            set
            {
                if (_showType != value)
                {
                    _showType = value;
                    FirePropertyChanged("ShowType");
                }
            }
        }

        public ObservableCollection<WidgetPreViewModeBase> items;
        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                return items;
            }
        }

        private double _panelWidth;
        public double PanelWidth
        {
            get
            {
                return _panelWidth;
            }
            set
            {
                if(_panelWidth!=value)
                {
                    _panelWidth = value;
                    FirePropertyChanged("PanelWidth");
                }
            }
        }

        private double _lineWidth;
        public double LineWidth
        {
            get
            {
                return _lineWidth;
            }
            set
            {
                if(_lineWidth!=value)
                {
                    _lineWidth = value;
                    FirePropertyChanged("PanelMargin");
                }
            }
        }
        public Thickness PanelMargin
        {
            get
            {
                return new Thickness(LineWidth, 0, 0, 0);
            }
        }
        #endregion

        #region Public Propery for Property Panel
        public Guid PageGID
        {
            get
            {
                return _page.Guid;
            }
        }

        public string Name 
        {
            get
            {
                return _page.Name;
            }
        }
        #endregion

        #region public function
        public void LoadAllChildrenWidgets(bool bIsNail)
        {
            if (items == null)
            {
                items = new ObservableCollection<WidgetPreViewModeBase>();
            }
            items.Clear();

            if(!_page.IsOpened)
            {
                _page.Open();
            }
            
            IPageView view = _page.PageViews.GetPageView(_styleGuid);
            if (view == null)
            {
                _page.Close();
                return;
            }
            foreach (IWidget wdg in _page.Widgets)
            {
                WidgetPreViewModeBase preItem = ReadOnlyWidgetFactory.CreateWidget(wdg, bIsNail);
                preItem.ChangeCurrentPageView(view);
                if (preItem == null)
                {
                    continue;
                }
                items.Add(preItem);
            }
            //_page.Close();
        }

        #endregion

    }
}
