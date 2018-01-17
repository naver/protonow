using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using System.Diagnostics;
using Naver.Compass.Service;
using Microsoft.Practices.ServiceLocation;

namespace Naver.Compass.WidgetLibrary
{
    public class RotateThumb : Thumb
    {
        #region Constructor
        public RotateThumb()
        {
            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();

            _infoItems = new List<WidgetViewModBase>();
            DragDelta += new DragDeltaEventHandler(this.RotateThumb_DragDelta);
            DragStarted += new DragStartedEventHandler(this.RotateThumb_DragStarted);
            DragCompleted += new DragCompletedEventHandler(this.RotateThumb_DragCompleted);
            Loaded += RotateThumb_Loaded;
            
        }

        void RotateThumb_Loaded(object sender, RoutedEventArgs e)
        {
            this.designerItem = DataContext as BaseWidgetItem;

            LoadCursor();
        }
        #endregion

        #region private member
        private double initialAngle;
        private RotateTransform rotateTransform;
        private Vector startVector;
        private Point centerPoint;
        private BaseWidgetItem designerItem;
        private Canvas canvas;
        private List<WidgetViewModBase> _infoItems;
        private ISelectionService _selectionService;

        private double groupCenterX;
        private double groupCenterY;
        private double groupIntialAngle;
        private void LoadCursor()
        {
            try
            {       
                this.Cursor = CommonFunction.GetRotateCur();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Load custom rotate cursor error.\nError:" + ex.Message.ToString());
            }
        }
        #endregion


        #region Event Handler
        private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _infoItems.Clear();            
            if (this.designerItem == null )
            {
                return;
            }
            
