using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class AdaptiveViewSet : XmlElementObject, IAdaptiveViewSet
    {
        internal AdaptiveViewSet(Document document)
            : base("AdaptiveViewSet")
        {
            _document = document;
            _affectAllViews = false;
            _base = new AdaptiveView(this, "Base");
            _views = new AdaptiveViews(this);
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            Clear();

            CheckTagName(element);

            LoadBoolFromChildElementInnerText("AffectAllViews", element, ref _affectAllViews);

            XmlElement baseElement = element[_base.TagName];
            if (baseElement != null)
            {
                _base.LoadDataFromXml(baseElement);
            }

            XmlElement viewsElement = element[_views.TagName];
            if (viewsElement != null)
            {
                _views.LoadDataFromXml(viewsElement);
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement element = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(element);

            SaveStringToChildElement("AffectAllViews", _affectAllViews.ToString(), xmlDoc, element);

            _base.SaveDataToXml(xmlDoc, element);
            _views.SaveDataToXml(xmlDoc, element);
        }

        #endregion

        #region IAdaptiveViewSet

        public IDocument ParentDocument
        {
            get { return _document; }
        }

        public bool AffectAllViews
        {
            get { return _affectAllViews; }
            set { _affectAllViews = value; }
        }

        public IAdaptiveView Base
        {
            get { return _base; }
        }

        public IAdaptiveViews AdaptiveViews
        {
            get { return _views; }
        }

        public IAdaptiveView CreateAdaptiveView(string name, IAdaptiveView parent)
        {
            AdaptiveView parentView = parent as AdaptiveView;
            if (parentView == null)
            {
                throw new ArgumentNullException("parent");
            }

            AdaptiveView view = new AdaptiveView(this, name);
            parentView.AddChildView(view);
            _views.Add(view);

            _document.OnAddAdaptiveView(view);

            return view;
        }

        public void DeleteAdaptiveView(IAdaptiveView view)
        {
            if (view == _base)
            {
                throw new ArgumentException("Cannot delete Base adaptive view!");
            }

            AdaptiveView viewToDelete = view as AdaptiveView;

            if (viewToDelete != null)
            {
                AdaptiveView parent = viewToDelete.ParentView as AdaptiveView;
                if (parent != null)
                {
                    // Remove this view from parent
                    parent.RemoveChildView(viewToDelete);

                    // Add child view to new parent
                    foreach (AdaptiveView childView in viewToDelete.ChildViews)
                    {
                        if (childView != null)
                        {
                            parent.AddChildView(childView);
                        }
                    }
                }

                // Remove this view from views
                _views.Remove(viewToDelete);

                _document.OnDeleteAdaptiveView(viewToDelete);
            }
        }

        public void ChangeParent(IAdaptiveView view, IAdaptiveView newParent)
        {
            if (view == _base)
            {
                throw new ArgumentException("Cannot change parent of Base adaptive view!");
            }

            AdaptiveView viewToChange = view as AdaptiveView;
            AdaptiveView newParentView = newParent as AdaptiveView;
            if (viewToChange == null)
            {
                throw new ArgumentNullException("view");
            }

            if (newParentView == null)
            {
                throw new ArgumentNullException("newParent");
            }

            // Remove view from old parent
            AdaptiveView oldParentView = viewToChange.ParentView as AdaptiveView;
            if (oldParentView != null && oldParentView != newParentView)
            {
                oldParentView.RemoveChildView(viewToChange);
            }

            newParentView.AddChildView(viewToChange);

            _document.OnChangeAdaptiveViewParent(viewToChange, oldParentView);
        }

        public bool MoveAdaptiveView(IAdaptiveView view, int delta)
        {
            if (view == _base)
            {
                throw new ArgumentException("Cannot move base adaptive view!");
            }

            AdaptiveView viewToMove = view as AdaptiveView;
            AdaptiveView parentView = viewToMove.ParentView as AdaptiveView;

            if(parentView != null)
            {
                AdaptiveViews views = parentView.ChildViews as AdaptiveViews;
                if(views != null)
                {
                    return views.MoveItem(viewToMove, delta);
                }
            }

            return false;
        }

        public bool MoveAdaptiveViewTo(IAdaptiveView view, int index)
        {
            if (view == _base)
            {
                throw new ArgumentException("Cannot move base adaptive view!");
            }

            AdaptiveView viewToMove = view as AdaptiveView;
            AdaptiveView parentView = viewToMove.ParentView as AdaptiveView;

            if (parentView != null)
            {
                AdaptiveViews views = parentView.ChildViews as AdaptiveViews;
                if (views != null)
                {
                    return views.MoveItemTo(viewToMove, index);
                }
            }

            return false;
        }

        #endregion

        #region Internal methods

        internal void Clear()
        {
            _base.ClearChildView();
            _views.Clear();
        }

        #endregion 

        #region Private Fields

        private Document _document;
        private bool _affectAllViews;
        private AdaptiveView _base;   // Root view of in this set.
        private AdaptiveViews _views; // Collection for all views in this set.

        #endregion

    }
}
