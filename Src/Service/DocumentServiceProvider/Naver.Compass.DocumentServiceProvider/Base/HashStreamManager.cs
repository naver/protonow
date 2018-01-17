using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;

namespace Naver.Compass.Service.Document
{
    internal class HashStreamManager : IHashStreamManager
    {
        internal HashStreamManager(Document document)
        {
            _document = document;
            _hashStreams = new Dictionary<string, Stream>();
        }

        public void Dispose()
        {
            ClearCachedStream();
        }

        public IDocument ParentDocument
        {
            get { return _document; }
        }

        public Stream GetStream(string hash)
        {
            if (String.IsNullOrEmpty(hash) || _document.WorkingImagesDirectory == null)
            {
                return null;
            }

            if (_hashStreams.ContainsKey(hash))
            {
                Stream stream = _hashStreams[hash];
                if (stream != null)
                {
                    stream.Position = 0;
                    return stream;
                }
            }

            // Try to load from file
            FileInfo[] files = _document.WorkingImagesDirectory.GetFiles(hash + "*");
            if (files.Length > 0)
            {
                // Just read the first match file, actually there should be only on match file as hash value is unique.
                Stream stream = new MemoryStream();
                using (FileStream fileStream = new FileStream(files[0].FullName, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(stream);
                    stream.Position = 0;
                }

                _hashStreams[hash] = stream;
                _cachedStreamSize += stream.Length;
                return stream;
            }
            else
            {
                // Remove the hash key if the correspoding file doesn't exist.
                _hashStreams.Remove(hash);
                return null;
            }
        }

        public string SetStream(Stream stream, string streamType)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (_document.WorkingImagesDirectory == null)
            {
                throw new ArgumentException("Images working directory is null!");
            }

            long oldPostion = stream.Position;
            stream.Position = 0;

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(stream);
            string imageMD5 = BitConverter.ToString(hash);
            string fileHash = imageMD5.Replace("-", "").ToLower();

            if (_hashStreams.ContainsKey(fileHash))
            {
                stream.Position = oldPostion;
                return fileHash;
            }

            stream.Position = 0;
            Stream newStream = new MemoryStream();
            stream.CopyTo(newStream);
            stream.Position = oldPostion;
            newStream.Position = 0;

            FileInfo[] files = _document.WorkingImagesDirectory.GetFiles(fileHash + "*");
            if (files.Length <= 0)
            {
                // Save the stream to file if the file doesn't exist
                string fileName = fileHash;
                if(!String.IsNullOrEmpty(streamType))
                {
                    fileName += @".";
                    fileName += streamType;
                }

                string fileFullName = Path.Combine(_document.WorkingImagesDirectory.FullName, fileName);

                if(streamType == @"svg")
                {
                    using (FileStream fileStream = new FileStream(fileFullName, FileMode.Create, FileAccess.Write))
                    {
                        newStream.CopyTo(fileStream);
                        newStream.Position = 0;
                    }
                }
                else
                {
                    VaryQualityAndSize(newStream, fileFullName);
                }
            }

            TryToGCStreams();

            _hashStreams[fileHash] = newStream;
            _cachedStreamSize += newStream.Length;

            return fileHash;
        }

        public void ClearCachedStream()
        {
            foreach (Stream stream in _hashStreams.Values)
            {
                stream.Close();
            }

            _hashStreams.Clear();
            _cachedStreamSize = 0;
        }

        private void TryToGCStreams()
        {
            if (_cachedStreamSize > MAX_CACHE_SIZE)
            {
                _cachedStreamSize = 0;

                // Look up unused streams or streams only used in closed page.
                List<string> gcList = new List<string>();
                foreach(string hash in _hashStreams.Keys)
                {
                    if (_document.ImagesData.AnyActiveConsumer(hash))
                    {
                        _cachedStreamSize += _hashStreams[hash].Length;
                        continue;
                    }
                    else
                    {
                        gcList.Add(hash);
                    }
                }

                foreach(string hash in gcList)
                {
                    Stream stream = _hashStreams[hash];
                    _hashStreams.Remove(hash);
                    if(stream != null)
                    {
                        stream.Close();
                    }
                }

                GC.Collect();
            }
        }


        /// <summary>
        /// JPG: lower down image quality
        /// BMP: convert to png
        /// others: do not change.
        /// </summary>
        /// <param name="imageStream"></param>
        /// <param name="fileFullName"></param>
        private void VaryQualityAndSize(Stream imageStream, string fileFullName)
        {
            // Get a bitmap.
            Bitmap bmp = new Bitmap(imageStream);

            ImageFormat format = bmp.RawFormat;
            if (ImageFormat.Jpeg.Equals(format))
            {
                ImageCodecInfo jpgEncoder = GetEncoder(format);

                // Create an Encoder object based on the GUID
                // for the Quality parameter category.
                System.Drawing.Imaging.Encoder myEncoder =
                    System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bmp.Save(fileFullName, jpgEncoder, myEncoderParameters);
                imageStream.Position = 0;
            }
            else if (ImageFormat.Bmp.Equals(format))
            {
                bmp.Save(fileFullName, ImageFormat.Png);
            }
            else
            {
                bmp.Save(fileFullName);
            }           
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private Document _document;
        private Dictionary<string, Stream> _hashStreams;
        private long _cachedStreamSize = 0;

        private const long MAX_CACHE_SIZE = 1024 * 1024 * 1024; // 1G
    }
}
