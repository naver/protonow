using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_LegacyDocument : TestCase
    {
        public Test_LegacyDocument()
            : base("Test_LegacyDocument")
        {
        }

        public override string Description
        {
            get
            {
                return "Test legacy document:\n" +
                    "* Open a pn file created in old version product.";
            }
        }

        protected override void RunInternal()
        {
            DirectoryInfo historyVersionDir = new DirectoryInfo(Path.Combine(Program.WORKING_DIRECTORY, "HistoryVersion"));
            DirectoryInfo legacyOutputDir = Directory.CreateDirectory(Path.Combine(Program.WORKING_DIRECTORY, "Legacy_Output"));

            foreach (DirectoryInfo versionDir in historyVersionDir.EnumerateDirectories())
            {
                DirectoryInfo LegacyOutputVersionDir = Directory.CreateDirectory(Path.Combine(legacyOutputDir.FullName, versionDir.Name));

                foreach (FileInfo file in versionDir.EnumerateFiles())
                {
                    string target = Path.Combine(LegacyOutputVersionDir.FullName, file.Name);
                    File.Copy(file.FullName, target, true);

                    Program.Service.Open(target);
                    Program.Service.Close();
                }
            }
        }
    }
}
