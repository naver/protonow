using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class ShowHideActionTarget : XmlElementObject, IShowHideActionTarget
    {
        public ShowHideActionTarget(Guid targetGuid)
            : base("ShowHideActionTarget")
        {
            _guid = targetGuid;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadElementGuidAttribute(element, ref _guid);

            LoadEnumFromChildElementInnerText<VisibilityType>("VisibilityType", element, ref _visibilityType);
            LoadEnumFromChildElementInnerText<ShowHideAnimateType>("AnimateType", element, ref _animateType);
            LoadIntFromChildElementInnerText("AnimateTime", element, ref _animateTime);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement targetElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(targetElement);

            SaveElementGuidAttribute(targetElement, _guid);

            SaveStringToChildElement("VisibilityType", _visibilityType.ToString(), xmlDoc, targetElement);
            SaveStringToChildElement("AnimateType", _animateType.ToString(), xmlDoc, targetElement);
            SaveStringToChildElement("AnimateTime", _animateTime.ToString(), xmlDoc, targetElement);
        }

        #endregion

        public Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        public VisibilityType VisibilityType
        {
            get { return _visibilityType; }
            set { _visibilityType = value; }
        }

        public ShowHideAnimateType AnimateType
        {
            get { return _animateType; }
            set { _animateType = value; }
        }

        public int AnimateTime
        {
            get { return _animateTime; }
            set { _animateTime = value; }
        }

        internal ShowHideActionTarget Clone()
        {
            ShowHideActionTarget newTarget = new ShowHideActionTarget(_guid);
            newTarget._visibilityType = _visibilityType;
            newTarget._animateType = _animateType;
            newTarget._animateTime = _animateTime;
            return newTarget;
        }

        private Guid _guid;
        private VisibilityType _visibilityType = VisibilityType.Show;
        private ShowHideAnimateType _animateType = ShowHideAnimateType.None;
        private int _animateTime = 500; // 500ms default value.
    }

    internal class InteractionShowHideAction: InteractionAction, IInteractionShowHideAction
    {
        public InteractionShowHideAction(IInteractionCase interactionCase)
            : base(interactionCase)
        {
            _actionType = ActionType.ShowHideAction;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            base.LoadDataFromXml(element);

            XmlElement tagetObjectsElement = element["TargetObjects"];
            if (tagetObjectsElement != null && tagetObjectsElement.ChildNodes.Count > 0)
            {
                XmlNodeList childList = tagetObjectsElement.ChildNodes;
                foreach (XmlElement childElement in childList)
                {
                    try
                    {
                        ShowHideActionTarget target = new ShowHideActionTarget(Guid.Empty);
                        target.LoadDataFromXml(childElement);
                        _targetObjects.Add(target.Guid, target);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message.ToString());
                        continue;
                    }
                }
            }
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            if (_targetObjects.Count <= 0)
            {
                return;
            }

            XmlElement actionElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(actionElement);

            base.SaveDataToXml(xmlDoc, actionElement);

            XmlElement tagetObjectsElement = xmlDoc.CreateElement("TargetObjects");
            actionElement.AppendChild(tagetObjectsElement);
            foreach (ShowHideActionTarget target in _targetObjects.Values)
            {
                target.SaveDataToXml(xmlDoc, tagetObjectsElement);
            }
        }

        #endregion

        #region IIteractionShowHideAction

        public IShowHideActionTarget AddTargetObject(Guid widgetGuid)
        {
            ShowHideActionTarget target = new ShowHideActionTarget(widgetGuid);
            _targetObjects[widgetGuid] = target;
            return target;
        }

        public bool DeleteTagetObject(Guid widgetGuid)
        {
            return _targetObjects.Remove(widgetGuid);
        }

        public void SetAllVisibilityType(VisibilityType type)
        {
            foreach (ShowHideActionTarget target in _targetObjects.Values)
            {
                target.VisibilityType = type;
            }
        }

        public void SetAllAnimateType(ShowHideAnimateType type)
        {
            foreach (ShowHideActionTarget target in _targetObjects.Values)
            {
                target.AnimateType = type;
            }
        }

        public void SetAllAnimateTime(int time)
        {
            foreach (ShowHideActionTarget target in _targetObjects.Values)
            {
                target.AnimateTime = time;
            }
        }

        public IShowHideActionTarget GetTarget(Guid widgetGuid)
        {
            if (_targetObjects.ContainsKey(widgetGuid))
            {
                return _targetObjects[widgetGuid];
            }
            else
            {
                return null;
            }
        }

        public IShowHideActionTarget this[Guid widgetGuid]
        {
            get { return GetTarget(widgetGuid); }
        }

        public List<IShowHideActionTarget> TargetObjects
        {
            get { return new List<IShowHideActionTarget>(_targetObjects.Values); }
        }

        public void ClearAllTargetObjects()
        {
            _targetObjects.Clear();
        }

        #endregion

        internal bool ContainsTarget(Guid targetGuid)
        {
            return _targetObjects.ContainsKey(targetGuid);
        }

        internal override void Update()
        {
            if (_targetObjects.Count <= 0)
            {
                return;
            }
            
            if (InteractionCase != null
                && InteractionCase.InteractionEvent != null
                && InteractionCase.InteractionEvent.InteractiveObject != null)
            {
                IPage contextPage = InteractionCase.InteractionEvent.InteractiveObject.ContextPage;
                if (contextPage != null)
                {
                    Dictionary<Guid, ShowHideActionTarget> newTargetObjects = new Dictionary<Guid, ShowHideActionTarget>();
                    foreach (ShowHideActionTarget target in _targetObjects.Values)
                    {                        
                        if (contextPage.Widgets.Contains(target.Guid) || contextPage.Groups.Contains(target.Guid))
                        {
                            // The target guid is still in this page.
                            newTargetObjects.Add(target.Guid, target);
                        }
                        else
                        {
                            // The target guid is not in this page, check with widgets originalGuid
                            Widget widget = contextPage.Widgets.OfType<Widget>().FirstOrDefault<Widget>(x => x.OriginalGuid == target.Guid);
                            if (widget != null)
                            {
                                target.Guid = widget.Guid;
                                newTargetObjects.Add(target.Guid, target);
                            }
                            else
                            {
                                // Check if the target is group
                                Group group = contextPage.Groups.OfType<Group>().FirstOrDefault<Group>(x => x.OriginalGuid == target.Guid);
                                if (group != null)
                                {
                                    target.Guid = group.Guid;
                                    newTargetObjects.Add(target.Guid, target);
                                }                                
                            }
                        }
                    }

                    _targetObjects = newTargetObjects;
                }
            }
        }

        internal override void Update(Dictionary<Guid, IObjectContainer> newTargets)
        {
            Dictionary<Guid, ShowHideActionTarget> newTargetObjects = new Dictionary<Guid, ShowHideActionTarget>();
            foreach (ShowHideActionTarget target in _targetObjects.Values)
            {
                if (newTargets.ContainsKey(target.Guid))
                {
                    IObjectContainer objects = newTargets[target.Guid];

                    // Add top group as the target first.
                    foreach(Group group in objects.GroupList)
                    {                        
                        if (group.ParentGroupGuid == Guid.Empty)
                        {
                            ShowHideActionTarget newTarget = target.Clone();
                            newTarget.Guid = group.Guid;
                            newTargetObjects.Add(newTarget.Guid, newTarget);
                        }
                    }

                    // Then add the widgets which are not in a group.
                    foreach(Widget widget in objects.WidgetList)
                    {
                        if(widget.ParentGroupGuid == Guid.Empty)
                        {
                            ShowHideActionTarget newTarget = target.Clone();
                            newTarget.Guid = widget.Guid;
                            newTargetObjects.Add(newTarget.Guid, newTarget);
                        }
                    }
                }
                else
                {
                    newTargetObjects.Add(target.Guid, target);
                }
            }

            _targetObjects = newTargetObjects;
        }

        private Dictionary<Guid, ShowHideActionTarget> _targetObjects = new Dictionary<Guid, ShowHideActionTarget>();
    }
}
