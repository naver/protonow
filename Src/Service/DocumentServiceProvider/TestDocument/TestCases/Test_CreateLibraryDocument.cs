using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_CreateLibraryDocument : TestCase
    {
        public Test_CreateLibraryDocument()
            : base("Test_CreateLibraryDocument")
        {
        }

        public override string Description
        {
            get
            {
                return "Test creating a library document which contains some custom objects:\n" +
                    "* Create a library document.\n" +
                    "* Create 2 new custom objects.\n" +
                    "* Save the library document.";
            }
        }

        protected override void RunInternal()
        {
            // Create a document and Document type is "DocumentType.Library" !!!!!!!!
            Program.Service.NewDocument(DocumentType.Library);  
            IDocument document = Program.Service.Document;

            // Create custom object 1, a custom object is actually a page.
            IDocumentPage page1 = document.CreatePage("CustomObject 1");
            ITreeNode node1 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node1.AttachedObject = page1;

            // Open the page.
            page1.Open();

            // Get the page view for base adaptive view.
            IPageView baseView1 = page1.PageViews[document.AdaptiveViewSet.Base.Guid];

            // Create a image widget in this custom object.
            IImage image = baseView1.CreateWidget(WidgetType.Image) as IImage;
            image.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            image.WidgetStyle.Height = 267;
            image.WidgetStyle.Width = 116;
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

            // Create a flicking widget in this custom object.
            IDynamicPanel dynamicPanel = baseView1.CreateWidget(WidgetType.DynamicPanel) as IDynamicPanel;
            dynamicPanel.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            dynamicPanel.WidgetStyle.Height = 198;
            dynamicPanel.WidgetStyle.Width = 152;
            dynamicPanel.WidgetStyle.X = 0;
            dynamicPanel.WidgetStyle.Y = 250;
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

            // Set the icon and tooltip for the library 1
            ICustomObjectPage objectPage1 = page1 as ICustomObjectPage;
            objectPage1.Tooltip = "This custom object contains a image and a dynamic panel which has 3 panels.";

            string iconFile1 = Path.Combine(Program.WORKING_IMAGES_DIRECTORY, "icon", "1.png");
            if (File.Exists(iconFile1))
            {
                using (FileStream fileStream = new FileStream(iconFile1, FileMode.Open, FileAccess.Read))
                {
                    MemoryStream imageStream = new MemoryStream();
                    fileStream.CopyTo(imageStream);
                    objectPage1.Icon = imageStream;
                }
            }

            // Close the custom object page.
            page1.Close();

            // Create custom object 2
            IDocumentPage page2 = document.CreatePage("CustomObject 2");
            ITreeNode node2 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node2.AttachedObject = page2;

            page2.Open();

            // Get the page view for base adaptive view.
            IPageView baseView2 = page2.PageViews[document.AdaptiveViewSet.Base.Guid];

            // Create a SVG widget in this library.
            ISvg svg = baseView2.CreateWidget(WidgetType.SVG) as ISvg;
            svg.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            svg.WidgetStyle.Height = 117;
            svg.WidgetStyle.Width = 150;
            svg.Name = "airplane 03";
            svg.Tooltip = "A airplane svg";

            string svgFile = Path.Combine(Program.WORKING_IMAGES_DIRECTORY, "Svg", "airplane 03.svg");
            if (File.Exists(svgFile))
            {
                using (FileStream fileStream = new FileStream(svgFile, FileMode.Open, FileAccess.Read))
                {
                    MemoryStream svgStream = new MemoryStream();
                    fileStream.CopyTo(svgStream);
                    svg.XmlStream = svgStream;
                }
            }

            // Set the icon for the library 2
            ICustomObjectPage objectPage2 = page2 as ICustomObjectPage;

            string iconFile2 = Path.Combine(Program.WORKING_IMAGES_DIRECTORY, "icon", "2.png");
            if (File.Exists(iconFile2))
            {
                using (FileStream fileStream = new FileStream(iconFile2, FileMode.Open, FileAccess.Read))
                {
                    MemoryStream imageStream = new MemoryStream();
                    fileStream.CopyTo(imageStream);
                    objectPage2.Icon = imageStream;
                }
            }

            page2.Close();

            // Save to a library file .libpn. The file extension is .libpn!!!!!
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".libpn");  
            Program.Service.Save(fileName);

            // Close the library document after you are done modifying libraries.
            Program.Service.Close();
        }
    }
}
