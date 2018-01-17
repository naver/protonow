using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialMasterBase
    {
        [NonSerialized]
        protected IMaster _wdg;

        #region public interface functions
        public SerialMasterBase(IMaster wdg)
        {
            _wdg = wdg;
            InitializeStyleMD5();
        }
        virtual public void Update(IMaster newWdg)
        {
            _wdg = newWdg;
            InitializeStyleMD5();
        }
        #endregion

        #region private functions
        private void InitializeStyleMD5()
        {
            IMasterStyle style;
            IPageView pageView;

            //Set base Style MD5
            pageView = _wdg.ParentPage.PageViews[_wdg.ParentPage.ParentDocument.AdaptiveViewSet.Base.Guid];
            style = _wdg.MasterStyle;
            if (style != null && pageView != null)
            {
                if (pageView.Masters.Contains(style.OwnerMaster.Guid))
                {
                    StyleMD5 += MD5HashManager.GetHash(style, true, true);
                }
                else
                {
                    StyleMD5 += MD5HashManager.GetHash(style, false, true);
                }
            }


            //Set All Other Style MD5
            foreach (IAdaptiveView view in _wdg.ParentPage.ParentDocument.AdaptiveViewSet.AdaptiveViews)
            {
                style = _wdg.GetMasterStyle(view.Guid);
                pageView = _wdg.ParentPage.PageViews[view.Guid];
                if (style != null && pageView != null)
                {
                    if (pageView.Masters.Contains(style.OwnerMaster.Guid))
                    {
                        AllStylesMD5 += MD5HashManager.GetHash(style, true, true);
                    }
                    else
                    {
                        AllStylesMD5 += MD5HashManager.GetHash(style, false, true);
                    }                
                }
            }

        }
        #endregion

        #region Serialize Data
        public string StyleMD5;
        [NonSerialized]
        public string AllStylesMD5;
        #endregion
    }
}
