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
using System.Windows.Data;
using System.Windows.Input;

namespace Naver.Compass.Module
{
    public class GuideBoxViewModel : ViewModelBase
    {
        #region constructor
        public GuideBoxViewModel()
        {
            if (IsInDesignMode)
                return;
            GuideItems = new ObservableCollection<GuideItemBase>();
            AddHoriGuideLineCommand = new DelegateCommand<object>(AddHorizGuideLineHandler);
            AddVertGuideLineCommand = new DelegateCommand<object>(AddVertGuideLineHandler);

            _ListEventAggregator.GetEvent<SelectionPageChangeEvent>().Subscribe(SelectionPageChangeHandler);
            _ListEventAggregator.GetEvent<DeleteGuideEvent>().Subscribe(DeleteGuideHandler);
            _ListEventAggregator.GetEvent<UpdateGridGuide>().Subscribe(UpdateGridGuideHandler);
        }

        public ObservableCollection<GuideItemBase> GuideItems { get; set; }

        public DelegateCommand<object> AddHoriGuideLineCommand { get; set; }
        public DelegateCommand<object> AddVertGuideLineCommand { get; set; }
        #endregion

        #region public functions
        /// <summary>
        /// Add Guide-lines
        /// </summary>
        public void AddHorizGuideLineHandler(object obj)
        {
            if (GuideType == GuideType.Global || _page == null || _pageView == null)
                return;

            var values = (object[])obj;

            Ruler ruler = values[0] as Ruler;
            PageEditorViewModel editor = values[1] as PageEditorViewModel;
            System.Windows.Point point = Mouse.GetPosition(ruler);
            IGuide guide = _pageView.CreateGuide(Orientation.Horizontal, 0, (point.X + ruler.CountShift) / Scale);
            HorizontalGuideLine hLine = new HorizontalGuideLine(guide, GuideType.Local,editor.EditorScale);
            GuideItems.Add(hLine);
            _document.IsDirty = true;
            editor.EditorCanvas.Focus();

            List<IGuide> guides = new List<IGuide>();
            guides.Add(guide);
            CreatePageGuideCommand cmd = new CreatePageGuideCommand(_pageView, guides);
            CurrentUndoManager.Push(cmd);

            ShowGuide(editor);
        }
        public void AddVertGuideLineHandler(object obj)
        {
            if (GuideType == GuideType.Global || _page == null || _pageView == null)
                return;

            var values = (object[])obj;

            Ruler ruler = values[0] as Ruler;
            PageEditorViewModel editor = values[1] as PageEditorViewModel;
            System.Windows.Point point = Mouse.GetPosition(ruler);
            IGuide guide = _pageView.CreateGuide(Orientation.Vertical, (point.X + ruler.CountShift) / Scale, 0);
            VerticalGuideLine vLine = new VerticalGuideLine(guide, GuideType.Local, editor.EditorScale);
            editor.EditorCanvas.Focus();
            GuideItems.Add(vLine);
            _document.IsDirty = true;

            List<IGuide> guides = new List<IGuide>();
            guides.Add(guide);
            CreatePageGuideCommand cmd = new CreatePageGuideCommand(_pageView, guides);
            CurrentUndoManager.Push(cmd);
            
            ShowGuide(editor);
        }

        #endregion

        #region private functions
        /// <summary>
        /// If Guide is hidden, show it.
        /// Fired when page guide is hidden, and create a new page guide.
        /// </summary>
        /// <param name="obj">PageEditorViewModel</param>
        private void ShowGuide(PageEditorViewModel editor)
        {
            if (GlobalData.IsShowPageGuide == false)
            {
                editor.IsPageGuideVisible = true;
            }
        }

        /// <summary>
        /// Selected page changed in Sitmap or EditorView
        /// Get current selected widget.
        /// </summary>
        private void SelectionPageChangeHandler(Guid pageGuid)
        {
            if (!IsSelected)
                return;

            if (pageGuid == Guid.Empty)
                return;

            if (_document != null)
            {
                _page = SelectionService.GetCurrentPage().ActivePage;
                if (_page != null && _page.IsOpened)
                {
                    LoadGuides();
                }
            }
        }
        private void UpdateGridGuideHandler(GridGuideType type)
        {
            if (!IsSelected)
                return;

            if (GridGuideType.Guide == type)
            {
                LoadGuides();
            }
        }

