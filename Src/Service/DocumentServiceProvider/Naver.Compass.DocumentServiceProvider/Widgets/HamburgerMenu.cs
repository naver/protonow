using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    internal class HamburgerMenu : PageEmbeddedWidget, IHamburgerMenu
    {
        internal HamburgerMenu(Page parentPage)
            : base(parentPage, "HamburgerMenu")
        {
            _widgetType = WidgetType.HamburgerMenu;

            _button = new HamburgerMenuButton(this, parentPage);
            _page = new HamburgerMenuPage(this, "");

            InitializeBaseViewStyleFromDefaultStyle();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            LoadEnumFromChildElementInnerText<HiddenOn>("HiddenOn", element, ref _hiddenOn);

            XmlElement buttonElement = element[_button.TagName];
            if (buttonElement != null)
            {
                _button.LoadDataFromXml(buttonElement);
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

            SaveStringToChildElement("HiddenOn", _hiddenOn.ToString(), xmlDoc, element);

            _button.SaveDataToXml(xmlDoc, element);

            _page.SaveDataToXml(xmlDoc, element);

            base.SaveDataToXml(xmlDoc, element);
        }

        #endregion

        public override Guid Guid
        {
            set 
            { 
                base.Guid = value;
                _button.Guid = value; // Also set to child image button as it has the same guid with menu.
            }
        }

        public override IPage ParentPage
        {
            set
            {
                base.ParentPage = value;
                _button.ParentPage = value; // Also set to child image button.
            }
        }

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

        public IHamburgerMenuPage MenuPage
        {
            get { return _page; }
        }

        public IHamburgerMenuButton MenuButton
        {
            get { return _button; }
        }

        public HiddenOn HiddenOn
        {
            get { return _hiddenOn; }
            set { _hiddenOn = value; }
        }

        internal override Guid CreatedViewGuid
        {
            set 
            {
                base.CreatedViewGuid = value;
                _button.CreatedViewGuid = value; 
            }
        }

        internal override bool HasBeenPlacedInBaseView
        {
            set 
            {
                base.HasBeenPlacedInBaseView = value;
                _button.HasBeenPlacedInBaseView = value; 
            }
        }

        internal override void OnAddToDocument()
        {
            base.OnAddToDocument();
            _button.OnAddToDocument();
        }

        internal override void OnDeleteFromDocument(bool isParentPageDeleted)
        {
            base.OnDeleteFromDocument(isParentPageDeleted);
            _button.OnDeleteFromDocument(isParentPageDeleted);
        }

        internal override void OnAddAdaptiveView(AdaptiveView view)
        {
            base.OnAddAdaptiveView(view);
            _button.OnAddAdaptiveView(view);
        }

        internal override void OnDeleteAdaptiveView(Guid viewGuid)
        {
            base.OnDeleteAdaptiveView(viewGuid);
            _button.OnDeleteAdaptiveView(viewGuid);
        }

        internal override void RebuildStyleChain(WidgetStyle newBaseStyle)
        {
            base.RebuildStyleChain(newBaseStyle);
            _button.RebuildStyleChain(newBaseStyle);
        }

        internal static bool SupportedStyleProperty(string stylePropertyName)
        {
            return false;
        }

        protected override Dictionary<string, StyleProperty> SupportStyleProperties
        {
            get { return new Dictionary<string,StyleProperty>(); }
        }

        private HiddenOn _hiddenOn = HiddenOn.Left;
        private HamburgerMenuButton _button;
        private HamburgerMenuPage _page;
    }
}
