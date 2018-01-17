using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public enum AnnotationFieldType
    {
        Text
    }

    public interface IAnnotationField : INamedObject
    {
        IAnnotationFieldSet AnnotationFieldSet { get; }

        AnnotationFieldType Type { get; }
    }
}
