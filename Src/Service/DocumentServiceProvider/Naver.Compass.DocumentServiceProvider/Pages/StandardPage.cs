using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class StandardPage : DocumentPage, IStandardPage
    {
        internal StandardPage(Document document, string pageName)
            : base("StandardPage", document)
        {
            _pageData = new StandardPageData(this, "StandardPage");
            _pageData.Name = pageName;

            InitializeBasePageView();
        }

        #region IInteractiveObject

        public IPage ContextPage
        {
            get { return this; }
        }

        public IInteractionEvents Events
        {
            get { return _pageData.Events; }
        }

        #endregion

        internal override void UpdateActions()
        {
            base.UpdateActions();

            foreach (InteractionEvent iEvent in _pageData.Events)
            {
                iEvent.UpdateActions();
            }
        }

        internal override PageData PageData
        {
            get { return _pageData; }
        }

        private StandardPageData _pageData;
    }
}
