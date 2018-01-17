using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal struct SerialAdaptiveView
    {
        public AdaptiveViewCondition Condition;
        public int Width;
        public int Height;
    }


    [Serializable]
    internal class SerialPage
    {
        [NonSerialized]
        protected IPage _page;

        #region public interface functions
        public SerialPage(IPage page)
        {
            _page = page;
            InitializeWidgetsMD5();
            InitializeMastersMD5();
            InitializePageProperty();
        }
        virtual protected void Update(IPage newPage)
        {
            _page = newPage;
            InitializeWidgetsMD5();
            InitializeMastersMD5();
            InitializePageProperty();
        }
        #endregion

        #region private functions
        private void InitializeWidgetsMD5()
        {
            foreach(IWidget it in _page.Widgets.OrderBy(x => x.WidgetStyle.Z))
            {
                WidgetsMD5+=MD5HashManager.GetHash(it, true);
            }
   
        }
        private void InitializeMastersMD5()
        {
            foreach (IMaster it in _page.Masters.OrderBy(x => x.MasterStyle.Z))
            {
                MastersMD5 += MD5HashManager.GetHash(it, true);
            }
        }
        private void InitializePageProperty()
        {
            if(_page is IDocumentPage)
            {
                Name =_page.Name;
            }

            if (_page is IMasterPage)
            {
                Name = string.Empty;
            }

            //IAnnotation convert
            NotesTextValues = _page.Annotation.TextValues;

            //adaptive view convert
            foreach (IAdaptiveView view in _page.ParentDocument.AdaptiveViewSet.AdaptiveViews)
            {
                SerialAdaptiveView ad = new SerialAdaptiveView();
                ad.Condition= view.Condition;
                ad.Width = view.Width;
                ad.Height = view.Height;
                AdaptiveViews.Add(ad);
            }
        }
        #endregion

        #region Serialize Data
        public string Name;
        public string MastersMD5;
        public string WidgetsMD5;

        [NonSerialized]
        public List<SerialAdaptiveView> AdaptiveViews=new List<SerialAdaptiveView>();
        [NonSerialized]
        Dictionary<string, string> NotesTextValues;
        #endregion
    }
}
