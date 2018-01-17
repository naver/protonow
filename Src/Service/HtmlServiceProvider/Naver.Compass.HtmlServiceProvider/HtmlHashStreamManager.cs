using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Naver.Compass.Service.Html
{
    internal class HtmlHashStreamManager : IHtmlHashStreamManager
    {
        public string WorkingDirectory
        {
            get
            {
                if(_workingDirectory != null)
                {
                    return _workingDirectory.FullName;
                }

                return String.Empty;
            }

            set
            {
                if(!String.IsNullOrEmpty(value))
                {
                    _workingDirectory = new DirectoryInfo(value);
                    if(!_workingDirectory.Exists)
                    {
                        _workingDirectory.Create();
                    }
                }

                // Reset the consumer list if working directory is reset;
                _consumers.Clear();
            }
        }

        public string GetConsumerStreamHash(Guid consumerGuid, Guid viewGuid)
        {
            ConsumerData consumer = _consumers.FirstOrDefault<ConsumerData>(x => x.ConsumerGuid == consumerGuid 
                                                                              && x.ViewGuid == viewGuid);
            if (consumer != null)
            {
                return consumer.Hash;
            }
            else
            {
                return String.Empty;
            }
        }

        public Stream GetConsumerStream(Guid consumerGuid, Guid viewGuid)
        {
            if (_workingDirectory == null)
            {
                throw new DirectoryNotFoundException("Working directory is not found.");
            }

            ConsumerData consumer = _consumers.FirstOrDefault<ConsumerData>(x => x.ConsumerGuid == consumerGuid
                                                                             && x.ViewGuid == viewGuid);
            if (consumer != null && !String.IsNullOrEmpty(consumer.Hash))
            {
                FileInfo[] files = _workingDirectory.GetFiles(consumer.Hash + "*");
                if (files.Length > 0)
                {
                    // Just read the first match file, actually there should be only on match file as hash value is unique.
                    Stream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(files[0].FullName, FileMode.Open, FileAccess.Read))
                    {
                        fileStream.CopyTo(stream);
                        stream.Position = 0;
                    }
                    
                    return stream;
                }
            }
         
            return null;
        }

        public string SetConsumerStream(Guid consumerGuid, Guid viewGuid, Stream stream, string streamType)
        {
            if (_workingDirectory == null)
            {
                throw new DirectoryNotFoundException("Working directory is not found.");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            long oldPostion = stream.Position;
            stream.Position = 0;

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(stream);
            string imageMD5 = BitConverter.ToString(hash);
            string fileHash = imageMD5.Replace("-", "").ToLower();

            ConsumerData consumer = _consumers.FirstOrDefault<ConsumerData>(x => x.ConsumerGuid == consumerGuid
                                                      && x.ViewGuid == viewGuid);
            
            if (consumer != null)
            {
                consumer.Hash = fileHash;
            }
            else
            {
                consumer = new ConsumerData(consumerGuid, viewGuid, fileHash);
                _consumers.Add(consumer);
            }

            FileInfo[] files = _workingDirectory.GetFiles(fileHash + "*");
            if (files.Length > 0)
            {
                // Image file already exist
                stream.Position = oldPostion;
                return fileHash;
            }

            // Save the stream to file.
            string fileName = fileHash;
            if (!String.IsNullOrEmpty(streamType))
            {
                fileName += @".";
                fileName += streamType;
            }

            string fileFullName = Path.Combine(_workingDirectory.FullName, fileName);
            using (FileStream fileStream = new FileStream(fileFullName, FileMode.Create, FileAccess.Write))
            {
                stream.Position = 0;
                stream.CopyTo(fileStream);
                stream.Position = oldPostion;
            }

            return fileHash;
        }

        class ConsumerData
        {
            internal ConsumerData(Guid consumerGuid, Guid viewGuid, string hash)
            {
                ConsumerGuid = consumerGuid;
                ViewGuid = viewGuid;
                Hash = hash;
            }

            internal Guid ConsumerGuid { get; set; }
            internal Guid ViewGuid { get; set; }
            internal string Hash { get; set; }
        }

        private DirectoryInfo _workingDirectory;
        private List<ConsumerData> _consumers = new List<ConsumerData>();
    }
}
