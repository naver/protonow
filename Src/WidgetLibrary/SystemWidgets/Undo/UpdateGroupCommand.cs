using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.WidgetLibrary
{
    public class UpdateGroupCommand : IUndoableCommand
    {
        public UpdateGroupCommand(IGroupOperation pageVM, List<Guid> groupGuids)
        {
            _pageVM = pageVM;
            _groupGuids = groupGuids;
        }

        public void Undo()
        {
            UpdateGroup();
        }

        public void Redo()
        {
            UpdateGroup();
        }

        private void UpdateGroup()
        {
            foreach (Guid guid in _groupGuids)
            {
                _pageVM.UpdateGroup(guid);
            }
        }

        IGroupOperation _pageVM;
        List<Guid> _groupGuids;
    }
}
