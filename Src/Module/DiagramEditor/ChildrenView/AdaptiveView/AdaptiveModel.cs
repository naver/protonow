using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Events;
using Naver.Compass.Common.CommonBase;
using System.Collections.ObjectModel;

namespace Naver.Compass.Module
{
    /// <summary>
    /// Keep adaptive panel state and checked veiw in every page.
    /// </summary>
    public class AdaptiveModel
    {
        private static AdaptiveModel _instance;
        public AdaptiveModel()
        {
            PageAdaptiveList = new Dictionary<Guid, AdaptivVieweNode>();
            //ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<DomLoadedEvent>().Subscribe(DomLoadedEventHandler);
            IsPagePropOpen = true;
            IsAdaptiveOpen = false;
            AdaptiveViewsList = new ObservableCollection<AdaptivVieweNode>();
        }

        //public static AdaptiveModel GetInstance()
        //{
        //    if (_instance == null)
        //    {
        //        _instance = new AdaptiveModel();
        //    }
        //    return _instance;
        //}

        /// <summary>
        /// Clear data when close document.
        /// </summary>
        /// <param name="loadType"></param>
        public void DomLoadedEventHandler(FileOperationType loadType)
        {
            switch (loadType)
            {
                case FileOperationType.Close:
                    PageAdaptiveList.Clear();
                    IsAdaptiveOpen = false;
                    break;
            }
        }

        //Guid:page guid
        //AdaptiveNode:current checked adaptive view
        public Dictionary<Guid, AdaptivVieweNode> PageAdaptiveList { get; set; }
        public ObservableCollection<AdaptivVieweNode> AdaptiveViewsList { get; set; }


        //all page share one state
        public bool IsAdaptiveOpen { get; set; }
        public bool IsPagePropOpen { get; set; }
    }
}
