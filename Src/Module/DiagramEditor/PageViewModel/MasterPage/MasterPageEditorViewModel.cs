using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module
{
    public class MasterPageEditorViewModel : ForbidAddingPageEditorViewModel
    {
        public MasterPageEditorViewModel(Guid pageGuid) :
            base(pageGuid)
        {
            _pageType = Common.CommonBase.PageType.MasterPage;
            _isForbidDrawerMenu = false;
            _isForbidToast = false;
            _isForbidSwipeviews = false;
            InitializeStartMsg(pageGuid);
        }


        #region Override binding property
        public override Service.Document.IPage ActivePage
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc.Document != null)
                {
                    return doc.Document.MasterPages.GetPage(_pageGID);
                }
                return null;
            }
            set
            {

            }
        }
        public override void UpdateTitle()
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null)
            {
                IPage pageItem = doc.Document.MasterPages.GetPage(_pageGID);
                if (pageItem != null)
                {
                    Title = pageItem.Name;
                }
            }
            else
            {
                Title = _pageGID.ToString();
            }
        }
        protected override void UpdateLanguageEventHandler(string str)
        {
            FirePropertyChanged("StartMsg");
            base.UpdateLanguageEventHandler(str);
        }

        public override Visibility StartMsgVisibility
        {
            get { return _startMsgVisibility; }
            set
            {
                if (_startMsgVisibility != value)
                {
                    _startMsgVisibility = value;
                    FirePropertyChanged("StartMsgVisibility");
                }
            }
        }
        public override String StartMsg
        {
            get
            {
                return GlobalData.FindResource("Master_page_Info");
            }

        }
        #endregion

        #region private functions
        void InitializeStartMsg(Guid pageGuid)
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null)
            {
                IPage pageItem = doc.Document.MasterPages.GetPage(_pageGID);
                if (pageItem == null)
                {
                    return;
                }

                pageItem.Open();
                if(pageItem.Widgets.Count<=0)
                {
                    _startMsgVisibility = Visibility.Visible;
                }
            }
        }
        #endregion
    }
}
