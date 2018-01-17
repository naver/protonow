using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.WidgetLibrary
{
    public class EditListCommand : IUndoableCommand
    {
        public EditListCommand(ListBaseWidgetViewModel listVM, List<IListItem> oldItems, List<IListItem> newItems)
        {
            _listVM = listVM;
            _oldItems = oldItems;
            _newItems = newItems;
        }
        public void Undo()
        {
            _listVM.IWidget.Items = _oldItems;
            _listVM.LoadList();
        }

        public void Redo()
        {
            _listVM.IWidget.Items = _newItems;
            _listVM.LoadList();
        }

        private ListBaseWidgetViewModel _listVM;
        private List<IListItem> _oldItems;
        private List<IListItem> _newItems;
    }
}
