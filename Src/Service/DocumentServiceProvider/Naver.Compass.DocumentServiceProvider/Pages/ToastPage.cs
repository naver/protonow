using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class ToastPage : EmbeddedPage, IToastPage
    {
        internal ToastPage(Toast toast, string pageName)
            : base("ToastPage", pageName)
        {
            Debug.Assert(toast != null);
            _toast = toast;

            InitializeBasePageView();
        }

        public override IPageEmbeddedWidget ParentWidget
        {
            get { return ParentToast; }
        }

        public IToast ParentToast
        {
            get { return _toast; }
        }
        
        private Toast _toast;
    }
}
