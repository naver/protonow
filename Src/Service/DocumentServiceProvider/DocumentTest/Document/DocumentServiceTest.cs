using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Naver.Compass.Service.Document;
using System.IO;

namespace DocumentTest.Document
{
    [TestClass]
    public class DocumentServiceTest
    {
        [TestMethod]
        public void NewDocument_Standard()
        {
            DocumentService docService = new DocumentService();
            docService.NewDocument(DocumentType.Standard);
            Assert.IsNotNull(docService.Document);
        }

        [TestMethod]
        public void NewDocument_Library()
        {
            DocumentService docService = new DocumentService();
            docService.NewDocument(DocumentType.Library);
            Assert.IsNotNull(docService.Document);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Open_InvalidFilePath_ShouldThrowArgumentException()
        {
            string filePath = Path.Combine(TestEnvironment.ResourceFolder, "InvalidFilePath.pn");
            DocumentService docService = new DocumentService();
            docService.Open(filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(ICSharpCode.SharpZipLib.Zip.ZipException))]
        public void Open_BadFileFormat_ShouldThrowException()
        {
            string filePath = Path.Combine(TestEnvironment.ResourceFolder, "BadFileFormat.pn");
            DocumentService docService = new DocumentService();
            docService.Open(filePath);
        }
    }
}
