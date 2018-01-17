using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class DebugMemoryStream : MemoryStream
    {
        public bool IsDead { get; set; }

        protected override void Dispose(bool disposing)
        {
            IsDead = true;
            base.Dispose(disposing);
        }
    }

    internal class StreamFileObject : IDisposable
    {
        internal StreamFileObject(string streamFileName)
        {
            _streamFileName = streamFileName;
        }

        internal void LoadStreamFromFile()
        {
            if (!File.Exists(_streamFileName))
            {
                _stream = null;
            }
            else
            {
                _stream = new MemoryStream();
                using (FileStream fileStream = new FileStream(_streamFileName, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(_stream);
                    _stream.Position = 0;
                }
            }
        }

        internal void SaveStreamToFile()
        {
            if (_stream != null)
            {
                _stream.Position = 0;

                using (FileStream fileStream = new FileStream(_streamFileName, FileMode.Create, FileAccess.Write))
                {
                    _stream.CopyTo(fileStream);
                    _stream.Position = 0;
                }
            }
        }

        internal virtual void DeleteStreamFile()
        {
            try
            {
                if (File.Exists(_streamFileName))
                {
                    File.Delete(_streamFileName);
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
            }
        }

        public void Dispose()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }
        }

        internal Stream DataStream
        {
            get
            {
                if (_stream != null)
                {
                    _stream.Position = 0;
                }
                else
                {
                    // Try to load from data file if it exists. The data file will be deleted if set DataStream to null.
                    LoadStreamFromFile();
                }

                return _stream;
            }

            set
            {
                // Close the previous stream and delete related data file.
                Dispose();

                if (value == null)
                {
                   // If the stream is set to null and the corresponding source file exists, delete it.
                    DeleteStreamFile();
                    return;
                }

                long oldPostion = value.Position;

                value.Position = 0;
                _stream = new MemoryStream();
                value.CopyTo(_stream);
                _stream.Position = 0;
                value.Position = oldPostion;
            }
        }

        public string StreamFileName
        {
            get { return _streamFileName; }
            set { _streamFileName = value; }
        }

        private string _streamFileName;
        private MemoryStream _stream;
    }
}
