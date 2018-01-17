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
    class GuidesSettingViewModel:ViewModelBase
    {

        public GuidesSettingViewModel()
        {
            _model = SettingModel.GetInstance();
        }

        public bool IsShowGlobalGuides
        {
            get
            {
                return _model.IsShowGlobalGuides;
            }
            set
            {
                if(_model.IsShowGlobalGuides!=value)
                {
                    _model.IsShowGlobalGuides = value;
                    FirePropertyChanged("IsShowGlobalGuides");
                }
            }
        }
        public bool IsShowPageGuides
        {
            get
            {
                return _model.IsShowPageGuides;
            }
            set
            {
                if(_model.IsShowPageGuides!=value)
                {
                    _model.IsShowPageGuides = value;
                    FirePropertyChanged("IsShowPageGuides");
                }
            }
        }
        public bool IsSnapToGuides
        {
            get
            {
                return _model.IsSnapToGuides;
            }
            set
            {
                if(_model.IsSnapToGuides!=value)
                {
                    _model.IsSnapToGuides = value;
                    FirePropertyChanged("IsSnapToGuides");
                }
            }
        }
        public bool IsLockGuides
        {
            get
            {
                return _model.IsLockGuides;
            }
            set
            {
                if(_model.IsLockGuides!=value)
                {
                    _model.IsLockGuides = value;
                    FirePropertyChanged("IsLockGuides");
                }
            }
        }

        public StyleColor GlobalGuideColor
        {
            get
            {
                return _model.GlobalGuideColor;
            }
            set
            {
                if (!_model.GlobalGuideColor.Equals(value))
                {
                    _model.GlobalGuideColor = value;
                    FirePropertyChanged("GlobalSelectedColor");
                }
            }
        }

        public Brush GlobalSelectedColor
        {
            get
            {
                return (new SolidColorBrush(GlobalGuideColor.ToColor()));
            }

        }

        public StyleColor LocalGuideColor
        {
            get
            {
                return _model.LocalGuideColor;
            }
            set
            {
                if (!_model.LocalGuideColor.Equals(value))
                {
                    _model.LocalGuideColor = value;
                    FirePropertyChanged("LocalSelectedColor");
                }
            }
        }

        public Brush LocalSelectedColor
        {
            get
            {
                return (new SolidColorBrush(LocalGuideColor.ToColor()));
            }

        }

        SettingModel _model;
    }
}
