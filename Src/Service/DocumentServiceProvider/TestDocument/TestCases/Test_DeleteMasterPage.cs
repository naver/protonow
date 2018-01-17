using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_DeleteMasterPage : TestCase
    {
        public Test_DeleteMasterPage()
            : base("Test_DeleteMasterPage")
        {
        }


        public override string Description
        {
            get
            {
                return "Test delete master page and its consumers.\n"; 
            }
        }

        protected override void RunInternal()
        {
            Program.Service.NewDocument(DocumentType.Standard);
            IDocument document = Program.Service.Document;

            IMasterPage masterPage = document.CreateMasterPage("Master 1");
            ITreeNode masterPageNode = document.DocumentSettings.LayoutSetting.MasterPageTree.AddChild(TreeNodeType.MasterPage);
            masterPageNode.AttachedObject = masterPage;
            masterPage.Open(); 
            IPageView masterBaseView = masterPage.PageViews[document.AdaptiveViewSet.Base.Guid];

            IButton button = masterBaseView.CreateWidget(WidgetType.Button) as IButton;
            button.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());

            // Size
            button.WidgetStyle.Height = 30;
            button.WidgetStyle.Width = 100;

            // Location
            button.WidgetStyle.X = 0;
            button.WidgetStyle.Y = 0;
            button.WidgetStyle.Z = 0;

            // Text things
            button.Name = "Button 1";
            button.Text = "Button";
            button.Tooltip = "Html button.";

            IImage image = masterBaseView.CreateWidget(WidgetType.Image) as IImage;
            image.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            image.WidgetStyle.Height = 267;
            image.WidgetStyle.Width = 116;
            image.WidgetStyle.X = 150;
            image.WidgetStyle.Y = 100;
            image.WidgetStyle.Z = 5;
            image.Name = "4.png";
            image.Tooltip = "A png image has 116 x 267 in size";

            // It is a png image by default. Set image stream
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

            IShape lable = masterBaseView.CreateWidget(WidgetType.Shape) as IShape;
            lable.ShapeType = ShapeType.Paragraph;
            lable.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            lable.WidgetStyle.Height = 100;
            lable.WidgetStyle.Width = 200;
            lable.WidgetStyle.X = 150;
            lable.WidgetStyle.Y = 300;
            lable.WidgetStyle.Z = 9;
            lable.Name = "Label 1";
            lable.Tooltip = "A label.";
            lable.SetRichText("Label");
            lable.WidgetStyle.LineColor = new StyleColor(ColorFillType.Solid, -16777216);
            lable.WidgetStyle.LineWidth = 0; // No border
            lable.WidgetStyle.FillColor = new StyleColor(ColorFillType.Solid, 16777215); // Transparent
            lable.WidgetStyle.HorzAlign = Alignment.Left;
            lable.WidgetStyle.VertAlign = Alignment.Top;

            IDocumentPage page = document.CreatePage("Page 1");
            ITreeNode pageNode = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            pageNode.AttachedObject = page;
            page.Open();
            IPageView baseView = page.PageViews[document.AdaptiveViewSet.Base.Guid];
            IMaster master = baseView.CreateMaster(masterPage.Guid);

            IDocumentPage page2 = document.CreatePage("Page 2");
            ITreeNode pageNode2 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            pageNode2.AttachedObject = page2;
            page2.Open();
            IPageView baseView2 = page2.PageViews[document.AdaptiveViewSet.Base.Guid];
            IMaster master2 = baseView2.CreateMaster(masterPage.Guid);

            IHamburgerMenu hamburgerMenu = baseView2.CreateWidget(WidgetType.HamburgerMenu) as IHamburgerMenu;
            hamburgerMenu.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            hamburgerMenu.WidgetStyle.Height = 280;
            hamburgerMenu.WidgetStyle.Width = 150;
            hamburgerMenu.WidgetStyle.X = 0;
            hamburgerMenu.WidgetStyle.Y = 700;
            hamburgerMenu.WidgetStyle.Z = 14;
            hamburgerMenu.Name = "HamburgerMenu 1";
            hamburgerMenu.Tooltip = "A hamburger menu.";

            // Menu botton
            hamburgerMenu.MenuButton.WidgetStyle.Height = 50;
            hamburgerMenu.MenuButton.WidgetStyle.Width = 50;
            hamburgerMenu.MenuButton.WidgetStyle.X = 0;
            hamburgerMenu.MenuButton.WidgetStyle.Y = 700;
            hamburgerMenu.MenuButton.WidgetStyle.Z = 0;

            // Menu page,  add a shape
            IPage menuPage = hamburgerMenu.MenuPage;
            menuPage.Open(); // Open page to edit.

            // Get the base view of menu page.
            IPageView menuBaseView = menuPage.PageViews[document.AdaptiveViewSet.Base.Guid];

            // Create widgts on the base view in the menu page.
            IShape diamond = menuBaseView.CreateWidget(WidgetType.Shape) as IShape;
            diamond.ShapeType = ShapeType.Diamond;
            diamond.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            diamond.WidgetStyle.Height = 100;
            diamond.WidgetStyle.Width = 100;
            diamond.Name = "Diamond 1";
            diamond.Tooltip = "A Diamond.";
            diamond.SetRichText("Diamond");
            menuPage.Close(); // Close Page to release resources.

            page2.Close();

            // Save the document to a pn file.
            string before = Path.Combine(Program.WORKING_DIRECTORY, _caseName + "_Before.pn");
            Program.Service.Save(before);

            // Delete master page.
            document.DeleteMasterPage(masterPage.Guid);
            document.DocumentSettings.LayoutSetting.MasterPageTree.RemoveChild(masterPageNode);

            string after = Path.Combine(Program.WORKING_DIRECTORY, _caseName + "_After_DeleteFromOpenedPage.pn");
            Program.Service.Save(after);

            page2.Open();
            string after2 = Path.Combine(Program.WORKING_DIRECTORY, _caseName + "_After_DeleteFromClosedPage.pn");
            Program.Service.Save(after2);


            // Close this document when you don't work on it anymore.
            Program.Service.Close();
        }
    }
}
