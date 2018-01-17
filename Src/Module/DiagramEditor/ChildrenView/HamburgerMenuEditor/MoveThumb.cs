using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Naver.Compass.Module.HamburgerMenuEditor
{
    public class MoveThumb : Thumb
    {
        private IPagePropertyData _page;
        private bool _isMousePressMove = false;
        private ISelectionService _selectionService;

        IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }

        public MoveThumb()
        {
            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
            DragCompleted += new DragCompletedEventHandler(this.MoveThumb_DragCompleted);
        }


        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isMousePressMove = false;
            ISelectionService selectService = ServiceLocator.Current.GetInstance<ISelectionService>();
            _page = selectService.GetCurrentPage();
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = this.DataContext as Control;
            if (designerItem != null)
            {
                MenuPageEditorViewModel menuPageEditor = designerItem.DataContext as MenuPageEditorViewModel;
                if (_isMousePressMove==false)
                {
                    menuPageEditor.CreateNewPropertyMementos();
                    menuPageEditor.PropertyMementos.AddPropertyMemento(new PropertyMemento("Raw_EditorRectLeft", menuPageEditor.EditorRectLeft, menuPageEditor.EditorRectLeft));
                    menuPageEditor.PropertyMementos.AddPropertyMemento(new PropertyMemento("Raw_EditorRectTop", menuPageEditor.EditorRectTop, menuPageEditor.EditorRectTop));
                    _isMousePressMove = true;
                }
                
                double left = Canvas.GetLeft(designerItem);
                double top = Canvas.GetTop(designerItem);

                double xOffset = e.HorizontalChange;
                double yOffset = e.VerticalChange;

                Rect doundingRect = new Rect(menuPageEditor.EditorRectLeft, menuPageEditor.EditorRectTop, menuPageEditor.EditorRectWidth, menuPageEditor.EditorRectHeight);
                Snap(doundingRect, ref xOffset, ref yOffset);
                Canvas.SetLeft(designerItem, left + xOffset);
                Canvas.SetTop(designerItem, top + yOffset);

            }
        }

        private void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
             var designerItem = this.DataContext as Control;
             if (designerItem != null && _isMousePressMove)
             {
                 MenuPageEditorViewModel menuPageEditor = designerItem.DataContext as MenuPageEditorViewModel;
                 menuPageEditor.PropertyMementos.SetPropertyNewValue("Raw_EditorRectLeft", menuPageEditor.EditorRectLeft);
                 menuPageEditor.PropertyMementos.SetPropertyNewValue("Raw_EditorRectTop", menuPageEditor.EditorRectTop);
                 PropertyChangeCommand cmd = new PropertyChangeCommand(menuPageEditor, menuPageEditor.PropertyMementos);
                 (_page as ISupportUndo).UndoManager.Push(cmd);
             }           
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = true;
        }

        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            e.Handled = true;
        }

        private void Snap(Rect boundingRect, ref double deltaX, ref double deltaY)
        {
            Rect newRect = new Rect(boundingRect.Left + deltaX, boundingRect.Top + deltaY, boundingRect.Width, boundingRect.Height);
            double oriDeltaX = deltaX;
            double oriDeltaY = deltaY;
            if (GlobalData.IsSnapToGuide)
            {
                IPageView pageView = _page.ActivePage.PageViews.GetPageView(_selectionService.GetCurrentPage().CurAdaptiveViewGID);
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
                double xSnap, ySnap;
                //Horizontal
                if (deltaX == oriDeltaX)
                {
                    xSnap = newRect.Left % GlobalData.GRID_SIZE;
                    if (xSnap <= CommonDefine.SnapMargin)
                    {
                        xSnap = -xSnap;
                        deltaX += xSnap;
                    }
                    else
                    {
                        double temp = (newRect.Left + CommonDefine.SnapMargin) % GlobalData.GRID_SIZE;
                        if (temp < CommonDefine.SnapMargin)
                        {
                            xSnap = GlobalData.GRID_SIZE - xSnap;
                            deltaX += xSnap;
                        }
                    }
                }
                //Vertical
                if (deltaY == oriDeltaY)
                {

                    ySnap = newRect.Top % GlobalData.GRID_SIZE;
                    if (ySnap <= CommonDefine.SnapMargin)
                    {
                        ySnap = -ySnap;
                        deltaY += ySnap;
                    }
                    else
                    {
                        double temp = (newRect.Top + CommonDefine.SnapMargin) % GlobalData.GRID_SIZE;
                        if (temp < CommonDefine.SnapMargin)
                        {
                            ySnap = GlobalData.GRID_SIZE - ySnap;
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

    }
}
