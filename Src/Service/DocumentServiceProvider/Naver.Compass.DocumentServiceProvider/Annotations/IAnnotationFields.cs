using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public interface IAnnotationFields : IEnumerable
    {
        IAnnotationFieldSet AnnotationFieldSet { get; }

        IAnnotationField GetAnnotationField(string fieldName);

        bool Contains(string fieldName);

        int Count { get; }

        IAnnotationField this[string fieldName] { get; }
    }
}
