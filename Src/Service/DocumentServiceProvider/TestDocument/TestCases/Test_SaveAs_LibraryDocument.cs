using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_SaveAs_LibraryDocument : TestCase
    {
        public Test_SaveAs_LibraryDocument()
            : base("Test_SaveAs_LibraryDocument")
        {
        }


        public override string Description
        {
            get
            {
                return "Test saving library document to new location:\n" +
                    "* Open a library document.\n" +
                    "* Save the library document to a new location";

            }
        }

        protected override void RunInternal()
        {
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, "Test_CreateLibraryDocument.libpn");
            string newFileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".libpn");

            Program.Service.Open(fileName);
            Program.Service.Save(newFileName);
            Program.Service.Close();
        }
    }
}
