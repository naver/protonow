using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.WidgetLibrary
{
    class EditListViewModel:ViewModelBase
    {
        public EditListViewModel(List<IWidget> widgets,bool bLoad)
        {
            widgetsList = widgets;
            this.AddCommand = new DelegateCommand<object>(AddExecute);
            this.MoveUpCommand = new DelegateCommand<object>(MoveUpExecute, CanExecuteMoveUp);
            this.MoveDownCommand = new DelegateCommand<object>(MoveDownExecute, CanExecuteMoveDown);
            this.DeleteCommand = new DelegateCommand<object>(DeleteExecute, CanExecuteDelete);
            this.DeleteAllCommand = new DelegateCommand<object>(DeleteAllExecute, CanExecuteDeleteAll);
            this.AddManyCommand = new DelegateCommand<object>(AddManyExecute);
            this.CheckItemCommand = new DelegateCommand<object>(CheckItemExecute);
            this.OKCommand = new DelegateCommand<Window>(OKExecute);
            this.CancelCommand = new DelegateCommand<Window>(CancelExecute);
            NodeList = new ObservableCollection<NodeViewModel>();
            if(bLoad)
                LoadList();          
        }

        #region Commands and functions.
        public DelegateCommand<object> AddCommand { get; private set; }
        public DelegateCommand<object> MoveUpCommand { get; private set; }
        public DelegateCommand<object> MoveDownCommand { get; private set; }
        public DelegateCommand<object> DeleteCommand { get; private set; }
        public DelegateCommand<object> DeleteAllCommand { get; private set; }
        public DelegateCommand<object> AddManyCommand { get; private set; }
        public DelegateCommand<object> CheckItemCommand { get; private set; }
        public DelegateCommand<Window> OKCommand { get; set; }
        public DelegateCommand<Window> CancelCommand { get; set; }

        private void AddExecute(object obj)
        {
            NodeViewModel node = new NodeViewModel(GetItemName());
            NodeList.Add(node);
            SelectValue = node;
        }
        private void MoveUpExecute(object obj)
        {
            NodeViewModel node = selectValue;
            int index = NodeList.IndexOf(selectValue);
            NodeList.Remove(node);
            NodeList.Insert(--index, node);
            SelectValue = node;
        }
        private bool CanExecuteMoveUp(object obj)
        {
            return selectValue != null && NodeList.IndexOf(selectValue) > 0;
        }
        private void MoveDownExecute(object obj)
        {
            NodeViewModel node = selectValue;
            int index = NodeList.IndexOf(node);
            NodeList.Remove(node);
            NodeList.Insert(++index, node);
            SelectValue = node;
        }
        private bool CanExecuteMoveDown(object obj)
        {
            return selectValue != null && NodeList.IndexOf(selectValue) < NodeList.Count - 1;
        }
        private void DeleteExecute(object obj)
        {
            int index = NodeList.IndexOf(selectValue);
            NodeList.Remove(selectValue);
            if (NodeList.Count <= 0)
                return;

            if (index > 0)
            {
                SelectValue = NodeList.ElementAt(--index);
            }
            else
            {
                SelectValue = NodeList.ElementAt(0);
            }
        }
        private bool CanExecuteDelete(object obj)
        {
            return selectValue != null && NodeList.Count >= 1;
        }

        private void DeleteAllExecute(object obj)
        {
            NodeList.Clear();
        }
        private bool CanExecuteDeleteAll(object obj)
        {
            return NodeList.Count >= 1;
        }

        private void AddManyExecute(object obj)
        {
            AddManyWindow win = new AddManyWindow();
            win.Owner = Application.Current.MainWindow;
            bool? bRet = win.ShowDialog();
            if((bool)bRet)
            {
                string itemString = (win.DataContext as AddManyViewModel).StringAdded;
                if (string.IsNullOrEmpty(itemString))
                    return;
                string[] items = itemString.Split(Environment.NewLine.ToCharArray(),StringSplitOptions.RemoveEmptyEntries);

                NodeViewModel newNode = null;
                foreach (string item in items)
                {
                    if (item.Trim() != string.Empty)
                    {
                        newNode = new NodeViewModel(item, false);
                        NodeList.Add(newNode);
                    }
                }
                if (newNode != null)
                    SelectValue = newNode;
            }
        }

        private void CheckItemExecute(object obj)
        {
            NodeViewModel node = obj as NodeViewModel;
            node.IsSelected = true;
            //if no allow multiple-select, uncheck others.
            if (IsMultiple == false)
            {
                foreach (NodeViewModel item in NodeList)
                {
                    if (item != node)
                    {
                        item.IsChecked = false;
                    }
                }
            }
        }
        private void OKExecute(Window win)
        {
            if(win!=null)
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                _document = doc.Document;

                foreach (var widget in widgetsList)
                {
                    IListBase list = widget as IListBase;
                    list.Items.Clear();

                    foreach (NodeViewModel node in NodeList)
                    {
                        IListItem item = list.CreateItem(node.Name);
                        item.IsSelected = node.IsChecked;
                    }
                    if (list.WidgetType == WidgetType.ListBox)
                    {
                        (list as IListBox).AllowMultiple = IsMultiple;
                    }
                }              

                _document.IsDirty = true;
                win.DialogResult = true;
                win.Close();
            }
        }

        private void CancelExecute(Window win)
        {
            if (win != null)
            {
                win.DialogResult = false;
                win.Close();
            }
        }


        /// <summary>
        /// Load listbox items or droplist items from Document.
        /// </summary>
        private void LoadList()
        {
            //IListBox and IDroplist has a same base IListBase
            IListBase baselist = widgetsList.ElementAt(0) as IListBase;

            //only one or same, load first one.

            foreach (IListItem item in baselist.Items)
            {
                NodeViewModel newItem = new NodeViewModel(item.TextValue, item.IsSelected);
                NodeList.Add(newItem);
            }
            //Only IListBox has AllowMultiple property.
            if (baselist.WidgetType == WidgetType.ListBox)
            {
                isMultiple = (baselist as IListBox).AllowMultiple;
            }

        }

        private void RefreshCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            MoveUpCommand.RaiseCanExecuteChanged();
            MoveDownCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// When IsMultiple turns to false, check only the first select item.
        /// </summary>
        private void CheckFirstItem()
        {
            bool bfirst = true;
            foreach (NodeViewModel item in NodeList)
            {
                if (item.IsChecked && bfirst)
                {
                    bfirst = false;
                    continue;
                }
                item.IsChecked = false;
            }
        }
        #endregion

        #region property
        public ObservableCollection<NodeViewModel> NodeList { get; set; }

        public WidgetType WidgetType
        {
            get
            {
                IListBase baselist = widgetsList.ElementAt(0) as IListBase;
                return baselist.WidgetType;
            }
        }
        private List<IWidget> widgetsList { get; set; }

        private NodeViewModel selectValue;
        public NodeViewModel SelectValue
        {
            get
            {
                return selectValue;
            }
            set
            {
                if (selectValue != value )
                {
                    //unselect old item
                    if (selectValue != null && value != null)
                        selectValue.IsEditboxFocus = false;
                    selectValue = value;
                    //select new item
                    if (selectValue != null)
                        selectValue.IsEditboxFocus = true;
                    FirePropertyChanged("SelectValue");
                    RefreshCommands();
                }
            }
        }

        private bool isMultiple;
        public bool IsMultiple
        {
            get
            {
                return isMultiple;
            }
            set
            {
                if(isMultiple!=value)
                {
                    if (value == false)
                    {
                        CheckFirstItem();
                    }
                    isMultiple = value;
                    FirePropertyChanged("IsMultiple");
                }
            }
        }
        #endregion

        #region private member
        private IDocument _document;
        
        private string DefaultItemName
        {
            get { return String.Concat(_itemName, _itemCounter++); }
        }

        private string GetItemName()
        {
            string noteName = DefaultItemName;

            if (NodeList.Any(x => x.Name == noteName))
            {
                return GetItemName();
            }

            return noteName;
        }

        private string _itemName = "List Item";
        private int _itemCounter = 1;
        #endregion
    }
}
