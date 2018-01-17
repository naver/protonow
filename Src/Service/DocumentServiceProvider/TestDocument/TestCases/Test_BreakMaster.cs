using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_BreakMaster : TestCase
    {
        public Test_BreakMaster()
            : base("Test_BreakMaster")
        {
        }


        public override string Description
        {
            get
            {
                return "Test break master.\n"; 
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
            image.WidgetStyle.Y = 0;
            image.WidgetStyle.Z = 1;
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
            lable.WidgetStyle.Width = 300;
            lable.WidgetStyle.X = 150;
            lable.WidgetStyle.Y = 300;
            lable.WidgetStyle.Z = 9;
            lable.Name = "Label 1";
            lable.Tooltip = "A label.";
            lable.SetRichText("Label");
            lable.WidgetStyle.LineColor = new StyleColor(ColorFillType.Solid, -16777216);
            lable.WidgetStyle.LineWidth = 0; // No border
            lable.WidgetStyle.FillColor = new StyleColor(ColorFillType.Solid, 4848); 
            lable.WidgetStyle.HorzAlign = Alignment.Left;
            lable.WidgetStyle.VertAlign = Alignment.Top;

            IDocumentPage page = document.CreatePage("Page 1");
            ITreeNode pageNode = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            pageNode.AttachedObject = page;
            page.Open();
            IPageView baseView = page.PageViews[document.AdaptiveViewSet.Base.Guid];

            IMaster master = baseView.CreateMaster(masterPage.Guid);
            master.MasterStyle.X = 99;
            master.MasterStyle.Y = 99;

            // Break this master
            IObjectContainer container = baseView.BreakMaster(master.Guid);
           
            // Save the document to a pn file.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);

            // Close this document when you don't work on it anymore.
            Program.Service.Close();
        }
    }
}
