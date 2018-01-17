using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Diagnostics;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Microsoft.Practices.ServiceLocation;

namespace Naver.Compass.Module
{
    public class MoveThumb : Thumb, INotifyPropertyChanged
    {
        public MoveThumb()
        {
            DragStarted += MoveThumb_DragStarted;
            DragDelta += MoveThumb_DragDelta;
            DragCompleted += new DragCompletedEventHandler(this.MoveThumb_DragCompleted);

            ISelectionService selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            ISupportUndo pageVM = selectionService.GetCurrentPage() as ISupportUndo;
            if (pageVM != null)
            {
                pageUndoManager = pageVM.UndoManager;
            }
        }

        #region private member
        private GuideItemBase guideItem;
        private Adorner adorner;
        private Canvas containerCanvas;
        private double scale;
        private double oldPos;
        private UndoManager pageUndoManager;
        #endregion


        #region binding propery
        public Thickness _inofLocation;
        public Thickness InofLocation
        {

            get { return _inofLocation; }
            set 
            {
                if (value != _inofLocation)
                {
                    _inofLocation = value;
                }
                _NotifyPropertyChanged("InofLocation");
            }
        }
        public string _infoGuide;
        public string InfoGuide
        {

            get { return _infoGuide; }
            set
            {
                if (value != _infoGuide)
                {
                    _infoGuide = value;
                }
                _NotifyPropertyChanged("InfoGuide");
            }
        }
        #endregion

        //public Thickness InofLocation 
        //    {

        //        get { return (Thickness)GetValue(InofLocationroperty); }
        //        set { SetValue(InofLocationroperty, value); }
        //    }

        //public static readonly DependencyProperty InofLocationroperty =
        //      DependencyProperty.Register("IsLocked", typeof(Thickness),
        //                                  typeof(MoveThumb),
        //                                  new FrameworkPropertyMetadata(new Thickness(0,0,0,-15)));

        #region INotifyPropertyChanged Members

        private void _NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {            
            this.guideItem = (DataContext as GuideLineDecorator).DataContext as GuideItemBase;

            //Set selection property
            containerCanvas = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(DataContext as GuideLineDecorator)) as Canvas;
            GuideBoxViewModel LineContianer = containerCanvas.DataContext as GuideBoxViewModel;
            scale = LineContianer.Scale;
            foreach (GuideItemBase item in LineContianer.GuideItems)
            {
                item.IsSelected = false;
            }
            guideItem.IsSelected = true;


            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
              if (adornerLayer != null)
              {
                  if (this.guideItem is HorizontalGuideLine)
                  {
                      this.adorner = new MousePositionAdorner(this,false);
                      InfoGuide = string.Format("y: {0}", Convert.ToInt16((this.guideItem as HorizontalGuideLine).ActualTop));
                      oldPos = guideItem.Guide.Y;
                  }
                  else
                  {
                      this.adorner = new MousePositionAdorner(this,true);
                      InfoGuide = string.Format("x: {0}", Convert.ToInt16((this.guideItem as VerticalGuideLine).ActualLeft));
                      oldPos = guideItem.Guide.X;
                  }                  
                  adornerLayer.Add(this.adorner);
                  
              }

        }
        void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.guideItem == null)
                return;
            if (guideItem.IsLocked == true)
                return;

            double minLeft = double.MaxValue;
            double minTop = double.MaxValue;

            double xOffset, yOffset;
            Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);


            xOffset = Math.Max(-minLeft, dragDelta.X);
            yOffset = Math.Max(-minTop, dragDelta.Y);

            if(this.guideItem is HorizontalGuideLine)
            {
                (this.guideItem as HorizontalGuideLine).ActualTop = Math.Round((this.guideItem as HorizontalGuideLine).ActualTop  + yOffset / scale);
                InfoGuide = string.Format("y: {0}", (this.guideItem as HorizontalGuideLine).ActualTop);

            }
            else
            {
                (this.guideItem as VerticalGuideLine).ActualLeft = Math.Round((this.guideItem as VerticalGuideLine).ActualLeft + xOffset / scale);
                InfoGuide = string.Format("x: {0}", (this.guideItem as VerticalGuideLine).ActualLeft);

            }

        }
        private void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.guideItem.IsSelected = false;
            if (this.adorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this.adorner);
                }
                this.adorner = null;
            }

            // redo/undo
            if (pageUndoManager == null)
                return;
            if(this.guideItem is HorizontalGuideLine)
            {
                HorizontalGuideLine hGuide = this.guideItem as HorizontalGuideLine;
                GuidePropertyChangeCommand cmd = new GuidePropertyChangeCommand(hGuide.Guide, "Y", oldPos, guideItem.Guide.Y);
                pageUndoManager.Push(cmd);
            }
            else
            {
                VerticalGuideLine hGuide = this.guideItem as VerticalGuideLine;
                GuidePropertyChangeCommand cmd = new GuidePropertyChangeCommand(hGuide.Guide, "X", oldPos, guideItem.Guide.X);
                pageUndoManager.Push(cmd);
            }
            
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {            
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                GuideLineDecorator LineDecorator = this.DataContext as GuideLineDecorator;
                if (LineDecorator == null)
                {
                    return;
                }

                containerCanvas = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(LineDecorator)) as Canvas;
                Point p=e.GetPosition(containerCanvas);

                if (LineDecorator.IsVline == true)
                {
                    InofLocation = new Thickness(0, p.Y, -60, 0);
                }
                else
                {
                    InofLocation = new Thickness(p.X, 0, 0, -24);
                   
                }
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            this.guideItem = (DataContext as GuideLineDecorator).DataContext as GuideItemBase;
            if (guideItem != null)
            {
                this.guideItem.UpdateGridGuide();
            }
            e.Handled = true;
        }
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            //this.guideItem = (DataContext as GuideLineDecorator).DataContext as GuideItemBase;
            //guideItem.IsSelected = !this.guideItem.IsSelected;
            e.Handled = true;
        }
    }
}
