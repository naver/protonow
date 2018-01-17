using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class AnnotationFieldSet : XmlElementObject, IAnnotationFieldSet
    {
        internal AnnotationFieldSet(Document document, string tagName)
            : base(tagName)
        {
            _document = document;
            _fields = new AnnotationFields(this);
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            // Load annotation fields
            XmlElement fieldsElement = element[_fields.TagName];
            if (fieldsElement != null)
            {
                _fields.LoadDataFromXml(fieldsElement);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            _fields.SaveDataToXml(xmlDoc, element);
        }

        #endregion

        #region IAnnotationFieldSet 

        public IDocument ParentDocument
        {
            get { return _document; }
        }

        public IAnnotationFields AnnotationFields 
        {
            get { return _fields; }
        }

        public IAnnotationField CreateAnnotationField(string fieldName, AnnotationFieldType type)
        {
            if (_fields.Contains(fieldName))
            {
                throw new ArgumentException("Field " + fieldName + " already exists!");
            }

            AnnotationField field = new AnnotationField(this, fieldName, type);
            _fields.Add(field);
            return field;
        }

        public void DeleteAnnotationField(string fieldName)
        {
            AnnotationField field = _fields.GetAnnotationField(fieldName) as AnnotationField;
            if (field != null)
            {
                _fields.Remove(field);
            }
        }

        public bool MoveAnnotationField(string fieldName, int delta)
        {
            AnnotationField field = _fields.GetAnnotationField(fieldName) as AnnotationField;
            if (field == null)
            {
                return false;
            }

            return _fields.MoveItem(field, delta);
        }

        #endregion

        #region Internal Methods

        internal void Clear()
        {
            _fields.Clear();
        }

        #endregion 

        #region Private Fields

        private Document _document;
        private AnnotationFields _fields;

        #endregion
    }
}
