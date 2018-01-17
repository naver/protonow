using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class Image : StreamWidget, IImage
    {
        internal Image(Page parentPage)
            : base(parentPage, "Image")
        {
            _widgetType = WidgetType.Image;

            _imageType = ImageType.PNG;

            InitializeBaseViewStyleFromDefaultStyle();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            if (!LoadEnumFromChildElementInnerText<ImageType>("ImageType", element, ref _imageType))
            {
                throw new XmlException("ImageType element is invalid!");
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement imageElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(imageElement);

            SaveStringToChildElement("ImageType", _imageType.ToString(), xmlDoc, imageElement);

            base.SaveDataToXml(xmlDoc, imageElement);
        }

        #endregion

        public override string StreamType
        {
            get { return _imageType.ToString().ToLower(); }
            set { }
        }

        public ImageType ImageType 
        {
            get { return _imageType; }
            set { _imageType = value; }
        }

        public Stream ImageStream 
        {
            get { return DataStream; }

            set { DataStream = value; }
        }

        internal static bool SupportedStyleProperty(string stylePropertyName)
        {
            return _supportStyleProperties.ContainsKey(stylePropertyName);
        }

        protected override IHashStreamManager HashStreamManager
        {
            get
            {
                return ParentDocumentObject != null ? ParentDocumentObject.ImagesStreamManager : null;
            }
        }

        protected override Dictionary<string, StyleProperty> SupportStyleProperties
        {
            get { return _supportStyleProperties; }
        }

        private static Dictionary<string, StyleProperty> _supportStyleProperties = new Dictionary<string, StyleProperty>();

        static Image()
        {
            // Add support style properties and default value.
            _supportStyleProperties[StylePropertyNames.WIDGET_ROTATE_PROP] = new StyleDoubleProperty(StylePropertyNames.WIDGET_ROTATE_PROP, 0);
            _supportStyleProperties[StylePropertyNames.OPACITY_PROP] = new StyleIntegerProperty(StylePropertyNames.OPACITY_PROP, 100);
        }

        private ImageType _imageType;
    }
}
