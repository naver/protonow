using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_CopyWidgets : TestCase
    {
        public Test_CopyWidgets()
            : base("Test_CopyWidgets")
        {
        }

        public override string Description
        {
            get
            {
                return "Test copy widgets and groups during pages:\n" +
                    "* Create a document.\n" +
                    "* Create 2 new pages.\n" +
                    "* Create some widgets and groups in the new page 1.\n" +
                    "* Copy widgets and groups in the new page 1 to new page 1 and new page 2.\n" +
                    "* Save the document.";
            }
        }

        protected override void RunInternal()
        {
            Program.Service.NewDocument(DocumentType.Standard);
            IDocument document = Program.Service.Document;
            
            IDocumentPage page1 = document.CreatePage("Page 1");
            ITreeNode node1 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node1.AttachedObject = page1;

            page1.Open();

            // Get the page view for base adaptive view.
            IPageView baseView1 = page1.PageViews[document.AdaptiveViewSet.Base.Guid];

            IImage image = baseView1.CreateWidget(WidgetType.Image) as IImage;
            image.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            image.WidgetStyle.Height = 267;
            image.WidgetStyle.Width = 116;
            image.Name = "4.png";
            image.Tooltip = "A png image has 116 x 267 in size";

            // Creata a image widget.
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

            // Create a flicking widget.
            IDynamicPanel dynamicPanel = baseView1.CreateWidget(WidgetType.DynamicPanel) as IDynamicPanel;
            dynamicPanel.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            dynamicPanel.WidgetStyle.Height = 198;
            dynamicPanel.WidgetStyle.Width = 152;
            dynamicPanel.WidgetStyle.X = 250;
            dynamicPanel.WidgetStyle.Y = 250;
            dynamicPanel.WidgetStyle.Z = 1;
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

            // Create a group 
            List<Guid> widgetGuidList = new List<Guid>();
            widgetGuidList.Add(image.Guid);
            widgetGuidList.Add(dynamicPanel.Guid);
            IGroup group = page1.CreateGroup(widgetGuidList);

            // Create a serialize writer.
            ISerializeWriter writer = document.CreateSerializeWriter(document.AdaptiveViewSet.Base.Guid);

            // Add widgets and groups you want to copy to the writer.
            writer.AddGroup(group);

            // Serialize widgets and groups to a stream.
            Stream stream = writer.WriteToStream();

            // Paste stream back to page 1.
            IObjectContainer container = baseView1.AddObjects(stream);

            // Change the new widgets location.
            foreach(IWidget widget in container.WidgetList)
            {
                widget.WidgetStyle.X = widget.WidgetStyle.X + 50;
                widget.WidgetStyle.Y = widget.WidgetStyle.Y + 50;
            }

            page1.Close();

            // Paste stream to page 2.
            IDocumentPage page2 = document.CreatePage("Page 2");
            ITreeNode node2 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node2.AttachedObject = page2;

            page2.Open();

            // Get the page view for base adaptive view.
            IPageView baseView2 = page2.PageViews[document.AdaptiveViewSet.Base.Guid];

            baseView2.AddObjects(stream); // Paste to page 2.

            page2.Close();
                                 
            // Save document.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);
            Program.Service.Close();
        }
    }
}
