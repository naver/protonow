using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module
{
    class ImportPageIconCommand : IUndoableCommand
    {
        public ImportPageIconCommand(PageIconViewModel pageIconVM, Stream oldStream, Stream newStream)
        {
            _pageIconVM = pageIconVM;
            _newStream = newStream;

            _oldStream = oldStream;
        }

        public void Undo()
        {
            if (_oldStream != null)
            {
                // Set Position to 0 to make a new ImgSource
                _oldStream.Position = 0;
                _pageIconVM.ImportIcon(_oldStream);
            }
            else
            {
                _pageIconVM.ClearIcon();
            }
        }

        public void Redo()
        {
            if (_newStream != null)
            {
                // Set Position to 0 to make a new ImgSource
                _newStream.Position = 0;
                _pageIconVM.ImportIcon(_newStream);
            }
            else
            {
                _pageIconVM.ClearIcon();
            }
        }

        private PageIconViewModel _pageIconVM;
        private Stream _oldStream;
        private Stream _newStream;

    }
}
