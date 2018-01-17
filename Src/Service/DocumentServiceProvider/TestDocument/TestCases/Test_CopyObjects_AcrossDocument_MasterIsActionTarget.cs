using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_CopyObjects_AcrossDocument_MasterIsActionTarget: TestCase
    {
        public Test_CopyObjects_AcrossDocument_MasterIsActionTarget()
            : base("Test_CopyObjects_AcrossDocument_MasterIsActionTarget")
        {
        }


        public override string Description
        {
            get
            {
                return "Test copy masters across documents, the master is a show hide action target." +
                       "Check breaking master away, target master is replaced with widgets and groups \n"; 
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
            lable.WidgetStyle.X = 350;
            lable.WidgetStyle.Y = 300;
            lable.WidgetStyle.Z = 9;
            lable.Name = "Label 1";
            lable.Tooltip = "A label.";
            lable.SetRichText("Label");

            List<Guid> guidList = new List<Guid>();
            guidList.Add(image.Guid);
            guidList.Add(lable.Guid);
            masterBaseView.CreateGroup(guidList);

            IDocumentPage page = document.CreatePage("Page 1");
            ITreeNode pageNode = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            pageNode.AttachedObject = page;
            page.Open();
            IPageView baseView = page.PageViews[document.AdaptiveViewSet.Base.Guid];

            IMaster master = baseView.CreateMaster(masterPage.Guid);

            IShape triangle = baseView.CreateWidget(WidgetType.Shape) as IShape;
            triangle.ShapeType = ShapeType.Triangle;
            triangle.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            triangle.WidgetStyle.Height = 100;
            triangle.WidgetStyle.Width = 100;
            triangle.WidgetStyle.X = 650;
            triangle.WidgetStyle.Y = 300;
            triangle.WidgetStyle.Z = 11;
            triangle.Name = "Triangle_Show_Hide";
            triangle.Tooltip = "A Triangle to triger Show or Hide.";
            triangle.SetRichText("Triangle_Show_Hide");

            IInteractionEvent iEvent = triangle.Events[EventType.OnClick];
            IInteractionCase iCase = iEvent.CreateCase("clickCase");
            IInteractionShowHideAction action = iCase.CreateAction(ActionType.ShowHideAction) as IInteractionShowHideAction;
            action.AddTargetObject(master.Guid);
            action.SetAllVisibilityType(VisibilityType.Toggle);
            action.SetAllAnimateType(ShowHideAnimateType.SlideRight);
            action.SetAllAnimateTime(500);

            ISerializeWriter writer = document.CreateSerializeWriter(document.AdaptiveViewSet.Base.Guid);
            writer.AddWidget(triangle);
            writer.AddMaster(master);
            Stream stream = writer.WriteToStream();

            // Create another document
            DocumentService Service = new DocumentService();
            Service.NewDocument(DocumentType.Standard);
            IDocument document2 = Service.Document;

            IDocumentPage page2 = document2.CreatePage("Page 1");
            ITreeNode pageNode2 = document2.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            pageNode2.AttachedObject = page2;
            page2.Open();
            IPageView baseView2 = page2.PageViews[document2.AdaptiveViewSet.Base.Guid];
            baseView2.AddObjects(stream);

            string target = Path.Combine(Program.WORKING_DIRECTORY, _caseName + "_Target.pn");
            Service.Save(target);
            Service.Close();

            string source = Path.Combine(Program.WORKING_DIRECTORY, _caseName + "_Source.pn");
            Program.Service.Save(source);
            Program.Service.Close();
        }
    }
}
