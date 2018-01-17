using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.WidgetLibrary
{
    public class NodeViewModel:ViewModelBase
    {
        /// <summary>
        /// Create
        /// </summary>
        public NodeViewModel(string name)
        {
            this._name = name;
            this._isSelected = true;
            this._isNodeEditable = true;
        }

        /// <summary>
        /// Load
        /// </summary>
        public NodeViewModel(string name, bool isChecked)
        {
            this._name = name;
            this._isChecked = isChecked;
        }


        private string _name;
        public string Name
        {
            get { return _name; }

            set
            {
                if(_name!=value)
                {
                    _name = value;
                    FirePropertyChanged("Name");
                }
            }
        }

        bool isEditboxFocus;
        //used to assist- edit node when click twice.
        public bool IsEditboxFocus
        {
            get { return isEditboxFocus; }
            set
            {
                if (isEditboxFocus != value)
                {
                    isEditboxFocus = value;
                    FirePropertyChanged("IsEditboxFocus");
                }
            }
        }

        private bool _isChecked;
        //Node be selected,can be many.(Data)
        //IsChecked ==== IsSelected in IWidget
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    FirePropertyChanged("IsChecked");
                }
            }
        }

        private bool _isSelected;
        //Item in listbox be selected, one every time.(UI)
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if(_isSelected!=value)
                {
                    _isSelected = value;
                    FirePropertyChanged("IsSelected");
                }
            }
        }

        private bool _isNodeEditable;
        public bool IsNodeEditable
        {
            get { return _isNodeEditable; }
            set
            {
                if (_isNodeEditable != value)
                {
                    _isNodeEditable = value;
                    FirePropertyChanged("IsNodeEditable");
                }
            }
        }
    }
}
