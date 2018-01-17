using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_AddCustomObjectToPage : TestCase        
    {
        public Test_AddCustomObjectToPage()
            : base("Test_AddCustomObjectToPage")
        {
        }

        public override string Description
        {
            get
            {
                return "Test loading Test_CreateLibraryInPage.libpn:\n" +
                    "* Create a document.\n" +
                    "* Create a new page.\n" +
                    "* Add the custom objects to the new page.\n" +
                    "* Save the document.";
            }
        }

        protected override void RunInternal()
        {
            // Load library 1.
            string libraryFileName1 = Path.Combine(Program.WORKING_DIRECTORY, "Test_CreateLibraryInPage.libpn");
            ILibrary library1 = Program.Service.LibraryManager.LoadLibrary(libraryFileName1);

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
                baseView1.AddCustomObject(customObject, 0, 0);  
            }

            foreach (ICustomObject customObject in library1.CustomObjects)
            {
                baseView1.AddCustomObject(customObject, 200, 200);  // 200 x 200 is the new base location of the custom object.
            }

            // Close the page.
            page1.Close();


            // Save the document.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);

            // Close the document.
            Program.Service.Close();

            // You can delete/remove/unloaded libraries explicitly from DocumentService if you don't want it.
            Program.Service.LibraryManager.DeleteLibrary(library1.Guid);
        }
    }
}
