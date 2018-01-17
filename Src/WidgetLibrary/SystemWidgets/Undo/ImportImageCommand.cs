using System.IO;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.WidgetLibrary
{
    class ImportImageCommand : IUndoableCommand
    {
        public ImportImageCommand(ImageWidgetViewModel imageVM, Stream oldStream, Stream newStream, bool isAutosizeOldValue, bool isAutosizeNewValue)
        {
            _imageVM = imageVM;
            _newStream = newStream;
            //if (oldStream != null)
            //{
            //    _oldStream = new MemoryStream((int)oldStream.Length);
            //    oldStream.Seek(0, SeekOrigin.Begin);
            //    oldStream.CopyTo(_oldStream);
            //    _oldStream.Seek(0, SeekOrigin.Begin);
            //}
            //else
            //{
            //    _oldStream = null;
            //}
            _oldStream = oldStream;

            _isAutosizeOldValue = isAutosizeOldValue;
            _isAutosizeNewValue = isAutosizeNewValue;
        }

        public void Undo()
        {
            if(_oldStream != null)
            {
                // Set Position to 0 to make a new ImgSource
                _oldStream.Position = 0;
                _imageVM.ImportImg(_oldStream, _isAutosizeOldValue);
            }
            else
            {
                _imageVM.ClearImg(false);
            }
        }

        public void Redo()
        {
            if (_newStream != null)
            {
                // Set Position to 0 to make a new ImgSource
                _newStream.Position = 0;
                _imageVM.ImportImg(_newStream, _isAutosizeNewValue);
            }
            else
            {
                _imageVM.ClearImg(false);
            }
        }


        private ImageWidgetViewModel _imageVM;
        private Stream _oldStream;
        private Stream _newStream;
        private bool _isAutosizeOldValue;
        private bool _isAutosizeNewValue;

    }
}
