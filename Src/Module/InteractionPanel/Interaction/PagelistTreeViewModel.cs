using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;

namespace Naver.Compass.Module
{
    /// <summary>
    /// PagelistTree is used in CaseEditorView and InteractionView(Popup)
    /// </summary>
    class PagelistTreeViewModel:ViewModelBase
    {
        public PagelistTreeViewModel()
        {
            _model = PagelistTreeModel.GetInstance();
            _model.LoadPageTree();
            this.SelectPageCommand = new DelegateCommand<object>(SelectPageExecute);
            searchPageName = String.Empty;
        }

        public DelegateCommand<object> SelectPageCommand { get; set; }
        public ObservableCollection<PageNode> PageList
        {
            get 
            {
                return _model.RootNode.Children; 
            }
        }

        private string searchPageName;
        public string SearchPageName
        {
            get { return searchPageName; }
            set
            {
                if (searchPageName != value)
                {
                    searchPageName = value;
                    FirePropertyChanged("SearchPageName");
                    ApplyFilter();
                }
            }
        }

        private void ApplyFilter()
        {
            foreach (var node in PageList)
            {
                node.ApplyFilter(SearchPageName, new Stack<PageNode>());
            }
        }

        public void SelectPageExecute(object obj)
        {
            PageNode node = obj as PageNode;
            if (node == null)
                return;

            PageInfo page = new PageInfo();
            page.Name = node.Name;
            page.Guid = node.Guid;
            page.IsInPopup = _model.IsInPopup;
            _ListEventAggregator.GetEvent<PageNameSelectedEvent>().Publish(page);
        }

        private PagelistTreeModel _model;
    }
}
