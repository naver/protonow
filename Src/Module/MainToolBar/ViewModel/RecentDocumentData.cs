using System.ComponentModel;

namespace MainToolBar.ViewModel
{
    public class RecentDocumentData : ToggleButtonData
    {
        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                if (_index != value)
                {
                    _index = value;
                    FirePropertyChanged("Index");
                }
            }
        }
        public string ActualPath
        {
            get
            {
                return _path;
            }

            set
            {
                if (_path != value)
                {
                    _path = value;
                    FirePropertyChanged("ActualPath");
                }
            }
        }
        private int _index;
        private string _path;
    }
}
