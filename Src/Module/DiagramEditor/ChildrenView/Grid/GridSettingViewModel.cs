using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Naver.Compass.Common;

namespace Naver.Compass.Module
{
    class GridSettingViewModel:ViewModelBase
    {
        public GridSettingViewModel()
        {
            //init data from global setting.
            _model = SettingModel.GetInstance();

        }
        public bool IsShowGrid
        {
            get
            {
                return _model.IsShowGrid;
            }
            set
            {
                if (_model.IsShowGrid != value)
                {
                    _model.IsShowGrid = value;
                    FirePropertyChanged("IsShowGrid");
                }
            }
        }
        public bool IsSnapToGrid
        {
            get
            {
                return _model.IsSnapToGrid;
            }
            set
            {
                if (_model.IsSnapToGrid != value)
                {
                    _model.IsSnapToGrid = value;
                    FirePropertyChanged("IsSnapToGrid");
                }
            }
        }
        public string GridSize
        {
            get
            {
                return _model.GridSize;
            }
            set
            {
                if (_model.GridSize != value)
                {
                    _model.GridSize = value;
                    FirePropertyChanged("GridSize");
                }
            }
        }

        public bool IsLineChecked
        {
            get
            {
                return _model.IsLineType;
            }
            set
            {
                if(true==value)
                {
                    _model.IsLineType = value;
                    FirePropertyChanged("IsLineChecked");
                }
            }
        }
        public bool IsIntersectChecked
        {
            get
            {
                return !_model.IsLineType;
            }
            set
            {
                if (true == value)
                {
                    _model.IsLineType = !value;
                    FirePropertyChanged("IsIntersectChecked");
                }
            }
        }

        public StyleColor GridColor
        {
            get
            {
                return _model.GridColor;
            }
            set
            {
                if(!_model.GridColor.Equals(value))
                {
                    _model.GridColor = value;
                    //FirePropertyChanged("GridColor");
                    FirePropertyChanged("SelectedColor");
                }
            }
        }

        public Brush SelectedColor
        {
            get
            {
                return (new SolidColorBrush(GridColor.ToColor()));
            }

        }

        private SettingModel _model;
    }
}
