using System.IO;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.WidgetLibrary
{
    class ClearImageCommand: IUndoableCommand
    {
        public ClearImageCommand(ImageWidgetViewModel imageVM, Stream oldStream, bool isAutosizeOldValue)
        {
            _imageVM = imageVM;
            if (oldStream != null)
            {               
                _oldStream = new MemoryStream((int)oldStream.Length);
                //(oldStream as MemoryStream).CopyTo(_oldStream,(int)oldStream.Length);
                oldStream.Seek(0, SeekOrigin.Begin);   
                oldStream.CopyTo(_oldStream);
                _oldStream.Seek(0, SeekOrigin.Begin);      
            }
            _isAutosizeOldValue = isAutosizeOldValue;
        }

        public void Undo()
        {
            // For now, we still can click Clear button if the image doesn't have stream. In other words, stream is null.
            if (_oldStream == null)
            {
                return;
            }

            // Set Position to 0 to make a new ImgSource
            _oldStream.Position = 0;
            _imageVM.ImportImg(_oldStream, _isAutosizeOldValue);
        }

        public void Redo()
        {
            _imageVM.ClearImg(false);
        }


        private ImageWidgetViewModel _imageVM;
        private MemoryStream _oldStream;
        private bool _isAutosizeOldValue;
    }
}
