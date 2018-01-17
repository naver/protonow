using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.Module
{
    class DeleteWidgetInGroupCommand : IUndoableCommand
    {
        public void AddCommand(DeleteWidgetInGroupSubCommand cmd)
        {
            if (cmd == null)
            {
                return;
            }
            _cmdList.Add(cmd);
        }

        public int Count
        {
            get { return _cmdList.Count; }
        }


        public void Undo()
        {
            // Undo in the reverse order so that groups can be restore correctly
            for (int i = _cmdList.Count - 1; i >= 0; i--)
            {
                DeleteWidgetInGroupSubCommand cmd = _cmdList[i];
                cmd.Undo();
            }
        }

        public void Redo()
        {
            foreach (DeleteWidgetInGroupSubCommand cmd in _cmdList)
            {
                cmd.Redo();
            }
        }

        private List<DeleteWidgetInGroupSubCommand> _cmdList = new List<DeleteWidgetInGroupSubCommand>();
    }
}
