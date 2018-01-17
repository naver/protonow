using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_ZipPages: TestCase
    {
        public Test_ZipPages()
            : base("Test_ZipPages")
        {
        }


        public override string Description
        {
            get
            {
                return "Test delete standard page and directory filter in fastzip:\n" +
                    "* Create a document.\n" +
                    "* Create lots of pages.\n" +
                    "* Create menu, toast and dynamic panel in new pages.\n" +
                    "* Delete a page.\n" +
                    "* Save the document and check if deleted page is not in zip file.";

            }
        }

        protected override void RunInternal()
        {
            // Create a new document.
            Program.Service.NewDocument(DocumentType.Standard);
            IDocument document = Program.Service.Document;

            // Create a page.
            IDocumentPage page1 = document.CreatePage("Page 1");

            // Create the page node in page tree.
            ITreeNode node = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node.AttachedObject = page1;

            // Must open the page before you read and modify it.
            page1.Open();

            // Get the page view for base adaptive view.
            IPageView baseView = page1.PageViews[document.AdaptiveViewSet.Base.Guid];

            IHamburgerMenu hamburgerMenu = baseView.CreateWidget(WidgetType.HamburgerMenu) as IHamburgerMenu;
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

            // Close the page if you don't want to work on it.
            page1.Close();


            // Create a page.
            IDocumentPage page2 = document.CreatePage("Page 2");

            // Create the page node in page tree.
            ITreeNode node2 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node2.AttachedObject = page2;

            // Must open the page before you read and modify it.
            page2.Open();

            // Get the page view for base adaptive view.
            IPageView baseView2 = page2.PageViews[document.AdaptiveViewSet.Base.Guid];

            IToast toast = baseView2.CreateWidget(WidgetType.Toast) as IToast;
            toast.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            toast.WidgetStyle.Height = 146;
            toast.WidgetStyle.Width = 298;
            toast.WidgetStyle.X = 200;
            toast.WidgetStyle.Y = 700;
            toast.WidgetStyle.Z = 15;
            toast.Name = "Toast 1";
            toast.Tooltip = "A ExposureTime toast.";
            toast.ExposureTime = 3;
            toast.DisplayPosition = ToastDisplayPosition.UserSetting;
            toast.CloseSetting = ToastCloseSetting.CloseButton;

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

            // Close the page if you don't want to work on it.
            page2.Close();


            // Create a page.
            IDocumentPage page3 = document.CreatePage("Page 3");

            // Create the page node in page tree.
            ITreeNode node3 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node3.AttachedObject = page3;

            // Must open the page before you read and modify it.
            page3.Open();

            // Get the page view for base adaptive view.
            IPageView baseView3 = page3.PageViews[document.AdaptiveViewSet.Base.Guid];

            IDynamicPanel dynamicPanel = baseView3.CreateWidget(WidgetType.DynamicPanel) as IDynamicPanel;
            dynamicPanel.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            dynamicPanel.WidgetStyle.Height = 198;
            dynamicPanel.WidgetStyle.Width = 152;
            dynamicPanel.WidgetStyle.X = 500;
            dynamicPanel.WidgetStyle.Y = 700;
            dynamicPanel.WidgetStyle.Z = 16;
            dynamicPanel.IsAutomatic = true;

            // Set start panel page as the first created page.
            dynamicPanel.StartPanelStatePage = dynamicPanel.CreatePanelStatePage("Panel 1");
            dynamicPanel.CreatePanelStatePage("Panel 2");
            dynamicPanel.CreatePanelStatePage("Panel 3");

            int imageFileName = 1;
            foreach (IPage statePage in dynamicPanel.PanelStatePages)
            {
                statePage.Open(); // Open page to edit.

                // Get the base view of state page.
                IPageView stateBaseView = statePage.PageViews[document.AdaptiveViewSet.Base.Guid];

                IImage statePageImage = stateBaseView.CreateWidget(WidgetType.Image) as IImage;
                statePageImage.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
                statePageImage.WidgetStyle.Height = 198;
                statePageImage.WidgetStyle.Width = 152;
                string statePageImageFile = Path.Combine(Program.WORKING_IMAGES_DIRECTORY, "HangGame", imageFileName + ".png");
                if (File.Exists(statePageImageFile))
                {
                    using (FileStream fileStream = new FileStream(statePageImageFile, FileMode.Open, FileAccess.Read))
                    {
                        MemoryStream imageStream = new MemoryStream();
                        fileStream.CopyTo(imageStream);
                        statePageImage.ImageStream = imageStream;
                    }
                }
                statePage.Close(); // Close Page to release resources.

                imageFileName++;
            }

            // Close the page if you don't want to work on it.
            page3.Close();

            document.DeletePage(page3.Guid);
            document.DocumentSettings.LayoutSetting.PageTree.RemoveChild(node3);

            // Save the document to a pn file.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);

            // Close this document when you don't work on it anymore.
            Program.Service.Close();

        }
    }
}
