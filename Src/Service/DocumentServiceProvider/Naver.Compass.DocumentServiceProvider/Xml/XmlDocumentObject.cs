using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    /// <summary>
    /// An object which data file is a xml file.
    /// </summary>
    internal abstract class XmlDocumentObject : XmlElementObject
    {
        internal XmlDocumentObject(string tagName)
            : base(tagName)
        {
        }

        internal void Load(string xmlFileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName);

            XmlElement documentElement = xmlDoc.DocumentElement;
            LoadDataFromXml(documentElement);
        }

        internal void Save(string xmlFileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            SaveDataToXml(xmlDoc, null);

            // If this property is false, XmlDocument auto-indents the output. This will make document contain lots of 
            // white space. Set this property to true to reduce document size.
            xmlDoc.PreserveWhitespace = true;

            xmlDoc.Save(xmlFileName);
        }

        internal void Load(Stream stream)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);

            XmlElement documentElement = xmlDoc.DocumentElement;
            LoadDataFromXml(documentElement);
        }

        internal void Save(Stream stream)
        {
            XmlDocument xmlDoc = new XmlDocument();
            SaveDataToXml(xmlDoc, null);

            // If this property is false, XmlDocument auto-indents the output. This will make document contain lots of 
            // white space. Set this property to true to reduce document size.
            xmlDoc.PreserveWhitespace = true;

            xmlDoc.Save(stream);
        }
    }
}
