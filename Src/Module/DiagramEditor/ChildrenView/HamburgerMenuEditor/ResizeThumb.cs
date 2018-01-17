using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service;
using Naver.Compass.Service.Document;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Naver.Compass.Module.HamburgerMenuEditor
{
    public class ResizeThumb : Thumb
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

        public ResizeThumb()
        {
            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
            DragCompleted += new DragCompletedEventHandler(this.ResizeThumb_DragCompleted);
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isMousePressMove = false;
            ISelectionService selectService = ServiceLocator.Current.GetInstance<ISelectionService>();
            _page = selectService.GetCurrentPage();
        }
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control designerItem = this.DataContext as Control;

            if (designerItem != null)
            {
                if (_isMousePressMove == false)
                {
                    MenuPageEditorViewModel menuPageEditor = designerItem.DataContext as MenuPageEditorViewModel;
                    menuPageEditor.CreateNewPropertyMementos();
                    menuPageEditor.PropertyMementos.AddPropertyMemento(new PropertyMemento("Raw_EditorRectLeft", menuPageEditor.EditorRectLeft, menuPageEditor.EditorRectLeft));
                    menuPageEditor.PropertyMementos.AddPropertyMemento(new PropertyMemento("Raw_EditorRectTop", menuPageEditor.EditorRectTop, menuPageEditor.EditorRectTop));
                    menuPageEditor.PropertyMementos.AddPropertyMemento(new PropertyMemento("Raw_EditorRectWidth", menuPageEditor.EditorRectWidth, menuPageEditor.EditorRectWidth));
                    menuPageEditor.PropertyMementos.AddPropertyMemento(new PropertyMemento("Raw_EditorRectHeight", menuPageEditor.EditorRectHeight, menuPageEditor.EditorRectHeight));

                    _isMousePressMove = true;
                }

                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = e.VerticalChange;
                        double snapBottom = Canvas.GetTop(designerItem) + (designerItem.ActualHeight + e.VerticalChange);
                        double snapBottomDelta = CalculateSnapChangeDelta(snapBottom, Service.Document.Orientation.Vertical);
                        deltaVertical += snapBottomDelta;
                        deltaVertical = Math.Min(-deltaVertical, designerItem.ActualHeight - designerItem.MinHeight);
                        designerItem.Height -= deltaVertical;
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        double snapTop = Canvas.GetTop(designerItem) + deltaVertical;
                        double snapTopDelta = CalculateSnapChangeDelta(snapTop, Service.Document.Orientation.Vertical);
                        deltaVertical += snapTopDelta;
                        Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) + deltaVertical);
                        designerItem.Height -= deltaVertical;
                        break;
                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        double snapLeft = Canvas.GetLeft(designerItem) + deltaHorizontal;
                        double snapLeftDelta = CalculateSnapChangeDelta(snapLeft, Service.Document.Orientation.Vertical);
                        deltaHorizontal += snapLeftDelta;
                        Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + deltaHorizontal);
                        designerItem.Width -= deltaHorizontal;
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = e.HorizontalChange;
                        double snapRight = Canvas.GetLeft(designerItem) + (designerItem.ActualWidth + e.HorizontalChange);
                        double snapRightDelta = CalculateSnapChangeDelta(snapRight, Service.Document.Orientation.Vertical);
                        deltaHorizontal += snapRightDelta;
                        deltaHorizontal = Math.Min(-deltaHorizontal, designerItem.ActualWidth - designerItem.MinWidth);
                        designerItem.Width -= deltaHorizontal;
                        break;
                    default:
                        break;
                }
            }

            e.Handled = true;
        }

        private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var designerItem = this.DataContext as Control;
            if (designerItem != null && _isMousePressMove)
            {
                MenuPageEditorViewModel menuPageEditor = designerItem.DataContext as MenuPageEditorViewModel;
                menuPageEditor.PropertyMementos.SetPropertyNewValue("Raw_EditorRectLeft", menuPageEditor.EditorRectLeft);
                menuPageEditor.PropertyMementos.SetPropertyNewValue("Raw_EditorRectTop", menuPageEditor.EditorRectTop);
                menuPageEditor.PropertyMementos.SetPropertyNewValue("Raw_EditorRectWidth", menuPageEditor.EditorRectWidth);
                menuPageEditor.PropertyMementos.SetPropertyNewValue("Raw_EditorRectHeight", menuPageEditor.EditorRectHeight);

                PropertyChangeCommand cmd = new PropertyChangeCommand(menuPageEditor, menuPageEditor.PropertyMementos);
                (_page as ISupportUndo).UndoManager.Push(cmd);
            }   
        }

        private double CalculateSnapChangeDelta(double valueToSnap, Service.Document.Orientation guideOrientation)
        {
            //Snap to Guide
            if (GlobalData.IsSnapToGuide)
            {
                IPageView pageView = _page.ActivePage.PageViews.GetPageView(_selectionService.GetCurrentPage().CurAdaptiveViewGID);
                if (pageView == null)
                    return 0;

                foreach (IGuide item in pageView.Guides)
                {
                    if (guideOrientation == Service.Document.Orientation.Horizontal
                        && item.Orientation == Service.Document.Orientation.Horizontal)
                    {
                        if (Math.Abs(valueToSnap - item.Y) < CommonDefine.SnapMargin)
                        {
                            return item.Y - valueToSnap;
                        }
                    }
                    else if (guideOrientation == Service.Document.Orientation.Vertical
                                && item.Orientation == Service.Document.Orientation.Vertical)
                    {
                        if (Math.Abs(valueToSnap - item.X) < CommonDefine.SnapMargin)
                        {
                            return item.X - valueToSnap;
                        }
                    }
                }

                foreach (IGuide item in _document.GlobalGuides)
                {
                    if (guideOrientation == Service.Document.Orientation.Horizontal
                        && item.Orientation == Service.Document.Orientation.Horizontal)
                    {
                        if (Math.Abs(valueToSnap - item.Y) < CommonDefine.SnapMargin)
                        {
                            return item.Y - valueToSnap;
                        }
                    }
                    else if (guideOrientation == Service.Document.Orientation.Vertical
                                && item.Orientation == Service.Document.Orientation.Vertical)
                    {
                        if (Math.Abs(valueToSnap - item.X) < CommonDefine.SnapMargin)
                        {
                            return item.X - valueToSnap;
                        }
                    }
                }
            }

            //Snap to grid
            if (GlobalData.IsSnapToGrid)
            {
                double snap = valueToSnap % GlobalData.GRID_SIZE;
                if (snap <= CommonDefine.SnapMargin)
                {
                    snap = -snap;
                }
                else
                {
                    double temp = (valueToSnap + CommonDefine.SnapMargin) % GlobalData.GRID_SIZE;
                    if (temp < CommonDefine.SnapMargin)
                    {
                        snap = GlobalData.GRID_SIZE - snap;
                    }
                    else
                    {
                        snap = 0;
                    }
                }

                return snap;
            }

            return 0;
        }
    }
}
