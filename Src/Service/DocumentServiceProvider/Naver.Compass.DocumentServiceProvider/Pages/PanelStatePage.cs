using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class PanelStatePage : EmbeddedPage, IPanelStatePage
    {
        internal PanelStatePage(DynamicPanel panel, string pageName)
            : base("PanelStatePage", pageName)
        {
            Debug.Assert(panel != null);
            _parentPanel = panel;

            InitializeBasePageView();
        }

        public override IPageEmbeddedWidget ParentWidget
        {
            get { return ParentPanel; }
        }

        public IDynamicPanel ParentPanel
        {
            get { return _parentPanel; }
        }

        private DynamicPanel _parentPanel;
    }
}
