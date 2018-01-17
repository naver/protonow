using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Linq;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using System.Diagnostics;
using Naver.Compass.Service;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;

namespace Naver.Compass.WidgetLibrary
{
    public class MoveThumb : Thumb
    {
        #region Constructor
        public MoveThumb()
        {
            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            _infoItems = new List<WidgetViewModBase>();
            _groups = new List<GroupViewModel>();
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }
        #endregion

        #region private member
        private RotateTransform rotateTransform;
        private BaseWidgetItem designerItem;
        private DesignerCanvas designerCanvas;
        private bool bIsMousePressMove = false;
        private bool bIsMouseClicked = false;
        private PreveiwMoveAdorner adorner = null;

        //Redo/Undo/Selected Widget information item
        private List<WidgetViewModBase> _infoItems;
        private List<GroupViewModel> _groups;
        private IPage _page;
        private ISelectionService _selectionService;

        //Total Vertical and Horizon offset
        private double _totalHorizonOffset = 0;
        private double _totaVerticalOffset = 0;

        //Keep offset when press shift key
        private double _shiftHorizonOffset = 0;
        private double _shiftVerticalOffset = 0;

        private IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }
        #endregion
        
        #region Event Handler
        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as BaseWidgetItem;
            _infoItems.Clear();
            _groups.Clear();
            bIsMousePressMove = false;
            bIsMouseClicked = true;
            _totalHorizonOffset = 0;
            _totaVerticalOffset = 0;

