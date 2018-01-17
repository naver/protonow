using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    internal class DynamicPanel : PageEmbeddedWidget, IDynamicPanel
    {
        internal DynamicPanel(Page parentPage)
            : base(parentPage, "DynamicPanel")
        {
            _widgetType = WidgetType.DynamicPanel;

            InitializeBaseViewStyleFromDefaultStyle();
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);
                        
            LoadGuidFromChildElementInnerText("StartPanelStatePage", element, ref _startPageGuid);
            LoadBoolFromChildElementInnerText("IsCircular", element, ref _isCircular);
            LoadBoolFromChildElementInnerText("IsAutomatic", element, ref _isAutomatic);
            LoadIntFromChildElementInnerText("AutomaticIntervalTime", element, ref _automaticIntervalTime);
            LoadIntFromChildElementInnerText("DurationTime", element, ref _durationTime);
            LoadEnumFromChildElementInnerText<NavigationType>("NavigationType", element, ref _navigationType);
            LoadBoolFromChildElementInnerText("ShowAffordanceArrow", element, ref _showAffordanceArrow);
            LoadEnumFromChildElementInnerText<DynamicPanelViewMode>("ViewMode", element, ref _viewMode);
            LoadIntFromChildElementInnerText("PanelWidth", element, ref _panelWidth);
            LoadDoubleFromChildElementInnerText("LineWith", element, ref _lineWidth);

            XmlElement statesElement = element["States"];
            if (statesElement != null)
            {
                XmlNodeList childList = statesElement.ChildNodes;
                if (childList == null || childList.Count <= 0)
                {
                    return;
                }

                foreach (XmlElement childElement in childList)
                {
                    PanelStatePage page = new PanelStatePage(this, "");
                    page.PageData.Clear(); // Page data is cleared now.
                    page.LoadDataFromXml(childElement);
                    _states.Add(page);

                    if(_startPageGuid == page.Guid)
                    {
                        _startPanelStatePage = page;
                    }
                }
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            if (_startPanelStatePage != null)
            {
                SaveStringToChildElement("StartPanelStatePage", _startPanelStatePage.Guid.ToString(), xmlDoc, element);
            }

            SaveStringToChildElement("IsCircular", _isCircular.ToString(), xmlDoc, element);
            SaveStringToChildElement("IsAutomatic", _isAutomatic.ToString(), xmlDoc, element);
            SaveStringToChildElement("AutomaticIntervalTime", _automaticIntervalTime.ToString(), xmlDoc, element);
            SaveStringToChildElement("DurationTime", _durationTime.ToString(), xmlDoc, element);
            SaveStringToChildElement("NavigationType", _navigationType.ToString(), xmlDoc, element);
            SaveStringToChildElement("ShowAffordanceArrow", _showAffordanceArrow.ToString(), xmlDoc, element);
            SaveStringToChildElement("ViewMode", _viewMode.ToString(), xmlDoc, element);
            SaveStringToChildElement("PanelWidth", _panelWidth.ToString(), xmlDoc, element);
            SaveStringToChildElement("LineWith", _lineWidth.ToString(), xmlDoc, element);

            XmlElement statesElement = xmlDoc.CreateElement("States");
            element.AppendChild(statesElement);

            foreach (PanelStatePage state in _states)
            {
                state.SaveDataToXml(xmlDoc, statesElement);
            }

            base.SaveDataToXml(xmlDoc, element);
        }

        #endregion

        // IPageEmbeddedWidget
        public override ReadOnlyCollection<IEmbeddedPage> EmbeddedPages
        {
            get 
            {
                return new ReadOnlyCollection<IEmbeddedPage>(_states.ToList<IEmbeddedPage>());
            }
        }

        public IPanelStatePage CreatePanelStatePage(string name)
        {
            PanelStatePage page = new PanelStatePage(this, name);

            AddPanelStatePage(page, -1);

            return page;
        }

        public void AddPanelStatePage(IPanelStatePage page, int index)
        {
            // Only can add page which was created in this panel.
            PanelStatePage pageToAdd = page as PanelStatePage;
            if (pageToAdd == null || pageToAdd.ParentWidget != this)
            {
                throw new ArgumentException("Input PanelStatePage is invalid.");
            }

            if (index < 0 || index > _states.Count)
            {
                _states.Add(pageToAdd);
            }
            else
            {
                _states.Insert(index, pageToAdd);
            }

            if (ParentDocument != null)
            {
                pageToAdd.OnAddToDocument();
            }
        }

        public IPanelStatePage GetPanelStatePage(Guid pageGuid)
        {
            return _states.FirstOrDefault<IPanelStatePage>(x => x.Guid == pageGuid);
        }

        public void DeletePanelStatePage(Guid pageGuid)
        {
            PanelStatePage page = GetPanelStatePage(pageGuid) as PanelStatePage;
            if (page != null)
            {
                if (ParentDocument != null)
                {
                    page.OnDeleteFromDocument();
                }

                _states.Remove(page);
            }
        }

        public bool MovePanelStatePage(IPanelStatePage page, int delta)
        {
            PanelStatePage pageToMove = page as PanelStatePage;
            if (pageToMove == null || pageToMove.ParentWidget != this)
            {
                throw new ArgumentException("Input PanelStatePage is invalid.");
            }

            if (_states.Contains(pageToMove))
            {
                int index = _states.IndexOf(pageToMove);
                if ((delta < 0) && ((index + delta) >= 0))
                {
                    _states.RemoveAt(index);
                    _states.Insert(index + delta, pageToMove);
                    return true;
                }

                if ((delta > 0) && ((index + delta) < _states.Count))
                {
                    _states.RemoveAt(index);
                    _states.Insert(index + delta, pageToMove);
                    return true;
                }
            }

            return false;
        }

        public bool MovePanelStatePageTo(IPanelStatePage page, int index)
        {
            PanelStatePage pageToMove = page as PanelStatePage;
            if (pageToMove == null || pageToMove.ParentWidget != this)
            {
                throw new ArgumentException("Input PanelStatePage is invalid.");

            }

            if (_states.Contains(pageToMove))
            {
                int oldIndex = _states.IndexOf(pageToMove);

                if (oldIndex == index)
                {
                    return true;
                }

                if (index >= 0 && index < _states.Count)
                {
                    _states.RemoveAt(oldIndex);
                    _states.Insert(index, pageToMove);
                    return true;
                }
            }

            return false;
        }

        public int IndexOf(IPanelStatePage page)
        {
            return _states.IndexOf(page as PanelStatePage);
        }

        // Return a copy
        public List<IPanelStatePage> PanelStatePages
        {
            get { return new List<IPanelStatePage>(_states); }
        }

        public IPanelStatePage StartPanelStatePage
        {
            get
            {
                if (_startPanelStatePage == null)
                {
                    _startPanelStatePage = _states.FirstOrDefault<PanelStatePage>(x => x.Guid == _startPageGuid);
                }

                return _startPanelStatePage;
            }
            set
            {
                if (_states != null && _states.Count > 0)
                {
                    if (_states.Contains(value))
                    {
                        _startPanelStatePage = value;
                        _startPageGuid = _startPanelStatePage == null ? Guid.Empty : _startPanelStatePage.Guid;
                    }
                }
            }
        }

        public bool IsCircular
        {
            get { return _isCircular; }
            set { _isCircular = value; }
        }

        public bool IsAutomatic
        {
            get { return _isAutomatic; }
            set { _isAutomatic = value; }
        }

        public int AutomaticIntervalTime
        {
            get { return _automaticIntervalTime; }
            set { _automaticIntervalTime = value; }
        }

        public int DurationTime
        {
            get { return _durationTime; }
            set { _durationTime = value; }
        }

        public NavigationType NavigationType
        {
            get { return _navigationType; }
            set { _navigationType = value; }
        }

        public bool ShowAffordanceArrow
        {
            get { return _showAffordanceArrow; }
            set { _showAffordanceArrow = value; }
        }

        public DynamicPanelViewMode ViewMode
        {
            get { return _viewMode; }
            set { _viewMode = value; }
        }

        public int PanelWidth
        {
            get { return _panelWidth; }
            set { _panelWidth = value; }
        }

        public double LineWith
        {
            get { return _lineWidth; }
            set { _lineWidth = value; }
        }

        internal static bool SupportedStyleProperty(string stylePropertyName)
        {
            return _supportStyleProperties.ContainsKey(stylePropertyName);
        }

        protected override Dictionary<string, StyleProperty> SupportStyleProperties
        {
            get { return _supportStyleProperties; }
        }

        private static Dictionary<string, StyleProperty> _supportStyleProperties = new Dictionary<string, StyleProperty>();

        static DynamicPanel()
        {
            // Add support style properties and default value.
            _supportStyleProperties[StylePropertyNames.OPACITY_PROP] = new StyleIntegerProperty(StylePropertyNames.OPACITY_PROP, 100);
        }

        private List<PanelStatePage> _states = new List<PanelStatePage>();
        private IPanelStatePage _startPanelStatePage;
        private Guid _startPageGuid = Guid.Empty;

        private bool _isCircular = false;
        private bool _isAutomatic = false;
        private int _automaticIntervalTime = 3000;  // Automatic Interval Time, millisecond, default value is 3000ms
        private int _durationTime = 500; // Duration Time, millisecond, default value is 500ms
        private NavigationType _navigationType = NavigationType.Dot;
        private bool _showAffordanceArrow = false;
        private DynamicPanelViewMode _viewMode = DynamicPanelViewMode.Full;
        private int _panelWidth = 100;
        private double _lineWidth = 3; //Default value is 3px.
    }
}
