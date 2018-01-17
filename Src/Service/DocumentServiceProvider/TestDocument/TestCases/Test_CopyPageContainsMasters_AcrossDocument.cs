using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_CopyPageContainsMasters_AcrossDocument : TestCase
    {
        public Test_CopyPageContainsMasters_AcrossDocument()
            : base("Test_CopyPageContainsMasters_AcrossDocument")
        {
        }


        public override string Description
        {
            get
            {
                return "Test copy page contains masters across document.\n";
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
            image.Name = "4.png Group_In_MasterPage";
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
            lable.ShapeType = ShapeType.Diamond;
            lable.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            lable.WidgetStyle.Height = 100;
            lable.WidgetStyle.Width = 200;
            lable.WidgetStyle.X = 350;
            lable.WidgetStyle.Y = 300;
            lable.WidgetStyle.Z = 9;
            lable.Name = "Label 1 Group_In_MasterPage";
            lable.Tooltip = "A label.";
            lable.SetRichText("Label Group_In_MasterPage");
            lable.WidgetStyle.FillColor = new StyleColor(ColorFillType.Solid, -888);

            List<Guid> guidList = new List<Guid>();
            guidList.Add(image.Guid);
            guidList.Add(lable.Guid);
            IGroup groupInMaster = masterBaseView.CreateGroup(guidList);
            groupInMaster.Name = "Group_In_MasterPage";

            IDocumentPage page = document.CreatePage("Page 1");
            ITreeNode pageNode = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            pageNode.AttachedObject = page;
            page.Open();
            IPageView baseView = page.PageViews[document.AdaptiveViewSet.Base.Guid];

            IMaster master2 = baseView.CreateMaster(masterPage.Guid);
            master2.MasterStyle.X = 10;
            master2.MasterStyle.Y = 10;
            master2.MasterStyle.Z = 1;

            IShape lable2 = baseView.CreateWidget(WidgetType.Shape) as IShape;
            lable2.ShapeType = ShapeType.RoundedRectangle;
            lable2.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            lable2.WidgetStyle.Height = 100;
            lable2.WidgetStyle.Width = 100;
            lable2.WidgetStyle.X = 500;
            lable2.WidgetStyle.Y = 0;
            lable2.WidgetStyle.Z = 2;
            lable2.Name = "Label Group_In_Page";
            lable2.SetRichText("Label Group_In_Page");

            guidList = new List<Guid>();
            guidList.Add(master2.Guid);
            guidList.Add(lable2.Guid);
            IGroup groupInPage = baseView.CreateGroup(guidList);
            groupInPage.Name = "Group_In_Page";

            IMaster master = baseView.CreateMaster(masterPage.Guid);
            master.MasterStyle.X = 0;
            master.MasterStyle.Y = 500;
            master.MasterStyle.Z = 3;

            ISerializeWriter writer = document.CreateSerializeWriter(Guid.Empty);
            writer.AddPage(page);
            Stream stream = writer.WriteToStream();

            // Create another document
            DocumentService Service = new DocumentService();
            Service.NewDocument(DocumentType.Standard);
            IDocument document2 = Service.Document;

            IObjectContainer container = document2.AddPages(stream);
            stream.Close();

            foreach (IStandardPage newPage in container.StandardPageList)
            {
                ITreeNode node22 = document2.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
                node22.AttachedObject = newPage;
            }

            string target = Path.Combine(Program.WORKING_DIRECTORY, _caseName + "_Target.pn");
            Service.Save(target);
            Service.Close();

            // Save the document to a pn file.
            string source = Path.Combine(Program.WORKING_DIRECTORY, _caseName + "_Source.pn");
            Program.Service.Save(source);
            Program.Service.Close();
        }
    }
}
