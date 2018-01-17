using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_SetWidgetDefaultStyle : TestCase
    {
        public Test_SetWidgetDefaultStyle()
            : base("Test_SetWidgetDefaultStyle")
        {
        }


        public override string Description
        {
            get
            {
                return "Test setting widget style as default styles.\n"; 
            }
        }

        protected override void RunInternal()
        {
            Program.Service.NewDocument(DocumentType.Standard);
            IDocument document = Program.Service.Document;

            IDocumentPage page = document.CreatePage("Page 1");

            // Create the page node in page tree.
            ITreeNode node = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node.AttachedObject = page;

            // Must open the page before you read and modify it.
            page.Open(); 

            IPageView baseView = page.PageViews[document.AdaptiveViewSet.Base.Guid];

            IShape roundedRectangle = baseView.CreateWidget(WidgetType.Shape) as IShape;

            roundedRectangle.ShapeType = ShapeType.RoundedRectangle;
            roundedRectangle.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            roundedRectangle.WidgetStyle.Height = 100;
            roundedRectangle.WidgetStyle.Width = 200;
            roundedRectangle.WidgetStyle.X = 10;
            roundedRectangle.WidgetStyle.Y = 10;
            roundedRectangle.WidgetStyle.Z = 0;
            roundedRectangle.Name = "RoundedRectangle 1";
            roundedRectangle.Tooltip = "A Rounded Rectangle.";
            roundedRectangle.SetRichText("RoundedRectangle");

            
            roundedRectangle.WidgetStyle.Bold = true;
            roundedRectangle.WidgetStyle.FillColor = new StyleColor(ColorFillType.Solid, -999);
            roundedRectangle.WidgetStyle.FontColor = new StyleColor(ColorFillType.Solid, -35689);
            roundedRectangle.WidgetStyle.FontSize = 22;
            roundedRectangle.WidgetStyle.FontFamily = "Gull Sans";
            roundedRectangle.WidgetStyle.Italic = true;
            roundedRectangle.WidgetStyle.Underline = true;
            roundedRectangle.WidgetStyle.LineColor = new StyleColor(ColorFillType.Solid, -45151);
            roundedRectangle.WidgetStyle.LineStyle = LineStyle.DashDotDot;
            roundedRectangle.WidgetStyle.LineWidth = 5;

            roundedRectangle.SetWidgetStyleAsDefaultStyle(document.AdaptiveViewSet.Base.Guid);

            IShape roundedRectangle2 = baseView.CreateWidget(WidgetType.Shape) as IShape;
            roundedRectangle2.ShapeType = ShapeType.RoundedRectangle;
            roundedRectangle2.Annotation.SetTextValue("CreatedTime", DateTime.Now.ToString());
            roundedRectangle2.WidgetStyle.Height = 100;
            roundedRectangle2.WidgetStyle.Width = 200;
            roundedRectangle2.WidgetStyle.X = 10;
            roundedRectangle2.WidgetStyle.Y = 250;
            roundedRectangle2.WidgetStyle.Z = 1;
            roundedRectangle2.Name = "RoundedRectangle 2";
            roundedRectangle2.Tooltip = "A new Rounded Rectangle.";
            roundedRectangle2.SetRichText("RoundedRectangle 2");

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
