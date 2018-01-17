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

namespace Naver.Compass.WidgetLibrary
{
    public class PreveiwMoveAdorner : Adorner
    {
        private DesignerCanvas designerCanvas;
        private VisualCollection visuals;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }


        private Canvas adornerCanvas;
        PreviewVisualMoveControl previewArea;

        public PreveiwMoveAdorner(DesignerCanvas designerCanvas)
            : base(designerCanvas)
        {        
            this.designerCanvas = designerCanvas;
            this.adornerCanvas = new Canvas();
            this.adornerCanvas.Background = Brushes.Transparent;
            this.adornerCanvas.IsHitTestVisible = false;
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.adornerCanvas);
            previewArea = new PreviewVisualMoveControl();
            this.adornerCanvas.Children.Add(this.previewArea);
            previewArea.AddPreviewWidgets(designerCanvas.SelectedItemAndChildren, designerCanvas);
            this.IsHitTestVisible = false;

            //this.rubberband = new Rectangle();
            //this.rubberband.Stroke = Brushes.Navy;
            //this.rubberband.StrokeThickness = 1;
            //this.rubberband.StrokeDashArray = new DoubleCollection(new double[] { 2 });
            //this.adornerCanvas.Children.Add(this.rubberband);
        }
        public void PreviewMove(double deltaHorizontal, double deltaVertical)
        {
            previewArea.PreviewMove(deltaHorizontal, deltaVertical);            
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
    }

}
