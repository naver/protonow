using Naver.Compass.Common.Helper;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Win32;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.Module
{
    partial class PageEditorViewModel
    {
        #region ImageEditor
        private bool _isImageEditorVisible;
        public bool IsImageEditorVisible
        {
            get { return this._isImageEditorVisible; }
            set
            {
                if (_isImageEditorVisible != value)
                {
                    _isImageEditorVisible = value;
                    FirePropertyChanged("IsImageEditorVisible");
                }
            }
        }

        public object NotifyFocus { get; set; }
        public void SetImageEditorFocus()
        {
            NotifyFocus = new object();
            FirePropertyChanged("NotifyFocus");
        }

        internal ImageEditorViewModel ImageEditorViewmodel { get; set; }
        internal MenuPageEditorViewModel MenuPageEditorViewModel { get; set; }
        internal void CopyWidget(WidgetViewModBase wdgItem, bool bIsCut)
        {
            //implement copy operation
            ISerializeWriter serializeWriter = _document.CreateSerializeWriter(_curAdaptiveViewGID);
            wdgItem.WidgetModel.SerializeObject(serializeWriter);

            Stream stream = serializeWriter.WriteToStream();
            if (stream == null)
            {
                return;
            }
            
            //Clipboard operation
            try
            {                
                var data = new DataObject();
                _copyTime = 0;
                _copyGID = Guid.NewGuid();            
                data.SetData(@"ProtoNowCopyID", _copyGID);
                data.SetData(@"ProtoNowAdaptiveID", _curAdaptiveViewGID);
                data.SetData(@"ProtoNowWidgets", stream);
            
                //Copy to Clipboard
                Clipboard.Clear();
                Clipboard.SetDataObject(data);
                //Clipboard.SetDataObject(data, true);

                //Reset Copy ID
                if (bIsCut == true)
                {
                    _copyGID = Guid.Empty;
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }           
        }
        #endregion ImageEditor

        #region Image Export
        public bool ExportPage2Image()
        {
            Rect rec = GetPageActualSize();
            if(rec.Height<=0 || rec.Width<=0)
            {
                return false;
            }

            try
            {
                //render the canvas
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)rec.Width, (int)rec.Height, 96, 96, PixelFormats.Pbgra32);
                DesignerBaseCanvas canvas = EditorCanvas as DesignerBaseCanvas;
                if(canvas==null)
                {
                    return false;
                }
                canvas.UpdateLayout();
                bmp.Render(canvas);

                //show file save dialog
                string imagFile = "";
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                //saveFileDialog.Filter = "PNG File(*.png)|*.png|JPG File(*.jpg)|*.jpg|BMP File(*.bmp)|*.bmp|GIF File(*.gif)|*.gif|TIF File(*.tif)|*.tif";
                saveFileDialog.Filter = "PNG File(*.png)|*.png";
                saveFileDialog.FileName = this.Title;
                if (saveFileDialog.ShowDialog() == true)
                {
                    imagFile = saveFileDialog.FileName;
                }
                else
                {
                    return false;
                }

                //export image
                string extensionString = Path.GetExtension(imagFile);
                BitmapEncoder encoder = null;
                switch (extensionString)
                {
                    case ".jpg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;
                    case ".bmp":
                        encoder = new BmpBitmapEncoder();
                        break;
                    case ".gif":
                        encoder = new GifBitmapEncoder();
                        break;
                    case ".tif":
                        encoder = new TiffBitmapEncoder();
                        break;
                    default:
                        throw new InvalidOperationException();

                }
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                using (Stream stm = File.Create(imagFile))
                {
                    encoder.Save(stm);
                }
            }
            catch (Exception exp)
            {
                NLogger.Warn("Export page into a image : " + exp.Message);
                return false;
            }
            return true;
        }
        public bool ExportObj2Image()
        {  
            if (1 != _selectionService.WidgetNumber)
            {
                return false;
            }

            WidgetViewModBase wdg = _selectionService.GetSelectedWidgets()[0] as WidgetViewModBase;
            if (wdg == null)
            {
                return false;
            }

            try
            {
                DesignerCanvas layout = EditorCanvas as DesignerCanvas;
                if (layout == null)
                {
                    return false;
                }
                BaseWidgetItem it = layout.SelectedItems.ToList<BaseWidgetItem>()[0];
                if (it == null)
                {
                    return false;
                }


                RenderTargetBitmap bmp = new RenderTargetBitmap((int)it.ActualWidth, (int)it.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(VisualTreeHelper.GetChild(it, 0) as Visual);

                //show file save dialog
                string imagFile = "";
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                //saveFileDialog.Filter = "PNG File(*.png)|*.png|JPG File(*.jpg)|*.jpg|BMP File(*.bmp)|*.bmp|GIF File(*.gif)|*.gif|TIF File(*.tif)|*.tif";
                saveFileDialog.Filter = "PNG File(*.png)|*.png";
                saveFileDialog.FileName = wdg.Type.ToString() + "-" + wdg.WidgetID.ToString().Substring(0, 8);
                if (saveFileDialog.ShowDialog() == true)
                {
                    imagFile = saveFileDialog.FileName;
                }
                else
                {
                    return false;
                }

                //export image
                string extensionString = Path.GetExtension(imagFile);
                BitmapEncoder encoder = null;
                switch (extensionString)
                {
                    case ".jpg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;
                    case ".bmp":
                        encoder = new BmpBitmapEncoder();
                        break;
                    case ".gif":
                        encoder = new GifBitmapEncoder();
                        break;
                    case ".tif":
                        encoder = new TiffBitmapEncoder();
                        break;
                    default:
                        throw new InvalidOperationException();

                }
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                using (Stream stm = File.Create(imagFile))
                {
                    encoder.Save(stm);
                }
            }
            catch(Exception exp)
            {
                NLogger.Warn("Export widget into a image exception : " + exp.Message);
                return false;
            }         
            return true;

        }
        private  Rect GetPageActualSize()
        {
            double w = 0;
            double h = 0;
            foreach (WidgetViewModBase it in items)
            {
                if(it.IsGroup==true)
                {
                    continue;
                }

                Rect rect = it.GetBoundingRectangle();
                if(rect.X+rect.Width>w)
                {
                    w = rect.X + rect.Width;
                }
                if (rect.Y + rect.Height > h)
                {
                    h = rect.Y + rect.Height;
                }
            }

            return new Rect(new System.Windows.Point(0, 0), new System.Windows.Point(w, h));
        }
        #endregion

    }
}
