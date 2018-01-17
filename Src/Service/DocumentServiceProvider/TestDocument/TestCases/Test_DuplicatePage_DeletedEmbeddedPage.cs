using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_DuplicatePage_DeletedEmbeddedPage : TestCase
    {
        public Test_DuplicatePage_DeletedEmbeddedPage()
            : base("Test_DuplicatePage_DeletedEmbeddedPage")
        {
        }

        public override string Description
        {
            get
            {
                return "Test duplicating a page which has a PageEmbeddedWidget and its embedded page is deleted:\n" +
                    "* Create a document.\n" +
                    "* Create a new page.\n" +
                    "* Create dynamic panel in the new pages.\n" +
                    "* Detele an embedded page in dynamic panel.\n" +
                    "* Duplicate the new page.\n" +
                    "* Save the document.";
            }
        }

        protected override void RunInternal()
        {
            // Create a new document.
            Program.Service.NewDocument(DocumentType.Standard);
            IDocument document = Program.Service.Document;
            
            // Create a new page.
            IDocumentPage page1 = document.CreatePage("Page 1");
            ITreeNode node1 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node1.AttachedObject = page1;

            page1.Open();

            // Get the page view for base adaptive view.
            IPageView baseView1 = page1.PageViews[document.AdaptiveViewSet.Base.Guid];

            IDynamicPanel dynamicPanel = baseView1.CreateWidget(WidgetType.DynamicPanel) as IDynamicPanel;
            dynamicPanel.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            dynamicPanel.WidgetStyle.Height = 198;
            dynamicPanel.WidgetStyle.Width = 152;
            dynamicPanel.IsAutomatic = true;

            // Set start panel page as the first created page.
            dynamicPanel.StartPanelStatePage = dynamicPanel.CreatePanelStatePage("Panel 1");
            IEmbeddedPage embeddedPage2 = dynamicPanel.CreatePanelStatePage("Panel 2");
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

            // Duplicate the page1.
            IDocumentPage page2 = document.DuplicatePage(page1.Guid);

            ITreeNode node2 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node2.AttachedObject = page2;

            dynamicPanel.DeletePanelStatePage(embeddedPage2.Guid);

            page1.Close();

            // Duplicate the page2.
            IDocumentPage page3 = document.DuplicatePage(page1.Guid);

            ITreeNode node3 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node3.AttachedObject = page3;

            // Save document to pn file.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);

            // CLose document.
            Program.Service.Close();
        }
    }
}
