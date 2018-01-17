using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    internal class Toast : PageEmbeddedWidget, IToast
    {
        internal Toast(Page parentPage)
            : base(parentPage, "Toast")
        {
            _widgetType = WidgetType.Toast;

            _page = new ToastPage(this, "");

            InitializeBaseViewStyleFromDefaultStyle();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            LoadIntFromChildElementInnerText("ExposureTime", element, ref _exposureTime);

            if (!LoadEnumFromChildElementInnerText<ToastCloseSetting>("CloseSetting", element, ref _closeSetting))
            {
                // For DOM version before 2.1.0.0 which doesn't have "CloseSetting" property
                // Old version is has close button if exposure time is <= 0.
                if (_exposureTime <= 0)
                {
                    _closeSetting = ToastCloseSetting.CloseButton;
                }
                else
                {
                    _closeSetting = ToastCloseSetting.ExposureTime;
                }
            }

            if (!LoadEnumFromChildElementInnerText<ToastDisplayPosition>("DisplayPosition", element, ref _position))
            {
                // For DOM version before 2.1.0.0 which doesn't have "DisplayPosition" property.
                // Old version position is always set by user.
                _position = ToastDisplayPosition.UserSetting;
            }

            XmlElement pageElement = element[_page.TagName];
            if (pageElement != null)
            {
                _page.PageData.Clear(); // Page data is cleared now.

                _page.LoadDataFromXml(pageElement);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            SaveStringToChildElement("ExposureTime", _exposureTime.ToString(), xmlDoc, element);
            SaveStringToChildElement("CloseSetting", _closeSetting.ToString(), xmlDoc, element);
            SaveStringToChildElement("DisplayPosition", _position.ToString(), xmlDoc, element);

            _page.SaveDataToXml(xmlDoc, element);

            base.SaveDataToXml(xmlDoc, element);
        }

        #endregion

        // IPageEmbeddedWidget
        public override ReadOnlyCollection<IEmbeddedPage> EmbeddedPages
        {
            get
            {
                List<IEmbeddedPage> list = new List<IEmbeddedPage>();
                list.Add(_page);
                return new ReadOnlyCollection<IEmbeddedPage>(list);
            }
        }

        public IToastPage ToastPage 
        {
            get { return _page; }
        }

        public int ExposureTime 
        {
            get { return _exposureTime; }
            set { _exposureTime = value; }
        }

        public ToastCloseSetting CloseSetting
        {
            get { return _closeSetting; }
            set { _closeSetting = value; }
        }

        public ToastDisplayPosition DisplayPosition
        {
            get { return _position; }
            set { _position = value; }
        }

        internal static bool SupportedStyleProperty(string stylePropertyName)
        {
            return _supportStyleProperties.ContainsKey(stylePropertyName);
        }

        protected override Dictionary<string, StyleProperty> SupportStyleProperties
        {
            get { return _supportStyleProperties; }
        }

        private static Dictionary<string, StyleProperty> _supportStyleProperties = new Dictionary<string, StyleProperty>();

        static Toast()
        {
            _supportStyleProperties[StylePropertyNames.OPACITY_PROP] = new StyleIntegerProperty(StylePropertyNames.OPACITY_PROP, 100);
        }

        private ToastPage _page;
        private int _exposureTime = 3; // Duration time for showing this toast, default is 3 seconds.
        private ToastCloseSetting _closeSetting = ToastCloseSetting.ExposureTime;
        private ToastDisplayPosition _position = ToastDisplayPosition.UserSetting;
    }
}