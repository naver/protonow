using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialToast : SerialWidgetBase
    {
        [NonSerialized]
        private IToast _element;

        #region public interface functions
        public SerialToast(IToast wdg) : base(wdg)
        {
            _element = wdg;
            InitializeProperty();
        }

        public void Update(IToast newWdg)
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
            ExposureTime = _element.ExposureTime;
            CloseSetting = _element.CloseSetting;
            DisplayPosition = _element.DisplayPosition;
            ChildPageMD5 = MD5HashManager.GetHash(_element.ToastPage,true);
        }

        #endregion

        #region Serialize Data
        public WidgetType ShapeType;
        public string Tooltip;
        public bool IsDisabled;
        public string Name;

        public int ExposureTime;
        public ToastCloseSetting CloseSetting;
        public ToastDisplayPosition DisplayPosition;
        public string ChildPageMD5;
        // "toastWidgets":[
        #endregion
    }
}
