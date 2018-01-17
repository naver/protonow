using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class Guide : XmlElementObject, IGuide
    {
        public Guide()
            : this(Orientation.Vertical, 0, 0)
        {
        }

        public Guide(Orientation orientation, double x, double y)
            : base("Guide")
        {
            _orientation = orientation;
            _x = x;
            _y = y;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadElementGuidAttribute(element, ref _guid);

            LoadEnumFromChildElementInnerText<Orientation>("Orientation", element, ref _orientation);
            LoadDoubleFromChildElementInnerText("X", element, ref _x);
            LoadDoubleFromChildElementInnerText("Y", element, ref _y);
            LoadBoolFromChildElementInnerText("IsLocked", element, ref _isLocked);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement guideElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(guideElement);

            SaveElementGuidAttribute(guideElement, _guid);

            SaveStringToChildElement("Orientation", _orientation.ToString(), xmlDoc, guideElement);
            SaveStringToChildElement("X", _x.ToString(), xmlDoc, guideElement);
            SaveStringToChildElement("Y", _y.ToString(), xmlDoc, guideElement);
            SaveStringToChildElement("IsLocked", _isLocked.ToString(), xmlDoc, guideElement);
        }

        #endregion

        #region IUniqueObject

        public Guid Guid
        {
            get { return _guid; }
        }

        #endregion

        #region IGuide

        public Orientation Orientation 
        {
            get { return _orientation; }
        }

        public double X 
        {
            get { return _x; }
            set { _x = value; } 
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }

        #endregion

        #region Private fields

        Guid _guid = Guid.NewGuid();
        Orientation _orientation = Orientation.Vertical;
        double _x;
        double _y;
        bool _isLocked;

        #endregion
    }
}
