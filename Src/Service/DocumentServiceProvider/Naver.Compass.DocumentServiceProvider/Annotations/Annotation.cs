using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    // Annotation contains values of annotation field about a IAnnotatedObject,
    // those values are corresponding to annotation field in annotation set.
    internal class Annotation : XmlElementObject, IAnnotation
    {
        internal Annotation(IAnnotatedObject annotatedObject)
            : base("Annotation")
        {
            // Annotation always resides in a IAnnotatedObject.
            Debug.Assert(annotatedObject != null);

            _annotatedObject = annotatedObject;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            XmlNodeList childList = element.ChildNodes;
            if (childList == null || childList.Count <= 0)
            {
                return;
            }

            foreach (XmlElement childElement in childList)
            {
                try
                {
                    XmlElement nameElement = childElement["Name"];
                    XmlElement textValueElement = childElement["TextValue"];

                    if (nameElement != null && textValueElement != null
                        && !string.IsNullOrEmpty(nameElement.InnerText) 
                        && !string.IsNullOrEmpty(textValueElement.InnerText))
                    {
                        _textValues[nameElement.InnerText] = textValueElement.InnerText;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            Dictionary<string, string> copyTextValues = new Dictionary<string, string>(_textValues);
            foreach (KeyValuePair<string, string> item in copyTextValues)
            {
                XmlElement fieldElement = xmlDoc.CreateElement("Field");
                element.AppendChild(fieldElement);

                XmlElement nameElement = xmlDoc.CreateElement("Name");
                fieldElement.AppendChild(nameElement);
                nameElement.InnerText = item.Key;

                XmlElement textValueElement = xmlDoc.CreateElement("TextValue");
                fieldElement.AppendChild(textValueElement);
                textValueElement.InnerText = item.Value;
            }
        }

        #endregion

        #region IAnnotation

        public IAnnotatedObject AnnotatedObject
        {
            get { return _annotatedObject; }
            set { _annotatedObject = value; }
        }

        public bool IsEmpty
        {
            get { return _textValues.Count <= 0; }
        }

        public string GetextValue(string fieldName)
        {
            if (_textValues.ContainsKey(fieldName))
            {
                return _textValues[fieldName];
            }
            else
            {
                return String.Empty;
            }
        }

        public void SetTextValue(string fieldName, string textValue)
        {
            if (!String.IsNullOrEmpty(fieldName))
            {
                if (!String.IsNullOrEmpty(textValue))
                {
                    // The text value is not null or empty, set the text value to values collection.
                    _textValues[fieldName] = textValue;
                }
                else if (_textValues.ContainsKey(fieldName))
                {
                    // The text value is null or empty, and values collection has this field name, remove it from collection.
                    _textValues.Remove(fieldName);
                }
            }
        }

        public void Clear()
        {
            _textValues.Clear();
        }

        #endregion

        public Dictionary<string, string> TextValues
        {
            get { return _textValues; }
            set { _textValues = new Dictionary<string, string>(value); }
        }

        private IAnnotatedObject _annotatedObject;
        private Dictionary<string, string> _textValues = new Dictionary<string, string>();
    }
}
