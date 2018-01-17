using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_SaveAs : TestCase
    {
        public Test_SaveAs()
            : base("Test_SaveAs")
        {
        }


        public override string Description
        {
            get
            {
                return "Test saving document to new location:\n" +
                    "* Open a document.\n" +
                    "* Save the document to a new location";

            }
        }

        protected override void RunInternal()
        {
            string fileName = Path.Combine(Program.WORKING_DIRECTORY, "Test_ZipPages.pn");
            string newFileName = Path.Combine(Program.WORKING_DIRECTORY, _caseName + ".pn");

            Program.Service.Open(fileName);
            Program.Service.Save(newFileName);
            Program.Service.Close();
        }
    }
}
