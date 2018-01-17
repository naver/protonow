using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Windows;

namespace Naver.Compass.Service.Html
{
    [Serializable]
    internal class SerialDynamicPanel : SerialWidgetBase
    {
        [NonSerialized]
        private IDynamicPanel _element;

        #region public interface functions
        public SerialDynamicPanel(IDynamicPanel wdg) : base(wdg)
        {
            _element = wdg;
            InitializeProperty();
        }

        public void Update(IDynamicPanel newWdg)
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

            IsAutomatic = _element.IsAutomatic;
            IsCircular = _element.IsCircular;
            AutomaticIntervalTime = _element.AutomaticIntervalTime;
            DurationTime = _element.DurationTime;
            NavigationType = _element.NavigationType;
            ViewMode = _element.ViewMode;
            PanelWidth = _element.PanelWidth;
            LineWith = _element.LineWith;

            PanelCount = _element.PanelStatePages.Count;
            for (int i= 0; i< PanelCount; i++)
            {
                PanelsMD5 += MD5HashManager.GetHash(_element.PanelStatePages[i], true);
            }            
        }

        #endregion

        #region Serialize Data
        public WidgetType ShapeType;
        public string Tooltip;
        public bool IsDisabled;
        public string Name;

        public bool IsAutomatic;
        public bool IsCircular;
        public int AutomaticIntervalTime;
        public int DurationTime;
        public NavigationType NavigationType;
        public DynamicPanelViewMode ViewMode;
        public int PanelWidth;
        public double LineWith;

        public int PanelCount;
        public string PanelsMD5;
        #endregion
    }
}
