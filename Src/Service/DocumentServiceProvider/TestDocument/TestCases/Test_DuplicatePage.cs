using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_DuplicatePage : TestCase
    {
        public Test_DuplicatePage()
            : base("Test_DuplicatePage")
        {
        }

        public override string Description
        {
            get
            {
                return "Test duplicating page:\n" +
                    "* Create a document.\n" +
                    "* Create 2 new pages.\n" +
                    "* Create some widgets in the new pages.\n" +
                    "* Close new page 1 and duplicate it.\n" +
                    "* Keep new page 2 openned and duplicate it.\n" +
                    "* Save the document.";
            }
        }

        protected override void RunInternal()
        {
            // Create a new document.
            Program.Service.NewDocument(DocumentType.Standard);
            IDocument document = Program.Service.Document;

            document.WidgetAnnotationFieldSet.CreateAnnotationField("CreatedTime", AnnotationFieldType.Text);

            // Create a new page.
            IDocumentPage closePage = document.CreatePage("Close Page");
            ITreeNode node1 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node1.AttachedObject = closePage;

            closePage.Open();

            // Get the page view for base adaptive view.
            IPageView baseView1 = closePage.PageViews[document.AdaptiveViewSet.Base.Guid];

            // Create a image in the new page.
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

            // Create a hamburger menu in the new page.
            IHamburgerMenu hamburgerMenu = baseView1.CreateWidget(WidgetType.HamburgerMenu) as IHamburgerMenu;
            hamburgerMenu.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            hamburgerMenu.WidgetStyle.Height = 280;
            hamburgerMenu.WidgetStyle.Width = 150;
            hamburgerMenu.WidgetStyle.X = 200;
            hamburgerMenu.WidgetStyle.Y = 200;
            hamburgerMenu.WidgetStyle.Z = 14;
            hamburgerMenu.Name = "HamburgerMenu 1";
            hamburgerMenu.Tooltip = "A hamburger menu.";

            // Menu botton
            hamburgerMenu.MenuButton.WidgetStyle.Height = 50;
            hamburgerMenu.MenuButton.WidgetStyle.Width = 50;
            hamburgerMenu.MenuButton.WidgetStyle.X = 600;
            hamburgerMenu.MenuButton.WidgetStyle.Y = 450;
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

            // Close the new page.
            closePage.Close();

            // Duplication the closed new page. then the document has two pages, they are the same except for 
            // the guids of page, widgets and groups.
            IDocumentPage newClosePage = document.DuplicatePage(closePage.Guid);
            newClosePage.Name = closePage.Name + "_Duplicated";
            ITreeNode node11 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node11.AttachedObject = newClosePage;

            // Create another new page.
            IDocumentPage openPage = document.CreatePage("Open Page");
            ITreeNode node2 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node2.AttachedObject = openPage;

            openPage.Open();

            // Get the page view for base adaptive view.
            IPageView baseView2 = openPage.PageViews[document.AdaptiveViewSet.Base.Guid];

            // Create a button.
            IButton button = baseView2.CreateWidget(WidgetType.Button) as IButton;
            button.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            button.WidgetStyle.Height = 30;
            button.WidgetStyle.Width = 100;
            button.WidgetStyle.X = 0;
            button.WidgetStyle.Y = 0;
            button.WidgetStyle.Z = 0;
            button.Name = "Button 1";
            button.Text = "Button";
            button.Tooltip = "Html button.";

            // Create a toast.
            IToast toast = baseView2.CreateWidget(WidgetType.Toast) as IToast;
            toast.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            toast.WidgetStyle.Height = 146;
            toast.WidgetStyle.Width = 298;
            toast.WidgetStyle.X = 650;
            toast.WidgetStyle.Y = 450;
            toast.WidgetStyle.Z = 15;
            toast.Name = "Toast 1";
            toast.Tooltip = "A ExposureTime toast.";
            toast.ExposureTime = 3;
            toast.DisplayPosition = ToastDisplayPosition.Top;
            toast.CloseSetting = ToastCloseSetting.ExposureTime;

            IPage toastPage = toast.ToastPage;
            toastPage.Open(); // Open page to edit.

            // Get the base view of toast page.
            IPageView toastBaseView = toastPage.PageViews[document.AdaptiveViewSet.Base.Guid];

            IShape ellipse = toastBaseView.CreateWidget(WidgetType.Shape) as IShape;
            ellipse.ShapeType = ShapeType.Ellipse;
            ellipse.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            ellipse.WidgetStyle.Height = 100;
            ellipse.WidgetStyle.Width = 100;
            ellipse.Name = "Ellipse 1";
            ellipse.Tooltip = "A Ellipse.";
            ellipse.SetRichText("Ellipse");
            toastPage.Close(); // Close Page to release resources.

            // Duplicate the opened page.
            IDocumentPage newOpenPage = document.DuplicatePage(openPage.Guid);
            newOpenPage.Name = openPage.Name + "_Duplicated";
            ITreeNode node22 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node22.AttachedObject = newOpenPage;

            // Close the page.
            openPage.Close();

            // Save document to pn file.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);

            // CLose document.
            Program.Service.Close();
        }
    }
}
