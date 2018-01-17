using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_DynamicPanel : TestCase
    {
        public Test_DynamicPanel()
            : base("Test_DynamicPanel")
        {
        }

        public override string Description
        {
            get
            {
                return "Test widgets dynamic panel:\n" +
                    "* Create a document.\n" +
                    "* Create a new page.\n" +
                    "* Create a dynamic panel in the new page.\n" +
                    "* Test dynamic panel methods, like move, delete and so on.\n" +
                    "* Save the document.";
            }
        }

        protected override void RunInternal()
        {
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

            IDynamicPanel dynamicPanel = baseView.CreateWidget(WidgetType.DynamicPanel) as IDynamicPanel;
            dynamicPanel.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            dynamicPanel.WidgetStyle.Height = 198;
            dynamicPanel.WidgetStyle.Width = 152;
            dynamicPanel.WidgetStyle.X = 0;
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

            //dynamicPanel.MovePanelStatePageTo(dynamicPanel.StartPanelStatePage, dynamicPanel.PanelStatePages.Count - 1);

            // Close the page if you don't want to work on it.
            page.Close();

            // Save the document to a pn file.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);

            // Close this document when you don't work on it anymore.
            Program.Service.Close();
        }
    }
}
