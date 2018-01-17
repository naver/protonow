using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialWidgetBase
    {
        [NonSerialized]
        protected IWidget _wdg;

        #region public interface functions
        public SerialWidgetBase(IWidget wdg)
        {
            _wdg = wdg;
            InitializeStyleMD5();
        }
        virtual protected void Update(IWidget newWdg)
        {
            _wdg = newWdg;
            InitializeStyleMD5();
        }
        #endregion

        #region private functions
        private void InitializeStyleMD5()
        {
            IWidgetStyle style;
            IPageView pageView;

            //Set base Style MD5
            pageView = _wdg.ParentPage.PageViews[_wdg.ParentPage.ParentDocument.AdaptiveViewSet.Base.Guid];
            style = _wdg.WidgetStyle;
            if (style != null && pageView != null)
            {
                if (pageView.Widgets.Contains(style.OwnerWidget.Guid))
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
                style = _wdg.GetWidgetStyle(view.Guid);
                pageView = _wdg.ParentPage.PageViews[view.Guid];
                if (style != null && pageView != null)
                {
                    if (pageView.Widgets.Contains(style.OwnerWidget.Guid))
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
