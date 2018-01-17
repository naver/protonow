using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using System.Windows;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service;

namespace Naver.Compass.InfoStructure
{
    /// <summary>
    /// Base class for all ViewModel classes in the application.
    /// It provides support for property change notifications 
    /// and has a DisplayName property.  This class is abstract.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region IsInDesginModel--just for desgin
        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        private static bool? _isInDesignMode;
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    _isInDesignMode= (bool)DependencyPropertyDescriptor
                        .FromProperty(prop, typeof(FrameworkElement))
                        .Metadata.DefaultValue;
                }
                return _isInDesignMode.Value;
            }
        }
        public bool IsInDesignMode
        {
            get
            {
                return IsInDesignModeStatic;
            }
        }
        #endregion

        #region DisplayName

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public virtual string DisplayName { get; protected set; }

        #endregion // DisplayName

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

        #region IDisposable Members
        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }


        #endregion // IDisposable Members

        #region Event Mechanism

        private IEventAggregator _event;
        public IEventAggregator _ListEventAggregator
        {
            get
            {
                if (_event == null)
                {
                    _event = ServiceLocator.Current.GetInstance<IEventAggregator>();
                }

                return _event;
            }
        }

        #endregion

        #region Protected

        private ISelectionService _selectionService;
        protected ISelectionService SelectionService
        {
            get
            {
                if(_selectionService ==null)
                {
                    _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
                }
                return _selectionService;
            }
        }

        protected UndoManager CurrentUndoManager
        {
            get
            {
                if (SelectionService != null)
                {
                    ISupportUndo pageVM = SelectionService.GetCurrentPage() as ISupportUndo;
                    if (pageVM != null)
                    {
                        return pageVM.UndoManager;
                    }
                }

                return null;
            }
        }

        #endregion
    }

}
