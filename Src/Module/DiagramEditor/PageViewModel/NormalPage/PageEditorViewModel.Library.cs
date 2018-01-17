using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Common.Helper;
using System.Windows.Controls;
using System.Windows.Documents;
using Naver.Compass.Service;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Naver.Compass.Service.CustomLibrary;
using Microsoft.Win32;
using System.Diagnostics;

namespace Naver.Compass.Module
{
    public partial class PageEditorViewModel
    {
        #region Private Image Functions
        /// <summary>
        /// Create thumbNail for custom widget.
        /// </summary>
        /// <param name="bAllWidget">true: all widgets in the page, false: selected widgets in the page</param>
        /// <returns>ThumbNail icon</returns>
        private Stream CreateIconImage(bool bAllWidget)
        {
            if (this.EditorCanvas == null)
            {
                return null;
            }

            //Get real height and width property of image bound box
            double minleft = double.MaxValue;
            double mintop = double.MaxValue;
            double maxRight = double.MinValue;
            double maxBottom = double.MinValue;
            DesignerCanvas DeCanvas = _editorCanvas as DesignerCanvas;

            IEnumerable<BaseWidgetItem> widgets;
            if (bAllWidget)
            {
                if (DeCanvas.AllWidgetItems.Count() <= 0)
                    return null;

                widgets = DeCanvas.AllWidgetItems.OrderBy(i => (i.DataContext as WidgetViewModelDate).ZOrder);
            }
            else
            {
                if (DeCanvas.SelectedItemAndChildren.Count() <= 0)
                    return null;

                widgets = DeCanvas.SelectedItemAndChildren.OrderBy(i => (i.DataContext as WidgetViewModelDate).ZOrder);
            }


            foreach (BaseWidgetItem element in widgets)
            {
                WidgetViewModelDate vm = element.DataContext as WidgetViewModelDate;
                Rect rec = new Rect(vm.Left, vm.Top, element.ActualWidth, element.ActualHeight);
                if (vm.RotateAngle != 0)
                {
                    //Point offset = element.TransformToAncestor(parent).Transform(new Point(0, 0));
                    rec = new Rect(0, 0, element.ActualWidth, element.ActualHeight);
                    rec = element.TransformToAncestor(DeCanvas).TransformBounds(rec);

                }
                minleft = Math.Min(minleft, rec.Left);
                mintop = Math.Min(mintop, rec.Top);
                maxRight = Math.Max(maxRight, rec.Left + rec.Width);
                maxBottom = Math.Max(maxBottom, rec.Top + rec.Height);
            }
            double nBoundWdith = maxRight - minleft;
            double nBoundHeight = maxBottom - mintop;

            //map all widget into a DrawingVisual, make output image ready
            DrawingVisual tDrawingVisual = new DrawingVisual();
            ScaleTransform stf = new ScaleTransform();
            double scale;
            if (nBoundWdith > nBoundHeight)
            {
                scale = 74d / nBoundWdith;
            }
            else
            {
                scale = 74d / nBoundHeight;
            }
            stf.ScaleX = Math.Min(1d, scale);
            stf.ScaleY = Math.Min(1d, scale); 
            tDrawingVisual.Transform = stf;
            using (DrawingContext context = tDrawingVisual.RenderOpen())
            {
                foreach (BaseWidgetItem element in widgets)
                {
                    VisualBrush tVisualBrush = new VisualBrush(element);
                    tVisualBrush.Stretch = Stretch.Fill;
                    tVisualBrush.AlignmentX = 0;
                    tVisualBrush.AlignmentY = 0;

                    WidgetViewModelDate vm = element.DataContext as WidgetViewModelDate;
                    double x = vm.Left - minleft;
                    double y = vm.Top - mintop;

                    Rect rec = new Rect(x, y, element.ActualWidth, element.ActualHeight);
                    if (vm.RotateAngle != 0)
                    {
                        //Point offset = element.TransformToAncestor(parent).Transform(new Point(0, 0));
                        rec = new Rect(0, 0, element.ActualWidth, element.ActualHeight);
                        rec = element.TransformToAncestor(DeCanvas).TransformBounds(rec);
                        rec.X= rec.X - minleft;
                        rec.Y = rec.Y - mintop;
                    }

                    context.DrawRectangle(tVisualBrush, null, rec);
                }
                context.Close();
            }
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)(nBoundWdith * scale), (int)(nBoundHeight * scale), 96, 96, PixelFormats.Pbgra32);
            
            bmp.Render(tDrawingVisual);


