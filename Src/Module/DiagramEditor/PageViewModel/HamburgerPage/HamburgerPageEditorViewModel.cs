using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Common.Helper;
using System.Windows.Controls;
using System.Windows.Documents;
using Naver.Compass.Service;
using System.Windows.Markup;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Naver.Compass.Module
{
    public class HamburgerPageEditorViewModel:WidgetPageEditorViewModel
    {
        public HamburgerPageEditorViewModel(IWidget widget)
            : base(widget)
        {
            _pageType = Common.CommonBase.PageType.HamburgerPage;
            InitializeHamburgPage(widget);
            InitializeCommonData();
        }

        
        #region Private Function and  Property
        private void InitializeHamburgPage(IWidget widget)
        {
            if (widget == null)
            {
                return;
            }
            _copyTime = 0;
            Title = @"Drawer Menu " + widget.Name;
            _pageGID = widget.Guid;
            ContentId = _pageGID.ToString();
            IPage page = (widget as IHamburgerMenu).MenuPage;
            _model = new PageEditorModel(page);
            SetDefaultAdaptive();
            _acitiveCurrentChildPage = page;
            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler);
        }
        private void Return2ParentPageExecute(object obj)
        {
            Guid parentGID = _widget.ParentPage.Guid;
            _ListEventAggregator.GetEvent<OpenNormalPageEvent>().Publish(parentGID);
        }
        #endregion

        #region Public Override functions
        public override void Close(Guid PageGID)
        {
            //if (PageGID!=Guid.Empty)
            //{
                
            //}
            
            if (_widget.ParentPage.IsOpened)
            {
                Return2ParentPageExecute(null);
                _ListEventAggregator.GetEvent<CloseWidgetPageEvent>().Publish(_widget);
                return;
            }
            else
            {
                (_widget as IHamburgerMenu).MenuPage.Close();
            }

            
        }
        #endregion


    }
}
