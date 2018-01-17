using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module
{
    class AddMasterViewModel : ViewModelBase
    {
        public AddMasterViewModel(IMasterPage page)
        {
            _masterPage = page;
            _model = PageListModel.GetInstance();
            _model.LoadPageTree();
            _selectedPageList = new List<IPage>();
        }

        private DelegateCommand<object> _okCommand;
        public DelegateCommand<object> OKCommand 
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new DelegateCommand<object>(OKExecute);
                }

                return _okCommand;
            }
        }

        private DelegateCommand<object> _cancelCommand;
        public DelegateCommand<object> CancelCommand 
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new DelegateCommand<object>(CancelExecute);
                }

                return _cancelCommand;
            }
        }

        private IMasterPage _masterPage;

        private bool _isAllPage = true;
        public bool IsAllPage
        {
            get
            {
                return _isAllPage;
            }
            set
            {
                if (_isAllPage != value)
                {
                    _isAllPage = value;
                    FirePropertyChanged("IsAllPage");
                }
            }
        }

        private bool _isLockLocation = true;
        public bool IsLockLocation
        {
            get
            {
                return _isLockLocation;
            }
            set
            {
                if(_isLockLocation!=value)
                {
                    _isLockLocation = value;
                    FirePropertyChanged("IsLockLocation");
                }
            }
        }

        double _left;
        public string Left
        {
            get
            {
                if (double.IsNaN(_left))
                    return null;
                return _left.ToString();
            }
            set
            {
                double newValue;
                Double.TryParse(value, out newValue);
                if (_left != newValue)
                {
                    _left = newValue;
                    FirePropertyChanged("Left");
                }
            }
        }

        double _top;
        public string Top
        {
            get
            {
                if (double.IsNaN(_top))
                    return null;
                return _top.ToString();
            }
            set
            {
                double newValue;
                Double.TryParse(value, out newValue);
                if (_top != newValue)
                {
                    _top = newValue;
                    FirePropertyChanged("Top");
                }
            }
        }

        private bool _isSendBack;
        public bool IsSendBack
        {
            get
            {
                return _isSendBack;
            }
            set
            {
                if (_isSendBack != value)
                {
                    _isSendBack = value;
                    FirePropertyChanged("IsSendBack");
                }
            }
        }

        public ObservableCollection<PageNode> PageList
        {
            get
            {
                return _model.RootNode.Children;
            }
        }

        private List<IPage> _selectedPageList { get; set; }

        private PageListModel _model;

        private void TraveralTree(PageNode rootNode)
        {
            foreach(var item in rootNode.Children.Where(a=>a.IsSelected))
            {
                _selectedPageList.Add(item.TreeNodeObject.AttachedObject);
            }
            foreach (var item in rootNode.Children)
            {
                TraveralTree(item);
            }
        }

        private void OKExecute(object obj)
        {

            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            var baseGuid = doc.Document.AdaptiveViewSet.Base.Guid;

            if(IsAllPage)
            {
                foreach (var page in doc.Document.Pages)
                {
                    AddMasterToPage(page, baseGuid);
                }
                _ListEventAggregator.GetEvent<AddMasterEvent>().Publish(_masterPage);
            }
            else
            {
                TraveralTree(_model.RootNode);
                foreach(var page in _selectedPageList)
                {
                    AddMasterToPage(page, baseGuid);
                }
                if (_selectedPageList.Count > 0)
                {
                    _ListEventAggregator.GetEvent<AddMasterEvent>().Publish(_masterPage);
                }
            }

            Window win = obj as Window;
            win.DialogResult = true;
            win.Close();
        }

        private void CancelExecute(object obj)
        {
            Window win = obj as Window;
            win.DialogResult = false;
            win.Close();
        }

        private void AddMasterToPage(IPage page, Guid baseGuid)
        {
            try
            {
                bool isOpened = page.IsOpened;
                if (!isOpened)
                {
                    page.Open();
                }
                var baseView = page.PageViews.GetPageView(baseGuid);
                IMaster master = baseView.CreateMaster(_masterPage.Guid);
                if (master != null)
                {
                    if (IsLockLocation)
                    {
                        bool isMasterOpen = _masterPage.IsOpened;
                        if(!isMasterOpen)
                        {
                            _masterPage.Open();
                        }
                        //Keep added master the same location as in masterpage views.
                        foreach (var pageview in page.PageViews)
                        {
                            var masterInView = pageview.Masters[master.Guid];
                            if (null == masterInView)
                                break;

                            double initX, initY;
                            var masterPageView = _masterPage.PageViews.GetPageView(pageview.Guid);
                            if(masterPageView.Widgets.Count<=0)
                            {
                                initX = initY = 0;
                            }
                            else
                            {
                                var regionStyle = _masterPage.PageViews.GetPageView(pageview.Guid).RegionStyle;
                                initX = regionStyle.X;
                                initY = regionStyle.Y;
                            }
                            masterInView.GetMasterStyle(pageview.Guid).X = initX;
                            masterInView.GetMasterStyle(pageview.Guid).Y = initY;
                        }
                        if (!isMasterOpen)
                        {
                            _masterPage.Close();
                        }
                        master.IsLocked = true;
                        master.IsLockedToMasterLocation = true;
                    }
                    else
                    {
                        master.MasterStyle.X = _left;
                        master.MasterStyle.Y = _top;
                    }
                    if (_isSendBack)
                    {
                        foreach (var widget in page.Widgets)
                        {
                            widget.WidgetStyle.Z += 1;
                        }
                        foreach (var item in page.Masters)
                        {
                            item.MasterStyle.Z += 1;
                        }
                        master.MasterStyle.Z = 0;
                    }
                    else
                    {
                        master.MasterStyle.Z = page.WidgetsAndMasters.Count;
                    }
                }
                if (!isOpened)
                {
                    page.Close();
                }
            }
            catch(Exception e)
            {
                NLogger.Error("Add Master to pages failed: " + e.Message);
            }
        }
    }
}
