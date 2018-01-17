using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    /*
     * Set for annotation fields. 
     * Annotation field name is unique and cannot create two or more fields with same name in this set.
     * */
    public interface IAnnotationFieldSet
    {
        IDocument ParentDocument { get; }

        IAnnotationFields AnnotationFields { get; }

        IAnnotationField CreateAnnotationField(string fieldName, AnnotationFieldType type);

        void DeleteAnnotationField(string fieldName);

        bool MoveAnnotationField(string fieldName, int delta);
    }
}
