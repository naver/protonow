using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_AddCustomObjectToLibrary : TestCase
    {
        public Test_AddCustomObjectToLibrary()
            : base("Test_AddCustomObjectToLibrary")
        {
        }

        public override string Description
        {
            get
            {
                return "Test create a custom object and add it in an existing library:\n" +
                    "* Load a library.\n" +
                    "* Create a document.\n" +
                    "* Create a new page 1.\n" +
                    "* Create some widgets in the new page 1.\n" +
                    "* Create a new  custom object with some widgets in new page 1 and add it in the imported library.\n" +
                    "* Create a new page 2.\n" +
                    "* Add all custom objects in the imported library to the new page 2.\n" +
                    "* Save the document.";
            }
        }

        protected override void RunInternal()
        {
            // Load a library file
            string libraryFileName = Path.Combine(Program.WORKING_DIRECTORY, "Test_CreateLibraryDocument.libpn");
            ILibrary libraryLoad = Program.Service.LibraryManager.LoadLibrary(libraryFileName);

            // Cache the library guid.
            Guid libraryGuid = libraryLoad.Guid;

            // Create a new document.
            Program.Service.NewDocument(DocumentType.Standard);
            IDocument document = Program.Service.Document;

            // Create a page.
            IDocumentPage page = document.CreatePage("Home");

            // Create the page node in page tree.
            ITreeNode node = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node.AttachedObject = page;

            // Must open the page before you read and modify it.
            page.Open();

            // Get the page view for base adaptive view.
            IPageView baseView = page.PageViews[document.AdaptiveViewSet.Base.Guid];

            // Create widgets on the base view in this page.
            IButton button = baseView.CreateWidget(WidgetType.Button) as IButton;
            button.WidgetStyle.Height = 30;
            button.WidgetStyle.Width = 100;
            button.Name = "Button 1";
            button.Text = "Button";
            button.Tooltip = "Html button.";

            // Get the loaded library with the cached guid.
            ILibrary libraryGet = Program.Service.LibraryManager.GetLibrary(libraryGuid);

            // Put the button in a writer.
            ISerializeWriter writer = document.CreateSerializeWriter(document.AdaptiveViewSet.Base.Guid);
            writer.AddWidget(button);

            // Create a new custom object which contains the button and add to the loaded library.
            libraryGet.AddCustomObject(writer, "New_Custom_Object_Which_Contains_Button", null, null);

            // Save changes to file
            libraryGet.Save(libraryGet.Name);

            page.Close();

            // Save the document to a pn file.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);

            // Close this document when you don't work on it anymore.
            Program.Service.Close();

            // Delete loaded libraries
            Program.Service.LibraryManager.DeleteLibrary(libraryGuid);
        }
    }
}
