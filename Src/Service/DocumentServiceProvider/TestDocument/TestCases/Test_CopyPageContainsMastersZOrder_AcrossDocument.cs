using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_CopyPageContainsMastersZOrder_AcrossDocument : TestCase
    {
        public Test_CopyPageContainsMastersZOrder_AcrossDocument()
            : base("Test_CopyPageContainsMastersZOrder_AcrossDocument")
        {
        }


        public override string Description
        {
            get
            {
                return "Test copy page contains masters across document and check their Z-order.\n";
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

            IShape diamond = masterBaseView.CreateWidget(WidgetType.Shape) as IShape;
            diamond.ShapeType = ShapeType.Diamond;
            diamond.WidgetStyle.Height = 200;
            diamond.WidgetStyle.Width = 200;
            diamond.WidgetStyle.X = 0;
            diamond.WidgetStyle.Y = 0;
            diamond.WidgetStyle.Z = 0;
            diamond.Name = "Z_0_In_MasterPage";
            diamond.SetRichText("Z_0_In_MasterPage");

            IShape ellipse = masterBaseView.CreateWidget(WidgetType.Shape) as IShape;
            ellipse.ShapeType = ShapeType.Ellipse;
            ellipse.WidgetStyle.Height = 200;
            ellipse.WidgetStyle.Width = 200;
            ellipse.WidgetStyle.X = 50;
            ellipse.WidgetStyle.Y = 50;
            ellipse.WidgetStyle.Z = 1;
            ellipse.Name = "Z_1_In_MasterPage";
            ellipse.SetRichText("Z_1_In_MasterPage");

            IShape rectangle = masterBaseView.CreateWidget(WidgetType.Shape) as IShape;
            rectangle.ShapeType = ShapeType.Rectangle;
            rectangle.WidgetStyle.Height = 200;
            rectangle.WidgetStyle.Width = 200;
            rectangle.WidgetStyle.X = 100;
            rectangle.WidgetStyle.Y = 100;
            rectangle.WidgetStyle.Z = 2;
            rectangle.Name = "Z_2_In_MasterPage";
            rectangle.SetRichText("Z_2_In_MasterPage");

            List<Guid> guidList = new List<Guid>();
            guidList.Add(ellipse.Guid);
            guidList.Add(rectangle.Guid);
            IGroup groupInMaster = masterBaseView.CreateGroup(guidList);
            groupInMaster.Name = "Group_In_MasterPage";

            IDocumentPage page = document.CreatePage("Page 1");
            ITreeNode pageNode = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            pageNode.AttachedObject = page;
            page.Open();
            IPageView baseView = page.PageViews[document.AdaptiveViewSet.Base.Guid];

            IShape round = baseView.CreateWidget(WidgetType.Shape) as IShape;
            round.ShapeType = ShapeType.RoundedRectangle;
            round.WidgetStyle.Height = 200;
            round.WidgetStyle.Width = 200;
            round.WidgetStyle.X = 0;
            round.WidgetStyle.Y = 0;
            round.WidgetStyle.Z = 0;
            round.Name = "Z_0_In_Page";
            round.SetRichText("Z_0_In_Page");

            IMaster master1 = baseView.CreateMaster(masterPage.Guid);
            master1.MasterStyle.X = 250;
            master1.MasterStyle.Y = 250;
            master1.MasterStyle.Z = 1;

            IShape triangle = baseView.CreateWidget(WidgetType.Shape) as IShape;
            triangle.ShapeType = ShapeType.Triangle;
            triangle.WidgetStyle.Height = 200;
            triangle.WidgetStyle.Width = 200;
            triangle.WidgetStyle.X = 100;
            triangle.WidgetStyle.Y = 100;
            triangle.WidgetStyle.Z = 2;
            triangle.Name = "Z_2_In_Page";
            triangle.SetRichText("Z_2_In_Page");
            triangle.WidgetStyle.FillColor = new StyleColor(ColorFillType.Solid, -22550);

            IMaster master2 = baseView.CreateMaster(masterPage.Guid);
            master2.MasterStyle.X = 500;
            master2.MasterStyle.Y = 500;
            master2.MasterStyle.Z = 3;

            IShape triangle1 = baseView.CreateWidget(WidgetType.Shape) as IShape;
            triangle1.ShapeType = ShapeType.Triangle;
            triangle1.WidgetStyle.Height = 200;
            triangle1.WidgetStyle.Width = 200;
            triangle1.WidgetStyle.X = 100;
            triangle1.WidgetStyle.Y = 100;
            triangle1.WidgetStyle.Z = 4;
            triangle1.Name = "Z_4_In_Page";
            triangle1.SetRichText("Z_4_In_Page");
            triangle1.WidgetStyle.FillColor = new StyleColor(ColorFillType.Solid, -22550);

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
