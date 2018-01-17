using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    internal abstract class PageEmbeddedWidget : Widget, IPageEmbeddedWidget
    {
        internal PageEmbeddedWidget(Page parentPage, string tagName)
            : base(parentPage, tagName)
        {
        }

        public abstract ReadOnlyCollection<IEmbeddedPage> EmbeddedPages { get; }

        internal override void OnAddToDocument()
        {
            base.OnAddToDocument();

            foreach(EmbeddedPage page in EmbeddedPages)
            {
                page.OnAddToDocument();
            }
        }

        internal override void OnDeleteFromDocument(bool isParentPageDeleted)
        {
            foreach (EmbeddedPage page in EmbeddedPages)
            {
                page.OnDeleteFromDocument();
            }
        }

        internal override void OnAddAdaptiveView(AdaptiveView view)
        {
            base.OnAddAdaptiveView(view);

            foreach (EmbeddedPage page in EmbeddedPages)
            {
                if (page != null && page.IsOpened)
                {
                    page.OnAddAdaptiveView(view);
                }
            }
        }

        internal override void OnDeleteAdaptiveView(Guid viewGuid)
        {
            base.OnDeleteAdaptiveView(viewGuid);

            foreach (EmbeddedPage page in EmbeddedPages)
            {
                if (page != null && page.IsOpened)
                {
                    page.OnDeleteAdaptiveView(viewGuid);
                }
            }
        }

        internal override void RebuildStyleChain(WidgetStyle newBaseStyle)
        {
            base.RebuildStyleChain(newBaseStyle);

            foreach (EmbeddedPage page in EmbeddedPages)
            {
                foreach (Widget widget in page.Widgets)
                {
                    widget.RebuildStyleChain(newBaseStyle);
                }
            }
        }

        internal override void UpdateAllGuids()
        {
            base.UpdateAllGuids();

            foreach (EmbeddedPage page in EmbeddedPages)
            {
                page.UpdateAllGuids();
            }
        }

        internal override void UpdateActions()
        {
            base.UpdateActions();

            foreach (EmbeddedPage page in EmbeddedPages)
            {
                page.UpdateActions();
            }
        }
    }
}
