using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Naver.Compass.Service.Html;
using Naver.Compass.Service.Document;


namespace TestHtmlService
{
    class Program
    {
        static void Main(string[] args)
        {
            // Open a document.
            DocumentService service = new DocumentService();
            service.Open(@"D:\Projects\protoNow\Viewer\Test_CreateMasters.pn");
            IDocument doc = service.Document;

            // Set the output folder.
            doc.GeneratorConfigurationSet.DefaultHtmlConfiguration.OutputFolder = @"D:\Projects\protoNow\Viewer\Output";

            // Render the document to html files.
            IHtmlServiceProvider html = new HtmlServiceProvider();
            html.Render(doc);

            // Open the start html file with IE.
            Process.Start("file:///" + html.StartHtmlFile.Replace(@"\", "/"));
        }
    }
}
