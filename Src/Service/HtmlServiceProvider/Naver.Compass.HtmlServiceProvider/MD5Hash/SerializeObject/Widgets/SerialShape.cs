using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialShape:SerialWidgetBase
    {
        [NonSerialized]
        private IShape _shape;

        #region public interface functions
        public SerialShape(IShape wdg) : base(wdg)
        {
            _shape = wdg;
            InitializeProperty();
        }

        public void Update(IShape newWdg)
        {
            _shape = newWdg;
            base.Update(newWdg);
            InitializeProperty();
        }
        #endregion

        #region private functions
        private void InitializeProperty()
        {
            ShapeType=_shape.ShapeType;
            Tooltip=_shape.Tooltip;
            IsDisabled=_shape.IsDisabled;
            Name=_shape.Name;
            RichText= HtmlFromXamlConverter.TransformXamlToHtml(_shape.RichText);
            Text=_shape.Text;
        }

        #endregion

        #region Serialize Data
        public ShapeType ShapeType;
        public string Tooltip;
        public bool IsDisabled;
        public string Name;
        public string RichText;
        public string Text;
        #endregion
    }
}