            if (this.designerItem != null)
            {
                this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                this.designerCanvas = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem))) as DesignerCanvas;
                _page = (designerCanvas.DataContext as IPagePropertyData).ActivePage;
            }
        }
        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Initialize to avoid exception    
            if (this.designerItem == null || this.designerCanvas == null)
            {
                return;
            }
            

            //Initialize the selected widgets' context when first move
            if (bIsMousePressMove==false)
            {
                designerItem.IsAfterDraged = true;
                if(designerItem.IsSelected==false)
                {
                    designerItem.SelectCurrentWidget();                    
                }

                if (Keyboard.IsKeyDown(Key.LeftCtrl) == true || Keyboard.IsKeyDown(Key.RightCtrl) == true)
                {
                    IsCtrlPressed = true;
                }
                else
                {
                    IsCtrlPressed = false;
                }

                if (IsCtrlPressed == true && Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    //Cursor = CopyCur;
                    //designerItem.IsStyleBrushModel = null;
                    if (designerItem == null)
                        return;
                    WidgetViewModBase wVM = designerItem.DataContext as WidgetViewModBase;
                    wVM.IsBrushModel = null;
                }

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(designerCanvas);
                if (adornerLayer != null)
                {
                    adorner = new PreveiwMoveAdorner(designerCanvas);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }

                IPagePropertyData Page=designerCanvas.DataContext as IPagePropertyData;    
                foreach (WidgetViewModBase item in Page.GetSelectedwidgets())
                {
                    if (item.IsLocked == true)
                        continue;

                    if (item.IsGroup == true)
                    {
                        GroupViewModel group = item as GroupViewModel;
                        if (group == null)
                        {
                            continue;
                        } 

                        // Create a property memento when drag start
                        foreach (WidgetViewModBase child in group.WidgetChildren)
                        {
                            child.CreateNewPropertyMementos();
                            child.PropertyMementos.AddPropertyMemento(new PropertyMemento("Left", child.Raw_Left, child.Raw_Left));
                            child.PropertyMementos.AddPropertyMemento(new PropertyMemento("Top", child.Raw_Top, child.Raw_Top));
                                                        
                            _infoItems.Add(child);
                        }
                        _groups.Add(group);
                    }
                    else
                    {
                        item.CreateNewPropertyMementos();
                       
                        item.PropertyMementos.AddPropertyMemento(new PropertyMemento("Left", item.Raw_Left, item.Raw_Left));
                        item.PropertyMementos.AddPropertyMemento(new PropertyMemento("Top", item.Raw_Top, item.Raw_Top));
                        
                        if (item.WidgetType == WidgetType.Toast && item.Top == 0)
                        {
                            item.PropertyMementos.AddPropertyMemento(new PropertyMemento("DisplayPosition", ToastDisplayPosition.Top, ToastDisplayPosition.Top));
                        }
                        _infoItems.Add(item);
                    }                       
                }
                bIsMousePressMove = true;
            }

            SendFirePositionEvent(false);

            //Move adorner to show drag process
            MoveAdorner(sender,e);

            //Move Widgets, the older solution, discarded
            //if (_infoItems.Count > 0 || _groups.Count > 0)
            //{
            //    MoveSelectedWidgets(sender,e);
            //}            

            //set routed event is handled
            e.Handled = true;
        }
        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            e.Handled = true;
        }
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            bIsMouseClicked = false;
            if (adorner != null)
            {
                AdornerLayer adornerLayer = adorner.Parent as AdornerLayer;
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(adorner);
                }
            }
            if (designerItem == null)
            {
                return;
            }

            e.Handled = false;
            
            //Cursor = Cursors.SizeAll;
            //designerItem.IsStyleBrushModel = false;
            if (designerItem == null)
                return;
            WidgetViewModBase wVM = designerItem.DataContext as WidgetViewModBase;
            if (wVM == null)
                return;
            wVM.IsBrushModel = false;

            if (bIsMousePressMove == false) //click
            {
                MouseClickEnd(e);
            }
            else//move
            {
                MouseDragEnd(e);
            }
            //IsCtrlPressed = false;
        }

        private void MouseClickEnd(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (designerItem.ParentID == Guid.Empty)//common widget
            {
                e.Handled = false;
                designerItem.OnPageChildMouseUp();
                bIsMousePressMove = false;
            }
            else//group children
            {
                e.Handled = false;
                designerItem.OnGroupChildMouseUp();
                bIsMousePressMove = false;
            }

            #region handle Hamburger Menu Eidtor
            //shift/control key down, close hamburger menu  edit UI
            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
            {
                _selectionService.GetCurrentPage().CancelEditHamburgerPage();
            }
            else
            {
                //click hamburger, open edit hamburger menu  edit UI.
                WidgetViewModBase widgetVM = designerItem.DataContext as WidgetViewModBase;
                if (widgetVM != null && widgetVM.Type == ObjectType.HamburgerMenu)
                {
                    _selectionService.GetCurrentPage().EditHanburgerPage();
                }
            }
            #endregion
            return;
        }
        
        private void MouseDragEnd(System.Windows.Input.MouseButtonEventArgs e)
        {
            bIsMousePressMove = false;

            if (IsCtrlPressed==false)
            {
                if (_infoItems.Count <= 0)
                {
                    return;
                }
                MoveSeleteedWidgets();
                PushUndoStack();
            }
            else
            {
                IPagePropertyData Page = designerCanvas.DataContext as IPagePropertyData;
                Page.DuplicateWidgets(new Point(_totalHorizonOffset, _totaVerticalOffset));
            }

            
        }
        
        private void PushUndoStack()
        {
            // Undo/Redo
            ISupportUndo pageVMUndo = designerCanvas.DataContext as ISupportUndo;
            IGroupOperation pageVMGroup = designerCanvas.DataContext as IGroupOperation;
            if (pageVMUndo == null)
            {
                return;
            }

            CompositeCommand cmds = new CompositeCommand();

            IPagePropertyData Page = designerCanvas.DataContext as IPagePropertyData;
            bool bHasGroup = Page.GetSelectedwidgets().Any(a => a.IsGroup);

            // Create undoable command for widgets
            foreach (WidgetViewModBase item in _infoItems)
            {
                
                item.PropertyMementos.SetPropertyNewValue("Left", item.Raw_Left);
                item.PropertyMementos.SetPropertyNewValue("Top", item.Raw_Top);
               
                if (item.WidgetType == WidgetType.Toast && item.Top != 0)
                {
                    item.PropertyMementos.SetPropertyNewValue("DisplayPosition", ToastDisplayPosition.UserSetting);
                }

                PropertyChangeCommand cmd = new PropertyChangeCommand(item, item.PropertyMementos);
                cmds.AddCommand(cmd);
            }

            // Create undoable command for groups
            if (pageVMGroup != null)
            {

                List<Guid> groupGuids = _groups.Select(x => x.WidgetID).ToList();

                if (designerItem.ParentID != Guid.Empty)
                {
                    groupGuids.Add(designerItem.ParentID);
                }

                if (groupGuids.Count > 0)
                {
                    UpdateGroupCommand cmd = new UpdateGroupCommand(pageVMGroup, groupGuids);
                    cmds.AddCommand(cmd);
                }
            }

            // Push to undo stack
            if (cmds.Count > 0)
            {
                List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();
                cmds.AddCommand(new SelectCommand(pageVMGroup, allSelects));

                cmds.DeselectAllWidgetsFirst();
                pageVMUndo.UndoManager.Push(cmds);
            }
        }
        #endregion

        #region Private Functions
        private void MoveAdorner(object sender, DragDeltaEventArgs e)
        {
            //Get X/Y  Move Offset
            double deltaHorizontal = 0;
            double deltaVertical = 0;
            GetMoveOffset(e, ref deltaHorizontal, ref deltaVertical);

            if (adorner != null)
            {
                adorner.PreviewMove(deltaHorizontal, deltaVertical);
            }

            _totalHorizonOffset = deltaHorizontal;
            _totaVerticalOffset = deltaVertical;
            Debug.WriteLine("---->Move:"+deltaHorizontal);
        }
        //New Solution, Called only one time after adorner moving is over
        private void MoveSeleteedWidgets()
        {
            if (this.designerItem.ParentID == Guid.Empty)
            {
                MoveExternalElements(_totalHorizonOffset, _totaVerticalOffset);
            }
            else
            {
                IGroupOperation pageVM = designerCanvas.DataContext as IGroupOperation;
                GroupStatus groupStatus = pageVM.GetGroupStatus(this.designerItem.ParentID);
                if (groupStatus == GroupStatus.Edit)
                {
                    MoveEditedGroup(pageVM, _totalHorizonOffset, _totaVerticalOffset);
                }
                else if (groupStatus == GroupStatus.Selected)
                {
                    MoveSelectedGroup(pageVM, _totalHorizonOffset, _totaVerticalOffset);
                }
            }
        }
        //Old Solution, Called many times in widgets moving process        
        private void MoveSelectedWidgets(object sender, DragDeltaEventArgs e){
            //Get X/Y  Move Offset
            double deltaHorizontal = 0;
            double deltaVertical = 0;
            GetMoveOffset(e, ref deltaHorizontal, ref deltaVertical);

            if (this.designerItem.ParentID == Guid.Empty)
            {
                MoveExternalElements(deltaHorizontal, deltaVertical);
            }
            else
            {
                IGroupOperation pageVM = designerCanvas.DataContext as IGroupOperation;
                GroupStatus groupStatus = pageVM.GetGroupStatus(this.designerItem.ParentID);
                if (groupStatus == GroupStatus.Edit)
                {
                    MoveEditedGroup(pageVM, deltaHorizontal, deltaVertical);
                }
                else if (groupStatus == GroupStatus.Selected)
                {
                    MoveSelectedGroup(pageVM, deltaHorizontal, deltaVertical);
                }
            }
        }
        private void GetMoveOffset(DragDeltaEventArgs e, ref double xOffset, ref double yOffset)
        {
            System.Windows.Point dragDelta = new System.Windows.Point(e.HorizontalChange, e.VerticalChange);
            RotateTransform rotateTransform = designerItem.RenderTransform as RotateTransform;
            if (rotateTransform != null)
            {
                dragDelta = rotateTransform.Transform(dragDelta);
                //Rect rec = rotateTransform.TransformBounds(new Rect(designerItem.RenderSize));
                //rotateTransform.
            }


            xOffset = dragDelta.X;
            yOffset = dragDelta.Y;

            //If shift key down, move widget by straight line.
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
            {
                if (Math.Abs(xOffset) > Math.Abs(yOffset))
                {
                    yOffset = 0;
                }
                else
                {
                    xOffset = 0;
                }
            }

            IPagePropertyData page = designerCanvas.DataContext as IPagePropertyData;
            Snap(page.BoundingRect, ref xOffset, ref yOffset);
        }
        private void MoveExternalElements(double deltaHorizontal, double deltaVertical)
        {
            if (this.designerItem.IsSelected == false)
            {
                return;
            }

            foreach (WidgetViewModBase item in _infoItems)
            {
                if (item == _infoItems.Last())
                {
                    SendFirePositionEvent(true);
                }

                if (item.IsLocked==true)
                {
                    continue;
                }

                item.Raw_Top = item.Top + deltaVertical;;
                item.Raw_Left = item.Left + deltaHorizontal;
                
            }

            IGroupOperation pageVM = designerCanvas.DataContext as IGroupOperation;
            List<Guid> GroupGIDs = pageVM.GetAllSelectedGroups();
            if (GroupGIDs.Count <= 0)
            {
                return;
            }

            foreach (GroupViewModel it in _groups)
            {
                pageVM.UpdateGroup(it.WidgetID);
            }
            
        }                
        private void MoveEditedGroup(IGroupOperation pageVM, double deltaHorizontal, double deltaVertical)
        {
            if (this.designerItem.IsSelected == false)
            {
                return;
            }

            foreach (WidgetViewModBase item in _infoItems)
            {
                if (item == _infoItems.Last())
                {
                    SendFirePositionEvent(true);
                }

                if (item.IsLocked == true)
                {
                    continue;
                }

                item.Raw_Left = item.Left + deltaHorizontal;
                item.Raw_Top = item.Top + deltaVertical;
            }

            pageVM.UpdateGroup(this.designerItem.ParentID);
        } 
        private void MoveSelectedGroup(IGroupOperation pageVM, double deltaHorizontal, double deltaVertical)
        {
            if (_groups.Count <= 0)
            {
                this.designerCanvas.InvalidateMeasure();
                return;
            }

            //TODO:
            if (this.designerItem.IsSelected == true)
            {
                return;
            }

            foreach (WidgetViewModBase item in _infoItems)
            {
                if (item == _infoItems.Last())
                {
                    SendFirePositionEvent(true);
                }

                if (item.IsLocked == true)
                {
                    continue;
                }

                item.Raw_Left = item.Left + deltaHorizontal;
                item.Raw_Top = item.Top + deltaVertical;
            }

            foreach (GroupViewModel it in _groups)
            {
                pageVM.UpdateGroup(it.WidgetID);
            }

        }

        private void SendFirePositionEvent(bool enable)
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<EnableFirePositionInRibbonEvent>().Publish(enable);
        }
        private void Snap(Rect boundingRect, ref double deltaX, ref double deltaY)
        {
            if (boundingRect == new Rect(0, 0, 0, 0))
                return;
            Rect newRect = new Rect(boundingRect.Left + deltaX, boundingRect.Top + deltaY, boundingRect.Width, boundingRect.Height);
            double oriDeltaX = deltaX;
            double oriDeltaY = deltaY;
            if (GlobalData.IsSnapToGuide)
            {
                IPageView pageView = _page.PageViews.GetPageView(_selectionService.GetCurrentPage().CurAdaptiveViewGID);
                if (pageView == null)
                    return;
                foreach (IGuide item in pageView.Guides)
                {
                    if (deltaY == oriDeltaY && item.Orientation == Service.Document.Orientation.Horizontal)
                    {
                        Snap2Guide(item, newRect, boundingRect, ref deltaY);
                    }
                    if (deltaX == oriDeltaX && item.Orientation == Service.Document.Orientation.Vertical)
                    {
                        Snap2Guide(item, newRect, boundingRect, ref deltaX);
                    }
                    if (deltaX != oriDeltaX && deltaY != oriDeltaY)
                        return;
                }

                foreach (IGuide item in _document.GlobalGuides)
                {
                    if (deltaY == oriDeltaY && item.Orientation == Service.Document.Orientation.Horizontal)
                    {
                        Snap2Guide(item, newRect, boundingRect, ref deltaY);
                    }
                    if (deltaX == oriDeltaX && item.Orientation == Service.Document.Orientation.Vertical)
                    {
                        Snap2Guide(item, newRect, boundingRect, ref deltaX);
                    }
                    if (deltaX != oriDeltaX && deltaY != oriDeltaY)
                        return;
                }
            }

            //snap to grid.
            if (GlobalData.IsSnapToGrid)
            {
                //Horizontal
                if (deltaX == oriDeltaX)
                {
                    //snap to left
                    double xSnap = CalculateSnapDelta(newRect.Left, CommonDefine.SnapMargin);
                    if (xSnap != 0)
                    {
                        deltaX += xSnap;
                    }
                    else
                    {
                        //snap to right
                        xSnap = CalculateSnapDelta(newRect.Right, CommonDefine.SnapMargin);
                        if (xSnap != 0)
                        {
                            deltaX += xSnap;
                        }
                    }
                }
                //Vertical
                if (deltaY == oriDeltaY)
                {
                    //snap to top
                    double ySnap = CalculateSnapDelta(newRect.Top, CommonDefine.SnapMargin);
                    if (ySnap != 0)
                    {
                        deltaY += ySnap;
                    }
                    else
                    {
                        //snap to bottom
                        ySnap = CalculateSnapDelta(newRect.Bottom, CommonDefine.SnapMargin);
                        if (ySnap != 0)
                        {
                            deltaY += ySnap;
                        }
                    }
                }
            }
        }
        private void Snap2Guide(IGuide guide, Rect newRect, Rect boundingRect, ref double delta)
        {
            if (guide.Orientation == Service.Document.Orientation.Horizontal)
            {
                //snap top 
                if (Math.Abs(newRect.Top - guide.Y) < CommonDefine.SnapMargin)
                {
                    delta = guide.Y - boundingRect.Top;
                }//snap bottom 
                if (Math.Abs(newRect.Bottom - guide.Y) < CommonDefine.SnapMargin)
                {
                    delta = guide.Y - boundingRect.Bottom;
                }
            }
            else
            {
                //snap left 
                if (Math.Abs(newRect.Left - guide.X) < CommonDefine.SnapMargin)
                {
                    delta = guide.X - boundingRect.Left;
                }
                //snap right
                if (Math.Abs(newRect.Right - guide.X) < CommonDefine.SnapMargin)
                {
                    delta = guide.X - boundingRect.Right;
                }
            }
        }

        /// <summary>
        /// Calculate delta after snap to grid.
        /// </summary>
        /// <param name="snapValue">widget left/right/top/bottom</param>
        /// <param name="snapMargin">less than which to snap</param>
        /// <returns>delta</returns>
        private double CalculateSnapDelta(double targertPosition, double snapMargin)
        {
            double snapValue = targertPosition % GlobalData.GRID_SIZE;
            //snap left/top of grid,example: valueToSnap%GRID_SIZE -> 122%10
            if (snapValue <= snapMargin)
            {
                return -snapValue;
            }
            else
            {
                //example:128%10
                double temp = (targertPosition + snapMargin) % GlobalData.GRID_SIZE;
                if (temp < snapMargin)
                {
                    return GlobalData.GRID_SIZE - snapValue;
                }
            }
            return 0;
        }
        #endregion


        #region Dependency Property
        public bool IsCtrlPressed
        {
            get { return (bool)GetValue(IsCtrlPressedProperty); }
            set {SetValue(IsCtrlPressedProperty, value);}
        }
        public static readonly DependencyProperty IsCtrlPressedProperty =
          DependencyProperty.Register("IsCtrlPressed", typeof(bool),
                                      typeof(MoveThumb),
                                      new PropertyMetadata(false, OnIsCtrlPressedChanged));

        static void OnIsCtrlPressedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thumb = sender as MoveThumb;
            if (thumb != null)
            {
                thumb.OnIsCtrlPressedChanged();
            }
        }
        private void OnIsCtrlPressedChanged()
        {
            if (designerItem == null)
                return;

            WidgetViewModBase wVM = designerItem.DataContext as WidgetViewModBase;
            if (wVM == null)
                return;
            if (IsCtrlPressed == true && Mouse.LeftButton == MouseButtonState.Pressed && bIsMouseClicked == true)
            {
                wVM.IsBrushModel = null;
            }
            else
            {
                wVM.IsBrushModel = false;
            }
        }

        public bool IsShiftPressed
        {
            get { return (bool)GetValue(IsShiftPressedProperty); }
            set { SetValue(IsShiftPressedProperty, value); }
        }
        public static readonly DependencyProperty IsShiftPressedProperty =
          DependencyProperty.Register("IsShiftPressed", typeof(bool),
                                      typeof(MoveThumb),
                                      new PropertyMetadata(false, OnIsShiftPressedChanged));

        static void OnIsShiftPressedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thumb = sender as MoveThumb;
            if (thumb != null)
            {
                thumb.OnIsShiftPressedChanged();
            }
        }
        private void OnIsShiftPressedChanged()
        {
            if(IsShiftPressed)
            {
                if (_totalHorizonOffset > _totaVerticalOffset)
                {
                    _shiftVerticalOffset = _totaVerticalOffset;
                    _totaVerticalOffset = 0;
                }
                else
                {
                    _shiftHorizonOffset = _totalHorizonOffset;
                    _totalHorizonOffset = 0;
                }
            }
            else
            {
                if (_totalHorizonOffset == 0)
                {
                    _totalHorizonOffset = _shiftHorizonOffset;
                }
                if (_totaVerticalOffset == 0)
                {
                    _totaVerticalOffset = _shiftVerticalOffset;
                }
            }

            if (adorner != null)
            {
                adorner.PreviewMove(_totalHorizonOffset, _totaVerticalOffset);
            }
        }
        #endregion

        #region TODO:
        private double minLeft = double.MaxValue;
        private double minTop = double.MaxValue;
        private void MoveThumb_DragDelta1111(object sender, DragDeltaEventArgs e)
        {
            //Initialize
            if (bIsMousePressMove == false)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(designerCanvas);
                if (adornerLayer != null)
                {
                    adorner = new PreveiwMoveAdorner(designerCanvas);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }

                foreach (ContentControl item in this.designerCanvas.SelectedItemandGroups)
                {
                    ContentPresenter wrapper = VisualTreeHelper.GetParent(item) as ContentPresenter;
                    if (wrapper == null)
                        continue;
                    minLeft = Math.Min(Canvas.GetLeft(wrapper), minLeft);
                    minTop = Math.Min(Canvas.GetTop(wrapper), minTop);
                }

            }
            bIsMousePressMove = true;
            if (this.designerItem == null || this.designerCanvas == null)
            {
                return;
            }

            //Get x/y offset
            Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);
            RotateTransform rotateTransform = designerItem.RenderTransform as RotateTransform;
            if (rotateTransform != null)
            {
                dragDelta = rotateTransform.Transform(dragDelta);
            }

            double deltaHorizontal = Math.Max(-minLeft, dragDelta.X);
            double deltaVertical = Math.Max(-minTop, dragDelta.Y);

            if (adorner != null)
            {
                adorner.PreviewMove(deltaHorizontal, deltaVertical);
            }

            e.Handled = true;
        }        
        #endregion
    }
}
