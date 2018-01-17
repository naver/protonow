using Microsoft.Practices.Prism.Commands;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.WidgetLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Naver.Compass.Module
{
    class ImageEditorViewModel : ViewModelBase
    {
        public ImageEditType EditType { get; private set; }

        private PageEditorViewModel _pageEditorViewModel;

        private ImageWidgetViewModel _img;

        public ImageEditorViewModel(PageEditorViewModel pageViewModel)
        {
            this._pageEditorViewModel = pageViewModel;
            this.EditType = ImageEditType.None;
            SliceCrossCommand = new DelegateCommand<object>(SliceCrossCommandHandler);
            SliceHorizontalCommand = new DelegateCommand<object>(SliceHorizontalCommandHandler);
            SliceVerticalCommand = new DelegateCommand<object>(SliceVerticalCommandHandler);
            CancelCommand = new DelegateCommand<object>(CancelCommandHandler);
            CropCropCommand = new DelegateCommand<object>(CropCropCommandHandler);
            CropCutCommand = new DelegateCommand<object>(CropCutCommandHandler);
            CropCopyCommand = new DelegateCommand<object>(CropCopyCommandHandler);
        }

        public void IniParentVM(PageEditorViewModel pageViewModel)
        {
            this._pageEditorViewModel = pageViewModel;
        }


        private RelayCommand _mouseMoveCommand;
        public RelayCommand MouseMoveCommand
        {
            get
            {
                if (_mouseMoveCommand == null)
                    _mouseMoveCommand = new RelayCommand(param => MouseMove((MouseEventArgs)param));
                return _mouseMoveCommand;
            }
            set { _mouseMoveCommand = value; }
        }

        private RelayCommand _mouseDownCommand;
        public RelayCommand MouseDownCommand
        {
            get
            {
                if (_mouseDownCommand == null)
                    _mouseDownCommand = new RelayCommand(param => MouseDown((MouseEventArgs)param));
                return _mouseDownCommand;
            }
            set { _mouseDownCommand = value; }
        }

        private RelayCommand _mouseDoubleClickCommand;
        public RelayCommand MouseDoubleClickCommand
        {
            get
            {
                if (_mouseDoubleClickCommand == null)
                    _mouseDoubleClickCommand = new RelayCommand(param => MouseDoubleClick((MouseEventArgs)param));
                return _mouseDoubleClickCommand;
            }
            set { _mouseDoubleClickCommand = value; }
        }

        private RelayCommand _keyDownCommand;
        public RelayCommand KeyDownCommand
        {
            get
            {
                if (_keyDownCommand == null)
                    _keyDownCommand = new RelayCommand(param => KeyDown((KeyEventArgs)param));
                return _keyDownCommand;
            }
            set { _keyDownCommand = value; }
        }

        public DelegateCommand<object> SliceCrossCommand { get; set; }
        public DelegateCommand<object> SliceHorizontalCommand { get; set; }
        public DelegateCommand<object> SliceVerticalCommand { get; set; }
        public DelegateCommand<object> CancelCommand { get; set; }

        public DelegateCommand<object> CropCropCommand { get; set; }
        public DelegateCommand<object> CropCutCommand { get; set; }
        public DelegateCommand<object> CropCopyCommand { get; set; }

        public void SliceCrossCommandHandler(object obj)
        {
            SliceType = Module.SliceType.Cross;
        }

        public void SliceHorizontalCommandHandler(object obj)
        {
            SliceType = Module.SliceType.Horizontal;
        }

        public void SliceVerticalCommandHandler(object obj)
        {
            SliceType = Module.SliceType.Vertical;
        }

        public void CancelCommandHandler(object obj)
        {
            this.CancleEdit();
        }

        public void CropCropCommandHandler(object obj)
        {
            try
            {
                this.CropSelectedImage();
            }
            catch (Exception ex)
            {
                NLogger.Warn("Crop image failed. ex:{0}", ex.ToString());
            }
            this.CancleEdit();
        }

        public void CropCutCommandHandler(object obj)
        {
            try
            {
                this.CropAndCutSelectedImg();
            }
            catch (Exception ex)
            {
                NLogger.Warn("Crop and cut Image failed. ex:{0}", ex.ToString());
            }
            this.CancleEdit();
        }

        public void CropCopyCommandHandler(object obj)
        {
            try
            {
                this.CopySelectedArea();
            }
            catch (Exception ex)
            {
                NLogger.Warn("Copy image failed. ex:{0}", ex.ToString());
            }
            this.CancleEdit();
        }

        public void CancleEdit()
        {
            this._pageEditorViewModel.IsImageEditorVisible = false;
            SetEditType(ImageEditType.None);
        }

        public Thickness VerticalLineMargin { get; set; }
        public Thickness HorizontalLineMargin { get; set; }

        public void MouseMove(MouseEventArgs e)
        {
            if (SliceLineVisible)
            {
                var editor_grid = GetImageEditorGrid(e.Source as UIElement);
                if (editor_grid != null)
                {
                    var position = e.GetPosition(editor_grid);
                    VerticalLineMargin = new Thickness(position.X, 0, 0, 0);
                    HorizontalLineMargin = new Thickness(0, position.Y, 0, 0);
                    FirePropertyChanged("VerticalLineMargin");
                    FirePropertyChanged("HorizontalLineMargin");
                }
            }
        }

        public void MouseDown(MouseEventArgs e)
        {
            (e.Source as UIElement).Focus();
            if (SliceLineVisible)
            {
                var editor_grid = GetImageEditorGrid(e.Source as UIElement);
                if (editor_grid != null)
                {
                    var position = e.GetPosition(editor_grid);
                    position.X /= Scale;
                    position.Y /= Scale;
                    try
                    {
                        SliceSelectedImage(position);
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn("Slice image failed.ex:{0}", ex.ToString());
                    }

                    this.CancleEdit();
                }

                e.Handled = true;
            }
            else if (this.CropToolsVisible)
            {
                e.Handled = true;
            }
        }

        public void MouseDoubleClick(MouseEventArgs e)
        {
            this.CropCropCommandHandler(null);
        }

        public void KeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.CancleEdit();
            }

            e.Handled = true;
        }


        private void SliceSelectedImage(System.Windows.Point slicePoint)
        {
            if (this._img == null) { return; }

            var img = this._img;
            var tb = this.ScaleImage(img);

            var bitmapRotate = ImageProcess.RotateImage(tb, img.RotateAngle);

            var rectRotate = new Rect();
            if (img.RotateAngle == 0d)
            {
                rectRotate = new Rect(img.Left, img.Top, img.ItemWidth, img.ItemHeight);
            }
            else
            {
                var newSize = ImageProcess.RotateSize(img.ItemWidth, img.ItemHeight, img.RotateAngle);
                rectRotate = new Rect(
                    (img.Left + img.ItemWidth / 2) - newSize.Width / 2,
                    (img.Top + img.ItemHeight / 2) - newSize.Height / 2,
                    newSize.Width,
                    newSize.Height);
            }

            var rectangles = new List<Rect>();

            ///      |            |
            ///   1  |      2     |   3
            /// _____|____________|_______
            ///   4  |      5     |   6
            /// _____|____________|_______
            ///   7  |      8     |   9
            ///      |            |

            /// in rect 1,3,7,9
            if ((slicePoint.X <= rectRotate.Left && slicePoint.Y <= rectRotate.Top)
                || (slicePoint.X >= rectRotate.Left + rectRotate.Width && slicePoint.Y <= rectRotate.Top)
                || (slicePoint.X <= rectRotate.Left && slicePoint.Y >= rectRotate.Top + rectRotate.Height)
                || (slicePoint.X >= rectRotate.Left + rectRotate.Width && slicePoint.Y >= rectRotate.Top + rectRotate.Height))
            {
                return;
            }

            /// in rect 2,8
            if ((slicePoint.X > rectRotate.Left && slicePoint.X < rectRotate.Left + rectRotate.Width)
                && (slicePoint.Y <= rectRotate.Top || slicePoint.Y >= rectRotate.Top + rectRotate.Height))
            {
                if (SliceType != Module.SliceType.Horizontal)
                {
                    rectangles.Add(new Rect(0, 0, slicePoint.X - rectRotate.Left, rectRotate.Height));
                    rectangles.Add(new Rect(slicePoint.X - rectRotate.Left, 0, rectRotate.Width - (slicePoint.X - rectRotate.Left), rectRotate.Height));
                }
            }

            /// in rect 4,6
            if ((slicePoint.X <= rectRotate.Left || slicePoint.X >= rectRotate.Left + rectRotate.Width)
                && (slicePoint.Y > rectRotate.Top && slicePoint.Y < rectRotate.Top + rectRotate.Height))
            {
                if (SliceType != Module.SliceType.Vertical)
                {
                    rectangles.Add(new Rect(0, 0, rectRotate.Width, slicePoint.Y - rectRotate.Top));
                    rectangles.Add(new Rect(0, slicePoint.Y - rectRotate.Top, rectRotate.Width, rectRotate.Height - (slicePoint.Y - rectRotate.Top)));
                }
            }

            /// in rect 5
            if ((slicePoint.X > rectRotate.Left && slicePoint.X < rectRotate.Left + rectRotate.Width)
                && (slicePoint.Y > rectRotate.Top && slicePoint.Y < rectRotate.Top + rectRotate.Height))
            {
                if (SliceType == Module.SliceType.Vertical)
                {
                    rectangles.Add(new Rect(0, 0, slicePoint.X - rectRotate.Left, rectRotate.Height));
                    rectangles.Add(new Rect(slicePoint.X - rectRotate.Left, 0, rectRotate.Width - (slicePoint.X - rectRotate.Left), rectRotate.Height));

                }
                else if (SliceType == Module.SliceType.Horizontal)
                {
                    rectangles.Add(new Rect(0, 0, rectRotate.Width, slicePoint.Y - rectRotate.Top));
                    rectangles.Add(new Rect(0, slicePoint.Y - rectRotate.Top, rectRotate.Width, rectRotate.Height - (slicePoint.Y - rectRotate.Top)));
                }
                else if (SliceType == Module.SliceType.Cross)
                {
                    rectangles.Add(new Rect(0, 0, slicePoint.X - rectRotate.Left, slicePoint.Y - rectRotate.Top));
                    rectangles.Add(new Rect(slicePoint.X - rectRotate.Left, 0, rectRotate.Width - (slicePoint.X - rectRotate.Left), slicePoint.Y - rectRotate.Top));
                    rectangles.Add(new Rect(0, slicePoint.Y - rectRotate.Top, slicePoint.X - rectRotate.Left, rectRotate.Height - (slicePoint.Y - rectRotate.Top)));
                    rectangles.Add(new Rect(slicePoint.X - rectRotate.Left, slicePoint.Y - rectRotate.Top, rectRotate.Width - (slicePoint.X - rectRotate.Left), rectRotate.Height - (slicePoint.Y - rectRotate.Top)));
                }
            }

            if (rectangles.Count > 0)
            {
                var source = bitmapRotate;
                var orignalsize = new Size(source.Width, source.Height);
                var newWidgets = new List<WidgetViewModBase>();
                foreach (var rect in rectangles)
                {
                    var widget = this._pageEditorViewModel.PageEditorModel.AddWidgetItem2Dom
                        (Naver.Compass.Service.Document.WidgetType.Image,
                        Naver.Compass.Service.Document.ShapeType.None,
                        rectRotate.Left + rect.X,
                        rectRotate.Top + rect.Y,
                        Convert.ToInt32(rect.Width),
                        Convert.ToInt32(rect.Height));
                    var vmItem = new ImageWidgetViewModel(widget);
                    vmItem.IsSelected = true;
                    var cb = new CroppedBitmap(
                        source,
                        new Int32Rect(
                            Convert.ToInt32(rect.X * orignalsize.Width / rectRotate.Width),
                            Convert.ToInt32(rect.Y * orignalsize.Height / rectRotate.Height),
                            Convert.ToInt32(rect.Width * orignalsize.Width / rectRotate.Width),
                            Convert.ToInt32(rect.Height * orignalsize.Height / rectRotate.Height)));
                    var ms = new MemoryStream();
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(cb));
                    encoder.Save(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    vmItem.ImportImgSeekLater(ms, false);
                    vmItem.Opacity = img.Opacity;
                    newWidgets.Add(vmItem);
                }
                img.IsSelected = false;
                this._pageEditorViewModel.ReplaceWidget(new List<WidgetViewModBase> { img }, newWidgets);
            }
        }

        private Rect GetIntersect(ImageWidgetViewModel img)
        {
            ///calculate intersect area of image and move thumb
            var rectangleImg = new Rect(img.Left, img.Top, img.ItemWidth, img.ItemHeight);
            var rectangleThumb = new Rect(CropToolsRectLeft / Scale, CropToolsRectTop / Scale, CropToolsRectWidth / Scale, CropToolsRectHeight / Scale);
            var rectangleIntersect = Rect.Intersect(rectangleImg, rectangleThumb);
            return rectangleIntersect;
        }

        private Rect GetIntersect(Rect rectangleImg)
        {
            ///calculate intersect area of image and move thumb
            var rectangleThumb = new Rect(CropToolsRectLeft / Scale, CropToolsRectTop / Scale, CropToolsRectWidth / Scale, CropToolsRectHeight / Scale);
            var rectangleIntersect = Rect.Intersect(rectangleImg, rectangleThumb);
            return rectangleIntersect;
        }

        private void CropSelectedImage()
        {
            if (this._img == null) { return; }

            var img = this._img;
            var tb = this.ScaleImage(img);

            var bitmapRotate = ImageProcess.RotateImage(tb, img.RotateAngle);

            var rectRotate = new Rect();
            if (img.RotateAngle == 0d)
            {
                rectRotate = new Rect(img.Left, img.Top, img.ItemWidth, img.ItemHeight);
            }
            else
            {
                var newSize = ImageProcess.RotateSize(img.ItemWidth, img.ItemHeight, img.RotateAngle);
                rectRotate = new Rect(
                    (img.Left + img.ItemWidth / 2) - newSize.Width / 2,
                    (img.Top + img.ItemHeight / 2) - newSize.Height / 2,
                    newSize.Width,
                    newSize.Height);
            }

            var rectangleIntersect = GetIntersect(rectRotate);

            ///no intersect area
            if (rectangleIntersect == Rect.Empty)
            {
                return;
            }

            var source = bitmapRotate;
            var orignalsize = new Size(source.Width, source.Height);
            var widget = this._pageEditorViewModel.PageEditorModel.AddWidgetItem2Dom
                        (Naver.Compass.Service.Document.WidgetType.Image,
                        Naver.Compass.Service.Document.ShapeType.None,
                        rectangleIntersect.Left,
                        rectangleIntersect.Top,
                        Convert.ToInt32(rectangleIntersect.Width),
                        Convert.ToInt32(rectangleIntersect.Height));
            var vmItem = new ImageWidgetViewModel(widget);
            vmItem.IsSelected = true;
            var cb = new CroppedBitmap(
                        source,
                        new Int32Rect(
                            Convert.ToInt32((rectangleIntersect.Left - rectRotate.Left) * orignalsize.Width / rectRotate.Width),
                            Convert.ToInt32((rectangleIntersect.Top - rectRotate.Top) * orignalsize.Height / rectRotate.Height),
                            Convert.ToInt32(rectangleIntersect.Width * orignalsize.Width / rectRotate.Width),
                            Convert.ToInt32(rectangleIntersect.Height * orignalsize.Height / rectRotate.Height)));
            var ms = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(cb));
            encoder.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            vmItem.ImportImgSeekLater(ms, false);
            vmItem.Opacity = img.Opacity;

            ///replace selected widget with cropped widget
            img.IsSelected = false;
            this._pageEditorViewModel.ReplaceWidget(
                new List<WidgetViewModBase> { img },
                new List<WidgetViewModBase> { vmItem });
        }

        private void CropAndCutSelectedImg()
        {
            if (this._img == null) { return; }

            var img = this._img;
            var tb = this.ScaleImage(img);

            var bitmapRotate = ImageProcess.RotateImage(tb, img.RotateAngle);

            var rectRotate = new Rect();
            if (img.RotateAngle == 0d)
            {
                rectRotate = new Rect(img.Left, img.Top, img.ItemWidth, img.ItemHeight);
            }
            else
            {
                var newSize = ImageProcess.RotateSize(img.ItemWidth, img.ItemHeight, img.RotateAngle);
                rectRotate = new Rect(
                    (img.Left + img.ItemWidth / 2) - newSize.Width / 2,
                    (img.Top + img.ItemHeight / 2) - newSize.Height / 2,
                    newSize.Width,
                    newSize.Height);
            }

            var rectangleIntersect = GetIntersect(rectRotate);

            ///no intersect area
            if (rectangleIntersect == Rect.Empty)
            {
                return;
            }

            var source = bitmapRotate;
            var orignalsize = new Size(source.Width, source.Height);

            ///to cut selected area
            var vmcopyItem = MakeCopyImageWidget(source, orignalsize, rectangleIntersect, rectRotate);
            vmcopyItem.IsSelected = false;
            _pageEditorViewModel.CopyWidget(vmcopyItem, true);
            _pageEditorViewModel.DeleteItem(vmcopyItem);

            ///rubber selected area
            var writablebitmap = new WriteableBitmap(source);
            var toRubberRect = new Int32Rect(
                            Convert.ToInt32((rectangleIntersect.Left - rectRotate.Left) * orignalsize.Width / rectRotate.Width),
                            Convert.ToInt32((rectangleIntersect.Top - rectRotate.Top) * orignalsize.Height / rectRotate.Height),
                            Convert.ToInt32(rectangleIntersect.Width * orignalsize.Width / rectRotate.Width),
                            Convert.ToInt32(rectangleIntersect.Height * orignalsize.Height / rectRotate.Height));
            ImageProcess.Rubber(toRubberRect.X, toRubberRect.Y, toRubberRect.Width, toRubberRect.Height, writablebitmap);

            var ms = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(writablebitmap));
            encoder.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var widget = this._pageEditorViewModel.PageEditorModel.AddWidgetItem2Dom
                        (Naver.Compass.Service.Document.WidgetType.Image,
                        Naver.Compass.Service.Document.ShapeType.None,
                        rectRotate.Left,
                        rectRotate.Top,
                        Convert.ToInt32(rectRotate.Width),
                        Convert.ToInt32(rectRotate.Height));
            var vmItem = new ImageWidgetViewModel(widget);
            vmItem.IsSelected = true;
            vmItem.ImportImgSeekLater(ms, false);
            vmItem.Opacity = img.Opacity;

            ///replace selected widget with cropped widget
            img.IsSelected = false;
            this._pageEditorViewModel.ReplaceWidget(
                new List<WidgetViewModBase> { img },
                new List<WidgetViewModBase> { vmItem });
        }

        private void CopySelectedArea()
        {
            //clear copy-data cache
            if (this._img == null) { return; }

            var img = this._img;
            var tb = this.ScaleImage(img);

            var bitmapRotate = ImageProcess.RotateImage(tb, img.RotateAngle);

            var rectRotate = new Rect();
            var newSize = ImageProcess.RotateSize(img.ItemWidth, img.ItemHeight, img.RotateAngle);
            rectRotate = new Rect(
                (img.Left + img.ItemWidth / 2) - newSize.Width / 2,
                (img.Top + img.ItemHeight / 2) - newSize.Height / 2,
                newSize.Width,
                newSize.Height);

            var rectangleIntersect = GetIntersect(rectRotate);

            ///no intersect area
            if (rectangleIntersect == Rect.Empty)
            {
                return;
            }

            var source = bitmapRotate;
            var orignalsize = new Size(source.Width, source.Height);

            var vmItem = MakeCopyImageWidget(source, orignalsize, rectangleIntersect, rectRotate);
            vmItem.Opacity = img.Opacity;

            img.IsSelected = false;
            _pageEditorViewModel.CopyWidget(vmItem, false);
            _pageEditorViewModel.DeleteItem(vmItem);
        }

        private TransformedBitmap ScaleImage(ImageWidgetViewModel img)
        {
            var tb = new TransformedBitmap();
            var scaleTransform = new ScaleTransform();

            BitmapFrame OriginSource = img.ImgOriginSource as BitmapFrame;
            if (OriginSource == null)
            {
                tb.BeginInit();
                tb.Source = null;
                tb.Transform = scaleTransform;
                tb.EndInit();
                return tb;
            }

            var scalex = img.ItemWidth / OriginSource.Width;
            var scaley = img.ItemHeight / OriginSource.Height;
            if (scaley <= scalex)
            {
                scaleTransform.ScaleX = 1;
                scaleTransform.ScaleY = (img.ItemHeight / OriginSource.Height) / scalex;
            }
            else
            {
                scaleTransform.ScaleY = 1;
                scaleTransform.ScaleX = (img.ItemWidth / OriginSource.Width) / scaley;
            }

            var checkWholeSize = (scaleTransform.ScaleX * OriginSource.Width) * (scaleTransform.ScaleY * OriginSource.Height);
            if (checkWholeSize > 3000 * 3000)
            {
                scaleTransform.ScaleX = Math.Sqrt(3000 * 3000 / checkWholeSize) * scaleTransform.ScaleX;
                scaleTransform.ScaleY = Math.Sqrt(3000 * 3000 / checkWholeSize) * scaleTransform.ScaleY;
            }

            tb.BeginInit();
            tb.Source = OriginSource as BitmapFrame;
            tb.Transform = scaleTransform;
            tb.EndInit();
            return tb;
        }

        private ImageWidgetViewModel MakeCopyImageWidget(BitmapSource source, Size orignalsize, Rect rectangleIntersect, Rect rectRotate)
        {
            var widget = this._pageEditorViewModel.PageEditorModel.AddWidgetItem2Dom
                        (Naver.Compass.Service.Document.WidgetType.Image,
                        Naver.Compass.Service.Document.ShapeType.None,
                        rectangleIntersect.Left,
                        rectangleIntersect.Top,
                        Convert.ToInt32(rectangleIntersect.Width),
                        Convert.ToInt32(rectangleIntersect.Height));
            var vmItem = new ImageWidgetViewModel(widget);
            vmItem.IsSelected = false;
            var cb = new CroppedBitmap(
                        source,
                        new Int32Rect(
                            Convert.ToInt32((rectangleIntersect.Left - rectRotate.Left) * orignalsize.Width / rectRotate.Width),
                            Convert.ToInt32((rectangleIntersect.Top - rectRotate.Top) * orignalsize.Height / rectRotate.Height),
                            Convert.ToInt32(rectangleIntersect.Width * orignalsize.Width / rectRotate.Width),
                            Convert.ToInt32(rectangleIntersect.Height * orignalsize.Height / rectRotate.Height)));
            var ms = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(cb));
            encoder.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            vmItem.ImportImgSeekLater(ms, false);

            return vmItem;
        }

        private Grid GetImageEditorGrid(UIElement ue)
        {
            if ((ue is Grid) && (ue as Grid).Name == "imageeditor_grid")
            {
                return ue as Grid;
            }

            var parent = VisualTreeHelper.GetParent(ue) as UIElement;
            if (parent == null)
            {
                return null;
            }

            return GetImageEditorGrid(parent);
        }
        internal void SetEditType(ImageEditType type, ImageWidgetViewModel img = null)
        {
            if (this.EditType == type)
                return;

            this.EditType = type;
            this._img = img;
            switch (type)
            {
                case ImageEditType.Slice:
                    SliceLineVisible = true;
                    CropToolsVisible = false;
                    SliceType = Module.SliceType.Cross;
                    this._pageEditorViewModel.SetImageEditorFocus();
                    break;
                case ImageEditType.Crop:
                    SliceLineVisible = false;
                    CropToolsVisible = true;
                    this.InitialCropTools();
                    this._pageEditorViewModel.SetImageEditorFocus();
                    break;
                default:
                    SliceLineVisible = false;
                    CropToolsVisible = false;
                    break;
            }
        }

        /// <summary>
        /// Initialize size and position of crop thumb
        /// </summary>
        private void InitialCropTools()
        {
            if (this._img == null) { return; }

            var img = this._img;
            CropToolsRectWidth = (img.ItemWidth * Scale) / 2;
            CropToolsRectHeight = (img.ItemHeight * Scale) / 2;
            CropToolsRectLeft = img.Left * Scale + (img.ItemWidth * Scale) / 4;
            CropToolsRectTop = img.Top * Scale + (img.ItemHeight * Scale) / 4;
        }

        private bool _sliceLineVisible = false;

        /// <summary>
        /// Gets or sets the Property property. This observable property 
        /// indicates ....
        /// </summary>
        public bool SliceLineVisible
        {
            get { return _sliceLineVisible; }
            set
            {
                if (_sliceLineVisible != value)
                {
                    _sliceLineVisible = value;
                    FirePropertyChanged("SliceLineVisible");
                }
            }
        }

        private bool _cropToolsVisible = false;

        /// <summary>
        /// Gets or sets the Property property. This observable property 
        /// indicates ....
        /// </summary>
        public bool CropToolsVisible
        {
            get { return _cropToolsVisible; }
            set
            {
                if (_cropToolsVisible != value)
                {
                    _cropToolsVisible = value;
                    FirePropertyChanged("CropToolsVisible");
                }
            }
        }

        private double _croptoolsRectWidth;
        public double CropToolsRectWidth
        {
            get { return _croptoolsRectWidth; }
            set
            {
                _croptoolsRectWidth = value;
                FirePropertyChanged("CropToolsRectWidth");
            }
        }

        private double _croptoolsRectHeight;
        public double CropToolsRectHeight
        {
            get { return _croptoolsRectHeight; }
            set
            {
                _croptoolsRectHeight = value;
                FirePropertyChanged("CropToolsRectHeight");
            }
        }

        private double _croptoolsRectLeft;
        public double CropToolsRectLeft
        {
            get { return _croptoolsRectLeft; }
            set
            {
                _croptoolsRectLeft = value;
                FirePropertyChanged("CropToolsRectLeft");
            }
        }

        private double _croptoolsRectTop;
        public double CropToolsRectTop
        {
            get { return _croptoolsRectTop; }
            set
            {
                _croptoolsRectTop = value;
                FirePropertyChanged("CropToolsRectTop");
            }
        }

        private Thickness _layoutBtnGridMargin;

        /// <summary>
        /// Gets or sets the Property property. This observable property 
        /// indicates ....
        /// </summary>
        public Thickness LayoutBtnGridMargin
        {
            get { return _layoutBtnGridMargin; }
            set
            {
                if (_layoutBtnGridMargin != value)
                {
                    _layoutBtnGridMargin = value;
                    FirePropertyChanged("LayoutBtnGridMargin");
                }
            }
        }

        private SliceType _sliceType = SliceType.Cross;

        /// <summary>
        /// Gets or sets the Property property. This observable property 
        /// indicates ....
        /// </summary>
        public SliceType SliceType
        {
            get { return _sliceType; }
            set
            {
                if (_sliceType != value)
                {
                    _sliceType = value;
                    FirePropertyChanged("SliceType");
                }
            }
        }

        private double _scale = 1;
        public double Scale
        {
            get { return _scale; }
            set
            {
                ChangeScale(value);
                _scale = value;
            }
        }

        private void ChangeScale(double scale)
        {
            CropToolsRectLeft = (CropToolsRectLeft / _scale) * scale;
            CropToolsRectTop = (CropToolsRectTop / _scale) * scale;
            CropToolsRectWidth = (CropToolsRectWidth / _scale) * scale;
            CropToolsRectHeight = (CropToolsRectHeight / _scale) * scale;
        }
    }
}
