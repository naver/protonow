using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IAnnotation
    {
        IAnnotatedObject AnnotatedObject { get; }

        bool IsEmpty { get; }

        string GetextValue(string fieldName);

        void SetTextValue(string fieldName, string textValue);

        void Clear();

        Dictionary<string, string> TextValues { get; set;}
    }
}
