using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public enum DecoChromeType
    {   
        RounedRotateDecorator,
        RotateDecorator,
        NoRotateDecorator,
        VlineDecorator,
        HlineDecorator,
        HorRiszeDecorator,
        MasterDecorator
    };

    public class SelectedbaseItem : ContentControl
    {
        public SelectedbaseItem()
        {
            IsAfterDraged = false;
        }

        #region Dependency Propery
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        //Is used as a target object when select.
        public bool IsTarget
        {
            get { return (bool)GetValue(IsTargetProperty); }
            set { SetValue(IsTargetProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected", typeof(bool),
                                      typeof(SelectedbaseItem),
                                      new FrameworkPropertyMetadata(false));


        public static readonly DependencyProperty IsTargetProperty =
          DependencyProperty.Register("IsTarget", typeof(bool),
                                      typeof(SelectedbaseItem),
                                      new FrameworkPropertyMetadata(false));
        public bool IsInEditModel
        {
            get { return (bool)GetValue(IsInEditModelProperty); }
            set { SetValue(IsInEditModelProperty, value); }
        }
        public static readonly DependencyProperty IsInEditModelProperty =
          DependencyProperty.Register("IsInEditModel", typeof(bool),
                                      typeof(SelectedbaseItem),
                                      new FrameworkPropertyMetadata(false));

        public bool? IsStyleBrushModel
        {
            get { return (bool?)GetValue(IsStyleBrushModelProperty); }
            set { SetValue(IsStyleBrushModelProperty, value); }
        }
        public static readonly DependencyProperty IsStyleBrushModelProperty =
          DependencyProperty.Register("IsStyleBrushModel", typeof(bool?),
                                      typeof(SelectedbaseItem),
                                      new FrameworkPropertyMetadata(false));

        public Guid ParentID
        {
            get { return (Guid)GetValue(ParentIDProperty); }
            set { SetValue(ParentIDProperty, value); }
        }
        public static readonly DependencyProperty ParentIDProperty =
            DependencyProperty.Register("ParentID", typeof(Guid), typeof(SelectedbaseItem), new FrameworkPropertyMetadata(Guid.Empty));

        public bool IsGroup
        {
            get { return (bool)GetValue(IsGroupProperty); }
            set { SetValue(IsGroupProperty, value); }
        }
        public static readonly DependencyProperty IsGroupProperty =
            DependencyProperty.Register("IsGroup", typeof(bool), typeof(SelectedbaseItem), new FrameworkPropertyMetadata(false));

        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        public static readonly DependencyProperty IsLockedProperty =
          DependencyProperty.Register("IsLocked", typeof(bool),
                                      typeof(SelectedbaseItem),
                                      new FrameworkPropertyMetadata(false));

        public bool IsFixed
        {
            get { return (bool)GetValue(IsFixedProperty); }
            set { SetValue(IsFixedProperty, value); }
        }

        public static readonly DependencyProperty IsFixedProperty =
          DependencyProperty.Register("IsFixed", typeof(bool),
                                      typeof(SelectedbaseItem),
                                      new FrameworkPropertyMetadata(false));
        #endregion

        //protected override void OnMouseUp(MouseButtonEventArgs e)
        //{
        //    base.OnMouseUp(e);
        //}
        //protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseLeftButtonUp(e);
        //}

        #region Public function called bye Move Thumb
        public void OnGroupChildMouseUp()
        {
            DesignerCanvas designer = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this))) as DesignerCanvas;
            if (designer == null)
            {
                return;
            }
            //e.Handled = false;
            IGroupOperation pageVM = designer.DataContext as IGroupOperation;
            GroupStatus groupStatus = pageVM.GetGroupStatus(ParentID);
            if (ClickInitialStatus == GroupStatus.UnSelect && groupStatus == GroupStatus.Selected)
            {
                return;
            }
            
            //this is option for the group's child widget
            if (groupStatus == GroupStatus.UnSelect)
            {
                //if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                //{
                //    pageVM.SetGroupStatus(ParentID, GroupStatus.Selected);
                //}
                //else
                //{
                //    designer.DeselectAll();
                //    pageVM.DeselectAllGroups();
                //    pageVM.SetGroupStatus(ParentID, GroupStatus.Selected);
                //}
            }
            else if (groupStatus == GroupStatus.Selected)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    pageVM.SetGroupStatus(ParentID, GroupStatus.UnSelect);
                }
                else
                {
                    designer.DeselectAll();
                    pageVM.DeselectAllGroups();
                    pageVM.SetGroupStatus(ParentID, GroupStatus.Edit);
                    this.IsSelected = true;
                }
            }
            else
            {
                //this is option for the common widget
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    return;
                }
                else
                {
                    if (_isSelectedDone == false)
                    {
                        if (IsSelected == true
                            && designer.SelectedItems.Count() == 1)
                        {
                            return;
                        }
                        pageVM.DeselectAllChildren(ParentID);
                        this.IsSelected = true;
                    }
                }  
            }
   
        }
        public void OnPageChildMouseUp()
        {
            DesignerCanvas designer = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this))) as DesignerCanvas;
            //object cc = VisualTreeHelper.GetParent(this);
            if (designer == null||IsInEditModel==true)
            {
                return;
            }          

            IGroupOperation pageVM = designer.DataContext as IGroupOperation;
            //this is option for the common widget
            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
            {
                return;
            }
            else
            {
                if (_isSelectedDone == false)
                {
                    if(IsSelected==true
                        && designer.SelectedItems.Count() == 1)
                    {
                        return;
                    }
                    designer.DeselectAll();
                    pageVM.DeselectAllGroups();
                    this.IsSelected = true;
                }
                
            }  

        }
        public void SelectCurrentWidget()
        {
            _isSelectedDone = false;
            

            if (IsInEditModel == false)
            {
                Focus();
            }
            //return;
            

            BaseWidgetItem wdg = this as  BaseWidgetItem;
            if (wdg.ParentID == Guid.Empty)
            {
                ClickPageWidget();
            }
            else
            {
                DesignerCanvas designer = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this))) as DesignerCanvas;
                IGroupOperation pageVM = designer.DataContext as IGroupOperation;
                GroupStatus groupStatus = pageVM.GetGroupStatus(this.ParentID);
                if (groupStatus == GroupStatus.UnSelect)
                {
                    ClickGroupChildInUnselected();
                }
                else if (groupStatus == GroupStatus.Selected)
                {
                    ClickGroupChildInSelected();
                }
                else if (groupStatus == GroupStatus.Edit)
                {
                    ClickGroupChildInEdited();
                }
            }

        }
        public bool IsAfterDraged
        { get; set; }
        #endregion

        #region Public Function
        public Rect GetBoundingRect()
        {
            WidgetViewModBase VM = DataContext as WidgetViewModBase;

            if (VM != null)
            {
                return VM.GetBoundingRectangle();
            }
            return new Rect(-1, -1, 0, 0);
        }
        #endregion

        #region Mouse Event Handler
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            e.Handled = false;
            BaseWidgetItem wdg = e.Source as BaseWidgetItem;
            if (wdg == null || wdg !=this)
            {
                return;
            }
            if(IsAfterDraged==false)
            {
                SelectCurrentWidget();
            }            
            IsAfterDraged = false;
        }
        protected  void OnPreviewMouseDown2222(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            _isSelectedDone = false;
            e.Handled = false;

            if (IsInEditModel == false)
            {
                Focus();
            }
            //return;

            BaseWidgetItem wdg = e.Source as BaseWidgetItem;
            if (wdg == null)
            {
                return;
            }
            
            if (wdg.ParentID == Guid.Empty)
            {
                ClickPageWidget();
            }
            else
            {
                DesignerCanvas designer = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this))) as DesignerCanvas;
                IGroupOperation pageVM = designer.DataContext as IGroupOperation;
                GroupStatus groupStatus = pageVM.GetGroupStatus(this.ParentID);
                if (groupStatus == GroupStatus.UnSelect)
                {
                    ClickGroupChildInUnselected();
                }
                else if (groupStatus == GroupStatus.Selected)
                {
                    ClickGroupChildInSelected();
                }
                else if (groupStatus == GroupStatus.Edit)
                {
                    ClickGroupChildInEdited();
                }
            }          
        }
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            //BaseWidgetItem wdg = e.Source as BaseWidgetItem;
            //if (wdg == null)
            //{
            //    e.Handled = false;
            //    return;
            //}



            //e.Handled = false;
            //if (wdg.ParentID == Guid.Empty)
            //{
            //    ClickPageWidget();
            //}
            //else
            //{
            //    DesignerCanvas designer = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this)) as DesignerCanvas;
            //    IGroupOperation pageVM = designer.DataContext as IGroupOperation;
            //    GroupStatus groupStatus = pageVM.GetGroupStatus(this.ParentID);
            //    if (groupStatus == GroupStatus.UnSelect)
            //    {
            //        ClickGroupChildInUnselected();
            //    }
            //    else if (groupStatus == GroupStatus.Selected)
            //    {
            //        ClickGroupChildInSelected();
            //    }
            //    else if (groupStatus == GroupStatus.Edit)
            //    {
            //        ClickGroupChildInEdited();
            //    }
            //}          

        }
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);            
            IsInEditModel = true;
            if (this.IsGroup != true)
            {
                IsSelected = true;
                DesignerCanvas designer = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this))) as DesignerCanvas;
                IGroupOperation pageVM = designer.DataContext as IGroupOperation;
                if (this.ParentID != Guid.Empty)
                {                    
                    if (pageVM != null)
                    {
                        pageVM.SetGroupStatus(ParentID, GroupStatus.Edit);
                    }
                }
                else
                {
                    if (pageVM != null)
                    {
                        pageVM.DeselectAllGroups();
                    }
                }
            }
            //Event should can continue it's route to let canvas DeSelect all other widgets selected
            e.Handled = false;
        }        
        #endregion Mouse Event Handler       

        #region Private functions and properties.
        GroupStatus ClickInitialStatus;
        private bool _isSelectedDone;
        private void ClickPageWidget()
        {
            DesignerCanvas designer = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this))) as DesignerCanvas;
            //object cc = VisualTreeHelper.GetParent(this);
            if (designer == null)
            {
                return;
            }            

            IGroupOperation pageVM = designer.DataContext as IGroupOperation;

            //this is option for the common widget
            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
            {
                int count = designer.SelectedItems.Count();

                foreach (BaseWidgetItem item in designer.SelectedItems)
                {                   
                    if (item.IsSelected == true &&
                        item.ParentID != Guid.Empty)
                    {
                        return;
                    }
                    if (count == 1)
                    {
                        item.IsTarget = true;
                    }
                }
              
                this.IsSelected = !this.IsSelected;
                if (count == 0 && IsSelected)
                {
                    this.IsTarget = true;
                }
            }
            else
            {
                if (!this.IsSelected)
                {
                    designer.DeselectAll();
                    pageVM.DeselectAllGroups();
                    this.IsSelected = true;
                    _isSelectedDone = true;
                }
            }  
        }
        private void ClickGroupChildInUnselected()
        {
            ClickInitialStatus = GroupStatus.UnSelect;
            DesignerCanvas designer = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this))) as DesignerCanvas;
            IGroupOperation pageVM = designer.DataContext as IGroupOperation;
            //this is option for the common widget
            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
            {
                foreach (BaseWidgetItem item in designer.SelectedItems)
                {
                    if (item.IsSelected == true &&
                        item.ParentID != Guid.Empty)
                    {
                        return;
                    }
                }
                pageVM.SetGroupStatus(ParentID, GroupStatus.Selected);
            }
            else
            {
                designer.DeselectAll();
                pageVM.DeselectAllGroups();
                pageVM.SetGroupStatus(ParentID, GroupStatus.Selected);
            }
        }
        private void ClickGroupChildInSelected()
        {
            ClickInitialStatus = GroupStatus.Selected;
            //DesignerCanvas designer = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this)) as DesignerCanvas;
            if (IsLocked == true)
            {
                OnGroupChildMouseUp();
            }
        }
        private void ClickGroupChildInEdited()
        {
            ClickInitialStatus = GroupStatus.Edit;
            DesignerCanvas designer = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this))) as DesignerCanvas;
            IGroupOperation pageVM = designer.DataContext as IGroupOperation;
            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
            {
                this.IsSelected = !this.IsSelected;
            }
            else
            {
                if (!this.IsSelected)
                {
                    pageVM.DeselectAllChildren(ParentID);
                    this.IsSelected = true;
                    _isSelectedDone = true;
                }
            }  
        }  
        #endregion
    }
    public class BaseWidgetItem : SelectedbaseItem
    {

        public BaseWidgetItem()
        {
            CanRotate = true;
            DecoChrome = DecoChromeType.RotateDecorator;
        }
        static BaseWidgetItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseWidgetItem), new FrameworkPropertyMetadata(typeof(BaseWidgetItem)));
        }


        public bool CanRotate
        {
            get;
            set;
        }
        public DecoChromeType DecoChrome
        {
            get;
            set;
        }
        public Cursor CopyCur
        {
            get
            {
                return CommonFunction.GetCopytCur();
            }
        }

    }
}