            //Output the image stream.
            BitmapEncoder encoder;
            encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            MemoryStream stream = new MemoryStream();
            encoder.Save(stream);
            return stream;
        }
        private Stream CreateThumbNailImage()
        {
            if (this.EditorCanvas == null)
            {
                return null ;
            }
            try
            {
                //Get real height and width property of image bound box
                //double minleft = double.MaxValue;
                //double mintop = double.MaxValue;
                double maxRight = double.MinValue;
                double maxBottom = double.MinValue;
                DesignerCanvas DeCanvas = _editorCanvas as DesignerCanvas;
                if (DeCanvas.AllWidgetItems.Count() <= 0)
                {
                    return null;
                }

                IEnumerable<BaseWidgetItem> SortedWidgets =
                    DeCanvas.AllWidgetItems.OrderBy(i => (i.DataContext as WidgetViewModelDate).ZOrder);
                foreach (BaseWidgetItem element in SortedWidgets)
                {
                    WidgetViewModelDate vm = element.DataContext as WidgetViewModelDate;
                    Rect rec = new Rect(vm.Left, vm.Top, element.ActualWidth, element.ActualHeight);
                    if (vm.RotateAngle != 0)
                    {
                        //Point offset = element.TransformToAncestor(parent).Transform(new Point(0, 0));
                        rec = new Rect(0, 0, element.ActualWidth, element.ActualHeight);
                        rec = element.TransformToAncestor(DeCanvas).TransformBounds(rec);

                    }

                    maxRight = Math.Max(maxRight, rec.Left + rec.Width);
                    maxBottom = Math.Max(maxBottom, rec.Top + rec.Height);
                }

                maxRight = maxRight + 10d;
                maxBottom = maxBottom + 10d;
                double scale= 80/Math.Max(maxRight, maxBottom);
                scale = Math.Min(scale,1d);
                //scale = 1;
                //map all widget into a DrawingVisual, make output image ready
                DrawingVisual tDrawingVisual = new DrawingVisual();
                ScaleTransform stf = new ScaleTransform();


                stf.ScaleX = scale;
                stf.ScaleY = scale;
                tDrawingVisual.Transform = stf;
                using (DrawingContext context = tDrawingVisual.RenderOpen())
                {
                    foreach (BaseWidgetItem element in SortedWidgets)
                    {
                        VisualBrush tVisualBrush = new VisualBrush(element);                        
                        tVisualBrush.Stretch = Stretch.Fill;
                        tVisualBrush.AlignmentX = 0;
                        tVisualBrush.AlignmentY = 0;

                        WidgetViewModelDate vm = element.DataContext as WidgetViewModelDate;
                        Rect rec = new Rect(vm.Left, vm.Top, element.ActualWidth, element.ActualHeight);
                        if (vm.RotateAngle != 0)
                        {
                            //Point offset = element.TransformToAncestor(parent).Transform(new Point(0, 0));
                            rec = new Rect(0, 0, element.ActualWidth, element.ActualHeight);
                            rec = element.TransformToAncestor(DeCanvas).TransformBounds(rec);
                            
                        }
                        
                        context.DrawRectangle(tVisualBrush, null, rec);
                    }     
                    context.Close();
                }
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)(maxRight * scale), (int)(maxBottom * scale), 96, 96, PixelFormats.Pbgra32);
                bmp.Render(tDrawingVisual);


                //Output the image stream.
                BitmapEncoder encoder;
                encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));

                ////using (Stream stm = File.Create(@"d:\123.png"))
                ////{
                ////    encoder.Save(stm);
                ////}
                ////return;
                MemoryStream stream = new MemoryStream();
                encoder.Save(stream);
                return stream;
            }
            catch
            {
                return null;
            }
                    
        }
        #endregion

        #region Public Functions
        private ISerializeWriter GetSerializeWriter(ref string name)
        {
            if (_selectionService.WidgetNumber <= 0)
            {
                return null;
            }
            ISerializeWriter serializeWriter = _document.CreateSerializeWriter(CurAdaptiveViewGID);

            foreach (WidgetViewModBase wdgItem in _selectionService.GetSelectedWidgets())
            {
                //use widget's name if one widget, else "untitled"
                if (_selectionService.WidgetNumber == 1 && !string.IsNullOrEmpty(wdgItem.Name))
                {
                    name = wdgItem.Name;
                }

                if (wdgItem is GroupViewModel)
                {
                    IGroup group = (wdgItem as GroupViewModel).ExternalGroup;
                    if (group != null)
                    {
                        serializeWriter.AddGroup(group);
                    }
                }
                else if (wdgItem.WidgetModel != null)
                {
                    if (wdgItem.WidgetModel!= null)
                    {
                        wdgItem.WidgetModel.SerializeObject(serializeWriter);
                    }
                }
            }
            return serializeWriter;
        }
        public void CreateLibrary()
        {
            try
            {
                RenameWindow win = new RenameWindow();
                win.NewName = CommonDefine.Untitled;
                win.Owner = Application.Current.MainWindow;
                win.ShowDialog();

                if (win.Result.HasValue && win.Result.Value)
                {
                    string libraryName = win.NewName;

                    string objectName = CommonDefine.Untitled;
                    ISerializeWriter writer = GetSerializeWriter(ref objectName);
                    if (writer == null)
                        return;

                    //Create a new Library. Thumbnail is empty now, and waiting for a improvement
                    IDocumentService DocService = ServiceLocator.Current.GetInstance<IDocumentService>();
                    ILibrary newLibrary =
                        DocService.LibraryManager.CreateLibrary(writer, objectName, CreateIconImage(false), null);
                    if (newLibrary != null)
                    {
                        ICustomLibraryService customLibraryService = ServiceLocator.Current.GetInstance<ICustomLibraryService>();
                        customLibraryService.GetMyLibrary().CreateCustomLibrary(newLibrary.Guid, libraryName);
                    }
                } 
            
            }
            catch(Exception ex)
            {
                NLogger.Warn("Add Library failed. ex:{0}", ex.ToString());
                
                MessageBox.Show(GlobalData.FindResource("Error_Create_Library") + ex.Message);
            }           
           
        }
        public void AddToLibrary(ICustomLibrary customLibrary)
        {
            try
            {
                string objectName = CommonDefine.Untitled;
                ISerializeWriter writer = GetSerializeWriter(ref objectName);
                if (writer == null)
                    return;

                IDocumentService DocService = ServiceLocator.Current.GetInstance<IDocumentService>();
                ILibrary library = DocService.LibraryManager.GetLibrary(customLibrary.LibraryGID, customLibrary.FileName);
                if (library != null)
                {
                    ICustomObject newObject = library.AddCustomObject(writer, objectName, CreateIconImage(false), null);
                    if (newObject != null)
                    {
                        customLibrary.AddCustomObject(newObject.Guid);
                    }
                }
                else
                {
                    // If we cannot get the specific library, there is something wrong about this custom library.
                    // Refresh this custom library
                    customLibrary.Refresh();
                }
            }
            catch(Exception ex)
            {
                NLogger.Warn("Add to library failed. ex:{0}", ex.ToString());

                MessageBox.Show(GlobalData.FindResource("Error_Create_Library") + ex.Message);
            }        
        }
        public void ExportToLibrary()
        {
            try
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = CommonDefine.LibraryFilter;
                saveFile.FileName = CommonDefine.Untitled;
                if (saveFile.ShowDialog() == true)
                {
                    string objectName = CommonDefine.Untitled;
                    ISerializeWriter writer = GetSerializeWriter(ref objectName);
                    if (writer == null)
                        return;

                    //Create a new Library. Thumbnail is empty now, and waiting for a improvement
                    IDocumentService DocService = ServiceLocator.Current.GetInstance<IDocumentService>();
                    ILibrary newLibrary =
                        DocService.LibraryManager.CreateLibrary(writer, objectName, CreateIconImage(false), null);
                    if (newLibrary != null)
                    {
                        newLibrary.SaveCopyTo(saveFile.FileName);
                        DocService.LibraryManager.DeleteLibrary(newLibrary.Guid);
                    }
                }
            }
            catch (Exception ex)
            {
                NLogger.Warn("Export to Library failed. ex:{0}", ex.ToString());

                MessageBox.Show(GlobalData.FindResource("Error_Create_Library") + ex.Message);
            }
        }
        public void CreatePreviewImage()
        {
            if (_isThumbnailUpdate == false)
            {
                return;
            }
            else 
            {
                _model.PreviewThumbnailImage = CreateThumbNailImage();
            }


        }
        public void CreateCustomWidgetIcon(bool bAllWidget)
        {
            Stream icon = null;
            if (bAllWidget == false)
            {
                icon=CreateIconImage(false);
                _model.SetCustomPageIcon(icon);

            }
            else if (_isThumbnailUpdate == true)
            {

                icon = CreateIconImage(true);
                _model.SetCustomPageIcon(icon);
            }            
            Debug.WriteLine("+++++++Out put Custom Widgets");

        }
        public bool IsUseThumbnailAsIcon
        {
            get
            {
                return _model.IsUseThumbnailAsIcon;
            }
            set
            {
                if(_model.IsUseThumbnailAsIcon!=value)
                {
                    _model.IsUseThumbnailAsIcon = value;
                }

                if(_model.IsUseThumbnailAsIcon==true)
                {
                    CreateCustomWidgetIcon(true);
                }
            }

        }
        #endregion

        
    }
}
