using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class AnnotationFields : XmlElementList<AnnotationField>, IAnnotationFields
    {
        internal AnnotationFields(AnnotationFieldSet set)
            : base("AnnotationFields")
        {
            _set = set;
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
                AnnotationField field = new AnnotationField(_set);
                field.LoadDataFromXml(childElement);
                Add(field);
            }
        }

        #endregion

        #region IAnnotationFields 

        public IAnnotationFieldSet AnnotationFieldSet
        {
            get { return _set; }
        }

        public IAnnotationField GetAnnotationField(string fieldName)
        {
            return _list.FirstOrDefault(x => x.Name == fieldName);
        }

        public bool Contains(string fieldName)
        {
            return _list.Any(x => x.Name == fieldName);
        }

        public IAnnotationField this[string fieldName]
        {
            get { return GetAnnotationField(fieldName); }
        }

        #endregion

        private AnnotationFieldSet _set;
    }
}
