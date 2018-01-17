using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal abstract class EmbeddedPage : Page, IEmbeddedPage
    {
        internal EmbeddedPage(string tagName, string pageName)
            : base(tagName)
        {
            _pageData = new PageData(this, tagName);
            _pageData.Name = pageName;        
        }

        public override void Dispose()
        {
            lock (this)
            {
                base.Dispose();

                _pageData.Clear();
            }
        }

        public override void AddWidget(IWidget widget)
        {
            if (widget == null)
            {
                throw new ArgumentNullException("widget");
            }

            if (widget is IPageEmbeddedWidget)
            {
                throw new CannotAddWidgetException("Cannot add PageEmbeddedWidget to an EmbeddedPage.");
            }

            base.AddWidget(widget);
        }

        public override void AddMaster(IMaster master)
        {
            throw new CannotAddMasterException("Cannot add Master to an EmbeddedPage.");
        }

        public abstract IPageEmbeddedWidget ParentWidget { get; }

        public override IDocument ParentDocument
        {
            get
            {
                return ParentWidget.ParentPage == null ? null : ParentWidget.ParentPage.ParentDocument;
            }

            set
            {
                // Do nothing, embedded page is resided in a page embedded widget, 
                // you cannot set parent docuemnt of embedded page directly.
            }
        }

        public override void Close()
        {
            lock (this)
            {
                base.Close();

                // In embedded page, just clear page data.
                _pageData.Clear();

                DocumentPage parentPage = ParentWidget.ParentPage as DocumentPage;
                if (parentPage != null)
                {
                    parentPage.OnEmbeddedPageClosed();
                }
            }
        }

        internal override void OnDeleteFromDocument()
        {
            lock (this)
            {
                // Here, the parent document must exist.
                Document document = ParentDocument as Document;
                if (document == null)
                {
                    throw new CannotDeletePageException("Failed to delete from document, the document is null.");
                }

                document.AllPages.Remove(Guid);

                document.ImagesData.DeleteConsumersInPage(Guid);

                // Broadcast add event to all widgets in this page.
                foreach (Widget widget in PageData.Widgets)
                {
                    widget.OnDeleteFromDocument(true);
                }

                if (IsOpened)
                {
                    Close();
                }
            }
        }
        
        internal override PageData PageData
        {
            get { return _pageData; }
        }

        protected override void InitializeWorkingDirectory()
        {
            if (_workingDirectory == null)
            {
                Page parentPage = ParentWidget.ParentPage as Page;
                Document document = ParentDocument as Document;
                if (document == null || document.WorkingPagesDirectory == null ||
                    parentPage == null || parentPage.WorkingDirectory == null)
                {
                    throw new Exception("Cannot initialize embadded page working directory.");
                }

                // Create page working directory once it is added in this document if the directory doesn't exist.
                // Embedded page working directory resides in parent page working directory.
                _workingDirectory = new DirectoryInfo(Path.Combine(document.WorkingPagesDirectory.FullName,
                                                           parentPage.Guid.ToString(), _pageData.Guid.ToString()));
                if (!_workingDirectory.Exists)
                {
                    _workingDirectory.Create();
                    _workingDirectory.CreateSubdirectory(Document.IMAGES_FOLDER_NAME);
                }

                if (!Directory.Exists(Path.Combine(_workingDirectory.FullName, Document.IMAGES_FOLDER_NAME)))
                {
                    _workingDirectory.CreateSubdirectory(Document.IMAGES_FOLDER_NAME);
                }
            }
        }

        private PageData _pageData;
    }
}
