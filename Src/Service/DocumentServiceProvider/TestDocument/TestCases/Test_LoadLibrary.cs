using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_LoadLibrary : TestCase        
    {
        public Test_LoadLibrary()
            : base("Test_LoadLibrary")
        {
        }

        public override string Description
        {
            get
            {
                return "Test loading libraries:\n" +
                    "* Load some .libpn files.\n" +
                    "* Create a document.\n" +
                    "* Create 2 new pages.\n" +
                    "* Add the custom objects to the new pages.\n" +
                    "* Save the document.";
            }
        }

        protected override void RunInternal()
        {
            // Load some .libpn files created in other test case.

            // The .libpn is actually a library and DocumentService will manage them when they are loaded.
            // DocumentSerive has a limitation of loaded libraries, if it exceeds the MAX_CACHE_COUNT in 
            // DocumentService, DocumentService will delete the LRU library.

            // Load library 1.
            string libraryFileName1 = Path.Combine(Program.WORKING_DIRECTORY, "Test_CreateLibraryInPage.libpn");
            ILibrary library1 = Program.Service.LibraryManager.LoadLibrary(libraryFileName1);

            // Load library 2.
            string libraryFileName2 = Path.Combine(Program.WORKING_DIRECTORY, "Test_CreateLibraryDocument.libpn");
            ILibrary library2 = Program.Service.LibraryManager.LoadLibrary(libraryFileName2);

            // Create a new document.
            Program.Service.NewDocument(DocumentType.Standard);
            IDocument document = Program.Service.Document;

            // Add libraries in library1 into the new page 1.
            IDocumentPage page1 = document.CreatePage("Page 1");
            ITreeNode node1 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node1.AttachedObject = page1;

            page1.Open();

            // Get the page view for base adaptive view.
            IPageView baseView1 = page1.PageViews[document.AdaptiveViewSet.Base.Guid];

            // Enumerate custom objects in the library1, you also can get custom object with the custom object guid 
            // if you cache the guids in other place.
            foreach (ICustomObject customObject in library1.CustomObjects)
            {
                baseView1.AddCustomObject(customObject, 100, 100);  // 100x100 is the new base location of the custom object.
            }

            // Close the page.
            page1.Close();

            // Add custom objects in library2 into new page 2
            IDocumentPage page2 = document.CreatePage("Page 2");
            ITreeNode node2 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node2.AttachedObject = page2;

            page2.Open();

            // Get the page view for base adaptive view.
            IPageView baseView2 = page2.PageViews[document.AdaptiveViewSet.Base.Guid];

            foreach (ICustomObject customObject in library2.CustomObjects)
            {
                baseView2.AddCustomObject(customObject);
            }

            page2.Close();

            // Save the document.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);

            // Close the document.
            Program.Service.Close();

            // You can delete/remove/unloaded libraries explicitly from DocumentService if you don't want it.
            Program.Service.LibraryManager.DeleteLibrary(library1.Guid);
            Program.Service.LibraryManager.DeleteLibrary(library2.Guid);
        }
    }
}
