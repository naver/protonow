using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialWidgetEventBase : SerialWidgetBase
    {

        #region public interface functions
        public SerialWidgetEventBase(IWidget wdg) : base(wdg)
        {
            InitializeEvent();
        }
        override protected void Update(IWidget newWdg)
        {
            base.Update(newWdg);
            InitializeEvent();
        }
        #endregion

        #region private functions
        private void InitializeEvent()
        {
            Events=_wdg.Events;
        }
        #endregion

        #region Serialize Data
        [NonSerialized]
        IInteractionEvents Events;
        #endregion
    }
}
