using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections.Generic;
using Naver.Compass.InfoStructure;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Document;
using Microsoft.Practices.ServiceLocation;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Naver.Compass.Service;
using Microsoft.Practices.Prism.Events;

namespace Naver.Compass.WidgetLibrary
{
    public class ResizeThumb : Thumb
    {
        #region Constructor
        public ResizeThumb()
        {
            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            _infoItems = new List<WidgetViewModBase>();
            _group = null;
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
            DragCompleted += new DragCompletedEventHandler(this.ResizeThumb_DragCompleted);
        }
        #endregion

        #region private member
        private RotateTransform rotateTransform;
        private double angle;
        private System.Windows.Point transformOrigin;
        private BaseWidgetItem designerItem;
        private DesignerCanvas canvas;
        private IPage _page;
        private ISelectionService _selectionService;
        

        //Redo/Undo/Selected Widget information item
        private List<WidgetViewModBase> _infoItems;
        GroupViewModel _group;
        //TODO:DELETE
        ContentPresenter ItemWrapper;

        IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }
        #endregion

        #region Widget Operation
        private void DragWidgetTop(double scale, WidgetViewModBase wdg)
        {
            if (scale == 1 || scale == 0)
            {
                return;
            }
            WidgetViewModBase widgetVM = wdg;
            double deltaVertical = widgetVM.ItemHeight - (widgetVM.ItemHeight * scale);
            double targeAngle = widgetVM.RotateAngle * Math.PI / 180.0;

            //this.transformOrigin.Y=0.5. rotate by center point(0.5H,0.5W)
            widgetVM.Raw_Left = widgetVM.Left + deltaVertical * Math.Sin(-targeAngle) - (0.5* deltaVertical * Math.Sin(-targeAngle));
            widgetVM.Raw_Top = widgetVM.Top + deltaVertical * Math.Cos(-targeAngle) + (0.5* deltaVertical * (1 - Math.Cos(-targeAngle)));
            widgetVM.Raw_ItemHeight = widgetVM.ItemHeight * scale;
        }
        private void DragWidgetBottom(double scale, WidgetViewModBase wdg)
        {
            if (scale == 1 || scale == 0)
            {
                return;
            }
            WidgetViewModBase widgetVM = wdg;
            double deltaVertical = widgetVM.ItemHeight - (widgetVM.ItemHeight * scale);
            double targeAngle = widgetVM.RotateAngle * Math.PI / 180.0;
            
            //this.transformOrigin.Y=0.5. rotate by center point(0.5H,0.5W)
            widgetVM.Raw_Left = widgetVM.Left - deltaVertical * 0.5 * Math.Sin(-targeAngle);
            widgetVM.Raw_Top = widgetVM.Top + (0.5 * deltaVertical * (1 - Math.Cos(-targeAngle)));
            widgetVM.Raw_ItemHeight = widgetVM.ItemHeight * scale;
        }
        private void DragWidgetLeft(double scale, WidgetViewModBase wdg)
        {
            if (scale == 1 || scale == 0)
            {
                return;
            }
            WidgetViewModBase widgetVM = wdg;
            double deltaHorizontal = widgetVM.ItemWidth - (widgetVM.ItemWidth * scale);
            double targeAngle = widgetVM.RotateAngle * Math.PI / 180.0;

            //this.transformOrigin.Y=0.5. rotate by center point(0.5H,0.5W)
            widgetVM.Raw_Top = widgetVM.Top + deltaHorizontal * Math.Sin(targeAngle) - 0.5 * deltaHorizontal * Math.Sin(targeAngle);
            widgetVM.Raw_Left = widgetVM.Left + deltaHorizontal * Math.Cos(targeAngle) + (0.5 * deltaHorizontal * (1 - Math.Cos(targeAngle)));
            widgetVM.Raw_ItemWidth = widgetVM.ItemWidth * scale;
        }
        private void DragWidgetRight(double scale, WidgetViewModBase wdg)
        {
            if (scale == 1 || scale == 0)
            {
                return;
            }
            WidgetViewModBase widgetVM = wdg;
            double deltaHorizontal = widgetVM.ItemWidth - (widgetVM.ItemWidth * scale);
            double targeAngle = widgetVM.RotateAngle * Math.PI / 180.0;

            //this.transformOrigin.Y=0.5. rotate by center point(0.5H,0.5W)
            widgetVM.Raw_Top = widgetVM.Top - 0.5 * deltaHorizontal * Math.Sin(targeAngle);
            widgetVM.Raw_Left = widgetVM.Left + (deltaHorizontal * 0.5 * (1 - Math.Cos(targeAngle)));
            widgetVM.Raw_ItemWidth = widgetVM.ItemWidth * scale;
        }
        #endregion

