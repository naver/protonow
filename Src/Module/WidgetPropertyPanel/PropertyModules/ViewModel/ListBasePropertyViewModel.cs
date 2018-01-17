
using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Module.Property
{
    class ListBasePropertyViewModel : PropertyViewModelBase
    {

        public ListBasePropertyViewModel()
        {
            this.EditListCommand = new DelegateCommand<object>(EditListExecute);
        }        
        
        #region Override function
        override protected void OnItemsAdd()
        {
            base.OnItemsAdd();
            CheckItemsSame();
            FirePropertyChanged("IsDisabled");
        }

        override public void OnPropertyChanged(string args)
        {
            base.OnPropertyChanged(args);
            switch (args)
            {
                case "IsDisabled":
                    {
                        FirePropertyChanged(args);
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region Binding line Property
        public bool? IsDisabled
        {
            get
            {
                IEnumerable<ListBaseWidgetViewModel> AllCheckBoxs = _VMItems.OfType<ListBaseWidgetViewModel>();
                if (AllCheckBoxs.Count() < 1)
                {
                    return null;
                }

                bool res = AllCheckBoxs.First().IsDisabled;
                foreach (ListBaseWidgetViewModel item in AllCheckBoxs)
                {
                    if (res != item.IsDisabled)
                    {
                        return null;
                    }
                }
                return res;

            }
            set
            {
                List<object> param = new List<object>();
                if (value == null)
                {
                    param.Insert(0, false);
                }
                else
                {
                    param.Insert(0, value);
                }

                param.Insert(1, _VMItems);
                WidgetPropertyCommands.Enable.Execute(param, CmdTarget);
                FirePropertyChanged("IsDisabled");
            }
        }
        #endregion

        #region command and property

        public String ItemsList { get; set; }
        private bool _IsItemsSame { get; set; }
        public DelegateCommand<object> EditListCommand { get; private set; }
        private void EditListExecute(object obj)
        {
            List<IWidget> list = new List<IWidget>();
            foreach (var item in _VMItems)
            {
                //Fore Undo
                ListBaseWidgetViewModel listVM = item as ListBaseWidgetViewModel;
                listVM.StoreOldItems();
                
                list.Add(listVM.IWidget);
            }
            if (list.Count <= 0)
                return;

            EditListWindow win = new EditListWindow(list,_IsItemsSame);
            win.Owner = Application.Current.MainWindow;

            bool? bRValue = win.ShowDialog();
            if ((bool)bRValue)
            {
                Naver.Compass.InfoStructure.CompositeCommand cmds = new Naver.Compass.InfoStructure.CompositeCommand();
                foreach (var item in _VMItems)
                {
                    ListBaseWidgetViewModel listVM = item as ListBaseWidgetViewModel;
                    listVM.LoadList();

                    //Undo
                    EditListCommand cmd = new EditListCommand(listVM, listVM.OldItems, listVM.IWidget.Items);
                    cmds.AddCommand(cmd);
                    listVM.ClearOldItems();
                }
                CurrentUndoManager.Push(cmds);

                RefreshItemsList(true);
            }
        }

        /// <summary>
        /// Check if every item in list widget has same 'TextValue'
        /// </summary>
        /// <returns></returns>
        private void CheckItemsSame()
        {
            if (_VMItems.Count < 1)
                return;

            bool bSame = true;
            IListBase baselist = (_VMItems.ElementAt(0) as ListBaseWidgetViewModel).IWidget;
            int itemsCount = baselist.Items.Count;
            //check if list widget has same items(text only).
            foreach (var item in _VMItems)
            {
                if (item == baselist)
                    continue;
                IListBase widget = (item as ListBaseWidgetViewModel).IWidget;
                if (widget == null || itemsCount != widget.Items.Count)
                {
                    bSame = false;
                    break;
                }
                else
                {
                    for (int i = 0; i < itemsCount - 1; i++)
                    {
                        if (widget.Items.ElementAt(i).TextValue != baselist.Items.ElementAt(i).TextValue)
                        {
                            bSame = false;
                            break;
                        }
                    }
                    if (bSame == false)
                        break;
                }
            }

            RefreshItemsList(bSame);

            _IsItemsSame = bSame;
        }

        /// <summary>
        /// Reresh items text list in property panel.
        /// </summary>
        /// <param name="bSame">if all widget has same items</param>
        private void RefreshItemsList(bool bSame)
        {
            ItemsList = string.Empty;
            if (bSame && _VMItems.Count > 0)
            {
                IListBase listBase = (_VMItems.ElementAt(0) as ListBaseWidgetViewModel).IWidget;
                foreach (IListItem item in listBase.Items)
                {
                    if (item == listBase.Items.ElementAt(0))
                    {
                        ItemsList += item.TextValue;
                    }
                    else
                    {
                        ItemsList += ", " + item.TextValue;
                    }
                }
            }
            FirePropertyChanged("ItemsList");
        }

        #endregion
    }
}
