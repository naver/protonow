using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialImage : SerialWidgetBase
    {
        [NonSerialized]
        private IImage _element;

        #region public interface functions
        public SerialImage(IImage wdg) : base(wdg)
        {
            _element = wdg;
            InitializeProperty();
        }

        public void Update(IImage newWdg)
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
            Tooltip = _element.Tooltip;
            IsDisabled = _element.IsDisabled;
            Name = _element.Name;
            ImgHash=_element.Hash;
            ImageType=_element.ImageType;
        }
        #endregion

        #region Serialize Data
        public WidgetType ShapeType;
        public string Tooltip;
        public bool IsDisabled;
        public string Name;

        public string ImgHash;
        public ImageType ImageType;

        #endregion
    }
}
