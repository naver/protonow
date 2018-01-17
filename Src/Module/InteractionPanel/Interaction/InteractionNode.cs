using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;
using System.Collections.ObjectModel;
using Naver.Compass.InfoStructure;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;
using System.ComponentModel;
using System.Diagnostics;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Module
{

    #region event node class
    public class EventNode
    {
        public EventNode(IInteractionEvent eventitem)
        {
            IsExpanded = true;
            InteractionEvent = eventitem;
            CaseCollection = new ObservableCollection<CaseNode>();
            foreach(IInteractionCase item in eventitem.Cases)
            {
                CaseNode node = new CaseNode(item);
                node.Parent = this;
                CaseCollection.Add(node);
            }
        }
        public string EventName
        {
            get 
            {
                return InteractionEvent.Name; 
            }
        }

        public bool IsExpanded { set; get; }

        public IInteractionEvent InteractionEvent { get; set; }

        public string EventImage { get; set; }

        public ObservableCollection<CaseNode> CaseCollection { get; set; }

        //get next default case name when create case.
        public string GetNextCaseName()
        {
            string PageName = "Case ";
            int PageCounter = 1;
            string caseName;
            do
            {
                caseName = String.Concat(PageName, PageCounter++);
            } while (CaseCollection.FirstOrDefault(x => x.CaseName == caseName) != null);

            return caseName;
        }
    }
    #endregion

    #region case node class
    public class CaseNode
    {
        
        public CaseNode(IInteractionCase caseItem)
        {
            IsExpanded = true;

            InteractionCase = caseItem;
            ActionCollection = new ObservableCollection<ActionNode>();

            foreach(IInteractionAction item in caseItem.Actions)
            {
                ActionNode node = new ActionNode(item);
                node.Parent = this;
                ActionCollection.Add(node);
            }
        }

        public IInteractionCase InteractionCase { get; set; }
        public object Parent { get; set; }

        public Guid guid
        {
            get { return InteractionCase.Guid; }
        }

        public string CaseName
        {
            get
            {
                return InteractionCase.Name;
            }
        }

        public bool IsExpanded { set; get; }

        public string CaseImage { get; set; }

        public ObservableCollection<ActionNode> ActionCollection { get; set; }

    }
    #endregion

    #region action node
    /// <summary>
    /// Define node of OrganizeTree in CaseEditor
    /// the four memeber are part of node string
    /// eg: Operate Object In Container ---Open Link in CurrentWindow
    /// </summary>
    public class ActionNode :ViewModelBase
    {
        private IInteractionAction interactionAction;

        public Guid Guid { get; set; }

        //Just for temporarily keeping guid in CaseEditor window
        public Guid PageGuid { get; set; }

        public string PageName 
        {
            get { return _document.Pages.GetPage(PageGuid).Name; }
        }

        public ActionNode()
        {

        }
        public ActionNode(IInteractionAction action)
        {
            interactionAction = action;
            Guid = action.Guid;
            switch(action.ActionType)
            {
                case ActionType.OpenAction:
                    IInteractionOpenAction openAction = action as IInteractionOpenAction;
                    OperateType = CommonData.ActionOpen;
                    if (openAction.LinkPageGuid==Guid.Empty)
                    {
                        pageOrLink = openAction.ExternalUrl;
                    }
                    else
                    {
                        IPage page = _document.Pages.GetPage(openAction.LinkPageGuid);
                        pageOrLink = page.Name;
                        PageGuid = openAction.LinkPageGuid;
                    }
                    IN = CommonData.IN;
                    if (openAction.OpenIn == ActionOpenIn.CurrentWindow)
                        Container = CommonData.CurrentWindow;
                    else if (openAction.OpenIn == ActionOpenIn.NewWindowOrTab)
                        Container = CommonData.NewWindowTab;
                    break;
                case ActionType.CloseAction:
                    OperateType = CommonData.ActionClose + " " + CommonData.CurrentWindow;
                    break;
            }
        }
        public object Parent { get; set; }

        public string OperateType { get; set; }
      //  public Guid LinkPageGuid { get; set; }

        private string pageOrLink;
        public string PageOrLink
        {
            get { return pageOrLink; }
            set
            {
                if (pageOrLink != value)
                {
                    pageOrLink = value;
                    FirePropertyChanged("PageOrLink");
                }
            }
        }
        public string IN { get; set; }

        private string container;
        public string Container
        {
            get { return container; }
            set
            {
                if (container != value)
                {
                    container = value;
                    FirePropertyChanged("Container");
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if(isSelected!=value)
                {
                    isSelected = value;
                    FirePropertyChanged("IsSelected");
                } 
            }
        }

        public bool IsExpanded { get; set; }

        IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }
    }
    #endregion

    public class WidgetNode : INotifyPropertyChanged
    {
        internal WidgetNode(InteractionTabVM tabVM, IUniqueObject target,ObjectType type, bool isSelected)
        {
            _targetObject = target as IRegion;
            _tabVM = tabVM;
            _objectType = type;
            _isSelected = isSelected;
        }

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void FirePropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion // INotifyPropertyChanged Members

        #region Data
        //IWidget or IGroup
        private IRegion _targetObject;
        private bool _isSelected;
        private InteractionTabVM _tabVM;
        private ObjectType _objectType;

        public string Name
        {
            get
            {
                string name = string.Empty;
                if(_targetObject !=null)
                {
                     name =_targetObject.Name;
                }

                // Use a double underscore to replace single underscore since in WPF, the _ is the mnemonic character, 
                // and Checkbox support _ mnemonic characters by default.
                if (String.IsNullOrEmpty(name) == false)
                {
                    name = name.Replace("_", "__");
                }

                return string.Format("{0}({1})", name, Type); 
            }
        }
        private string Type
        {
            get
            {

                return ConvertObject.GetTypeName(_objectType);
            }
        }
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if(_isSelected!=value)
                {
                    _isSelected = value;
                    if(value)
                    {
                        _tabVM.AddTarget(_targetObject);
                    }
                    else
                    {
                        _tabVM.DeleteTaget(_targetObject);
                    }
                }
                FirePropertyChanged("IsSelected");
            }
        }
        public IUniqueObject TargetObject
        {
            get { return _targetObject; }
        }
        #endregion
    }
}
