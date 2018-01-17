using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.Helper;
using System.Diagnostics;

using UndoCompositeCommand = Naver.Compass.InfoStructure.CompositeCommand;
using System.Collections.Generic;
using System.Windows.Threading;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.WidgetLibrary
{
    public class ImageWidgetViewModel : WidgetRotateViewModBase
    {
        public ImageWidgetViewModel()
        {

        }
        public ImageWidgetViewModel(IWidget widget)
        {
            _model = new ImageModel(widget);
            _bSupportBorder = false;
            _bSupportBackground = false;
            _bSupportText = false;
            _bSupportTextVerAlign = false;
            _bSupportTextHorAlign = false;
            widgetGID = widget.Guid;
            Type = ObjectType.Image;
            //szTargetFile = @"D:\VideoCapture.png";

            _bSupportGradientBackground = false;
            _bSupportGradientBorderline = false;
            _bSupportRotate = true;
            _bSupportTextRotate = false;

            _imageStream = (_model as ImageModel).ImageStream;
            AsyncCreateNailStream();
        }
        
        #region  private member
        private bool _isAutosize = false;
        protected Stream _imageStream = null;
        private Visibility _backgroundShow = Visibility.Visible;

        #endregion

        #region Widget Routed Event Handler by Command
        private DelegateCommand<object> _doubleClickCommand = null;
        override public ICommand DoubleClickCommand
        {
            get
            {
                if (_doubleClickCommand == null)
                {
                    _doubleClickCommand = new DelegateCommand<object>(OnDoubleClick);
                }
                return _doubleClickCommand;
            }
        }
        private void OnDoubleClick(object obj)
        {
            ImportImg();
            if (ParentID != Guid.Empty)
            {
                //UpdateGroup(img.ParentID);
            }
        }
        #endregion

        #region Private Function
        private void CreateImageFromPath(string path)
        {
            Stream oldStream ;
            if (_imageStream != null)
            {
                oldStream = new MemoryStream((int)_imageStream.Length);
                _imageStream.Seek(0, SeekOrigin.Begin);
                _imageStream.CopyTo(oldStream);
                oldStream.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                oldStream = null;
            }

            bool isAutosizeOldValue = _isAutosize;
            PropertyMementos mementos = CreateNewPropertyMementos();
            mementos.AddPropertyMemento(new PropertyMemento("ItemHeight", ItemHeight, ItemHeight));
            mementos.AddPropertyMemento(new PropertyMemento("ItemWidth", ItemWidth, ItemWidth));

            _isAutosize = true;

            try
            {
                //reader = File.OpenRead(openFile.FileName);
                _imageStream = new MemoryStream(File.ReadAllBytes(path));
                (_model as ImageModel).ImageStream = _imageStream;

                // Get ImgSource so that ItemHeight and ItemWidth have the actual size value of the new image.
                // Here, FirePropertyChanged("ImgSource"); will not go into ImgSource get method when dragging a image file 
                // on canvas, this cause hight and width are not updated to actual image size.
                // FirePropertyChanged("ImgSource") works well when importing image via file dialog, I don't know why 
                // it doesn't work when dragging.
                // This is a workaround, remove this when you figure out the root cause.               
                // ImageSource temp = ImgSource;

                FirePropertyChanged("ImgSource");

                if (CurrentUndoManager != null)
                {
                    mementos.SetPropertyNewValue("ItemHeight", ItemHeight);
                    mementos.SetPropertyNewValue("ItemWidth", ItemWidth);

                    PropertyChangeCommand propCmd = new PropertyChangeCommand(this, mementos);
                    ImportImageCommand imgCmd = new ImportImageCommand(this, oldStream, _imageStream, isAutosizeOldValue, _isAutosize);

                    UndoCompositeCommand cmds = new UndoCompositeCommand();
                    cmds.AddCommand(imgCmd);
                    cmds.AddCommand(propCmd);

                    cmds.DeselectAllWidgetsFirst();
                    CurrentUndoManager.Push(cmds);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CreateNailStream()
        {
            if (_imageStream == null)
            {
                _nailStream = null;
                return;
            }

            //Create the nail image stream for property panel            
            try
            {
                _imageStream.Seek(0, SeekOrigin.Begin);
                using (System.Drawing.Image drawingImage = System.Drawing.Image.FromStream(_imageStream))
                {
                    if (drawingImage.Width > 127 || drawingImage.Height > 127)
                    {
                        using (System.Drawing.Image thumbImage =
                            drawingImage.GetThumbnailImage(127, 74, () => { return true; }, IntPtr.Zero))
                        {

                            _nailStream = new MemoryStream();
                            thumbImage.Save(_nailStream, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                    else
                    {
                        _nailStream = _imageStream;
                    }
                }

            }
            catch
            {
                _nailStream = null;
            }
            
        }
        private void AsyncCreateNailStream()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                CreateNailStream();
                if(IsSelected==true)
                {
                    FirePropertyChanged("NailStream");
                }
            }), DispatcherPriority.Background, null);
        }
        private void CreateNailStreamAndShow()
        {
            CreateNailStream();
            FirePropertyChanged("NailStream");
        }
        #endregion

        #region Public Function
        //Called by Open File
        public void ImportImg()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = CommonDefine.ImageFilter;
            if (openFile.ShowDialog() == false)
            {
                return;
            }


            try
            {
                CreateImageFromPath(openFile.FileName);
                CreateNailStreamAndShow();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        //Called by paste image from clipboard
        public void ImportImg(BitmapSource source, bool isAutosize = true)
        {
            if (source == null)
            {
                return;
            }

            _isAutosize = isAutosize;

            try
            {
                MemoryStream outStream = new MemoryStream();
                
                BmpBitmapEncoder enc = new BmpBitmapEncoder();
                //var enc = new PngBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(source));
                enc.Save(outStream);
                outStream.Position = outStream.Seek(0, SeekOrigin.Begin);
               
                _imageStream = outStream;
                (_model as ImageModel).ImageStream = _imageStream;
                FirePropertyChanged("ImgSource");
                CreateNailStreamAndShow();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Called by Redo/Undo, isAutosize=false
        public void ImportImg(Stream ImgStream, bool isAutosize = true)
        {
            if (ImgStream == null)
            {
                return;
            }
            _isAutosize = false;

            try
            {
                //reader = File.OpenRead(openFile.FileName);
                _imageStream = ImgStream;
                (_model as ImageModel).ImageStream = _imageStream;
                FirePropertyChanged("ImgSource");
                CreateNailStreamAndShow();
                //_imageStream.Position = _imageStream.Seek(0, SeekOrigin.End);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, GlobalData.FindResource("Common_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        //Called by Image Editor Module
        public void ImportImgSeekLater(Stream ImgStream, bool isAutosize = true)
        {
            if (ImgStream == null)
            {
                return;
            }
            _isAutosize = isAutosize;

            try
            {
                //reader = File.OpenRead(openFile.FileName);
                _imageStream = ImgStream;
                (_model as ImageModel).ImageStream = _imageStream;
                FirePropertyChanged("ImgSource");
                CreateNailStreamAndShow();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        //Called by Drag File
        public void ImportImg(string path, bool isAutosize = true)
        {
            if (String.IsNullOrEmpty(path))
            {
                return;
            }

            CreateImageFromPath(path);
            CreateNailStreamAndShow();
        }

        //
        public void ClearImg(bool pushToUndoStack = true)
        {
            PropertyMementos mementos = CreateNewPropertyMementos();
            mementos.AddPropertyMemento(new PropertyMemento("ItemHeight", ItemHeight, ItemHeight));
            mementos.AddPropertyMemento(new PropertyMemento("ItemWidth", ItemWidth, ItemWidth));
            mementos.AddPropertyMemento(new PropertyMemento("Opacity", Opacity, Opacity));
            ClearImageCommand imgCmd = new ClearImageCommand(this, _imageStream, _isAutosize);

            //reader = File.OpenRead(openFile.FileName);

            _nailStream = null;
            _imageStream = null;
            (_model as ImageModel).ImageStream = _imageStream;
            BackgroundShow = Visibility.Visible;
            Opacity = 1;
            FirePropertyChanged("ImgSource");
            CreateNailStreamAndShow();
            if (pushToUndoStack && CurrentUndoManager != null)
            {
                mementos.SetPropertyNewValue("ItemHeight", ItemHeight);
                mementos.SetPropertyNewValue("ItemWidth", ItemWidth);
                mementos.SetPropertyNewValue("Opacity", Opacity);
                PropertyChangeCommand propCmd = new PropertyChangeCommand(this, mementos);

                UndoCompositeCommand cmds = new UndoCompositeCommand();
                cmds.AddCommand(imgCmd);
                cmds.AddCommand(propCmd);

                cmds.DeselectAllWidgetsFirst();
                CurrentUndoManager.Push(cmds);
            }
        }
        public Stream ImgStream
        {
            get { return _imageStream; }
        }
        #endregion

        #region Binding Propery
        //public string ImgSource
        //{
        //    get
        //    {
        //        return (@"../Media/paint.png");
        //    }
        //}
        //BitmapImage _imgSource;
        private Stream _nailStream = null;
        public Stream NailStream
        {
            get 
            {
                return _nailStream;
            }
        }

        bool isDowload=false;
        public void UnloadImage(bool isLoaded)
        {
            if (isDowload == isLoaded)
            { return; }
            isDowload = isLoaded;
            FirePropertyChanged("ImgSource");
        }

        public ImageSource ImgSource
        {
            get
            {
                if(isDowload==true)
                {
                    return null;
                }
                if (_imageStream == null)
                {
                    //return new BitmapImage();
                    BackgroundShow = Visibility.Visible;
                    return null;
                }

                try
                {                    
                    //Create ImageSource
                    _imageStream.Seek(0, SeekOrigin.Begin);
                    BackgroundShow = Visibility.Collapsed;
                    ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                    BitmapFrame imgSoruce = imageSourceConverter.ConvertFrom(_imageStream) as BitmapFrame;
                    //BitmapSource it = imgSoruce.Thumbnail;
                    ImageModel imgModel = _model as ImageModel;
                    if (imgSoruce != null && imgSoruce.Decoder!=null)
                    {
                        if (imgSoruce.Decoder is BmpBitmapDecoder)
                            imgModel.ImageType = ImageType.BMP;
                        else if (imgSoruce.Decoder is GifBitmapDecoder)
                            imgModel.ImageType = ImageType.GIF;
                        else if (imgSoruce.Decoder is IconBitmapDecoder)
                            imgModel.ImageType = ImageType.ICO;
                        else if (imgSoruce.Decoder is JpegBitmapDecoder)
                            imgModel.ImageType = ImageType.JPG;
                        else if (imgSoruce.Decoder is PngBitmapDecoder)
                            imgModel.ImageType = ImageType.PNG;
                        else 
                            imgModel.ImageType = ImageType.PNG;
                    }

                    //Adjust the image size
                    if (_isAutosize == true)
                    {
                        ItemHeight = imgSoruce.PixelHeight;
                        ItemWidth = imgSoruce.PixelWidth;
                        _isAutosize = false;
                    } 
                    return imgSoruce;
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
            set
            {
                FirePropertyChanged("ImgSource");
            }
        }
        public ImageSource ImgOriginSource
        {
            get
            {
                if (_imageStream == null)
                {
                    return null;
                }
                try
                {
                    _imageStream.Seek(0, SeekOrigin.Begin);
                    ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                    BitmapFrame imgSoruce = imageSourceConverter.ConvertFrom(_imageStream) as BitmapFrame;
                    return imgSoruce;
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
        }
        public Visibility BackgroundShow
        {
            get { return _backgroundShow; }
            set
            {
                if (_backgroundShow != value)
                {
                    _backgroundShow = value;
                    FirePropertyChanged("BackgroundShow");
                }
            }
        }
        #endregion

        #region Override UpdateWidgetStyle2UI Functions
        override protected void UpdateWidgetStyle2UI()
        {
            base.UpdateWidgetStyle2UI();
            //UpdateTextStyle();
            //UpdateFontStyle();
            //UpdateBackgroundStyle();
        }
        #endregion 

    }
}
