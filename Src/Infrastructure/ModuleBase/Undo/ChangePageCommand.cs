using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Events;

namespace Naver.Compass.InfoStructure
{
    class ChangePageCommand : IUndoableCommand
    {
        internal ChangePageCommand()
        {
            ISelectionService selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            if (selectionService != null)
            {
                _pageVM = selectionService.GetCurrentPage();
                _activePage = _pageVM.ActivePage;
                _acitveAdaptiveID = _pageVM.CurAdaptiveViewGID;
            }

            _listEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
        }

        public void Undo()
        {
            ChangePage();            
        }

        public void Redo()
        {
            ChangePage();
        }

        private void ChangePage()
        {
            if(_pageVM.PageType == PageType.NormalPage)
            {
                //_listEventAggregator.GetEvent<OpenNormalPageEvent>().Publish(_pageVM.PageGID);
            }
            else if(_pageVM.PageType == PageType.DynamicPanelPage)
            {
                //_listEventAggregator.GetEvent<OpenWidgetPageEvent>().Publish((_pageVM.ActivePage as IPanelStatePage).ParentPanel);
                if (_pageVM.ActivePage != _activePage)
                {
                    _pageVM.ActivePage = _activePage;
                }
            }
            else if (_pageVM.PageType == PageType.HamburgerPage)
            {
                //_listEventAggregator.GetEvent<OpenWidgetPageEvent>().Publish();
                //if (_pageVM.ActivePage != _activePage)
                //{
                //    _pageVM.ActivePage = _activePage;
                //}
            }
            _pageVM.SetAdaptiveView(_acitveAdaptiveID);
        }

        private IEventAggregator _listEventAggregator;
        private IPagePropertyData _pageVM;
        private IPage _activePage;
        private Guid _acitveAdaptiveID;
    }
}
