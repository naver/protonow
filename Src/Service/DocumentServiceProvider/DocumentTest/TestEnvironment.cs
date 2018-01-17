using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace DocumentTest
{
    internal static class TestEnvironment
    {
        static TestEnvironment()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            _assemblyFolder = Path.GetDirectoryName(path);

            _outputFolder = Path.Combine(_assemblyFolder, "Output");

            if(Directory.Exists(_outputFolder))
            {
                Directory.Delete(_outputFolder);
            }

            Directory.CreateDirectory(_outputFolder);

            _resourceFolder = Path.Combine(_assemblyFolder, @"Res");

            if (!Directory.Exists(_resourceFolder))
            {
                Directory.CreateDirectory(_resourceFolder);
            }
        }

        internal static string AssemblyFolder
        {
            get { return _assemblyFolder; }
        }

        internal static string OutputFolder
        {
            get { return _outputFolder; }
        }

        internal static string ResourceFolder
        {
            get { return _resourceFolder; }
        }

        private static string _assemblyFolder;
        private static string _outputFolder;
        private static string _resourceFolder;
    }
}
