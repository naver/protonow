using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MainToolBar.ViewModel
{
    public class GalleryData : ControlData
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<GalleryCategoryData> CategoryDataCollection
        {
            get
            {
                if (_controlDataCollection == null)
                {
                    _controlDataCollection = new ObservableCollection<GalleryCategoryData>();

                    Uri smallImage = new Uri("Images/Paste_16x16.png", UriKind.Relative);
                    Uri largeImage = new Uri("Images/Paste_32x32.png", UriKind.Relative);

                    for (int i = 0; i < ViewModelData.GalleryCategoryCount; i++)
                    {
                        _controlDataCollection.Add(new GalleryCategoryData()
                        {
                            Label = "GalleryCategory " + i,
                            SmallImage = smallImage,
                            LargeImage = largeImage,
                            ToolTipTitle = "ToolTip Title",
                            ToolTipDescription = "ToolTip Description",
                            ToolTipImage = smallImage,
                            Command = ViewModelData.DefaultCommand
                        });
                    }
                }
                return _controlDataCollection;
            }
        }
        private ObservableCollection<GalleryCategoryData> _controlDataCollection;

        public GalleryItemData SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    FirePropertyChanged("SelectedItem");
                }
            }
        }
        GalleryItemData _selectedItem;

        public bool CanUserFilter
        {
            get
            {
                return _canUserFilter;
            }

            set
            {
                if (_canUserFilter != value)
                {
                    _canUserFilter = value;
                    FirePropertyChanged("CanUserFilter");
                }
            }
        }

        private bool _canUserFilter;
    }

    public class GalleryData<T> : ControlData
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<GalleryCategoryData<T>> CategoryDataCollection
        {
            get
            {
                if (_controlDataCollection == null)
                {
                    _controlDataCollection = new ObservableCollection<GalleryCategoryData<T>>();
                }
                return _controlDataCollection;
            }
        }
        private ObservableCollection<GalleryCategoryData<T>> _controlDataCollection;

        public T SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (!Object.Equals(value, _selectedItem))
                {
                    IsExcuteAfterUpdate = true;
                    _selectedItem = value;
                    FirePropertyChanged("SelectedItem");
                    IsExcuteAfterUpdate = false;
                }
            }
        }
        T _selectedItem;

        bool _gradientEnable;
        public bool GradientEnable
        {
            get { return _gradientEnable; }
            set
            {
                if (value != _gradientEnable)
                {
                    _gradientEnable = value;
                    FirePropertyChanged("GradientEnable");
                }
            }
        }

        public bool CanUserFilter
        {
            get
            {
                return _canUserFilter;
            }

            set
            {
                if (_canUserFilter != value)
                {
                    _canUserFilter = value;
                    FirePropertyChanged("CanUserFilter");
                }
            }
        }  
        private bool _canUserFilter;

        private bool _isExcuteAfterUpdate;
        public bool IsExcuteAfterUpdate
        {
            get
            {
                return _isExcuteAfterUpdate;
            }

            set
            {
                if (_isExcuteAfterUpdate != value)
                {
                    _isExcuteAfterUpdate = value;
                    FirePropertyChanged("IsExcuteAfterUpdate");
                }
            }
        }

    
    }
}
