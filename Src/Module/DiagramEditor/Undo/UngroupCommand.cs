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
    public class UngroupCommand : IUndoableCommand
    {
        public UngroupCommand(PageEditorViewModel pageVM, GroupViewModel groupVM)
        {
            _pageVM = pageVM;
            _group = groupVM.ExternalGroup;
        }

        public void Undo()
        {
            // Add the group to document
            _pageVM.PageEditorModel.AddClonedGroup2Dom(_group);

            // GroupViewModel is newly create when create a group, old VM is invalid.
            _pageVM.CreateGroupRender(_group);
        }

        public void Redo()
        {
            GroupViewModel groupVM = _pageVM.Items.FirstOrDefault(x => x.WidgetID == _group.Guid) as GroupViewModel;
            if (groupVM != null)
            {
                _pageVM.UnGroup(groupVM);
            }
        }

        private PageEditorViewModel _pageVM;
        private IGroup _group;
    }
}
