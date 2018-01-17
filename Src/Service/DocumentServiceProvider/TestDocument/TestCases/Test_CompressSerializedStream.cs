using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;
using System.IO.Compression;

namespace TestDocument
{
    class Test_CompressSerializedStream : TestCase
    {
        public Test_CompressSerializedStream()
            : base("Test_CompressSerializedStream")
        {
        }

        public override string Description
        {
            get
            {
                return "Test compress serialized stream\n";
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

            // DeflateStream Compress
            long deflateCompressStart = DateTime.Now.Ticks;
            MemoryStream deflateCompressedStream = new MemoryStream();
            using (DeflateStream deflateStream = new DeflateStream(deflateCompressedStream, CompressionMode.Compress, true))
            {
                stream.Position = 0;
                stream.CopyTo(deflateStream);
            }
            long deflateCompressEnd = DateTime.Now.Ticks;
            double deflateCompressRate = Math.Round((double)(stream.Length - deflateCompressedStream.Length) / (double)stream.Length * 100, 2);
            Console.WriteLine("DeflateStream compress length {0} to length {1}, compress rate: {2}%,  take ticks: {3}.",
                              stream.Length, deflateCompressedStream.Length, deflateCompressRate, deflateCompressEnd - deflateCompressStart);

            // GZipStream Compress
            long gZipStreamCompressStart = DateTime.Now.Ticks;
            MemoryStream gZipStreamCompressedStream = new MemoryStream();
            using (GZipStream gZipStream = new GZipStream(gZipStreamCompressedStream, CompressionMode.Compress, true))
            {
                stream.Position = 0;
                stream.CopyTo(gZipStream);
            }
            long gZipStreamCompressEnd = DateTime.Now.Ticks;
            double gZipStreamCompressRate = Math.Round((double)(stream.Length - gZipStreamCompressedStream.Length) / (double)stream.Length * 100, 2);
            Console.WriteLine("GZipStream compress length {0} to length {1}, compress rate: {2}%,  take ticks: {3}.",
                              stream.Length, gZipStreamCompressedStream.Length, gZipStreamCompressRate, gZipStreamCompressEnd - gZipStreamCompressStart);

            page1.Close();

            // Paste stream to page 2.
            IDocumentPage page2 = document.CreatePage("Page 2");
            ITreeNode node2 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node2.AttachedObject = page2;
            page2.Open();

            // Get the page view for base adaptive view.
            IPageView baseView2 = page2.PageViews[document.AdaptiveViewSet.Base.Guid];

            IDocumentPage page3 = document.CreatePage("Page 3");
            ITreeNode node3 = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node3.AttachedObject = page3;
            page3.Open();

            // Get the page view for base adaptive view.
            IPageView baseView3 = page3.PageViews[document.AdaptiveViewSet.Base.Guid];

            // DeflateStream Decompress
            long deflateDecompressStart = DateTime.Now.Ticks;
            deflateCompressedStream.Position = 0;
            MemoryStream deflateDecompressedStream = new MemoryStream();
            using (DeflateStream deflateStream = new DeflateStream(deflateCompressedStream, CompressionMode.Decompress, true))
            {
                deflateStream.CopyTo(deflateDecompressedStream);
            }
            long deflateDecompressEnd = DateTime.Now.Ticks;
            Console.WriteLine("DeflateStream decompress take ticks: {0}.", deflateDecompressEnd - deflateDecompressStart);

            deflateDecompressedStream.Position = 0;
            baseView2.AddObjects(deflateDecompressedStream); // Paste to page 2.
            page2.Close();

            // GZipStream Decompress
            long gZipStreamDecompressStart = DateTime.Now.Ticks;
            gZipStreamCompressedStream.Position = 0;
            MemoryStream gZipStreamDecompressedStream = new MemoryStream();
            using (GZipStream gZipStream = new GZipStream(gZipStreamCompressedStream, CompressionMode.Decompress, true))
            {
                gZipStream.CopyTo(gZipStreamDecompressedStream);
            }
            long gZipStreamDecompressEnd = DateTime.Now.Ticks;
            Console.WriteLine("DeflateStream decompress take ticks: {0}.", gZipStreamDecompressEnd - gZipStreamDecompressStart);


            gZipStreamDecompressedStream.Position = 0;
            baseView3.AddObjects(gZipStreamDecompressedStream); // Paste to page 3.
            page3.Close();

            // Save the document to a pn file.
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");
            Program.Service.Save(fileName);

            Program.Service.Close();
        }
    }
}
