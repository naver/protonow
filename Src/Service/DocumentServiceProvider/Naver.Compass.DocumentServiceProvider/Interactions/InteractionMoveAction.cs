using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class MoveActionTarget : XmlElementObject, IMoveActionTarget
    {
        public MoveActionTarget(Guid targetGuid)
            : base("MoveActionTarget")
        {
            _guid = targetGuid;
        }

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            LoadElementGuidAttribute(element, ref _guid);

            LoadDoubleFromChildElementInnerText("X", element, ref _x);
            LoadDoubleFromChildElementInnerText("Y", element, ref _y);
            LoadEnumFromChildElementInnerText<MoveType>("MoveType", element, ref _moveType);
            LoadEnumFromChildElementInnerText<MoveAnimateType>("AnimateType", element, ref _animateType);
            LoadIntFromChildElementInnerText("AnimateTime", element, ref _animateTime);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement targetElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(targetElement);

            SaveElementGuidAttribute(targetElement, _guid);

            SaveStringToChildElement("X", _x.ToString(), xmlDoc, targetElement);
            SaveStringToChildElement("Y", _y.ToString(), xmlDoc, targetElement);
            SaveStringToChildElement("MoveType", _moveType.ToString(), xmlDoc, targetElement);
            SaveStringToChildElement("AnimateType", _animateType.ToString(), xmlDoc, targetElement);
            SaveStringToChildElement("AnimateTime", _animateTime.ToString(), xmlDoc, targetElement);
        }

        #endregion

        public Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        public MoveType MoveType
        {
            get { return _moveType; }
            set { _moveType = value; }
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public MoveAnimateType AnimateType
        {
            get { return _animateType; }
            set { _animateType = value; }
        }

        public int AnimateTime
        {
            get { return _animateTime; }
            set { _animateTime = value; }
        }

        internal MoveActionTarget Clone()
        {
            MoveActionTarget newTarget = new MoveActionTarget(_guid);
            newTarget._moveType = _moveType;
            newTarget._x = _x;
            newTarget._y = _y;
            newTarget._animateType = _animateType;
            newTarget._animateTime = _animateTime;
            return newTarget;
        }

        private Guid _guid;
        private MoveType _moveType = MoveType.By;
        private double _x = 0d;
        private double _y = 0d;
        private MoveAnimateType _animateType = MoveAnimateType.None;
        private int _animateTime = 0;
    }

    internal class InteractionMoveAction: InteractionAction, IInteractionMoveAction
    {
        public InteractionMoveAction(IInteractionCase interactionCase)
            : base(interactionCase)
        {
            _actionType = ActionType.MoveAction;
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
                        MoveActionTarget target = new MoveActionTarget(Guid.Empty);
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
            foreach (MoveActionTarget target in _targetObjects.Values)
            {
                target.SaveDataToXml(xmlDoc, tagetObjectsElement);
            }
        }

        #endregion

        #region IIteractionMoveAction

        public IMoveActionTarget AddTargetObject(Guid widgetGuid)
        {
            MoveActionTarget target = new MoveActionTarget(widgetGuid);
            _targetObjects[widgetGuid] = target;
            return target;
        }

        public bool DeleteTagetObject(Guid widgetGuid)
        {
            return _targetObjects.Remove(widgetGuid);
        }

        public void SetAllMoveType(MoveType type)
        {
            foreach (MoveActionTarget target in _targetObjects.Values)
            {
                target.MoveType = type;
            }
        }

        public void SetAllX(double x)
        {
            foreach (MoveActionTarget target in _targetObjects.Values)
            {
                target.X = x;
            }
        }

        public void SetAllY(double y)
        {
            foreach (MoveActionTarget target in _targetObjects.Values)
            {
                target.Y = y;
            }
        }

        public void SetAllAnimateType(MoveAnimateType type)
        {
            foreach (MoveActionTarget target in _targetObjects.Values)
            {
                target.AnimateType = type;
            }
        }

        public void SetAllAnimateTime(int time)
        {
            foreach (MoveActionTarget target in _targetObjects.Values)
            {
                target.AnimateTime = time;
            }
        }

        public IMoveActionTarget GetTarget(Guid widgetGuid)
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

        public IMoveActionTarget this[Guid widgetGuid]
        {
            get
            {
                return GetTarget(widgetGuid);
            }
        }

        public List<IMoveActionTarget> TargetObjects
        {
            get 
            {
                return new List<IMoveActionTarget>(_targetObjects.Values); 
            }
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
                    Dictionary<Guid, MoveActionTarget> newTargetObjects = new Dictionary<Guid, MoveActionTarget>();
                    foreach (MoveActionTarget target in _targetObjects.Values)
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
            Dictionary<Guid, MoveActionTarget> newTargetObjects = new Dictionary<Guid, MoveActionTarget>();
            foreach (MoveActionTarget target in _targetObjects.Values)
            {
                if (newTargets.ContainsKey(target.Guid))
                {
                    IObjectContainer objects = newTargets[target.Guid];

                    // Add top group as the target first.
                    foreach(Group group in objects.GroupList)
                    {                        
                        if (group.ParentGroupGuid == Guid.Empty)
                        {
                            MoveActionTarget newTarget = target.Clone();
                            newTarget.Guid = group.Guid;
                            newTargetObjects.Add(target.Guid, target);
                        }
                    }

                    // Then add the widgets which are not in a group.
                    foreach(Widget widget in objects.WidgetList)
                    {
                        if(widget.ParentGroupGuid != Guid.Empty)
                        {
                            MoveActionTarget newTarget = target.Clone();
                            newTarget.Guid = widget.Guid;
                            newTargetObjects.Add(target.Guid, target);
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

        private Dictionary<Guid, MoveActionTarget> _targetObjects = new Dictionary<Guid, MoveActionTarget>();
    }
}
