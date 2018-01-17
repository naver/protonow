using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Service.Document;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.Module
{
    class CreateGroupCommand : IUndoableCommand
    {
        public CreateGroupCommand(PageEditorViewModel pageVM, GroupViewModel groupVM)
        {
            _pageVM = pageVM;
            _group = groupVM.ExternalGroup;
        }

        public void Undo()
        {
            // Ad group vm is created everytime executing group, so find the current group VM according to group guid.
            GroupViewModel groupVM = _pageVM.Items.FirstOrDefault(x => x.WidgetID == _group.Guid) as GroupViewModel;
            if (groupVM != null)
            {
                _pageVM.UnGroup(groupVM);
            }
        }

        public void Redo()
        {
            // Add the group to document
            _pageVM.PageEditorModel.AddClonedGroup2Dom(_group);

            // GroupViewModel is newly create when create a group, old VM is invalid.
            _pageVM.CreateGroupRender(_group);
        }

        private PageEditorViewModel _pageVM;
        private IGroup _group;
    }
}