        private void DeleteGuideHandler(GuideInfo guideInfo)
        {
            if (!IsSelected)
                return;

            //Delete all guides,execute in both global and local GuideBoxViewModel.
            if (guideInfo.Guide == null)
            {
                DeleteAllGuides();
                return;
            }

            //Make sure execute once
            if (guideInfo.Type != this.GuideType)
                return;

            //Delete page or global guide
            if (GuideType == GuideType.Global )
            {
                DeleteGlobalGuide(guideInfo.Guide);
            }
            else
            {
                DeletePageGuide(guideInfo.Guide);
            }
        }
        /// <summary>
        /// Delete all from doc in Global GuideBoxViewModel, and clear global guides in view.
        /// Clear page guides in view
        /// </summary>
        private void DeleteAllGuides()
        {
            if(GuideType==GuideType.Global)
            {
                if (_document != null && _page != null && _pageView != null)
                {
                    
                    Naver.Compass.InfoStructure.CompositeCommand cmds = new Naver.Compass.InfoStructure.CompositeCommand();

                    #region delete local guides(only current view)
                    if (_pageView.Guides.Count > 0)
                    {
                        List<IGuide> guides = new List<IGuide>();

                        foreach (IGuide guide in _pageView.Guides)
                        {
                            if (guide.IsLocked == false)
                            {
                                guides.Add(guide);
                            }
                        }

                        //delete from doc
                        foreach (IGuide guide in guides)
                        {
                            _pageView.DeleteGuide(guide.Guid);
                        }

                        // Redo/Undo for page guides
                        DeletePageGuideCommand cmd = new DeletePageGuideCommand(_pageView, guides);
                        cmds.AddCommand(cmd);
                    }
                    #endregion

                    #region delete global guides.

                    if (_document.GlobalGuides.Count > 0)
                    {
                        List<IGuide> golbalGuides = new List<IGuide>();
                        foreach (IGuide guide in _document.GlobalGuides)
                        {
                            if (guide.IsLocked == false)
                            {
                                golbalGuides.Add(guide);
                            }
                        }

                        //delete from doc
                        foreach (IGuide guide in golbalGuides)
                        {
                            _document.DeleteGlobalGuide(guide.Guid);
                        }

                        // Redo/Undo for Global Guides
                        DeleteGlobalGuideCommand gCmd = new DeleteGlobalGuideCommand(_document, golbalGuides);
                        cmds.AddCommand(gCmd);
                    }
                    #endregion

                    if (cmds.Count > 0)
                    {
                        CurrentUndoManager.Push(cmds);
                    }

                    LoadGlobalGuide();
                }
            }
            else
            {
                LoadPageGuide();
            }

        }

        private void DeletePageGuide(IGuide guide)
        {
            if (_page != null && _pageView != null)
            {
                List<IGuide> guides = new List<IGuide>();
                guides.Add(guide);

                DeletePageGuideCommand cmd = new DeletePageGuideCommand( _pageView, guides);
                CurrentUndoManager.Push(cmd);

                //Remove from View
                GuideItems.Remove(GuideItems.FirstOrDefault(x => x.Guide == guide));
                //Remove from doc

                _pageView.DeleteGuide(guide.Guid);
            }
        }

        private void DeleteGlobalGuide(IGuide guide)
        {
            if (_document != null )
            {
                List<IGuide> guides = new List<IGuide>();
                guides.Add(guide);
                DeleteGlobalGuideCommand cmd = new DeleteGlobalGuideCommand(_document, guides);
                CurrentUndoManager.Push(cmd);

                //Remove from View
                GuideItems.Remove(GuideItems.FirstOrDefault(x => x.Guide == guide));
                //Remove from doc
                _document.DeleteGlobalGuide(guide.Guid);
            }
        }

        public void LoadGuides()
        {
            //reset page-view when change page or change adaptive view.
            _pageView = _page.PageViews.GetPageView(SelectionService.GetCurrentPage().CurAdaptiveViewGID);
            if (_pageView == null)
            {
                _pageView = _page.PageViews.GetPageView(_document.AdaptiveViewSet.Base.Guid);
            }
            if (_pageView == null)
            {
                return;
            }

            if (GuideType == GuideType.Local)
            {
                LoadPageGuide();
            }
            else
            {
                LoadGlobalGuide();
            }
        }

        private void LoadPageGuide()
        {
            GuideItems.Clear();

            if (_page != null && _pageView != null)
            {
                foreach (IGuide item in _pageView.Guides)
                {
                    switch (item.Orientation)
                    {
                        case Orientation.Horizontal:
                            GuideItems.Add(new HorizontalGuideLine(item as IGuide, GuideType.Local, scale));
                            break;
                        case Orientation.Vertical:
                            GuideItems.Add(new VerticalGuideLine(item as IGuide, GuideType.Local, scale));
                            break;
                    }
                }
            }
        }

        private void LoadGlobalGuide()
        {
            GuideItems.Clear();

            if (_document != null)
            {
                foreach (IGuide item in _document.GlobalGuides)
                {
                    switch (item.Orientation)
                    {
                        case Orientation.Horizontal:
                            GuideItems.Add(new HorizontalGuideLine(item as IGuide, GuideType.Global, scale));
                            break;
                        case Orientation.Vertical:
                            GuideItems.Add(new VerticalGuideLine(item as IGuide, GuideType.Global, scale));
                            break;
                    }
                }
            }
        }

        private void ChangeScale(double scale)
        {
            foreach (var item in GuideItems)
            {
                VerticalGuideLine vline = item as VerticalGuideLine;
                if (vline != null)
                {
                    vline.Scale = scale;
                }
                else
                {
                    (item as HorizontalGuideLine).Scale = scale;
                }
            }
        }
        #endregion

        #region binding properties
        public Visibility GuideBoxVisibility
        {
            get { return guideBoxVisibility; }
            set
            {
                guideBoxVisibility = value;
                FirePropertyChanged("GuideBoxVisibility");

            }
        }

        //Global and local use the same viwmodel
        public GuideType GuideType { get; set; }

        //is current guidebox is in current selected page.
        public bool IsSelected { get; set; }

        public bool IsShowGridCheck
        {
            get
            {
                return GlobalData.IsShowGrid;
            }
        }

        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                ChangeScale(value);
            }
        }

        #endregion

        #region private fields

        private Visibility guideBoxVisibility;

        private IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        private IPageView _pageView;
        private IPage _page;
        private double scale = 1;
        #endregion

    }

    #region Guide Converter
    public class GuideWidthHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            
            double scale = (double)parameter;
            double Value = (double)value * scale;

            return Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
