using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal abstract class DocumentPage : Page, IDocumentPage
    {
        internal DocumentPage(string tagName, Document document)
            : base(tagName)
        {
            _document = document;
        }
                
        public override void Dispose()
        {
            lock (this)
            {
                base.Dispose();

                foreach (PageEmbeddedWidget widget in PageData.Widgets.OfType<PageEmbeddedWidget>())
                {
                    foreach (EmbeddedPage embeddedPage in widget.EmbeddedPages)
                    {
                        embeddedPage.Dispose();
                    }
                }

                PageData.Clear();
            }
        }

        public override IDocument ParentDocument
        {
            get { return _document; }
            set { _document = value as Document; }
        }

        public override void Close()
        {
            lock(this)
            {
                base.Close();

                TryToClearPageData();
            }
        }

        internal override void OnAddToDocument()
        {
            base.OnAddToDocument();

            // Enumerate sub directories if the page is closed, and add embedded page guid to the AllPages.
            if (!IsOpened)
            {
                // Here, document and _workingDirectory must not be null.
                Document document = ParentDocument as Document;
                foreach (DirectoryInfo pageDir in _workingDirectory.EnumerateDirectories("*", SearchOption.AllDirectories))
                {
                    Guid guid;
                    if (Guid.TryParse(pageDir.Name, out guid))
                    {
                        // Only add the key, the value is null untill you open the page.
                        if (!document.AllPages.ContainsKey(guid))
                        {
                            document.AllPages.Add(guid, null);
                        }
                    }
                }
            }
        }

        internal override void OnDeleteFromDocument()
        {
            lock (this)
            {
                Document document = ParentDocument as Document;
                if (document == null)
                {
                    throw new CannotDeletePageException("Failed to delete page from document, the document is null.");
                }

                // Make sure remove this page from AllPages collection first, this way widgts could know this page is deleted.
                document.AllPages.Remove(Guid);

                document.ImagesData.DeleteConsumersInPage(Guid);

                if (!PageData.IsCleared)
                {
                    // Page data is not cleared
                    foreach (Widget widget in PageData.Widgets)
                    {
                        widget.OnDeleteFromDocument(true);
                    }
                }
                else
                {
                    if (_workingDirectory != null)
                    {
                        // Page data was cleared, remove child page from document AllPages based on directory information. 
                        foreach (DirectoryInfo pageDir in _workingDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                        {
                            Guid guid;
                            if (Guid.TryParse(pageDir.Name, out guid))
                            {
                                document.AllPages.Remove(guid);

                                document.ImagesData.DeleteConsumersInPage(guid);
                            }
                        }
                    }
                }

                if (IsOpened)
                {
                    Close();
                }
            }
        }

        internal void OnEmbeddedPageClosed()
        {
            Document document = ParentDocument as Document;
            if (document != null)
            {
                // Try to clear page data only if this page is closed and page data is not cleared, 
                // and this page is still in the document.

                // If this page is not in the document, this method must be fired by Page.OnDeleteFromDocument()
                // and it try to close all embedded pages. So when page is deleting, we don't care for clearing page data here.
                if (!IsOpened && !PageData.IsCleared && document.AllPages.ContainsKey(Guid))
                {
                    TryToClearPageData();
                }
            }
        }

        protected override void InitializeWorkingDirectory()
        {
            if (_workingDirectory == null)
            {
                Document document = ParentDocument as Document;
                if (document == null || document.WorkingPagesDirectory == null)
                {
                    throw new Exception("Cannot initialize document page working directory.");
                }

                // Create page working directory once it is added in this document if the directory doesn't exist.
                _workingDirectory = new DirectoryInfo(Path.Combine(document.WorkingPagesDirectory.FullName,
                                                                   PageData.Guid.ToString()));
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

        // Check if there is opened embedded page, if so, do not clear the page data so that
        // embedded page can have the same parent widget if parent page is reopened.
        private void TryToClearPageData()
        {
            List<Guid> embeddedPageGuidList = new List<Guid>();
            if (!HasOpenedEmbeddedPage(embeddedPageGuidList))
            {
                PageData.Clear();

                // Set embedded page to null in AllPages because new embedded page object will be created next time when load page data.
                Document document = ParentDocument as Document;
                if (document != null)
                {
                    foreach (Guid guid in embeddedPageGuidList)
                    {
                        if (document.AllPages.ContainsKey(guid))
                        {
                            document.AllPages[guid] = null;
                        }
                    }
                }
            }
        }

        private bool HasOpenedEmbeddedPage(List<Guid> embeddedPageGuidList)
        {
            foreach (PageEmbeddedWidget widget in PageData.Widgets.OfType<PageEmbeddedWidget>())
            {
                foreach (EmbeddedPage page in widget.EmbeddedPages)
                {
                    if (page.IsOpened)
                    {
                        return true;
                    }

                    embeddedPageGuidList.Add(page.Guid);
                }
            }

            return false;
        }

        protected Document _document;
        protected bool? _containsPageEmbeddedWidget = null;

    }
}
