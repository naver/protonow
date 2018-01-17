using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.CommonBase;
using System.ComponentModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service;

namespace Naver.Compass.WidgetLibrary
{
    public class DropInfo
    {
        public DragEventArgs e;
        public Point position;
    }

    public class DesignerCanvas : Canvas
    {
        private Point? dragStartPoint = null;
        public DesignerCanvas()
        {
            if (DesignerProperties.GetIsInDesignMode(this) == true)
            {
                return;
            }
            //EditorTarget = this;
            Loaded += DesignerCanvas_Loaded;
        }

        void DesignerCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            IPagePropertyData page = DataContext as IPagePropertyData;
            page.EditorCanvas = this;
            if (!page.IsNeedReturnFocus)
            {
                Focus();
            }
            else
            {
                page.IsNeedReturnFocus = false;
            }            
        }

        #region Binding Property
        //All Widgets except group
        public IEnumerable<BaseWidgetItem> AllWidgetItems
        {
            get
            {
                List<BaseWidgetItem> selectedItems = new List<BaseWidgetItem>();
                foreach (ContentPresenter item in this.Children)
                {
                    if (VisualTreeHelper.GetChildrenCount(VisualTreeHelper.GetChild(item, 0)) <= 0)
                    {
                        continue;
                    }

                    BaseWidgetItem wdg = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(item, 0), 0) as BaseWidgetItem;
                    //BaseWidgetItem wdg= item.Content as BaseWidgetItem;
                    if (wdg != null && wdg.IsGroup == false)
                    {
                        selectedItems.Add(wdg);
                    }
                }
                return selectedItems;
            }
        }
        //Selected Widgets
        public IEnumerable<BaseWidgetItem> SelectedItems
        {
            get
            {
                //var selectedItems = from item in this.Children.OfType<BaseWidgetItem>()
                //                    where item.IsSelected == true
                //                    select item;

                List<BaseWidgetItem> selectedItems = new List<BaseWidgetItem>();
                foreach (ContentPresenter item in this.Children)
                {
                    if (VisualTreeHelper.GetChildrenCount(VisualTreeHelper.GetChild(item, 0)) <= 0)
                    {
                        continue;
                    }

                    BaseWidgetItem wdg = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(item, 0), 0) as BaseWidgetItem;
                    //BaseWidgetItem wdg= item.Content as BaseWidgetItem;
                    if (wdg != null && wdg.IsSelected == true && wdg.IsGroup == false)
                    {
                        selectedItems.Add(wdg);
                    }
                }
                return selectedItems;
            }
        }
        //Selected Widgets and Groups
        public IEnumerable<BaseWidgetItem> SelectedItemandGroups
        {
            get
            {
                //var selectedItems = from item in this.Children.OfType<BaseWidgetItem>()
                //                    where item.IsSelected == true
                //                    select item;

                List<BaseWidgetItem> selectedItems = new List<BaseWidgetItem>();
                foreach (ContentPresenter item in this.Children)
                {
                    BaseWidgetItem wdg = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(item, 0), 0) as BaseWidgetItem;
                    //BaseWidgetItem wdg= item.Content as BaseWidgetItem;
                    if (wdg != null && wdg.IsSelected == true )
                    {
                        selectedItems.Add(wdg);
                    }
                }
                return selectedItems;
            }
        }        
        //Selected Groups
        public IEnumerable<BaseWidgetItem> AllGroupChildItems
        {
            get
            {
                //var selectedItems = from item in this.Children.OfType<BaseWidgetItem>()
                //                    where item.IsSelected == true
                //                    select item;

                List<BaseWidgetItem> selectedItems = new List<BaseWidgetItem>();
                foreach (ContentPresenter item in this.Children)
                {
                    BaseWidgetItem wdg = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(item, 0), 0) as BaseWidgetItem;
                    //BaseWidgetItem wdg= item.Content as BaseWidgetItem;
                    if (wdg != null && wdg.ParentID != Guid.Empty)
                    {
                        selectedItems.Add(wdg);
                    }
                }
                return selectedItems;
            }
        }
        //Selected Widgets and Children
        public IEnumerable<BaseWidgetItem> SelectedItemAndChildren
        {
            get
            {
                List<BaseWidgetItem> selectedItems = new List<BaseWidgetItem>();
                List<GroupViewModel> selectedGroups = new List<GroupViewModel>();
                foreach (ContentPresenter item in this.Children)
                {
                    if (VisualTreeHelper.GetChildrenCount(VisualTreeHelper.GetChild(item, 0)) <= 0)
                    {
                        continue;
                    }
                    BaseWidgetItem wdg = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(item, 0), 0) as BaseWidgetItem;
                    if (wdg != null && wdg.IsSelected == true && wdg.IsGroup == false)
                    {
                        selectedItems.Add(wdg);
                    }
                    else if(wdg != null && wdg.IsSelected == true && wdg.IsGroup == true)
                    {
                        GroupViewModel gVM= wdg.DataContext as GroupViewModel;
                        if (gVM!=null)
                        {
                            selectedGroups.Add(gVM);
                        }   
                    }
                }

                if (selectedGroups.Count > 0)
                {
                    foreach(ContentPresenter item in this.Children)
                    {
                        if (VisualTreeHelper.GetChildrenCount(VisualTreeHelper.GetChild(item, 0)) <= 0)
                        {
                            continue;
                        }
                        BaseWidgetItem wdg = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(item, 0), 0) as BaseWidgetItem;
                        if (wdg != null && wdg.ParentID != Guid.Empty && wdg.IsGroup == false)
                        {
                            foreach(GroupViewModel gVM in selectedGroups)
                            {
                                if(true==gVM.IsChild(wdg.ParentID,true))
                                {
                                    selectedItems.Add(wdg);
                                    continue;
                                }
                            }
                        }
                    }
                }         
                return selectedItems;
            }
        }
        #endregion

        //#region Dependency Propery
        //public IInputElement EditorTarget
        //{
        //    get { return (IInputElement)GetValue(EditorTargetProperty); }
        //    set { SetValue(EditorTargetProperty, value); }
        //}

        //public static readonly DependencyProperty EditorTargetProperty =
        //  DependencyProperty.Register("EditorTarget", typeof(IInputElement),
        //                              typeof(DesignerCanvas),
        //                              new FrameworkPropertyMetadata(null));

        //#endregion

        #region Public Member
        public bool IsAnySelectedOjb 
        {
            get
            {
                bool Res = false;
                List<BaseWidgetItem> selectedItems = new List<BaseWidgetItem>();
                foreach (ContentPresenter item in this.Children)
                {
                    BaseWidgetItem wdg = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(item, 0), 0) as BaseWidgetItem;
                    //BaseWidgetItem wdg= item.Content as BaseWidgetItem;
                    if (wdg != null && wdg.IsSelected == true)
                    {
                        Res = true;
                        break;
                    }
                }
                return Res;
            }
        }
        public void DeselectAll()
        {
            ISelectionService _selectionSrv= ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            _selectionSrv.RemoveAllWidgets();
         

            foreach (BaseWidgetItem item in this.SelectedItems)
            {
                item.IsSelected = false;
            }
            IGroupOperation pageVM = DataContext as IGroupOperation;
            pageVM.DeselectAllGroups();
        }
        #endregion        

        #region event handler

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if ((e.Key == Key.F2 ||e.Key == Key.Space) 
                && Keyboard.Modifiers == ModifierKeys.None 
                && SelectedItems.Count() == 1)
            {
                BaseWidgetItem item = SelectedItems.First();

                if (item != e.Source && item.IsInEditModel != true)
                {
                    item.IsInEditModel = true;
                }
            }

            base.OnKeyUp(e);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
 	         base.OnMouseLeftButtonUp(e);
             IPagePropertyData page = DataContext as IPagePropertyData;
             if (page != null)
             {
                 page.CommonEventNotify("FormatPaint", true);
             }
        }
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            IPagePropertyData page = DataContext as IPagePropertyData;
            if (page != null)
            {
                page.CommonEventNotify("FormatPaint", false);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focusable = true;
            bool b = this.Focus();

            if (e.Source == this || (e.Source as BaseWidgetItem) == null)
            {
                //drag to select widget only left mouse
                if(e.LeftButton == MouseButtonState.Pressed)
                {
                    this.dragStartPoint = new Point?(e.GetPosition(this));
                }
                this.DeselectAll();
            }
            else if (e.ClickCount == 2 && (e.Source as BaseWidgetItem) != null)
            {
                //TODO:Here need more Improvement.
                foreach (BaseWidgetItem item in this.SelectedItems)
                {
                    if (item != e.Source && item.IsSelected == true)
                    {
                        item.IsSelected = false;
                    }                    
                }
                e.Handled = true;
            }

            //if (e.ClickCount == 1 )
            //{
            //    IPagePropertyData page = DataContext as IPagePropertyData;
            //    if (page != null)
            //    {
            //        if (e.LeftButton == MouseButtonState.Pressed)
            //        {
            //            page.CommonEventNotify("FormatPaint", true);
            //        }
            //        else
            //        {
            //            page.CommonEventNotify("FormatPaint", false);
            //        }

            //    }
            //}
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.dragStartPoint = null;
            }

            if (this.dragStartPoint.HasValue)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this, this.dragStartPoint);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }
                //e.Handled = true;
            }
        }
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (IsImageFile(files))
                {
                    ExcuteItemAdd(e);
                }
                else 
                {
                    foreach (string path in files)
                    {
                        if (CommonFunction.IsProjectFilePath(path))
                        {
                            ExcuteOpenProject(path);
                            break;
                        }
                    }
                }

                bool b=this.Focus();
                e.Handled = true;
            }
            else if (e.Data.GetDataPresent("DESIGNER_ITEM")
                || e.Data.GetDataPresent("SVG_ITEM")
                || e.Data.GetDataPresent("CUSTOM_ITEM")
                || e.Data.GetDataPresent("MASTER_ITEM"))
            {
                ExcuteItemAdd(e);

                bool b = this.Focus();
                e.Handled = true;
            }
        }

        private bool IsImageFile(string[] files)
        {
            foreach (string path in files)
            {
               if (CommonFunction.IsImageFilePath(path))
               {
                   return true;
               }
            }
            return false;
        }

        private void ExcuteItemAdd(DragEventArgs e)
        {
            Point position = e.GetPosition(this);
            Snap(ref position);
            DropInfo parameter = new DropInfo();
            parameter.e = e;
            parameter.position = position;
            this.DeselectAll();
            (DataContext as EditPaneViewModelBase).AddItemCommand.Execute(parameter);
        }

        private void ExcuteOpenProject(string path)
        {
            IEventAggregator listEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            if (listEventAggregator != null)
            {
                listEventAggregator.GetEvent<OpenFileEvent>().Publish(path);
            }
        }
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            //this.Focus();
        }

        /// <summary>
        /// Snap to grid when create widgets.
        /// </summary>
        /// <param name="leftTop"></param>
        void Snap(ref Point leftTop)
        {
            if (!GlobalData.IsSnapToGrid)
                return;

            double xSnap = leftTop.X % GlobalData.GRID_SIZE;
            double ySnap = leftTop.Y % GlobalData.GRID_SIZE;

            if (xSnap < GlobalData.GRID_SIZE / 2)
                xSnap = -xSnap;
            else xSnap = GlobalData.GRID_SIZE - xSnap;

            if (ySnap < GlobalData.GRID_SIZE / 2)
                ySnap = -ySnap;
            else ySnap = GlobalData.GRID_SIZE - ySnap;

            leftTop.X += xSnap;
            leftTop.Y += ySnap;
        }
        #endregion

    }
}
