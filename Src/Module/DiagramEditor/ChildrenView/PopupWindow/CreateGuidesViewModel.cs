using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Naver.Compass.Module
{
    class CreateGuidesViewModel : ViewModelBase
    {
        public CreateGuidesViewModel(Guid guid)
        {
            if(_document!=null)
            {
                _page = _document.Pages.GetPage(guid);
            }
            this.PresetChangedCommand = new DelegateCommand<object>(PresetChangedExecute);
            this.OKCommand = new DelegateCommand<object>(OKExecute);
            this.CancelCommand = new DelegateCommand<object>(CancelExecute);

        }

        public DelegateCommand<object> OKCommand { get; set; }
        public DelegateCommand<object> CancelCommand { get; set; }
        public DelegateCommand<object> PresetChangedCommand { get; set; }

        private void PresetChangedExecute(object obj)
        {
            if (obj == null)
                return;

            switch((int)obj)
            {
                case 0:
                    ColumnsCount = 12;
                    ColumnWidth = 60;
                    GutterWidth = 20;
                    ColumnMargin = 10;
                    break;
                case 1:
                    ColumnsCount = 16;
                    ColumnWidth = 40;
                    GutterWidth = 20;
                    ColumnMargin = 10;
                    break;
                case 2:
                    ColumnsCount = 12;
                    ColumnWidth = 80;
                    GutterWidth = 20;
                    ColumnMargin = 10;
                    break;
                case 3:
                    ColumnsCount = 15;
                    ColumnWidth = 60;
                    GutterWidth = 20;
                    ColumnMargin = 10;
                    break;
            }

        }
        private void OKExecute(object obj)
        {
            if ((_isGlobalChecked && _document != null) || (!_isGlobalChecked && _page != null))
            {
                if (RowsCount != 0 || ColumnsCount != 0)
                {
                    CreateGuides();
                }
            }
            Window win = obj as Window;
            win.DialogResult = true;
            win.Close();
        }

        private void CancelExecute(object obj)
        {
            Window win = obj as Window;
            win.DialogResult = false;
            win.Close();
        }

        private void CreateGuides()
        {
            _guideList = new List<IGuide>();

            if (ColumnsCount > 0)
                CreateVerticalGuides();

            if (RowsCount > 0)
                CreateHorizontalGuides();       
   
            if(_isGlobalChecked)
            {
                CreateGlobalGuideCommand cmd = new CreateGlobalGuideCommand( _document, _guideList);
                CurrentUndoManager.Push(cmd);
            }
            else
            {
                IPageView pageView = _page.PageViews.GetPageView(SelectionService.GetCurrentPage().CurAdaptiveViewGID);
                if (pageView == null)
                {
                    pageView = _page.PageViews.GetPageView(_document.AdaptiveViewSet.Base.Guid);
                }
                if (pageView == null)
                {
                    return;
                }
                CreatePageGuideCommand cmd = new CreatePageGuideCommand(pageView, _guideList);
                CurrentUndoManager.Push(cmd);
            }
        }

        /// <summary>
        /// Horenzontal Guide.
        /// </summary>
        private void CreateHorizontalGuides()
        {
            bool bRet = true;
           
            for (int index = 0; index <= RowsCount; index++)
            {
                int position;
                if (index == 0)
                {
                    if (RowMargin != 0)
                    {//if a guide's position greater than max,  stop creating guides greater than it
                        bRet = CreateGuide(Orientation.Horizontal, 0, RowMargin);
                        if (!bRet) break;
                    }
                    continue;
                }

                position = RowMargin + index * RowHeight + (index - 1) * GutterHeight;

                bRet = CreateGuide(Orientation.Horizontal, 0, position);
                if (!bRet) break;

                if (index != RowsCount)
                {
                    position += GutterHeight;
                    bRet = CreateGuide(Orientation.Horizontal, 0, position);
                    if (!bRet) break;
                }
                else
                {
                    position += RowMargin;
                    bRet = CreateGuide(Orientation.Horizontal, 0, position);
                    if (!bRet) break;
                }
            }
        }

        /// <summary>
        ///Vertical guide
        /// </summary>
        private void CreateVerticalGuides()
        {
            bool bRet = true;
            for (int index = 0; index <= ColumnsCount; index++)
            {
                int position;
                if (index == 0)
                {
                    if (ColumnMargin != 0)
                    {
                        bRet = CreateGuide(Orientation.Vertical, ColumnMargin, 0);
                        if (!bRet) break;
                    }
                    continue;
                }

                position = ColumnMargin + index * ColumnWidth + (index - 1) * GutterWidth;

                bRet = CreateGuide(Orientation.Vertical, position, 0);
                if (!bRet) break;

                if (index != ColumnsCount)
                {
                    position += GutterWidth;

                    bRet = CreateGuide(Orientation.Vertical, position, 0);
                    if (!bRet) break;
                }
                else
                {
                    position += ColumnMargin;
                    bRet = CreateGuide(Orientation.Vertical, position, 0);
                    if (!bRet) break;
                }

            }
        }

        /// <summary>
        /// Create Guide. if position > max, create failed.
        /// </summary>
        private bool CreateGuide(Orientation orient,int x, int y)
        {
            if (orient == Orientation.Vertical)
            {
                if (x > CommonDefine.MaxEditorWidth)
                    return false;
            }
            else
            {
                if (y > CommonDefine.MaxEditorHeight)
                    return false;
            }

            if (_isGlobalChecked)
            {
                IGuide guide = _document.CreateGlobalGuide(orient, x, y);
                _guideList.Add(guide);
            }
            else
            {
                IPageView pageView = _page.PageViews.GetPageView(SelectionService.GetCurrentPage().CurAdaptiveViewGID);
                if (pageView != null)
                {
                    IGuide guide = pageView.CreateGuide(orient, x, y);
                    _guideList.Add(guide);
                }
            }
            return true;
        }


        private IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        private IPage _page;

        private List<IGuide> _guideList;

        #region property
        private int _columnsCount;
        public int ColumnsCount
        {
            get
            {
                return _columnsCount;
            }
            set
            {
                if (_columnsCount != value)
                {
                    _columnsCount = value;
                    FirePropertyChanged("ColumnsCount");
                }
            }
        }
        public int _columnWidth = 60;
        public int ColumnWidth
        {
            get
            {
                return _columnWidth;
            }
            set
            {
                if (_columnWidth != value)
                {
                    _columnWidth = value;
                    FirePropertyChanged("ColumnWidth");
                }
            }
        }
        public int _gutterWidth = 20;
        public int GutterWidth
        {
            get
            {
                return _gutterWidth;
            }
            set
            {
                if (_gutterWidth != value)
                {
                    _gutterWidth = value;
                    FirePropertyChanged("GutterWidth");
                }
            }
        }
        public int _columnMargin = 10;
        public int ColumnMargin
        {
            get
            {
                return _columnMargin;
            }
            set
            {
                if (_columnMargin != value)
                {
                    _columnMargin = value;
                    FirePropertyChanged("ColumnMargin");
                }
            }
        }
        public int _rowsCount;
        public int RowsCount
        {
            get
            {
                return _rowsCount;
            }
            set
            {
                if (_rowsCount != value)
                {
                    _rowsCount = value;
                    FirePropertyChanged("RowsCount");
                }
            }
        }

        private int _rowHeight = 40;
        public int RowHeight
        {
            get
            {
                return _rowHeight;
            }
            set
            {
                if (_rowHeight != value)
                {
                    _rowHeight = value;
                    FirePropertyChanged("RowHeight");
                }
            }
        }
        public int _gutterHeight = 20;
        public int GutterHeight
        {
            get
            {
                return _gutterHeight;
            }
            set
            {
                if (_gutterHeight != value)
                {
                    _gutterHeight = value;
                    FirePropertyChanged("GutterHeight");
                }
            }
        }
        public int _rowMargin = 0;
        public int RowMargin
        {
            get
            {
                return _rowMargin;
            }
            set
            {
                if (_rowMargin != value)
                {
                    _rowMargin = value;
                    FirePropertyChanged("RowMargin");
                }
            }
        }

        private bool _isGlobalChecked = true;
        public bool IsGlobalChecked
        {
            get
            {
                return _isGlobalChecked;
            }
            set
            {
                if(_isGlobalChecked!=value)
                {
                    _isGlobalChecked = value;
                    FirePropertyChanged("IsGlobalChecked");
                }
            }
        }
        #endregion


    }
}
