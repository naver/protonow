using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Service.Document;
using System.Diagnostics;
using System.Collections.Specialized;

namespace Naver.Compass.Service
{
    public class SelectionServiceProvider : ISelectionService
    {
        public SelectionServiceProvider()
        {
            //Selection Data
            _widgtes = new RangeObservableCollection<IWidgetPropertyData>();
            _widgtes.CollectionChanged += WidgtesCollectionChangedHandler;

            //Notify Property
            _bCanSendPageNotify = true;
            _bCanSendWdgNotify = true;

            //Clone data
            _ToCopyList = new List<object>();

            _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
        }

        #region private number and function
        private RangeObservableCollection<IWidgetPropertyData> _widgtes;
        private IPagePropertyData _pages;
        private bool _bCanSendPageNotify;
        private bool _bCanSendWdgNotify;
        #endregion private number and function


        #region interface ISelectionService
        public void RegisterWidget(IWidgetPropertyData wdgVM)
        {
            if (_widgtes.Contains(wdgVM))
            {
                return;
            }
            INotifyPropertyChanged vm = wdgVM as INotifyPropertyChanged;
            vm.PropertyChanged += WdgPropertyChangedHandler;
            
            //_widgtes.Add(wdgVM);
            try
            {
                _widgtes.Add(wdgVM);
            }
            catch (System.Exception ex)
            {
            	 Debug.WriteLine(ex.Message);
            }

        }
        public void RemoveWidget(IWidgetPropertyData wdgVM)
        {
            if (_widgtes.Contains(wdgVM))
            {
                INotifyPropertyChanged vm = wdgVM as INotifyPropertyChanged;
                vm.PropertyChanged -= WdgPropertyChangedHandler;
                _widgtes.Remove(wdgVM);
            }
        }
        public void RegisterWidgets(List<IWidgetPropertyData> ToRegList)
        {
            RemoveAllWidgets();
            foreach (IWidgetPropertyData item in ToRegList)
            {
                INotifyPropertyChanged vm = item as INotifyPropertyChanged;
                vm.PropertyChanged += WdgPropertyChangedHandler;
            }
            try
            {
                _widgtes.AddRange(ToRegList);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public void RemoveAllWidgets()
        {
            foreach (IWidgetPropertyData item in _widgtes)
            {
                INotifyPropertyChanged vm = item as INotifyPropertyChanged;
                vm.PropertyChanged -= WdgPropertyChangedHandler;
            }
            _widgtes.Clear();
        }

        public int WidgetNumber { get { return _widgtes.Count; } }

        public void RegisterPage(IPagePropertyData pageVM, List<IWidgetPropertyData> ToAddWidgetList)
        {
            //register the page
            if (_pages == pageVM)
            {
                return;
            }
            _pages = pageVM;
            INotifyPropertyChanged vm = pageVM as INotifyPropertyChanged;
            vm.PropertyChanged += PagePropertyChangedHandler;

            //register selected widgets if exist
            if (ToAddWidgetList.Count <= 0)
                return;

            foreach (IWidgetPropertyData item in ToAddWidgetList)
            {
                INotifyPropertyChanged wVM = item as INotifyPropertyChanged;
                wVM.PropertyChanged += WdgPropertyChangedHandler;
                //_widgtes.Add(item);
            }
            _widgtes.AddRange(ToAddWidgetList);
            //_widgtes.Concat(ToAddWidgetList);
        }
        public void RemovePage(IPagePropertyData pageVM)
        {
            if (_pages == pageVM)
            {
                _widgtes.Clear();
                INotifyPropertyChanged vm = pageVM as INotifyPropertyChanged;
                vm.PropertyChanged -= PagePropertyChangedHandler;
                _pages = null;
                _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Publish(Guid.Empty);
                return;
            }            
        }
        public IPagePropertyData GetCurrentPage()
        {
            return _pages;
        }

        public void AllowWdgPropertyNotify(bool bIsAllowed)
        {
            _bCanSendWdgNotify = bIsAllowed;
        }
        public void AllowPagePropertyNotify(bool bIsAllowed)
        {
            _bCanSendPageNotify = bIsAllowed;
        }
        public void UpdateSelectionNotify()
        {
            _ListEventAggregator.GetEvent<SelectionChangeEvent>().Publish(null);
        }


        public List<IWidgetPropertyData> GetSelectedWidgets()
        {            
            return _widgtes.ToList<IWidgetPropertyData>();
        }
        public List<Guid> GetSelectedWidgetGUIDs()
        {
            List<Guid> AllGids = new List<Guid>();
            AllGids.Clear();
            foreach(IWidgetPropertyData item in _widgtes)
            {
                AllGids.Add(item.WidgetID);
            }
            return AllGids;
        }
        #endregion interface ISelectionService


        #region Data Trigger Handler
        private void PagePropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {      
            switch (e.PropertyName)
            {
                case "IsSelected":
                    {
                        Type t = sender.GetType();
                        PropertyInfo propertyInfo = t.GetProperty(e.PropertyName);
                        if (propertyInfo == null)
                            return;
                        bool isSelect = (bool)propertyInfo.GetValue(sender, null);
                        if (isSelect == true)
                        {
                            IPagePropertyData page = sender as IPagePropertyData;
                            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Publish(page.PageGID);
                        }
                        else
                        {
                            //_ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Publish(Guid.Empty);
                        }
                        return;
                    }
                case "EditorCanvas":
                    {
                        IPagePropertyData page = sender as IPagePropertyData;
                        _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Publish(page.PageGID);
                        return;
                    }
                case "IsActive":
                    return;
            }           
            if (_bCanSendPageNotify == false)
            {
                return;
            }

            return;
            //TODO:
           // throw new NotImplementedException();
        }
        private void WdgPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsSelected":
                    return;
                case "CanEdit":
                    return;
                case "DoubleClickVisibility":
                    return;
            }

            if (_pages!=null)
            {
                _pages.SetIsThumbnailUpdate(true);
            }

            if (_bCanSendWdgNotify == false)
            {
                return;
            }

            _ListEventAggregator.GetEvent<SelectionPropertyChangeEvent>().Publish(e.PropertyName);

            //if (_bCanSendWdgNotify == true)
            //{
            //    Type t = sender.GetType();
            //    PropertyInfo propertyInfo = t.GetProperty(e.PropertyName);
            //    if (propertyInfo == null)
            //        return;
            //    var temp = propertyInfo.GetValue(sender, null); 
            //}

            return;
            //TODO:
            //throw new NotImplementedException();
        }
        void WidgtesCollectionChangedHandler(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_bCanSendWdgNotify == false)
            {
                return;
            }
            
