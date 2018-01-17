using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{

    internal abstract class StreamWidget : Widget, IStreamWidget
    {
        public StreamWidget(Page parentPage, string tagName)
            : base(parentPage, tagName)
        {

        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {            
            base.LoadDataFromXml(element);

            LoadStringFromChildElementInnerText("Hash", element, ref _hash);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            SaveStringToChildElement("Hash", _hash, xmlDoc, parentElement);

            base.SaveDataToXml(xmlDoc, parentElement);
        }

        #endregion

        public string Hash
        {
            get
            {
                if(string.IsNullOrEmpty(_hash))
                {
                    return string.Empty;
                }
                return _hash;
            }
            set { }
        }

        public abstract string StreamType { get; set; }

        public Guid ParentPageGuid
        {
            get
            {
                return ParentPage != null ? ParentPage.Guid : Guid.Empty;
            }
        }

        public Stream DataStream
        {
            get 
            {
                if (HashStreamManager != null)
                {
                    return HashStreamManager.GetStream(_hash);
                }
                else if (_tempStream != null)
                {
                    return _tempStream;
                }

                return null;
            }

            set 
            {
                if (value == null)
                {
                    if(!String.IsNullOrEmpty(_hash))
                    {
                        RemoveImageConsumer();
                        _hash = String.Empty;
                    }
                }
                else
                {
                    if (HashStreamManager != null)
                    {
                        string newHash = HashStreamManager.SetStream(value, StreamType);
                        if (newHash != _hash)
                        {
                            if (!String.IsNullOrEmpty(_hash))
                            {
                                RemoveImageConsumer();
                            }
                            _hash = newHash;
                            AddImageConsumer();
                        }
                    }
                    else
                    {
                        // This widget has been not added to a document, save to the temperary stream

                        long oldPostion = value.Position;

                        value.Position = 0;
                        _tempStream = new MemoryStream();
                        value.CopyTo(_tempStream);
                        _tempStream.Position = 0;
                        value.Position = oldPostion;
                    }
                }
            }
        }

        #region Events

        internal override void OnAddToDocument()
        {
            base.OnAddToDocument();

            // The _tempStream is out of use after this widget was added to a document.
            if (_tempStream != null)
            {
                _hash = ParentDocumentObject.ImagesStreamManager.SetStream(_tempStream, StreamType);
                _tempStream.Close();
                _tempStream = null;
            }

            if (!String.IsNullOrEmpty(_hash))
            {
                AddImageConsumer();
            }
        }

        internal override void OnDeleteFromDocument(bool isParentPageDeleted)
        {
            if (!isParentPageDeleted && !String.IsNullOrEmpty(_hash))
            {
                RemoveImageConsumer();
            }
        }

        #endregion

        protected abstract IHashStreamManager HashStreamManager { get; }

        protected void AddImageConsumer()
        {
            if (ParentDocumentObject != null)
            {
                ParentDocumentObject.ImagesData.AddConsumer(this);
            }
        }

        protected void RemoveImageConsumer()
        {
            if (ParentDocumentObject != null)
            {
                ParentDocumentObject.ImagesData.DeleteConsumer(this);
            }
        }

        private string _hash;
        private Stream _tempStream;
    }
}
