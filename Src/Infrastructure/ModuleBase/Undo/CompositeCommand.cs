using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.InfoStructure
{
    public class CompositeCommand : IUndoableCommand
    {
        public void AddCommand(IUndoableCommand cmd)
        {
            if(cmd == null)
            {
                return;
            }
            _cmdList.Add(cmd);
        }

        public void InsertCommand(int index, IUndoableCommand cmd)
        {
            if (cmd == null)
            {
                return;
            }
            _cmdList.Insert(index, cmd);
        }

        public void DeselectAllWidgetsFirst()
        {
            _cmdList.Insert(0, new DeselectAllWidgetsCommand());
        }

        public void MoveFocusToEditCanvasAtEnd()
        {
            _cmdList.Add(new MoveFocusToEditCanvasCommand());
        }

        public int Count
        {
            get { return _cmdList.Count; }
        }

        public void Clear()
        {
            _cmdList.Clear();
        }

        public void Undo()
        {
            foreach(IUndoableCommand cmd in _cmdList)
            {
                cmd.Undo();
            }
        }

        public void Redo()
        {
            foreach (IUndoableCommand cmd in _cmdList)
            {
                cmd.Redo();
            }
        }

        private List<IUndoableCommand> _cmdList = new List<IUndoableCommand>();
    }
}
