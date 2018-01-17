using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class ImagesData : XmlDocumentObject, IHashStreamConsumerManager
    {
        class ConsumerData
        {
            internal ConsumerData(string hash, Guid guid, Guid pageGuid)
            {
                Hash = hash;
                Guid = guid;
                PageGuid = pageGuid;
            }

            internal string Hash { get; set; }
            internal Guid Guid { get; set; }
            internal Guid PageGuid { get; set; }
        }

        public ImagesData(Document document)
            : base("Images")
        {
            Debug.Assert(document != null);
            _document = document;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            XmlElement hashesElement = element["Hashes"];
            if (hashesElement == null)
            {
                return;
            }

            foreach (XmlElement hashElement in hashesElement.ChildNodes)
            {
                string hash = "";
                if (LoadElementStringAttribute(hashElement, "Value", ref hash))
                {
                    if (!String.IsNullOrEmpty(hash) && !_hashes.ContainsKey(hash))
                    {
                        _hashes[hash] = new List<ConsumerData>();

                        foreach (XmlElement consumerElement in hashElement.ChildNodes)
                        {
                            Guid guid = Guid.Empty;
                            Guid pageGuid = Guid.Empty; 
                            if(LoadGuidFromChildElementInnerText("Guid", consumerElement, ref guid) &&
                                LoadGuidFromChildElementInnerText("PageGuid", consumerElement, ref pageGuid))
                            {
                                _hashes[hash].Add(new ConsumerData(hash, guid, pageGuid));
                            }
                        }
                    }
                }
            }
        }

        /*
         * <Images>
         *   <Hash Value="xxxxxx">
         *     <Consumer>
         *       <Guid></Guid>
         *       <PageGuid></PageGuid>
         *     </Consumer>
         *     <Consumer>
         *     ...
         *     </Consumer>
         *   </Hash>
         *   <Hash>
         *   ......
         *   </Hash>
         * </Images>
         */
        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement imagesElement = xmlDoc.CreateElement(TagName);
            xmlDoc.AppendChild(imagesElement);

            SaveStringToChildElement("FileVersion", VersionData.THIS_FILE_VERSION, xmlDoc, imagesElement);

            XmlElement hashesElement = xmlDoc.CreateElement("Hashes");
            imagesElement.AppendChild(hashesElement);

            foreach (string hash in _hashes.Keys)
            {
                List<ConsumerData> consumers = _hashes[hash];
                if (consumers != null && consumers.Count > 0)
                {
                    XmlElement hashElement = xmlDoc.CreateElement("Hash");
                    hashesElement.AppendChild(hashElement);
                    SaveElementStringAttribute(hashElement, "Value", hash);

                    foreach (ConsumerData consumer in consumers)
                    {
                        XmlElement consumerElement = xmlDoc.CreateElement("Consumer");
                        hashElement.AppendChild(consumerElement);

                        SaveStringToChildElement("Guid", consumer.Guid.ToString(), xmlDoc, consumerElement);
                        SaveStringToChildElement("PageGuid", consumer.PageGuid.ToString(), xmlDoc, consumerElement);
                    }
                }
            }
        }

        #endregion

        public void AddConsumer(IHashStreamConsumer consumer)
        {
            if (consumer == null)
            {
                return;
            }

            AddConsumer(consumer.Hash, consumer.Guid, consumer.ParentPageGuid);
        }

        public void AddConsumer(string hash, Guid consumerGuid, Guid parentPageGuid)
        {
            List<ConsumerData> consumers = null;
            if (_hashes.ContainsKey(hash))
            {
                consumers = _hashes[hash];
            }

            if (consumers == null)
            {
                consumers = _hashes[hash] = new List<ConsumerData>();
            }

            if (!consumers.Any<ConsumerData>(x => x.Guid == consumerGuid))
            {
                consumers.Add(new ConsumerData(hash, consumerGuid, parentPageGuid));
            }
        }

        public void DeleteConsumer(IHashStreamConsumer consumer)
        {
            if (_hashes.ContainsKey(consumer.Hash))
            {
                List<ConsumerData> consumers = _hashes[consumer.Hash];
                if (consumers != null)
                {
                    consumers.RemoveAll(x => x.Guid == consumer.Guid);
                }
            }
        }

        public void DeleteConsumer(Guid consumerGuid)
        {
            foreach (List<ConsumerData> consumers in _hashes.Values)
            {
                consumers.RemoveAll(x => x.Guid == consumerGuid);
            }
        }

        public void DeleteConsumersInPage(Guid pageGuid)
        {
            foreach (List<ConsumerData> consumers in _hashes.Values)
            {
                consumers.RemoveAll(x => x.PageGuid == pageGuid);
            }
        }

        public bool ContainsConsumer(Guid consumerGuid)
        {
            foreach (List<ConsumerData> consumers in _hashes.Values)
            {
                if (consumers.Any<ConsumerData>(x => x.Guid == consumerGuid))
                {
                    return true;
                }
            }

            return false;
        }

        public bool AnyConsumer(string hash)
        {
            if (_hashes.ContainsKey(hash))
            {
                List<ConsumerData> consumers = _hashes[hash];
                if (consumers != null)
                {
                    return consumers.Count > 0;
                }
            }

            return false;
        }

        public bool AnyActiveConsumer(string hash)
        {
            if (_hashes.ContainsKey(hash))
            {
                List<ConsumerData> consumers = _hashes[hash];
                if (consumers != null && consumers.Count > 0)
                {
                    foreach (ConsumerData consumer in consumers)
                    {
                        Page consumerPage = _document.AllPages[consumer.PageGuid];
                        if (consumerPage != null && consumerPage.IsOpened)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            _hashes.Clear();
        }

        private Document _document;
        private Dictionary<string, List<ConsumerData>> _hashes = new Dictionary<string, List<ConsumerData>>();
    }
}
