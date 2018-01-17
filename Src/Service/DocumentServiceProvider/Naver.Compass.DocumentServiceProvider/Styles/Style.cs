using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    internal class Style : XmlElementObject, IStyle
    {
        public Style(string tagName)
            : base(tagName)
        {            
        }

        public IEnumerator<IStyleProperty> GetEnumerator()
        {
            return _styleProperties.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            // Style only create string or color properties, if you want they to be specific type properties, override this method. 
            XmlElement propertiesElement = element["StyleProperties"];
            if (propertiesElement != null && propertiesElement.ChildNodes.Count > 0 )
            {
                foreach (XmlElement propertyElement in propertiesElement.ChildNodes)
                {
                    XmlElement colorElement = propertyElement["Color"];
                    if (colorElement != null)
                    {
                        StyleColor color = new StyleColor();
                        if (LoadStyleColorFromXml(colorElement, ref color))
                        {
                            SetStyleProperty(propertyElement.Name, color);
                        }
                    }
                    else
                    {
                        string value = null;
                        if (LoadStringFromChildElementInnerText("Value", propertyElement, ref value))
                        {
                            SetStyleProperty(propertyElement.Name, value);
                        }
                    }
                }
            }
        }

        /*
         * <StyleProperties>
         *     <PropertyName>
         *         <Value></Value>   
         *     </PropertyName>
         *     <PropertyName>
         *         <Color>
         *             ...
         *         </Color>   
         *     </PropertyName>
         * </StyleProperties>
         * */
        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            CheckTagName(parentElement);

            XmlElement propertiesElement = xmlDoc.CreateElement("StyleProperties");
            parentElement.AppendChild(propertiesElement);

            foreach (StyleProperty property in _styleProperties.Values)
            {               
                XmlElement propertyElement = xmlDoc.CreateElement(property.Name);
                propertiesElement.AppendChild(propertyElement);

                if (property is StyleColorProperty)
                {
                    StyleColorProperty colorProperty = property as StyleColorProperty;
                    SaveStyleColorToXml(colorProperty.ColorValue, xmlDoc, propertyElement);
                }
                else
                {
                    SaveStringToChildElement("Value", property.Value, xmlDoc, propertyElement);
                }
            }
        }

        #endregion

        public int Count
        {
            get { return _styleProperties.Count; }
        }

        public bool Contains(string name)
        {
            return _styleProperties.ContainsKey(name);
        }

        public IStyleProperty GetStyleProperty(string name)
        {
            if(Contains(name))
            {
                return _styleProperties[name];
            }

            return null;
        }

        public IStyleProperty SetStyleProperty(string name, bool value)
        {
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            StyleBooleanProperty property = null;
            if (Contains(name))
            {
                property = _styleProperties[name] as StyleBooleanProperty;
                if (property != null && property.BooleanValue != value)
                {
                    property.BooleanValue = value;
                }
            }

            if (property == null)
            {
                property = new StyleBooleanProperty(name, value);
                _styleProperties[name] = property;
            }

            return property;
        }

        public IStyleProperty SetStyleProperty(string name, double value)
        {
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            StyleDoubleProperty property = null;
            if (Contains(name))
            {
                property = _styleProperties[name] as StyleDoubleProperty;
                if (property != null && property.DoubleValue != value)
                {
                    property.DoubleValue = value;
                }
            }

            if (property == null)
            {
                property = new StyleDoubleProperty(name, value);
                _styleProperties[name] = property;
            }

            return property;
        }

        public IStyleProperty SetStyleProperty<T>(string name, T enumValue)
                where T : struct, IConvertible
        {
            if (String.IsNullOrEmpty(name) || !typeof(T).IsEnum)
            {
                return null;
            }

            StyleEnumProperty<T> property = null;
            if (Contains(name))
            {
                property = _styleProperties[name] as StyleEnumProperty<T>;
                if (property != null && !property.EnumValue.Equals(enumValue))
                {
                    property.EnumValue = enumValue;
                }
            }

            if (property == null)
            {
                property = new StyleEnumProperty<T>(name, enumValue);
                _styleProperties[name] = property;
            }

            return property;
        }

        
        public IStyleProperty SetStyleProperty(string name, int value)
        {
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            StyleIntegerProperty property = null;
            if (Contains(name))
            {
                property = _styleProperties[name] as StyleIntegerProperty;
                if (property != null && property.IntegerValue != value)
                {
                    property.IntegerValue = value;
                }
            }

            if (property == null)
            {
                property = new StyleIntegerProperty(name, value);
                _styleProperties[name] = property;
            }

            return property;
        }

        public IStyleProperty SetStyleProperty(string name, string value)
        {
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            StyleProperty property = null;
            if (Contains(name))
            {
                property = _styleProperties[name];
                if (property != null && property.Value != value)
                {
                    property.Value = value;
                }
            }

            if (property == null)
            {
                property = new StyleProperty(name, value);
                _styleProperties[name] = property;
            }

            return property;
        }

        public IStyleProperty SetStyleProperty(string name, StyleColor color)
        {
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            StyleColorProperty property = null;
            if (Contains(name))
            {
                property = _styleProperties[name] as StyleColorProperty;
                if (property != null && !property.ColorValue.Equals(color))
                {
                    property.ColorValue = new StyleColor(color);
                }
            }

            if (property == null)
            {
                property = new StyleColorProperty(name, color);
                _styleProperties[name] = property;
            }

            return property;
        }

        public bool RemoveStyleProperty(string name)
        {
            return _styleProperties.Remove(name);
        }

        public void Clear()
        {
            _styleProperties.Clear();
        }

        internal void SetStyleProperty(StyleProperty property)
        {
            if (property == null || String.IsNullOrEmpty(property.Name))
            {
                return;
            }

            _styleProperties[property.Name] = property;
        }

        internal static void CopyStyle(Style source, Style target, List<string> includingList, List<string> excludingList)
        {
            // Including list has high priority.
            if (includingList != null)
            {
                foreach (StyleProperty property in source._styleProperties.Values)
                {                    
                    if (includingList.Contains(property.Name))
                    {
                        target.SetStyleProperty(property.Clone());
                    }
                }
            }
            else if(excludingList != null)
            {
                foreach (StyleProperty property in source._styleProperties.Values)
                {
                    if (!excludingList.Contains(property.Name))
                    {
                        target.SetStyleProperty(property.Clone());
                    }
                }
            }
            else
            {
                foreach (StyleProperty property in source._styleProperties.Values)
                {
                    target.SetStyleProperty(property.Clone());
                }
            }
        }

        protected bool LoadStyleBooleanProperty(XmlElement parentElement, string propertyName)
        {
            XmlElement propertyElement = parentElement[propertyName];
            if (propertyElement != null)
            {
                string value = null;
                if (LoadStringFromChildElementInnerText("Value", propertyElement, ref value))
                {
                    StyleBooleanProperty property = new StyleBooleanProperty(propertyName, false);
                    property.Value = value;
                    SetStyleProperty(property);
                    return true;
                }
            }

            return false;
        }

        protected bool LoadStyleDoubleProperty(XmlElement parentElement, string propertyName)
        {
            XmlElement propertyElement = parentElement[propertyName];
            if (propertyElement != null)
            {
                string value = null;
                if (LoadStringFromChildElementInnerText("Value", propertyElement, ref value))
                {
                    StyleDoubleProperty property = new StyleDoubleProperty(propertyName, 0);
                    if (property.SetStringValue(value))
                    {
                        SetStyleProperty(property);
                        return true;
                    }
                }
            }

            return false;
        }

        protected bool LoadStyleEnumProperty<T>(XmlElement parentElement, string propertyName)
            where T : struct, IConvertible
        {
            XmlElement propertyElement = parentElement[propertyName];
            if (propertyElement != null)
            {
                string value = null;
                if (LoadStringFromChildElementInnerText("Value", propertyElement, ref value))
                {
                    StyleEnumProperty<T> property = new StyleEnumProperty<T>(propertyName, default(T));
                    if (property.SetStringValue(value))
                    {
                        SetStyleProperty(property);
                        return true;
                    }
                }
            }

            return false;
        }

        protected bool LoadStyleIntegerProperty(XmlElement parentElement, string propertyName)
        {
            XmlElement propertyElement = parentElement[propertyName];
            if (propertyElement != null)
            {
                string value = null;
                if (LoadStringFromChildElementInnerText("Value", propertyElement, ref value))
                {
                    StyleIntegerProperty property = new StyleIntegerProperty(propertyName, 0);
                    if (property.SetStringValue(value))
                    {
                        SetStyleProperty(property);
                        return true;
                    }
                }
            }

            return false;
        }

        protected bool LoadStyleStringProperty(XmlElement parentElement, string propertyName)
        {
            XmlElement propertyElement = parentElement[propertyName];
            if (propertyElement != null)
            {
                string value = null;
                if (LoadStringFromChildElementInnerText("Value", propertyElement, ref value))
                {
                    SetStyleProperty(propertyName, value);
                    return true;
                }
            }

            return false;
        }

        protected bool LoadStyleColorProperty(XmlElement parentElement, string propertyName)
        {
            XmlElement propertyElement = parentElement[propertyName];
            if (propertyElement != null)
            {
                XmlElement colorElement = propertyElement["Color"];
                if (colorElement != null)
                {
                    StyleColor color = new StyleColor();
                    if (LoadStyleColorFromXml(colorElement, ref color))
                    {
                        SetStyleProperty(propertyElement.Name, color);
                        return true;
                    }
                }
            }

            return false;
        }

        protected Dictionary<string, StyleProperty> _styleProperties = new Dictionary<string,StyleProperty>();
    }
}
