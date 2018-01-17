using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Common.Helper;
using System.Windows.Controls;
using System.Windows.Documents;
using Naver.Compass.Service;
using System.Windows.Threading;
using System.ComponentModel;

namespace Naver.Compass.Module
{
    class FlickingStateManagerViewModel : ViewModelBase
    {
        public FlickingStateManagerViewModel(DynamicPageEditorViewModel OwnerPageVM)
        {
            this.AddDeviceCommand = new DelegateCommand<object>(AddDeviceExecute);
            this.MoveUpCommand = new DelegateCommand<object>(MoveUpExecute, CanExecuteMoveUp);
            this.MoveDownCommand = new DelegateCommand<object>(MoveDownExecute, CanExecuteMoveDown);
            this.DeleteDeviceCommand = new DelegateCommand<object>(DeleteDeviceExecute, CanExecuteDeleteDevice);
            this.CloseWindowCommand = new DelegateCommand<Window>(CloseWindowExecute);

            //new
            DynamicChildren = OwnerPageVM.DynamicChildren;
            foreach(DynamicChildNodViewModel item in DynamicChildren)
            {
                item.PropertyChanged += PagePropertyChangedHandler;
            }
            DyncWidget = OwnerPageVM.DyncWidget;
        }

        #region Commands 
        public DelegateCommand<object> AddDeviceCommand { get; private set; }
        public DelegateCommand<object> MoveUpCommand { get; private set; }
        public DelegateCommand<object> MoveDownCommand { get; private set; }
        public DelegateCommand<object> DeleteDeviceCommand { get; private set; }
        public DelegateCommand<Window> CloseWindowCommand { get; private set; }
        public DelegateCommand<object> ExchangeWidthHeightCommand { get; private set; }
        #endregion

        #region Property Change Handler
        private void PagePropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    {
                        HasChange = true;
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region Commands Handler
        private void AddDeviceExecute(object obj)
        {
            //Dom
            IPage newPage=DyncWidget.CreatePanelStatePage("P." + (DynamicChildren.Count+1));
            if (newPage == null)
            {
                return;
            }

            //UI
            DynamicChildNodViewModel childVM = new DynamicChildNodViewModel(newPage);
            DynamicChildren.Add(childVM);

            //Select
            SelectValue = childVM;
            _document.IsDirty = true;
            HasChange = true;
        }

        private void MoveUpExecute(object obj)
        {
            IPage item = selectValue.Page;
            DynamicChildNodViewModel node = selectValue;
            int index = DynamicChildren.IndexOf(node);

            //delete
            DyncWidget.PanelStatePages.Remove(item as IPanelStatePage);
            DynamicChildren.Remove(node);

            //add
            index--;
            DyncWidget.PanelStatePages.Insert(index, item as IPanelStatePage);
            DynamicChildren.Insert(index, node);


            //Select
            SelectValue = node;
            _document.IsDirty = true;
            HasChange = true;
        }
        public bool CanExecuteMoveUp(object obj)
        {
            return DynamicChildren.IndexOf(selectValue) > 0 && selectValue != null;
        }

        private void MoveDownExecute(object obj)
        {
            IPage item = selectValue.Page;
            DynamicChildNodViewModel node = selectValue;
            int index = DynamicChildren.IndexOf(node);

            //delete
            DyncWidget.PanelStatePages.Remove(item as IPanelStatePage);
            DynamicChildren.Remove(node);

            //add
            index++;
            DyncWidget.PanelStatePages.Insert(index, item as IPanelStatePage);
            DynamicChildren.Insert(index, node);


            //Select
            SelectValue = node;
            _document.IsDirty = true;
            HasChange = true;
        }
        private bool CanExecuteMoveDown(object obj)
        {
            return DynamicChildren.IndexOf(selectValue) < DynamicChildren.Count - 1 && selectValue != null;
        }

        private void DeleteDeviceExecute(object obj)
        {
            DynamicChildNodViewModel node = selectValue;

            //UI
            int index = DynamicChildren.IndexOf(node);
            DynamicChildren.Remove(node);

            //DOM
            DyncWidget.PanelStatePages.Remove(node.Page as IPanelStatePage);
            


            //Select
            if (index > 0)
            {
                SelectValue = DynamicChildren.ElementAt(--index);
            }
            else
            {
                SelectValue = DynamicChildren.ElementAt(0);
            }
            if (node.IsChecked == true)
            {
                SelectValue.IsChecked = true;
            }            
            _document.IsDirty = true;
            HasChange = true;
        }
        public bool CanExecuteDeleteDevice(object obj)
        {
            return selectValue != null && DynamicChildren.Count > 3;
        }

        public void CloseWindowExecute(Window win)
        {
            if (win != null)
            {
                win.Close();
            }
        }
        #endregion

        #region Private Functions and  Properties
        private IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc != null)
                    return doc.Document;
                return null;
            }
        }
        private IDynamicPanel DyncWidget;

        private void RefreshCommands()
        {
            AddDeviceCommand.RaiseCanExecuteChanged();
            MoveUpCommand.RaiseCanExecuteChanged();
            MoveDownCommand.RaiseCanExecuteChanged();
            DeleteDeviceCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Public Binding Property
        public ObservableCollection<DynamicChildNodViewModel> DynamicChildren { get; set; }
        public string Name
        {
            get { return DyncWidget.Name; }
        } 
        private DynamicChildNodViewModel selectValue;
        public DynamicChildNodViewModel SelectValue
        {
            get
            {
                return selectValue;
            }
            set
            {
                if (selectValue != value)
                {
                    //unselect old item
                    if (selectValue != null)
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

        public bool HasChange { get; set; }
        
        #endregion
    }

}
