using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Naver.Compass.Service;
using Microsoft.Practices.ServiceLocation;

namespace Naver.Compass.InfoStructure
{
    public class UndoManager
    {
        public UndoManager(int undoLimit = 100)
        {
            _undoLimit = undoLimit;

            _undoStack = new List<IUndoableCommand>(_undoLimit);
            _redoStack = new List<IUndoableCommand>(_undoLimit);
        }

        #region Public 

        public int UndoLimit
        {
            get { return _undoLimit; }
            set { _undoLimit = value; }
        }

        public bool CanUndo
        {
            get
            {
                if (_undoStack.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        public bool CanRedo
        {
            get
            {
                if (_redoStack.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Undo()
        {
            if(!CanUndo)
            {
                return;
            }

            IUndoableCommand command = _undoStack[_undoStack.Count - 1];
            _undoStack.RemoveAt(_undoStack.Count - 1);

            if(command != null)
            {
                command.Undo();

                // Add to redo stack
                _redoStack.Add(command);
            }

        }

        public void Redo()
        {
            if(!CanRedo)
            {
                return;
            }
         

            IUndoableCommand command = _redoStack[_redoStack.Count - 1];
            _redoStack.RemoveAt(_redoStack.Count - 1);

            if (command != null)
            {
                command.Redo();

                // Add to undo stack
                _undoStack.Add(command);
            }
        }

        public void Push(IUndoableCommand command)
        {
            BuildCompositeCommandAndPush(command);
        }

        public void BuildCompositeCommandAndPush(IUndoableCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException();
            }

            CompositeCommand commandToPush = command as CompositeCommand;
            if (commandToPush == null)
            {
                // If the push command is not CompositeCommand, then build a  CompositeCommand
                // to hold it as we should change to the page in which target widgets located first when undo/redo
                CompositeCommand cmds = new CompositeCommand();
                cmds.AddCommand(command);
                commandToPush = cmds;
            }


            ISelectionService selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            if (selectionService != null && null != selectionService.GetCurrentPage())
            {
                // Add change page command in the first item.
                commandToPush.InsertCommand(0, new ChangePageCommand());
            }


            if (_undoStack.Count >= _undoLimit)
            {
                _undoStack.RemoveAt(0);
            }

            _undoStack.Add(commandToPush);

            Debug.WriteLine("Pushed a IUndoableCommand into undo stack. Undo stack count is : {0}.", _undoStack.Count);

            // New command is pushed in, clear the redo stack.
            _redoStack.Clear();
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        #endregion

        #region Private

        private int _undoLimit;
        private List<IUndoableCommand> _undoStack;
        private List<IUndoableCommand> _redoStack;

        #endregion
    }
}