            this.canvas = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)))as Canvas;
            if (this.canvas == null)
            {
                return;
            }
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(designerItem)) as ContentPresenter;
            this.centerPoint = wrapper.TranslatePoint(
                new Point(wrapper.ActualWidth * designerItem.RenderTransformOrigin.X,
                            wrapper.ActualHeight * designerItem.RenderTransformOrigin.Y),
                            this.canvas);

            //this.centerPoint = wrapper.TranslatePoint(
            //    new Point(wrapper.ActualWidth/2,
            //              wrapper.ActualHeight/2),
            //              this.canvas);

            Point startPoint = Mouse.GetPosition(this.canvas);
            this.startVector = Point.Subtract(startPoint, this.centerPoint);


            this.rotateTransform = designerItem.RenderTransform as RotateTransform;
            if (this.rotateTransform == null)
            {
                designerItem.RenderTransform = new RotateTransform(0);
                this.initialAngle = 0;
            }
            else
            {
                this.initialAngle = this.rotateTransform.Angle;
            }

            groupIntialAngle = 0;
   
            if(this.designerItem.IsGroup==true)
            {
                WidgetViewModBase group = this.designerItem.DataContext as WidgetViewModBase;
                groupCenterX = group.Left + group.ItemWidth / 2;
                groupCenterY = group.Top + group.ItemHeight / 2;
            }
        }
        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Initialize the selected widgets' context when first rotate
            if (_infoItems.Count <= 0)
            {
               
                WidgetViewModBase wdg = designerItem.DataContext as WidgetViewModBase;
                if (wdg.IsGroup ==false)
                {
                    wdg.CreateNewPropertyMementos();
                    wdg.PropertyMementos.AddPropertyMemento(new PropertyMemento("RotateAngle", wdg.RotateAngle, wdg.RotateAngle));
                    _infoItems.Add(wdg);
                }
                else
                {
                    foreach(WidgetViewModBase item in (wdg as GroupViewModel).WidgetChildren)
                    {
                        item.CreateNewPropertyMementos();

                        item.CreateNewPropertyMementos();

                        item.PropertyMementos.AddPropertyMemento(new PropertyMemento("RotateAngle", item.RotateAngle, item.RotateAngle));
                        item.PropertyMementos.AddPropertyMemento(new PropertyMemento("Left", item.Raw_Left, item.Raw_Left));
                        item.PropertyMementos.AddPropertyMemento(new PropertyMemento("Top", item.Raw_Top, item.Raw_Top));
                        _infoItems.Add(item);
                    }                   
                }

   
            }

            //Rotate the current widget 
            if (this.designerItem != null && this.canvas != null)
            {
                ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(designerItem)) as ContentPresenter;

                Point currentPoint = Mouse.GetPosition(this.canvas);
                Vector deltaVector = Point.Subtract(currentPoint, this.centerPoint);

                double angle = Vector.AngleBetween(this.startVector, deltaVector);

                RotateTransform rotateTransform = designerItem.RenderTransform as RotateTransform;
                angle = this.initialAngle + Math.Round(angle, 0);

                

                if (angle < 0)
                {
                    angle += 360;
                }
                else if (angle >= 360)
                {
                    angle -= 360;
                }

                
                wrapper.InvalidateMeasure();

                if (this.designerItem.IsGroup == true)
                {
                    GroupViewModel group = this.designerItem.DataContext as GroupViewModel;

                    DesignerCanvas dc = canvas as DesignerCanvas;

                    foreach(WidgetViewModBase item in group.WidgetChildren)
                    {
                        RotateTransform rt = new RotateTransform(); // bw.RenderTransform as RotateTransform;
                        rt.Angle = angle - groupIntialAngle;
                        rt.CenterX = groupCenterX;
                        rt.CenterY = groupCenterY;

                        double oldAngle = item.RotateAngle;
                        item.RotateAngle = Convert.ToInt16(angle - groupIntialAngle);
                        Rect rect1 = rt.TransformBounds(new Rect(item.Left, item.Top, item.ItemWidth, item.ItemHeight));
                        Rect rect2 = item.RevertBoundingRectangle(rect1);

                        //item.IsActual = true;
                        item.Raw_Left = rect2.Left;
                        item.Raw_Top = rect2.Top;

                        //only widget can rotate
                        if(item is WidgetRotateViewModBase)
                        {
                            int newAngle = Convert.ToInt16(oldAngle + (angle - groupIntialAngle)) % 360;
                            if (newAngle < 0)
                            {
                                newAngle += 360;
                            }
                            else if (angle >= 360)
                            {
                                newAngle -= 360;
                            }
                            item.RotateAngle = newAngle;
                        }
                        else
                        {
                            item.RotateAngle = 0;
                        }
                        
                    }

                    group.Refresh();
                    groupIntialAngle = angle;
                }
                else
                {
                    rotateTransform.Angle = angle;
                }


                if (this.designerItem.ParentID != Guid.Empty )
                {
                    IGroupOperation pageVM = canvas.DataContext as IGroupOperation;
                    pageVM.UpdateGroup(this.designerItem.ParentID);
                }

                if (this.designerItem.IsGroup)
                {
                    IGroupOperation pageVM = canvas.DataContext as IGroupOperation;
                    pageVM.UpdateGroup((this.designerItem.DataContext as GroupViewModel).WidgetID);
                }
            }
        }
        private void RotateThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (_infoItems.Count <= 0)
            {
                return;
            }

            // Undo/Redo
            ISupportUndo pageVMUndo = canvas.DataContext as ISupportUndo;
            IGroupOperation pageVMGroup = canvas.DataContext as IGroupOperation;
            if (pageVMUndo == null)
            {
                return;
            }

            CompositeCommand cmds = new CompositeCommand();

            // Create undoable command for widgets
            foreach (WidgetViewModBase item in _infoItems)
            {
                item.PropertyMementos.SetPropertyNewValue("RotateAngle", item.RotateAngle);

                if(this.designerItem.IsGroup)
                {
                    item.PropertyMementos.SetPropertyNewValue("Left", item.Raw_Left);
                    item.PropertyMementos.SetPropertyNewValue("Top", item.Raw_Top);
                }
                PropertyChangeCommand cmd = new PropertyChangeCommand(item, item.PropertyMementos);
                cmds.AddCommand(cmd);
            }

            // Create undoable command for groups
            if (pageVMGroup != null)
            {
                List<Guid> groupGuids = new List<Guid>();

                if (designerItem.ParentID != Guid.Empty)
                {
                    groupGuids.Add(designerItem.ParentID);

                    UpdateGroupCommand cmd = new UpdateGroupCommand(pageVMGroup, groupGuids);
                    cmds.AddCommand(cmd);
                }

                if (this.designerItem.IsGroup)
                {
                    groupGuids.Add((designerItem.DataContext as GroupViewModel).WidgetID);

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


    }
}
