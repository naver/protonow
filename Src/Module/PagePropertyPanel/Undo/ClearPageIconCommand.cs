using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module
{
       class ClearPageIconCommand: IUndoableCommand
    {
           public ClearPageIconCommand(PageIconViewModel pageIconVM, Stream oldStream)
        {
            _pageIconVM = pageIconVM;
            if (oldStream != null)
            {               
                _oldStream = new MemoryStream((int)oldStream.Length);
                oldStream.Seek(0, SeekOrigin.Begin);   
                oldStream.CopyTo(_oldStream);
                _oldStream.Seek(0, SeekOrigin.Begin);      
            }
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
            _pageIconVM.ImportIcon(_oldStream);
        }

        public void Redo()
        {
            _pageIconVM.ClearIcon();
        }

        private PageIconViewModel _pageIconVM;
        private Stream _oldStream;
    }
}
