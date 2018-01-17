using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    /// <summary>
    /// An object which data can be loaded from or saved to a XmlElement object.
    /// </summary>
    internal abstract class XmlElementObject
    {
        internal XmlElementObject(string tagName)
        {
            _tagName = tagName;
        }

        /// <summary>
        /// Get the tag name of Xml element for the object.
        /// </summary>
        internal string TagName
        {
            get { return _tagName; }
            set { _tagName = value; }
        }

        /// <summary>
        /// Load object data from an XmlElement. Make sure we don't refer other object in this method, 
        /// just load reference object Guid and get the reference object in other methods when we need it.
        /// This way, we just load data in this method and don't involve other function, and this will help 
        /// in serializing/unserializing object, because we may can not get reference object at that time. 
        /// </summary>
        /// <param name="element">Xml element contains data.</param>
        internal abstract void LoadDataFromXml(XmlElement element);

        /// <summary>
        /// Save object data to an XmlElement in a XmlDocument.
        /// 
        /// If there is a collection will be saved in this method, should copy to a temp collection before 
        /// in foreach statement. See comments in SaveDataToXml() in XmlElementDictionary.
        /// </summary>
        /// <param name="xmlDoc">Xml document object.</param>
        /// <param name="parentElement">Parent Xml element.</param>
        internal abstract void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement);

        #region Helper Methods

        protected void CheckTagName(XmlElement element)
        {
            if (element == null || element.Name != TagName)
            {
                throw new ArgumentException(TagName + " element is required!");
            }
        }

        protected bool LoadStringFromChildElementInnerText(string childElementName, XmlElement parentElement, ref string value)
        {
            if (parentElement != null)
            {
                XmlElement childElement = parentElement[childElementName];
                if (childElement != null)
                {
                    value = childElement.InnerText;
                    return true;
                }
            }

            return false;
        }

        protected bool LoadIntFromChildElementInnerText(string childElementName, XmlElement parentElement, ref int value)
        {
            if (parentElement != null)
            {
                XmlElement childElement = parentElement[childElementName];
                if (childElement != null)
                {
                    try
                    {
                        value = Int32.Parse(childElement.InnerText);
                        return true;
                    }
                    catch
                    {
                    }
                }
            }

            return false;
        }

        protected bool LoadDoubleFromChildElementInnerText(string childElementName, XmlElement parentElement, ref double value)
        {
            if (parentElement != null)
            {
                XmlElement childElement = parentElement[childElementName];
                if (childElement != null)
                {
                    try
                    {
                        value = Double.Parse(childElement.InnerText);
                        return true;
                    }
                    catch
                    {
                    }
                }
            }

            return false;
        }

        protected bool LoadBoolFromChildElementInnerText(string childElementName, XmlElement parentElement, ref bool value)
        {
            if (parentElement != null)
            {
                XmlElement childElement = parentElement[childElementName];
                if (childElement != null)
                {
                    try
                    {
                        value = Boolean.Parse(childElement.InnerText);
                        return true;
                    }
                    catch
                    {
                    }
                }
            }

            return false;
        }

        protected bool LoadEnumFromChildElementInnerText<T>(string childElementName, XmlElement parentElement, ref T value)
        {
            if (!typeof(T).IsEnum)
            {
                return false;
            }

            if (parentElement != null)
            {
                XmlElement childElement = parentElement[childElementName];
                if (childElement != null)
                {
                    try
                    {
                        value = (T)Enum.Parse(typeof(T), childElement.InnerText);
                        return true;
                    }
                    catch
                    {
                    }
                }
            }

            return false;
        }

        protected bool LoadGuidFromChildElementInnerText(string childElementName, XmlElement parentElement, ref Guid value)
        {
            if (parentElement != null)
            {
                XmlElement childElement = parentElement[childElementName];
                if (childElement != null)
                {
                    try
                    {
                        value = Guid.Parse(childElement.InnerText);
                        return true;
                    }
                    catch
                    {
                    }
                }
            }

            return false;
        }

        protected bool SaveStringToChildElement(string childElementName, string innerText, XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement childElement = xmlDoc.CreateElement(childElementName);
            parentElement.AppendChild(childElement);
            childElement.InnerText = innerText;

            return true;
        }

        protected bool LoadStyleColorFromXml(XmlElement colorElement, ref StyleColor styleColor)
        {
            if (colorElement != null)
            {
                LoadEnumFromChildElementInnerText<ColorFillType>("FillType", colorElement, ref styleColor.FillType);
                LoadIntFromChildElementInnerText("ARGB", colorElement, ref styleColor.ARGB);
                LoadDoubleFromChildElementInnerText("Angle", colorElement, ref styleColor.Angle);

                if (styleColor.Frames == null)
                {
                    styleColor.Frames = new Dictionary<double, int>();
                }

                XmlElement framesElement = colorElement["Frames"];
                if (framesElement != null)
                {
                    XmlNodeList childList = framesElement.ChildNodes;
                    if (childList != null || childList.Count > 0)
                    {
                        foreach (XmlElement childElement in childList)
                        {
                            try
                            {
                                XmlElement keyElement = childElement["Key"];
                                XmlElement valueElement = childElement["Value"];

                                if (keyElement != null && valueElement != null)
                                {
                                    styleColor.Frames[Double.Parse(keyElement.InnerText)] = Int32.Parse(valueElement.InnerText);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }

            return true;
        }

        protected bool SaveStyleColorToXml(StyleColor styleColor, XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement propertyElement = xmlDoc.CreateElement("Color");
            parentElement.AppendChild(propertyElement);
            SaveStringToChildElement("FillType", styleColor.FillType.ToString(), xmlDoc, propertyElement);
            SaveStringToChildElement("ARGB", styleColor.ARGB.ToString(), xmlDoc, propertyElement);
            SaveStringToChildElement("Angle", styleColor.Angle.ToString(), xmlDoc, propertyElement);

            if(styleColor.Frames != null)
            {
                XmlElement framesElement = xmlDoc.CreateElement("Frames");
                propertyElement.AppendChild(framesElement);

                // Copy the collection in case the original collection is modifying at this time.
                Dictionary<double, int> copyFrames = new Dictionary<double, int>(styleColor.Frames);
                foreach (KeyValuePair<double, int> frame in copyFrames)
                {
                    XmlElement frameElement = xmlDoc.CreateElement("Frame");
                    framesElement.AppendChild(frameElement);

                    SaveStringToChildElement("Key", frame.Key.ToString(), xmlDoc, frameElement);
                    SaveStringToChildElement("Value", frame.Value.ToString(), xmlDoc, frameElement);
                }
            }

            return true;
        }

        protected bool LoadElementGuidAttribute(XmlElement element, ref Guid guid)
        {
            if(element != null)
            {
                try
                {
                    string stringValue = element.GetAttribute("Guid");
                    if (!String.IsNullOrEmpty(stringValue))
                    {
                        guid = new Guid(stringValue);
                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        protected bool SaveElementGuidAttribute(XmlElement element, Guid guid)
        {
            if(element != null)
            {
                element.SetAttribute("Guid", guid.ToString());
                return true;
            }

            return false;
        }

        protected bool LoadElementBoolAttribute(XmlElement element, string name, ref bool value)
        {
            if (element != null)
            {
                try
                {
                    string stringValue = element.GetAttribute(name);
                    if (!String.IsNullOrEmpty(stringValue))
                    {
                        value = Boolean.Parse(stringValue);
                        return true;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        protected bool SaveElementBoolAttribute(XmlElement element, string name, bool value)
        {
            if (element != null)
            {
                element.SetAttribute(name, value.ToString());
                return true;
            }

            return false;
        }

        protected bool LoadElementStringAttribute(XmlElement element, string name, ref string value)
        {
            if (element != null && element.HasAttribute(name))
            {
                value = element.GetAttribute(name);
                return true;
            }

            return false;
        }

        protected bool SaveElementStringAttribute(XmlElement element, string name, string value)
        {
            if (element != null && !String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(value))
            {
                element.SetAttribute(name, value);
                return true;
            }

            return false;
        }

        #endregion

        private string _tagName;
    }
}
