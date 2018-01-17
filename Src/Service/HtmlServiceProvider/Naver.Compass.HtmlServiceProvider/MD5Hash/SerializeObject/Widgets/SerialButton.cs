using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialButton : SerialWidgetBase
    {
        [NonSerialized]
        private IButton _element;

        #region public interface functions
        public SerialButton(IButton wdg) : base(wdg)
        {
            _element = wdg;
            InitializeProperty();
        }

        public void Update(IButton newWdg)
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
            RichText= _element.RichText;
            Text= _element.Text;
        }

        #endregion

        #region Serialize Data
        public WidgetType ShapeType;
        public string Tooltip;
        public bool IsDisabled;
        public string Name;
        public string RichText;
        public string Text;
        #endregion
    }
}
