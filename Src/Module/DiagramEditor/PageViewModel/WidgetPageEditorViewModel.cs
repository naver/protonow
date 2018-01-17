using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using System.Collections.ObjectModel;

namespace Naver.Compass.Module
{
    /// <summary>
    /// Base PageEditor VM for widget has a child page.
    /// Like hamburgerMenu, Filcking area, Toast
    /// </summary>
    public class WidgetPageEditorViewModel : ForbidAddingPageEditorViewModel
    {
        public WidgetPageEditorViewModel(IWidget widget)
        {
            _widget = widget;

        }

        private void RefreshPageUI(IPage AcitiveCurrentChildPage)
        {
            items.CollectionChanged -= items_CollectionChanged;
            Items.Clear();
            _model = new PageEditorModel(AcitiveCurrentChildPage);
            AsyncLoadAllWidgets();
        }

        #region Override Property

        virtual public IPage AcitiveCurrentChildPage
        {
            get
            {
                return _acitiveCurrentChildPage;
            }
            set
            {
                if (_acitiveCurrentChildPage != value)
                {
                    _acitiveCurrentChildPage = value;
                    RefreshPageUI(_acitiveCurrentChildPage);
                }
            }
        }

        override public IPage ActivePage
        {
            get
            {
                return _acitiveCurrentChildPage;
            }
        }
        #endregion

        #region Pulic Binding Property
        virtual public int FlickingWidth
        {
            get
            {
                return Convert.ToInt32(_widget.GetWidgetStyle(_model.ActivePageView.Guid).Width * EditorScale);
            }
        }
        virtual public int FlickingHeight
        {
            get
            {
                return Convert.ToInt32(_widget.GetWidgetStyle(_model.ActivePageView.Guid).Height * EditorScale);
            }
        }
        #endregion

        #region Override Property and Functions
        override public double EditorScale
        {
            get { return _model.EditorScale; }
            set
            {
                if (_model.EditorScale != value)
                {
                    _model.EditorScale = value;
                    FirePropertyChanged("EditorScale");
                    FirePropertyChanged("AdaptiveWidth");
                    FirePropertyChanged("AdaptiveHeight");
                    FirePropertyChanged("DeviceBoxWidth");
                    FirePropertyChanged("DeviceBoxHeight");
                    FirePropertyChanged("FlickingWidth");
                    FirePropertyChanged("FlickingHeight");
                    FirePropertyChanged("IsGridVisible");
                }
            }
        }
        override protected void OnPannelSelected(bool bIsSelected)
        {
            if (_model.IsPageOpen() == false)
            {
                return;
            }

            base.OnPannelSelected(bIsSelected);
            if (bIsSelected == true)
            {
                FirePropertyChanged("FlickingWidth");
                FirePropertyChanged("FlickingHeight");
            }
            else
            {
                //Leave the page, and send message to parent widget(hamburger/toast) to refresh UI                

                Guid widgetGuid;
                if (_widget.WidgetType == WidgetType.HamburgerMenu)
                {
                    widgetGuid = (_widget as IHamburgerMenu).MenuPage.Guid;
                    _ListEventAggregator.GetEvent<RefreshWidgetChildPageEvent>().Publish(widgetGuid);
                }
                else
                {                    
                    if (_isThumbnailUpdate == true)
                    {
                        widgetGuid = _widget.Guid;
                        _ListEventAggregator.GetEvent<RefreshWidgetChildPageEvent>().Publish(widgetGuid);
                        _isThumbnailUpdate = false;
                    }
                }
            }
        }



        protected override void CheckAdaptiveView(Guid guid)
        {
            base.CheckAdaptiveView(guid);
            FirePropertyChanged("FlickingWidth");
            FirePropertyChanged("FlickingHeight");
        }

        #endregion

        #region potected and private member
        protected IWidget _widget;
        protected IPage _acitiveCurrentChildPage = null;

        #endregion

    }
}
