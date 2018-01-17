using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Naver.Compass.InfoStructure
{
    public interface IUndoableCommand
    {
        void Undo();
        void Redo();
    }
}
