using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Naver.Compass.Common.CommonBase
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        public RangeObservableCollection()
            : base()
        {
            _suspendCollectionChangeNotification = false;
        }


        bool _suspendCollectionChangeNotification;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suspendCollectionChangeNotification)
            {
                base.OnCollectionChanged(e);
            }
        }

        public void SuspendCollectionChangeNotification()
        {
            _suspendCollectionChangeNotification = true;
        }

        public void ResumeCollectionChangeNotification()
        {
            _suspendCollectionChangeNotification = false;
        }


        public void AddRange(IEnumerable<T> items)
        {
            this.SuspendCollectionChangeNotification();
            int index = base.Count;
            try
            {
                foreach (var i in items)
                {
                    base.InsertItem(base.Count, i);
                }
            }
            finally
            {
                this.ResumeCollectionChangeNotification();
                var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                this.OnCollectionChanged(arg);
            }
        }
        public void RemoveRange(IEnumerable<T> items)
        {
            this.SuspendCollectionChangeNotification();
            try
            {
                foreach (T item in items)
                {
                    this.Items.Remove(item);
                }
            }
            finally
            {
                this.ResumeCollectionChangeNotification();
                var arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                this.OnCollectionChanged(arg);
            }
        }

    }
}
