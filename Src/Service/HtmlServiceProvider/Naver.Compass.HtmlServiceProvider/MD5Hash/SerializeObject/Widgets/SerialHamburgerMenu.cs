using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialHamburgerMenu : SerialWidgetBase
    {
        [NonSerialized]
        private IHamburgerMenu _element;

        #region public interface functions
        public SerialHamburgerMenu(IHamburgerMenu wdg) : base(wdg)
        {
            _element = wdg;
            InitializeProperty();
        }

        public void Update(IHamburgerMenu newWdg)
        {
            _element = newWdg;
            base.Update(newWdg);
            InitializeProperty();
        }
        #endregion

        #region private functions
        private void InitializeProperty()
        {
            ShapeType = _element.WidgetType;
            Tooltip= _element.Tooltip;
            IsDisabled= _element.IsDisabled;
            Name= _element.Name;

            HiddenOn = _element.HiddenOn;
            MenuButtonMD5 = MD5HashManager.GetHash(_element.MenuButton, true); ;
            ChildPageMD5 = MD5HashManager.GetHash(_element.MenuPage, true);
        }

        #endregion

        #region Serialize Data
        public WidgetType ShapeType;
        public string Tooltip;
        public bool IsDisabled;
        public string Name;

        public HiddenOn HiddenOn;
        public string MenuButtonMD5;
        public string ChildPageMD5;
        #endregion
    }
}
