using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    /// <summary>
    /// Interaction logic for PreviewVisualMoveControl.xaml
    /// </summary>
    public partial class PreviewVisualMoveControl : UserControl
    {
        public PreviewVisualMoveControl()
        {
            InitializeComponent();
        }

        double minleft = double.MaxValue;
        double mintop = double.MaxValue;
        public void AddPreviewWidgets(IEnumerable<BaseWidgetItem> widgets, Canvas parent)
        {    
            //Get real left and top property of bound box
            double maxRight = double.MinValue;
            double maxBottom = double.MinValue;
            if (widgets.Count()<=0)
            {
                return;
            }
            foreach (BaseWidgetItem element in widgets)
            {
                WidgetViewModelDate vm = element.DataContext as WidgetViewModelDate;
        
                Rect rec = new Rect(vm.Raw_Left, vm.Raw_Top, vm.Raw_ItemWidth, vm.Raw_ItemHeight);
               
                if (vm.RotateAngle != 0)
                {                   
                    rec = new Rect(0, 0, vm.Raw_ItemWidth, vm.Raw_ItemHeight);
                    
                    rec = element.TransformToAncestor(parent).TransformBounds(rec);
                }
                minleft = Math.Min(minleft, rec.Left);
                mintop = Math.Min(mintop, rec.Top);
                maxRight = Math.Max(maxRight, rec.Left + rec.Width);
                maxBottom = Math.Max(maxBottom, rec.Top + rec.Height);
            }

            try
            {
                Canvas.SetLeft(this, minleft);
                Canvas.SetTop(this, mintop);
                this.Width = maxRight - minleft;
                this.Height = maxBottom - mintop;
            }
            catch (Exception)
            {
                NLogger.Warn("Preview Canvas Width and Height must be non-negative:"
                    + minleft+","+ mintop+","+ maxRight+","+ maxBottom);
                this.Width = 300;
                this.Height = 300;
            }



            //Set real left and top property of preview widget
            foreach (BaseWidgetItem element in widgets)
            {
                WidgetViewModelDate vm = element.DataContext as WidgetViewModelDate;

                Rect rec = new Rect(vm.Left, vm.Top, element.ActualWidth, element.ActualHeight);
                
                if (vm.RotateAngle != 0)
                {
                    rec = new Rect(0, 0, element.ActualWidth, element.ActualHeight);
                    rec = element.TransformToAncestor(parent).TransformBounds(rec);
                }
                double left = rec.Left - minleft;
                double top = rec.Top - mintop;

                VisualBrush visualBrush = new VisualBrush();
                visualBrush.AutoLayoutContent = true;
                
                visualBrush.Visual = element.Parent as Visual;
                visualBrush.Stretch = Stretch.None;
                element.ClipToBounds = true;
                visualBrush.AlignmentX = 0;
                visualBrush.AlignmentY = 0;
 
                Rectangle rectangle = new Rectangle();

                try
                {
                    rectangle.Height = rec.Height;
                    rectangle.Width = rec.Width;
                    rectangle.Fill = visualBrush;

                }
                catch
                {
                    NLogger.Warn("Widget Rectabgke Width and Height must be non-negative.");
                    continue;
                }

                AdornerMovePreviewCanvas.Children.Add(rectangle);
                Canvas.SetLeft(rectangle, left);
                Canvas.SetTop(rectangle, top);

            }
            AdornerMovePreviewCanvas.InvalidateVisual();
        }

        public void PreviewMove(double deltaHorizontal, double deltaVertical)
        {
            Canvas.SetLeft(this, minleft + deltaHorizontal);
            Canvas.SetTop(this, mintop + deltaVertical);
        }
    }
    
}
