using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class AdaptiveView : XmlElementObject, IAdaptiveView
    {
        internal AdaptiveView(AdaptiveViewSet set, string name)
            : base("AdaptiveView")
        {
            _set = set;
            _name = name;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadElementGuidAttribute(element, ref _guid);

            LoadGuidFromChildElementInnerText("ParentViewGuid", element, ref _parentViewGuid);
            LoadBoolFromChildElementInnerText("IsVisible", element, ref _isVisible);
            LoadBoolFromChildElementInnerText("IsChecked", element, ref _isChecked);
            LoadStringFromChildElementInnerText("Name", element, ref _name);
            LoadEnumFromChildElementInnerText<AdaptiveViewCondition>("Condition", element, ref _condition);
            LoadIntFromChildElementInnerText("Width", element, ref _width);
            LoadIntFromChildElementInnerText("Height", element, ref _height);

            XmlElement viewsElement = element["ChildViewsGuid"];
            if (viewsElement != null && viewsElement.ChildNodes.Count > 0)
            {
                XmlNodeList childList = viewsElement.ChildNodes;
                foreach (XmlElement childElement in childList)
                {
                    try
                    {
                        Guid guid = new Guid(childElement.InnerText);
                        _childViewGuidList.Add(guid);

                    }
                    catch (Exception ex)
                    {
                       Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            SaveElementGuidAttribute(element, _guid);

            if (_parentViewGuid != Guid.Empty)
            {
                SaveStringToChildElement("ParentViewGuid", _parentViewGuid.ToString(), xmlDoc, element);
            }

            SaveStringToChildElement("IsVisible", _isVisible.ToString(), xmlDoc, element);
            SaveStringToChildElement("IsChecked", _isChecked.ToString(), xmlDoc, element);
            SaveStringToChildElement("Name", _name, xmlDoc, element);
            SaveStringToChildElement("Condition", _condition.ToString(), xmlDoc, element);
            SaveStringToChildElement("Width", _width.ToString(), xmlDoc, element);
            SaveStringToChildElement("Height", _height.ToString(), xmlDoc, element);

            XmlElement viewsElement = xmlDoc.CreateElement("ChildViewsGuid");
            element.AppendChild(viewsElement);

            List<Guid> copyList = new List<Guid>(_childViewGuidList);
            foreach (Guid guid in copyList)
            {
                SaveStringToChildElement("Guid", guid.ToString(), xmlDoc, viewsElement);
            }
        }

        #endregion

        #region IUniqueObject, INamedObject

        public Guid Guid
        {
            get { return _guid; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region IAdaptiveView

        public IAdaptiveViewSet AdaptiveViewSet
        {
            get { return _set; }
        }
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }
        
        public string Description
        {
            get
            {
                switch(_condition)
                {
                    case AdaptiveViewCondition.LessOrEqual:
                        {
                            return String.Format(@"{0}({1} x {2} and below)", _name,
                                                 _width == 0 ? "any" : _width.ToString(),
                                                 _height == 0 ? "any" : _height.ToString());
                        }
                    case AdaptiveViewCondition.GreaterOrEqual:
                        {
                            return String.Format(@"{0}({1} x {2} and above)", _name,
                                                 _width == 0 ? "any" : _width.ToString(),
                                                 _height == 0 ? "any" : _height.ToString());
                        }
                    default:
                        return string.Empty;
                }
            }
        }

        public AdaptiveViewCondition Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public IAdaptiveView ParentView
        {
            get
            {
                if (_parentView == null)
                {
                    if (_set.Base.Guid == _parentViewGuid)
                    {
                        _parentView = _set.Base as AdaptiveView;
                    }
                    else
                    {
                        _parentView = _set.AdaptiveViews[_parentViewGuid] as AdaptiveView;
                    }
                }

                return _parentView;
            }
            set
            {
                _parentView = value as AdaptiveView;
                if (_parentView != null)
                {
                    _parentViewGuid = _parentView.Guid;
                }
                else
                {
                    _parentViewGuid = Guid.Empty;
                }
            }
        }

        public IAdaptiveViews ChildViews
        {
            get
            {
                if (_childViews == null)
                {
                    _childViews = new AdaptiveViews(_set);
                    foreach (Guid guid in _childViewGuidList)
                    {
                        AdaptiveView view = _set.AdaptiveViews[guid] as AdaptiveView;
                        if (view != null)
                        {
                            _childViews.Add(view);
                        }
                    }
                }

                return _childViews;
            }
        }

        #endregion

        #region Internal Methods

        internal void AddChildView(AdaptiveView view)
        {
            if (view != null)
            {
                /*
                 * AdpativeView object only can be created via IApdativeViewSet and
                 * the new created object is already in AdaptiveViewSet, so we don't need 
                 * keeping the input object, we just keep the guid and can get the object
                 * from set with this guid when we need the object.
                 * */
                _childViewGuidList.Add(view.Guid);
                view.ParentView = this;

                if (_childViews != null)
                {
                    _childViews.Add(view);
                }
            }
        }

        internal void RemoveChildView(AdaptiveView view)
        {
            if (view != null)
            {
                _childViewGuidList.Remove(view.Guid);
                if(_childViews != null)
                {
                    _childViews.Remove(view);
                }
            }
        }

        internal void ClearChildView()
        {
            _childViewGuidList.Clear();
            if (_childViews != null)
            {
                _childViews.Clear();

                // Make object list to null and can be retrieved from guid list again when needed.
                _childViews = null; 
            }
        }

        #endregion

        #region Private Fields

        private AdaptiveViewSet _set;

        private Guid _guid = Guid.NewGuid();
        private string _name;
        
        private bool _isVisible = true;
        private bool _isChecked = false;
        private AdaptiveViewCondition _condition = AdaptiveViewCondition.LessOrEqual;
        private int _width;
        private int _height;
        
        private Guid _parentViewGuid;
        private AdaptiveView _parentView;
        
        private List<Guid> _childViewGuidList = new List<Guid>();
        private AdaptiveViews _childViews;

        #endregion
    }
}
