using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naver.Compass.Service.Html
{
    // Base class for objects which can be saved to a file.
    internal abstract class JsFileObject
    {
        public virtual void SaveToJsFile()
        {
            string filePath = SaveDirectory;
            if(!String.IsNullOrEmpty(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            filePath += @"\";
            filePath += FileName;

            if(!String.IsNullOrEmpty(filePath))
            {
                using (StreamWriter outfile = new StreamWriter(filePath))
                {
                    outfile.Write(this.ToString());
                }
            }
        }

        protected abstract string SaveDirectory
        {
            get;
        }

        protected abstract string FileName
        {
            get;
        }
    }
}
