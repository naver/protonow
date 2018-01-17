using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.InfoStructure
{
    public interface ISupportUndo
    {
        UndoManager UndoManager { get; set; }
    }
}