            _ListEventAggregator.GetEvent<SelectionChangeEvent>().Publish(null);
            //TODO:
           // throw new NotImplementedException();
        }
        #endregion Data Trigger Handler

        #region Copy/Paste  (Codes will be removed later, it's redundancy now)
        private List<object> _ToCopyList;
        private int _copyIndex = 1;
        public List<object> GetCloneCacheData()
        {
            return _ToCopyList;
        }
        public void ClearCloneCacheData()
        {
            _ToCopyList.Clear();
            _copyIndex += 1;
        }
        public  int GetCopyIndex()
        {
            return _copyIndex;
        }
        #endregion

        #region Event Mechanism
        public IEventAggregator _ListEventAggregator;
        #endregion

    }

    //public class RangeObservableCollection<T> : ObservableCollection<T>
    //{
    //    bool isDeferNotify = false;
    //    //public override event NotifyCollectionChangedEventHandler CollectionChanged;

    //    public void AddRange(IEnumerable<T> list)
    //    {
    //       isDeferNotify = true;
    //       //var obj=CollectionChanged;
    //       CollectionChanged = null;

    //        foreach (T item in list)
    //        {
    //            this.Items.Add(item);
    //        }

    //        isDeferNotify = false;    
    //        //CollectionChanged = obj;
    //        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
           
    //    }

    //    public void RemoveRange(IEnumerable<T> rangeData)
    //    {
    //        // isDeferNotify = true;           
    //        var obj=CollectionChanged;
    //        CollectionChanged = null;

    //        foreach (T data in rangeData)
    //        {
    //            Remove(data);
    //        }

    //       // isDeferNotify = false;
    //        CollectionChanged = obj;
    //        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //    }

    //    //protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    //    //{
    //    //    //if use DeferNotify, UI Operation is  not fluent
    //    //    //if (!isDeferNotify)
    //    //    //{
    //    //    base.OnCollectionChanged(e);
    //    //    // }
    //    //}

    //}
}
