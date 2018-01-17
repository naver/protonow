using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;

namespace MainToolBar.ViewModel
{
    class RecentMenuItemData: INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public ICommand iCommand
        {
            get
            {
                return _command;
            }

            set
            {
                if (_command != value)
                {
                    _command = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Command"));
                }
            }
        }
        private ICommand _command;

         public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }
}
