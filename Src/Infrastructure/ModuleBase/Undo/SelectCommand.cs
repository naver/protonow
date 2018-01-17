using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Common.CommonBase;

namespace Naver.Compass.InfoStructure
{
    public class SelectCommand : IUndoableCommand
    {
        public SelectCommand(IGroupOperation pageVM, WidgetViewModBase selectedWidget)
        {
            List<WidgetViewModBase> selectedWidgetList = new List<WidgetViewModBase>();
            selectedWidgetList.Add(selectedWidget);
            CheckGroupHelper(selectedWidgetList);
            _pageVM = pageVM;
        }

        public SelectCommand(IGroupOperation pageVM, List<WidgetViewModBase> selectedWidgetList)
        {
            CheckGroupHelper(selectedWidgetList);
            _pageVM = pageVM;
        }

        public SelectCommand(IGroupOperation pageVM, List<IWidgetPropertyData> selectedWidgetList)
        {
            CheckGroupHelper(selectedWidgetList);
            _pageVM = pageVM;
        }

        public void Undo()
        {
            Select();
        }

        public void Redo()
        {
            Select();
        }

        private void Select()
        {
            if (_pageVM == null)
            {
                return;
            }

            foreach (WidgetViewModBase item in _widgetList)
            {
                if (_targetWidgetID != null && item.WidgetID == _targetWidgetID)
                {
                    item.IsTarget = true;
                }
                item.IsSelected = true;
            }

            foreach(Guid guid in _groupGuids)
            {
                _pageVM.SetGroupStatus(guid, GroupStatus.Selected);
            }
        }

        private void CheckGroupHelper(List<WidgetViewModBase> widgetList)
        {
            foreach(WidgetViewModBase item in widgetList)
            {
                AddToList(item);
            }
        }

        private void CheckGroupHelper(List<IWidgetPropertyData> widgetList)
        {
            foreach (WidgetViewModBase item in widgetList)
            {
                if(item.IsTarget)
                {
                    _targetWidgetID = item.WidgetID;
                }
                AddToList(item);
            }
        }

        private void AddToList(WidgetViewModBase item)
        {
            if (item != null)
            {
                if (item.IsGroup)
                {
                    _groupGuids.Add(item.WidgetID);
                }
                else
                {
                    _widgetList.Add(item);
                }
            }
        }

        List<WidgetViewModBase> _widgetList = new List<WidgetViewModBase>();
        Guid _targetWidgetID;
        IGroupOperation _pageVM;
        List<Guid> _groupGuids = new List<Guid>();
    }
}
