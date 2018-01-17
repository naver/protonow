using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Practices.Prism.Commands;
using System.Windows;

namespace Naver.Compass.InfoStructure
{
    public class EditPaneViewModelBase : ViewModelBase
    {
        public EditPaneViewModelBase()
        {
            
        }

        #region Docking Pannel Binding Property
        //Icon Image
        public ImageSource IconSource
        {
            get;
            protected set;
        }

        //Title
        private string _title = null;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    FirePropertyChanged("Title");
                }
            }
        }

        //ContentId
        private string _contentId = null;
        public string ContentId
        {
            get { return _contentId; }
            set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    FirePropertyChanged("ContentId");
                }
            }
        }

        //IsSelected
        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    //Modify selection service's data when change current page,20140307
                    OnPannelSelected(value);


                    FirePropertyChanged("IsSelected");
                }
            }
        }

        virtual protected void OnPannelSelected(bool bIsSelected)
        {

        }

        //IsActive
        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    try
                    {
                        _isActive = value;
                        FirePropertyChanged("IsActive");
                    }
                    catch (Exception)
                     {
                    }
                }
            }
        }

        public bool IsNeedReturnFocus { get; set; }

        #endregion Docking Pannel Binding Property

        #region Base widget operation command
        //Add Item Command
        private DelegateCommand<object> _addItemCommand = null;
        public ICommand AddItemCommand
        {
            get
            {
                if (_addItemCommand == null)
                {
                    _addItemCommand = new DelegateCommand<object>(OnItemAdded);
                }
                return _addItemCommand;
            }
        }
        virtual protected void OnItemAdded(object obj)
        {

        }

        //Remove Item Command
        private DelegateCommand<object> _removeItemCommand = null;
        public ICommand RemoveItemCommand
        {
            get
            {
                if (_removeItemCommand == null)
                {
                    _removeItemCommand = new DelegateCommand<object>(OnItemRemoved);
                }
                return _removeItemCommand;
            }
        }
        virtual protected void OnItemRemoved(object obj)
        {

        }
        #endregion Base widget operation command
    }
}