        #region Group Operation part_1(line)
        private void DragGroupTop(double scale,GroupViewModel group)
        {
            GroupViewModel groupVM = group;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupBottom = groupVM.Top + groupVM.ItemHeight;

            if (scale == 1)
            {
                return;
            }

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;

                if (item.IsLocked == true )
                {
                    continue;
                }
                //if (item.IsLocked == true || item.CanDragResize == false)
                //{
                //    continue;
                //}

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double deltaY = (groupBottom - ItemTop) * (scale - 1);
                BoundRect.Y -= deltaY;
                if ((item.RotateAngle % 90) != 45)
                {
                    BoundRect.Height *= scale;
                }
                else
                {
                    BoundRect.Height *= scale;
                    BoundRect.Width *= scale;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    if (groupBottom > BoundRect.Bottom)
                    {
                        item.Top -= deltaY;
                        //item.ItemHeight *= scale;

                        //item.ItemHeight *= scale;
                        //item.Top = BoundRect.Y + 0.5 * (BoundRect.Height - item.ItemHeight);
                    }
                    //item.Top -= deltaY;
                    //item.ItemHeight *= scale;
                    continue;
                }
                //if (realRec.Width < 10 || realRec.Height < 10)
                //{
                //    realRec.Width = Math.Max(10, realRec.Width);
                //    realRec.Height = Math.Max(10, realRec.Height);
                //}
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {
                    continue;
                }

                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }

        }
        private void DragGroupBottom(double scale, GroupViewModel group)
        {
            GroupViewModel groupVM = group;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupTop = groupVM.Top;

            if (scale == 1)
            {
                return;
            }

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;
                if (item.IsLocked == true || item.CanDragResize == false)
                {
                    continue;
                }
                if (item.IsLocked == true)
                {
                    continue;
                }
                //if (item.IsLocked == true || item.CanDragResize == false)
                //{
                //    continue;
                //}
                
                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double deltaY = (ItemTop - groupTop) * (scale - 1);
                BoundRect.Y += deltaY;
                if ((item.RotateAngle % 90) != 45)
                {
                    BoundRect.Height *= scale;
                }
                else
                {
                    BoundRect.Height *= scale;
                    BoundRect.Width *= scale;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    item.Top += deltaY;
                    //item.ItemHeight *= scale;
                    continue;
                }

                // if (realRec.Top < 0 || realRec.Left < 0 || realRec.Width < 10 || realRec.Height < 10 || realRec.Right < 0 || realRec.Bottom < 0)                 

                //if (realRec.Width < 10 || realRec.Height < 10)
                //{
                //    realRec.Width = Math.Max(10, realRec.Width);
                //    realRec.Height = Math.Max(10, realRec.Height);
                //}
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {

                     continue;
                }
                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }
        }
        private void DragGroupLeft(double scale, GroupViewModel group)
        {
            GroupViewModel groupVM = group;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupRight = groupVM.Left + groupVM.ItemWidth;

            if (scale == 1)
            {
                return;
            }

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;
                if (item.IsLocked == true)
                {
                    continue;
                }
                //if (item.IsLocked == true || item.CanDragResize == false)
                //{
                //    continue;
                //}

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemLeft = BoundRect.Left;
                double deltaX = (groupRight - ItemLeft) * (scale - 1);
                BoundRect.X -= deltaX;
                if ((item.RotateAngle % 90) != 45)
                {
                    BoundRect.Width *= scale;
                }
                else
                {
                    BoundRect.Height *= scale;
                    BoundRect.Width *= scale;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    if (groupRight > BoundRect.Right)
                    {
                        item.Left -= deltaX;
                    }
                    continue;
                }
                //if (realRec.Width < 10 || realRec.Height < 10)
                //{
                //    realRec.Width = Math.Max(10, realRec.Width);
                //    realRec.Height = Math.Max(10, realRec.Height);
                //}
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Height - BoundRect.Height) > 0.2)
                {
                    continue;
                }

                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }
        }
        private void DragGroupRight(double scale, GroupViewModel group)
        {
            GroupViewModel groupVM = group;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupLeft = groupVM.Left;

            if (scale == 1)
            {
                return;
            }

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;
                if (item.IsLocked == true)
                {
                    continue;
                }
                //if (item.IsLocked == true || item.CanDragResize == false)
                //{
                //    continue;
                //}

                Rect BoundRect = item.GetBoundingRectangle(false);

                double ItemLeft = BoundRect.Left;
                double deltaX = (ItemLeft - groupLeft) * (scale - 1);
                //double deltaW = BoundRect.Width * scale;
                BoundRect.X += deltaX;
                if ((item.RotateAngle % 90) != 45)
                {
                    BoundRect.Width *= scale;
                }
                else
                {
                    BoundRect.Height *= scale;
                    BoundRect.Width *= scale;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    item.Left += deltaX;
                    //item.ItemWidth *= scale;
                    continue;
                }
                //if (realRec.Width < 10 || realRec.Height < 10)
                //{
                //    realRec.Width = Math.Max(10, realRec.Width);
                //    realRec.Height = Math.Max(10, realRec.Height);
                //}
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Height - BoundRect.Height) > 0.2)
                {
                    continue;
                }

                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }
        }
        #endregion

        #region Group Operation part_2(position)
        private void DragGroupTopLeft(double scaleW, double scaleH, GroupViewModel group)
        {
            if (scaleW == 1 && scaleH == 1)
            {
                return;
            }
            GroupViewModel groupVM = group;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupBottom = groupVM.Top + groupVM.ItemHeight;
            double groupRight = groupVM.Left + groupVM.ItemWidth;

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;
                if (item.IsLocked == true)
                {
                    continue;
                }
                //if (item.IsLocked == true || item.CanDragResize == false)
                //{
                //    continue;
                //}

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double ItemLeft = BoundRect.Left;

                double angle = Math.Abs(item.RotateAngle) % 90;
                if (angle == 45)
                {
                    double deltaY = (groupBottom - ItemTop) * (Math.Max(scaleH, scaleW) - 1);
                    double deltaX = (groupRight - ItemLeft) * (Math.Max(scaleH, scaleW) - 1);
                    BoundRect.Y -= deltaY;
                    BoundRect.X -= deltaX;
                    BoundRect.Height *= Math.Max(scaleH, scaleW);
                    BoundRect.Width *= Math.Max(scaleH, scaleW);
                }
                else
                {
                    double deltaY = (groupBottom - ItemTop) * (scaleH - 1);
                    double deltaX = (groupRight - ItemLeft) * (scaleW - 1);
                    BoundRect.Y -= deltaY;
                    BoundRect.X -= deltaX;
                    BoundRect.Height *= scaleH;
                    BoundRect.Width *= scaleW;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    continue;
                }
                //if (realRec.Width < 10 || realRec.Height < 10)
                //{
                //    realRec.Width = Math.Max(10, realRec.Width);
                //    realRec.Height = Math.Max(10, realRec.Height);
                //}
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {
                    continue;
                }
                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }

        }
        private void DragGroupTopRight(double scaleW, double scaleH, GroupViewModel group)
        {
            if (scaleW == 1 && scaleH == 1)
            {
                return;
            }
            GroupViewModel groupVM = group;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupBottom = groupVM.Top + groupVM.ItemHeight; ;
            double groupLeft = groupVM.Left;

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;
                if (item.IsLocked == true)
                {
                    continue;
                }
                //if (item.IsLocked == true || item.CanDragResize == false)
                //{
                //    continue;
                //}

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double ItemLeft = BoundRect.Left;

                double angle = Math.Abs(item.RotateAngle) % 90;
                if (angle == 45)
                {
                    double deltaY = (groupBottom - ItemTop) * (Math.Max(scaleH, scaleW) - 1);
                    double deltaX = (ItemLeft - groupLeft) * (Math.Max(scaleH, scaleW) - 1);
                    BoundRect.Y -= deltaY;
                    BoundRect.X += deltaX;
                    BoundRect.Height *= Math.Max(scaleH, scaleW);
                    BoundRect.Width *= Math.Max(scaleH, scaleW);
                }
                else
                {
                    double deltaY = (groupBottom - ItemTop) * (scaleH - 1);
                    double deltaX = (ItemLeft - groupLeft) * (scaleW - 1);
                    BoundRect.Y -= deltaY;
                    BoundRect.X += deltaX;
                    BoundRect.Height *= scaleH;
                    BoundRect.Width *= scaleW;
                    //if (scaleH < 1 || scaleW < 1)
                    //{
                    //    int i = 0;
                    //}
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    continue;
                }
                //if (realRec.Width < 10 || realRec.Height < 10)
                //{
                //    realRec.Width = Math.Max(10, realRec.Width);
                //    realRec.Height = Math.Max(10, realRec.Height);
                //}
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {
                    continue;
                }
                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }

        }
        private void DragGroupBottomLeft(double scaleW, double scaleH, GroupViewModel group)
        {
            if (scaleW == 1 && scaleH == 1)
            {
                return;
            }
            GroupViewModel groupVM = group;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupTop = groupVM.Top;
            double groupRight = groupVM.Left + groupVM.ItemWidth;

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;
                if (item.IsLocked == true)
                {
                    continue;
                }
                //if (item.IsLocked == true || item.CanDragResize == false)
                //{
                //    continue;
                //}

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double ItemLeft = BoundRect.Left;

                double angle = Math.Abs(item.RotateAngle) % 90;
                if (angle == 45)
                {
                    double deltaY = (ItemTop - groupTop) * (Math.Max(scaleH, scaleW) - 1);
                    double deltaX = (groupRight - ItemLeft) * (Math.Max(scaleH, scaleW) - 1);
                    BoundRect.Y += deltaY;
                    BoundRect.X -= deltaX;
                    BoundRect.Height *= Math.Max(scaleH, scaleW);
                    BoundRect.Width *= Math.Max(scaleH, scaleW);
                }
                else
                {
                    double deltaY = (ItemTop - groupTop) * (scaleH - 1);
                    double deltaX = (groupRight - ItemLeft) * (scaleW - 1);
                    BoundRect.Y += deltaY;
                    BoundRect.X -= deltaX;
                    BoundRect.Height *= scaleH;
                    BoundRect.Width *= scaleW;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    continue;
                }
                //if (realRec.Width < 10 || realRec.Height < 10)
                //{
                //    realRec.Width = Math.Max(10, realRec.Width);
                //    realRec.Height = Math.Max(10, realRec.Height);
                //}
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {
                    continue;
                }
                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }
        }
        private void DragGroupBottomRight(double scaleW, double scaleH, GroupViewModel group)
        {
            if (scaleW == 1 && scaleH == 1)
            {
                return;
            }
            GroupViewModel groupVM = group;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupTop = groupVM.Top;
            double groupLeft = groupVM.Left;

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;
                if (item.IsLocked == true)
                {
                    continue;
                }
                //if (item.IsLocked == true || item.CanDragResize == false)
                //{
                //    continue;
                //}

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double ItemLeft = BoundRect.Left;


                double angle = Math.Abs(item.RotateAngle) % 90;
                if (angle == 45)
                {
                    double deltaY = (ItemTop - groupTop) * (Math.Max(scaleH, scaleW) - 1);
                    double deltaX = (ItemLeft - groupLeft) * (Math.Max(scaleH, scaleW) - 1);
                    BoundRect.Y += deltaY;
                    BoundRect.X += deltaX;
                    BoundRect.Height *= Math.Max(scaleH, scaleW);
                    BoundRect.Width *= Math.Max(scaleH, scaleW);
                }
                else
                {
                    double deltaY = (ItemTop - groupTop) * (scaleH - 1);
                    double deltaX = (ItemLeft - groupLeft) * (scaleW - 1);
                    BoundRect.Y += deltaY;
                    BoundRect.X += deltaX;
                    BoundRect.Height *= scaleH;
                    BoundRect.Width *= scaleW;
                }


                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    continue;
                }
                //if (realRec.Width < 10 || realRec.Height < 10)
                //{
                //    realRec.Width = Math.Max(10, realRec.Width);
                //    realRec.Height = Math.Max(10, realRec.Height);
                //}
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {
                    continue;
                }
                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }
        }
        #endregion

        #region Count the drage change information(vertical/horizontal)
        private DrageChangeData GetDragWidgetData(DragDeltaEventArgs e)
        {
            DrageChangeData data = new DrageChangeData();

            double deltaVertical = 0, deltaHorizontal = 0;
            switch (VerticalAlignment)
            {
                case System.Windows.VerticalAlignment.Bottom:
                    double snapBottomChange = e.VerticalChange;
                    double snapBottom = Canvas.GetTop(ItemWrapper) + (ItemWrapper.ActualHeight + e.VerticalChange);
                    double snapBottomDelta = CalculateSnapChangeDelta(snapBottom, Service.Document.Orientation.Horizontal);
                    snapBottomChange += snapBottomDelta;

                    deltaVertical = Math.Min(-snapBottomChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    data.ScaleY = (ItemWrapper.ActualHeight - deltaVertical) / ItemWrapper.ActualHeight;
                    break;

                case System.Windows.VerticalAlignment.Top:
                    double snapTopChange = e.VerticalChange;
                    double snapTop = Canvas.GetTop(ItemWrapper) + e.VerticalChange;
                    double snapTopDelta = CalculateSnapChangeDelta(snapTop, Service.Document.Orientation.Horizontal);
                    snapTopChange += snapTopDelta;

                    deltaVertical = Math.Min(snapTopChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    data.ScaleY = (ItemWrapper.ActualHeight - deltaVertical) / ItemWrapper.ActualHeight;
                    break;

                default:
                    break;
            }

            switch (HorizontalAlignment)
            {
                case System.Windows.HorizontalAlignment.Left:
                    double snapLeftChange = e.HorizontalChange;
                    double snapLeft = Canvas.GetLeft(ItemWrapper) + e.HorizontalChange;
                    double snapLeftDelta = CalculateSnapChangeDelta(snapLeft, Service.Document.Orientation.Vertical);
                    snapLeftChange += snapLeftDelta;

                    deltaHorizontal = Math.Min(snapLeftChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    data.ScaleX= (ItemWrapper.ActualWidth - deltaHorizontal) / ItemWrapper.ActualWidth;
                    break;

                case System.Windows.HorizontalAlignment.Right:
                    double snapRightChange = e.HorizontalChange;
                    double snapRight = Canvas.GetLeft(ItemWrapper) + (ItemWrapper.ActualWidth + e.HorizontalChange);
                    double snapRightDelta = CalculateSnapChangeDelta(snapRight, Service.Document.Orientation.Vertical);
                    snapRightChange += snapRightDelta;

                    deltaHorizontal = Math.Min(-snapRightChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    data.ScaleX = (ItemWrapper.ActualWidth - deltaHorizontal) / ItemWrapper.ActualWidth;
                    break;

                default:
                    break;

            }
            return data;

        }
        private DrageChangeData GetDragWidgetProRataData(DragDeltaEventArgs e)
        {
            DrageChangeData data = new DrageChangeData();

            double height, width;
            if (VerticalAlignment == System.Windows.VerticalAlignment.Bottom
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
            {
                height = ItemWrapper.ActualHeight + e.VerticalChange;
                width = ItemWrapper.ActualWidth + e.HorizontalChange;
                //height = ItemWrapper.ActualHeight + e.VerticalChange;
                //width = ItemWrapper.ActualWidth + e.HorizontalChange;
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Bottom
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
            {
                height = ItemWrapper.ActualHeight + e.VerticalChange;
                width = ItemWrapper.ActualWidth - e.HorizontalChange;
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Top
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
            {
                height = ItemWrapper.ActualHeight - e.VerticalChange;
                width = ItemWrapper.ActualWidth - e.HorizontalChange;
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Top
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
            {
                height = ItemWrapper.ActualHeight - e.VerticalChange;
                width = ItemWrapper.ActualWidth + e.HorizontalChange;
            }
            else
            {
                data.ScaleX = 1;
                data.ScaleY=1;
                return data;
            }


            if (height < ItemWrapper.MinHeight || width < ItemWrapper.MinWidth)
            {
                height = Math.Max(10, height);
                width = Math.Max(10, width);
            }
            double rata = Math.Max(height / ItemWrapper.ActualHeight, width / ItemWrapper.ActualWidth);

            double deltaVertical = ItemWrapper.ActualHeight - ItemWrapper.ActualHeight * rata;
            double deltaHorizontal = ItemWrapper.ActualWidth - ItemWrapper.ActualWidth * rata;
            //Debug.WriteLine("e.v--e.h---v---h:  " + e.VerticalChange + " , " + e.HorizontalChange + " , " + deltaVertical + " , " + deltaHorizontal);


            data.ScaleX = (ItemWrapper.ActualWidth - deltaHorizontal) / ItemWrapper.ActualWidth;
            data.ScaleY = (ItemWrapper.ActualHeight - deltaVertical) / ItemWrapper.ActualHeight;
            return data;

        }
        private DrageChangeData GetDragGroupData(DragDeltaEventArgs e)
        {
            DrageChangeData data = new DrageChangeData();

            double deltaVertical, deltaHorizontal, scaleY = 1, scaleX = 1;
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            if (groupVM == null)
            {
                data.ScaleX = 1;
                data.ScaleY = 1;
                return data;
            }

            //Get the correct prorata--(Max Value)
            switch (VerticalAlignment)
            {
                case System.Windows.VerticalAlignment.Bottom:
                    double snapBottomChange = e.VerticalChange;
                    double snapBottom = Canvas.GetTop(ItemWrapper) + (ItemWrapper.ActualHeight + e.VerticalChange);
                    double snapBottomDelta = CalculateSnapChangeDelta(snapBottom, Service.Document.Orientation.Horizontal);
                    snapBottomChange += snapBottomDelta;

                    deltaVertical = Math.Min(-snapBottomChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    scaleY = (ItemWrapper.ActualHeight - deltaVertical) / ItemWrapper.ActualHeight;
                    //scaleY = Math.Max(minscaleY, scaleY);
                    //DragGroupBottom(scaleY);
                    break;

                case System.Windows.VerticalAlignment.Top:
                    double snapTopChange = e.VerticalChange;
                    double snapTop = Canvas.GetTop(ItemWrapper) + e.VerticalChange;
                    double snapTopDelta = CalculateSnapChangeDelta(snapTop, Service.Document.Orientation.Horizontal);
                    snapTopChange += snapTopDelta;
                    deltaVertical = Math.Min(snapTopChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    scaleY = (ItemWrapper.ActualHeight - deltaVertical) / ItemWrapper.ActualHeight;
                    //scaleY = Math.Max(minscaleY, scaleY);
                    //DragGroupTop(scaleY);
                    break;

                default:
                    break;
            }
            switch (HorizontalAlignment)
            {
                case System.Windows.HorizontalAlignment.Left:
                    double snapLeftChange = e.HorizontalChange;
                    double snapLeft = Canvas.GetLeft(ItemWrapper) + e.HorizontalChange;
                    double snapLeftDelta = CalculateSnapChangeDelta(snapLeft, Service.Document.Orientation.Vertical);
                    snapLeftChange += snapLeftDelta;
                    deltaHorizontal = Math.Min(snapLeftChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    scaleX = (ItemWrapper.ActualWidth - deltaHorizontal) / ItemWrapper.ActualWidth;
                    //scaleX = Math.Max(minscaleX, scaleX);
                    //DragGroupLeft(scaleX);
                    break;

                case System.Windows.HorizontalAlignment.Right:
                    double snapRightChange = e.HorizontalChange;
                    double snapRight = Canvas.GetLeft(ItemWrapper) + (ItemWrapper.ActualWidth + e.HorizontalChange);
                    double snapRightDelta = CalculateSnapChangeDelta(snapRight, Service.Document.Orientation.Vertical);
                    snapRightChange += snapRightDelta;
                    deltaHorizontal = Math.Min(-snapRightChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    scaleX = (ItemWrapper.ActualWidth - deltaHorizontal) / ItemWrapper.ActualWidth;
                    // scaleX = Math.Max(minscaleX, scaleX);
                    //DragGroupRight(scaleX);
                    break;

                default:
                    break;
            }

            data.ScaleX = scaleX;
            data.ScaleY = scaleY;
            return data;

        }
        private DrageChangeData GetDragGroupProRataData(DragDeltaEventArgs e)
        {
            DrageChangeData data = new DrageChangeData();

            double deltaVertical, deltaHorizontal, scaleY = 1, scaleX = 1, scale = 1;
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            if (groupVM == null)
            {
                data.ScaleX = 1;
                data.ScaleY = 1;
                return data;
            }

            //Get the correct prorata--(Max Value)
            switch (VerticalAlignment)
            {
                case System.Windows.VerticalAlignment.Bottom:
                    double snapBottomChange = e.VerticalChange;

                    deltaVertical = Math.Min(-snapBottomChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    scaleY = (ItemWrapper.ActualHeight - deltaVertical) / ItemWrapper.ActualHeight;
                    //DragGroupBottom(scaleY);
                    break;

                case System.Windows.VerticalAlignment.Top:
                    double snapTopChange = e.VerticalChange;

                    deltaVertical = Math.Min(snapTopChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    scaleY = (ItemWrapper.ActualHeight - deltaVertical) / ItemWrapper.ActualHeight;
                    //DragGroupTop(scaleY);
                    break;

                default:
                    break;
            }
            switch (HorizontalAlignment)
            {
                case System.Windows.HorizontalAlignment.Left:
                    double snapLeftChange = e.HorizontalChange;

                    deltaHorizontal = Math.Min(snapLeftChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    scaleX = (ItemWrapper.ActualWidth - deltaHorizontal) / ItemWrapper.ActualWidth;
                    //DragGroupLeft(scaleX);
                    break;

                case System.Windows.HorizontalAlignment.Right:
                    double snapRightChange = e.HorizontalChange;

                    deltaHorizontal = Math.Min(-snapRightChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    scaleX = (ItemWrapper.ActualWidth - deltaHorizontal) / ItemWrapper.ActualWidth;
                    //DragGroupRight(scaleX);
                    break;

                default:
                    break;
            }
            scale = Math.Max(scaleX, scaleY);

            data.ScaleX = scale;
            data.ScaleY = scale;
            return data;

        }



        /// <summary>
        /// Calculate snap value if Snap to Guide / Snap to Grid checked.
        /// </summary>
        /// <param name="valueToSnap">new left/top/bottom/right of widget</param>
        /// <param name="guideOrientation">only used for "snap to Guide"</param>
        /// <returns></returns>
        private double CalculateSnapChangeDelta(double valueToSnap, Service.Document.Orientation guideOrientation)
        {
            //Snap to Guide
            if (GlobalData.IsSnapToGuide)
            {
                IPageView pageView = _page.PageViews.GetPageView(_selectionService.GetCurrentPage().CurAdaptiveViewGID);
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
                //snap left/top of grid,example: valueToSnap%GRID_SIZE -> 122%10
                //left/top side of grid,not widget.
                if (snap <= CommonDefine.SnapMargin)
                {
                    snap = -snap;
                }
                else
                {
                    double temp = (valueToSnap + CommonDefine.SnapMargin) % GlobalData.GRID_SIZE;
                    //example:128%10
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
        #endregion
               
        #region Event Handler
        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = this.DataContext as BaseWidgetItem;
            _infoItems.Clear();
            _group = null;
            if (this.designerItem == null)
                return;

            this.canvas = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem))) as DesignerCanvas;
            ItemWrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            if (this.canvas != null)
            {
                this.transformOrigin = this.designerItem.RenderTransformOrigin;

                this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                if (this.rotateTransform != null)
                {
                    this.angle = this.rotateTransform.Angle * Math.PI / 180.0;
                }
                else
                {
                    this.angle = 0.0d;
                }


                ShowSelectionSizeInfo(true);

                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc.Document != null)
                {
                    //_page = doc.Document.Pages.GetPage((canvas.DataContext as IPagePropertyData).PageGID);
                    _page = (canvas.DataContext as IPagePropertyData).ActivePage;
                }
            }

        }
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Initialize the selected widgets' context when first move
            if (this.designerItem == null)
            {
                return;
            }
            if (e.VerticalChange==0 && e.HorizontalChange==0)
            {
                return;
            }

            //Initialize the undo stack while first drag action. 
            if (_infoItems.Count <= 0)
            {
                InitializeSelectionUndoStack();      
            }

            //Get Vertical and Horizontal delta data
            DrageChangeData data = GetDragChangeData(e);

            //Resize the Selected Object
            if (data.ScaleX==1 && data.ScaleY==1)
            {
                return;
            }

            ResizeSelectionObjects(data);

            e.Handled = true;
        }
        private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            ShowSelectionSizeInfo(false);

            if (_infoItems.Count <= 0)
            {
                return;
            }

            if (designerItem == null)
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
                item.PropertyMementos.SetPropertyNewValue("Top", item.Raw_Top);
                item.PropertyMementos.SetPropertyNewValue("Left", item.Raw_Top);
                item.PropertyMementos.SetPropertyNewValue("ItemWidth", item.ItemWidth);
                item.PropertyMementos.SetPropertyNewValue("ItemHeight", item.ItemHeight);

                PropertyChangeCommand cmd = new PropertyChangeCommand(item, item.PropertyMementos);
                cmds.AddCommand(cmd);
            }
            WidgetViewModBase dynamicPanel = _infoItems.FirstOrDefault(x => x.Type == ObjectType.DynamicPanel);
            if (dynamicPanel != null)
            {
                ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<RefreshWidgetChildPageEvent>().Publish(dynamicPanel.widgetGID);

                RefreshDynamicPanelCommand cmd = new RefreshDynamicPanelCommand(dynamicPanel.widgetGID);
                cmds.AddCommand(cmd);
            }



            // Create undoable command for groups
            if (pageVMGroup != null)
            {
                List<Guid> groupGuids = new List<Guid>();

                // Rezise the group
                if (_group != null)
                {
                    groupGuids.Add(_group.WidgetID);
                }

                // Resize the widget in a group
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

        #region Event Handler(Additional Multi-Ojbects Operation Functions)


        /// <summary>
        /// Adgust drag-direction according to rotate angle.
        /// </summary>
        /// <param name="dragData">Drag info </param>
        /// <param name="angle">angle delta</param>
        private void AdjustDirection(ref DrageChangeData dragData, int angle)
        {
            if (angle < 0)
            {
                angle += 360;
            }


           //360度 : divide four parts(left,down,right,top)
            Key dragPart;
            if (angle < 45 || angle >= 315)
            {
                dragData.DragDirection.HorAlignment = HorizontalAlignment;
                dragData.DragDirection.VerAlignment = VerticalAlignment;
                return;
            }
            if (angle >= 45 && angle <= 135)
            {
                dragPart = Key.Right;
                CommonFunction.Swap<double>(ref dragData.ScaleX, ref dragData.ScaleY);
            }
            else if (angle >= 225 && angle < 315)
            {
                dragPart = Key.Left;
                CommonFunction.Swap<double>(ref dragData.ScaleX, ref dragData.ScaleY);
            }
            else
            {//angle>=135&& angle <225
                dragPart = Key.Down;
            }

            Directions dragDirection = new Directions();

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    {
                        if (dragPart == Key.Right)
                        {
                            dragDirection.VerAlignment = VerticalAlignment.Top;
                        }
                        else if (dragPart == Key.Left)
                        {
                            dragDirection.VerAlignment = VerticalAlignment.Bottom;
                        }
                        else
                        {
                            dragDirection.HorAlignment = HorizontalAlignment.Right;
                        }
                    }
                    break;
                case HorizontalAlignment.Right:
                    {
                        if (dragPart == Key.Right)
                        {
                            dragDirection.VerAlignment = VerticalAlignment.Bottom;
                        }
                        else if (dragPart == Key.Left)
                        {
                            dragDirection.VerAlignment = VerticalAlignment.Top;
                        }
                        else
                        {
                            dragDirection.HorAlignment = HorizontalAlignment.Left;
                        }                      
                    }
                    break;
                default:
                    {
                        if (dragPart == Key.Down)
                        {
                            dragDirection.HorAlignment = HorizontalAlignment.Stretch;
                        }
                        else
                        {
                            dragDirection.VerAlignment = VerticalAlignment.Stretch;
                        }   
                    }
                    break;
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    {
                        if (dragPart == Key.Right)
                        {
                            dragDirection.HorAlignment = HorizontalAlignment.Right;
                        }
                        else if (dragPart == Key.Left)
                        {
                            dragDirection.HorAlignment = HorizontalAlignment.Left;
                        }
                        else
                        {
                            dragDirection.VerAlignment = VerticalAlignment.Bottom;
                        }
                    }
                    break;
                case VerticalAlignment.Bottom:
                    {
                        if (dragPart == Key.Right)
                        {
                            dragDirection.HorAlignment = HorizontalAlignment.Left;
                        }
                        else if (dragPart == Key.Left)
                        {
                            dragDirection.HorAlignment = HorizontalAlignment.Right;
                        }
                        else
                        {
                            dragDirection.VerAlignment = VerticalAlignment.Top;
                        }
                    }
                    break;
                default:
                    {
                        if (dragPart == Key.Down)
                        {
                            dragDirection.VerAlignment = VerticalAlignment.Stretch;                            
                        }   
                        else
                        {
                            dragDirection.HorAlignment = HorizontalAlignment.Stretch;
                        }
                    }
                    break;
            }
            dragData.DragDirection = dragDirection;       
        }
        private void ShowSelectionSizeInfo(bool bIsShow)
        {
            foreach (WidgetViewModBase widget in _selectionService.GetSelectedWidgets())
            {
                widget.IsResing = bIsShow ? Visibility.Visible : Visibility.Collapsed;
            }

        }
        private void InitializeSelectionUndoStack()
        {
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();

            foreach (var widget in allSelects)
            {
                if (widget.IsGroup == false)
                {
                    WidgetViewModBase wdg = widget as WidgetViewModBase;
                    wdg.CreateNewPropertyMementos();

                    wdg.PropertyMementos.AddPropertyMemento(new PropertyMemento("Top", wdg.Raw_Top, wdg.Raw_Top));
                    wdg.PropertyMementos.AddPropertyMemento(new PropertyMemento("Left", wdg.Raw_Left, wdg.Raw_Left));
                    wdg.PropertyMementos.AddPropertyMemento(new PropertyMemento("ItemWidth", wdg.ItemWidth, wdg.ItemWidth));
                    wdg.PropertyMementos.AddPropertyMemento(new PropertyMemento("ItemHeight", wdg.ItemHeight, wdg.ItemHeight));
                    _infoItems.Add(wdg);
                }
                else
                {
                    GroupViewModel group = widget as GroupViewModel;
                    if (group == null)
                    {
                        return;
                    }
                    foreach (WidgetViewModBase child in group.WidgetChildren)
                    {
                        child.CreateNewPropertyMementos();
                        child.PropertyMementos.AddPropertyMemento(new PropertyMemento("Top", child.Raw_Top, child.Raw_Top));
                        child.PropertyMementos.AddPropertyMemento(new PropertyMemento("Left", child.Raw_Left, child.Raw_Left));
                        child.PropertyMementos.AddPropertyMemento(new PropertyMemento("ItemWidth", child.ItemWidth, child.ItemWidth));
                        child.PropertyMementos.AddPropertyMemento(new PropertyMemento("ItemHeight", child.ItemHeight, child.ItemHeight));
                        _infoItems.Add(child);
                    }
                    _group = group;
                }
            }

        }
        private DrageChangeData GetDragChangeData(DragDeltaEventArgs e)
        {
            DrageChangeData data;
            if (this.designerItem.IsGroup == false)
            {
                if (GlobalData.IsLockRatio || (Keyboard.Modifiers & (ModifierKeys.Shift)) != ModifierKeys.None)
                {
                    if (VerticalAlignment != System.Windows.VerticalAlignment.Stretch
                        && HorizontalAlignment != System.Windows.HorizontalAlignment.Stretch)
                    {
                        data = GetDragWidgetProRataData(e);
                    }
                    else
                    {
                        data = GetDragWidgetData(e);
                    }
                }
                else
                {
                    data = GetDragWidgetData(e);
                }

            }
            else
            {
                if (GlobalData.IsLockRatio || (Keyboard.Modifiers & (ModifierKeys.Shift)) != ModifierKeys.None)
                {
                    if (VerticalAlignment != System.Windows.VerticalAlignment.Stretch
                        && HorizontalAlignment != System.Windows.HorizontalAlignment.Stretch)
                    {
                        data = GetDragGroupProRataData(e);
                    }
                    else
                    {
                        data = GetDragGroupData(e);
                    }

                }
                else
                {
                    data = GetDragGroupData(e);
                }
            }
            data.DragDirection.HorAlignment = HorizontalAlignment;
            data.DragDirection.VerAlignment = VerticalAlignment;

            return data;
        }

        private void ResizeSelectionObjects(DrageChangeData e)
        { 
            List<IWidgetPropertyData> allSelects = _selectionService.GetSelectedWidgets();
            foreach(IWidgetPropertyData item in allSelects)
            {
                if(item.IsGroup==true)
                {
                    GroupViewModel group = item as GroupViewModel;
                    ResizeGroup(e, group);
                }
                else
                {
                    WidgetViewModBase wdg= item as WidgetViewModBase;
                    if (wdg.IsLocked == true || wdg.CanDragResize==false)
                    {
                        continue;
                    }
                    ResizeWidget(e, wdg);
                }
            }

            if (this.designerItem.ParentID != Guid.Empty)
            {
                IGroupOperation pageVM = canvas.DataContext as IGroupOperation;
                pageVM.UpdateGroup(this.designerItem.ParentID);
            }
        }
        private void ResizeWidget(DrageChangeData e,WidgetViewModBase obj)
        {

            WidgetViewModBase dragWidget = designerItem.DataContext as WidgetViewModBase;
            if (obj.widgetGID != dragWidget.widgetGID)
            {
                AdjustDirection(ref e, dragWidget.RotateAngle - obj.RotateAngle);
            }

            switch(e.DragDirection.HorAlignment)
            {
                case HorizontalAlignment.Left:
                    DragWidgetLeft(e.ScaleX, obj);
                    break;
                case HorizontalAlignment.Right:
                    DragWidgetRight(e.ScaleX, obj);
                    break;
                default:
                    break;
            }
            switch(e.DragDirection.VerAlignment)
            {
                case VerticalAlignment.Top:
                    DragWidgetTop(e.ScaleY, obj);
                    break;
                case VerticalAlignment.Bottom:
                    DragWidgetBottom(e.ScaleY, obj);
                    break;
                default:
                    break;
            }        

        }
        private void ResizeGroup(DrageChangeData e,WidgetViewModBase obj)
        {
            GroupViewModel groupVM = obj as GroupViewModel;
            if (groupVM == null)
            {
                return;
            }

            Double w = groupVM.ItemWidth;
            Double h = groupVM.ItemHeight;
            if((w*e.ScaleX<5)||(h*e.ScaleY<5))
            {
                return;
            }

            WidgetViewModBase dragWidget = designerItem.DataContext as WidgetViewModBase;
            if (groupVM.widgetGID != dragWidget.widgetGID)
            {
                AdjustDirection(ref e, dragWidget.RotateAngle - groupVM.RotateAngle);
            }

            if(e.DragDirection.VerAlignment == VerticalAlignment.Stretch)
            {
                switch (e.DragDirection.HorAlignment)
                {
                    case HorizontalAlignment.Left:
                        DragGroupLeft(e.ScaleX, groupVM);
                        break;
                    case HorizontalAlignment.Right:
                        DragGroupRight(e.ScaleX, groupVM);
                        break;
                    default:
                        break;
                }
            }
            else if(e.DragDirection.HorAlignment == HorizontalAlignment.Stretch)
            {
                switch (e.DragDirection.VerAlignment)
                {
                    case VerticalAlignment.Top:
                        DragGroupTop(e.ScaleY, groupVM);
                        break;
                    case VerticalAlignment.Bottom:
                        DragGroupBottom(e.ScaleY, groupVM);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if(e.DragDirection.VerAlignment==VerticalAlignment.Top&&e.DragDirection.HorAlignment==HorizontalAlignment.Left)
                {
                    DragGroupTopLeft(e.ScaleX, e.ScaleY, groupVM);
                }
                else if(e.DragDirection.VerAlignment==VerticalAlignment.Bottom&&e.DragDirection.HorAlignment==HorizontalAlignment.Left)
                {
                    DragGroupBottomLeft(e.ScaleX, e.ScaleY, groupVM);
                }
                else if(e.DragDirection.VerAlignment==VerticalAlignment.Top&&e.DragDirection.HorAlignment==HorizontalAlignment.Right)
                {
                    DragGroupTopRight(e.ScaleX, e.ScaleY, groupVM);
                }
                else
                {
                    DragGroupBottomRight(e.ScaleX, e.ScaleY, groupVM);
                }
            }

            //UI render
            IGroupOperation pageVM = canvas.DataContext as IGroupOperation;
            Guid groupID = groupVM.widgetGID;
            pageVM.UpdateGroup(groupID);

        }
        #endregion
    }

    public struct Directions
    {
       public HorizontalAlignment HorAlignment;
       public VerticalAlignment VerAlignment;
    }
    public struct DrageChangeData
    {
        public double ScaleX;
        public double ScaleY;
        public Directions DragDirection;
    }

    #region Older Resize Thumb
    public class ResizeThumb0 : Thumb
    {
        #region Constructor
        public ResizeThumb0()
        {
            _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();

            _infoItems = new List<WidgetViewModBase>();
            _group = null;
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
            DragCompleted += new DragCompletedEventHandler(this.ResizeThumb_DragCompleted);
        }
        #endregion

        #region private member
        private RotateTransform rotateTransform;
        private double angle;
        private Adorner adorner;
        private System.Windows.Point transformOrigin;
        private BaseWidgetItem designerItem;
        private DesignerCanvas canvas;
        private IPage _page;
        private ISelectionService _selectionService;


        //Redo/Undo/Selected Widget information item
        private List<WidgetViewModBase> _infoItems;
        GroupViewModel _group;
        //TODO:DELETE
        ContentPresenter ItemWrapper;

        IDocument _document
        {
            get
            {
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                return doc.Document;
            }
        }
        #endregion

        #region Widget Operation
        private void DragWidgetTop(double delta)
        {
            double deltaVertical = delta;

            (ItemWrapper.DataContext as WidgetViewModBase).Raw_Left = Canvas.GetLeft(ItemWrapper) + deltaVertical * Math.Sin(-this.angle) - (this.transformOrigin.Y * deltaVertical * Math.Sin(-this.angle));
            (ItemWrapper.DataContext as WidgetViewModBase).Raw_Top = Canvas.GetTop(ItemWrapper) + deltaVertical * Math.Cos(-this.angle) + (this.transformOrigin.Y * deltaVertical * (1 - Math.Cos(-this.angle)));

            this.designerItem.Height = ItemWrapper.ActualHeight - deltaVertical;
        }
        private void DragWidgetBottom(double delta)
        {
            double deltaVertical = delta;

            (ItemWrapper.DataContext as WidgetViewModBase).Raw_Left = Canvas.GetLeft(ItemWrapper) - deltaVertical * this.transformOrigin.Y * Math.Sin(-this.angle);
            (ItemWrapper.DataContext as WidgetViewModBase).Raw_Top = Canvas.GetTop(ItemWrapper) + (this.transformOrigin.Y * deltaVertical * (1 - Math.Cos(-this.angle)));
            this.designerItem.Height = ItemWrapper.ActualHeight - deltaVertical;
        }
        private void DragWidgetLeft(double delta)
        {
            double deltaHorizontal = delta;

            (ItemWrapper.DataContext as WidgetViewModBase).Raw_Top = Canvas.GetTop(ItemWrapper) + deltaHorizontal * Math.Sin(this.angle) - this.transformOrigin.X * deltaHorizontal * Math.Sin(this.angle);
            (ItemWrapper.DataContext as WidgetViewModBase).Raw_Left = Canvas.GetLeft(ItemWrapper) + deltaHorizontal * Math.Cos(this.angle) + (this.transformOrigin.X * deltaHorizontal * (1 - Math.Cos(this.angle)));
            this.designerItem.Width = ItemWrapper.ActualWidth - deltaHorizontal;
        }
        private void DragWidgetRight(double delta)
        {
            double deltaHorizontal = delta;

            (ItemWrapper.DataContext as WidgetViewModBase).Raw_Top = Canvas.GetTop(ItemWrapper) - this.transformOrigin.X * deltaHorizontal * Math.Sin(this.angle);
            (ItemWrapper.DataContext as WidgetViewModBase).Raw_Left = Canvas.GetLeft(ItemWrapper) + (deltaHorizontal * this.transformOrigin.X * (1 - Math.Cos(this.angle)));

            this.designerItem.Width = ItemWrapper.ActualWidth - deltaHorizontal;
        }
        #endregion

        #region Group Operation part_1(line)
        private void DragGroupTop(double scale)
        {
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupBottom = Canvas.GetTop(wrapper) + this.designerItem.Height;
            if (scale == 1)
            {
                return;
            }

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double deltaY = (groupBottom - ItemTop) * (scale - 1);
                BoundRect.Y -= deltaY;
                if ((item.RotateAngle % 90) != 45)
                {
                    BoundRect.Height *= scale;
                }
                else
                {
                    BoundRect.Height *= scale;
                    BoundRect.Width *= scale;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    if (groupBottom > BoundRect.Bottom)
                    {
                        item.Top -= deltaY;
                        //item.ItemHeight *= scale;

                        //item.ItemHeight *= scale;
                        //item.Top = BoundRect.Y + 0.5 * (BoundRect.Height - item.ItemHeight);
                    }
                    //item.Top -= deltaY;
                    //item.ItemHeight *= scale;
                    continue;
                }
                if (realRec.Width < 10 || realRec.Height < 10)
                {
                    realRec.Width = Math.Max(10, realRec.Width);
                    realRec.Height = Math.Max(10, realRec.Height);
                }
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {
                    continue;
                }

                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }

        }
        private void DragGroupBottom(double scale)
        {
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupTop = Canvas.GetTop(wrapper);
            if (scale == 1)
            {
                return;
            }

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double deltaY = (ItemTop - groupTop) * (scale - 1);
                BoundRect.Y += deltaY;
                if ((item.RotateAngle % 90) != 45)
                {
                    BoundRect.Height *= scale;
                }
                else
                {
                    BoundRect.Height *= scale;
                    BoundRect.Width *= scale;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    item.Top += deltaY;
                    //item.ItemHeight *= scale;
                    continue;
                }

                // if (realRec.Top < 0 || realRec.Left < 0 || realRec.Width < 10 || realRec.Height < 10 || realRec.Right < 0 || realRec.Bottom < 0)                 

                if (realRec.Width < 10 || realRec.Height < 10)
                {
                    realRec.Width = Math.Max(10, realRec.Width);
                    realRec.Height = Math.Max(10, realRec.Height);
                }
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {

                    continue;
                }
                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }
        }
        private void DragGroupLeft(double scale)
        {
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupRight = Canvas.GetLeft(wrapper) + this.designerItem.Width;
            if (scale == 1)
            {
                return;
            }

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemLeft = BoundRect.Left;
                double deltaX = (groupRight - ItemLeft) * (scale - 1);
                BoundRect.X -= deltaX;
                if ((item.RotateAngle % 90) != 45)
                {
                    BoundRect.Width *= scale;
                }
                else
                {
                    BoundRect.Height *= scale;
                    BoundRect.Width *= scale;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    if (groupRight > BoundRect.Right)
                    {
                        item.Left -= deltaX;
                    }
                    continue;
                }
                if (realRec.Width < 10 || realRec.Height < 10)
                {
                    realRec.Width = Math.Max(10, realRec.Width);
                    realRec.Height = Math.Max(10, realRec.Height);
                }
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Height - BoundRect.Height) > 0.2)
                {
                    continue;
                }

                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }
        }
        private void DragGroupRight(double scale)
        {
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupLeft = Canvas.GetLeft(wrapper);
            if (scale == 1)
            {
                return;
            }

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;

                Rect BoundRect = item.GetBoundingRectangle(false);

                double ItemLeft = BoundRect.Left;
                double deltaX = (ItemLeft - groupLeft) * (scale - 1);
                //double deltaW = BoundRect.Width * scale;
                BoundRect.X += deltaX;
                if ((item.RotateAngle % 90) != 45)
                {
                    BoundRect.Width *= scale;
                }
                else
                {
                    BoundRect.Height *= scale;
                    BoundRect.Width *= scale;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    item.Left += deltaX;
                    //item.ItemWidth *= scale;
                    continue;
                }
                if (realRec.Width < 10 || realRec.Height < 10)
                {
                    realRec.Width = Math.Max(10, realRec.Width);
                    realRec.Height = Math.Max(10, realRec.Height);
                }
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Height - BoundRect.Height) > 0.2)
                {
                    continue;
                }

                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }
        }
        #endregion

        #region Group Operation part_2(position)
        private void DragGroupTopLeft(double scaleW, double scaleH)
        {
            if (scaleW == 1 && scaleH == 1)
            {
                return;
            }
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupBottom = Canvas.GetTop(wrapper) + this.designerItem.Height;
            double groupRight = Canvas.GetLeft(wrapper) + this.designerItem.Width;

            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;
                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double ItemLeft = BoundRect.Left;

                double angle = Math.Abs(item.RotateAngle) % 90;
                if (angle == 45)
                {
                    double deltaY = (groupBottom - ItemTop) * (Math.Max(scaleH, scaleW) - 1);
                    double deltaX = (groupRight - ItemLeft) * (Math.Max(scaleH, scaleW) - 1);
                    BoundRect.Y -= deltaY;
                    BoundRect.X -= deltaX;
                    BoundRect.Height *= Math.Max(scaleH, scaleW);
                    BoundRect.Width *= Math.Max(scaleH, scaleW);
                }
                else
                {
                    double deltaY = (groupBottom - ItemTop) * (scaleH - 1);
                    double deltaX = (groupRight - ItemLeft) * (scaleW - 1);
                    BoundRect.Y -= deltaY;
                    BoundRect.X -= deltaX;
                    BoundRect.Height *= scaleH;
                    BoundRect.Width *= scaleW;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    continue;
                }
                if (realRec.Width < 10 || realRec.Height < 10)
                {
                    realRec.Width = Math.Max(10, realRec.Width);
                    realRec.Height = Math.Max(10, realRec.Height);
                }
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {
                    continue;
                }
                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }

        }
        private void DragGroupTopRight(double scaleW, double scaleH)
        {
            if (scaleW == 1 && scaleH == 1)
            {
                return;
            }
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupBottom = Canvas.GetTop(wrapper) + this.designerItem.Height;
            double groupLeft = Canvas.GetLeft(wrapper);


            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double ItemLeft = BoundRect.Left;

                double angle = Math.Abs(item.RotateAngle) % 90;
                if (angle == 45)
                {
                    double deltaY = (groupBottom - ItemTop) * (Math.Max(scaleH, scaleW) - 1);
                    double deltaX = (ItemLeft - groupLeft) * (Math.Max(scaleH, scaleW) - 1);
                    BoundRect.Y -= deltaY;
                    BoundRect.X += deltaX;
                    BoundRect.Height *= Math.Max(scaleH, scaleW);
                    BoundRect.Width *= Math.Max(scaleH, scaleW);
                }
                else
                {
                    double deltaY = (groupBottom - ItemTop) * (scaleH - 1);
                    double deltaX = (ItemLeft - groupLeft) * (scaleW - 1);
                    BoundRect.Y -= deltaY;
                    BoundRect.X += deltaX;
                    BoundRect.Height *= scaleH;
                    BoundRect.Width *= scaleW;
                    //if (scaleH < 1 || scaleW < 1)
                    //{
                    //    int i = 0;
                    //}
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    continue;
                }
                if (realRec.Width < 10 || realRec.Height < 10)
                {
                    realRec.Width = Math.Max(10, realRec.Width);
                    realRec.Height = Math.Max(10, realRec.Height);
                }
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {
                    continue;
                }
                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }

        }
        private void DragGroupBottomLeft(double scaleW, double scaleH)
        {
            if (scaleW == 1 && scaleH == 1)
            {
                return;
            }
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupTop = Canvas.GetTop(wrapper);
            double groupRight = Canvas.GetLeft(wrapper) + this.designerItem.Width;


            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double ItemLeft = BoundRect.Left;

                double angle = Math.Abs(item.RotateAngle) % 90;
                if (angle == 45)
                {
                    double deltaY = (ItemTop - groupTop) * (Math.Max(scaleH, scaleW) - 1);
                    double deltaX = (groupRight - ItemLeft) * (Math.Max(scaleH, scaleW) - 1);
                    BoundRect.Y += deltaY;
                    BoundRect.X -= deltaX;
                    BoundRect.Height *= Math.Max(scaleH, scaleW);
                    BoundRect.Width *= Math.Max(scaleH, scaleW);
                }
                else
                {
                    double deltaY = (ItemTop - groupTop) * (scaleH - 1);
                    double deltaX = (groupRight - ItemLeft) * (scaleW - 1);
                    BoundRect.Y += deltaY;
                    BoundRect.X -= deltaX;
                    BoundRect.Height *= scaleH;
                    BoundRect.Width *= scaleW;
                }

                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    continue;
                }
                if (realRec.Width < 10 || realRec.Height < 10)
                {
                    realRec.Width = Math.Max(10, realRec.Width);
                    realRec.Height = Math.Max(10, realRec.Height);
                }
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {
                    continue;
                }
                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }
        }
        private void DragGroupBottomRight(double scaleW, double scaleH)
        {
            if (scaleW == 1 && scaleH == 1)
            {
                return;
            }
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            Guid groupID = groupVM.widgetGID;
            ContentPresenter wrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            double groupTop = Canvas.GetTop(wrapper);
            double groupLeft = Canvas.GetLeft(wrapper);


            foreach (WidgetViewModBase item in groupVM.WidgetChildren)
            {
                if (item.ParentID != groupID)
                    continue;

                Rect BoundRect = item.GetBoundingRectangle(false);
                double ItemTop = BoundRect.Top;
                double ItemLeft = BoundRect.Left;


                double angle = Math.Abs(item.RotateAngle) % 90;
                if (angle == 45)
                {
                    double deltaY = (ItemTop - groupTop) * (Math.Max(scaleH, scaleW) - 1);
                    double deltaX = (ItemLeft - groupLeft) * (Math.Max(scaleH, scaleW) - 1);
                    BoundRect.Y += deltaY;
                    BoundRect.X += deltaX;
                    BoundRect.Height *= Math.Max(scaleH, scaleW);
                    BoundRect.Width *= Math.Max(scaleH, scaleW);
                }
                else
                {
                    double deltaY = (ItemTop - groupTop) * (scaleH - 1);
                    double deltaX = (ItemLeft - groupLeft) * (scaleW - 1);
                    BoundRect.Y += deltaY;
                    BoundRect.X += deltaX;
                    BoundRect.Height *= scaleH;
                    BoundRect.Width *= scaleW;
                }


                Rect realRec = item.RevertBoundingRectangle(BoundRect);
                if (realRec.Width > CommonDefine.MaxEditorWidth)
                {
                    continue;
                }
                if (realRec.Width < 10 || realRec.Height < 10)
                {
                    realRec.Width = Math.Max(10, realRec.Width);
                    realRec.Height = Math.Max(10, realRec.Height);
                }
                Rect validRec = item.GetBoundingRectangle(item.RotateAngle, realRec);
                if (Math.Abs(validRec.Width - BoundRect.Width) > 0.2)
                {
                    continue;
                }
                item.Top = realRec.Top;
                item.Left = realRec.Left;
                item.ItemWidth = realRec.Width;
                item.ItemHeight = realRec.Height;
            }
        }
        #endregion

        #region Pirvate Functions
        private void DragWidget(DragDeltaEventArgs e)
        {
            double deltaVertical = 0, deltaHorizontal = 0;
            switch (VerticalAlignment)
            {
                case System.Windows.VerticalAlignment.Bottom:
                    double snapBottomChange = e.VerticalChange;
                    double snapBottom = Canvas.GetTop(ItemWrapper) + (ItemWrapper.ActualHeight + e.VerticalChange);
                    double snapBottomDelta = CalculateSnapChangeDelta(snapBottom, Service.Document.Orientation.Horizontal);
                    snapBottomChange += snapBottomDelta;

                    deltaVertical = Math.Min(-snapBottomChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    DragWidgetBottom(deltaVertical);
                    break;

                case System.Windows.VerticalAlignment.Top:
                    double snapTopChange = e.VerticalChange;
                    double snapTop = Canvas.GetTop(ItemWrapper) + e.VerticalChange;
                    double snapTopDelta = CalculateSnapChangeDelta(snapTop, Service.Document.Orientation.Horizontal);
                    snapTopChange += snapTopDelta;

                    deltaVertical = Math.Min(snapTopChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    DragWidgetTop(deltaVertical);
                    break;

                default:
                    break;
            }

            switch (HorizontalAlignment)
            {
                case System.Windows.HorizontalAlignment.Left:
                    double snapLeftChange = e.HorizontalChange;
                    double snapLeft = Canvas.GetLeft(ItemWrapper) + e.HorizontalChange;
                    double snapLeftDelta = CalculateSnapChangeDelta(snapLeft, Service.Document.Orientation.Vertical);
                    snapLeftChange += snapLeftDelta;

                    deltaHorizontal = Math.Min(snapLeftChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    DragWidgetLeft(deltaHorizontal);
                    break;

                case System.Windows.HorizontalAlignment.Right:
                    double snapRightChange = e.HorizontalChange;
                    double snapRight = Canvas.GetLeft(ItemWrapper) + (ItemWrapper.ActualWidth + e.HorizontalChange);
                    double snapRightDelta = CalculateSnapChangeDelta(snapRight, Service.Document.Orientation.Vertical);
                    snapRightChange += snapRightDelta;

                    deltaHorizontal = Math.Min(-snapRightChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    DragWidgetRight(deltaHorizontal);
                    break;

                default:
                    break;
            }

            //Debug.WriteLine("e.v--e.h---v---h:  " + e.VerticalChange + " , " + e.HorizontalChange + " , " + deltaVertical + " , " + deltaHorizontal);
        }
        private void DragWidgetProRata(DragDeltaEventArgs e)
        {
            double height, width;
            if (VerticalAlignment == System.Windows.VerticalAlignment.Bottom
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
            {
                height = ItemWrapper.ActualHeight + e.VerticalChange;
                width = ItemWrapper.ActualWidth + e.HorizontalChange;
                //height = ItemWrapper.ActualHeight + e.VerticalChange;
                //width = ItemWrapper.ActualWidth + e.HorizontalChange;
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Bottom
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
            {
                height = ItemWrapper.ActualHeight + e.VerticalChange;
                width = ItemWrapper.ActualWidth - e.HorizontalChange;
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Top
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
            {
                height = ItemWrapper.ActualHeight - e.VerticalChange;
                width = ItemWrapper.ActualWidth - e.HorizontalChange;
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Top
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
            {
                height = ItemWrapper.ActualHeight - e.VerticalChange;
                width = ItemWrapper.ActualWidth + e.HorizontalChange;
            }
            else
            {
                return;
            }


            if (height < ItemWrapper.MinHeight || width < ItemWrapper.MinWidth)
            {
                height = Math.Max(10, height);
                width = Math.Max(10, width);
            }
            double rata = Math.Max(height / ItemWrapper.ActualHeight, width / ItemWrapper.ActualWidth);

            double deltaVertical = ItemWrapper.ActualHeight - ItemWrapper.ActualHeight * rata;
            double deltaHorizontal = ItemWrapper.ActualWidth - ItemWrapper.ActualWidth * rata;
            //Debug.WriteLine("e.v--e.h---v---h:  " + e.VerticalChange + " , " + e.HorizontalChange + " , " + deltaVertical + " , " + deltaHorizontal);
            switch (VerticalAlignment)
            {
                case System.Windows.VerticalAlignment.Bottom:
                    DragWidgetBottom(deltaVertical);
                    break;

                case System.Windows.VerticalAlignment.Top:
                    DragWidgetTop(deltaVertical);
                    break;

                default:
                    break;
            }

            switch (HorizontalAlignment)
            {
                case System.Windows.HorizontalAlignment.Left:
                    DragWidgetLeft(deltaHorizontal);
                    break;

                case System.Windows.HorizontalAlignment.Right:
                    DragWidgetRight(deltaHorizontal);
                    break;

                default:
                    break;
            }
        }

        private void DragGroup(DragDeltaEventArgs e)
        {
            double deltaVertical, deltaHorizontal, scaleY = 1, scaleX = 1;
            double minscaleX, minscaleY;
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            if (groupVM == null)
            {
                return;
            }

            //Get the correct prorata--(Max Value)
            switch (VerticalAlignment)
            {
                case System.Windows.VerticalAlignment.Bottom:
                    double snapBottomChange = e.VerticalChange;
                    double snapBottom = Canvas.GetTop(ItemWrapper) + (ItemWrapper.ActualHeight + e.VerticalChange);
                    double snapBottomDelta = CalculateSnapChangeDelta(snapBottom, Service.Document.Orientation.Horizontal);
                    snapBottomChange += snapBottomDelta;

                    deltaVertical = Math.Min(-snapBottomChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    scaleY = (ItemWrapper.ActualHeight - deltaVertical) / ItemWrapper.ActualHeight;
                    //scaleY = Math.Max(minscaleY, scaleY);
                    //DragGroupBottom(scaleY);
                    break;

                case System.Windows.VerticalAlignment.Top:
                    double snapTopChange = e.VerticalChange;
                    double snapTop = Canvas.GetTop(ItemWrapper) + e.VerticalChange;
                    double snapTopDelta = CalculateSnapChangeDelta(snapTop, Service.Document.Orientation.Horizontal);
                    snapTopChange += snapTopDelta;
                    deltaVertical = Math.Min(snapTopChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    scaleY = (ItemWrapper.ActualHeight - deltaVertical) / ItemWrapper.ActualHeight;
                    //scaleY = Math.Max(minscaleY, scaleY);
                    //DragGroupTop(scaleY);
                    break;

                default:
                    break;
            }
            switch (HorizontalAlignment)
            {
                case System.Windows.HorizontalAlignment.Left:
                    double snapLeftChange = e.HorizontalChange;
                    double snapLeft = Canvas.GetLeft(ItemWrapper) + e.HorizontalChange;
                    double snapLeftDelta = CalculateSnapChangeDelta(snapLeft, Service.Document.Orientation.Vertical);
                    snapLeftChange += snapLeftDelta;
                    deltaHorizontal = Math.Min(snapLeftChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    scaleX = (ItemWrapper.ActualWidth - deltaHorizontal) / ItemWrapper.ActualWidth;
                    //scaleX = Math.Max(minscaleX, scaleX);
                    //DragGroupLeft(scaleX);
                    break;

                case System.Windows.HorizontalAlignment.Right:
                    double snapRightChange = e.HorizontalChange;
                    double snapRight = Canvas.GetLeft(ItemWrapper) + (ItemWrapper.ActualWidth + e.HorizontalChange);
                    double snapRightDelta = CalculateSnapChangeDelta(snapRight, Service.Document.Orientation.Vertical);
                    snapRightChange += snapRightDelta;
                    deltaHorizontal = Math.Min(-snapRightChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    scaleX = (ItemWrapper.ActualWidth - deltaHorizontal) / ItemWrapper.ActualWidth;
                    // scaleX = Math.Max(minscaleX, scaleX);
                    //DragGroupRight(scaleX);
                    break;

                default:
                    break;
            }

            //Set Value
            if (VerticalAlignment == System.Windows.VerticalAlignment.Bottom
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
            {
                //DragGroupBottomRight(scaleX, scaleY);
                DragGroupBottom(scaleY);
                DragGroupRight(scaleX);
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Bottom
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
            {
                //DragGroupBottomLeft(scaleX, scaleY);
                DragGroupBottom(scaleY);
                DragGroupLeft(scaleX);
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Top
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
            {
                //DragGroupTopLeft(scaleX, scaleY);
                DragGroupTop(scaleY);
                DragGroupLeft(scaleX);
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Top
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
            {
                //DragGroupTopRight(scaleX, scaleY);
                DragGroupTop(scaleY);
                DragGroupRight(scaleX);
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Top)
            {
                DragGroupTop(scaleY);
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Bottom)
            {
                DragGroupBottom(scaleY);
            }
            else if (HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
            {
                DragGroupLeft(scaleX);
            }
            else if (HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
            {
                DragGroupRight(scaleX);
            }
            else
            {
                return;
            }

            //UI render
            IGroupOperation pageVM = canvas.DataContext as IGroupOperation;
            Guid groupID = groupVM.widgetGID;
            pageVM.UpdateGroup(groupID);
        }
        private void DragGroupProRata(DragDeltaEventArgs e)
        {
            double deltaVertical, deltaHorizontal, scaleY = 1, scaleX = 1, scale = 1;
            GroupViewModel groupVM = this.designerItem.DataContext as GroupViewModel;
            if (groupVM == null)
            {
                return;
            }
            //Get the correct prorata--(Max Value)
            switch (VerticalAlignment)
            {
                case System.Windows.VerticalAlignment.Bottom:
                    double snapBottomChange = e.VerticalChange;

                    deltaVertical = Math.Min(-snapBottomChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    scaleY = (ItemWrapper.ActualHeight - deltaVertical) / ItemWrapper.ActualHeight;
                    //DragGroupBottom(scaleY);
                    break;

                case System.Windows.VerticalAlignment.Top:
                    double snapTopChange = e.VerticalChange;

                    deltaVertical = Math.Min(snapTopChange, ItemWrapper.ActualHeight - ItemWrapper.MinHeight);
                    scaleY = (ItemWrapper.ActualHeight - deltaVertical) / ItemWrapper.ActualHeight;
                    //DragGroupTop(scaleY);
                    break;

                default:
                    break;
            }
            switch (HorizontalAlignment)
            {
                case System.Windows.HorizontalAlignment.Left:
                    double snapLeftChange = e.HorizontalChange;

                    deltaHorizontal = Math.Min(snapLeftChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    scaleX = (ItemWrapper.ActualWidth - deltaHorizontal) / ItemWrapper.ActualWidth;
                    //DragGroupLeft(scaleX);
                    break;

                case System.Windows.HorizontalAlignment.Right:
                    double snapRightChange = e.HorizontalChange;

                    deltaHorizontal = Math.Min(-snapRightChange, ItemWrapper.ActualWidth - ItemWrapper.MinWidth);
                    scaleX = (ItemWrapper.ActualWidth - deltaHorizontal) / ItemWrapper.ActualWidth;
                    //DragGroupRight(scaleX);
                    break;

                default:
                    break;
            }
            scale = Math.Max(scaleX, scaleY);


            //Set Value
            if (VerticalAlignment == System.Windows.VerticalAlignment.Bottom
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
            {
                DragGroupBottomRight(scale, scale);
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Bottom
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
            {
                DragGroupBottomLeft(scale, scale);
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Top
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
            {
                DragGroupTopLeft(scale, scale);
            }
            else if (VerticalAlignment == System.Windows.VerticalAlignment.Top
                && HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
            {
                DragGroupTopRight(scale, scale);
            }
            else
            {
                return;
            }

            //UI render
            IGroupOperation pageVM = canvas.DataContext as IGroupOperation;
            Guid groupID = groupVM.widgetGID;
            pageVM.UpdateGroup(groupID);
        }

        /// <summary>
        /// Calculate snap value if Snap to Guide / Snap to Grid checked.
        /// </summary>
        /// <param name="valueToSnap">new left/top/bottom/right of widget</param>
        /// <param name="guideOrientation">only used for "snap to Guide"</param>
        /// <returns></returns>
        private double CalculateSnapChangeDelta(double valueToSnap, Service.Document.Orientation guideOrientation)
        {
            //Snap to Guide
            if (GlobalData.IsSnapToGuide)
            {
                IPageView pageView = _page.PageViews.GetPageView(_selectionService.GetCurrentPage().CurAdaptiveViewGID);
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
                //snap left/top of grid,example: valueToSnap%GRID_SIZE -> 122%10
                //left/top side of grid,not widget.
                if (snap <= CommonDefine.SnapMargin)
                {
                    snap = -snap;
                }
                else
                {
                    double temp = (valueToSnap + CommonDefine.SnapMargin) % GlobalData.GRID_SIZE;
                    //example:128%10
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
        #endregion

        #region event Handler
        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = this.DataContext as BaseWidgetItem;
            _infoItems.Clear();
            _group = null;
            if (this.designerItem == null)
                return;

            this.canvas = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem))) as DesignerCanvas;
            ItemWrapper = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this.designerItem)) as ContentPresenter;
            if (this.canvas != null)
            {
                this.transformOrigin = this.designerItem.RenderTransformOrigin;

                this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                if (this.rotateTransform != null)
                {
                    this.angle = this.rotateTransform.Angle * Math.PI / 180.0;
                }
                else
                {
                    this.angle = 0.0d;
                }

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.canvas);
                if (adornerLayer != null)
                {
                    this.adorner = new SizeAdorner(this.designerItem);
                    adornerLayer.Add(this.adorner);
                }

                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc.Document != null)
                {
                    //_page = doc.Document.Pages.GetPage((canvas.DataContext as IPagePropertyData).PageGID);
                    _page = (canvas.DataContext as IPagePropertyData).ActivePage;
                }
            }

        }
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Initialize the selected widgets' context when first move
            if (this.designerItem == null)
            {
                return;
            }
            if (_infoItems.Count <= 0)
            {
                if (this.designerItem.IsGroup == false)
                {
                    WidgetViewModBase wdg = designerItem.DataContext as WidgetViewModBase;
                    wdg.CreateNewPropertyMementos();

                    wdg.PropertyMementos.AddPropertyMemento(new PropertyMemento("Top", wdg.Raw_Top, wdg.Raw_Top));
                    wdg.PropertyMementos.AddPropertyMemento(new PropertyMemento("Left", wdg.Raw_Left, wdg.Raw_Left));
                    wdg.PropertyMementos.AddPropertyMemento(new PropertyMemento("ItemWidth", wdg.ItemWidth, wdg.ItemWidth));
                    wdg.PropertyMementos.AddPropertyMemento(new PropertyMemento("ItemHeight", wdg.ItemHeight, wdg.ItemHeight));
                    _infoItems.Add(wdg);
                }
                else
                {
                    GroupViewModel group = designerItem.DataContext as GroupViewModel;
                    if (group == null)
                    {
                        return;
                    }
                    foreach (WidgetViewModBase child in group.WidgetChildren)
                    {
                        child.CreateNewPropertyMementos();
                        child.PropertyMementos.AddPropertyMemento(new PropertyMemento("Top", child.Raw_Top, child.Raw_Top));
                        child.PropertyMementos.AddPropertyMemento(new PropertyMemento("Left", child.Raw_Left, child.Raw_Left));
                        child.PropertyMementos.AddPropertyMemento(new PropertyMemento("ItemWidth", child.ItemWidth, child.ItemWidth));
                        child.PropertyMementos.AddPropertyMemento(new PropertyMemento("ItemHeight", child.ItemHeight, child.ItemHeight));
                        _infoItems.Add(child);
                    }
                    _group = group;
                }

            }

            //Resize the current widget or group
            if (this.designerItem.IsGroup == false)
            {
                if (GlobalData.IsLockRatio || (Keyboard.Modifiers & (ModifierKeys.Shift)) != ModifierKeys.None)
                {
                    if (VerticalAlignment != System.Windows.VerticalAlignment.Stretch
                        && HorizontalAlignment != System.Windows.HorizontalAlignment.Stretch)
                    {
                        DragWidgetProRata(e);
                    }
                    else
                    {
                        DragWidget(e);
                    }

                }
                else
                {
                    DragWidget(e);
                }

            }
            else
            {
                if (GlobalData.IsLockRatio || (Keyboard.Modifiers & (ModifierKeys.Shift)) != ModifierKeys.None)
                {
                    if (VerticalAlignment != System.Windows.VerticalAlignment.Stretch
                        && HorizontalAlignment != System.Windows.HorizontalAlignment.Stretch)
                    {
                        DragGroupProRata(e);
                    }
                    else
                    {
                        DragGroup(e);
                    }

                }
                else
                {
                    DragGroup(e);
                }

            }

            if (this.designerItem.ParentID != Guid.Empty)
            {
                IGroupOperation pageVM = canvas.DataContext as IGroupOperation;
                pageVM.UpdateGroup(this.designerItem.ParentID);
            }

            e.Handled = true;
        }
        private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.adorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.canvas);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this.adorner);
                }
                this.adorner = null;
            }

            if (_infoItems.Count <= 0)
            {
                return;
            }

            if (designerItem == null)
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
                item.PropertyMementos.SetPropertyNewValue("Top", item.Raw_Top);
                item.PropertyMementos.SetPropertyNewValue("Left", item.Raw_Top);
                item.PropertyMementos.SetPropertyNewValue("ItemWidth", item.ItemWidth);
                item.PropertyMementos.SetPropertyNewValue("ItemHeight", item.ItemHeight);

                PropertyChangeCommand cmd = new PropertyChangeCommand(item, item.PropertyMementos);
                cmds.AddCommand(cmd);
            }
            WidgetViewModBase dynamicPanel = _infoItems.FirstOrDefault(x => x.Type == ObjectType.DynamicPanel);
            if (dynamicPanel != null)
            {
                ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<RefreshWidgetChildPageEvent>().Publish(dynamicPanel.widgetGID);

                RefreshDynamicPanelCommand cmd = new RefreshDynamicPanelCommand(dynamicPanel.widgetGID);
                cmds.AddCommand(cmd);
            }



            // Create undoable command for groups
            if (pageVMGroup != null)
            {
                List<Guid> groupGuids = new List<Guid>();

                // Rezise the group
                if (_group != null)
                {
                    groupGuids.Add(_group.WidgetID);
                }

                // Resize the widget in a group
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



    }
    #endregion
}
