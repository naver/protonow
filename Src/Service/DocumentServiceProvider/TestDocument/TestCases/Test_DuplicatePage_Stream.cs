using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_DuplicatePage_Stream : TestCase
    {
        public Test_DuplicatePage_Stream()
            : base("Test_DuplicatePage_Stream")
        {
        }

        public override string Description
        {
            get
            {
                return "Test duplicating page via stream, this is used typically across document:\n" +
                    "* Create a document.\n" +
                    "* Create a new page 1.\n" +
                    "* Create some widgets in the page 1.\n" +
                    "* Add a new page with stream of page 1 .\n" +
                    "* Save the document.";
            }
        }

        protected override void RunInternal()
        {
            // Create a new document.
            Program.Service.NewDocument(DocumentType.Standard);
            IDocument document = Program.Service.Document;
            
            // Create a new page 1.
            IDocumentPage page1 = document.CreatePage("Page 1");
            ITreeNode node1 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node1.AttachedObject = page1;

            page1.Open();

            // Get the page view for base adaptive view.
            IPageView baseView1 = page1.PageViews[document.AdaptiveViewSet.Base.Guid];

            // Create a image.
            IImage image = baseView1.CreateWidget(WidgetType.Image) as IImage;
            image.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            image.WidgetStyle.Height = 267;
            image.WidgetStyle.Width = 116;
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

            // Create a hamburger menu.
            IHamburgerMenu hamburgerMenu = baseView1.CreateWidget(WidgetType.HamburgerMenu) as IHamburgerMenu;
            hamburgerMenu.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            hamburgerMenu.WidgetStyle.Height = 280;
            hamburgerMenu.WidgetStyle.Width = 150;
            hamburgerMenu.WidgetStyle.X = 200;
            hamburgerMenu.WidgetStyle.Y = 200;
            hamburgerMenu.WidgetStyle.Z = 1;
            hamburgerMenu.Name = "HamburgerMenu 1";
            hamburgerMenu.Tooltip = "A hamburger menu.";

            // Menu botton
            hamburgerMenu.MenuButton.WidgetStyle.Height = 50;
            hamburgerMenu.MenuButton.WidgetStyle.Width = 50;
            hamburgerMenu.MenuButton.WidgetStyle.X = 200;
            hamburgerMenu.MenuButton.WidgetStyle.Y = 200;
            hamburgerMenu.MenuButton.WidgetStyle.Z = 0;

            // Menu page,  add a shape
            IPage menuPage = hamburgerMenu.MenuPage;
            menuPage.Open(); // Open page to edit.
            
            // Get the base view of menu page.
            IPageView menuBaseView = menuPage.PageViews[document.AdaptiveViewSet.Base.Guid];

            IShape diamond = menuBaseView.CreateWidget(WidgetType.Shape) as IShape;
            diamond.ShapeType = ShapeType.Diamond;
            diamond.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            diamond.WidgetStyle.Height = 100;
            diamond.WidgetStyle.Width = 100;
            diamond.Name = "Diamond 1";
            diamond.Tooltip = "A Diamond.";
            diamond.SetRichText("Diamond");
            menuPage.Close(); // Close Page to release resources.

            // Create a serialize writer first.
            ISerializeWriter writer = document.CreateSerializeWriter(document.AdaptiveViewSet.Base.Guid);

            // Add the new page 1 to the writer.
            writer.AddPage(page1);

            // Serialize page 1 to a stream.
            Stream stream = writer.WriteToStream();
            
            // The data of page is already in stream, so close the page will not change anything in stream.
            page1.Close();

            // Add pages in stream. 
            IObjectContainer container = document.AddPages(stream);
            stream.Close();

            foreach(IStandardPage page in container.StandardPageList)
            {
                ITreeNode node22 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
                node22.AttachedObject = page;
            }

            // Save document.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);

            // Close document.
            Program.Service.Close();
        }
    }
}
