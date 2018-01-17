using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Naver.Compass.Module
{
    class MenuPageEditorViewModel:ViewModelBase
    {
        public MenuPageEditorViewModel(PageEditorViewModel pageViewModel)
        {
            this._pageEditorViewModel = pageViewModel;
            _ListEventAggregator.GetEvent<RefreshWidgetChildPageEvent>().Subscribe(RefreshFlickingUIHandler);
        }

        #region Command
        private RelayCommand _mouseDoubleClickCommand;
        public RelayCommand MouseDoubleClickCommand
        {
            get
            {
                if (_mouseDoubleClickCommand == null)
                    _mouseDoubleClickCommand = new RelayCommand(param => MouseDoubleClick((MouseEventArgs)param));
                return _mouseDoubleClickCommand;
            }
            set { _mouseDoubleClickCommand = value; }
        }

        private RelayCommand _editorouseDownCommand;
        public RelayCommand EditorMouseDownCommand
        {

            get
            {
                if (_editorouseDownCommand == null)
                    _editorouseDownCommand = new RelayCommand(param => EditorMouseDown((MouseEventArgs)param));
                return _editorouseDownCommand;
            }
            set { _editorouseDownCommand = value; }

        }
        #endregion

        #region Binding style and Properties
        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                LoadAllChildrenWidgets();
                return _items;
            }
        }

        //used for redo/undo when resizing
        public double Raw_EditorRectWidth
        {
            set
            {
                EditorRectWidth = value;
            }
        }

        //used for binding
        public double EditorRectWidth
        {
            get
            {
                return (_hamburgerVM == null) ? 0 : _hamburgerVM.MenuPageWidth ;
            }
            set
            {
                _hamburgerVM.MenuPageWidth = value ;
                FirePropertyChanged("EditorRectWidth");
                EditorFocus();
            }
        }
        public double Raw_EditorRectHeight
        {
            set
            {
                EditorRectHeight = value;
            }
        }
        public double EditorRectHeight
        {
            get
            {
                return (_hamburgerVM == null) ? 0 : _hamburgerVM.MenuPageHeight;
            }
            set
            {
                _hamburgerVM.MenuPageHeight = value;
                FirePropertyChanged("EditorRectHeight");
                EditorFocus();
            }
        }
        public double Raw_EditorRectLeft
        {
            set
            {
                EditorRectLeft = value;
            }
        }

        public double EditorRectLeft
        {
            get
            {
                return (_hamburgerVM == null) ? 0 : _hamburgerVM.MenuPageLeft;
            }
            set
            {
                _hamburgerVM.MenuPageLeft = value;
                FirePropertyChanged("EditorRectLeft");
                EditorFocus();
            }
        }
        public double Raw_EditorRectTop
        {
            set
            {
                EditorRectTop = value;
            }
        }

        public double EditorRectTop
        {
            get
            {
                return (_hamburgerVM == null) ? 0 : _hamburgerVM.MenuPageTop;
            }
            set
            {
                _hamburgerVM.MenuPageTop = value;
                FirePropertyChanged("EditorRectTop");
                EditorFocus();
            }
        }
        #endregion

        #region Public properties and Functions
        public void IniParentVM(PageEditorViewModel pageViewModel)
        {
            this._pageEditorViewModel = pageViewModel;
            
        }
        public Thickness VerticalLineMargin { get; set; }
        public Thickness HorizontalLineMargin { get; set; }

        public void MouseDoubleClick(MouseEventArgs e)
        {
            _ListEventAggregator.GetEvent<OpenWidgetPageEvent>().Publish(_hamburgerVM.Widget);

            _hamburgerVM.IsChildPageOpened = true;
        }
        public void EditorMouseDown(MouseEventArgs e)
        {
            (e.Source as UIElement).Focus();
            if(e.RightButton == MouseButtonState.Pressed)
            {
                e.Handled = true;
            }
        }
        internal void InitHamburgerEditor(HamburgerMenuViewModel hamburgerVM)
        {
            this._hamburgerVM = hamburgerVM;
         
            UpdateHanburgerEditor();
        }
        internal void UpdateHanburgerEditor()
        {
            FirePropertyChanged("EditorRectWidth");
            FirePropertyChanged("EditorRectHeight");
            FirePropertyChanged("EditorRectLeft");
            FirePropertyChanged("EditorRectTop");
            FirePropertyChanged("Items");
        }
        public void LoadAllChildrenWidgets()
        {
            if (_items == null)
            {
                _items = new ObservableCollection<WidgetPreViewModeBase>();
            }
            _items.Clear();

            _hamburgerVM.Widget.MenuPage.Open();

            Guid viewID = _pageEditorViewModel.CurAdaptiveViewGID;
            IPageView view = _hamburgerVM.Widget.MenuPage.PageViews.GetPageView(viewID);
            if (view == null)
            {
                return;
            }

            foreach (IWidget wdg in _hamburgerVM.Widget.MenuPage.Widgets)
            {
                WidgetPreViewModeBase preItem = ReadOnlyWidgetFactory.CreateWidget(wdg,false);
                if (preItem == null)
                {
                    continue;
                }
                preItem.ChangeCurrentPageView(view);
                _items.Add(preItem);
            }
        }
        #endregion

        #region Private properties and Functions
        private PageEditorViewModel _pageEditorViewModel;
        private HamburgerMenuViewModel _hamburgerVM;
        private ObservableCollection<WidgetPreViewModeBase> _items;
        /// <summary>
        /// Selected page changed in Sitmap or EditorView
        /// For getting current selected widget.
        /// </summary>
        private void RefreshFlickingUIHandler(Guid pageGuid)
        {
            if (pageGuid == Guid.Empty 
                || _hamburgerVM == null 
                || pageGuid != _hamburgerVM.Widget.MenuPage.Guid 
                || _hamburgerVM.Widget.ParentPage.IsOpened == false)
                return;

            FirePropertyChanged("Items");
        }

        /// <summary>
        /// Change scale then resize, must set focus to editor canvas.
        /// </summary>
        private void EditorFocus()
        {
            this._pageEditorViewModel.EditorCanvas.Focus();
        }
        #endregion

        #region Undo/Redo

        public PropertyMementos CreateNewPropertyMementos()
        {
            _mementos = new PropertyMementos();
            return _mementos;
        }

        public PropertyMementos PropertyMementos
        {
            get { return _mementos; }
        }

        private PropertyMementos _mementos;

        #endregion    
    }
}
