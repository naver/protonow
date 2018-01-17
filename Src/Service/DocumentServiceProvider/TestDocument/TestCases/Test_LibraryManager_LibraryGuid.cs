using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.IO;

namespace TestDocument
{
    class Test_LibraryManager_LibraryGuid : TestCase
    {
        public Test_LibraryManager_LibraryGuid()
            : base("Test_LibraryManager_LibraryGuid")
        {
        }

        public override string Description
        {
            get
            {
                return "Test get guid from file and copy and creata new library with new guid.\n";
            }
        }

        protected override void RunInternal()
        {
            ILibraryManager manager = Program.Service.LibraryManager;
            Guid guid = manager.PeekLibraryGuidFromFile(Path.Combine(Program.WORKING_DIRECTORY, "Test_CreateLibraryDocument.libpn"));

            Console.WriteLine("Guid is :" + guid.ToString());

            manager.CreateNewLibraryFile(Path.Combine(Program.WORKING_DIRECTORY, "Test_CreateLibraryInPage.libpn"),
                Path.Combine(Program.WORKING_DIRECTORY, "Test_CreateLibraryInPage_NewGuid.libpn"));
        }
    }
}
