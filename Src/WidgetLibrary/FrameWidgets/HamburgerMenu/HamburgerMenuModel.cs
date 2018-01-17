using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Naver.Compass.WidgetLibrary
{
    public class HamburgerMenuModel : ImageModel
    {
        public HamburgerMenuModel(IWidget widget)
            : base(widget)
        {
            _hamburger = widget as IHamburgerMenu;

            if(_style.IsFixed!=_menuButtonStyle.IsFixed)
            {
                _menuButtonStyle.IsFixed = _style.IsFixed;
            }
        }

        public bool IsAnyChildrenPageOpen()
        {
            return _hamburger.MenuPage.IsOpened;
        }

        #region Menu button own binding style property
        override public double Left
        {
            get { return _menuButtonStyle.X; }
            set
            {
                if (_menuButtonStyle.X != value)
                {
                    _menuButtonStyle.X = value;
                    _document.IsDirty = true;
                }
            }
        }
        override public double Top
        {
            get { return _menuButtonStyle.Y; }
            set
            {
                if (_menuButtonStyle.Y != value)
                {
                    _menuButtonStyle.Y = value;
                    _document.IsDirty = true;
                }
            }
        }
        override public double ItemWidth
        {
            get { return _menuButtonStyle.Width; }
            set
            {
                if (_menuButtonStyle.Width != value)
                {
                    _menuButtonStyle.Width = value;
                    _document.IsDirty = true;
                }
            }
        }
        override public double ItemHeight
        {
            get { return _menuButtonStyle.Height; }
            set
            {
                if (_menuButtonStyle.Height != value)
                {
                    _menuButtonStyle.Height = value;
                    _document.IsDirty = true;
                }
            }
        }
        override public bool IsVisible
        {
            get { return _menuButtonStyle.IsVisible; }
            set
            {
                if (_menuButtonStyle.IsVisible != value)
                {
                    _menuButtonStyle.IsVisible = value;
                    _document.IsDirty = true;
                }
            }
        }

        public override bool IsFixed
        {
            get
            {
                return base.IsFixed;
            }

            set
            {
                if (_menuButtonStyle.IsFixed != value)
                {
                    _menuButtonStyle.IsFixed = value;
                }
                base.IsFixed = value;
            }
        }
        #endregion Menu button own style property

        #region Binding Style Property
        public double MenuPageLeft
        {
            get {return base.Left; }
            set {base.Left = value;}
        }
        public double MenuPageTop
        {
            get { return base.Top; }
            set { base.Top = value; }
        }
        public double MenuPageWidth
        {
            get { return base.ItemWidth; }
            set { base.ItemWidth = value; }
        }
        public double MenuPageHeight
        {
            get { return base.ItemHeight; }
            set { base.ItemHeight = value; }
        }
        public bool IsMenuPageVisible
        {
            get { return base.IsVisible; }
            set { base.IsVisible = value; }
        }

        
        #endregion Binding Style Property

        #region Self property for binding
        public HiddenOn HiddenOn
        {
            get { return _hamburger.HiddenOn; }
            set
            {
                if (_hamburger.HiddenOn != value)
                {
                    _hamburger.HiddenOn = value;
                    _document.IsDirty = true;
                }
            }
        }
        public override string Tooltip
        {
            get
            {
                return _hamburger.Tooltip;
            }
            set
            {
                if(_hamburger.Tooltip!=value)
                {
                    _hamburger.Tooltip = value;
                    _document.IsDirty = true;
                }
            }
        }
        override public Stream ImageStream
        {
            get
            {
                return _hamburger.MenuButton.ImageStream;
            }
            set
            {
                _hamburger.MenuButton.ImageStream = value;
                _document.IsDirty = true;
            }
        }
        #endregion

        #region Private members
        private IHamburgerMenu _hamburger = null;
        private IWidgetStyle _menuButtonStyle
        {
            get
            { 
                IWidgetStyle buttonStyle;
                buttonStyle = _hamburger.MenuButton.GetWidgetStyle(StyleGID);
                if (buttonStyle == null)
                {
                    buttonStyle = _hamburger.MenuButton.WidgetStyle;
                }
                return buttonStyle;
            }
        }
        #endregion
    }
}
