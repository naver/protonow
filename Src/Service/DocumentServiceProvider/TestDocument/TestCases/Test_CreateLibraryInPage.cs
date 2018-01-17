using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_CreateLibraryInPage : TestCase
    {
        public Test_CreateLibraryInPage()
            : base("Test_CreateLibraryInPage")
        {
        }

        public override string Description
        {
            get
            {
                return "Test creating a library in a page:\n" +
                    "* Create a document.\n" +
                    "* Create a new page.\n" +
                    "* Create some widgets in the new page.\n" +
                    "* Create a custom object in a new library document based on the selected widgets in the new page.\n" +
                    "* Export the new library to a .libpn file";
            }
        }

        protected override void RunInternal()
        {
            // Create a document.
            Program.Service.NewDocument(DocumentType.Standard);  
            IDocument document = Program.Service.Document;

            // Create a new page.
            IDocumentPage page1 = document.CreatePage("Page 1");
            ITreeNode node1 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node1.AttachedObject = page1;

            page1.Open();

            // Get the page view for base adaptive view.
            IPageView baseView1 = page1.PageViews[document.AdaptiveViewSet.Base.Guid];

            // Create a image widget in the new page.
            IImage image = baseView1.CreateWidget(WidgetType.Image) as IImage;
            image.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            image.WidgetStyle.Height = 267;
            image.WidgetStyle.Width = 116;
            image.WidgetStyle.WidgetRotate = 60;
            image.WidgetStyle.X = 100;
            image.WidgetStyle.Y = 100;
            image.Name = "4.png";
            image.Tooltip = "A png image has 116 x 267 in size";

            string imageFile = Path.Combine(Program.WORKING_IMAGES_DIRECTORY, "HangGame", "4.png");
            if (File.Exists(imageFile))
            {
                using (FileStream fileStream = new FileStream(imageFile, FileMode.Open, FileAccess.Read))
                {
                    MemoryStream imageStream = new MemoryStream();
                    fileStream.CopyTo(imageStream);
                    image.ImageStream = imageStream;
                }
            }

            // Create a serialize writer first.
            ISerializeWriter writer = document.CreateSerializeWriter(document.AdaptiveViewSet.Base.Guid);

            // Add selected widgets in the writer.
            writer.AddWidget(image);

            MemoryStream iconStream = new MemoryStream();
            string iconFile = Path.Combine(Program.WORKING_IMAGES_DIRECTORY, "icon", "3.png");
            if (File.Exists(iconFile))
            {
                using (FileStream fileStream = new FileStream(iconFile, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(iconStream);
                }
            }

            // Create a custom object which name is "CustomObjectName", the custom object is contains the widgets 
            // you added in ISerializeWriter.
            // The new custom object is added to a new library document and the new library is managed by DocumentService.
            ILibrary library = Program.Service.LibraryManager.CreateLibrary(writer, "CustomObjectName", iconStream, null);

            iconStream.Dispose();

            // Save the new library to a library file .libpn, the file extension is .libpn!!!!!!!
            string libraryFileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".libpn");
            library.Save(libraryFileName);

            // Close the page.
            page1.Close();

            // Close the document.
            Program.Service.Close();

            // Delete the new library from DocumentService.
            Program.Service.LibraryManager.DeleteLibrary(library.Guid);
        }
    }
}
