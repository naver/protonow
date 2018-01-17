using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using Naver.Compass.Service.Document;

namespace TestDocument
{
    class Program
    {
        static Program()
        {
            TestCases[1] = new Test_CreateAllTypeWidgets();
            TestCases[2] = new Test_DuplicatePage();
            TestCases[3] = new Test_DuplicatePage_Stream();
            TestCases[4] = new Test_DuplicatePage_DeletedEmbeddedPage();
            TestCases[5] = new Test_CopyWidgets();
            TestCases[6] = new Test_CreateLibraryDocument();
            TestCases[7] = new Test_SaveAs_LibraryDocument();
            TestCases[8] = new Test_CreateLibraryInPage();
            TestCases[9] = new Test_LoadLibrary();
            TestCases[10] = new Test_LegacyDocument();
            TestCases[11] = new Test_DynamicPanel();
            TestCases[12] = new Test_ZipPages();
            TestCases[13] = new Test_ZipPages_EmbeddedPage();
            TestCases[14] = new Test_SaveAs();
            TestCases[15] = new Test_AddCustomObjectToLibrary();
            TestCases[16] = new Test_LRU_LibraryAlgorithm();
            TestCases[17] = new Test_LibraryManager_LibraryGuid();
            TestCases[18] = new Test_CompressSerializedStream();
            TestCases[19] = new Test_AddCustomObjectToPage();
            TestCases[20] = new Test_SetWidgetDefaultStyle();

            TestCases[21] = new Test_CreateMasters();
            TestCases[22] = new Test_CreateMasterInLibrary();
            TestCases[23] = new Test_CopyMasters();
            TestCases[24] = new Test_CopyMasters_AcrossDocument();
            TestCases[25] = new Test_CopyObjects_AcrossDocument_MasterIsActionTarget();
            TestCases[26] = new Test_CopyPageContainsMasters();
            TestCases[27] = new Test_CopyPageContainsMasters_AcrossDocument();
            TestCases[28] = new Test_DeleteMasterPage();
            TestCases[29] = new Test_CopyPageContainsMastersZOrder_AcrossDocument();
            TestCases[30] = new Test_BreakMaster();
        }

        static void Main(string[] args)
        {
            string Usage = "Usage: Input following text to run test.\n" +
                           "<test case number> : test case number [1 ~ " + TestCases.Count + "].\n" +
                           "? : Test case descriptions.\n" +
                           "a | all : all test case.\n" +
                           "e | exit : exit.\n";

            Console.WriteLine(Usage);

            string input = Console.ReadLine();
            while (String.Compare(input, @"e", true) != 0 && String.Compare(input, @"exit", true) != 0)
            {

                if (String.Compare(input, @"?", true) == 0)
                {
                    Console.WriteLine("Test Case Number : ");

                    input = Console.ReadLine();

                    int index = -1;
                    if (Int32.TryParse(input, out index) && TestCases.ContainsKey(index))
                    {
                        Console.WriteLine(TestCases[index].Description);
                    }
                    else
                    {
                        Console.WriteLine("No such test case.");
                    }
                }
                else
                {
                    bool runAll = String.Compare(input, @"a", true) == 0 || String.Compare(input, @"all", true) == 0;

                    if (runAll)
                    {
                        foreach (TestCase tCase in TestCases.Values)
                        {
                            tCase.Run();
                        }
                    }
                    else
                    {
                        int index = -1;
                        if (Int32.TryParse(input, out index) && TestCases.ContainsKey(index))
                        {
                            TestCases[index].Run();
                        }
                        else
                        {
                            Console.WriteLine(Usage);
                        }
                    }
                }

                input = Console.ReadLine();
            }

            Service.Dispose();
        }

        public static Dictionary<int, TestCase> TestCases = new Dictionary<int, TestCase>();        
        public static DocumentService Service = new DocumentService();

        public static readonly string WORKING_DIRECTORY = @"D:\Projects\protoNow\DOMv8";
        public static readonly string WORKING_IMAGES_DIRECTORY = WORKING_DIRECTORY + @"\images";
    }
}
