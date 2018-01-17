using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class AnnotationField : XmlElementObject, IAnnotationField
    {
        #region Constructors

        internal AnnotationField(AnnotationFieldSet set)
            : this(set, string.Empty, AnnotationFieldType.Text)
        {
        }

        internal AnnotationField(AnnotationFieldSet set, string name, AnnotationFieldType type)
            : base("AnnotationField")
        {
            _set = set;
            _name = name;
            _type = type;
        }

        #endregion

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadStringFromChildElementInnerText("Name", element, ref _name);
            LoadEnumFromChildElementInnerText<AnnotationFieldType>("Type", element, ref _type);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            SaveStringToChildElement("Name", _name, xmlDoc, element);
            SaveStringToChildElement("Type", _type.ToString(), xmlDoc, element);
        }

        #endregion

        #region INamedObject

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region IAnnotationField

        public IAnnotationFieldSet AnnotationFieldSet
        {
            get { return _set; }
        }

        public AnnotationFieldType Type
        {
            get { return _type; }
        }

        #endregion

        #region Private Fields

        private AnnotationFieldSet _set;
        private string _name;
        private AnnotationFieldType _type = AnnotationFieldType.Text;

        #endregion
    }
}
