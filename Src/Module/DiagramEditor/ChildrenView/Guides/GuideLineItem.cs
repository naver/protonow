using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Naver.Compass.Module
{
    public class GuideItemBase:ViewModelBase
    {        
        protected IGuide _guide;
        protected GuideType _type;
        public GuideItemBase(IGuide guid, GuideType type)
        {
            _type = type;
            _guide = guid;
            if (type == GuideType.Global)
            {
                lineColor = new SolidColorBrush(Color.FromRgb(GlobalData.GlobalGuideColor.R,GlobalData.GlobalGuideColor.G,GlobalData.GlobalGuideColor.B));
            }
            else
            {
                lineColor = new SolidColorBrush(Color.FromRgb(GlobalData.LocalGuideColor.R, GlobalData.LocalGuideColor.G, GlobalData.LocalGuideColor.B)); 
            }
            LockCommand = new DelegateCommand<object>(LockExecute);
            DeleteCommand = new DelegateCommand<object>(DeleteExecute, CanDeleteExecute);
            _ListEventAggregator.GetEvent<GlobalLockGuides>().Subscribe(GlobalLockGuidesHandler);
            //isLocked = true;
        }

        #region binding propery
        public bool IsLocked
        {
            get { return _guide.IsLocked; }
            set
            {
                if (_guide.IsLocked != value)
                {
                    _guide.IsLocked = value;
                    FirePropertyChanged("IsLocked");
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    FirePropertyChanged("IsSelected");
                }
            }
        }

        private SolidColorBrush lineColor;
        public SolidColorBrush LineColor
        {
            get { return lineColor; }
            set
            {
                if(lineColor!=value)
                {
                    lineColor = value;
                    FirePropertyChanged("LineColor");
                }
            }
        }

        public Cursor LockCursor
        {
            get
            {
                return CommonFunction.GetLockCur();
            }
        }

        public bool IsShowGridCheck
        {
            get
            {
                return GlobalData.IsShowGrid;
            }
        }
        public bool IsSnapToGridCheck
        {
            get
            {
                return GlobalData.IsSnapToGrid;
            }
        }

        public bool IsShowGlobalGuide
        {
            get
            {
                return GlobalData.IsShowGlobalGuide;
            }
        }
        public bool IsShowPageGuide
        {
            get
            {
                return GlobalData.IsShowPageGuide;
            }
        }
        public bool IsSnapToGuide
        {
            get
            {
                return GlobalData.IsSnapToGuide;
            }
        }

        //Golbal lock, all guides will be locked.
        public Visibility IsMoveThumbVisible
        {
            get
            {
                if (GlobalData.IsLockGuides)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }
        public IGuide Guide
        {
            get { return _guide; }
        }

        public DelegateCommand<object> LockCommand { get; set; }
        public DelegateCommand<object> DeleteCommand { get; set; }

        private void LockExecute(object obj)
        {
            GuidePropertyChangeCommand cmd = new GuidePropertyChangeCommand(_guide, "IsLocked", IsLocked, !IsLocked);
            CurrentUndoManager.Push(cmd);

            IsLocked = !IsLocked;

        }
        protected void DeleteExecute(object obj)
        {
            _ListEventAggregator.GetEvent<DeleteGuideEvent>().Publish(new GuideInfo(this._type, _guide));
        }

        protected bool CanDeleteExecute(object obj)
        {
            return !IsLocked;
        }

        public void UpdateGridGuide()
        {
            FirePropertyChanged("IsShowGridCheck");
            FirePropertyChanged("IsSnapToGridCheck");
            FirePropertyChanged("IsShowPageGuide");
            FirePropertyChanged("IsShowGlobalGuide");
            FirePropertyChanged("IsSnapToGuide");
        }

        private void GlobalLockGuidesHandler(object obj)
        {
            FirePropertyChanged("IsMoveThumbVisible");
        }
        #endregion

    }

    class HorizontalGuideLine : GuideItemBase
    {
        public HorizontalGuideLine(IGuide guid, GuideType type, double scale)
            : base(guid, type)
        {
            this._scale = scale;
        }
        public double Top
        {
            get
            {
                return _guide.Y * Scale;
            }
        }

        public double ActualTop
        {
            get
            {
                return _guide.Y;
            }
            set
            {
                if (_guide.Y != value)
                {
                    _guide.Y = value;
                    IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                    doc.Document.IsDirty = true;
                    FirePropertyChanged("Top");
                }
            }
        }

        private double _scale = 1.0;
        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                FirePropertyChanged("Top");
            }
        }

    }

    class VerticalGuideLine : GuideItemBase
    {

        public VerticalGuideLine(IGuide guid, GuideType type, double scale)
            : base(guid,type)
        {
            this._scale = scale;
        }
        public double Left
        {
            get
            {
                return _guide.X * Scale;
            }
        }

        public double ActualLeft
        {
            get
            {
                return _guide.X;
            }
            set
            {
                if (_guide.X != value)
                {
                    _guide.X = value;
                    IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                    doc.Document.IsDirty = true;
                    FirePropertyChanged("Left");
                }
            }
        }

        private double _scale = 1.0;
        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                FirePropertyChanged("Left");
            }
        }
    }
}
