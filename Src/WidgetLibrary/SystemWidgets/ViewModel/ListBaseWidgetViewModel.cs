using Microsoft.Practices.Prism.Commands;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Naver.Compass.WidgetLibrary
{
    public class ListBaseWidgetViewModel : WidgetViewModBase
    {
        public ListBaseWidgetViewModel(IWidget widget)
        {
            _model = new WidgetModel(widget);
           _bSupportBorder = false;
           _bSupportBackground = true;
           _bSupportText = true;
           _bSupportTextVerAlign = false;
           _bSupportTextHorAlign = false;

            widgetGID = widget.Guid;

            ItemsList = new ObservableCollection<NodeViewModel>();
            LoadList();
            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = false;
            _bSupportTextRotate = false;
        }

        private DelegateCommand<object> _doubleClickCommand = null;
        override public ICommand DoubleClickCommand
        {
            get
            {
                if (_doubleClickCommand == null)
                {
                    _doubleClickCommand = new DelegateCommand<object>(OnDoubleClick);
                }
                return _doubleClickCommand;
            }
        }
        private void OnDoubleClick(object obj)
        {
            EditListItems();
        }

        #region Public members

        public ObservableCollection<NodeViewModel> ItemsList { get; set; }
        public IListBase IWidget
        {
            get { return _model.WdgDom as IListBase; }
        }
        public List<IListItem> OldItems { get; set; }


        public string SelectedItem
        {
            get
            {
                NodeViewModel selectNode = ItemsList.FirstOrDefault(x => x.IsChecked == true);
                if (selectNode != null)
                    return selectNode.Name;
                if (ItemsList.Count > 0)
                    return ItemsList.ElementAt(0).Name;
                return null;
            }
        }

        /// <summary>
        /// Before edit list, store old items first.
        /// </summary>
        public void StoreOldItems()
        {
            OldItems = new List<IListItem>(IWidget.Items);
        }
        public void ClearOldItems()
        {
            if (OldItems != null)
            {
                OldItems.Clear();
            }
        }

        public void EditListItems()
        {
            List<IWidget> list = new List<IWidget>();
            list.Add(IWidget);
            StoreOldItems();

            EditListWindow win = new EditListWindow(list);
            win.Owner = Application.Current.MainWindow;

            bool? bRValue = win.ShowDialog();
            if ((bool)bRValue)
            {
                LoadList();
                EditListCommand cmd = new EditListCommand(this, OldItems, IWidget.Items);
                CurrentUndoManager.Push(cmd);
                ClearOldItems();
            }
        }

        public void LoadList()
        {
            ItemsList.Clear();
            foreach (IListItem item in IWidget.Items)
            {
                ItemsList.Add(new NodeViewModel(item.TextValue, item.IsSelected));
            }

            FirePropertyChanged("SelectedItem");
        }
        #endregion

        #region Binding line Property
        public bool IsDisabled
        {
            get
            {
                return _model.IsDisabled;
            }
            set
            {
                if (_model.IsDisabled != value)
                {
                    _model.IsDisabled = value;
                    FirePropertyChanged("IsDisabled");
                }
            }
        }
        #endregion Binding line Property

    }
}

