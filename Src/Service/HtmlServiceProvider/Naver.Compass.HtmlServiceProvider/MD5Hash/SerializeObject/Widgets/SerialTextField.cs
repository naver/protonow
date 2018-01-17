using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialTextField : SerialWidgetBase
    {
        [NonSerialized]
        private ITextField _element;

        #region public interface functions
        public SerialTextField(ITextField wdg) : base(wdg)
        {
            _element = wdg;
            InitializeProperty();
        }

        public void Update(ITextField newWdg)
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

            Text = _element.Text;
            HintText = _element.HintText;
            HideBorder = _element.HideBorder;
            ReadOnly = _element.ReadOnly;
            TextFieldType = _element.TextFieldType;
        }
        #endregion

        #region Serialize Data
        public string Text;
        public string HintText;
        public bool HideBorder;
        public bool ReadOnly;
        public TextFieldType TextFieldType;


        public WidgetType ShapeType;
        public string Tooltip;
        public bool IsDisabled;
        public string Name;
        #endregion
    }
}
