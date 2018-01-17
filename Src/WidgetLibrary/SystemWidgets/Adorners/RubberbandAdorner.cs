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
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;

namespace Naver.Compass.WidgetLibrary
{
    public class RubberbandAdorner : Adorner
    {
        private Point? startPoint, endPoint;
        private Rectangle rubberband;
        private DesignerCanvas designerCanvas;
        private VisualCollection visuals;
        private Canvas adornerCanvas;
        ISelectionService selectionService;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public RubberbandAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint)
            : base(designerCanvas)
        {
            this.designerCanvas = designerCanvas;
            this.startPoint = dragStartPoint;

            this.adornerCanvas = new Canvas();
            this.adornerCanvas.Background = Brushes.Transparent
;
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.adornerCanvas);

            this.rubberband = new Rectangle();
            this.rubberband.Stroke = Brushes.Navy;
            this.rubberband.StrokeThickness = 1;
            this.rubberband.StrokeDashArray = new DoubleCollection(new double[] { 2 });

            this.adornerCanvas.Children.Add(this.rubberband);


            selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured)
                {
                    this.CaptureMouse();
                }

                this.endPoint = e.GetPosition(this);
                this.UpdateRubberband();
                this.UpdateSelection();
                e.Handled = false;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }

            selectionService.UpdateSelectionNotify();

            AdornerLayer adornerLayer = this.Parent as AdornerLayer;
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.adornerCanvas.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }

        private void UpdateRubberband()
        {
            double left = Math.Min(this.startPoint.Value.X, this.endPoint.Value.X);
            double top = Math.Min(this.startPoint.Value.Y, this.endPoint.Value.Y);

            double width = Math.Abs(this.startPoint.Value.X - this.endPoint.Value.X);
            double height = Math.Abs(this.startPoint.Value.Y - this.endPoint.Value.Y);

            this.rubberband.Width = width;
            this.rubberband.Height = height;
            Canvas.SetLeft(this.rubberband, left);
            Canvas.SetTop(this.rubberband, top);
        }

        private void UpdateSelection()
        {
            selectionService.AllowWdgPropertyNotify(false);
            Rect rubberBand = new Rect(this.startPoint.Value, this.endPoint.Value);
            foreach (ContentPresenter item in this.designerCanvas.Children)
            {
                if (item == null)
                    continue;
                //Rect itemRect = new Rect(item.RenderSize);
                //Rect itemBounds = item.TransformToAncestor(designerCanvas).TransformBounds(itemRect);

                BaseWidgetItem widget = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(item, 0),0) as BaseWidgetItem;

                if (widget == null || widget.Visibility!=Visibility.Visible)
                    continue;

                if (rubberBand.Contains(widget.GetBoundingRect())) // Select Contained Mode
                //if (rubberBand.IntersectsWith(itemBounds)) // Select Intersected Mode
                {
                   
                    if (widget.ParentID!=Guid.Empty)
                        continue;
                    if (widget.IsGroup == true)
                    { 
                        GroupViewModel groupVM = widget.DataContext as GroupViewModel;
                        IGroupOperation pageVM = designerCanvas.DataContext as IGroupOperation;
                        Guid groupID = groupVM.widgetGID;
                        pageVM.SetGroupStatus(groupID, GroupStatus.Selected);
                    }
                    else
                    {
                        widget.IsSelected = true;
                    }
                    
                }
                else
                {

                    if (widget.IsGroup == true)
                    {
                        GroupViewModel groupVM = widget.DataContext as GroupViewModel;
                        IGroupOperation pageVM = designerCanvas.DataContext as IGroupOperation;
                        Guid groupID = groupVM.widgetGID;
                        pageVM.SetGroupStatus(groupID, GroupStatus.UnSelect);
                    }
                    else
                    {
                        widget.IsSelected = false;
                    }
                }
            }
            selectionService.AllowWdgPropertyNotify(true);
        }
        //////////////////////////////////////////////////
    }
}
